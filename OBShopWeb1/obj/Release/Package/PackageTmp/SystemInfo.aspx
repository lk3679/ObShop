<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SystemInfo.aspx.cs" Inherits="OBShopWeb.SystemInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>系統資訊</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:LinkButton ID="linkBtnProductChange" runat="server"  ForeColor="Red" Font-Size="20pt" PostBackUrl="~/ProductChange.aspx" CssClass="style3"></asp:LinkButton>
    </div>
    </form>
</body>
</html>

