var d1;

$("document").ready(function () {
    var color1;
    // 指定滑鼠移入時的觸發事件
    $("tr").children().not("th").parent().mouseover(function () {
        color1 = $(this).css("background-color")
        $(this).css("background", d1);
    });

    // 指定滑鼠移出時的觸發事件
    $("tr").children().not("th").parent().mouseout(function () {
        $(this).css("background", color1);
    });
});
