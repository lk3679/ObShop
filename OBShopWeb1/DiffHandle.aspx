<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DiffHandle.aspx.cs" Inherits="OBShopWeb.DiffHandle" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>TW-差異化處理記錄</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<script type="text/javascript" src="js/utils.js"></script>
<script type="text/javascript" src="js/jquery.js"></script>
<script type="text/javascript" src="js/jquery.blockUI.js"></script>
<script type="text/javascript">
    $(document).ready(function () {
        $('#btn_Search').click(function () {
            $.blockUI({
                message: $('<h4 style="text-align:center"><img src="image/loading4.gif" /> <br/><br/>loading...</h4>'),
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
        __doPostBack('btn_Search', '');
    }
    function Cancel(info) {
        if (confirm("確認 " + info + " 取消回報?")) {
            return true;
        }
        else {
            return false;
        }
    }
    function Handle(info) {
        if (confirm("確認 " + info + " 已處理?")) {
            return true;
        }
        else {
            return false;
        }
    }
    function Confirm(info) {
        if (confirm("確認 " + info + " 修正傳票 已建立?")) {
            return true;
        }
        else {
            return false;
        }
    }
    function Confirm2(info) {
        if (confirm("主管確認 " + info + " 原因 正確?")) {
            return true;
        }
        else {
            return false;
        }
    }
    function Confirm3(info) {
        if (confirm("會計確認 " + info + " 修正傳票 已建立?")) {
            return true;
        }
        else {
            return false;
        }
    }
    function Finish(info) {
        if (confirm("確認 " + info + " 結案?")) {
            return true;
        }
        else {
            return false;
        }
    }
</script>
<script type="text/javascript" src="js/lightbar2.js"></script>
<script type="text/javascript">
    d2 = "#DDDDDD";
</script>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:Panel ID="P_header" runat="server">
        <div id="div_header" runat="server">
            <span class="style1">TW-差異化處理記錄</span>
            <asp:Label ID="lbl_Pos" runat="server" CssClass="style1" Visible="False" Text="(門市)"></asp:Label>
            <asp:Label ID="lbl_ShipOutType" runat="server" CssClass="style1" ForeColor="White"></asp:Label>
            <br />
            <br />
            <span class="style2">傳票號碼：</span>
            <asp:TextBox ID="txt_Ticket" runat="server" Width="100px" CssClass="style2"></asp:TextBox>
            &nbsp;<asp:Button ID="btn_Ticket_Search" runat="server" CssClass="style2" OnClick="btn_Ticket_Search_Click"
                Text="傳票查詢" OnClientClick="return disable1(this)" /><br />
            <span class="style2">日期範圍：</span>
            <asp:TextBox ID="txt_Start" runat="server" CssClass="style2" Width="100px"></asp:TextBox>
            <asp:CalendarExtender ID="ImgBtn_Date_CalendarExtender" runat="server" DaysModeTitleFormat="yyyy-MM-dd"
                Enabled="True" Format="yyyy-MM-dd" PopupButtonID="Imgbtn_Date" TargetControlID="txt_Start"
                TodaysDateFormat="yyyy-MM-dd">
            </asp:CalendarExtender>
            &nbsp;<asp:ImageButton ID="ImgBtn_Date" runat="server" ImageUrl="~/Image/Calendar.png" />
            <span class="style2">～</span><asp:TextBox ID="txt_End" runat="server" CssClass="style2"
                Width="100px"></asp:TextBox>
            <asp:CalendarExtender ID="ImgBtn_Date2_CalendarExtender" runat="server" DaysModeTitleFormat="yyyy-MM-dd"
                Enabled="True" Format="yyyy-MM-dd" PopupButtonID="ImgBtn_Date2" TargetControlID="txt_End"
                TodaysDateFormat="yyyy-MM-dd">
            </asp:CalendarExtender> 
            &nbsp;
            <asp:CheckBox ID="CB_Date" runat="server" CssClass="style2" Text="不限日期範圍(最多六個月)" />
            <br />
            <span class="style2">類別：</span>
            <asp:DropDownList ID="DDL_Type" runat="server" CssClass="style2">
                <asp:ListItem Value="-1">全部</asp:ListItem>
                <asp:ListItem Value="1" Enabled="False">出貨</asp:ListItem>
                <asp:ListItem Value="2" Enabled="False">海運</asp:ListItem>
                <asp:ListItem Value="3" Enabled="False">寄倉</asp:ListItem>
                <asp:ListItem Value="4" Enabled="False">版借出</asp:ListItem>
                <asp:ListItem Value="5" Enabled="False">瑕疵</asp:ListItem>
                <asp:ListItem Value="9" Enabled="False">調出</asp:ListItem>
                <asp:ListItem Value="11" Enabled="False">調回</asp:ListItem>
                <asp:ListItem Value="12" Enabled="False">強制移儲</asp:ListItem>
                <asp:ListItem Value="13" Enabled="False">台直</asp:ListItem>
                <asp:ListItem Value="18" Enabled="False">台組進貨</asp:ListItem>
                <asp:ListItem Value="28" Selected="True">門市進貨</asp:ListItem>
            </asp:DropDownList>
            &nbsp; <span class="style2">狀態：</span>
            <asp:DropDownList ID="DDL_Status" runat="server" CssClass="style2">
                <asp:ListItem Value="-1">全部</asp:ListItem>
                <asp:ListItem Value="待處理">待處理</asp:ListItem>
                <asp:ListItem Value="已處理" Selected="True">已處理(驗貨)</asp:ListItem>
                <asp:ListItem Value="已結案">已結案</asp:ListItem>
            </asp:DropDownList>
            &nbsp; 
            <%--<span class="style2">處理流程：</span>
            <asp:DropDownList ID="DDL_ServiceType" runat="server" CssClass="style2" ForeColor="#006666">
                <asp:ListItem Value="-1">全部</asp:ListItem>
                <asp:ListItem Value="0">待查原因</asp:ListItem>
                <asp:ListItem Value="1">【主管】未確認</asp:ListItem>
                <asp:ListItem Value="2">【會計】未確認</asp:ListItem>
                <asp:ListItem Value="3">【會計】已確認</asp:ListItem>
            </asp:DropDownList>
            &nbsp;--%>
            <asp:Button ID="btn_Search" runat="server" CssClass="style2" OnClick="btn_Search_Click"
                OnClientClick="return disable3(this)" Text="查詢" />
            &nbsp;&nbsp;
            <asp:Button ID="btn_Xls" runat="server" Text="匯出XLS" CssClass="style13PDA" onclick="btn_Xls_Click" 
                    BackColor="#FF9933" BorderColor="Red" BorderStyle="Outset" BorderWidth="3px" ForeColor="#CC0000" Visible="True" />
            &nbsp;
            <asp:Label ID="lbl_Message" runat="server" Text="" CssClass="style3"></asp:Label>
            <br />
            <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToValidate="txt_Start"
                CssClass="style3" ErrorMessage="開始日期格式錯誤" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
            &nbsp;<asp:CompareValidator ID="CompareValidator4" runat="server" ControlToValidate="txt_End"
                CssClass="style3" ErrorMessage="結束日期格式錯誤" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
            &nbsp;<asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txt_End"
                ControlToValidate="txt_Start" CssClass="style3" ErrorMessage="結束日期必須大於開始日期" Operator="LessThanEqual"></asp:CompareValidator>
            <%--<br />
            <span class="style2" style="color: #008080">※備註(【傳票確認】必填)：</span>--%>
            <asp:TextBox ID="txt_Comment" runat="server" Width="400px" CssClass="style2" Visible="False" ></asp:TextBox>
            <asp:CheckBox ID="CB_Comment" runat="server" Enabled="False" Checked="True" CssClass="style2" Text="必填" ForeColor="#008080" Visible="False" />
        </div>
    </asp:Panel>
    <asp:Panel ID="P_list" runat="server">
        <div id="div_list" runat="server">
            <hr />
            <asp:Label ID="lbl_Count" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>

            <asp:GridView ID="gv_List" runat="server" CellPadding="4" CellSpacing="2" CssClass="style4gv"
                ForeColor="#333333" GridLines="None" OnPageIndexChanging="gv_List_PageIndexChanging"
                OnRowDataBound="gv_List_RowDataBound" Width="100%" EmptyDataText="無資料" AutoGenerateColumns="False">
                <Columns>
                    <%--<asp:BoundField HeaderText="" DataField="序號"/>--%>
                    <asp:BoundField HeaderText="類別" DataField="類別" />
                    <asp:BoundField HeaderText="Id" DataField="Id" />
                    <asp:TemplateField HeaderText="傳票">
                        <ItemTemplate>
                            <asp:HyperLink ID="TicketId" runat="server" Text='<%# Eval("傳票") %>' Target="_blank" 
                                NavigateUrl='<%# ((Eval("類別").ToString() != "出貨" && Eval("類別").ToString() != "補撿")) ? 
                                string.Format("http://erp03.obdesign.com.tw/admin/products/checked.aspx?SendID={0}", Eval("傳票")) : "" %>'></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="箱號" DataField="箱號" />
                    <asp:BoundField HeaderText="廠商" DataField="調出廠商" Visible="False" />
                    <asp:BoundField HeaderText="帳號" DataField="帳號" />
                    <asp:BoundField HeaderText="出貨序號" DataField="出貨序號" />
                    <asp:BoundField HeaderText="產品編號" DataField="產品編號" />
                    <asp:BoundField HeaderText="數量" DataField="數量" />
                    <asp:BoundField HeaderText="回報人" DataField="回報人" />
                    <asp:BoundField HeaderText="回報日期" DataField="回報日期" />
                    <asp:BoundField HeaderText="處理人" DataField="處理人" />
                    <asp:BoundField HeaderText="處理日期" DataField="處理日期" />
                    <asp:BoundField HeaderText="結案人" DataField="結案人" />
                    <asp:BoundField HeaderText="結案日期" DataField="結案日期" />
                    <asp:BoundField HeaderText="處理狀態" DataField="處理狀態" />
                    <%--<asp:BoundField HeaderText="主管" DataField="主管" />
                    <asp:BoundField HeaderText="會計" DataField="會計" />--%>
                    <asp:BoundField HeaderText="備註" DataField="備註" />
                    <asp:BoundField HeaderText="備註HU" DataField="備註HU"/>
                    <asp:TemplateField HeaderText="功能">
                        <ItemTemplate>
                            <asp:Button ID="btn_Cancel" runat="server" Text="取消回報" OnClick="btn_Cancel_Click" CssClass="style2" ticket = '<%# Eval("Id")%>'
                                Visible="False" OnClientClick='<%# string.Format("return Cancel(\"【{0}】【{1}】\");", Eval("傳票"), Eval("產品編號")) %>'/>
                            <asp:Button ID="btn_Handle" runat="server" Text="處理" OnClick="btn_Handle_Click" CssClass="style2" ticket = '<%# Eval("傳票")%>'
                                Visible="False" OnClientClick='<%# string.Format("return Handle(\"【{0}】【{1}】\");", Eval("傳票"), Eval("產品編號")) %>'/>
                            <asp:Button ID="btn_Finish" runat="server" Text="結案" OnClick="btn_Finish_Click" CssClass="style2" ticket = '<%# Eval("傳票")%>'
                                Visible="False" OnClientClick='<%# string.Format("return Finish(\"【{0}】【{1}】\");", Eval("傳票"), Eval("產品編號")) %>'/>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
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
