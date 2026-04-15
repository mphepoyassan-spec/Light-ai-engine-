# LightAI Engine

A lightweight, production-ready local AI system with an ASP.NET Core backend, ONNX Runtime inference engine, and a modern web-based chat UI.

## 🚀 Features
- **ASP.NET Core Backend**: High-performance C# API.
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

## 🛠️ Setup Instructions

### 1. Install .NET SDK
Ensure you have the .NET 8.0+ SDK installed.

### 2. Run the Backend
```bash
cd backend_csharp
dotnet run --urls "http://0.0.0.0:8000"
```
The server will start at `http://localhost:8000`.

### 3. Open the Frontend
Simply open `frontend/index.html` in any modern web browser or navigate to `http://localhost:8000/frontend/index.html`.

## 📊 API Usage

### Chat Endpoint (`POST /chat`)
Main endpoint for the chat UI.
```bash
curl -X POST http://localhost:8000/chat \
     -H "Content-Type: application/json" \
     -d '{"message": "Hello, how are you?", "session_id": "user-123"}'
```

### Streaming Endpoint (`POST /chat/stream`)
SSE-based streaming responses.

### Clear History (`POST /clear`)
Reset conversation for a session.

### Raw Prediction Endpoint (`POST /predict`)
Direct access to the inference engine.
