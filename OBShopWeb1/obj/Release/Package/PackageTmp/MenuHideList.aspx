<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuHideList.aspx.cs" Inherits="OBShopWeb.MenuHideList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Cache-Control" content="no-cache" />
    <meta http-equiv="Pragma" content="no-cache" />
    <title>隱藏Menu清單</title>
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
        c1 = "#333333";
        c2 = "#999922";
    </script>
</head>
<body style="background-color: #FFFFFF">
    <form id="form1" runat="server">
    <div>
    <table cellpadding="10" cellspacing="4" border="2" style="border-color: #333333" bgcolor="#669999">
        <tr>
            <td valign="top">
                <span class="style5menuHead" style="color: #FFFF66">[門市作業]</span>
                <br />
                <br />
                <asp:HyperLink ID="HL_StockReport" runat="server" NavigateUrl="~/StockReport.aspx"
                    Target="_blank" CssClass="style5menu">庫存報表</asp:HyperLink>
                <br />
                <br />
            </td>
            
        </tr>
    </table>
        
    </div>
    </form>
</body>
</html>
