// Question management

let totalQuestions = 0;
let orderedQuestions = parseInt(localStorage.getItem('orderQuestion')) || 0;
let userAnswers = {};
let resultId = parseInt(localStorage.getItem('quizResultId')) || null;

// Initialize quiz
async function initializeQuiz() {
	try {
		var idQuiz = document.body.dataset.quizId;
		//const response = await fetch(`/Ykhoa/StartQuiz?idQuiz=${idQuiz}`, { method: 'POST' });
		if (!resultId) {
			const response = await fetch(`/Ykhoa/StartQuiz?idQuiz=${idQuiz}`, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
			});
			if (!response.ok) throw new Error('Failed to start quiz');
			const data = await response.json();
			resultId = data.id;
			totalQuestions = data.totalQuestions;
			orderedQuestions = data.orderQuestion;

			localStorage.setItem('orderQuestion', orderedQuestions);
		} else {
			// Validate result and restore state
			const response = await fetch('/YKhoa/GetResults', {
				headers: { 'X-Result-ID': resultId }
			});
			if (!response.ok) {
				localStorage.removeItem('quizResultId');
				resultId = null;
				return initializeQuiz();
			}
			const data = await response.json();
			totalQuestions = data.totalQuestions;
			orderedQuestions = data.orderQuestion || orderedQuestions;

			localStorage.setItem('orderQuestion', orderedQuestions);

			// Fetch answers for current result
			const answersResponse = await fetch('/Ykhoa/answers', {
				headers: { 'X-Result-ID': resultId }
			});

			if (answersResponse.ok) {
				const answers = await answersResponse.json();
				userAnswers = answers.reduce((acc, answer) => {
					acc[answer.dislayOrder] = {
						selectedOption: answer.selectedOption,
						isCorrect: answer.isCorrect,
						explanation: answer.explanation,
						correctOption: answer.correctOption
					};
					return acc;
				}, {});
			}
		}
		updateNavigationButtons();
		await updateQuestion(orderedQuestions);
	} catch (error) {
		showNotification('Không thể khởi tạo bài kiểm tra!', 'warning');
		console.error(error);
	}
}

// Fetch question from backend
async function updateQuestion(dislayOrder) {
	try {
		const response = await fetch(`/YKhoa/GetQuestion?orderQuestion=${dislayOrder}`, {
			headers: { 'X-Result-ID': resultId }
		});
		if (!response.ok) throw new Error('Invalid response');
		const data = await response.json();

		document.querySelector('.question-title').textContent = data.title;
		document.querySelector('.question-text').textContent = data.questionText;

		const options = document.querySelectorAll('.answer-option');
		options.forEach((option, index) => {
			if (data.options[index]) {
				option.style.display = 'flex';
				option.querySelector('div:last-child').textContent = data.options[index].text;
				option.dataset.answerId = data.options[index].id; // Lưu AnswerId
				option.classList.remove('selected', 'correct', 'incorrect', 'answered');
				option.style.pointerEvents = 'auto';
			} else {
				option.style.display = 'none';
			}
		});

		const explanation = document.querySelector('.explanation');
		explanation.style.display = 'none';
		explanation.innerHTML = '';

		const questionSection = document.querySelector('.question-section');
		questionSection.classList.remove('answered');
		if (userAnswers[dislayOrder]) {
			const savedOption = Array.from(options).find(opt => parseInt(opt.dataset.answerId) === userAnswers[dislayOrder].selectedOption);
			if (savedOption) {
				savedOption.classList.add('selected');
				if (userAnswers[dislayOrder].isCorrect) {
					savedOption.classList.add('correct');
				} else {
					savedOption.classList.add('incorrect');
					if (userAnswers[dislayOrder].correctOption !== null) {
						const correctOption = Array.from(options).find(opt => parseInt(opt.dataset.answerId) === userAnswers[dislayOrder].correctOption);
						if (correctOption) correctOption.classList.add('correct');
					}
				}
				questionSection.classList.add('answered');
				explanation.innerHTML = `<p><strong>Giải thích:</strong> ${userAnswers[dislayOrder].explanation || ''}</p>`;
				explanation.style.display = 'block';
				options.forEach(opt => {
					opt.style.pointerEvents = 'none';
					opt.classList.add('answered');
				});
			}
		}
		//currentQuestion = questionNumber;
		//localStorage.setItem('currentQuestion', currentQuestion);
		orderedQuestions = dislayOrder;
		localStorage.setItem('orderQuestion', orderedQuestions);
	} catch (error) {
		showNotification('Không thể tải câu hỏi!', 'warning');
		console.log(error);
	}
}

