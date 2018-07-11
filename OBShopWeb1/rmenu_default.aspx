<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rmenu_default.aspx.cs" Inherits="OBShopWeb.rmenu_default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>系統資訊</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <script type="text/javascript">
        var url = "SystemInfo.aspx";
        window.open(url, target = "content");
    </script>
</head>
<body style="background-color:#FFFFFF">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:Timer ID="Timer1" runat="server" Interval="300000">
    </asp:Timer>
    <div class="style1">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <%--<span style="color: #006699;">[請選擇項目]</span>--%>
        <span style="color: #006699;">[系統資訊]</span>
        &nbsp;&nbsp;
        <span class="style3">線上人數：</span>
        <asp:Label ID="lbl_Online" runat="server" Text="" CssClass="style3"></asp:Label>
        </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
