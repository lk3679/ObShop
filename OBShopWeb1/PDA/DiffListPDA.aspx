<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DiffListPDA.aspx.cs" Inherits="OBShopWeb.PDA.DiffListPDA" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>差異清單</title>
    <link rel="stylesheet" type="text/css" href="../layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <span class="style1"><strong>差異清單</strong></span>
    <br />
    &nbsp;
<%--    <asp:Button ID="btn_Submit" runat="server" Text="確認" CssClass="style13PDA" onclick="btn_Submit_Click" UseSubmitBehavior="false" 
        OnClientClick="this.disabled='true';document.body.style.cursor='wait';this.form.submit();"/>--%>
    <asp:Button ID="btn_Submit" runat="server" Text="確認" CssClass="style13PDA" onclick="btn_Submit_Click"/>
    <hr />
<%--    <span class="style13PDA">箱號：</span><asp:Label ID="lbl_Package_ID" runat="server"  CssClass="style13PDA" Text=""></asp:Label>
    <br />--%>
    <span class="style13PDA">儲位名稱：</span><asp:Label ID="lbl_Storage_NO" runat="server"  CssClass="style13PDA" Text=""></asp:Label>
    <br />
    <hr />    
    <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text="" ></asp:Label>
    <br />
    <span class="style13PDA"></span>
    <asp:Label ID="lbl_More" runat="server" CssClass="style13PDA" ForeColor="#006600"></asp:Label>
    <br />
    <span class="style13PDA"></span>
    <asp:Label ID="lbl_Lack" runat="server" CssClass="style13PDA" ForeColor="Red"></asp:Label>
    <br />
    <span class="style13PDA"></span>
    <asp:Label ID="lbl_Product" runat="server" CssClass="style13PDA" Text=""></asp:Label>
    </div>
    </form>
</body>
</html>
