from fastapi import FastAPI, Request, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from fastapi.staticfiles import StaticFiles
from fastapi.responses import StreamingResponse
from pydantic import BaseModel
from typing import List, Optional, AsyncGenerator
import uvicorn
import os
import asyncio
import json

from .model_loader import ModelLoader
from .token_saver import TokenSaver
from .cache import ResponseCache
from .conversation_memory import ConversationMemory
from .error_handler import setup_error_handlers

app = FastAPI(title="LightAI Engine")

# CORS for frontend access
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

setup_error_handlers(app)

# Serve frontend
frontend_path = os.path.join(os.path.dirname(os.path.dirname(__file__)), "frontend")
if os.path.exists(frontend_path):
    app.mount("/frontend", StaticFiles(directory=frontend_path), name="frontend")

# Initialize components
model_loader = ModelLoader()
token_saver = TokenSaver()
cache = ResponseCache()
memory = ConversationMemory()

# On startup, load the model
@app.on_event("startup")
async def startup_event():
    model_loader.load()

class ChatMessage(BaseModel):
    role: str
    content: str

class ChatRequest(BaseModel):
    message: str
    session_id: str = "default"

class PredictRequest(BaseModel):
    text: str

@app.get("/")
async def root():
    return {"status": "online", "engine": "LightAI"}

@app.post("/chat")
async def chat(request: ChatRequest):
    # 1. Check Cache
    cached_response = cache.get(request.message)
    if cached_response:
        return {"response": cached_response, "cached": True}

    # 2. Get Memory
    history = memory.get_history(request.session_id)

    # 3. Add User Message
    memory.add_message(request.session_id, "user", request.message)

    # 4. Token Optimization
    trimmed_history = token_saver.trim_context(memory.get_history(request.session_id))
    prompt = token_saver.optimize_prompt(trimmed_history)

    # 5. Inference
    try:
        response = model_loader.predict(prompt)
        # Clean response if necessary
        response = response.strip()
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Inference error: {str(e)}")

    # 6. Add AI Response to Memory
    memory.add_message(request.session_id, "assistant", response)

    # 7. Update Cache
    cache.set(request.message, response)

    return {"response": response, "cached": False}

async def stream_generator(prompt: str, session_id: str, original_message: str):
    # This is a mock streaming generator.
    # In a real ONNX setup, you'd yield tokens one by one.
    try:
        response = model_loader.predict(prompt)
        words = response.split()
        full_response = ""
        for word in words:
            full_response += word + " "
            yield f"data: {json.dumps({'text': word + ' '})}\n\n"
            await asyncio.sleep(0.05) # Simulate typing speed

        # After stream ends, save to memory and cache
        memory.add_message(session_id, "assistant", full_response.strip())
        cache.set(original_message, full_response.strip())
        yield "data: [DONE]\n\n"
    except Exception as e:
        yield f"data: {json.dumps({'error': True, 'message': str(e)})}\n\n"

@app.post("/chat/stream")
async def chat_stream(request: ChatRequest):
    # 1. Check Cache (Same as /chat)
    cached_response = cache.get(request.message)
    if cached_response:
        async def cached_gen():
            yield f"data: {json.dumps({'text': cached_response, 'cached': True})}\n\n"
            yield "data: [DONE]\n\n"
        return StreamingResponse(cached_gen(), media_type="text/event-stream")

    # 2. Prepare Context
    memory.add_message(request.session_id, "user", request.message)
    trimmed_history = token_saver.trim_context(memory.get_history(request.session_id))
    prompt = token_saver.optimize_prompt(trimmed_history)

    return StreamingResponse(
        stream_generator(prompt, request.session_id, request.message),
        media_type="text/event-stream"
    )

@app.post("/predict")
async def predict(request: PredictRequest):
    try:
        response = model_loader.predict(request.text)
        return {"prediction": response}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)
