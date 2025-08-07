
	// Dữ liệu câu hỏi từ Model
	const question = @Html.Raw(Json.Serialize(Model.ToList()));
	console.log(question);
	let currentIndex = 0;

	function renderQuestion(index) {
		// Kiểm tra dữ liệu câu hỏi
		const q = question[index];
	if (!q || !q.content || !q.answers || !Array.isArray(q.answers)) {
		document.getElementById("questionContainer").innerHTML = "<p>Không có câu hỏi nào hoặc dữ liệu không hợp lệ.</p>";
	return;
		}

	// Tạo HTML cho câu hỏi và đáp án
	let html = `
	<p class="question-content fw-bold mb-3" id="questionContent">
		${q.content}
	</p>
	<div class="row">
		`;
		q.answers.forEach(answer => {
			if (answer && answer.id && answer.content) { // Kiểm tra dữ liệu đáp án
			html += `
					<div class="col-6 mb-1">
						<input type="checkbox" name="answer" value="${answer.id}">
						<span>${answer.answerContent}</span>
					</div>
				`;
			}
		});
		html += `</div>`;

	// Cập nhật DOM
	const questionContainer = document.getElementById("questionContainer");
	const checkAnswerBtn = document.getElementById("checkAnswerBtn");
	const nextQuestionBtn = document.getElementById("nextQuestion");
	const explanation = document.getElementById("explanation");

	// Kiểm tra sự tồn tại của các phần tử DOM
	if (!questionContainer || !checkAnswerBtn || !nextQuestionBtn || !explanation) {
		console.error("Một hoặc nhiều phần tử DOM không tồn tại.");
	return;
		}

	questionContainer.innerHTML = html;
	questionContainer.dataset.questionId = q.id; // Sử dụng q.id (chữ thường)
	checkAnswerBtn.disabled = true; // Vô hiệu hóa nút kiểm tra
	nextQuestionBtn.style.display = "none"; // Sửa lỗi từ styled sang style
	explanation.innerHTML = ""; // Xóa phần giải thích
	}

	// Gọi hàm renderQuestion khi trang tải
	document.addEventListener("DOMContentLoaded", () => {
		if (!document.getElementById("questionContainer")) {
		console.error("questionContainer không tồn tại trên trang.");
	return;
		}
	renderQuestion(currentIndex);
	});


	document.getElementById("QuizForm").addEventListener("submit", async function (e) {
		e.preventDefault(); // ❌ Ngăn submit truyền thống (full page reload)

	const questionId = document.getElementById("question").dataset.questionId;

	const selectAnswer = Array.from(document.querySelectorAll("input[name='answer']:checked"))
			.map(cb => parseInt(cb.value));

	if (selectAnswer.length != 0) {
		document.getElementById("checkAnswerBtn").disabled = false;
		}

	const response = await fetch('/YKhoa/submitanswers', {
		method: 'POST',
	headers: {'Content-Type': 'application/json' },
	body: JSON.stringify({questionId: parseInt(questionId), selectAnswer })
		});

	const data = await response.json();
	const explanationBox = document.getElementById('explanation');

	if (!data.isCorrect) {
		explanationBox.innerHTML = `
            ${data.wrongAnswers.length > 0 ? `
            <strong>Đáp án sai bạn đã chọn:</strong>
            <ul>
                ${data.wrongAnswers.map(a => `<li style ="color:red"><strong>${a.content}</strong>: ${a.explanation}</li>`).join('')}
            </ul>` : ''}

            ${data.missedAnswers.length > 0 ? `
            <strong>Đáp án đúng bạn đã bỏ sót:</strong>
            <ul>${data.missedAnswers.map(a => `<li>${a}</li>`).join('')}</ul>` : ''}

            <strong>Đáp án đúng bạn chọn là:</strong>
            <ul>${data.correctAnswers.map(a => `<li>${a}</li>`).join('')}</ul>
        `;
		} else {
		explanationBox.innerHTML = `<span style="color: green;"><strong>Chính xác!</strong></span>`;
		}

	// Optional: Vô hiệu hóa nút trả lời sau khi đã trả lời
	document.getElementById("checkAnswerBtn").disabled = true;
	});
	document.getElementById("nextQuestion").addEventListener("click", () => {
		currentIndex++;
	renderQuestion(currentIndex);
	});
