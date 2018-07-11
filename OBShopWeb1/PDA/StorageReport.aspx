<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageReport.aspx.cs" Inherits="OBShopWeb.PDA.StorageReport" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>TW-儲位報表查詢</title>
    <link rel="stylesheet" type="text/css" href="../layout.css" />
</head>
<script type="text/javascript" src="../js/utils.js"></script>
<script type="text/javascript" src="../js/jquery.js"></script>
<script type="text/javascript" src="../js/jquery.blockUI.js"></script>
<script type="text/javascript">
    $(document).ready(function () {
        $('#btn_Search').click(function () {
            $.blockUI({
                message: $('<h4 style="text-align:center"><img src="../image/loading4.gif" /> <br/><br/>loading...</h4>'),
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
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:Panel ID="P_header" runat="server">
        <div id="div_header" runat="server">
            <span class="style1"><strong>儲位報表查詢</strong></span>
            <asp:Label ID="lbl_ShipOutType" runat="server" CssClass="style1" 
                ForeColor="White"></asp:Label>
            &nbsp;
            <asp:CheckBox ID="CB_NewFormat" runat="server" CssClass="style2" Text="新儲位格式" 
                Checked="False" AutoPostBack="True" 
                oncheckedchanged="CB_NewFormat_CheckedChanged" />
            <br />
            <br />
            <%--日期區塊--%>
            <asp:Panel ID="PL_Date" runat="server">
            <span class="style2">日期範圍：</span>
            <asp:TextBox ID="txt_Start" runat="server" CssClass="style2" Width="100px"></asp:TextBox>
            <asp:CalendarExtender ID="ImgBtn_Date_CalendarExtender" runat="server" 
                DaysModeTitleFormat="yyyy-MM-dd" Enabled="True" Format="yyyy-MM-dd" 
                PopupButtonID="Imgbtn_Date" TargetControlID="txt_Start" 
                TodaysDateFormat="yyyy-MM-dd">
            </asp:CalendarExtender>
            &nbsp;
            <asp:ImageButton ID="ImgBtn_Date" runat="server" 
                ImageUrl="~/Image/Calendar.png" />
            <span class="style2">～</span><asp:TextBox ID="txt_End" runat="server" 
                CssClass="style2" Width="100px"></asp:TextBox>
            <asp:CalendarExtender ID="ImgBtn_Date2_CalendarExtender" runat="server" 
                DaysModeTitleFormat="yyyy-MM-dd" Enabled="True" Format="yyyy-MM-dd" 
                PopupButtonID="ImgBtn_Date2" TargetControlID="txt_End" 
                TodaysDateFormat="yyyy-MM-dd">
            </asp:CalendarExtender>
            &nbsp;
            <asp:ImageButton ID="ImgBtn_Date2" runat="server" ImageUrl="~/Image/Calendar.png"/>
            &nbsp;&nbsp;
            </asp:Panel>
            
            <span class="style2">報表類別：</span>
            <asp:DropDownList ID="DDL_Choice" runat="server" CssClass="style2" 
                AutoPostBack="True" onselectedindexchanged="DDL_Choice_SelectedIndexChanged" >
                <asp:ListItem Value="0" Enabled="False">普通</asp:ListItem>
                <asp:ListItem Value="1" Selected="True">打銷</asp:ListItem>
                <asp:ListItem Value="2">無貨</asp:ListItem>
                <asp:ListItem Value="3">問題</asp:ListItem>
                <asp:ListItem Value="4">不良</asp:ListItem>
                <asp:ListItem Value="5">補貨</asp:ListItem>
                <asp:ListItem Value="6">散貨</asp:ListItem>
                <%--<asp:ListItem Value="7">過季</asp:ListItem>--%>
                <asp:ListItem Value="-1">--------</asp:ListItem>
                <asp:ListItem Value="8">暫存</asp:ListItem>
                <%--<asp:ListItem Value="9">問題暫存</asp:ListItem>--%>
                <asp:ListItem Value="10">不良暫存</asp:ListItem>
                <%--<asp:ListItem Value="11">海運暫存</asp:ListItem>
                <asp:ListItem Value="12">換貨暫存</asp:ListItem>
                <asp:ListItem Value="13">散貨暫存</asp:ListItem>
                <asp:ListItem Value="14">調回暫存</asp:ListItem>
                <asp:ListItem Value="15">預購暫存</asp:ListItem>--%>
                <asp:ListItem Value="20">展售</asp:ListItem>
                <asp:ListItem Value="21">寄倉</asp:ListItem>
                <asp:ListItem Value="-1">--------</asp:ListItem>
                <%--<asp:ListItem Value="95">無貨回報儲位列表</asp:ListItem>
                <asp:ListItem Value="98">入庫無差異無貨產品</asp:ListItem>
                <asp:ListItem Value="101">無貨有庫存列表</asp:ListItem>
                <asp:ListItem Value="102">盤點打銷與出貨相關</asp:ListItem>
                <asp:ListItem Value="103">出貨無貨列表</asp:ListItem>
                <asp:ListItem Value="104">打銷產品列表</asp:ListItem>--%>
                <asp:ListItem Value="111">展售上架時間</asp:ListItem>
                <asp:ListItem Value="112">總倉展售款式差異</asp:ListItem>
            </asp:DropDownList>
            <asp:Label ID="lbl_Floor" runat="server" Text="&nbsp;樓層/區域：" CssClass="style2" Visible="False"></asp:Label>
            <asp:DropDownList ID="DDL_Floor" runat="server" CssClass="style2" 
                Visible="False" AutoPostBack="True" 
                onselectedindexchanged="DDL_Floor_SelectedIndexChanged">
                <asp:ListItem Value="-1">全部</asp:ListItem>
                <asp:ListItem Value="2">2F</asp:ListItem>
                <asp:ListItem Value="3">3F</asp:ListItem>
                <asp:ListItem Value="4">4F</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="DDL_Area" runat="server" CssClass="style2" Visible="False">
            </asp:DropDownList>
            <%--<asp:Label ID="lbl_bigger" runat="server" Text="&nbsp;高於&nbsp;" CssClass="style2" Visible="False"></asp:Label>--%>
            <asp:Label ID="lbl_smaller" runat="server" Text="&nbsp;低於&nbsp;" CssClass="style2" Visible="False"></asp:Label>
            <asp:DropDownList ID="DDL_Percent" runat="server" CssClass="style2" Visible="False">
                <asp:ListItem Value="20">20%</asp:ListItem>
                <asp:ListItem Value="40">40%</asp:ListItem>
                <asp:ListItem Value="60">60%</asp:ListItem>
                <asp:ListItem Value="80">80%</asp:ListItem>
            </asp:DropDownList>
            <%--<span class="style2">類別：</span>--%>
            <asp:Label ID="lbl_Type" runat="server" Text="&nbsp;類別：&nbsp;" CssClass="style2" Visible="False"></asp:Label>
            <asp:DropDownList ID="DDL_Type" runat="server" CssClass="style2" Visible="False">
                <asp:ListItem Value="-1">全部</asp:ListItem>
                <asp:ListItem Value="1">出貨</asp:ListItem>
                <asp:ListItem Value="2" Selected="True">海運</asp:ListItem>
                <asp:ListItem Value="3">寄倉</asp:ListItem>
                <asp:ListItem Value="4">版借出</asp:ListItem>
                <asp:ListItem Value="5">瑕疵</asp:ListItem>
                <asp:ListItem Value="9">調出</asp:ListItem>
                <asp:ListItem Value="11">調回</asp:ListItem>
            </asp:DropDownList>
            <%--<span class="style2">狀態：</span>--%>
            <asp:Label ID="lbl_Status" runat="server" Text="&nbsp;狀態：&nbsp;" CssClass="style2" Visible="False"></asp:Label>
            <asp:DropDownList ID="DDL_Status" runat="server" CssClass="style2" Visible="False">
                <asp:ListItem Value="-1">全部</asp:ListItem>
                <asp:ListItem Value="待處理">待處理</asp:ListItem>
                <asp:ListItem Value="已處理">已處理</asp:ListItem>
                <asp:ListItem Value="已結案">已結案</asp:ListItem>
            </asp:DropDownList>

            &nbsp;&nbsp;<asp:Button ID="btn_Search" runat="server" CssClass="style2" 
                onclick="btn_Search_Click" Text="查詢" />
            &nbsp;&nbsp;<asp:Button ID="btn_XLS" runat="server" CssClass="style2" 
                onclick="btn_XLS_Click" Text="匯出XLS" BackColor="#FF9933" 
                BorderColor="Red" BorderStyle="Outset" BorderWidth="3px" 
                ForeColor="#CC0000" />
            &nbsp;
            <asp:CheckBox ID="CB_Date" runat="server" CssClass="style2" Text="不限日期範圍" 
                Checked="False"/>
                <br />
            <asp:CompareValidator ID="CompareValidator3" runat="server" 
                ControlToValidate="txt_Start" CssClass="style3" ErrorMessage="開始日期格式錯誤" 
                Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
            &nbsp;<asp:CompareValidator ID="CompareValidator4" runat="server" 
                ControlToValidate="txt_End" CssClass="style3" ErrorMessage="結束日期格式錯誤" 
                Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
            &nbsp;<asp:CompareValidator ID="CompareValidator1" runat="server" 
                ControlToCompare="txt_End" ControlToValidate="txt_Start" CssClass="style3" 
                ErrorMessage="結束日期必須大於開始日期" Operator="LessThanEqual"></asp:CompareValidator>
            </div>
    </asp:Panel>
    <asp:Panel ID="P_list" runat="server">
        <div id="div_list" runat="server">
            <hr />
            &nbsp;<asp:Label ID="lbl_Count" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>

            &nbsp;<asp:Label ID="lbl_Message" runat="server" Text="" CssClass="style3"></asp:Label>

            <asp:GridView ID="gv_List" runat="server" CellPadding="4" CellSpacing="2" 
                CssClass="style4gv" ForeColor="#333333" GridLines="None" 
                onpageindexchanging="gv_List_PageIndexChanging" 
                onrowdatabound="gv_List_RowDataBound" Width="100%" EmptyDataText="無資料">
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
    </asp:Panel>
    </form>
</body>
</html>
