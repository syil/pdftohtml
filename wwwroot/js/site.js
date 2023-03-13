// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function() {
    $('#upload-form').on("submit", function(e) {
        if ($('#file-select')[0].files.length > 0) {
            var file = $('#file-select')[0].files[0];
            
            const reader = new FileReader();
            reader.addEventListener("loadend", function() {
                var base64Str = this.result.substr(this.result.lastIndexOf(",") + 1, this.result.length);
                
                $.post("/home/pdffile", { html: base64Str }).done(function(data) {
                    $("#pdf-view").attr("src", data);
                    e.target.reset();
                });
            });
            reader.readAsDataURL(file);
            
            $('#pdf-view').attr("src", "data:text/html; charset=utf-8,<h1 class='text-center'>Yükleniyor</div>");
        }

        e.preventDefault();
        return false;
    });

    $('#reset-button').on("click", function () {
        $("#pdf-view").attr("src", "about:blank");
    });
});