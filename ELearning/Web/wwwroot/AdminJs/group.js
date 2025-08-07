$(document).ready(() => {
    // Initialize jqValidation setup for real-time  site.js
    jqValidation();
    // Initialize form validation
    $('#form-add-group').validate({
        rules: {
            "ten-nhom": {
                required: true,
            },
            "ghi-chu": {
                required: true,
            },
            "mon-hoc": {
                required: true,
            },
            "nam-hoc": {
                required: true
            },
            "hoc-ky": {
                required: true,
            },
        },
        messages: {
            "ten-nhom": {
                required: "Vui lòng nhập tên nhóm",
            },
            "ghi-chu": {
                required: "Vui lòng không để trống trường này",
            },
            "mon-hoc": {
                required: "Vui lòng chọn môn học",
            },
            "nam-hoc": {
                required: "Vui lòng chọn năm học",
            },
            "hoc-ky": {
                required: "Vui lòng chọn học kỳ",
            },
            "mssv": {
                required: "Vui lòng nhập mã số sinh viên",
                number: "Mã số sinh viên phải là số"
            },
        }
    });

    $('#add-user').validate({
        rules: {
            "mssv": {
                required: true,
                number: true
            },
        },
        messages: {
            "mssv": {
                required: "Vui lòng nhập mã số sinh viên",
                number: "Mã số sinh viên phải là số"
            },
        }
    });

    $(".btn-copy-code").click(function (e) {
        e.preventDefault();
        var text = $("#show-invited-code").text();
        navigator.clipboard.writeText(text)
            .then(function () {
                // Hiển thị thông báo thành công
            })
            .catch(function (error) {
                // Xử lý lỗi nếu có
                console.error("Lỗi sao chép:", error);
            });
    });
});

