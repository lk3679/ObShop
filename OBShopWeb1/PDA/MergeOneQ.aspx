<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MergeOneQ.aspx.cs" Inherits="OBShopWeb.PDA.MergeOneQ" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>問題上架</title>
    <link rel="stylesheet" type="text/css" href="../layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <span class="style1"><strong>問題儲位上架(請謹慎使用)</strong></span>
    <br />
    <hr />
    <span class="style13PDA">輸入:</span>
    <asp:TextBox ID="txt_Input" runat="server" CssClass="style13PDA" Width="100" AutoPostBack="true"
        OnTextChanged="txt_Input_TextChanged"></asp:TextBox>
<%--    <span class="style13PDA">總數:</span>
    <asp:TextBox ID="txt_AllNum" runat="server" CssClass="style13PDA" Width="20"></asp:TextBox>--%>
    &nbsp;
<%--    <asp:Button ID="btn_Submit" runat="server" Text="確認" CssClass="style13PDA" Width="40"
        OnClick="btn_Submit_Click" Visible="False" UseSubmitBehavior="false" 
        OnClientClick="this.disabled='true';document.body.style.cursor='wait';this.form.submit();"/>--%>
    <asp:Button ID="btn_Submit" runat="server" Text="確認" CssClass="style13PDA" Width="40"
        OnClick="btn_Submit_Click" UseSubmitBehavior="false" 
        OnClientClick="this.disabled='true';document.body.style.cursor='wait';"/>
<%--<asp:Button runat="server" Text="條件查尋" ID="btnSearch" UseSubmitBehavior="false" OnClientClick="this.disabled='true';document.body.style.cursor='wait';this.form.submit();"></asp:Button>--%>
    <br />
    <span class="style13PDA">件數:</span>
    <asp:TextBox ID="txt_Num" runat="server" CssClass="style13PDA" Width="30"></asp:TextBox>
    <hr />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <span class="style13PDA">儲位名稱：</span>
            <asp:Label ID="lbl_TargetStorage_NO" runat="server" CssClass="style13PDA" Text=""></asp:Label>
            &nbsp;<asp:Label ID="lbl_TargetStorage_NO_Type" runat="server" CssClass="style13PDA"
                Text=""></asp:Label>
            <br />
            <hr />
            <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text=""></asp:Label>
            <br />
            <span class="style13PDA" style="color: #0066FF">目前已點件數：</span>
            <asp:Label ID="lbl_CurrentNum" runat="server" CssClass="style13PDA" ForeColor="#0066FF">0</asp:Label>
            <br />
            <span class="style13PDA" style="color: #006600">產品名稱：</span>
            <br />
            <asp:Label ID="lbl_Product" runat="server" CssClass="style13PDA" Text="" ForeColor="#006600"></asp:Label>
            <br />
            <asp:HiddenField ID="hide_ProductBarcode" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>  
    </div>
    </form>
</body>
</html>

