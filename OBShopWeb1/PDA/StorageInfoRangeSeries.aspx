<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageInfoRangeSeries.aspx.cs" Inherits="OBShopWeb.PDA.StorageInfoRangeSeries" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>系列盤點報表</title>
    <link rel="stylesheet" type="text/css" href="../layout.css" />
    <script type="text/javascript" src="../js/jquery.js"></script>
    <script type="text/javascript" src="../js/lightbar2.js"></script>
    <script type="text/javascript">
        d2 = "#DDDDDD";
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <span class="style1"><strong>系列盤點報表</strong></span>
    &nbsp;&nbsp;
    <span class="style2" style="color: #006600"><strong>(可查詢儲位：散貨、普通、補貨、過季、問題)</strong></span>
    &nbsp;&nbsp;
    <asp:CheckBox ID="CB_不良" runat="server" Text="只查不良儲位" CssClass="style2" ForeColor="#9900CC" />
    <br />
    <br />
    <hr />
    <span class="style2" style="color: #006600">●系列編號(用換行或用","分隔) ※請注意是否有全形空白</span>
    <br />
    <asp:TextBox ID="txt_系列" runat="server" CssClass="style13PDA" MaxLength="200" 
            Width="281px" ForeColor="#0066FF" Height="375px" TextMode="MultiLine"></asp:TextBox>
    &nbsp;
    <asp:Button ID="btn_Submit" runat="server" Text="查詢" CssClass="style13PDA" onclick="btn_Submit_Click"/>
    
    &nbsp;&nbsp;
    <asp:Button ID="btnXls" runat="server" Text="匯出XLS" CssClass="style13PDA" onclick="btnXls_Click" 
         BackColor="#FF9933" BorderColor="Red" BorderStyle="Outset" BorderWidth="3px" ForeColor="#CC0000"/>

    <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text="" ></asp:Label>

    <hr />
    <br />
    &nbsp;<asp:Label ID="lbl_Count" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>

    <asp:GridView ID="gv_List" runat="server" CellPadding="4" CellSpacing="2" 
        CssClass="style4gv" ForeColor="#333333" GridLines="None" Width="500px" 
        EmptyDataText="無資料">
        <AlternatingRowStyle BackColor="White" />
        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
        <SortedAscendingCellStyle BackColor="#FDF5AC" />
        <SortedAscendingHeaderStyle BackColor="#4D0000" />
        <SortedDescendingCellStyle BackColor="#FCF6C0" />
        <SortedDescendingHeaderStyle BackColor="#820000" />
    </asp:GridView>
    <hr />
    </div>
    </form>
</body>
</html>