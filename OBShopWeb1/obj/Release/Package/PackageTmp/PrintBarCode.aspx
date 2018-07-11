<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintBarCode.aspx.cs" Inherits="OBShopWeb.PrintBarCode" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Cache-Control" content="no-cache" />
    <meta http-equiv="Pragma" content="no-cache" />
    <title>條碼列印</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <span class="style1">條碼列印</span>
        <br />
        <br />
        <table>
            <tr>
                <td>
                    <span class="style2">列印種類：</span>
                </td>
                <td>
                    <asp:RadioButtonList ID="radioBtnType" runat="server" AutoPostBack="True" RepeatDirection="Horizontal"
                        OnSelectedIndexChanged="radioBtnType_SelectedIndexChanged" CssClass="style2">
                        <asp:ListItem Value="0" Selected="True">產品條碼</asp:ListItem>
                        <asp:ListItem Value="1">補貨/展售儲位</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
        </table>
        <span class="style2">列印伺服器：</span><asp:DropDownList ID="DDL_destination_ticket" 
            runat="server" CssClass="style2" ForeColor="#9900CC">
        </asp:DropDownList>
    </div>
    <asp:Label ID="lbl_Message" runat="server" Font-Size="18pt" ForeColor="Red" CssClass="style3"></asp:Label>
    <br />
    <hr />
    <asp:Panel ID="panelProduct" runat="server" Visible="True">
        <p>
            <span class="style2">產品ID：</span>
            <asp:TextBox ID="txt_productId" runat="server" Width="150" AutoPostBack="true" OnTextChanged="txt_productId_TextChanged" CssClass="style2"/>
            <span class="style2">數量：</span>
            <asp:DropDownList ID="DDL_Quantity" runat="server" Width="50px" CssClass="style2">
                <asp:ListItem Value="1" Selected="True"></asp:ListItem>
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
        </p>
        <asp:ListBox ID="lsbList" runat="server" Width="250" Height="350" DataTextField="Id"
            DataValueField="Barcode" CssClass="style2" ForeColor="#003300"></asp:ListBox>
        <br />
        <asp:Button ID="btnPrintProduct" runat="server" CssClass="style2" Text="列印" OnClick="btnPrintProduct_Click"
            Visible="False" />
    </asp:Panel>
    <asp:Panel ID="panelTicket" runat="server" Visible="False">
        <span class="style2">輸入補貨/展售儲位：</span><asp:TextBox ID="txt_Shelf" runat="server"
            AutoPostBack="True" CssClass="style2" OnTextChanged="txt_Shelf_TextChanged"></asp:TextBox>
        <asp:GridView ID="gv_product_id" runat="server" CellPadding="2" ForeColor="#333333"
            GridLines="None" CellSpacing="2" CssClass="style2" Width="100%" PageSize="20"
            AutoGenerateColumns="False">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField HeaderText="序號" />
                <asp:BoundField HeaderText="產品編號" />
                <asp:BoundField HeaderText="產品條碼" />
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
                <asp:BoundField HeaderText="價格" />
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
        <asp:Button ID="btnPrintShelf" runat="server" Text="列印" CssClass="style2" Visible="False"
            OnClick="btnPrintShelf_Click" />
    </asp:Panel>
    <div>
    </div>
    </form>
</body>
</html>