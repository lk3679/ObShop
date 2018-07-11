<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PickCheck.aspx.cs" Inherits="OBShopWeb.PickCheck" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>撿貨確認</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <span class="style1"><strong>撿貨確認</strong></span>
        <asp:Label ID="lbl_ShipOutType" runat="server" CssClass="style1" ForeColor="White"></asp:Label>
        <br/><br />
        <asp:Label ID="lbl_PickCheck_NO" runat="server" Text="撿貨單號：" CssClass="style2"></asp:Label>
        &nbsp; 
        <asp:TextBox ID="txt_PickCheck_NO" runat="server" CssClass="style2" 
            AutoPostBack="True" ontextchanged="txt_PickCheck_NO_TextChanged"></asp:TextBox>
        <hr />
        <%--<span class="style2"><strong>撿貨類別：</strong></span>
        <asp:DropDownList ID="DDL_ShipOutType" runat="server" CssClass="style2" ForeColor="#6600CC">
            <asp:ListItem Value="請選擇"></asp:ListItem>
            <asp:ListItem Value="銷售"></asp:ListItem>
            <asp:ListItem Value="調出"></asp:ListItem>
        </asp:DropDownList>--%>
        <%--<asp:RadioButtonList ID="RB_ShipOutType" runat="server"  CssClass="style2" RepeatDirection="Horizontal" ForeColor="#6600CC">
                <asp:ListItem Selected="True" Value="請選擇"></asp:ListItem>
                <asp:ListItem Value="銷售">銷售</asp:ListItem>
                <asp:ListItem Value="調出">調出</asp:ListItem>
                </asp:RadioButtonList>
        <br />--%>
        <br />
        <span class="style2">傳票號碼：</span><asp:Label ID="lbl_Ticket_Id" runat="server" CssClass="style2"></asp:Label>
        <br />
        <span class="style2">銷售單號：</span><asp:Label ID="lbl_Ship_Id" runat="server" CssClass="style2"></asp:Label>
        <%--<br />
        <asp:Label ID="lbl_Union" runat="server" CssClass="style2"></asp:Label>
        <br />
        <span class="style2">傳票號碼：</span><asp:Label ID="lbl_Ticket_Id" runat="server" CssClass="style2"></asp:Label>
        <br />
        <span class="style2">倉庫號碼：</span><asp:Label ID="lbl_Repository_Id" runat="server" CssClass="style2"></asp:Label>
        <br />--%>
        <hr />
        <asp:Label ID="lbl_Message" runat="server" Text="" CssClass="style3"></asp:Label>
        </div>
    </form>
</body>
</html>
