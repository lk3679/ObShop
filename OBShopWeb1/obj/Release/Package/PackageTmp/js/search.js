$(document).ready(function () {
    //backspace key lock
    $(this).keydown(function () {
        var k = window.event.keyCode;
        if (k == 8) {
            window.event.keyCode = 0;
            window.event.returnValue = false;
        }
    });

    //key¦r·j´M
    var keyinTemp = '';
    $(this).keypress(function (ex) {
        if (ex.keyCode == '13' || ex.keyCode == '32') {
            keyinTemp = '';
            return;
        }
        keyinTemp += String.fromCharCode(ex.keyCode);
        var shippings = $("table tr td a:contains('" + keyinTemp + "')");
        if (shippings.length > 0)
            shippings[0].focus();
    });
});
