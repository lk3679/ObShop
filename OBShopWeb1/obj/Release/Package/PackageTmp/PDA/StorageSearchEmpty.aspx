<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageSearchEmpty.aspx.cs" Inherits="OBShopWeb.PDA.StorageSearchEmpty" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>空儲位查詢-新</title>
    <link rel="stylesheet" type="text/css" href="../layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <span class="style1"><strong>空儲位查詢-新</strong></span>
    <br />
    <hr />
    <%--<span class="style13PDA" >樓層：</span>--%>
    <asp:DropDownList ID="ddl_Floor" runat="server" CssClass="style13PDA" 
        AutoPostBack="True" onselectedindexchanged="ddl_Floor_SelectedIndexChanged" Visible="False">
    </asp:DropDownList>
    <span class="style13PDA">區域：</span>
    <asp:DropDownList ID="ddl_Area" runat="server" CssClass="style13PDA" 
        AutoPostBack="True" onselectedindexchanged="ddl_Area_SelectedIndexChanged">
    </asp:DropDownList>
    &nbsp;&nbsp;
    <asp:Button ID="btn_Submit" runat="server" Text="確認" CssClass="style13PDA" Visible="False" />
    &nbsp;&nbsp;
    <asp:Button ID="btnXls" runat="server" Text="匯出Excel" CssClass="style13PDA" 
            onclick="btnXls_Click" />
    <hr />
    <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text="" ></asp:Label>
    <span class="style13PDA">空儲位：</span><asp:Label ID="lbl_Num" runat="server"  CssClass="style13PDA" Text="" ForeColor="#0066FF" Font-Bold="True"></asp:Label>
    <br /><asp:Label ID="lbl_Content" runat="server"  CssClass="style13PDA" Text=""></asp:Label>    
    </div>
    </form>
</body>
</html>
