<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pos_day_end_report.aspx.cs" Inherits="OBShopWeb.pos_day_end_report" %>

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
        $(function () {
            $("#datepicker").datepicker({ dateFormat: "yy/mm/dd" });

            $("#datepicker").change(function () {
                SelectDate = $("#datepicker").val();
                var url = "?Date=" + SelectDate + "&PosNo="+PosNo;
                window.location = url;
            })

            $(".scroll").click(function () {
                var StartNo=$(this).attr("data-start");
                var EndNo=$(this).attr("data-end");
                var url = "day_end_report.aspx?PosNo=" + PosNo + "&StartNo=" + StartNo + "&EndNo=" + EndNo + "&Date=" + SelectDate;
                window.location.href = url;
            })

            $("#PosNo").change(function () {
                var PosNo = $("#PosNo").find('option:selected').val();
                SelectDate = $("#datepicker").val();
                var url = "?Date=" + SelectDate + "&PosNo=" + PosNo;
                window.location.href = url;
            })
        })

    </script>
</head>
<body>
    <form runat="server">
        <asp:Label ID="ResultLabel" runat="server" Text="" ForeColor="Red"></asp:Label><br/>  
         <asp:Button ID="PrintBtn" runat="server" Text="列印日結報表" OnClick="PrintBtn_Click" /> 
        <%if( ck.Name.Contains("店長")){ %>
        <a href="pos_day_end_report2.aspx" >查日結和月結</a>
        <%} %>
        <br/>  
    </form>

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
    日期：<input type="text" id="datepicker" value="<%=Date %>" /><br />
    <br />
    交易日期：<%=Date %><br />
    店號：<%=StoreNo %> &nbsp;&nbsp;&nbsp;	&nbsp;&nbsp;&nbsp;	       機號：<%=PosNo=="All"?"全部":PosNo %>
    <br />
    ----------------------------<br />
    收銀員名稱：<%=ck.Name %><br />
    <br />

    發票銷售額  ： <%=TotalOrderAmount  %><br />
    應稅銷售額   ： 0<br />
    免稅銷售額  ： 0<br />
    總稅額       ：0<br />
    <br />

    開始交易序號 : <%=StartOrderID %><br />
    結束交易序號 : <%=EndOrderID %><br />
    銷售總筆數   : <%=TotalOrderNum %><br />
    銷售總件數   : <%=TotalQuantity %><br />
    <br />

    現金筆數      ： <%=TotalCashOrderNum %><br />
    現金收入      ： <%=CashIncome %><br />
    現金折讓       ：<%=AllowancesCashAmount %><br />
    信用卡筆數   ：<%=TotalCardOrderNum %><br />
    信用卡收入   ：<%=CardIncome %><br />
    信用卡折讓   ：<%=AllowancesCardAmount %><br />
    實收&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ：&nbsp;<%=(CashIncome+CardIncome)-ReturnTotalAmount  %><br />
    <%foreach (OBShopWeb.Poslib.InvoiceRoll IR in InvoiceList){ %>
    開始發票號碼：<%=IR.StartInvoice %><br />
    結束發票號碼：<%=IR.EndInvoice %><br />
    現金&nbsp;&nbsp;&nbsp; ： <%=IR.Cash %><br />
    信用卡： <%=IR.Credit %><br />
    小計&nbsp;&nbsp;&nbsp; ：&nbsp;<%=IR.Cash+IR.Credit  %><br />
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
