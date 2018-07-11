<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceAudit.aspx.cs" Inherits="OBShopWeb.InvoiceAudit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>    
    <style type="text/css">
        .colTextWrap {
            text-wrap: normal;
        }
        .colHeader3 {
            width: 12ex;
        }
        .colHeader4 {
            width: 14ex;
        }
        .colHeaderTime {
            width: 22ex;
        }
        .colTextAlignCenter {
            text-align: center;
        }
        .colTextAlignRight {
            text-align: right;
        }
        table.tablesorter thead tr .header {
            background-image: url(Image/tablesorter/bg.gif);
            background-repeat: no-repeat;
            background-position: center right;
            cursor: pointer;
        }
        table.tablesorter thead tr .headerSortUp {
            background-image: url(Image/tablesorter/asc.gif);
        }
        table.tablesorter thead tr .headerSortDown {
            background-image: url(Image/tablesorter/desc.gif);
        }
    </style>
    <script src="js/jquery-1.9.1.js"></script>
    <link href="css/jquery-ui.css" rel="stylesheet" />
    <link href="css/jquery-ui.theme.min.css" rel="stylesheet" />
    <script src="js/jquery-ui.min.js"></script>
    <script src="js/jquery.tablesorter.js"></script>
    <script type="text/javascript">
        $(function () {
            $("#txtStartDate").datepicker({ dateFormat: "yy/mm/dd" });
            $("#txtEndDate").datepicker({ dateFormat: "yy/mm/dd" });
            $("#GridView1").tablesorter();
        })
    </script>
</head>
<body>
    <form id="form1" runat="server">
        訂單日期：<input type="text" id="txtStartDate" name="StartDate" runat="server"/>～<input type="text" id="txtEndDate" name="EndDate" runat="server"/>
        <br />
        <br />
        訂單編號：<asp:TextBox ID="txtOrderIDStart" runat="server"></asp:TextBox>～<asp:TextBox ID="txtOrderIDEnd" runat="server"></asp:TextBox>
        <br />
        <br />
        發票（捲）：<asp:DropDownList ID="ddlInvoiceRoll" runat="server"></asp:DropDownList>
        <div style="display:inline-block; width: 300px"></div>
        <asp:Button ID="btnQuery" runat="server" Text="查詢" OnClick="btnQuery_Click" />
        <br />
        <br />        
    <div>
        <asp:GridView ID="GridView1" runat="server" CssClass="tablesorter" EnableViewState="false" AutoGenerateColumns="false" OnPreRender="GridView1_PreRender">
            <Columns>
                <asp:BoundField DataField="InvoiceNo" HeaderText="發票號碼" ItemStyle-CssClass="colHeader4 colTextWrap colTextAlignCenter"/>
                <asp:BoundField DataField="OrderID" HeaderText="訂單編號" ItemStyle-CssClass="colHeader4 colTextAlignRight"/>
                <asp:BoundField DataField="Status" HeaderText="訂單狀態" ItemStyle-CssClass="colHeader4 colTextAlignCenter"/>
                <asp:BoundField DataField="Amount" HeaderText="訂單金額" ItemStyle-CssClass="colHeader4 colTextAlignRight"/>
                <asp:BoundField DataField="PosNo" HeaderText="收銀機" ItemStyle-CssClass="colHeader3 colTextAlignCenter"/>
                <asp:BoundField DataField="OrderTime" HeaderText="訂單時間" ItemStyle-CssClass="colHeaderTime"/>
                <asp:BoundField DataField="ReturnTime" HeaderText="退貨時間" ItemStyle-CssClass="colHeaderTime"/>
                <asp:BoundField DataField="Products" HeaderText="訂單明細" ItemStyle-CssClass="colHeader4 colTextWrap" />
            </Columns>
        </asp:GridView>
        <br />
    </div>
    </form>
</body>
</html>
