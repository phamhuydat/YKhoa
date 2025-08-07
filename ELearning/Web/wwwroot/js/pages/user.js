Dashmix.onLoad((() => class {
    static initValidation() {
        Dashmix.helpers("jq-validation"), jQuery(".form-add-user").validate({
            rules: {
                "mssv": {
                    required: !0,
                    number: true
                },
                "user_email": {
                    required: !0,
                    email: !0
                },
                "user_name": {
                    required: !0,
                },
                "user_gender": {
                    required: !0,
                },
                "datepicker": {
                    required: !0,
                },
                "user_nhomquyen": {
                    required: !0,
                },
                "user_password": {
                    required: !0,
                    minlength: 4
                },
            },
            messages: {
                "masv": {
                    required: "Vui lòng nhập mã sinh viên của bạn",
                    number: "Mã sinh viên phải là các ký tự số"
                },
                "user_email": {
                    required: "Vui lòng cung cấp email của bạn",
                    email: "Phải nhập đúng định dạng email"
                },
                "user_name": {
                    required: "Cung cấp đầy đủ họ tên",
                },
                "user_gender": {
                    required: "Tích chọn 1 trong 2",
                },
                "datepicker": {
                    required: "Vui lòng cho biết ngày sinh của bạn",
                },
                "user_nhomquyen": {
                    required: "Vui lòng chọn nhóm quyền",
                },
                "user_password": {
                    required: "Nhập mật khẩu",
                    minlength: "Mật khẩu phải có ít nhất 5 ký tự!"
                },
            }
        })
    }

    static init() {
        this.initValidation()
    }
}.init()));


