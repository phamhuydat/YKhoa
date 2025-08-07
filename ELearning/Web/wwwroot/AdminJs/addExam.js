function getMinutesBetweenDates(start, end) {
    // Chuyển đổi đối số thành đối tượng Date
    const startDate = new Date(start);
    const endDate = new Date(end);

    // Tính số phút giữa hai khoảng thời gian
    const diffMs = endDate.getTime() - startDate.getTime();
    const diffMins = Math.round(diffMs / 60000);
    // Trả về số phút tính được
    return diffMins;
}

function getToTalQuestionOfChapter(chuong, monhoc, dokho) {
    var result = 0;
    console.log(chuong, monhoc, dokho);
    $.ajax({
        url: "/Admin/Exam/GetCountQuestion",
        type: "post",
        data: {
            subjectId: monhoc,
            chapterId: chuong,
            level: dokho,
        },
        async: false,
        success: function (response) {
            console.log(response);
            result = response;
        },
    });
    return result;
}
let groups = [];

$(document).ready(() => {
    function getMinutesBetweenDates(start, end) {
        // Chuyển đổi đối số thành đối tượng Date
        const startDate = new Date(start);
        const endDate = new Date(end);

        // Tính số phút giữa hai khoảng thời gian
        const diffMs = endDate.getTime() - startDate.getTime();
        const diffMins = Math.round(diffMs / 60000);

        // Trả về số phút tính được
        return diffMins;
    }
    function getToTalQuestionOfChapter(chuong, monhoc, dokho) {
        var result = 0;
        console.log(chuong, monhoc, dokho);
        $.ajax({
            url: "/Admin/Exam/GetCountQuestion",
            type: "post",
            data: {
                subjectId: monhoc,
                chapterId: chuong,
                level: dokho,
            },
            async: false,
            success: function (response) {
                result = response;
            },
        });
        return result;
    }

    $('.jq-select2').select2();
    // Initialize jqValidation setup for real-time  site.js
    jqValidation();
    // Add custom validation methods
    $.validator.addMethod(
        "validTimeEnd",
        function (value, element) {
            const startTime = new Date($("#time-start").val());
            const currentTime = new Date();
            const endTime = new Date(value);
            return endTime > startTime && endTime > currentTime;
        },
        "Thời gian kết thúc phải lớn hơn thời gian bắt đầu và không bé hơn thời gian hiện tại"
    );

    $.validator.addMethod(
        "validTimeStart",
        function (value, element) {
            const startTime = new Date(value);
            const currentTime = new Date();
            return startTime > currentTime;
        },
        "Thời gian bắt đầu không được bé hơn thời gian hiện tại"
    );

    // bug
    $.validator.addMethod(
        "validSoLuong",
        function (value, element, param) {

            subjectId = $("#nhom-hp").val();
            //console.log($("#nhom-hp").val())
            const result = parseInt(getToTalQuestionOfChapter(0, subjectId, param)) >= parseInt(value);
            return result;
        },
        "Số lượng câu hỏi không đủ"
    );

    $.validator.addMethod(
        "validThoigianthi",
        function (value, element, param) {
            const startTime = new Date($("#time-start").val());
            const endTime = new Date($("#time-end").val());
            const duration = parseInt(getMinutesBetweenDates(startTime, endTime), 10);
            return startTime < endTime && duration >= parseInt(value, 10);
        },
        "Thời gian làm bài không hợp lệ"
    );

    // Initialize validation
    $(".form-add-Exam").validate({
        rules: {
            "name-exam": {
                required: true,
            },
            "time-start": {
                required: true,
                validTimeStart: true,
            },
            "time-end": {
                required: true,
                validTimeEnd: true,
            },
            "exam-time": {
                required: true,
                digits: true,
                validThoigianthi: true,
            },
            "nhom-hp": {
                required: true,
            },
            chuong: {
                required: true,
            },
            eqCount: {
                required: !0,
                digits: true,
                validSoLuong: 1,
            },
            mqCount: {
                required: !0,
                digits: true,
                validSoLuong: 2,
            },
            hqCount: {
                required: !0,
                digits: true,
                validSoLuong: 3,
            },
        },
        messages: {
            "name-exam": {
                required: "Vui lòng nhập tên đề kiểm tra",
            },
            "time-start": {
                required: "Vui lòng chọn thời điểm bắt đầu của bài kiểm tra",
                validTimeStart: "Thời gian bắt đầu không được bé hơn thời gian hiện tại",
            },
            "time-end": {
                required: "Vui lòng chọn thời điểm kết thúc của bài kiểm tra",
                validTimeEnd: "Thời gian kết thúc không hợp lệ",
            },
            "exam-time": {
                required: "Vui lòng chọn thời gian làm bài kiểm tra",
                digits: "Vui lòng nhập số",
            },
            "nhom-hp": {
                required: "Vui lòng chọn nhóm học phần giảng dạy",
            },
            chuong: {
                required: "Vui lòng chọn số chương cho đề kiểm tra",
            },
            eqCount: {
                required: "Vui lòng cho biết số câu dễ",
                digits: "Vui lòng nhập số",
            },
            mqCount: {
                required: "Vui lòng cho biết số câu trung bình",
                digits: "Vui lòng nhập số",
            },
            hqCount: {
                required: "Vui lòng cho biết số câu khó",
                digits: "Vui lòng nhập số",
            },
        },
    });
});


