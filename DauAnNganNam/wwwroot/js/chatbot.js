function toggleChat() {
    const body = document.getElementById("chat-body");
    body.style.display = body.style.display === "none" ? "block" : "none";
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
        playAnswerAudio(data.answer);

    } catch (error) {
        console.error('Error:', error);
        appendMessage("bot", '❌ Đã có lỗi xảy ra khi kết nối tới server.');
    }

    async function playAnswerAudio(answerText) {

        try {
            const response = await fetch('/Ais/TextToSpeech', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({ text: answerText })
            });

            if (!response.ok) throw new Error('Không thể phát âm thanh.');

            const audioBlob = await response.blob();
            const audioUrl = URL.createObjectURL(audioBlob);
            const audio = new Audio(audioUrl);
            audio.play();
        } catch (error) {
            //alert("Đã xảy ra lỗi khi phát âm thanh.");
        }
    }
}
