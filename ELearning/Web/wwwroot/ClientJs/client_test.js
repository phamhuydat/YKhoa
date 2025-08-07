document.addEventListener("alpine:init", () => {
    Alpine.data("clientTest", () => ({
        _list: [],
        searchQuery: '',
        filterStatus: 3, // Default to 'Tất cả'
        filterText: 'Tất cả',
        currentPage: 1,
        pageSize: 5,

        init() {
            this.refreshData();
        },

        refreshData() {
            fetch("/Test/LoadListExam")
                .then(x => x.json())
                .then(json => {
                    this._list = json;
                })
                .catch(err => {
                    console.log(err);
                });
        },
        get filteredList() {
            let filtered = this._list;

            if (this.filterStatus !== 3) {
                filtered = filtered.filter(item => item.isStatus === this.filterStatus);
            }

            if (this.searchQuery) {
                filtered = filtered.filter(item => item.examName.toLowerCase().includes(this.searchQuery.toLowerCase()));
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
        searchTests() {
            this.currentPage = 1;
        },
        setFilter(status) {
            this.filterStatus = status;
            this.filterText = status === 0 ? 'Chưa mở' : status === 1 ? 'Đang mở' : status === 2 ? 'Đã đóng' : 'Tất cả';
            this.currentPage = 1;
        },
        formatDateTime(dateTime) {
            const date = new Date(dateTime);
            const day = String(date.getDate()).padStart(2, '0');
            const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are zero-based
            const year = date.getFullYear();
            const hours = String(date.getHours()).padStart(2, '0');
            const minutes = String(date.getMinutes()).padStart(2, '0');
            return `${hours}:${minutes} ${day}/${month}/${year}`;
        },


    }));
});
