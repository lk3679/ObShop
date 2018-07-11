<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryNum.aspx.cs" Inherits="OBShopWeb.PDA.InventoryNum" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>盤點</title>
    <link rel="stylesheet" type="text/css" href="../layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <span class="style1"><strong>盤點</strong></span>
    <br />
    <span class="style13PDA">輸入:</span>
    <asp:TextBox ID="txt_Input" runat="server" CssClass="style13PDA" Width="100" AutoPostBack="true" ontextchanged="txt_Input_TextChanged" ></asp:TextBox>
    　
    <asp:Button ID="btn_Submit" runat="server" Text="盤點完成" CssClass="style13PDA" onclick="btn_Submit_Click"/>
    <asp:CheckBox ID="CB_EmptyCantDo" runat="server" CssClass="style13PDA" Text="空儲位不可盤" Visible="False" Checked="False"/>
    <br />
    <span class="style13PDA">件數:</span>
    <asp:TextBox ID="txt_Num" runat="server" CssClass="style13PDA" Width="40"></asp:TextBox>
    &nbsp;&nbsp;
    <asp:Label ID="lbl_ShelfQuantity" runat="server" ForeColor="#006600" Text="" CssClass="style13PDA"></asp:Label>
    <hr />
    <span class="style13PDA">儲位名稱：</span>
    <asp:Label ID="lbl_Storage_NO" runat="server"  CssClass="style13PDA" Text=""></asp:Label>
    &nbsp;<asp:Label ID="lbl_Storage_NO_Type" runat="server" CssClass="style13PDA" Text=""></asp:Label>
    <br />
    <hr />
    <asp:Label CssClass="style3" ID="lblInventoryLog" runat="server" ForeColor="#006600"></asp:Label>
    <br />
    <span class="style13PDA" style="color: #0066FF">目前已點件數：</span><asp:Label ID="lbl_CurrentNum"
        runat="server" CssClass="style13PDA" ForeColor="#0066FF">0</asp:Label>
    <br />
    <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text="" ></asp:Label>
    <br />
    <asp:Label ID="lbl_Product" runat="server" CssClass="style13PDA" Text=""></asp:Label>
    <br />    
    </div>
    </form>
</body>
</html>
