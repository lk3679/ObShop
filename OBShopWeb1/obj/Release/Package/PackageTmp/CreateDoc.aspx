<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateDoc.aspx.cs" Inherits="OBShopWeb.CreateDoc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button1" runat="server" Text="測試扣數" onclick="Button1_Click" Visible="False" />
        <asp:Label ID="lbl_Message" runat="server" Text=""></asp:Label>
    </div>
    </form>
</body>
</html>