// Handle answer selection
async function selectAnswer(element) {
	const questionSection = element.closest('.question-section');
	if (questionSection.classList.contains('answered')) {
		return;
	}

	const options = questionSection.querySelectorAll('.answer-option');
	options.forEach(opt => opt.classList.remove('selected', 'correct', 'incorrect'));
	element.classList.add('selected');

	const selectedAnswerId = parseInt(element.dataset.answerId);

	try {
		const response = await fetch(`/YKhoa/SubmitAnswer?orderQuestion=${orderedQuestions}`, {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				'X-Result-ID': resultId,
			},
			body: JSON.stringify({ selectedOption: selectedAnswerId })
		});
		if (!response.ok) throw new Error('Invalid response');
		const result = await response.json();



		userAnswers[orderedQuestions] = {
			selectedOption: selectedAnswerId,
			isCorrect: result.isCorrect,
			explanation: result.explanation,
			correctOption: result.correctOption
		};

		if (result.isCorrect) {
			element.classList.add('correct');
			showNotification('Đáp án đúng!', 'success');
		} else {
			element.classList.add('incorrect');
			if (result.correctOption !== null) {
				const correctOption = Array.from(options).find(opt => parseInt(opt.dataset.answerId) === result.correctOption);
				if (correctOption) correctOption.classList.add('correct');
			}
			showNotification('Đáp án sai!', 'warning');
		}

		questionSection.classList.add('answered');
		options.forEach(opt => {
			opt.style.pointerEvents = 'none';
			opt.classList.add('answered');
		});

		const explanation = questionSection.querySelector('.explanation');
		console.log(result.explanation);
		explanation.innerHTML = `<p><strong>Giải thích:</strong> ${result.explanation || ''}</p>`;
		explanation.style.display = 'block';

		element.style.transform = 'scale(0.98)';
		setTimeout(() => {
			element.style.transform = 'translateY(-2px)';
		}, 100);
	} catch (error) {
		showNotification('Lỗi khi kiểm tra đáp án!', 'warning');
		console.error(error);
	}
}

// Navigate to next question
async function nextQuestion() {
	if (userAnswers[orderedQuestions] === undefined) {
		showNotification('Vui lòng chọn một đáp án trước khi tiếp tục!', 'warning');
		return;
	}

	const button = document.getElementById('nextBtn');
	const originalText = button.innerHTML;
	button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang tải...';
	button.disabled = true;

	try {
		await new Promise(resolve => setTimeout(resolve, 1000));
		button.innerHTML = originalText;
		button.disabled = false;

		if (orderedQuestions < totalQuestions) {
			orderedQuestions++;
			await updateQuestion(orderedQuestions);
			updateNavigationButtons();
			document.querySelector('.question-section').scrollIntoView({ behavior: 'smooth' });
		} else {
			showNotification('Bạn đã hoàn thành bài trắc nghiệm!', 'success');
			await showResults();
			localStorage.removeItem('quizResultId');
			//localStorage.removeItem('currentQuestion');
			localStorage.removeItem('orderQuestion');
		}
	} catch (error) {
		button.innerHTML = originalText;
		button.disabled = false;
		showNotification('Lỗi khi chuyển câu hỏi!', 'warning');
		console.error(error);
	}
}

// Navigate to previous question
async function previousQuestion() {
	if (orderedQuestions > 1) {
		orderedQuestions--;
		await updateQuestion(orderedQuestions);
		updateNavigationButtons();
		document.querySelector('.question-section').scrollIntoView({ behavior: 'smooth' });
	}
}

// Show results
async function showResults() {
	try {
		const response = await fetch('/Ykhoa/results', {
			headers: { 'X-Result-ID': resultId }
		});
		if (!response.ok) throw new Error('Invalid response');
		const data = await response.json();

		const resultMessage = `
			<div class="text-center">
				<h4>Kết quả bài làm</h4>
				<div class="mb-3">
					<div class="display-4 text-primary">${data.score}%</div>
					<p>Bạn đã trả lời đúng ${data.correctAnswers}/${data.totalAnswered} câu</p>
				</div>
				<div class="progress mb-3">
					<div class="progress-bar bg-${data.score >= 80 ? 'success' : data.score >= 60 ? 'warning' : 'danger'}" 
						 style="width: ${data.score}%"></div>
				</div>
				<p class="text-muted">
					${data.score >= 80 ? 'Xuất sắc! Bạn có kiến thức tốt về y học.' :
				data.score >= 60 ? 'Khá tốt! Bạn cần ôn tập thêm một chút.' :
					'Cần cố gắng hơn. Hãy học thêm và thử lại!'}
				</p>
			</div>
		`;

		document.querySelector('.question-section').innerHTML = resultMessage;
		document.querySelector('.navigation-buttons').style.display = 'none';
	} catch (error) {
		showNotification('Lỗi khi tải kết quả!', 'warning');
		console.log(error);
	}
}

