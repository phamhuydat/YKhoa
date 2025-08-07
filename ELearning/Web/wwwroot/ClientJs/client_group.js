
document.addEventListener("alpine:init", () => {
    Alpine.data("clientGroup", () => ({
        _list: [],
        _listGroups: [],
        _listUsers: [],
        _listExams: [],
        _listNoty: [],


        invitedCode: "",

        init() {
            this.refreshData();
        },

        refreshData() {
            fetch("/GroupUser/LoadListGroup")
                .then(x => x.json())
                .then(json => {
                    this._listGroups = json;
                })
                .catch(err => {
                    console.log(err);
                });


        },


        BtnJoinGroup() {
            fetch("/GroupUser/JoinGroup", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(this.invitedCode)
            })
                .then(res => res.json())
                .then(data => {
                    if (!data.success) {
                        console.log(data);
                        showNotification({
                            type: 'danger',
                            message: data.message,
                        });
                        this.refreshData();
                    }
                    else {
                        showNotification({
                            type: 'success',
                            message: data.message,
                        });
                        this.refreshData();
                    }
                })
                .catch(err => {
                    console.log(err);
                    showNotification({
                        type: 'danger',
                        message: "Lỗi sersver rồi",
                    });
                });
        },

        BtnLeaveGroup(id) {
            fetch("/GroupUser/LeaveGroup" + id)
                .then(res => res.json())
                .then(data => {
                    if (!data.success) {
                        console.log(data);
                        showNotification({
                            type: 'danger',
                            message: data.message,
                        });
                        this.refreshData();
                    }
                    else {
                        showNotification({
                            type: 'success',
                            message: data.message,
                        });
                        this.refreshData();
                    }
                })
                .catch(err => {
                    console.log(err);
                    showNotification({
                        type: 'danger',
                        message: "Lỗi sersver rồi",
                    });
                });
        },

        LoadInfoGroup(id) {
            fetch("/GroupUser/LoadListUser/" + id)
                .then(x => x.json())
                .then(json => {
                    this._listUsers = json;
                })
                .catch(err => {
                    console.log(err);
                });

            fetch("/GroupUser/LoadListExam/" + id)
                .then(x => x.json())
                .then(json => {
                    this._listExams = json;
                })
                .catch(err => {
                    console.log(err);
                });


            fetch("/GroupUser/LoadNoty/" + id)
                .then(x => x.json())
                .then(json => {
                    this._listNoty = json;
                    console.log(this._listNoty);
                })
                .catch(err => {
                    console.log(err);
                });
        },

        formatDateTime(dateTime) {
            const date = new Date(dateTime);
            const day = String(date.getDate()).padStart(2, '0');
            const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are zero-based
            const year = date.getFullYear();
            const hours = String(date.getHours()).padStart(2, '0');
            const minutes = String(date.getMinutes()).padStart(2, '0');
            return `${hours}:${minutes} ${day}/${month}/${year}`;
        }

    }));
});
