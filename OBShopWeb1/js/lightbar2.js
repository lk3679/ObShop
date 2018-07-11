var d2;

$("document").ready(function () {
    var color2;
    // 指定滑鼠移入時的觸發事件
    $("tr").not(":first").mouseover(function () {
        color2 = $(this).css("background-color")
        $(this).css("background", d2);
    });

    // 指定滑鼠移出時的觸發事件
    $("tr").not(":first").mouseout(function () {
        $(this).css("background", color2);
    });
});