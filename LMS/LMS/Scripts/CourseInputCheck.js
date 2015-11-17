$(document).ready(function () {
    $('#courseForm').validate({
        rules: {
            Title: {
                required: true,
                minlength: 3
            },
            fileSite: {
                url: true
            },
            Description: {
                required: true,
                minlength: 10
            },
           
            tags: {
                required: true
            }
        },

        messages: {

            Title: {
                required: "This field is empty!",
                minlength: "Minimal length: 5"
            },
            fileSite: {
                url: "Not valid url"
            },
            Description: {
                required: "This field is empty!",
                minlength: "Minimal length: 10",
            },
            
            tags: {
                required: "This field is empty!"
            }
        }
    });
});