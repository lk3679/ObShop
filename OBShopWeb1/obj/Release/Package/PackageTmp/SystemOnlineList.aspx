<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SystemOnlineList.aspx.cs" Inherits="OBShopWeb.SystemOnlineList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title> 門市系統Online名單</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <script type="text/javascript" src="js/utils.js"></script>
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/lightbar2.js"></script>
    <script type="text/javascript">
        d2 = "#DDDDDD";
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:Timer ID="Timer1" runat="server" Interval="60000" ontick="Timer1_Tick">
    </asp:Timer>
    <div>
    <span class="style1">門市系統Online名單</span><asp:Label ID="lbl_Shop" runat="server" Text="" CssClass="style1"></asp:Label>
    &nbsp;&nbsp;
    <span class="style2" style="color: #9900CC"><strong>(每分鐘刷新一次)</strong></span>
    &nbsp;
    <span class="style2"><strong>排序：</strong></span>
    <asp:DropDownList ID="DDL_OrderBy" runat="server" CssClass="style2" ForeColor="#006600" AutoPostBack="True">
        <asp:ListItem Selected="True" Value="0">登入時間</asp:ListItem>
        <asp:ListItem Value="1">帳號</asp:ListItem>
        <asp:ListItem Value="2">姓名</asp:ListItem>
        <asp:ListItem Value="3">IP位址</asp:ListItem>
    </asp:DropDownList>
    <br />
    <br />
    <hr />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <asp:Label ID="lbl_Count" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>
        <asp:GridView ID="gv_List" runat="server" CellSpacing="1" CellPadding="4" CssClass="style4gv"
            ForeColor="#333333" GridLines="None" Width="800px" EmptyDataText="無資料">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <EditRowStyle BackColor="#999999" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#E9E7E2" />
            <SortedAscendingHeaderStyle BackColor="#506C8C" />
            <SortedDescendingCellStyle BackColor="#FFFDF8" />
            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
        </asp:GridView>
        </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
