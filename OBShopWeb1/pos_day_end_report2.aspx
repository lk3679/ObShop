<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pos_day_end_report2.aspx.cs" Inherits="OBShopWeb.pos_day_end_report2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <script src="js/jquery-1.9.1.js"></script>
    <link href="css/jquery-ui.css" rel="stylesheet" />
    <link href="css/jquery-ui.theme.min.css" rel="stylesheet" />
    <script src="js/jquery-ui.min.js"></script>
    <script type="text/javascript">

        var SelectDate = '<%=Date %>';
        var PosNo = '<%=PosNo %>';
        var Month = '<%=Month %>';

        $(function () {

            if (Month.length > 1) {
                $("#MonthSpan").show();
                $("#DateSpan").hide();
            } else {
                $("#MonthSpan").hide();
                $("#DateSpan").show();
            }

            $("#datepicker").datepicker({ dateFormat: "yy/mm/dd" });

            $("#datepicker").change(function () {
                DateQuery();
            })

            $("#PosNo").change(function () {
                PosNo = $("#PosNo").find('option:selected').val();

                if (Month.length > 1) {
                    MonthQuery();
                } else {
                    DateQuery();
                }
            })

            $("#monpicker").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: "yy/mm/"
            }).focus(function () {
                var thisCalendar = $(this);
                $('.ui-datepicker-calendar').detach();
                $('.ui-datepicker-close').click(function () {
                    var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    thisCalendar.datepicker('setDate', new Date(year, month, 1));
                    MonthQuery();
                });
            });

            $("#MonthQuery").click(function () {
                $("#MonthSpan").show();
                $("#DateSpan").hide();
                MonthQuery();

            })

            $("#DateQuery").click(function () {
                $("#DateSpan").show();
                $("#MonthSpan").hide();
                DateQuery();
            })
        })

        function DateQuery() {
            SelectDate = $("#datepicker").val();
            var url = "?Date=" + SelectDate + "&PosNo=" + PosNo;
            window.location = url;
        }


        function MonthQuery() {
            SelectMon = $("#monpicker").val();
            var url = "?Month=" + SelectMon + "&PosNo=" + PosNo;
            window.location.href = url;
        }

    </script>
</head>
<body>
    POS機：
    <select id="PosNo">
        <option value="All">全部</option>
        <%foreach (System.Data.DataRow row in PosDT.Rows)
          { %>
        <option value="<%=row["PosNo"].ToString() %>" <%if (PosNo == row["PosNo"].ToString())
                                                        { %> selected="selected" <%} %>><%=row["PosNo"].ToString() %></option>
        <% } %>
    </select><br />
    <br />
    <span id="DateSpan"  style="display:none">日期：<input type="text" id="datepicker" value="<%=Date %>" /><br />
         <a href="#" id="MonthQuery"> 月結查詢</a>
    </span>
   
    <span id="MonthSpan"  style="display:none" >月份：<input type="text" id="monpicker" value="<%=Month==""?Date.Substring(0,8):Year+"/"+Month %>" />
        <br /> <a href="#"  id="DateQuery"> 日結查詢</a>
    </span>
   
    <br /><br />
    交易日期：<%=Month==""?Date:Year+"/"+Month %><br />
    店號：<%=StoreNo %> &nbsp;&nbsp;&nbsp;	&nbsp;&nbsp;&nbsp;	       機號：<%=PosNo=="All"?"全部":PosNo %>
    <br />
    <hr />
    收銀員名稱：<br />
    <br />

    發票銷售額  ： <%=TotalOrderAmount  %><br />
    應稅銷售額   ： 0<br />
    免稅銷售額  ： 0<br />
    總稅額       ：0<br />
    <br />

    開始交易序號 : <%=StartOrderID %><br />
    結束交易序號 : <%=EndOrderID %><br />
    銷售總筆數     : <%=TotalOrderNum %><br />
    銷售總件數     : <%=TotalQuantity %><br />
    <br />

    現金筆數       ： <%=TotalCashOrderNum %><br />
    現金收入       ： <%=CashIncome %><br />
    現金折讓       ：<%=AllowancesCashAmount %><br />
    信用卡筆數   ：<%=TotalCardOrderNum %><br />
    信用卡收入   ：<%=CardIncome %><br />
    信用卡折讓   ：<%=AllowancesCardAmount %><br />
     <hr />
    <%foreach (OBShopWeb.Poslib.InvoiceRoll IR in InvoiceList)
      { %>
    開始發票號碼：<%=IR.StartInvoice %><br />
    結束發票號碼：<%=IR.EndInvoice %><br />
    現金&nbsp;&nbsp;&nbsp; ： <%=IR.Cash %><br />
    信用卡： <%=IR.Credit %><br />
    小計&nbsp;&nbsp;&nbsp; ：&nbsp;<%=IR.Cash+IR.Credit  %><br />
     <hr />
    <% } %>
    
    <br />

    現金作廢<br />
    發票號碼&nbsp;&nbsp;&nbsp;	  &nbsp;&nbsp;&nbsp;     金額<br />
    <%foreach (OBShopWeb.Poslib.Order.FailInvoice inv in FailCashInvoiceList)
      { %>
    <%=inv.InvoiceNo %>&nbsp;&nbsp;&nbsp;		<%=inv.InvoiceNoAmount %><br />
    <%} %><br />

    現金作廢金額   : <%=ReturnTotalCashAmount %><br />
    <br />
    現金折讓<br />
    發票號碼&nbsp;&nbsp;&nbsp;	  &nbsp;&nbsp;&nbsp;     金額<br />
     <%foreach (OBShopWeb.Poslib.Order.FailInvoice inv in AllowancesCashList)
      { %>
    <%=inv.InvoiceNo %>&nbsp;&nbsp;&nbsp;		<%=inv.InvoiceNoAmount %><br />
    <%} %><br />
    <br />
    信用卡作廢<br />
    發票號碼&nbsp;&nbsp;&nbsp;	  &nbsp;&nbsp;&nbsp;     金額<br />
    <%foreach (OBShopWeb.Poslib.Order.FailInvoice inv in FailCardInvoiceList)
      { %>
    <%=inv.InvoiceNo %>&nbsp;&nbsp;&nbsp;		<%=inv.InvoiceNoAmount %><br />
    <%} %><br />

    信用卡作廢金額   :  <%=ReturnTotalCardAmount %><br />
    <br />

    信用卡折讓<br />
    發票號碼&nbsp;&nbsp;&nbsp;	  &nbsp;&nbsp;&nbsp;     金額<br />
     <%foreach (OBShopWeb.Poslib.Order.FailInvoice inv in AllowancesCardList)
      { %>
    <%=inv.InvoiceNo %>&nbsp;&nbsp;&nbsp;		<%=inv.InvoiceNoAmount %><br />
    <%} %><br />
    <br />

    退貨總金額   : <%=ReturnTotalAmount  %><br />
    <br />

    其他作廢發票<br />
    發票號碼&nbsp;&nbsp;&nbsp;	  &nbsp;&nbsp;&nbsp;     金額<br />
    <%foreach (OBShopWeb.Poslib.Order.FailInvoice inv in FailOhterInvoiceList)
      { %>
    <%=inv.InvoiceNo %>&nbsp;&nbsp;&nbsp;		<%=inv.InvoiceNoAmount %><br />
    <%} %><br />

    發票更正總筆數,張數: <%=FailOrderNum %>,<%=FailInvoiceNum %>
</body>
</html>
