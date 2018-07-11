<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoProductCheck.aspx.cs"
    Inherits="OBShopWeb.NoProductCheck" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>無貨回報確認</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<script type="text/javascript" src="js/jquery.js"></script>
<script type="text/javascript" src="js/json2.js"></script>
<script type="text/javascript">
    function Report() {
        if (confirm("確定 回報 【" + $("#lbl_Product").text() + "】 ?")) {
            return true;
        }
        else {
            return false;
        }
    }
    function Change() {
        if (confirm("確定 更換 【" + $("#lbl_Product").text() + "】 【" + $("#lbl_Shelf").text() + "】成建議儲位?")) {
            return true;
        }
        else {
            return false;
        }
    }
</script>
<script type="text/javascript" src="js/utils.js"></script>
<script type="text/javascript" src="js/jquery.blockUI.js"></script>
<script type="text/javascript">
    $(document).ready(function () {
        $('#btn_Report').click(function () {
            $.blockUI({
                message: $('<h4 style="text-align:center"><img src="/image/loading.gif" /> <br /><br />loading...</h4>'),
                css: {
                    top: ($(window).height()) / 2 + 'px',
                    left: ($(window).width()) / 3 + 'px',
                    color: '#fff',
                    background: 'none',
                    border: '0px',
                    opacity: 0.6
                }
            });
        });
    });
</script>
<body>
    <form id="form1" runat="server">
    <div>
        <span class="style1"><strong>無貨回報確認</strong></span>
        <asp:Label ID="lbl_ShipOutType" runat="server" CssClass="style1" ForeColor="White"></asp:Label>
        &nbsp;&nbsp;
        <asp:HyperLink ID="HL_無貨建議記錄" runat="server" CssClass="style2" NavigateUrl="NoProductCheckHistory.aspx"
            ForeColor="#9900CC" Target="_blank">※此單【無貨建議記錄】查詢</asp:HyperLink>
        <br />
        <br />
        <span class="style2">檢貨單號：</span><asp:Label ID="lblPickNum" runat="server" CssClass="style2"></asp:Label>&nbsp;&nbsp;
        <br />
        <span class="style2"><asp:Label ID="lblNumName" runat="server" Text=""></asp:Label></span><asp:Label ID="lblNum" runat="server" CssClass="style2"></asp:Label>&nbsp;&nbsp;
        <br /> 
        <asp:Label ID="lbl_Allot" runat="server" CssClass="style2" ForeColor="#006600"></asp:Label><br />
        <span class="style2">買家帳號：</span><asp:Label ID="lbl_Account" runat="server" CssClass="style2"></asp:Label><br />
        <%-- <span class="style2">序號：</span><asp:Label ID="lbl_ShipId" runat="server" CssClass="style2"></asp:Label><br />--%>
        <span class="style2">儲位名稱：</span><asp:Label ID="lbl_Shelf" runat="server" CssClass="style2"></asp:Label><br />
        <span class="style2">商品編號：</span><asp:Label ID="lbl_Product" runat="server" CssClass="style2"></asp:Label><br />
        <span class="style2">缺少數量：</span><asp:Label ID="lbl_Quantity" runat="server" CssClass="style2"></asp:Label><br />
        <span class="style2">原因：</span><asp:Label ID="lblComment" runat="server" CssClass="style2" ForeColor="Red"></asp:Label><br />
        <br /> 
        <asp:Button ID="btn_Report" runat="server" Text="無貨回報" OnClientClick="return Report();"
            OnClick="btn_Report_Click" CssClass="style2" />
        &nbsp;<asp:Label ID="lblMsg" runat="server" ForeColor="Red" CssClass="style2"></asp:Label>
        <br />
        <hr />
        <asp:Label ID="lbl_Message" runat="server" Text="" CssClass="style3"></asp:Label>
        <asp:GridView ID="gv_List" runat="server" CellPadding="2" CellSpacing="2" CssClass="style4gv"
            ForeColor="#333333" GridLines="None" OnRowCommand="Grid_RowCommand" Width="100%"
            AutoGenerateColumns="False">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField HeaderText="儲位類型" DataField="儲位類型" />
                <asp:BoundField HeaderText="建議儲位" DataField="建議儲位" />
                <asp:TemplateField HeaderText="無貨處理">
                    <ItemTemplate> 
                        <asp:Button ID="Btn_Execution" runat="server" Text="確認完成" OnClientClick="return Change();"
                            CommandName="btnEdit" CommandArgument='<%#Eval("建議儲位")%>' CssClass="style2" />
                    </ItemTemplate>
                </asp:TemplateField>
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
    </div>
    </form>
</body>
</html>