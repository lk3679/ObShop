<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShipOutTicket.aspx.cs"
    Inherits="OBShopWeb.ShipOutTicket" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Cache-Control" content="no-cache" />
    <meta http-equiv="Pragma" content="no-cache" />
    <title>傳票出庫</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<script type="text/javascript" src="js/jquery.js"></script>
<script type="text/javascript" src="js/search.js"></script>
<script type="text/javascript" src="js/utils.js"></script>
<script type="text/javascript" src="js/jquery.blockUI.js"></script>
<script type="text/javascript">
    $(document).ready(function () {
        $('#btn_All_Search').click(function () {
            $.blockUI({
                message: $('<h4 style="text-align:center"><img src="image/loading4.gif" /> <br /><br />loading...</h4>'),
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
<script type="text/javascript" src="../js/lightbar2.js"></script>
<script type="text/javascript">
    d2 = "#DDDDDD";
</script>
<script type="text/javascript">
    function CreateDoc(element, pickType, ticket,area, store) {

        var r = confirm("確定產生【" + ticket + "】 撿貨單? \n(※會扣儲位庫存，務必確認傳票正確)");

        if (r) {
            element.disabled = true;
            element.style.display = "none";
            var url = "CreateDoc.aspx?pickType=" + pickType + "&tick=" + ticket + "&area=" + area + "&store=" + store;
      
            window.open(url, target = "_self");
        }

        return false;
    }

    function Verify(element, ticket, area, store) {
        var r = confirm("確定檢驗【" + ticket + "】 調出單?");

        if (r) {
            element.disabled = true;
            element.style.display = "none";
            var url = "ShipOutVerify.aspx?tick=" + ticket + "&area=" + area + "&store=" + store;
            //window.location = url;
            window.open(url, target = "_self");
        }

        return false;
    }

    function disable1(element) {
        element.disabled = true;
        __doPostBack('btn_Ticket_Search', '');
    }
    function disable2(element) {
        element.disabled = true;
        __doPostBack('btn_Date_Search', '');
    }
    function disable3(element) {
        element.disabled = true;
        __doPostBack('btn_VDate_Search', '');
    }
    function disable4(element) {
        element.disabled = true;
        __doPostBack('btn_All_Search', '');
    }
</script>
<body>
    <form id="form1" runat="server">
    <div>
        <span class="style1">傳票出庫</span>
        <br />
        <br />
        <span class="style2">種類：</span>
        <asp:DropDownList ID="DDL_Type" runat="server" CssClass="style2">
            <asp:ListItem Value="1" Selected="True">調出</asp:ListItem> 
            <asp:ListItem Value="6" >瑕疵</asp:ListItem> 
        </asp:DropDownList>
        &nbsp;
        <asp:Button ID="btn_Search" CssClass="style2" runat="server" Text="查詢" 
            onclick="btn_Search_Click" />
        <br class="style3" />
        <br />
        &nbsp; &nbsp;<asp:Label ID="lbl_Message" runat="server" CssClass="style3"></asp:Label>
        <br />
        <hr />
        &nbsp;<asp:Label ID="lbl_Count" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>
        <asp:GridView ID="gv_List" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
            CellSpacing="2" CssClass="style4gv" Width="100%" PageSize="20" OnRowDataBound="gv_List_RowDataBound"
            EmptyDataText="無資料" >
            <AlternatingRowStyle BackColor="White" />
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
        <hr />
        <br />
    </div>
    </form>
</body>
</html>