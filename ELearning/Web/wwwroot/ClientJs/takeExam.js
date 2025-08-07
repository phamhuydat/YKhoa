document.addEventListener('alpine:init', () => {
	Alpine.data("takeTest", () => ({
		_listQuestion: [], // Danh sách câu hỏi
		userAnswers: [],   // Đáp án của người dùng
		infoExam: {},      // Thông tin bài thi
		examId: 0,         // ID của bài thi
		countdownInterval: null, // Để quản lý bộ đếm ngược
		unansweredQuestions: [], // Danh sách câu hỏi chưa trả lời

		// Lấy ID bài thi từ URL
		getIdFromUrl() {
			const pathSegments = window.location.pathname.split('/');
			return pathSegments[pathSegments.length - 1];
		},

		// Định dạng thời gian thành HH:MM:SS
		formatTime(seconds) {
			const hrs = Math.floor(seconds / 3600);
			const mins = Math.floor((seconds % 3600) / 60);
			const secs = seconds % 60;
			return `${hrs.toString().padStart(2, '0')}:${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
		},

		// Khởi tạo component
		init() {
			this.examId = this.getIdFromUrl();
			this.fetchQuestions();
		},

		// Thiết lập đếm ngược
		startCountdown(workTimeInSeconds) {
			if (workTimeInSeconds == 0) {
				this.submitAnswers(); // Nộp bài tự động
			}
			this.infoExam.workTime = this.formatTime(workTimeInSeconds); // Thời gian ban đầu

			this.countdownInterval = setInterval(() => {
				workTimeInSeconds--;

				// Khi hết thời gian, dừng đếm ngược và tự động nộp bài
				if (workTimeInSeconds <= 0) {
					clearInterval(this.countdownInterval);
					this.infoExam.workTime = "00:00:00";
				} else {
					this.infoExam.workTime = this.formatTime(workTimeInSeconds);
				}
			}, 1000); // Cập nhật mỗi giây
		},

		// Lấy câu hỏi từ server
		async fetchQuestions() {
			try {
				const response = await fetch(`/Test/TakeExamServer/${this.examId}`);
				if (!response.ok) {
					throw new Error(`Server error: ${response.status}`);
				}

				const data = await response.json();
				this._listQuestion = data.questions;
				this.infoExam = data.examVM;

				// Khởi tạo danh sách đáp án
				this._listQuestion.forEach(question => {
					this.userAnswers[question.id] = {
						resultId: this.infoExam.resultId,
						questionId: question.id,
						answerId: question.answerId == 0 ? null : question.answerId   // Nếu chưa chọn thì null
					};
				});

				// Bắt đầu đếm ngược
				const workTimeInSeconds = this.infoExam.workTime;
				this.startCountdown(workTimeInSeconds);
			} catch (error) {
				console.error('Fetch error:', error);
				showNotification({
					type: 'error',
					message: 'Đang tải bài thi. Vui lòng thử lại!',
				});
			}
		},

		// Lưu đáp án tạm thời khi người dùng chọn
		saveAnswer(questionId, id) {
			if (!this.userAnswers[questionId]) {
				this.userAnswers[questionId] = {};
			}
			this.userAnswers[questionId].answerId = id;
		},

		checkUnansweredQuestions() {
			this.unansweredQuestions = [];
			for (const questionId in this.userAnswers) {
				if (this.userAnswers[questionId].answerId === null) {
					this.unansweredQuestions.push(parseInt(questionId));
				}
			}
		},
		// Nộp bài thi
		submitAnswers() {
			this.checkUnansweredQuestions();
			if (this.unansweredQuestions.length > 0) {
				showNotification({
					type: 'warning',
					message: 'Bạn vẫn còn câu hỏi chưa trả lời. Vui lòng trả lời tất cả các câu hỏi trước khi nộp bài.',
				});
				return;
			}

			clearInterval(this.countdownInterval); // Dừng đếm ngược


			const data = Object.values(this.userAnswers).map(result => ({
				resultId: parseInt(result.resultId),
				questionId: parseInt(result.questionId),
				answerId: parseInt(result.answerId),
			}));

			try {
				fetch('/Test/SubmitAnswers', {
					method: 'POST',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify(data),
				})
					.then(res => res.json())
					.then(data => {
						showNotification({
							type: 'success',
							message: data.message,
						});
						window.location.href = `/Test/StartTest/${this.examId}`;
					})
			} catch (error) {
				console.error('Error:', error);
				showNotification({
					type: 'danger',
					message: 'Lỗi server',
				});
			}
		},

		scrollToQuestion(index) {
			const questionElement = document.querySelector(`#question-${index}`);
			if (questionElement) {
				questionElement.scrollIntoView({ behavior: 'smooth' });
			}
		}
	}));
});
