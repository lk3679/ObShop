<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="menu_ship_manage.aspx.cs"
    Inherits="OBShopWeb.menu_ship_manage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Cache-Control" content="no-cache" />
    <meta http-equiv="Pragma" content="no-cache" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <style type="text/css">
        body
        {
            margin-top: 10px;
            margin-left: 10px;
            margin-right: 0px;
            margin-bottom: 0px;
        }
    </style>
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/lockkey.js"></script>
    <script type="text/javascript" src="js/menu.js"></script>
    <script type="text/javascript">
        var url = "Choice.aspx";
        window.open(url, target = "content");

        c1 = "#CC9966";
        c2 = "#999922";
    </script>
</head>
<body style="background-color: #CC9966">
    <form id="form1" runat="server">
    <div style="padding: 2px; margin: 2px; background-color: #883300;">
        <span class="style5menuHead"><strong>[物流管理]</strong></span> 
        &nbsp;<asp:HyperLink ID="HL_daily" runat="server" NavigateUrl="~/TicketDaily.aspx" 
            Target="content" CssClass="style5menu">結貨單</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_daily2" runat="server" NavigateUrl="~/TicketDaily2.aspx"
            Target="content" CssClass="style5menu">包裹查詢</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_status" runat="server" NavigateUrl="~/SentShipping.aspx"
            Target="content" CssClass="style5menu">出貨狀態查詢</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_reshipout" runat="server" NavigateUrl="~/ReShipOut.aspx"
            Target="content" CssClass="style5menu">重複出貨</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_packageReport" runat="server" NavigateUrl="~/PackageReport.aspx"
            Target="content" CssClass="style5menu">包裹統計</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_stockAdjust" runat="server" NavigateUrl="~/StockAdjust.aspx"
            Target="content" CssClass="style5menu" Visible="False">庫存調整</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_noProduct" runat="server" NavigateUrl="~/NoProductReport.aspx"
            Target="content" CssClass="style5menu" Visible="False">無貨刪單</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_PickCheckList" runat="server" NavigateUrl="~/PickCheckList.aspx"
            Target="content" CssClass="style5menu">撿分狀態</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ShipInShelfStatus.aspx"
            Target="content" CssClass="style5menu">入庫狀態</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/SeparateMove.aspx"
            Target="content" CssClass="style5menu">拆包移儲</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_ExportCount" runat="server" NavigateUrl="~/ExportCount.aspx"
            Target="content" CssClass="style5menu">倉出貨量</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_SetWeight" runat="server" NavigateUrl="~/SetWeight.aspx"
            Target="content" CssClass="style5menu" Visible="False">秤重</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_MistakeReport" runat="server" NavigateUrl="~/MistakeReport.aspx"
            Target="content" CssClass="style5menu" Visible="False">撿貨誤差</asp:HyperLink>
       <%-- &nbsp;<asp:HyperLink ID="HL_unpackage" runat="server" NavigateUrl="~/UnPackage.aspx"
            Target="content" CssClass="style5menu" Visible="False">官網拆包</asp:HyperLink>--%>
        &nbsp;<asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/ShipOutOrderAnalysis.aspx"
            Target="content" CssClass="style5menu"  Visible="True">訂單分析</asp:HyperLink>
    </div>
    </form>
</body>
</html>