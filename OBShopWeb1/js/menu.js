var c1, c2;

$("document").ready(function(){
    var name = "";
    var all = $(".style5menu");
    //�I��
    $(".style5menu").click(function () {
        $(".style5menu").css("background", "");
        $(this).css("background", c1);

        name = this.id;
    });

    //�w�]
    $('div a').css('font-size', 'large');

    //�ƹ�����
    $('div a').mouseover(function () {
        //���쪺���ӳ]��������
        checkslect();

        $(this).css('font-size', 'x-large');
        $(this).css("background", c1);
    });

    //�ƹ����}
    $('div a').mouseout(function () {
        $('div a').css('font-size', 'large');
        $(this).css("background", "");
        //���쪺���ӳ]��������
        checkslect();
    });

    //���쪺���ӳ]��������
    function checkslect() {
        for (var vi = 0; vi < all.length; vi++) {
            if (name == all[vi].id) {
                $(all[vi]).css("background", c2);
                $(all[vi]).css('font-size', 'x-large');
            }
        }
    }
});