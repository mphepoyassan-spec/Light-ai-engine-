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

    chatForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const message = userInput.value.trim();

        if (!message) return;

        // Add user message to UI
        addMessage(message, 'user');
        userInput.value = '';

        // Show typing indicator
        showTyping();

        try {
            const response = await fetch(`${API_URL}/chat`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    message: message,
                    session_id: sessionId
                }),
            });

            const data = await response.json();

            // Hide typing indicator
            hideTyping();

            if (data.error) {
                addMessage(`Error: ${data.message}`, 'assistant');
            } else {
                addMessage(data.response, 'assistant');
            }
        } catch (error) {
            hideTyping();
            addMessage('Could not connect to the local AI engine. Make sure the backend is running.', 'assistant');
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
