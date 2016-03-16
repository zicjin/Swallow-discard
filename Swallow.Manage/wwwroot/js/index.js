$(function () {
    $("select.sel-query").on("change", function (e) {
        var parsed = queryUri.parse(location.search);
        var key = $(this).attr("name").split('_')[1];
        parsed[key] = $(this).val();
        location.search = queryUri.stringify(parsed);
    })
})