document.addEventListener("alpine:init", () => {
    Alpine.data("AddExam", () => ({
        _listSubject: [],
        _listChapter: [],
        _listGroup: [],
        _updinData: {
            id: 0,
            title: "",
            timeStart: "",
            timeEnd: "",
            workTime: 0,
            subjectId: 0,
            chapterId: [],
            groupId: [],
            eqCount: 0,
            mqCount: 0,
            hqCount: 0,
            autoExam: true,
            mixQuestion: false,
            mixAnswer: false,
            submitWhenExit: false,
            showAnswer: false,
        },

        choicesInstance: null,
        selectedChapters: [],


        init() {
            if (this.getIdFromUrl() != "createExam") {
                this.LoadDataEdit();
                this.LoadListGroup();
            }

            if (this._updinData.autoExam === true) {
                if (this.choicesInstance) {
                    return; // Nếu đã khởi tạo rồi, không làm gì nữa
                }
                // Khởi tạo Choices.js
                this.choicesInstance = new Choices(this.$refs.selectElement, {
                    removeItemButton: true,
                    shouldSort: false,
                });

                // Thay thế danh sách option trong Choices.js bằng _listChapter
                this.updateChoices();

                // Lắng nghe thay đổi từ Choices.js
                this.$refs.selectElement.addEventListener("change", (event) => {
                    this.selectedChapters = Array.from(event.target.selectedOptions).map(
                        (option) => option.value
                    );
                });
            }
            this.LoadSubject();

        },

        getIdFromUrl() {
            const pathSegments = window.location.pathname.split('/');
            return pathSegments[pathSegments.length - 1];
        },

        // load data khi edit exam
        LoadDataEdit() {
            fetch(`/Admin/Exam/GetExam/${this.getIdFromUrl()}`)
                .then(response => response.json())
                .then(data => {
                    this._updinData = data.model;
                    this.LoadListGroup(this._updinData.subjectId);
                }).catch(err => {
                    console.log(err);
                });
        },

        LoadSubject() {
            fetch("/Admin/Exam/GetSubject")
                .then(x => x.json())
                .then(json => {
                    this._listSubject = json;
                })
                .catch(err => {
                    console.log(err);
                });
        },
        LoadChapter() {
            fetch(`/Admin/Exam/GetChapter?subjectId=${this._updinData.subjectId}`)
                .then(x => x.json())
                .then(json => {
                    this._listChapter = json;
                    this.updateChoices();
                })
                .catch(err => {
                    console.log(err);
                });
        },

        LoadListGroup(id) {
            fetch(`/Admin/Exam/GetListGroup?subjectId=${id}`)
                .then(x => x.json())
                .then(json => {
                    this._listGroup = json;
                })
                .catch(err => {
                    console.log(err);
                });
        },

        updateChoices() {
            // Xóa tất cả các option hiện tại
            this.choicesInstance.clearChoices();
            // Thêm các option mới từ _listChapter
            this._listChapter.forEach((chapter) => {
                this.choicesInstance.setChoices([
                    {
                        value: chapter.id,
                        label: chapter.chapterName,
                        selected: this.selectedChapters.includes(chapter.id),
                    },
                ]);
            });
        },

        formatDateTime(dateTimeStr) {
            const [date, time] = dateTimeStr.split(' ');
            const [day, month, year] = date.split('-');
            return `${year} - ${month} - ${day}T${time}:00`;
        },

        refreshData() {
            this._updinData = {
                id: 0,
                title: "",
                timeStart: "",
                timeEnd: "",
                workTime: 0,
                subjectId: 0,
                chapterId: [],
                groupId: [],
                eqCount: 0,
                mqCount: 0,
                hqCount: 0,
                autoExam: true,
                mixQuestion: false,
                mixAnswer: false,
                submitWhenExit: false,
                showAnswer: false,

            }
        },

        formatDateTime(dateTimeStr) {
            const date = new Date(dateTimeStr);
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            const hours = String(date.getHours()).padStart(2, '0');
            const minutes = String(date.getMinutes()).padStart(2, '0');
            return `${year}-${month}-${day}T${hours}:${minutes}:00`;
        },
        SaveExam() {
            const selectedItems = this.choicesInstance.getValue();
            this._updinData.chapterId = selectedItems.map(item => item.value); // Trả về mảng giá trị

            const data = {
                title: this._updinData.title,
                timeStart: this.formatDateTime(this._updinData.timeStart),
                timeEnd: this.formatDateTime(this._updinData.timeEnd),
                workTime: parseInt(this._updinData.workTime, 10),
                subjectId: parseInt(this._updinData.subjectId, 10),
                isAutomatic: this._updinData.autoExam,
                mixQuestion: this._updinData.mixQuestion,
                mixAnswer: this._updinData.mixAnswer,
                seeAnswer: this._updinData.showAnswer,
                submitWhenExit: this._updinData.submitWhenExit,
                eQCount: parseInt(this._updinData.eqCount, 10),
                mQCount: parseInt(this._updinData.mqCount, 10),
                hQCount: parseInt(this._updinData.hqCount, 10),
                status: true, // Assuming status is true when saving
                automaticExams: this._updinData.chapterId.map(chapterId => ({
                    chapterId: chapterId
                })),
                handOutExams: this._updinData.groupId.map(groupId => ({
                    groupId: groupId
                }))
            };
            console.log(data);

            fetch("/Admin/Exam/CreateExam", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(data),
            })
                .then(response => response.json())
                .then(response => {
                    if (response.success) {
                        showNotification({
                            type: 'success',
                            message: response.message,
                        });

                        if (!this._updinData.autoExam) {
                            fetch("/Admin/Exam/AddManualExam" + data.id);
                        }
                        this.refreshData();
                    } else {
                        showNotification({
                            type: 'danger',
                            message: response.message,
                        });
                    }
                })
                .catch((error) => {
                    console.error("Error:", error.message);
                    showNotification({
                        type: 'danger',
                        message: 'Lỗi server',
                    });
                });

        }
    }))
})