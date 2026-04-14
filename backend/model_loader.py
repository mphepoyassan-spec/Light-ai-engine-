import onnxruntime as ort
from transformers import AutoTokenizer
import numpy as np
import os
import requests

class ModelLoader:
    def __init__(self, model_path="backend/model.onnx", tokenizer_name="gpt2"):
        self.model_path = model_path
        self.tokenizer_name = tokenizer_name
        self.tokenizer = None
        self.session = None
        self.is_ready = False

    def download_demo_model(self):
        """Placeholder for downloading a tiny ONNX model if not exists"""
        if not os.path.exists(self.model_path):
            print(f"Model not found at {self.model_path}. Please provide an ONNX model.")
            # In a real environment, we might download one here.
            # For this task, we assume the user will provide it or we use a mock if it fails.
            return False
        return True

    def load(self):
        try:
            self.tokenizer = AutoTokenizer.from_pretrained(self.tokenizer_name)
            if os.path.exists(self.model_path):
                self.session = ort.InferenceSession(self.model_path)
                self.is_ready = True
                print(f"Model loaded successfully from {self.model_path}")
            else:
                print(f"Warning: Model file {self.model_path} not found. Running in mock mode.")
        except Exception as e:
            print(f"Error loading model: {e}")

    def predict(self, text, max_length=50):
        if not self.is_ready:
            return f"[Mock Response] You said: {text}"

        inputs = self.tokenizer(text, return_tensors="np")
        input_ids = inputs["input_ids"].astype(np.int64)

        # This is a very simplified inference loop for demonstration.
        # Real LLM inference with ONNX requires handling KV caches and autoregressive generation.
        # For a truly production-ready system, onnxruntime-genai is recommended.

        # Simple one-shot prediction for demo (if model supports it)
        ort_inputs = {self.session.get_inputs()[0].name: input_ids}
        ort_outs = self.session.run(None, ort_inputs)

        # Decode the first output
        output_ids = np.argmax(ort_outs[0], axis=-1)
        response = self.tokenizer.decode(output_ids[0], skip_special_tokens=True)
        return response
