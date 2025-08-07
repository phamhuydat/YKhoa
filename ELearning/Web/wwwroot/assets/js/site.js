function jqValidation() {
    jQuery.validator.setDefaults({
        errorClass: "invalid-feedback animated fadeIn",
        errorElement: "div",
        onkeyup: function (element) { $(element).valid(); },   // Validate each keystroke
        onfocusout: function (element) { $(element).valid(); }, // Validate when focus is lost
        onchange: function (element) { $(element).valid(); },   // Validate on change
        errorPlacement: (e, t) => {
            jQuery(t).addClass("is-invalid"),
                jQuery(t).parents("div:not(.input-group)").first().append(e);
        },
        highlight: (e) => {
            jQuery(e)
                .parents("div:not(.input-group)")
                .first()
                .find(".is-invalid")
                .removeClass("is-invalid")
                .addClass("is-invalid");
        },
        success: (e) => {
            jQuery(e)
                .parents("div:not(.input-group)")
                .first()
                .find(".is-invalid")
                .removeClass("is-invalid"),
                jQuery(e).remove();
        },
    }),
        jQuery.validator.addMethod(
            "emailWithDot",
            function (e, t) {
                return (
                    this.optional(t) ||
                    /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i.test(
                        e
                    )
                );
            },
            "Please enter a valid email address"
        );
}

function showNotification(options = {}) {
    if ($.isEmptyObject(options)) {
        // Initialize click events for elements with .js-notify if no options are provided
        $(".js-notify:not(.js-notify-enabled)").each((index, element) => {
            $(element)
                .addClass("js-notify-enabled")
                .on("click", (e) => {
                    let $target = $(e.currentTarget);
                    $.notify({
                        icon: $target.data("icon") || "",
                        message: $target.data("message"),
                        url: $target.data("url") || "",
                    }, {
                        element: "body",
                        type: $target.data("type") || "info",
                        placement: {
                            from: $target.data("from") || "top",
                            align: $target.data("align") || "right",
                        },
                        allow_dismiss: true,
                        newest_on_top: true,
                        showProgressbar: false,
                        offset: 20,
                        spacing: 10,
                        z_index: 1033,
                        delay: 2000,
                        timer: 1000,
                        animate: {
                            enter: "animated fadeIn",
                            exit: "animated fadeOutDown",
                        },
                        template: `
                            <div data-notify="container" class="col-11 col-sm-4 alert alert-{0} alert-dismissible" role="alert">
                                <p class="mb-0">
                                    <span data-notify="icon"></span>
                                    <span data-notify="title">{1}</span>
                                    <span data-notify="message">{2}</span>
                                </p>
                                <div class="progress" data-notify="progressbar">
                                    <div class="progress-bar progress-bar-{0}" role="progressbar" style="width: 0%;"></div>
                                </div>
                                <a href="{3}" target="{4}" data-notify="url"></a>
                                <button type="button" class="close" data-notify="dismiss" aria-label="Close">
                                    <i class="fa fa-times"></i>
                                </button>
                            </div>
                        `,
                    });
                });
        });
    } else {
        // Show notification with provided options
        $.notify({
            icon: options.icon || "",
            message: options.message,
            url: options.url || "",
        }, {
            element: options.element || "body",
            type: options.type || "info",
            placement: {
                from: options.from || "top",
                align: options.align || "right",
            },
            allow_dismiss: options.allow_dismiss !== false,
            newest_on_top: options.newest_on_top !== false,
            showProgressbar: !!options.show_progress_bar,
            offset: options.offset || 20,
            spacing: options.spacing || 10,
            z_index: options.z_index || 1033,
            delay: options.delay || 2000,
            timer: options.timer || 2000,
            animate: {
                enter: options.animate_enter || "animated fadeIn",
                exit: options.animate_exit || "animated fadeOutDown",
            },
            template: `
                <div data-notify="container" class="col-11 col-sm-4 alert alert-{0} alert-dismissible" role="alert">
                    <p class="mb-0">
                        <span data-notify="icon"></span>
                        <span data-notify="title">{1}</span>
                        <span data-notify="message">{2}</span>
                    </p>
                    <div class="progress" data-notify="progressbar">
                        <div class="progress-bar progress-bar-{0}" role="progressbar" style="width: 0%;"></div>
                    </div>
                    <a href="{3}" target="{4}" data-notify="url"></a>
                </div>
            `,
        });
    }
}

