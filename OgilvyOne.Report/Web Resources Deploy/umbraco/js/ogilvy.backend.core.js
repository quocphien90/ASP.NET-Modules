$(function () {
    var findCon = $('[og-find="true"]');
    var cmd = findCon.find('[og-find-command="true"]');
    findCon.find('input[name]').keypress(function (event) {
        if (event.which == 13) {
            cmd.click();
            event.preventDefault();
        }
    });
    cmd.click(function () {
        var param = {};
        findCon.find('input[name]').each(function () {
            param[$(this).attr('name')] = $(this).val();
        });
        findCon.find('select[name]').each(function () {
            param[$(this).attr('name')] = $(this).val();
        });
        window.location = $(this).attr('href') + "?" + $.param(param);
    });

    findCon.find('div.umbDateTimePicker').each(function () {
        var divparent = $(this).parent();
        $(this).find('input').attr('name', divparent.attr('date-name'));
    });

    findCon.find('select[d2-selected]').each(function () {
        $(this).val($(this).attr('d2-selected'));
    });
    findCon.find('select[selected-value]').each(function () {
        $(this).val($(this).attr('selected-value'));
    });

    var paging = $('[og-paging="true"]');
    paging.find('[og-data="button"]').click(function () {
        var href = $(this).attr('href');
        var page = paging.find('[og-data="page"]').val();
        window.location = href + page;
    });
});
