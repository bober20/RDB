setPagerListener();
setVacancyNumber();
setVacancyDescription();
function setPagerListener() {
    $('input.admin-checkbox').unbind();
    $('input.admin-checkbox').on('change', function (e) {
        e.preventDefault();
        var userId = $(this).data('user-id');
        var action = !!$(this).is(':checked');
        var role = $(this).data('role');
        var data = {
            userId: userId,
            action: action,
            role: role
        };
        $.ajax({
            url: "https://localhost:7130/Admin/update/",
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (data) {
                console.log(data);
            },
            error: function (xhr, status, error) {
                console.log('Error: ' + error);
            }
        });
    });

    $(document).ajaxComplete(setPagerListener);
}

function setVacancyNumber() {
    $('input.vacancy-number').unbind();

    $('input.vacancy-number').on('blur', function (e) {
        e.preventDefault();

        var vacancyId = $(this).data('vacancy-id');
        var newVacancyNumber = $(this).val();

        var data = {
            vacancyId: vacancyId,
            vacancyNumber: newVacancyNumber
        };

        $.ajax({
            url: "https://localhost:7130/Positions/UpdateVacancyNumber/",
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                console.log('Vacancy updated: ' + data.vacancyId + ' ' + data.vacancyNumber);
            },
            error: function (xhr, status, error) {
                console.log('Error:', error);
            }
        });
    });

    $(document).ajaxComplete(setVacancyNumber);
}

function setVacancyDescription() {
    $('input.vacancy-description').unbind();

    $('input.vacancy-description').on('blur', function (e) {
        e.preventDefault();

        var vacancyId = $(this).data('vacancy-id');
        var newVacancyDescription = $(this).val();

        var data = {
            vacancyId: vacancyId,
            vacancyDescription: newVacancyDescription
        };

        $.ajax({
            url: "https://localhost:7130/Positions/UpdateVacancyDescription/",
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                console.log('Vacancy updated: ' + data.vacancyId + ' ' + data.vacancyNumber);
            },
            error: function (xhr, status, error) {
                console.log('Error:', error);
            }
        });
    });

    $(document).ajaxComplete(setVacancyDescription);
}
