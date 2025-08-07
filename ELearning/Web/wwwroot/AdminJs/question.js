CKEDITOR.replace("option-content");
CKEDITOR.replace("js-ckeditor");

$(document).ready(() => {
    $('.jq-select2').select2();

    // Initialize jqValidation setup for real-time  site.js
    jqValidation();
    // Initialize form validation
    $("#form_add_question").validate({
        rules: {
            "mon-hoc": {
                required: true,
            },
            "chuong": {
                required: true,
            },
            "dokho": {
                required: true,
            },
            "js-ckeditor": {
                required: true,
            },
        },
        messages: {
            "mon-hoc": {
                required: "Vui lòng chọn môn học",
            },
            "chuong": {
                required: "Vui lòng chọn chương.",
            },
            "dokho": {
                required: "Vui lòng chọn mức độ.",
            },
            "js-ckeditor": {
                required: "Vui lòng không để trống câu hỏi.",
            },

        },
        errorClass: "is-invalid",
        validClass: "is-valid",
    });

    $("#form-add-doc").validate({
        rules: {
            "monhocfile": {
                required: true,
            },
            "chuongfile": {
                required: true,
            },
            "file-cau-hoi": {
                required: true,
            }
        },
        messages: {
            "monhocfile": {
                required: "Vui lòng không để trống môn học.",
            },
            "chuongfile": {
                required: "Vui lòng không để trống chương.",
            },
            "file-cau-hoi": {
                required: "Vui lòng chọn file câu hỏi.",
            }
        },
    });


});

