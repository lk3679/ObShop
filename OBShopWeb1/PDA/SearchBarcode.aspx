<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchBarcode.aspx.cs" Inherits="OBShopWeb.PDA.SearchBarcode" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>產編/條碼查詢</title>
    <link rel="stylesheet" type="text/css" href="../layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <span class="style1"><strong>產編/條碼查詢</strong></span>
    <br />
    <hr />
    <span class="style13PDA"><strong>請輸入產編或條碼：</strong></span>
    <asp:TextBox ID="txt_Input" runat="server" CssClass="style13PDA" Width="100" 
        AutoPostBack="true" ontextchanged="txt_Input_TextChanged" ></asp:TextBox>
        &nbsp;
        <br />
    <hr />
    <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text="" ></asp:Label><br />
    <span class="style13PDA"><strong>輸入： </strong></span>
    <asp:Label ID="lbl_ProductID" runat="server" CssClass="style13PDA" Text="" ForeColor="#006600"></asp:Label><br />
    <span class="style13PDA"><strong>結果： </strong></span>
    <asp:Label ID="lbl_Info" runat="server" CssClass="style13PDA" ForeColor="#0066FF"></asp:Label>
    <br />    
    </div>
    </form>
</body>
</html>
