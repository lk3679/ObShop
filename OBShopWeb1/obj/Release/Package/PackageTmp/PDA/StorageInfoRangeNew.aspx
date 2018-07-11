<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageInfoRangeNew.aspx.cs" Inherits="OBShopWeb.PDA.StorageInfoRangeNew" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>儲位盤點報表-新</title>
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
    <span class="style1"><strong>儲位盤點報表-新</strong></span>
    &nbsp;&nbsp;
    <span class="style2" style="color: #006600"><strong>(可查詢儲位：散貨、普通、補貨、過季、問題)</strong></span>
    &nbsp;&nbsp;
    <asp:HyperLink ID="HL_系列盤點" runat="server" CssClass="style2" 
        NavigateUrl="StorageInfoRangeSeries.aspx" ForeColor="#FF3300" Target="_blank">系列盤點報表</asp:HyperLink>
    <br />
    <br />
    <hr />
    <%--<span class="style13PDA" >樓層：</span>--%>
    <asp:DropDownList ID="ddl_Floor" runat="server" CssClass="style13PDA" 
        AutoPostBack="True" onselectedindexchanged="ddl_Floor_SelectedIndexChanged" 
        ForeColor="#0066FF" Visible="False">
    </asp:DropDownList>
    <span class="style13PDA">區域：</span>
    <asp:DropDownList ID="ddl_Area" runat="server" CssClass="style13PDA" 
        AutoPostBack="false" onselectedindexchanged="ddl_Area_SelectedIndexChanged"
        ForeColor="#0066FF">
    </asp:DropDownList>
    &nbsp;&nbsp;
    <span class="style13PDA">起迄(8碼)：</span>
    <asp:TextBox ID="txt_StartShelf" runat="server" CssClass="style13PDA" MaxLength="8" Width="80px" ForeColor="#0066FF">00000000</asp:TextBox>
    <span class="style13PDA">～</span>
    <asp:TextBox ID="txt_EndShelf" runat="server" CssClass="style13PDA" MaxLength="8" Width="80px" ForeColor="#0066FF">01ZZZZZZ</asp:TextBox>
    &nbsp;
    <span class="style13PDA">可銷狀態：</span>
    <asp:DropDownList ID="DDL_Type" runat="server" CssClass="style13PDA" ForeColor="#CC0000">
        <asp:ListItem Value="-1" Selected="True">全部(不含狀態)</asp:ListItem>
        <asp:ListItem Value="2">全部(含狀態)</asp:ListItem>
        <asp:ListItem Value="1">可銷(含狀態)</asp:ListItem>
        <asp:ListItem Value="0">不可銷(含狀態)</asp:ListItem>
    </asp:DropDownList>
    &nbsp;
    <asp:Button ID="btn_Submit" runat="server" Text="查詢" CssClass="style13PDA" onclick="btn_Submit_Click"/>
    &nbsp;&nbsp;
    <asp:Button ID="btnXls" runat="server" Text="匯出XLS" CssClass="style13PDA" onclick="btnXls_Click" 
         BackColor="#FF9933" BorderColor="Red" BorderStyle="Outset" BorderWidth="3px" ForeColor="#CC0000"/>
    &nbsp;&nbsp;
    <asp:Button ID="btn_GetAllCount" runat="server" Text="總庫存數/可銷數" CssClass="style13PDA" onclick="btn_GetAllCount_Click" 
         BackColor="#CCFF33" BorderColor="#003300" BorderStyle="Outset" BorderWidth="3px" ForeColor="#003300"/>
    <hr />
    <asp:Panel ID="P_AllCount" runat="server" Visible="False">
    <fieldset class="style2">
    <legend class="style2" style="color: #0066FF; border-color: #003366">總庫存數/可銷數</legend>
        <asp:GridView ID="gv_AllCount" runat="server" CellPadding="4" CellSpacing="1"
            CssClass="style4gv" ForeColor="#333333" GridLines="None" Width="500px">
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
    </fieldset>
    </asp:Panel>
    <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text="" ></asp:Label>
    <br />
    &nbsp;<asp:Label ID="lbl_Count" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>

    <asp:GridView ID="gv_List" runat="server" CellPadding="4" CellSpacing="2" 
        CssClass="style4gv" ForeColor="#333333" GridLines="None" Width="500px" 
        EmptyDataText="無資料" onprerender="gv_List_PreRender" onrowdatabound="gv_List_RowDataBound">
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
