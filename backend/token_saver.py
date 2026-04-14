import re

class TokenSaver:
    def __init__(self, max_context_messages=5):
        self.max_context_messages = max_context_messages
        self.filler_words = ["uh", "um", "er", "ah", "like", "you know", "basically", "actually"]

    def clean_text(self, text):
        # Remove common filler words (case insensitive)
        for word in self.filler_words:
            pattern = re.compile(rf'\s*\b{word}\b\s*', re.IGNORECASE)
            text = pattern.sub(' ', text)

        # Remove extra spaces
        text = " ".join(text.split())

        # Remove redundant punctuation
        text = re.sub(r'([!?.,]){2,}', r'\1', text)

        # Remove leading/trailing punctuation that might be left after filler word removal
        text = re.sub(r'^\W+', '', text)

        return text.strip()

    def trim_context(self, messages):
        """Keep only last N messages"""
        if len(messages) > self.max_context_messages:
            return messages[-self.max_context_messages:]
        return messages

    def optimize_prompt(self, messages):
        """Convert list of messages into a structured short prompt"""
        prompt = ""
        for msg in messages:
            role = msg.get("role", "user")
            content = self.clean_text(msg.get("content", ""))
            if role == "user":
                prompt += f"User: {content}\n"
            else:
                prompt += f"AI: {content}\n"
        prompt += "AI:"
        return prompt
