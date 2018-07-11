<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShipInTicket.aspx.cs" Inherits="OBShopWeb.ShipInTicket" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>門市調入</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<script type="text/javascript" src="js/jquery.js"></script>
<script type="text/javascript" src="js/search.js"></script>
<script type="text/javascript">
    function Check(element, id, box, areaId,ticketType) {
        var r = confirm("確認 驗貨入新儲位? 【" + id + "】\n\n※ 傳票將無法再作修改，請確認該傳票內容皆正確 ※");

        if (r) {
            element.disabled = true;
            var DDL_Type = document.getElementById("DDL_Type").value;
            var url = "ShipInVerify.aspx?ticket_id=" + id + "&box=" + box + "&areaId=" + areaId + "&ticketType=" + ticketType + "&importType=" + DDL_Type;
            window.open(url, "_blank");
        }

        return false;
    }
</script>

<script type="text/javascript" src="js/lightbar.js"></script>
<script type="text/javascript">
    d1 = "#DDDDDD";
</script>
<body>
    <form id="form1" runat="server">
      <div>
        <span class="style1">進貨調入</span>
        <br />
        <br /><span class="style2">種類：</span>
        <asp:DropDownList ID="DDL_Type" runat="server" CssClass="style2"> 
            <asp:ListItem Value="0" Selected="True">進貨</asp:ListItem>
            <%--<asp:ListItem Value="1">調出</asp:ListItem> --%>
        </asp:DropDownList>
        &nbsp;
        <asp:Button ID="btn_Search" CssClass="style2" runat="server" Text="查詢" 
              onclick="btn_Search_Click"  />
        <br class="style3" />
        <br />
        <hr />
        <asp:GridView ID="gv_List" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
            CellSpacing="2" CssClass="style4gv" Width="100%" PageSize="20" OnRowDataBound="gv_List_RowDataBound">
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
