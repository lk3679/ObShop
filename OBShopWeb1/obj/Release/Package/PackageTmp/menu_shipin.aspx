<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="menu_shipin.aspx.cs" Inherits="OBShopWeb.menu_shipin" %>

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
        var url = "blank.aspx";
        window.open(url, target = "content");

        c1 = "#447744";
        c2 = "#999922";
    </script>
</head>
<body style="background-color: #447744">
    <form id="form1" runat="server">
    <div style="padding: 2px; margin: 2px; background-color: #003300;">
        <span class="style5menuHead"><strong>[入庫作業]</strong></span> &nbsp;
        <asp:HyperLink ID="HL_barcode" runat="server" NavigateUrl="~/PrintBarCode.aspx" Target="content"
            CssClass="style5menu">條碼列印</asp:HyperLink>
        &nbsp;
        <asp:HyperLink ID="HL_ShipInTicket" runat="server" NavigateUrl="~/ShipInTicket.aspx"
            Target="content" CssClass="style5menu">進貨調入傳票</asp:HyperLink>
        &nbsp;
        <asp:HyperLink ID="HL_DiffHandle" runat="server" NavigateUrl="~/DiffHandle.aspx"
            Target="content" CssClass="style5menu">驗貨差異處理</asp:HyperLink>
    </div>
    </form>
</body>
</html>