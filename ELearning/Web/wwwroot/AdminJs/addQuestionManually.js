document.addEventListener("alpine:init", () => {
    Alpine.data("addQuestion", () => ({
        _dataExam: {},
        _listChapter: [],
        _dataQuestion: [],
        _listQuestionInExam: [],
        _idExam: 0,
        searchQuery: '',
        selectedChapter: '',
        selectedDifficulty: '',

        currentPage: 1,        // Trang hiện tại
        totalPages: 1,         // Tổng số trang
        itemsPerPage: 10,       // Số câu hỏi mỗi trang
        _dataFilterQuestion: [], // Dữ liệu câu hỏi đã lọc

        init() {
            this._idExam = this.getIdFromUrl();
            this.refreshData();
            this.LoadChapter();

            this.updatePageData();
        },

        getIdFromUrl() {
            const pathSegments = window.location.pathname.split('/');
            return pathSegments[pathSegments.length - 1];
        },

        refreshData() {
            fetch(`/Admin/Exam/GetDetailExam/${this._idExam}`)
                .then(response => response.json())
                .then(data => {
                    this._dataExam = data.exam;
                    this._dataQuestion = data.listQuestion;
                    this._listQuestionInExam = data.detailExam;

                    // Tính toán tổng số trang từ số câu hỏi
                    this.totalPages = Math.ceil(this._dataQuestion.length / this.itemsPerPage);
                    this.updateExamInfo();
                    this.filterQuestions();
                    this.updatePageData();
                }).catch(err => {
                    console.log(err);
                });
        },

        LoadChapter() {
            fetch(`/Admin/Question/GetChapter?subjectId=${this._dataExam.subjectId}`)
                .then(response => response.json())
                .then(data => {
                    this._listChapter = data;
                }).catch(err => {
                    console.log(err);
                });
        },

        updateExamInfo() {
            document.getElementById('name-test').innerText = this._dataExam.title;
            document.getElementById('test-time').innerText = this._dataExam.workTime;
            this.updateQuestionCounts();
        },

        getDifficultyLevel(level) {
            switch (level) {
                case 1: return 'Dễ';
                case 2: return 'Trung bình';
                case 3: return 'Khó';
                default: return 'Không xác định';
            }
        },

        updateQuestionCounts() {
            const easyCount = this._listQuestionInExam.filter(q => q.level === 1).length;
            const mediumCount = this._listQuestionInExam.filter(q => q.level === 2).length;
            const hardCount = this._listQuestionInExam.filter(q => q.level === 3).length;

            document.getElementById('slcaude').innerText = easyCount;
            document.getElementById('slcautb').innerText = mediumCount;
            document.getElementById('slcaukho').innerText = hardCount;

            document.getElementById('ttcaude').innerText = this._dataExam.eqCount;
            document.getElementById('ttcautb').innerText = this._dataExam.mqCount;
            document.getElementById('ttcaukho').innerText = this._dataExam.hqCount;
        },

        isQuestionInExam(questionId) {
            return this._listQuestionInExam.some(q => q.id === questionId);
        },

        addQuestionToExam(question) {
            if (!this.isQuestionInExam(question.id)) {
                // kiểm tra số lượng câu hỏi theo mức độ trong đề
                if (question.level === 1 && this._listQuestionInExam.filter(q => q.level === 1).length
                    >= this._dataExam.eqCount) {
                    showNotification({
                        type: 'danger',
                        message: "Số lượng câu hỏi dễ đã đạt giới hạn",
                    });

                } else if (question.level === 2 && this._listQuestionInExam.filter(q => q.level === 2).length
                    >= this._dataExam.mqCount) {
                    showNotification({
                        type: 'danger',
                        message: "Số lượng câu hỏi trung bình đã đạt giới hạn",
                    });
                } else if (question.level === 3 && this._listQuestionInExam.filter(q => q.level === 3).length
                    >= this._dataExam.hqCount) {
                    showNotification({
                        type: 'danger',
                        message: "Số lượng câu hỏi khó đã đạt giới hạn",
                    });
                } else {
                    this._listQuestionInExam.push(question);
                    this.updateQuestionCounts();
                }
            }
        },

        removeQuestionFromExam(questionId) {
            this._listQuestionInExam = this._listQuestionInExam.filter(q => q.id !== questionId);
            this.updateQuestionCounts();
        },

        moveQuestionUp(index) {
            if (index >= 0) {
                const temp = this._listQuestionInExam[index];
                this._listQuestionInExam[index] = this._listQuestionInExam[index - 1];
                this._listQuestionInExam[index - 1] = temp;
            }
        },

        moveQuestionDown(index) {
            if (index < this._listQuestionInExam.length - 1) {
                const temp = this._listQuestionInExam[index];
                this._listQuestionInExam[index] = this._listQuestionInExam[index + 1];
                this._listQuestionInExam[index + 1] = temp;
            }
        },

        saveExamDetail() {
            // map _listQuestionInExam thành data với các trường QuestionId, và DisplayOrder = Index
            var data = this._listQuestionInExam.map((q, index) => {
                return {
                    questionId: q.id,
                    examId: parseInt(this._idExam),
                    displayOrder: index + 1
                };
            });


            fetch(`/Admin/Exam/SaveExamDetail/${this._idExam}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        showNotification({
                            type: 'success',
                            message: "Lưu câu hỏi thành công",
                        });
                    } else {
                        showNotification({
                            type: 'danger',
                            message: "Lưu câu hỏi thất bại",
                        });
                    }
                }).catch(err => {
                    console.log(err);
                });


        },



        filterQuestions() {
            return this._dataQuestion.filter(q => {
                const matchesSearch = q.content.toLowerCase().includes(this.searchQuery.toLowerCase());
                const matchesChapter = this.selectedChapter ? q.chapterId === this.selectedChapter : true;
                const matchesDifficulty = this.selectedDifficulty ? q.level === parseInt(this.selectedDifficulty) : true;
                return matchesSearch && matchesChapter && matchesDifficulty;
            });
        },

        updatePageData() {
            // Tính toán dữ liệu câu hỏi cần hiển thị cho trang hiện tại
            const start = (this.currentPage - 1) * this.itemsPerPage;
            const end = start + this.itemsPerPage;
            this._dataFilterQuestion = this.filterQuestions().slice(start, end);
        },

        goToPage(page) {
            if (page >= 1 && page <= this.totalPages) {
                this.currentPage = page;
                this.updatePageData();
            }
        },

        prevPage() {
            if (this.currentPage > 1) {
                this.currentPage--;
                this.updatePageData();
            }
        },

        nextPage() {
            if (this.currentPage < this.totalPages) {
                this.currentPage++;
                this.updatePageData();
            }
        },
    }));
});
