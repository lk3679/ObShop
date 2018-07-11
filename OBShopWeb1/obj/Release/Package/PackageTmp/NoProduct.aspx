<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoProduct.aspx.cs" Inherits="OBShopWeb.NoProduct" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>問題回報</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/json2.js"></script>
    <script type="text/javascript" src="js/lightbar.js"></script>
    <script type="text/javascript">
        d1 = "#DDDDDD";
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <span class="style1"><strong>問題回報</strong></span><asp:Label ID="lbl_ShipOutType"
            runat="server" CssClass="style1" ForeColor="White"></asp:Label>
        <br />
        <br />
        <table>
            <tr>
                <td>
                    <asp:RadioButtonList ID="RB_Flaw" runat="server" CssClass="style2" RepeatDirection="Horizontal">
                        <asp:ListItem Selected="True" Value="0">無貨</asp:ListItem>
                        <asp:ListItem Value="1">瑕疵</asp:ListItem>
                    </asp:RadioButtonList>

                </td>
                <td>
                    <asp:Panel ID="P_Reason" runat="server" >
                        &nbsp; &nbsp; &nbsp; <span class="style2" style="color: #006600"><strong>原因：</strong></span>
                        <asp:DropDownList ID="DDL_Reason" runat="server" CssClass="style2" ForeColor="#006600">
                            <asp:ListItem Value="0" Selected="True">未回報</asp:ListItem>
                            <asp:ListItem Value="1">回報錯誤</asp:ListItem>
                        </asp:DropDownList>
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <asp:Label ID="lbl_PickCheck_NO" runat="server" Text="輸入單號：" CssClass="style2"></asp:Label> 
                    <asp:TextBox ID="txt_PickCheck_NO" runat="server" CssClass="style2" AutoPostBack="True"
                        OnTextChanged="txt_PickCheck_NO_TextChanged"></asp:TextBox>  
                </td> 
            </tr>
        </table>
        <hr />
        <span class="style2">檢貨單號：</span>
        <asp:Label ID="lblPickNo" runat="server" CssClass="style2" ForeColor="#006699"></asp:Label>
        <br />
        <hr />
        <asp:Label ID="lbl_Message" runat="server" Text="" CssClass="style3"></asp:Label>
        <asp:GridView ID="gv_List" runat="server" CellPadding="2" CellSpacing="2" CssClass="style4gv"
            ForeColor="#333333" GridLines="None" OnRowDataBound="gv_List_RowDataBound" Width="100%"
            AutoGenerateColumns="False">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField HeaderText="" DataField="序號" />
                <asp:BoundField HeaderText="儲位名稱" DataField="儲位名稱" />
                <asp:BoundField HeaderText="商品編號" DataField="商品編號" />
                <asp:BoundField HeaderText="應有數量" DataField="應有數量" />
                <asp:BoundField HeaderText="分貨順序" DataField="GuestId" />
                <asp:BoundField HeaderText="出貨序號" DataField="出貨序號" />
                <asp:TemplateField HeaderText="短缺數量">
                    <ItemTemplate>
                        <asp:TextBox ID="LackNum" runat="server" Width="50" Text='<%# Bind("短缺數量")%>'></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="無貨處理">
                    <ItemTemplate>
                        <asp:Button ID="Btn_Execution" runat="server" Text="執行" OnClick="Btn_Execution_Click"
                            CommandName='<%# Bind("序號")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="貨運單號" DataField="貨運單號" />
                <asp:BoundField HeaderText="買家帳號" DataField="帳號" />
            </Columns>
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
        <asp:HiddenField ID="HideMe" runat="server" />
        <asp:HiddenField ID="HideString" runat="server" />
    </div>
    </form>
</body>
</html>