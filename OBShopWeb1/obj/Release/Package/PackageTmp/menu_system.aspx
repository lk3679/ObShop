<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="menu_system.aspx.cs" Inherits="OBShopWeb.menu_system" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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

        c1 = "#006699";
        c2 = "#999922";
    </script>
</head>

<body style="background-color: #006699">
    <form id="form2" runat="server" >
    <div style="padding: 2px; margin: 2px; background-color: #002255;">
        <span class="style5menuHead">
        <strong>[系統管理]</strong></span>
     
        <asp:HyperLink ID="HL_system_authManagment" runat="server" NavigateUrl="~/AuthManagement.aspx" 
            Target= "content" CssClass="style5menu">權限管理</asp:HyperLink>
        &nbsp;
        <asp:HyperLink ID="HL_system_AuthPro" runat="server" NavigateUrl="~/AuthPro.aspx" 
            Target= "content" CssClass="style5menu">新權限管理</asp:HyperLink>
        &nbsp;
        <asp:HyperLink ID="HL_MenuHideList" runat="server" NavigateUrl="~/MenuHideList.aspx" 
            Target= "content" CssClass="style5menu">隱藏功能</asp:HyperLink>  &nbsp;
    </div>
    </form>
</body>
</html>
