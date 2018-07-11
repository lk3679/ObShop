<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchProductStock.aspx.cs" Inherits="OBShopWeb.PDA.SearchProductStock" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>產品查詢儲位庫存</title>
    <link rel="stylesheet" type="text/css" href="../layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <span class="style1"><strong>產品查詢儲位庫存</strong></span>
        <br />
        <hr />
        <asp:TextBox ID="txt_Input" runat="server" CssClass="style13PDA" Width="100" AutoPostBack="true"
            OnTextChanged="txt_Input_TextChanged"></asp:TextBox>
        &nbsp;<span class="style2"><strong>儲位：</strong></span>
        <asp:DropDownList ID="ddl_Type" runat="server" CssClass="style2" >
            <asp:ListItem Text="全部" Value="-1"></asp:ListItem>
            <asp:ListItem Text="標準" Value="0"></asp:ListItem>
            <asp:ListItem Text="散貨" Value="1"></asp:ListItem>
            <asp:ListItem Text="補貨" Value="2"></asp:ListItem>
            <%--<asp:ListItem Text="過季" Value="3"></asp:ListItem>--%>
            <asp:ListItem Text="問題" Value="4"></asp:ListItem>
            <asp:ListItem Text="不良" Value="5"></asp:ListItem>
            <asp:ListItem Text="打銷" Value="9"></asp:ListItem>
            <asp:ListItem Text="無貨" Value="10"></asp:ListItem>
            <asp:ListItem Text="展售" Value="20"></asp:ListItem>
        </asp:DropDownList>
        &nbsp;&nbsp;
        <asp:Button ID="btn_XLS" runat="server" CssClass="style2" 
                onclick="btn_XLS_Click" Text="匯出XLS" BackColor="#FF9933" 
                BorderColor="Red" BorderStyle="Outset" BorderWidth="3px" 
                ForeColor="#CC0000" />
        <hr />
        <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text=""></asp:Label><br />
        <asp:Label ID="lbl_ProductID" runat="server" CssClass="style2" Text="" ForeColor="#006600"></asp:Label><br />
        <asp:Label ID="lbl_Info1" runat="server" CssClass="style13PDA" ForeColor="#0066FF"></asp:Label><br />
        <br />
        <asp:GridView ID="gv_List" runat="server" Visible="False">
        </asp:GridView>
    </div>
    </form>
</body>
</html>
