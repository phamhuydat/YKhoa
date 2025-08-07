$(document).ready(() => {
    $(".form-taothongbao").validate({
        rules: {
            "name-exam": {
                required: !0,
            },
            "nhom-hp": {
                required: !0,
            },
        },
        messages: {
            "name-exam": {
                required: "Nhập nội dung thông báo cần gửi",
            },
            "nhom-hp": {
                required: "Vui lòng chọn nhóm học phần",
            },
        }
    });
});


document.addEventListener("alpine:init", () => {
    Alpine.data("addNotify", () => ({

        _listGroup: [],
        _listSubject: [],
        subjectId: 0,

        _updinData: {
            id: 0,
            content: "",
            details: []
        },

        init() {

            if (window.location.pathname === "/Admin/Notify/Update/") {

                this._updinData.id = this.getIdFromUrl();
            }
            this.LoadSubject();

        },

        getIdFromUrl() {
            const pathSegments = window.location.pathname.split('/');
            return pathSegments[pathSegments.length - 1];
        },

        LoadData(id) {
            fetch(`/Admin/Exam/GetNotifyById?id=${id}`)
                .then(x => x.json())
                .then(json => {
                    this._updinData = json;
                })
                .catch(err => {
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

        LoadListGroup(id) {
            fetch(`/Admin/Exam/GetListGroup?subjectId=${id}`)
                .then(x => x.json())
                .then(json => {
                    this._listGroup = json;
                    console.log(this._listGroup);
                })
                .catch(err => {
                    console.log(err);
                });
        },

        SaveNotify() {
            // Ensure _updinData has the correct structure
            var data = {
                id: this._updinData.id,
                content: this._updinData.content,
                details: this._updinData.details.map(x => ({
                    notificationId: this._updinData.id,
                    groupId: x,
                }))
            };

            console.log(data);

            fetch("/Admin/Notify/SaveNotify", {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data) // Send the mapped data
            })
                .then(x => x.json())
                .then(json => {
                    if (json.success) {
                        showNotification({
                            type: 'success',
                            message: "Tạo thông báo thành công",
                        });
                    } else {
                        showNotification({
                            type: 'danger',
                            message: "Dữ liệu không hợp lẹ",
                        });
                    }
                })
                .catch(err => {
                    showNotification({
                        type: 'danger',
                        message: "Lỗi server",
                    });
                });
        }

    }));
})

