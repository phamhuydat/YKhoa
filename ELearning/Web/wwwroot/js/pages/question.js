Dashmix.helpersOnLoad(["jq-select2", "js-ckeditor"]);
CKEDITOR.replace("option-content");

Dashmix.onLoad(() =>
    class {
        static initValidation() {
            Dashmix.helpers("jq-validation"),
                jQuery("#form_add_question").validate({
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
        }
        static init() {
            this.initValidation();
        }
    }.init()
);
