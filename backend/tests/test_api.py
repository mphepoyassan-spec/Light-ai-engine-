import pytest
from fastapi.testclient import TestClient
from backend.app import app

client = TestClient(app)

def test_root():
    response = client.get("/")
    assert response.status_code == 200
    assert response.json()["status"] == "online"

def test_chat_endpoint():
    response = client.post("/chat", json={"message": "Hello", "session_id": "test"})
    assert response.status_code == 200
    assert "response" in response.json()
    assert "[Mock Mode]" in response.json()["response"]

def test_clear_endpoint():
    # Add something to chat first
    client.post("/chat", json={"message": "Keep me", "session_id": "test_clear"})
    response = client.post("/clear", json={"session_id": "test_clear", "message": ""})
    assert response.status_code == 200
    assert response.json()["message"] == "History cleared"

def test_predict_endpoint():
    response = client.post("/predict", json={"text": "Hello"})
    assert response.status_code == 200
    assert "prediction" in response.json()
