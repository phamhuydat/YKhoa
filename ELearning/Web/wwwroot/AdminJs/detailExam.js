document.addEventListener("alpine:init", () => {
    Alpine.data("detailExam", () => ({
        _list: [],
        _resultDetails: [],
        _modalExam: {},
        allData: [],
        groups: [],
        ExamId: 0,
        groupId: 0,
        groupName: '',
        status: 0,
        searchQuery: '',
        currentPage: 1,
        pageSize: 10,
        totalItems: 0,
        studentSuccess: 0,           // hoàn thành
        // học sinh chưa làm 
        studentNotDo: 0,
        // học sinh đang làm
        studentDoing: 0,

        // điểm trung bình của sinh viên đã làm
        averageScore: 0,

        init() {
            this._modalExam = new bootstrap.Modal("#modal-show-test");

            this.ExamId = this.getIdFromUrl();
            this.refreshData();
        },

        getIdFromUrl() {
            const pathSegments = window.location.pathname.split('/');
            return pathSegments[pathSegments.length - 1];
        },

        async refreshData() {
            fetch(`/Admin/Exam/ListStudentOfTakingExam/${this.ExamId}`)
                .then(x => x.json())
                .then(json => {
                    this._list = json.users;
                    this.statistic();
                    this.groups = json.groups;
                })
                .catch(err => {
                    console.log(err);
                });
        },

        get filteredList() {
            let filtered = this._list;


            if (this.groupId !== 0) {
                filtered = filtered.filter(item => item.groupId === this.groupId);
                this.groupName = this.groups.find(group => group.id === this.groupId).groupName;
            }

            if (this.status !== 0) {
                filtered = filtered.filter(item => item.status === this.status);
            }

            if (this.searchQuery) {
                filtered = filtered.filter(item => item.fullName.toLowerCase().includes(this.searchQuery.toLowerCase()));
            }

            return filtered;
        },

        get paginatedList() {
            const start = (this.currentPage - 1) * this.pageSize;
            const end = start + this.pageSize;
            return this.filteredList.slice(start, end);
        },

        get totalPages() {
            return Math.ceil(this.filteredList.length / this.pageSize);
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
        },

        setGroupId(groupId) {
            this.groupId = groupId;
            this.groupName = this.groups.find(group => group.id === this.groupId).groupName;
            this.currentPage = 1;
        },

        setStatus(status) {
            this.status = status;
            this.currentPage = 1;
        },

        setSearch(searchQuery) {
            this.searchQuery = searchQuery;
            this.currentPage = 1;
        },

        formatTime(seconds) {
            const minutes = Math.floor(seconds / 60);
            const remainingSeconds = seconds % 60;
            return `${minutes}m ${remainingSeconds}s`;
        },

        formatDateTime(dateTime) {
            const date = new Date(dateTime);
            const day = String(date.getDate()).padStart(2, '0');
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const year = date.getFullYear();
            const hours = String(date.getHours()).padStart(2, '0');
            const minutes = String(date.getMinutes()).padStart(2, '0');
            const seconds = String(date.getSeconds()).padStart(2, '0');
            return `${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
        },

        // thống kê số lượng học sinh đã làm, chưa làm, đang làm và điểm trung bình của sinh viên đã làm
        statistic() {
            this.studentSuccess = this._list.filter(item => item.status === 1).length;
            this.studentNotDo = this._list.filter(item => item.status === 2).length;
            this.studentDoing = this._list.filter(item => item.status === 0).length;
            this.totalItems = this._list + this.studentNotDo + this.studentDoing;
            // điểm trung bình của sinh viên đã làm
            let sum = 0;
            let count = 0;
            this._list.forEach(item => {
                if (item.status === 1) {
                    sum += item.testScores;
                    count++;
                }
            });

            this.averageScore = count > 0 ? (sum / count).toFixed(2) : 0;

        },

        ShowTakeExam(id) {
            this._modalExam.show();

            fetch(`/Admin/Exam/ResultDetail/?userId=${id}&examId=${this.ExamId}`)
                .then(x => x.json())
                .then(json => {
                    this._resultDetails = json;
                })
                .catch(err => {
                    console.log(err);
                    showNotification({
                        type: 'success',
                        message: "không có bài làm",
                    });
                });
        },

        print_pdf(id) {
            fetch(`/Admin/File/ExportPDF/?userId=${id}&examId=${this.ExamId}`, { method: "POST" })
                .then(response => response.text())
                .then(base64String => {
                    const binaryString = atob(base64String);  // Giải mã base64
                    const binaryLen = binaryString.length;
                    const bytes = new Uint8Array(binaryLen);

                    for (let i = 0; i < binaryLen; i++) {
                        bytes[i] = binaryString.charCodeAt(i);
                    }

                    const blob = new Blob([bytes], { type: "application/pdf" });
                    const url = URL.createObjectURL(blob);

                    // Tạo liên kết ẩn để tải xuống tệp PDF
                    const a = document.createElement("a");
                    a.href = url;
                    a.download = "result.pdf";
                    a.style.display = "none";
                    document.body.appendChild(a);
                    a.click();

                    setTimeout(() => {
                        document.body.removeChild(a);
                        URL.revokeObjectURL(url);
                    }, 100);
                })
                .catch(error => {
                    console.error("Lỗi khi tải file PDF:", error);
                });


        }




    }));

});
