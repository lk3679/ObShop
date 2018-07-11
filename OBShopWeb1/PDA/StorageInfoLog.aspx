<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageInfoLog.aspx.cs" Inherits="OBShopWeb.PDA.StorageInfoLog" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>門市-儲位記錄查詢</title>
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
            <span class="style1"><strong>門市-儲位記錄查詢</strong></span>
            <asp:Label ID="lbl_ShipOutType" runat="server" CssClass="style1" ForeColor="White"></asp:Label>
            &nbsp;
            <asp:CheckBox ID="CB_ShowStorageId" runat="server" CssClass="style2" 
                Text="顯示儲位ID" Checked="False" Visible="False" ForeColor="#006699" />
            <br />
            <br />
            <span class="style2">日期範圍：</span>
            <asp:TextBox ID="txt_Start" runat="server" CssClass="style2" Width="100px"></asp:TextBox>
            <asp:CalendarExtender ID="ImgBtn_Date_CalendarExtender" runat="server" DaysModeTitleFormat="yyyy-MM-dd"
                Enabled="True" Format="yyyy-MM-dd" PopupButtonID="Imgbtn_Date" TargetControlID="txt_Start"
                TodaysDateFormat="yyyy-MM-dd"></asp:CalendarExtender>
            &nbsp;<asp:ImageButton ID="ImgBtn_Date" runat="server" ImageUrl="~/Image/Calendar.png" />
            <span class="style2">～</span>
            <asp:TextBox ID="txt_End" runat="server" CssClass="style2" Width="100px"></asp:TextBox>
            <asp:CalendarExtender ID="ImgBtn_Date2_CalendarExtender" runat="server" DaysModeTitleFormat="yyyy-MM-dd"
                Enabled="True" Format="yyyy-MM-dd" PopupButtonID="ImgBtn_Date2" TargetControlID="txt_End"
                TodaysDateFormat="yyyy-MM-dd"></asp:CalendarExtender>
            &nbsp;
            <asp:ImageButton ID="ImgBtn_Date2" runat="server" ImageUrl="~/Image/Calendar.png" />&nbsp;&nbsp;
            
            <asp:Label ID="lbl_shoptype" runat="server" Text="&nbsp;類別：&nbsp;" CssClass="style2"></asp:Label>
            <asp:DropDownList ID="DDL_shoptype" runat="server" CssClass="style2" 
                ForeColor="#006600">
                <asp:ListItem Value="-1">全部</asp:ListItem>
            </asp:DropDownList>
            <br />
            <span class="style2">傳票：</span>
            <asp:TextBox ID="txt_ticketId" runat="server" CssClass="style2" Width="100px" ForeColor="#0066FF"></asp:TextBox>
            <span class="style2">出貨序號：</span>
            <asp:TextBox ID="txt_shipoutId" runat="server" CssClass="style2" Width="100px" ForeColor="#0066FF"></asp:TextBox>
            <span class="style2">產品：</span>
            <asp:TextBox ID="txt_productId" runat="server" CssClass="style2" Width="120px" ForeColor="#0066FF"></asp:TextBox>
            <span class="style2">儲位：</span>
            <asp:TextBox ID="txt_shelfId" runat="server" CssClass="style2" Width="250px" ForeColor="#0066FF"></asp:TextBox>
            &nbsp;&nbsp;
            <asp:Button ID="btn_Search" runat="server" CssClass="style2" OnClick="btn_Search_Click" Text="查詢" />
            &nbsp;
            <asp:Button ID="btn_Clear_Txt" runat="server" CssClass="style2" OnClick="btn_Clear_Txt_Click" Text="清除輸入" />
            &nbsp;
            <asp:CheckBox ID="CB_Date" runat="server" CssClass="style2" Text="不限日期範圍" Checked="False" />
            <br />
            <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToValidate="txt_Start"
                CssClass="style3" ErrorMessage="開始日期格式錯誤" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
            &nbsp;<asp:CompareValidator ID="CompareValidator4" runat="server" ControlToValidate="txt_End"
                CssClass="style3" ErrorMessage="結束日期格式錯誤" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
            &nbsp;<asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txt_End"
                ControlToValidate="txt_Start" CssClass="style3" ErrorMessage="結束日期必須大於開始日期" Operator="LessThanEqual"></asp:CompareValidator>
        </div>
    </asp:Panel>
    <asp:Panel ID="P_list" runat="server">
        <div id="div_list" runat="server">
            <hr />
            <span class="style2" style="color: #006666">【儲位移動】</span>
            <asp:CheckBox ID="CB_Search_ActionLog" runat="server" Checked="True" CssClass="style2"/>
            <br />
            &nbsp;<asp:Label ID="lbl_Count_ActionLog" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>
            <asp:GridView ID="gv_List_ActionLog" runat="server" CellSpacing="1" CellPadding="4" CssClass="style4gv"
                ForeColor="#333333" GridLines="None" Width="1200px" EmptyDataText="無資料">
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
            <hr />
            <span class="style2" style="color: #006666">【出貨暫存】</span>
            <asp:CheckBox ID="CB_Search_ExportLog" runat="server" Checked="True" CssClass="style2"/>
            <br />
            &nbsp;<asp:Label ID="lbl_Count_ExportLog" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>
            <asp:GridView ID="gv_List_ExportLog" runat="server" CellSpacing="1" CellPadding="4" CssClass="style4gv"
                ForeColor="#333333" GridLines="None" Width="1200px" EmptyDataText="無資料">
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
            <hr />
            <span class="style2" style="color: #006666">【入庫暫存】</span>
            <asp:CheckBox ID="CB_Search_ImportLog" runat="server" Checked="True" CssClass="style2"/>
            <br />
            &nbsp;<asp:Label ID="lbl_Count_ImportLog" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>
            <asp:GridView ID="gv_List_ImportLog" runat="server" CellSpacing="1" CellPadding="4" CssClass="style4gv"
                ForeColor="#333333" GridLines="None" Width="1200px" EmptyDataText="無資料">
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
            <hr />
            <span class="style2" style="color: #006666">【差異回報】</span>
            <asp:CheckBox ID="CB_Search_IssueReport" runat="server" Checked="True" CssClass="style2"/>
            <br />
            &nbsp;<asp:Label ID="lbl_Count_IssueReport" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>
            <asp:GridView ID="gv_List_IssueReport" runat="server" CellSpacing="1" CellPadding="4" CssClass="style4gv"
                ForeColor="#333333" GridLines="None" Width="1200px" EmptyDataText="無資料">
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
            <hr />
            <span class="style2" style="color: #006666">【儲位內容】</span>
            <asp:CheckBox ID="CB_Search_StorageDetail" runat="server" Checked="True" CssClass="style2"/>
            <br />
            &nbsp;<asp:Label ID="lbl_Count_StorageDetail" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>
            <asp:GridView ID="gv_List_StorageDetail" runat="server" CellSpacing="1" CellPadding="4" CssClass="style4gv"
                ForeColor="#333333" GridLines="None" Width="1200px" EmptyDataText="無資料">
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
            <hr />
        </div>
    </asp:Panel>
    </form>
</body>
</html>
