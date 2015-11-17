$(document).ready(function () {
    var teachersHide = true;
    var usersHide = true;
    var queryT = true;
    var queryU = true;
    $('#users').hide();
    $('#teachers').hide();
    $('#tloading').hide();
    $('#uloading').hide();
    /*using ajax to load all users joined this course*/
    $('#addUsers').click(function (event) {
        event.preventDefault();
        $('#users').hide();
        $('#lWatched').val('true');
        var id = $('#ID').val();
        var url = '/Group/GetUsersForGroup/' + id;
        $('#users tbody > tr').remove();
        $('#uloading').show();
        if (queryU == true) {
            queryU = false;
            $.post(url, null, function (data) {
                $('#users tbody').append(data);
                $('#uloading').hide();
                $('#users').show();
                usersHide = true;
                $('#hideUsers').val('Зховати');
            });
            queryU = true;
        }
    });
    $('#addTeachers').click(function (event) {
        event.preventDefault();
        $('#tWatched').val('true');
        $('#teachers').hide();
        var id = $('#ID').val();
        var url = '/Group/GetUsersForGroup/' + id + '/Teacher';
        $('#teachers tbody > tr').remove();
        $('#tloading').show();
        if (queryT == true) {
            queryT = false;
            $.post(url, null, function (data) {
                $('#teachers tbody').append(data);
                $('#tloading').hide();
                $('#teachers').show();
                teachersHide = true;
                $('#hideTeachers').val('Зховати');
            });
            queryT = true;
        }
        $(this).prop('disabled',false);
    });
    $('#Course_ID').change(function (event) {
        $('#addUsers').trigger('click');
        $('#addTeachers').trigger('click');
    });
    
    $('#hideTeachers').click(function (event) {
        if (teachersHide) {
            $('#teachers').hide();
            $(this).val('Показати');
            teachersHide = false;
        }
        else {
            $('#teachers').show();
            $(this).val('Зховати');
            teachersHide = true;
        }
    });
    $('#hideLearners').click(function (event) {
        if (usersHide) {
            $('#users').hide();
            $(this).val('Показати');
            usersHide = false;
        }
        else {
            $('#users').show();
            $(this).val('Зховати');
            usersHide = true;
        }
    });

});