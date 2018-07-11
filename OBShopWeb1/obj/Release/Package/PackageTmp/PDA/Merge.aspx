<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Merge.aspx.cs" Inherits="OBShopWeb.PDA.Merge" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>批量合併(儲位→儲位)</title>
    <link rel="stylesheet" type="text/css" href="../layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <span class="style1"><strong>批量合併(儲位→儲位)</strong></span>
    <br />
    <hr />
    <asp:TextBox ID="txt_Input" runat="server" CssClass="style13PDA" Width="100" 
    AutoPostBack="true" ontextchanged="txt_Input_TextChanged" ></asp:TextBox>
    <asp:Button ID="btn_Submit" runat="server" Text="確認" CssClass="style13PDA" onclick="btn_Submit_Click" Visible="False"
        UseSubmitBehavior="false" OnClientClick="this.disabled='true';document.body.style.cursor='wait';"/>
    <hr />
    <span class="style13PDA">來源儲位：</span>
    <asp:Label ID="lbl_FromStorage_NO" runat="server"  CssClass="style13PDA" Text=""></asp:Label>
    &nbsp;<asp:Label ID="lbl_FromStage_NO_Type" runat="server" CssClass="style13PDA" Text=""></asp:Label>
    <br />
    <span class="style13PDA">目的儲位：</span>
    <asp:Label ID="lbl_TargetStorage_NO" runat="server"  CssClass="style13PDA" Text=""></asp:Label>
    &nbsp;<asp:Label ID="lbl_TargetStorage_NO_Type" runat="server" CssClass="style13PDA" Text=""></asp:Label>
    <br />
    <hr />
    <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text="" ></asp:Label> 
    <br />
    <asp:Label ID="lbl_ShelfQuantity" runat="server" ForeColor="#006600" Text="" CssClass="style13PDA"></asp:Label>
    <br />
    <span class="style13PDA">來源儲位用量：</span>
    <asp:Label ID="lbl_Volume" runat="server" CssClass="style13PDA" ForeColor="#006600"></asp:Label>
    <br />
    <asp:CheckBox ID="CB_Info" runat="server" Text="顯示儲位內容" CssClass="style13PDA"/>
    <br />
    <asp:Label ID="lbl_Info" runat="server" CssClass="style13PDA" ForeColor="#0066FF"></asp:Label>       
    </div>
    </form>
</body>
</html>
