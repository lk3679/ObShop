<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pos_transaction.aspx.cs" Inherits="OBShopWeb.pos_transaction" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>橘熊門市系統</title>
     <script src="js/jquery-1.9.1.js"></script>
         <link href="css/jquery-ui.css" rel="stylesheet" />
    <link href="css/jquery-ui.theme.min.css" rel="stylesheet" />
    <script src="js/jquery-ui.min.js"></script>
  <link rel="Stylesheet" type="text/css" href="css/POS_style.css"/>
    <script type="text/javascript">
        var PosListString = '<%=PosList %>'

        $(function () {

            var Height = $(window).height() * 2 / 3;
            var Width = $(window).width() * 3 / 4;

            $("#OrderItem").dialog({
                open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                width: Width,
                height: Height,
                autoOpen: false,
                modal: true,
                buttons: {
                    "關閉": function () {
                        $(this).dialog("close");

                    }
                }
            });


            $("#StartDate").datepicker({ dateFormat: "yy/mm/dd" });
            $("#EndDate").datepicker({ dateFormat: "yy/mm/dd" });
            GetName();
            GetAllPosNo();


            $(".keyboicon_").click(function () {
                $(".KeyinFloat").show();
            });

            $(".ESC").click(function () {
                $("#TempInput").val("");
                $(".KeyinFloat").hide();
            });

            $(".number").click(function () {
                var input = $("#TempInput").val();
                var num = $(this).html();
                input += num;
                $("#TempInput").val(input);

            })

            $(".enter").click(function () {
                $("#OrderID").val($("#TempInput").val())
                $("#TempInput").val("");
                $(".KeyinFloat").hide();
            })

            $(".backspace").click(function () {
                var input = $("#TempInput").val();
                input = input.substring(0, input.length - 1);
                $("#TempInput").val(input);
            })

            $(document).on("click", ".OrderID", function () {
                var OrderID = $(this).attr("data-orderid");
                console.log("OrderID:"+OrderID)
                var url = "?act=GetOrderItem&OrderID=" + OrderID;
                $.get(url, function (data) {
                    var json = JSON.parse(data);
                    console.log(json)
                    if (json.result) {
                        $("#OrderItem").html(json.data);
                        $("#OrderItem").dialog('open');
                    }
                    else {
                        $("#OrderItem").html("查不到訂單明細");
                        $("#OrderItem").dialog('open');
                    }
                })
            })



            $(".logout").click(function () {
                var clerk = {};
                localStorage.setItem('clerk', JSON.stringify(clerk));
                window.location.href = "logout.aspx";
            });

            $(".SendQuery").click(function () {

                var OrderID = $("#OrderID").val();
                var StartDate = $("#StartDate").val();
                var EndDate = $("#EndDate").val();
                var ShowNullifiedInvoices = $("#ShowNullifiedInvoices").is(":checked");
                var PosNo = $("#PosNo").find('option:selected').val();
                var ClerkID = $("#ClerkID").find('option:selected').val();
                
                var PostData = {
                    OrderID: OrderID,
                    StartDate: StartDate,
                    EndDate: EndDate,
                    ShowNullifiedInvoices: ShowNullifiedInvoices,
                    PosNo: PosNo,
                    ClerkID: ClerkID
                }
                console.log(PostData)

                $.ajax({
                    type: 'POST',
                    url: "?act=Query",
                    data: PostData,
                    dataType: "json",
                    success: function (data) {
                        console.log(data);
                        $("#OrderTable").find(".OrderItem").remove();
                        $("#OrderTable").append(data.OrderList);
                    },
                    error: function (data) {

                    }
                });


            })

        })

        function GetName() {
            var LoginClerk = localStorage.getItem("clerk");
            if (LoginClerk == "{}" || LoginClerk == undefined) {
                window.location.href = "logout.aspx";
            } else {
                var clerk = JSON.parse(LoginClerk);
                ClerkNmae = clerk.name;
                ClerkID = clerk.id;
                //console.log(ClerkNmae)
                $("#ClerkNmae").html(ClerkNmae);
            }
        }

        function GetAllPosNo() {
            var PosList = JSON.parse(PosListString);
            var options = '<option value="" selected="selected">All</option>';
            for (var i = 0; i < PosList.length; i++) {
                options += '<option value="' + PosList[i].PosNo + '">POS' + PosList[i].PosNo + '</option>';
            }
            $("#PosNo").html(options);
        }
    </script>
