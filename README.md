# LightAI Engine

A lightweight, production-ready local AI system with a FastAPI backend, ONNX Runtime inference engine, and a modern web-based chat UI.

## 🚀 Features
- **FastAPI Backend**: High-performance asynchronous API.
- **ONNX Runtime**: Optimized for CPU inference.
- **Token Saver System**:
  - Input cleaning (removes filler words and extra spaces).
  - Context trimming (keeps only the last 5 messages).
  - Prompt optimization (converts chat history to structured prompts).
- **Memory & Caching**:
  - Short-term conversation memory.
  - Response caching for repeated queries.
- **Modern UI**: Clean, responsive chat interface with dark mode and typing indicators.
- **Offline Ready**: Runs fully on localhost after initial setup.

## 📁 Project Structure
```
/lightai-engine
  ├── backend/
  │    ├── app.py                  # Main FastAPI application
  │    ├── model_loader.py          # ONNX model loading & inference
  │    ├── error_handler.py         # Global exception handling
  │    ├── token_saver.py           # Token optimization logic
  │    ├── cache.py                 # Response caching system
  │    ├── conversation_memory.py   # Short-term chat history
  │    └── model.onnx               # (User provided) ONNX model file
  ├── frontend/
  │    ├── index.html               # Chat UI
  │    ├── style.css                # Modern styling
  │    └── app.js                   # Frontend logic & API integration
  ├── requirements.txt              # Project dependencies
  └── README.md                     # Documentation
```

## 🛠️ Setup Instructions

### 1. Install Dependencies
Ensure you have Python 3.8+ installed.
```bash
pip install -r requirements.txt
```

### 2. Add an ONNX Model
Place your ONNX model file at `backend/model.onnx`.
If no model is found, the system will run in **Mock Mode** for demonstration purposes.

*Note: For best results, use a quantized LLM like Phi-3 or Llama-3 exported to ONNX format.*

### 3. Run the Backend
```bash
python3 -m backend.app
```
The server will start at `http://localhost:8000`.

### 4. Open the Frontend
Simply open `frontend/index.html` in any modern web browser.

## 📊 API Usage

### Chat Endpoint (`/chat`)
Main endpoint for the chat UI. Handles memory, token optimization, and caching.
```bash
curl -X POST http://localhost:8000/chat \
     -H "Content-Type: application/json" \
     -d '{"message": "Hello, how are you?", "session_id": "user-123"}'
```

### Raw Prediction Endpoint (`/predict`)
Direct access to the inference engine.
```bash
curl -X POST http://localhost:8000/predict \
     -H "Content-Type: application/json" \
     -d '{"text": "Once upon a time"}'
```

## 🧠 How Token Saver Works
The `TokenSaver` system ensures high efficiency by:
1. **Cleaning**: Removing filler words like "um", "ah", and "actually".
2. **Trimming**: Keeping only the most relevant recent context to stay within token limits.
3. **Structuring**: Formatting the history into a compact `User: ... \n AI: ...` prompt for the model.

## 🐳 Docker Support (Optional)
To run in a containerized environment:
```bash
docker build -t lightai-engine .
docker run -p 8000:8000 lightai-engine
```
*(Note: Create a simple Dockerfile if needed)*
