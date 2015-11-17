$(document).ready(function () {
    $('#courseForm').validate({
        rules: {
            'CourseElement.Title': {
                required: true,
                minlength: 3
            },
            fileSite: {
                url: true
            },
            'CourseElement.Description': {
                required: true,
                minlength: 10
            },
           
            tags: {
                required: true
            }
        },

        messages: {

            'CourseElement.Title': {
                required: "This field is empty!",
                minlength: "Minimal length: 3"
            },
            fileSite: {
                url: "Not valid url"
            },
            'CourseElement.Description': {
                required: "This field is empty!",
                minlength: "Minimal length: 10",
            },
            
            tags: {
                required: "This field is empty!"
            }
        }
    });
});