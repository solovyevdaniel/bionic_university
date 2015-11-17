/*adding teacher to teachers group in course management*/

    $('.teacherButton').click(function (event) {
        var currentTr = $(this).closest('tr');
        currentTr.remove();

        var input = currentTr.find('td:last').find('input');
        input.attr('name', 'teachers');

        var teachersTable = $('#teachersTable');
        teachersTable.append(currentTr);
    });


    $('.studentButton').click(function (event) {
        
        var currentTr = $(this).closest('tr');
        currentTr.remove();

        var input = currentTr.find('td:last').find('input');
        input.attr('name', 'students');

        var studentsTable = $('#studentsTable');
        studentsTable.append(currentTr);

    });

    $('.userButton').click(function (event) {

        var currentTr = $(this).closest('tr');
        currentTr.remove();

        var input = currentTr.find('td:last').find('input');
        input.attr('name', 'users');

        var usersTable = $('#usersTable');
        usersTable.append(currentTr);
    });

