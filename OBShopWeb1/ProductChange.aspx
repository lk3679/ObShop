<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductChange.aspx.cs" Inherits="OBShopWeb.ProductChange" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Cache-Control" content="no-cache" />
    <meta http-equiv="Pragma" content="no-cache" />
    <title>產品異動</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <span class="style1">產品價格異動</span>
        <br /> 
        <br /> 
        <span class="style2">列印伺服器：</span>
        <asp:DropDownList ID="DDL_destination_ticket" runat="server" CssClass="style2" ForeColor="#9900CC">
        </asp:DropDownList>
    </div> 
    
    <asp:Label ID="lbl_Message" runat="server" Font-Size="18pt" ForeColor="Red" CssClass="style3"></asp:Label>
    <br />
    <hr />
    <asp:Panel ID="panelChange" runat="server" > 
        <asp:GridView ID="gv_product_id" runat="server" CellPadding="2" ForeColor="#333333"
            GridLines="None" CellSpacing="2" CssClass="style2" Width="100%" PageSize="20"
            AutoGenerateColumns="False">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField HeaderText="序號" />
                <asp:BoundField HeaderText="儲位位置" />
                <asp:BoundField HeaderText="產品編號" />
                <asp:BoundField HeaderText="原售價" />
                <asp:BoundField HeaderText="異動售價" />
                <asp:TemplateField HeaderText="數量">
                    <ItemTemplate>
                        <asp:DropDownList ID="DDL_Quantity" runat="server" Width="50px" CssClass="style2">
                            <asp:ListItem Value="0"></asp:ListItem>
                            <asp:ListItem Value="1"></asp:ListItem>
                            <asp:ListItem Value="2"></asp:ListItem>
                            <asp:ListItem Value="3"></asp:ListItem>
                            <asp:ListItem Value="4"></asp:ListItem>
                            <asp:ListItem Value="5"></asp:ListItem>
                            <asp:ListItem Value="6"></asp:ListItem>
                            <asp:ListItem Value="7"></asp:ListItem>
                            <asp:ListItem Value="8"></asp:ListItem>
                            <asp:ListItem Value="9"></asp:ListItem>
                            <asp:ListItem Value="10"></asp:ListItem>
                            <asp:ListItem Value="20"></asp:ListItem>
                            <asp:ListItem Value="30"></asp:ListItem>
                            <asp:ListItem Value="40"></asp:ListItem>
                            <asp:ListItem Value="50"></asp:ListItem>
                            <asp:ListItem Value="60"></asp:ListItem>
                            <asp:ListItem Value="70"></asp:ListItem>
                            <asp:ListItem Value="80"></asp:ListItem>
                            <asp:ListItem Value="90"></asp:ListItem>
                            <asp:ListItem Value="100"></asp:ListItem>
                        </asp:DropDownList>
                    </ItemTemplate> 
                </asp:TemplateField>
                <asp:BoundField HeaderText="建立時間" />
            </Columns>
            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" HorizontalAlign="NotSet" />
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
            <SortedAscendingCellStyle BackColor="#FDF5AC" />
            <SortedAscendingHeaderStyle BackColor="#4D0000" />
            <SortedDescendingCellStyle BackColor="#FCF6C0" />
            <SortedDescendingHeaderStyle BackColor="#820000" />
        </asp:GridView>
        <br />
        <asp:Button ID="btnPrintShelf" runat="server" Text="列印展售/補貨儲位" 
            CssClass="style2" Visible="False" onclick="btnPrintShelf_Click" />
    </asp:Panel>
    </form>
</body>
</html>
