<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="logistics_print.aspx.cs" Inherits="OBShopWeb.logistics_print" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>績效報表(總分)</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<body >
    <form id="form1" runat="server">
    <div>
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        <span class="style1"><strong>績效報表(總分)</strong></span><br class="style2" /><br />
        <span class="style2">開始日期：</span><asp:TextBox ID="txt_Start" runat="server" 
            Width="100px" CssClass="style2"></asp:TextBox>
        <asp:CalendarExtender ID="ImgBtn_Date_CalendarExtender" runat="server" 
            DaysModeTitleFormat="yyyy-MM-dd" Enabled="True" Format="yyyy-MM-dd" 
            PopupButtonID="Imgbtn_Date" TargetControlID="txt_Start" 
            TodaysDateFormat="yyyy-MM-dd">
        </asp:CalendarExtender>
        &nbsp;<asp:ImageButton ID="ImgBtn_Date" runat="server" ImageUrl="~/Image/Calendar.png" />
        &nbsp;<asp:CompareValidator ID="CompareValidator1" runat="server" 
            ControlToCompare="txt_End" ControlToValidate="txt_Start" CssClass="style3" 
            ErrorMessage="結束日期必須大於開始日期" Operator="LessThanEqual"></asp:CompareValidator>
        <br />
            <span class="style2">結束日期：</span><asp:TextBox ID="txt_End" runat="server" 
            Width="100px" CssClass="style2"></asp:TextBox>
        <asp:CalendarExtender ID="ImgBtn_Date2_CalendarExtender" runat="server" 
            DaysModeTitleFormat="yyyy-MM-dd" Enabled="True" Format="yyyy-MM-dd" 
            PopupButtonID="ImgBtn_Date2" TargetControlID="txt_End" 
            TodaysDateFormat="yyyy-MM-dd">
        </asp:CalendarExtender>
        &nbsp;<asp:ImageButton ID="ImgBtn_Date2" runat="server" ImageUrl="~/Image/Calendar.png" />
        <br />
        <asp:Button ID="btn_Search" runat="server" Text="查詢" CssClass="style2" onclick="btn_Search_Click" />
        &nbsp;<br />
        <hr />
        <asp:GridView ID="gv_logistics" runat="server" 
            CellPadding="2" ForeColor="#333333" GridLines="None" CellSpacing="3" 
            Cssclass="style4gv" onprerender="gv_logistics_PreRender" Width="100%" 
            onrowdatabound="gv_logistics_RowDataBound">
            <AlternatingRowStyle BackColor="#FFFBD6" />
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
