var c1, c2;

$("document").ready(function(){
    var name = "";
    var all = $(".style5menu");
    //點擊
    $(".style5menu").click(function () {
        $(".style5menu").css("background", "");
        $(this).css("background", c1);

        name = this.id;
    });

    //預設
    $('div a').css('font-size', 'large');

    //滑鼠指到
    $('div a').mouseover(function () {
        //找選到的那個設成有底色
        checkslect();

        $(this).css('font-size', 'x-large');
        $(this).css("background", c1);
    });

    //滑鼠移開
    $('div a').mouseout(function () {
        $('div a').css('font-size', 'large');
        $(this).css("background", "");
        //找選到的那個設成有底色
        checkslect();
    });

    //找選到的那個設成有底色
    function checkslect() {
        for (var vi = 0; vi < all.length; vi++) {
            if (name == all[vi].id) {
                $(all[vi]).css("background", c2);
                $(all[vi]).css('font-size', 'x-large');
            }
        }
    }
});