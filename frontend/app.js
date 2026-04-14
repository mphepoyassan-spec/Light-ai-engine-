document.addEventListener('DOMContentLoaded', () => {
    const chatForm = document.getElementById('chat-form');
    const userInput = document.getElementById('user-input');
    const chatMessages = document.getElementById('chat-messages');
    const typingIndicator = document.getElementById('typing-indicator');
    const sessionId = 'session-' + Math.random().toString(36).substr(2, 9);

    const API_URL = 'http://localhost:8000';

    const addMessage = (content, role) => {
        const messageDiv = document.createElement('div');
        messageDiv.classList.add('message', role);

        const bubble = document.createElement('div');
        bubble.classList.add('bubble');
        bubble.textContent = content;

        messageDiv.appendChild(bubble);
        chatMessages.appendChild(messageDiv);

        // Scroll to bottom
        chatMessages.scrollTop = chatMessages.scrollHeight;
    };

    const showTyping = () => {
        typingIndicator.classList.remove('hidden');
        chatMessages.scrollTop = chatMessages.scrollHeight;
    };

    const hideTyping = () => {
        typingIndicator.classList.add('hidden');
    };

    const addStreamingMessage = () => {
        const messageDiv = document.createElement('div');
        messageDiv.classList.add('message', 'assistant');
        const bubble = document.createElement('div');
        bubble.classList.add('bubble');
        messageDiv.appendChild(bubble);
        chatMessages.appendChild(messageDiv);
        return bubble;
    };

    chatForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const message = userInput.value.trim();

        if (!message) return;

        addMessage(message, 'user');
        userInput.value = '';
        showTyping();

        try {
            const response = await fetch(`${API_URL}/chat/stream`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ message: message, session_id: sessionId }),
            });

            if (!response.ok) throw new Error('Stream request failed');

            hideTyping();
            const bubble = addStreamingMessage();
            const reader = response.body.getReader();
            const decoder = new TextDecoder();

            while (true) {
                const { value, done } = await reader.read();
                if (done) break;

                const chunk = decoder.decode(value);
                const lines = chunk.split('\n');

                for (const line of lines) {
                    if (line.startsWith('data: ')) {
                        const dataStr = line.slice(6).trim();
                        if (dataStr === '[DONE]') break;

                        try {
                            const data = JSON.parse(dataStr);
                            if (data.text) {
                                bubble.textContent += data.text;
                                chatMessages.scrollTop = chatMessages.scrollHeight;
                            }
                        } catch (e) {
                            console.error('Error parsing stream chunk', e);
                        }
                    }
                }
            }
        } catch (error) {
            hideTyping();
            addMessage('Could not connect to the local AI engine.', 'assistant');
            console.error('Error:', error);
        }
    });

    // Initial check to see if backend is online
    fetch(`${API_URL}/`)
        .then(res => res.json())
        .then(data => {
            if (data.status === 'online') {
                console.log('Backend is online');
                document.getElementById('status-dot').classList.add('online');
            }
        })
        .catch(err => {
            console.error('Backend is offline');
            document.getElementById('status-dot').classList.remove('online');
            document.getElementById('status-text').textContent = 'Local Engine Offline';
        });
});