document.addEventListener("alpine:init", () => {
    Alpine.data("questions", () => ({
        _list: [],
        _listSubject: [],
        _listChapter: [],
        _listAnswer: [],
        _modal: {},
        open: false,
        _editAnswer: false,
        _editIndex: -1,
        _noti: {},
        _modalSetting: {
            title: "",
            url: "",
            primaryButtonText: ""
        },

        _updinData: {
            id: 0,
            content: "",
            subjectId: 0,
            chapterId: 0,
            level: 0,
            options: {
                id: 0,
                answersContent: "",
                status: false,
            },
        },
        _answer: {
            answersContent: "",
            status: false
        },

        _importFile: {
            subjectId: 0,
            chapterId: 0,
            file: null
        },
        _listContentFile: [],

        currentPage: 1,
        itemsPerPage: 10,
        searchTerm: "",

        get filteredList() {
            if (!this.searchTerm) {
                return this._list;
            }
            return this._list.filter(item =>
                item.content.toLowerCase().includes(this.searchTerm.toLowerCase())
            );
        },

        get paginatedList() {
            const start = (this.currentPage - 1) * this.itemsPerPage;
            const end = start + this.itemsPerPage;
            return this.filteredList.slice(start, end);
        },

        get totalPages() {
            return Math.ceil(this.filteredList.length / this.itemsPerPage);
        },


        init() {
            var config = {
                durations: {
                    success: 2000
                },
                labels: {
                    success: "Thành công"
                }
            };

            this._modal = new bootstrap.Modal("#modal-add-question");
            this.refreshData();
            this.LoadSubject();
            this.LoadChapter();
        },

        async refreshData() {
            fetch("/Admin/Question/ListItem")
                .then(x => x.json())
                .then(json => {
                    this._list = json.map((item, index) => ({
                        ...item,
                        rowIndex: index + 1
                    }));
                })
                .catch(err => {
                    console.log(err);
                });
        },

        LoadSubject() {
            fetch("/Admin/Question/GetSubject")
                .then(x => x.json())
                .then(json => {
                    this._listSubject = json;
                })
                .catch(err => {
                    console.log(err);
                });
        },
        LoadChapter() {
            // Lấy danh sách chương theo môn học
            fetch(`/Admin/Question/GetChapter?subjectId=${this._updinData.subjectId
                }`)
                .then(x => x.json())
                .then(json => {
                    this._listChapter = json;
                })
                .catch(err => {
                    console.log(err);
                });
        },
        OpenModalAdd() {
            this._modal.show();
            this._modalSetting = {
                title: "Thêm câu hỏi",
                url: "/Admin/Question/CreateQuestion",
                primaryButtonText: "Lưu câu hỏi"
            };
            // Xóa dữ liệu khi mở modal add
            this._updinData = {
                id: 0,
                content: "",
                subjectName: "",
                chapterName: "",
                level: 0,
                options: [
                ],
            };
        },

        OpenModalEdit(idQuestion) {
            this._modal.show();
            this._modalSetting = {
                title: "Sửa câu hỏi",
                url: "/Admin/Question/EditQuestion/" + idQuestion,
                primaryButtonText: "Lưu câu hỏi"
            };

            //get data from server
            fetch(`/ Admin / Question / GetQuestionById ? id = ${idQuestion}`)
                .then(x => x.json())
                .then(json => {
                    this._updinData.subjectId = json.subjectId;
                    this._updinData.chapterId = json.chapterId;
                    this._updinData.level = json.level;
                    this._updinData.id = json.id;
                    this._updinData.content = json.content;

                    CKEDITOR.instances['js-ckeditor'].setData(this._updinData.content);
                    this._updinData.options = json.options;
                    this._updinData.options.forEach((answer, idx) => {
                        answer.rowIndex = idx + 1;
                        answer.answersContent = answer.answerContent;
                        answer.status = answer.status;
                    });

                    this.LoadSubject();
                    this.LoadChapter();
                })
                .catch(err => {
                    console.log(err);
                });
        },

        SaveQuestion() {
            // Prepare the data to be sent to the server
            this._updinData.content = CKEDITOR.instances['js-ckeditor'].getData();
            const data = {
                id: this._updinData.id,
                content: this._updinData.content,
                subjectId: this._updinData.subjectId,
                chapterId: this._updinData.chapterId,
                level: this._updinData.level,
                options: this._updinData.options.map(option => ({
                    id: option.id,
                    answerContent: option.answersContent,
                    status: option.status
                }))
            };

            fetch(this._modalSetting.url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            })
                .then(response => response.json())
                .then(result => {
                    if (result.success) {
                        // Handle success
                        this.refreshData();
                        showNotification({
                            type: 'success',
                            //icon: 'btn-close',
                            message: result.message,
                        });

                    } else {
                        // Handle error
                        showNotification({
                            type: 'danger',
                            //icon: 'btn-close',
                            message: data.message,
                        });
                    }
                })
                .catch(error => {
                    console.log(error);
                    showNotification({
                        type: 'danger',
                        //icon: 'btn-close',
                        message: 'Lỗi rồi',
                    });
                });
        },
        DeleteQuestion(idQuestion) {
            fetch("/Admin/Question/DeleteQuestion/" + idQuestion, {
                method: "POST"
            })
                .then(x => x.json())
                .then(json => {
                    console.log(json);
                    this.refreshData();
                    showNotification({
                        type: 'success',
                        //icon: 'btn-close',
                        message: json.message,
                    });

                })
                .catch(err => {
                    console.log(err);
                    showNotification({
                        type: 'danger',
                        //icon: 'btn-close',
                        message: 'Xóa môn học không thành công',
                    });
                    Dashmix.helpers('jq-notify', { type: 'danger', icon: 'fa fa-times me-1', message: '' });

                });
        },

        handleFile(event) {
            this._importFile.file = event.target.files[0];

            if (!this._importFile.file) {
                console.log("No file selected");
                return;
            }

            const formData = new FormData();
            formData.append("file", this._importFile.file); // Thêm file
            formData.append("subjectId", parseInt(this._updinData.subjectId)); // Thêm subjectId
            formData.append("chapterId", parseInt(this._updinData.chapterId)); // Thêm chapterId

            fetch("/Admin/Question/ProcessFile", {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(result => {
                    this._listContentFile = result;
                })
                .catch(error => {
                    console.log(error);
                });

        },

        ImportFileWord() {
            if (!this._importFile.file) {
                console.log("khong cos file");
                return;
            }

            const formData = new FormData();
            formData.append("fileWord", this._importFile.file); // Thêm file
            formData.append("subjectId", parseInt(this._updinData.subjectId)); // Thêm subjectId
            formData.append("chapterId", parseInt(this._updinData.chapterId)); // Thêm chapterId

            fetch("/Admin/Question/ImportFileWord", {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(result => {
                    console.log(result);
                    showNotification({
                        type: result.sucess ? 'danger' : 'success',
                        message: result.message,
                    });
                    this.refreshData();

                    // Clear data
                    this._importFile.file = null;
                    this._updinData.subjectId = 0;
                    this._updinData.chapterId = 0;
                    this._modal.hide();
                })
                .catch(error => {
                    console.log(error);
                    showNotification({
                        type: 'danger',
                        message: 'Lỗi rồi server ra mà fix di',
                    });

                    this._importFile.file = null;
                    this._updinData.subjectId = 0;
                    this._updinData.chapterId = 0;
                });
        },


        OpenAddAnswer() {
            this.open = !this.open;
            this._answer = {
                answersContent: '',
                status: false
            };
            this._editAnswer = false;
            CKEDITOR.instances['option-content'].setData('');
            // Open your modal here
        },
        CloseAnswer() {
            this.open = !this.open;
            this._answer = {
                answersContent: '',
                status: false
            };
        },
        AddOrEditAnswer() {
            if (this._editAnswer) {
                this._answer.answersContent = CKEDITOR.instances['option-content'].getData();

                this._updinData.options[this._editIndex] = { ...this._answer };

                console.log(this._updinData.options[this._editIndex]);

                this._answer.answersContent = CKEDITOR.instances['option-content'].setData('');
                this._answer = { answersContent: '', status: false };


                this.open = false;

                this._editAnswer = false;
            }
            else {
                this._answer.answersContent = CKEDITOR.instances['option-content'].getData();
                if (this._answer.answersContent.trim() === '') {
                    //alert('Vui lòng nhập nội dung câu trả lời!');
                    return;
                }

                this._updinData.options.push({
                    rowIndex: this._updinData.options.length + 1,
                    answersContent: this._answer.answersContent,
                    status: this._answer.status
                });
                this.open = false;
                this._answer.answersContent = CKEDITOR.instances['option-content'].setData('');

                // Reset _answer sau khi thêm
                this._answer = { answersContent: '', status: false };
            }
            //alert('Câu trả lời đã được thêm vào danh sách!');
        },
        EditAnswer(index) {

            this._editAnswer = true;
            this._editIndex = index;
            this._answer = { ...this._updinData.options[index] };
            CKEDITOR.instances['option-content'].setData(this._answer.answersContent);
            this.open = true;
        },
        DeleteAnswer(index) {
            this._updinData.options.splice(index, 1);
            // Update rowIndex for remaining answers
            this._updinData.options.forEach((answer, idx) => {
                answer.rowIndex = idx + 1;
            });
        },


        prevPage() {
            if (this.currentPage > 1) {
                this.currentPage--;
            }
        },

        nextPage() {
            if (this.currentPage < this.totalPages) {
                this.currentPage++;
            }
        },

        goToPage(page) {
            this.currentPage = page;
        }


    }))
});