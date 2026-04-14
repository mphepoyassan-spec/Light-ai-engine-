from fastapi import FastAPI, Request, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from fastapi.staticfiles import StaticFiles
from fastapi.responses import StreamingResponse
from pydantic import BaseModel
from typing import List, Optional
from contextlib import asynccontextmanager
import uvicorn
import os
import json
import asyncio

from .model_loader import ModelLoader
from .token_saver import TokenSaver
from .cache import ResponseCache
from .conversation_memory import ConversationMemory
from .error_handler import setup_error_handlers

# Initialize components
model_loader = ModelLoader()
token_saver = TokenSaver()
cache = ResponseCache()
memory = ConversationMemory()

@asynccontextmanager
async def lifespan(app: FastAPI):
    # On startup, load the model
    model_loader.load()
    yield
    # On shutdown, we could add cleanup here if needed

app = FastAPI(title="LightAI Engine", lifespan=lifespan)

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

@app.post("/chat/stream")
async def chat_stream(request: ChatRequest):
    # 1. Add User Message
    memory.add_message(request.session_id, "user", request.message)

    # 2. Token Optimization
    trimmed_history = token_saver.trim_context(memory.get_history(request.session_id))
    prompt = token_saver.optimize_prompt(trimmed_history)

    # 3. Stream Inference (Mock implementation for streaming)
    async def event_generator():
        try:
            full_response = model_loader.predict(prompt)
            # Simulate streaming by splitting response into words
            words = full_response.split()
            for i, word in enumerate(words):
                chunk = word + (" " if i < len(words) - 1 else "")
                yield f"data: {json.dumps({'chunk': chunk, 'done': False})}\n\n"
                await asyncio.sleep(0.05)

            # Save complete response to memory
            memory.add_message(request.session_id, "assistant", full_response)
            yield f"data: {json.dumps({'chunk': '', 'done': True})}\n\n"
        except Exception as e:
            yield f"data: {json.dumps({'error': str(e)})}\n\n"

    return StreamingResponse(event_generator(), media_type="text/event-stream")

@app.post("/clear")
async def clear_history(request: ChatRequest):
    memory.clear_history(request.session_id)
    return {"message": "History cleared", "session_id": request.session_id}

@app.post("/predict")
async def predict(request: PredictRequest):
    try:
        response = model_loader.predict(request.text)
        return {"prediction": response}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)