</head>
<body>
    <div class="All">

        <!--最上面的頁面連結按鈕-->
        <div class="Top_Menu">
            <ul>
                <li><a href="pos_check_out.aspx">交易結帳</a></li>
                <li><a href="pos_stock_query.aspx">庫存查詢</a></li>
                <li><a href="pos_transaction.aspx">交易查詢</a></li>
                <li><a href="#">販促設定</a></li>
                <li><a href="index.aspx">管理頁面</a></li>
            </ul>
        </div>

        <!--左邊結帳細目-->
        <div class="BIG_list">

            <div class="SearchDIV">
                <ul>
                    <li>請輸入訂單編號：<input id="OrderID" type="text" size="30" /></li>
                    <li>請輸入交易日期：
                        <input id="StartDate" type="text" size="30" value="<%=currentTime.ToString("yyyy/MM/dd") %>"/>
                        ~<input id="EndDate" type="text" size="30"  value="<%=currentTime.ToString("yyyy/MM/dd") %>" /></li>
                    <li>進階篩選：<input type="checkbox" id="ShowNullifiedInvoices" />
                        只看已作廢發票   <span style="margin-left: 20px;">特定機台：</span>
                        <select id="PosNo">
                        </select>
                        <span style="margin-left: 20px;">特定收銀員：</span>
                        <select id="ClerkID">
                            <option value="" selected="selected">所有員工</option>                            
                            <%foreach(System.Data.DataRow row in AllStaff.Rows){ %>
                                 <option value="<%=row["id"].ToString() %>"><%=row["account"].ToString() %></option>
                            <%} %>
                           
                        </select>
                    </li>
                </ul>

            </div>



            <div class="SearchICON">
                <li class="keyboicon_">呼叫數字鍵盤</li>
                <li class="icon_proDetail SendQuery">送出查詢</li>
            </div>



            <div style="clear: both;"></div>
            <!--間容ie7的div浮動-->

            <div class="BIGpro_infoTIT">查詢結果</div>

            <div class="BIGpro_infoDT">
                <table  id="OrderTable" width="1000" border="0" cellspacing="0" cellpadding="0">
                    <tbody>
                        <tr>
                            <td style="width: 10%;">訂單編號</td>
                            <td style="width: 20%;">交易明細</td>
                            <td style="width: 6%;">銀行授權碼</td>
                            <td style="width: 10%;">交易方式</td>
                            <td style="width: 10%;">金額</td>
                            <td style="width: 10%;">發票號碼</td>
                            <td style="width: 6%;">狀態</td>
                            <td style="width: 6%;">機台</td>
                            <td style="width: 20%;">收銀員</td>
                        </tr>

                    </tbody>
                </table>

            </div>




            <div style="clear: both;"></div>
            <!--間容ie7的div浮動-->

            <div class="BIGbottom_info">
                <ul>
                    <li>登入者：<span id="ClerkNmae"></span> <span class="logout">登出</span>
                    </li>
                </ul>
            </div>

            <div>
            </div>

        </div>



    </div>


    <!--右邊鍵入區-->
        <div class="KeyinFloat" style="display:none;left:40%" >

            <ul>
                <li class="li-N"><input name="TextBoxEmail1" style="font-size:30px;" type="text" size="22" id="TempInput" /> </li>
                <li class="li-0 number">7</li>
                <li class="number">8</li>
                <li class="number">9</li>
                <li style="width: 70px;">─</li>
                <li style="width: 70px;">┼</li>
                <li class="li-0 number">4</li>
                <li class="number">5</li>
                <li class="number">6</li>
                <li style="width: 150px;" class="ESC">ESC</li>
                <li class="li-0 number">1</li>
                <li class="number">2</li>
                <li class="number">3</li>
                <li class="backspace"></li>
                <li class="li-0 number" style="width: 174px;">0</li>
                <li>‧</li>
                <li class="enter"></li>
            </ul>

        </div>
        <!--右邊鍵入區END-->
    <div id="OrderItem" style="display: none"></div>
</body>
</html>
