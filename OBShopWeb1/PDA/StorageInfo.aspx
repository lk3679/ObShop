<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageInfo.aspx.cs" Inherits="OBShopWeb.PDA.StorageInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>儲位內容</title>
    <link rel="stylesheet" type="text/css" href="../layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <span class="style1"><strong>儲位內容</strong></span>
    <br />
    <hr />
    <asp:TextBox ID="txt_Input" runat="server" CssClass="style13PDA" Width="100" 
        AutoPostBack="true" ontextchanged="txt_Input_TextChanged" ></asp:TextBox>
    &nbsp;
    &nbsp;
    <asp:CheckBox ID="CB_Log" runat="server" CssClass="style13PDA" Text="顯示Log"/>
    <span class="style13PDA">, </span><asp:CheckBox ID="CB_BOTLog" runat="server" CssClass="style13PDA" Text="印單Log"/>
    <hr />
    <span class="style13PDA">儲位名稱：</span>
    <asp:Label ID="lbl_Storage_NO" runat="server" CssClass="style13PDA" Text=""></asp:Label>
    &nbsp;<asp:Label ID="lbl_Storage_NO_Type" runat="server" CssClass="style13PDA" Text=""></asp:Label>
    <br />
    <span class="style13PDA">儲位用量：</span>
    <asp:Label ID="lbl_Volume" runat="server" CssClass="style13PDA" ForeColor="#006600"></asp:Label>
    <br />
    <hr />
    <%--<span class="style13PDA">儲位內容：</span>--%>
    <asp:Label ID="lbl_Info" runat="server" CssClass="style13PDA" ForeColor="#0066FF"></asp:Label>
    <br />
    <asp:Label ID="lbl_Log" runat="server" CssClass="style13PDA" ForeColor="#FF6600"></asp:Label>
    <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text="" ></asp:Label>     
    </div>
    </form>
</body>
</html>
