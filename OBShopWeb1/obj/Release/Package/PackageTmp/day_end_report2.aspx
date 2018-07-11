<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="day_end_report2.aspx.cs" Inherits="OBShopWeb.day_end_report2" %>

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
        $(function () {
            $("#datepicker").datepicker({ dateFormat: "yy/mm/dd" });

            $("#datepicker").change(function () {
                var url = "?date="+$("#datepicker").val();
                window.location = url;
            })
        })

    </script>
</head>
<body>
    <a href="javascript:history.back();">返回選單</a>
    <form runat="server">
        <asp:Label ID="ResultLabel" runat="server" Text="" ForeColor="Red"></asp:Label><br/>  
        <br/>  
    </form>
   
交易日期：<%=Date %><br/>
店號：<%=StoreNo %> &nbsp;&nbsp;&nbsp;	&nbsp;&nbsp;&nbsp;	       機號：<%=PosNo=="All"?"全部":PosNo %>  <br/>  
----------------------------<br/>
收銀員名稱：<br/><br/>
   
發票銷售額   : <%=TotalOrderAmount  %><br/>
應稅銷售額   : 0<br/>
免稅銷售額   : 0<br/>
總稅額       : 0<br/><br/>

開始交易序號 : <%=StartOrderID %><br/>
結束交易序號 : <%=EndOrderID %><br/>
銷售總筆數   : <%=TotalOrderNum %><br/>
銷售總件數   :  <%=TotalQuantity %><br/><br/>

現金筆數      : <%=TotalCashOrderNum %><br/>
現金收入      : <%=CashIncome %><br/>
信用卡筆數   :<%=TotalCardOrderNum %><br/>
信用卡收入   : <%=CardIncome %><br/>

開始發票號碼 : <%=StartInvoiceNo %><br/>
結束發票號碼 : <%=EndInvoiceNo %><br/><br/>

現金作廢<br/>
發票號碼&nbsp;&nbsp;&nbsp;	  &nbsp;&nbsp;&nbsp;     金額<br/>
  <%foreach(OBShopWeb.Poslib.Order.FailInvoice inv in FailCashInvoiceList){ %>
<%=inv.InvoiceNo %>&nbsp;&nbsp;&nbsp;		<%=inv.InvoiceNoAmount %><br/>
 <%} %><br/>

現金作廢     : <%=ReturnTotalCashAmount %><br/><br/>

信用卡作廢<br/>
發票號碼&nbsp;&nbsp;&nbsp;	  &nbsp;&nbsp;&nbsp;     金額<br/>
  <%foreach(OBShopWeb.Poslib.Order.FailInvoice inv in FailCardInvoiceList){ %>
<%=inv.InvoiceNo %>&nbsp;&nbsp;&nbsp;		<%=inv.InvoiceNoAmount %><br/>
 <%} %><br/>

信用卡作廢   :  <%=ReturnTotalCardAmount %><br/><br/>

退貨總金額   : <%=ReturnTotalAmount  %><br/><br/>

其他作廢發票<br/>
發票號碼&nbsp;&nbsp;&nbsp;	  &nbsp;&nbsp;&nbsp;     金額<br/>
  <%foreach (OBShopWeb.Poslib.Order.FailInvoice inv in FailOhterInvoiceList)
    { %>
<%=inv.InvoiceNo %>&nbsp;&nbsp;&nbsp;		<%=inv.InvoiceNoAmount %><br/>
 <%} %><br/>

發票更正總筆數,張數: <%=FailOrderNum %>,<%=FailInvoiceNum %>
</body>
</html>
