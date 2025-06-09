function toggleChat() {
    const body = document.getElementById("chat-body");
    const widget = document.getElementById("chatbot-widget");
    const toggle = document.getElementById("chat-toggle");
    const chatContent  = document.getElementById("chat-messages");

    if (body.style.display === "none") {
        body.style.display = "block";
        widget.classList.remove("closed");
        toggle.classList.remove("closed");
        if (chatContent.children.length === 0) {
            appendMessage("bot", "Xin chào bạn, mình là trợ lý ảo Kim Ấn rất vui khi có thể giải đáp câu hỏi của bạn về chủ đề lịch sử Việt Nam!");
        }
        toggle.classList.add("open");
    } else {
        body.style.display = "none";
        widget.classList.add("closed");
        toggle.classList.remove("open");
        toggle.classList.add("closed");
    }
}

function handleKey(event) {
    if (event.key === "Enter") {
        const input = document.getElementById("chat-input");
        const message = input.value.trim();
        if (message) {
            appendMessage("user", message);
            input.value = "";
            askGeminiFromChat(message);
        }
    }
}

function appendMessage(sender, text) {
    const container = document.getElementById("chat-messages");
    const msg = document.createElement("div");
    msg.className = `chat-bubble ${sender}`;
    msg.innerText = text;
    container.appendChild(msg);
    container.scrollTop = container.scrollHeight;
}

async function askGeminiFromChat(question) {
    appendMessage("bot", "⏳ Đang xử lý...");
    try {
        const response = await fetch('/Ais/AskGemini', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({ question })
        });

        // Xóa tin nhắn "Đang xử lý..."
        const loadingMsg = document.querySelector("#chat-messages .bot:last-child");
        if (loadingMsg && loadingMsg.innerText.includes("Đang xử lý")) {
            loadingMsg.remove();
        }

        if (!response.ok) {
            const errorData = await response.json();
            appendMessage("bot", `❌ Lỗi: ${errorData.error || response.statusText}`);
            return;
        }

        const data = await response.json();
        appendMessage("bot", data.answer);

    } catch (error) {
        console.error('Error:', error);
        appendMessage("bot", '❌ Đã có lỗi xảy ra khi kết nối tới server.');
    }
}