document.addEventListener("alpine:init", () => {
    Alpine.data("groups", () => ({
        _listGroups: [],
        _listUser: [],
        _modal: {},
        _modalUser: {},
        viewUser: false,
        currentYear: new Date().getFullYear(),
        _years: [],
        _listSubject: [],
        _modalSetting: {
            title: "",
            url: "",
            primaryButtonText: ""
        },
        _updinData: {
            id: 0,
            groupName: "",
            note: "",
            subjectId: "",
            subjectName: "",
            academicYear: "",
            semester: "",
        },
        mssv: "",
        InvitedCode: "",
        groupId: 0,
        groupName: "",

        init() {
            if (window.location.pathname === "/Admin/Group") {
                this._modal = new bootstrap.Modal("#modal-add-group");
                this.refreshData();
                this.LoadSubject();
                this.generateYearRange();
            }

            else {
                this._modalUser = new bootstrap.Modal("#modal-add-user");
                this.groupId = this.getIdFromUrl();
                this.LoadDataUser();
                this.LoadInvitation();

            }
        },
        getIdFromUrl() {
            const pathSegments = window.location.pathname.split('/');
            return pathSegments[pathSegments.length - 1];
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
        generateYearRange() {
            const startYear = this.currentYear - 5;  // Adjust as needed (e.g., start from 5 years ago)
            const endYear = this.currentYear + 5;    // Adjust as needed (e.g., go up to 5 years in the future)
            for (let year = startYear; year < endYear; year++) {
                // Create a year range as 'yyyy-yyyy'
                this._years.push(year + '-' + (year + 1));
            }
        },

        async refreshData() {
            fetch("/Admin/Group/ListGroup")
                .then(x => x.json())
                .then(json => {
                    this._listGroups = json;
                })
                .catch(err => {
                    console.log(err);
                });
        },

        async LoadDataUser() {
            fetch("/Admin/Group/ListUser/" + this.groupId)
                .then(x => x.json())
                .then(json => {
                    this._listUser = json.map((item, index) => ({
                        ...item,
                        rowIndex: index + 1
                    }));
                    this.groupName = this._listUser.map(x => x.groupName)[0];
                })
                .catch(err => {
                    console.log(err);
                });

        },

        async LoadInvitation() {
            var url = `/Admin/Group/GetInvitationCode?id=${this.groupId}`;
            fetch(url)
                .then(x => x.json())
                .then(json => {
                    this.InvitedCode = json.data;
                })
                .catch(err => {
                    console.log(err);
                });
        },

        OpenModalAdd() {
            this._modal.show();
            this._modalSetting = {
                title: "Thêm nhóm",
                url: "/Admin/Group/CreateGroup",
                primaryButtonText: "Lưu"
            };
            // Xóa dữ liệu khi mở modal add
        },

        OpenModalEdit(id) {
            this._modal.show();
            this._modalSetting = {
                title: "Cập nhật thông tin",
                url: "/Admin/Group/EditGroup/" + id,
                primaryButtonText: "Cập nhật"
            }

            // Lấy dữ liệu cho thao tác update
            fetch("/Admin/Group/GetGroup/" + id)
                .then(res => res.json())
                .then(json => {
                    this._updinData = json;
                    console.log(this._updinData);
                });
        },

        OpenListStudent(id) {
            window.location.href = "/Admin/Group/GetViewUser/" + id;
        },

        SaveData() {
            var data = {
                id: this._updinData.id,
                groupName: this._updinData.groupName,
                note: this._updinData.note,
                subjectId: this._updinData.subjectId,
                academicYear: this._updinData.academicYear,
                semester: this._updinData.semester,
            }
            fetch(this._modalSetting.url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(data)
            })
                .then(res => {
                    return res.json();
                })
                .then(data => {
                    if (data.success) {
                        showNotification({
                            type: 'success',
                            message: data.message,
                        });
                        this.refreshData();
                    }
                    else {
                        showNotification({
                            type: 'danger',
                            message: data.message,
                        });
                    }
                })
                .catch(err => {
                    console.log(err)
                    showNotification({
                        type: 'danger',
                        message: "Thêm nhóm không thành công lỗi server",
                    });
                })
        },

        BtnAddUser() {
            var url = `/Admin/Group/AddUserToGroup?mssv=${this.mssv}`;
            var data = {
                groupId: this.groupId,
                userId: 0
            }

            fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(data)
            })
                .then(res => {
                    return res.json();
                })
                .then(data => {
                    if (data.success) {
                        showNotification({
                            type: 'success',
                            message: data.message,
                        });
                        this.LoadDataUser();
                    }
                    else {
                        showNotification({
                            type: 'danger',
                            message: data.message,
                        });
                    }
                })
                .catch(err => {
                    console.log(err)
                    showNotification({
                        type: 'danger',
                        message: "Thêm sinh viên không thành công lỗi server",
                    });
                })
        },

        BtnResetInvitedCode() {
            var url = `/Admin/Group/UpdateInvitedCode?id=${this.groupId}`;

            fetch(url)
                .then(res => {
                    return res.json();
                })
                .then(data => {
                    if (data.success) {
                        this.LoadInvitation();
                    }
                    else {
                        showNotification({
                            type: 'danger',
                            message: data.message,
                        });
                    }
                })
                .catch(err => {
                    console.log(err)
                    showNotification({
                        type: 'danger',
                        message: "Reset mã code không thành công lỗi server",
                    });
                })
        },

        ExportExcelResultGroup() {
            fetch(`/Admin/File/ExportExcelTranscript?groupId=${this.groupId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => {
                    if (response.ok) {
                        if (response.success === false) {
                            showNotification({
                                type: 'danger',
                                message: "Không có bài thi nào",
                            });
                            return;
                        }
                        return response.blob();
                    } else {

                        showNotification({
                            type: 'danger',
                            message: "Không có file dowload",
                        });
                    }
                })
                .then(blob => {
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.style.display = 'none';
                    a.href = url;
                    a.download = 'transcript.xlsx'; // Set the file name
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                })
                .catch(error => {
                    console.error('Error:', error);
                    showNotification({
                        type: 'danger',
                        message: 'Failed to download file',
                    });
                });
        }

    }))
});