// Update navigation buttons
function updateNavigationButtons() {
	const backBtn = document.getElementById('backBtn');
	const nextBtn = document.getElementById('nextBtn');

	backBtn.disabled = orderedQuestions <= 1;
	nextBtn.innerHTML = orderedQuestions >= totalQuestions ?
		'<i class="fas fa-check"></i> Hoàn thành' :
		'Tiếp theo <i class="fas fa-arrow-right"></i>';
}

// Notification function
function showNotification(message, type) {
	const notification = document.createElement('div');
	notification.className = `alert alert-${type === 'warning' ? 'warning' : 'success'} alert-dismissible fade show position-fixed`;
	notification.style.cssText = `
		top: 20px;
		right: 20px;
		z-index: 9999;
		min-width: 300px;
		box-shadow: 0 4px 12px rgba(0,0,0,0.15);
	`;

	notification.innerHTML = `
		<div class="d-flex align-items-center">
			<i class="fas fa-${type === 'warning' ? 'exclamation-triangle' : 'check-circle'} me-2"></i>
			<span>${message}</span>
			<button type="button" class="btn-close ms-auto" data-bs-dismiss="alert"></button>
		</div>
	`;

	document.body.appendChild(notification);
	setTimeout(() => {
		if (notification.parentNode) {
			notification.remove();
		}
	}, 3000);
}

// Chat functions
function toggleChat() {
	const chatWindow = document.getElementById('chatWindow');
	chatWindow.classList.toggle('active');
	if (chatWindow.classList.contains('active')) {
		document.getElementById('chatInput').focus();
	}
}

function closeChat() {
	document.getElementById('chatWindow').classList.remove('active');
}

async function sendMessage() {
	const input = document.getElementById('chatInput');
	const message = input.value.trim();

	if (!message) return;

	addMessage(message, 'user');
	input.value = '';
	showTypingIndicator();

	try {
		const response = await fetch('/Quiz/chat', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				'X-Result-ID': resultId
			},
			body: JSON.stringify({ message })
		});
		if (!response.ok) throw new Error('Invalid response');
		const data = await response.json();
		hideTypingIndicator();
		addMessage(data.response, 'bot');
	} catch (error) {
		hideTypingIndicator();
		showNotification('Lỗi khi gửi tin nhắn!', 'warning');
		console.error(error);
	}
}

function addMessage(text, type) {
	const messagesContainer = document.getElementById('chatMessages');
	const messageDiv = document.createElement('div');
	messageDiv.className = `message ${type}`;

	if (type === 'user') {
		messageDiv.innerHTML = `
			<div class="message-content">${text}</div>
			<div class="message-avatar">
				<i class="fas fa-user"></i>
			</div>
		`;
	} else {
		messageDiv.innerHTML = `
			<div class="message-avatar">
				<i class="fas fa-robot"></i>
			</div>
			<div class="message-content">${text}</div>
		`;
	}

	messagesContainer.appendChild(messageDiv);
	messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

function showTypingIndicator() {
	const messagesContainer = document.getElementById('chatMessages');
	const typingDiv = document.createElement('div');
	typingDiv.className = 'message bot';
	typingDiv.id = 'typingIndicator';
	typingDiv.innerHTML = `
		<div class="message-avatar">
			<i class="fas fa-robot"></i>
		</div>
		<div class="typing-indicator">
			<div class="typing-dot"></div>
			<div class="typing-dot"></div>
			<div class="typing-dot"></div>
		</div>
	`;

	messagesContainer.appendChild(typingDiv);
	messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

function hideTypingIndicator() {
	const typingIndicator = document.getElementById('typingIndicator');
	if (typingIndicator) {
		typingIndicator.remove();
	}
}

function handleKeyPress(event) {
	if (event.key === 'Enter') {
		sendMessage();
	}
}

// Initialize animations and quiz
document.addEventListener('DOMContentLoaded', function () {
	initializeQuiz();
	const cards = document.querySelectorAll('.info-card');
	cards.forEach((card, index) => {
		card.style.opacity = '0';
		card.style.transform = 'translateX(-20px)';
		setTimeout(() => {
			card.style.transition = 'all 0.5s ease';
			card.style.opacity = '1';
			card.style.transform = 'translateX(0)';
		}, index * 200);
	});
});