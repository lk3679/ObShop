<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="menu_logistics.aspx.cs"
    Inherits="OBShopWeb.menu_logistics" %>

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
        var url = "logistics_account.aspx";
        window.open(url, target = "content");

        c1 = "#996699";
        c2 = "#999922";
    </script>
</head>
<body style="background-color: #996699">
    <form id="form1" runat="server">
    <div style="padding: 2px; margin: 2px; background-color: #551155;">
        <span class="style5menuHead"><strong>[人員管理]</strong></span>
        <asp:HyperLink ID="HL_logistics_account" runat="server" NavigateUrl="~/logistics_account.aspx"
            Target="content" CssClass="style5menu">人員管理</asp:HyperLink>
        &nbsp;
        <asp:HyperLink ID="HL_logistics_print" runat="server" NavigateUrl="~/logistics_print.aspx"
            Target="content" CssClass="style5menu">績效報表</asp:HyperLink>
        &nbsp;
        <asp:HyperLink ID="HL_logistics_print2" runat="server" NavigateUrl="~/logistics_print2.aspx"
            Target="content" CssClass="style5menu">績效報表(區間)</asp:HyperLink>
        &nbsp;
        <asp:HyperLink ID="HL_logisticsScoreSetting" runat="server" NavigateUrl="~/logisticsScoreSetting.aspx" 
            Target= "content" CssClass="style5menu">權重設定</asp:HyperLink>
    </div>
    </form>
</body>
</html>
