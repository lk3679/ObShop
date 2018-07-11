<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="logout.aspx.cs" Inherits="OBShopWeb.logout" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>OB嚴選門市系統-已登出</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <style type="text/css">
        #abc { position: relative; margin: 0 auto; width: 520px; text-align:left }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <br />
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        <br />
    <div id="abc">
        <asp:Panel ID="Panel1" runat="server" BorderColor="Gray" 
            BorderStyle="Dashed" BorderWidth="1px" style="text-align: center">
        <!--如果沒有自動跳回首頁請按此&nbsp; -->
            <br />
            <br />
        <asp:HyperLink ID="HL_Return" runat="server" NavigateUrl="~/Default.aspx" 
            style="color: #006699" CssClass="style2">重新登入</asp:HyperLink>
            <br />
            <br />
            <br />
        </asp:Panel>
    </div>    
    </form>
</body>
</html>
