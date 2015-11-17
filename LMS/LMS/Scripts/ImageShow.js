function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#blah')
              .attr('src', e.target.result);
              
        };
        reader.readAsDataURL(input.files[0]);
    }
}
function readURLText(input) {
    if (input.length != 0) {
        $('#blah')
          .attr('src', input);
              
    }
}