<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pos_checkout_client.aspx.cs" Inherits="OBShopWeb.pos_checkout_client" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>消費者畫面</title>
    <link href="css/POS_style.css" rel="stylesheet" />
    <link href="css/POS_Consumer.css?v=<%=DateTime.Now.ToString("yyyyMMddHHmm") %>" rel="stylesheet" />
    <script src="js/jquery-1.9.1.js"></script>

    <script src="js/jquery.signalR-2.0.3.js"></script>
    <script src="http://localhost:8080/signalr/hubs"></script>

    <script type="text/javascript">
        var Version = '<%=DateTime.Now.ToString("yyyyMMddHHmm") %>';

        $(function () {
            ImageUrl = "url(./Image/pos_for_ConsumerBG.jpg?v=" + Version + ")";
            $(".Consumer").css("background-image", ImageUrl);
            TableInitialization();
            SignalRInitialization();
        })

        function SignalRInitialization() {

            $.connection.hub.url = "http://localhost:8080/signalr";
            PosManager = $.connection.myHub;
            $.connection.hub.start();

            PosManager.client.getCheckOutInfo = function (CheckOutInfoString) {

                if (CheckOutInfoString == "{}" || CheckOutInfoString == undefined) {
                    return;
                }

                var CheckOutInfo = JSON.parse(CheckOutInfoString);
                $(".AllQuantity").html(CheckOutInfo.AllQuantity);
                $(".TotalAmount").html(CheckOutInfo.TotalAmount);
                $(".OriginalTotalAmount").html(CheckOutInfo.OriginalTotalAmount)
                $(".received").html(CheckOutInfo.received);
                $(".change").html(CheckOutInfo.change);
                TableInitialization();
                var ItemList = CheckOutInfo.ItemList;
                $(".ItmeList").html($(".ItmeList").html() + ItemList);
                $("br").remove();

                if (CheckOutInfo.condition == "buy") {
                    $(".ItmeList tr").each(function () {
                        if ($(this).find("strike").html() != undefined) {
                            var OrginlAmount = $(this).find("strike").html();
                            var amount = $(this).find(".amount").html();
                            if (OrginlAmount != amount) {
                                $(this).find("strike").show();
                                $(this).find(".amount").css("color", "red");
                            }
                        }
                    })

                    $(".change_msg").html("找零");
                    $(".OriginalTotalAmountMsg").show();
                    $(".TotalAmountMsg").html("折扣後金額");
                }

                if (CheckOutInfo.condition == "return") {
                    $(".change_msg").html("退款");
                    $(".OriginalTotalAmountMsg").hide();
                    $(".TotalAmountMsg").html("應收金額");
                }

                //超過7筆之後只顯示後7筆
                HideItmeList();
            }

        }

        function TableInitialization() {
            var TableTitle = '<tr>';
            TableTitle += '<td class="tit_list" style="width: 280px; background-color: #FDF6AB;">款名</td>';
            TableTitle += '<td class="tit_list" style="width: 50px; background-color: #FDF6AB;">顏色</td>';
            TableTitle += '<td class="tit_list" style="width: 50px; background-color: #FDF6AB;">尺寸</td>';
            TableTitle += '<td class="tit_list" style="width: 50px; background-color: #FDF6AB;">數量</td>';
            TableTitle += ' <td class="tit_list" style="width: 100px; background-color: #FDF6AB;">金額</td>';
            TableTitle += '</tr>';
            $(".ItmeList").html(TableTitle);
        }

        function HideItmeList() {
            var RowNum = $(".ItmeList tr").length;
            //console.log("商品項：" + RowNum);
            if (RowNum > 7) {
                for (var i = 0; i < RowNum - 7; i++) {
                    if (i > 0) {
                        $(".ItmeList").find("tr:eq(" + i + ")").hide();
                    }
                }
            }
        }

    </script>
</head>

<body>
  <div class="Consumer">

      <!--價格-->
      <div class="ShowPrice">
          <table width="100%" border="0" cellspacing="0" cellpadding="0">
              <tbody>
                  <tr >
                      <td height="260" valign="top">
                          <table class="ItmeList" width="100%" border="0" cellspacing="0" cellpadding="0">
                              <tbody>
                                  
                              </tbody>
                          </table>


                      </td>
                  </tr>
                  <tr>
                      <td height="115" valign="middle" style="border-top: 1px solid #3E3E3E;">
                          <table width="100%" border="0" cellspacing="0" cellpadding="0">
                              <tbody>
                                  <tr>
                                      <td width="60%" align="right"><div class="OriginalTotalAmountMsg">商品總金額：<span class="allprc OriginalTotalAmount">0</span>元</div></td>
                                      <td width="40%" align="right">總數：<span class="Prcnob AllQuantity">0</span>件</td>
                                  </tr>
                                  <tr>
                                      <td align="right"><span class="TotalAmountMsg">折扣後金額</span>：<span class="allprc TotalAmount">0</span>元</td>
                                      <td width="40%" align="right">已收金額：<span class="Prcnob received">0</span>元</td>
                                  </tr>
                                  <tr>
                                      <td align="right"></td>
                                      <td width="40%" align="right"><span class="change_msg">找零</span>：<span class="Prcnob change">0</span>元</td>
                                  </tr>
                              </tbody>
                          </table>
                      </td>
                  </tr>
              </tbody>
          </table>

      </div>

  </div>
    
</body>
</html>