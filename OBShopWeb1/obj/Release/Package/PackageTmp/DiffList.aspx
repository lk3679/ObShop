<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DiffList.aspx.cs" Inherits="OBShopWeb.DiffList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>進貨調回差異確認</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
	<script type="text/javascript" src="js/jquery.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <span class="style1"><strong>進貨調回差異確認</strong></span>
        <asp:Label ID="lbl_ShipOutType" runat="server" CssClass="style1" ForeColor="White"></asp:Label>
        <br />
        <br />
        &nbsp;<asp:Button ID="btn_Check" runat="server" Text="確認" onclick="btn_Check_Click" CssClass="style2" onclientclick="this.disabled = true;"  UseSubmitBehavior="false"/>
        <hr />
        <span class="style2">儲位名稱：</span><asp:Label ID="lbl_Storage_NO" runat="server" CssClass="style2" Text=""></asp:Label>
        &nbsp;<asp:Label ID="lbl_Storage_NO_Type" runat="server" CssClass="style13PDA" Text=""></asp:Label>
        <br />
        <hr />
        <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text=""></asp:Label>
        <asp:Label ID="lbl_Num" runat="server" CssClass="style2" Visible="False" 
            ForeColor="#0066CC"></asp:Label>
        <br />
        <%--<asp:Label ID="lbl_More" runat="server" CssClass="style2" Text="" ForeColor="Green"></asp:Label>
        <br />--%>
        <asp:Label ID="lbl_Product" runat="server" CssClass="style2" Text="" Visible="False"></asp:Label>
        <asp:Label ID="lbl_ProductReal" runat="server" Visible="False"></asp:Label>
        <asp:Label ID="lbl_ProductNormal" runat="server" CssClass="style2" Text=""></asp:Label>
        <asp:ListBox ID="listboxNormal" runat="server" CssClass="style2" Height="150px" Width="200px"></asp:ListBox> 
        <br /> <br />
        <asp:Label ID="lbl_ProductDiff" runat="server" CssClass="style2" Text="" ></asp:Label>
        <asp:ListBox ID="listboxDiff" runat="server" CssClass="style2" Height="150px" Width="200px"></asp:ListBox>
        <br />
    </div>
    </form>
</body>
</html>
