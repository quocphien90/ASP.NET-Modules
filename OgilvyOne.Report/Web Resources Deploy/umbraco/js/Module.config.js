$(document).ready(function () {

    // Check all checkboxes in GridView when the one in a table head is checked:
    $('#tableGrid').find(".check-all").click(function () {
	        //alert("test");
	        $(this).parent().parent().parent().parent().find("input[type='checkbox']").attr('checked', $(this).is(':checked'));
	    }
    );
    //$(".LanguageControl a").on('click', function () {
    //    var langID = $(this).attr("title");
    //    alert(langID);
    //    $('#' + langID).css("display", "block");
    //});

    // click change tabs in Multi Lang controls
    //$('#right').contents().find('.LangButton').click(function () {
    $('.LangButton').click(function () {
        //alert("asdasdd");
        var idTab = $(this).attr('title');
        $('.tab-content').hide();
        //alert(idTab);
        $('#' + idTab).show();

        $('.LangButton').removeClass('current');
        $(this).addClass('current');
    });

});