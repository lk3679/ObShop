$(document).ready(function () {
    //F5 key lock
    $(this).keydown(function () {
        var k = window.event.keyCode;
        if (k == 116) {
            window.event.keyCode = 0;
            window.event.returnValue = false;
        }
    });
});
