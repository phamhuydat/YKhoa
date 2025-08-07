$(document).ready(() => {
    jqValidation();
    $(".form-phancong").validate({
        rules: {
            "giang-vien": {
                required: !0,
            }
        },
        messages: {
            "giang-vien": {
                required: "Vui lòng chọn giảng viên",
            }
        },
    });

    // Initialize form validatio
});


document.addEventListener("alpine:init", () => {
    Alpine.data("assignment", () => ({
        _list: [],
        _modal: {},
        currentPage: 1,
        itemsPerPage: 10,
        searchTerm: "",
        _listTeacher: [],
        _listSubject: [],
        _updinData: {
            subjectId: [],
            userId: 0,
        },

        get filteredList() {
            if (!this.searchTerm) {
                return this._list;
            }

            return this._list.filter(item =>
                item.fullName.toLowerCase().includes(this.searchTerm.toLowerCase())
            );
        },

        get paginatedList() {
            const start = (this.currentPage - 1) * this.itemsPerPage;
            const end = start + this.itemsPerPage;
            console.log(this.filteredList.slice(start, end))
            return this.filteredList.slice(start, end);
        },

        get totalPages() {
            return Math.ceil(this.filteredList.length / this.itemsPerPage);
        },

        init() {
            this._modal = new bootstrap.Modal("#modal-add-assignment");
            this.refreshData();
            this.LoadListSubject();
            this.LoadListTeacher();
        },
        async refreshData() {
            fetch("/Admin/Assignment/GetList")
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

        LoadListTeacher() {
            fetch("/Admin/Assignment/GetListTeacher")
                .then(x => x.json())
                .then(json => {
                    this._listTeacher = json;
                })
                .catch(err => {
                    console.log(err);
                });
        },

        LoadListSubject() {
            fetch("/Admin/Assignment/GetListSubject")
                .then(x => x.json())
                .then(json => {
                    this._listSubject = json;
                })
                .catch(err => {
                    console.log(err);
                });
        },


        Save() {
            fetch("/Admin/Assignment/Save", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(this._updinData),
            })
                .then(x => x.json())
                .then(json => {
                    if (json.success) {
                        this._modal.hide();
                        this.refreshData();
                        showNotification({
                            type: 'success',
                            message: "Phân công thành công",
                        });
                    }
                })
                .catch(err => {
                    showNotification({
                        type: 'danger',
                        message: "Lỗi rồi",
                    });
                    console.log(err);
                });








        }








    }));

})