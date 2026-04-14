class ConversationMemory:
    def __init__(self):
        self.conversations = {}

    def get_history(self, session_id):
        return self.conversations.get(session_id, [])

    def add_message(self, session_id, role, content):
        if session_id not in self.conversations:
            self.conversations[session_id] = []
        self.conversations[session_id].append({"role": role, "content": content})

    def clear_history(self, session_id):
        if session_id in self.conversations:
            del self.conversations[session_id]

    def summarize_old_messages(self, messages, keep_last=3):
        """
        In a real scenario, this might use another LLM call or a rule-based approach
        to summarize. Here we just provide a placeholder that could be expanded.
        """
        if len(messages) <= keep_last:
            return messages

        # Simplified 'summarization' by just keeping the most recent
        return messages[-keep_last:]
