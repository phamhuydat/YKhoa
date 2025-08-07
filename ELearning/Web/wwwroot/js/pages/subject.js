Dashmix.helpersOnLoad(["js-flatpickr", "jq-datepicker", "jq-select2"]);

Dashmix.onLoad(() =>
    class {
        static initValidation() {
            Dashmix.helpers("jq-validation"),
                jQuery(".form-add-subject").validate({
                    rules: {
                        mamonhoc: {
                            required: !0,
                            digits: true,
                        },
                        tenmonhoc: {
                            required: !0,
                        },
                        sotinchi: {
                            required: !0,
                        },
                        sotiet_lt: {
                            required: !0,
                        },
                        sotiet_th: {
                            required: !0,
                        },
                    },
                    messages: {
                        mamonhoc: {
                            required: "Vui lòng nhập mã môn học",
                            digits: "Mã môn học phải là các ký tự số",
                        },
                        tenmonhoc: {
                            required: "Vui lòng cung cấp tên môn học",
                        },
                        sotinchi: {
                            required: "Vui lòng cho biết số tín chỉ",
                        },
                        sotiet_lt: {
                            required: "Vui lòng nhập số tiết lý thuyết",
                        },
                        sotiet_th: {
                            required: "Vui lòng nhập số tiết thực hành",
                        },
                    },
                });
        }

        static init() {
            this.initValidation();
        }
    }.init()
);


$(document).ready(function () {
    function resetFormChapter() {
        $("#collapseChapter").collapse("hide");
        $("#name_chapter").val("");
    }

    $(".close-chapter").click(function (e) {
        e.preventDefault();
        $("#collapseChapter").collapse("hide");
    });

    $(document).on("click", ".chapter-edit", function () {
        //$("#add-chapter").hide();
        //$("#edit-chapter").show();
        //let id = $(this).data("id");
        //$("#machuong").val(id);
        $("#collapseChapter").collapse("show");
        //let name = $(this).closest("td").closest("tr").children().eq(1).text();
        //$("#name_chapter").val(name);
    });

});

// Pagination
//const mainPagePagination = new Pagination();
//mainPagePagination.option.controller = "subject";
//mainPagePagination.option.model = "MonHocModel";
//mainPagePagination.option.limit = 10;
//mainPagePagination.getPagination(
//    mainPagePagination.option,
//    mainPagePagination.valuePage.curPage
//);
