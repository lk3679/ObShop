<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="logisticsScoreSetting.aspx.cs"
    Inherits="OBShopWeb.logisticsScoreSetting" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>績效權重設定</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <script type="text/javascript" src="js/jquery.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <span class="style1">績效權重設定</span>
        <br />
        <br />
        <hr />
        <asp:GridView ID="gv_logistics" runat="server" CellPadding="2" CellSpacing="2" CssClass="style4gv"
            ForeColor="#333333" GridLines="None" Width="600px" EmptyDataText="無資料" AutoGenerateColumns="False"
            OnRowDataBound="gv_logistics_RowDataBound">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="名稱" ReadOnly="True" SortExpression="Name" />
                <asp:BoundField DataField="oldScore" HeaderText="原權重" ReadOnly="True" SortExpression="Name" />
                <asp:TemplateField HeaderText="設定權重" SortExpression="newScore">
                    <ItemTemplate>
                        <asp:TextBox ID="txtScore" runat="server" Text='<%# Bind("oldScore") %>' CssClass="style2"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#888888" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
            <SortedAscendingCellStyle BackColor="#FDF5AC" />
            <SortedAscendingHeaderStyle BackColor="#4D0000" />
            <SortedDescendingCellStyle BackColor="#FCF6C0" />
            <SortedDescendingHeaderStyle BackColor="#820000" />
        </asp:GridView>
        <hr />
        <asp:Button ID="btn_Send" runat="server" Text="儲存修改" OnClick="btn_Send_Click" CssClass="style2" />
        &nbsp;<asp:Label ID="lbl_Msg" runat="server" CssClass="style3"></asp:Label>
    </div>
    </form>
</body>
</html>