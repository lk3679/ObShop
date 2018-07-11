<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="menu_ship_manage1.aspx.cs" Inherits="OBShopWeb.menu_ship_manage1" %>

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
        var url = "RequireList.aspx";
        window.open(url, target = "content");

        c1 = "#996699";
        c2 = "#999922";
    </script> 
</head>
<body style="background-color: #CC9966">
    <form id="form1" runat="server">
    <div style="padding: 2px; margin: 2px; background-color: #883300;">
        <span class="style5menuHead"><strong>[物流管理]</strong></span> 
        &nbsp;<asp:HyperLink ID="HL_RequireList" runat="server" NavigateUrl="~/RequireList.aspx"
            Target="content" CssClass="style5menu" Visible="True">調撥需求單</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_RequireCheck" runat="server" NavigateUrl="~/RequireCheck.aspx"
            Target="content" CssClass="style5menu" Visible="False">需求單審核</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_ActivitiesSetting" runat="server" NavigateUrl="~/ActivitiesSetting.aspx"
            Target="content" CssClass="style5menu">折扣設定</asp:HyperLink>
    </div>
    </form>
</body>
</html>
