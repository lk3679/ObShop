<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MergePart2.aspx.cs" Inherits="OBShopWeb.PDA.MergePart2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>移動產品</title>
    <link rel="stylesheet" type="text/css" href="../layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <span class="style1"><strong>移動產品</strong></span>
    <hr />    
    <span class="style13PDA">輸入：</span>
    <asp:TextBox ID="txt_Input" runat="server" CssClass="style2" Width="100" AutoPostBack="true"
        OnTextChanged="txt_Input_TextChanged"></asp:TextBox>
    &nbsp;&nbsp;
    <asp:Button ID="btn_Submit" runat="server" Text="確認" CssClass="style13PDA" onclick="btn_Submit_Click" Visible="False" 
        UseSubmitBehavior="false" OnClientClick="this.disabled='true';"/>
    <br />
    <span class="style13PDA">件數：</span>
    <asp:TextBox ID="txt_Num" runat="server" CssClass="style13PDA" Width="30"></asp:TextBox>
    <asp:CheckBox ID="CB_NoCheck" runat="server" CssClass="style13PDA" Text="解開封印" Visible="False" Checked="True" />
    <hr />
    <span class="style13PDA">來源：</span>
    <asp:Label ID="lbl_FromStorage_NO" runat="server" CssClass="style13PDA" Text=""></asp:Label>
    &nbsp;
    <asp:Label ID="lbl_FromStage_NO_Type" runat="server" CssClass="style13PDA" Text=""></asp:Label>
    &nbsp;
    <asp:CheckBox ID="CB_LockFrom" runat="server" CssClass="style13PDA" Text="鎖定" />
    <br />
    <span class="style13PDA">目的：</span>
    <asp:Label ID="lbl_TargetStorage_NO" runat="server"  CssClass="style13PDA" Text=""></asp:Label>
    &nbsp;<asp:Label ID="lbl_TargetStorage_NO_Type" runat="server" CssClass="style13PDA" Text=""></asp:Label>
    <br />
    <hr />
    <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text=""></asp:Label>
    <br />
    <asp:Label ID="lbl_ShelfQuantity" runat="server" ForeColor="#006600" Text="" CssClass="style13PDA"></asp:Label>
    <br />
    <span class="style13PDA" style="color: #0066FF">目前已刷件數：</span><asp:Label ID="lbl_CurrentNum"
                runat="server" CssClass="style13PDA" ForeColor="#0066FF">0</asp:Label>
    <br />
    <asp:Label ID="lbl_Product" runat="server" CssClass="style13PDA" Text=""></asp:Label>
    <br />
    </form>
</body>
</html>
