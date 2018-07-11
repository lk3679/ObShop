<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintStorageNew.aspx.cs"
    Inherits="OBShopWeb.PDA.PrintStorageNew" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>儲位列印-新儲位格式</title>
    <link rel="stylesheet" type="text/css" href="../layout.css" />
    <script type="text/javascript" src="../js/jquery.js"></script>
    <script type="text/javascript" src="../js/json2.js"></script>
    <script type="text/javascript">

        //全選
        function allcheck() {
            var o1 = $("span[key='CB_gv1'] input");
            for (var i = 0; i < $(o1).length; i++) {
                o1[i].checked = true;
            }
            return false;
        }
        //全不選
        function alluncheck() {
            var o1 = $("span[key='CB_gv1'] input");
            for (var i = 0; i < $(o1).length; i++) {
                o1[i].checked = false;
            }
            return false;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <span class="style1"><strong>儲位列印-新儲位格式</strong></span>
        <br />
        <br />
        <table>
            <tr>
                <td>
                    <span class="style2">列印地區：</span>
                </td>
                <td>
                    <asp:RadioButtonList ID="radioStore" runat="server" RepeatDirection="Horizontal"
                        AutoPostBack="True" OnSelectedIndexChanged="radioStore_SelectedIndexChanged" CssClass="style2">
                        <asp:ListItem Value="2">龜山</asp:ListItem>
                        <asp:ListItem Value="6"  Selected="True">門市</asp:ListItem>
                    </asp:RadioButtonList> 
                </td>
            </tr>
            <tr>
                <td>
                    <span class="style2">列印伺服器：</span>
                </td>
                <td>
                    <asp:DropDownList ID="dropMachine" runat="server" CssClass="style2" ForeColor="#9900CC">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <%--<span class="style2">樓層：</span>--%><asp:DropDownList ID="ddl_Floor" runat="server"
            CssClass="style2" AutoPostBack="true" OnSelectedIndexChanged="ddl_Floor_SelectedIndexChanged"
            Visible="False">
        </asp:DropDownList>
        &nbsp; <span class="style2">區域：</span><asp:DropDownList ID="ddl_Area" runat="server"
            CssClass="style2" AutoPostBack="true" OnSelectedIndexChanged="ddl_Area_SelectedIndexChanged"
            ForeColor="#0066FF">
        </asp:DropDownList>
        &nbsp; <span class="style2">排：</span><asp:DropDownList ID="ddl_Shelf1" runat="server"
            CssClass="style2">
            <asp:ListItem Value="-1">全部</asp:ListItem>
        </asp:DropDownList>
        &nbsp; <span class="style2">座：</span><asp:DropDownList ID="ddl_Shelf2" runat="server"
            CssClass="style2">
            <asp:ListItem Value="-1">全部</asp:ListItem>
        </asp:DropDownList>
        &nbsp; <span class="style2">層：</span><asp:DropDownList ID="ddl_Shelf3" runat="server"
            CssClass="style2">
            <asp:ListItem Value="-1">全部</asp:ListItem>
        </asp:DropDownList>
        &nbsp;
        <asp:Button ID="btn_Filter" runat="server" Text="篩選" Visible="True" CssClass="style2"
            OnClick="btn_Filter_Click" />
        <hr />
        <asp:Button ID="btn_SelectAll" runat="server" Text="全選" Visible="True" CssClass="style2"
            OnClientClick="return allcheck();" />
        &nbsp;
        <asp:Button ID="btn_UnSelectAll" runat="server" Text="全不選" Visible="True" CssClass="style2"
            OnClientClick="return alluncheck();" />
        &nbsp;<asp:Label ID="lbl_Message" runat="server" Text="" CssClass="style3"></asp:Label>
        &nbsp;<asp:GridView ID="gv_List" runat="server" CellPadding="2" CellSpacing="2" CssClass="style4gv"
            ForeColor="#333333" GridLines="None" Width="600px" AutoGenerateColumns="False">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:TemplateField HeaderText="選取">
                    <ItemTemplate>
                        <asp:CheckBox ID="CB_Select" key="CB_gv1" runat="server" />
                    </ItemTemplate>
                    <ItemStyle Width="50px" />
                </asp:TemplateField>
                <asp:BoundField DataField="儲位名稱" HeaderText="儲位名稱" />
                <asp:BoundField HeaderText="儲位類別" DataField="儲位類別" />
                <asp:BoundField HeaderText="儲位容量" DataField="儲位容量" />
                <asp:BoundField HeaderText="撿貨群組" DataField="撿貨群組" />
                <asp:BoundField HeaderText="儲位類別ID" DataField="儲位類別ID" />
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
    </div>
    <hr />
    <asp:Button ID="btn_Execution" runat="server" Text="列印" CssClass="style2" OnClick="btn_Print_Click" />
    </form>
</body>
</html>