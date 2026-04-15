document.addEventListener('DOMContentLoaded', () => {
    const chatForm = document.getElementById('chat-form');
    const userInput = document.getElementById('user-input');
    const chatMessages = document.getElementById('chat-messages');
    const typingIndicator = document.getElementById('typing-indicator');
    const clearBtn = document.getElementById('clear-btn');
    const statusDot = document.getElementById('status-dot');
    const statusText = document.getElementById('status-text');

    const sessionId = 'session-' + Math.random().toString(36).substr(2, 9);

    const API_URL = window.location.origin.includes('localhost') || window.location.origin.includes('127.0.0.1')
                    ? window.location.origin
                    : 'http://localhost:8000';

    const addMessage = (content, role) => {
        const messageDiv = document.createElement('div');
        messageDiv.classList.add('message', role);

        const bubble = document.createElement('div');
        bubble.classList.add('bubble');
        bubble.textContent = content;

        messageDiv.appendChild(bubble);
        chatMessages.appendChild(messageDiv);

        chatMessages.scrollTop = chatMessages.scrollHeight;
        return bubble;
    };

    const showTyping = () => {
        typingIndicator.classList.remove('hidden');
        chatMessages.scrollTop = chatMessages.scrollHeight;
    };

    const hideTyping = () => {
        typingIndicator.classList.add('hidden');
    };

    chatForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const message = userInput.value.trim();

        if (!message) return;

        addMessage(message, 'user');
        userInput.value = '';

        showTyping();

        try {
            // Using streaming endpoint
            const response = await fetch(`${API_URL}/chat/stream`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    message: message,
                    session_id: sessionId
                }),
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Server error');
            }

            hideTyping();
            const aiBubble = addMessage('', 'assistant');

            const reader = response.body.getReader();
            const decoder = new TextDecoder();

            while (true) {
                const { value, done } = await reader.read();
                if (done) break;

                const chunkStr = decoder.decode(value);
                const lines = chunkStr.split('\n');

                for (const line of lines) {
                    if (line.startsWith('data: ')) {
                        try {
                            const data = JSON.parse(line.substring(6));
                            if (data.error) {
                                aiBubble.textContent = `Error: ${data.error}`;
                            } else if (data.chunk) {
                                aiBubble.textContent += data.chunk;
                                chatMessages.scrollTop = chatMessages.scrollHeight;
                            }
                        } catch (e) {
                            console.error('Error parsing SSE:', e);
                        }
                    }
                }
            }
        } catch (error) {
            hideTyping();
            addMessage(`Error: ${error.message}. Make sure the backend is running.`, 'assistant');
            console.error('Error:', error);
        }
    });

    clearBtn.addEventListener('click', async () => {
        if (confirm('Clear chat history?')) {
            try {
                await fetch(`${API_URL}/clear`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ session_id: sessionId, message: '' })
                });
                chatMessages.innerHTML = '';
                addMessage("Chat history cleared. How can I help you now?", 'assistant');
            } catch (error) {
                console.error('Error clearing history:', error);
            }
        }
    });

    const checkStatus = () => {
        fetch(`${API_URL}/`)
            .then(res => res.json())
            .then(data => {
                if (data.status === 'online') {
                    statusDot.className = 'online';
                    statusText.textContent = `Local Engine Online (${data.lang})`;
                }
            })
            .catch(err => {
                statusDot.className = '';
                statusText.textContent = 'Local Engine Offline';
            });
    };

    checkStatus();
    setInterval(checkStatus, 10000);
});
