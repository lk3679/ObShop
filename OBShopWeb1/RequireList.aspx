<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RequireList.aspx.cs" Inherits="OBShopWeb.RequireList" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server"> 
     <title>需求單列表</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<script type="text/javascript" src="/js/jquery.js"></script>
<script type="text/javascript" src="/js/lightbar2.js"></script>
<script type="text/javascript">
    d2 = "#DDDDDD";
</script>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <div>
        
        <asp:Panel ID="P_header" runat="server">
            <div id="div_header" runat="server">
                <span class="style1">需求單列表</span>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:HyperLink ID="HL_AddRequire" runat="server" CssClass="style2" 
                    NavigateUrl="RequireAdd.aspx" ForeColor="#FF6600" Target="_blank">※新增需求單※</asp:HyperLink>
                <br />
                <br />
                <span class="style2">狀態：</span>
                <asp:DropDownList ID="DDL_Status" runat="server" CssClass="style2" ForeColor="#0066FF">
                    <asp:ListItem Value="-2">全部</asp:ListItem>
                    <asp:ListItem Value="1" Selected="True">未建傳票</asp:ListItem>
                    <asp:ListItem Value="10">已建傳票</asp:ListItem>
                    <asp:ListItem Value="-1">取消</asp:ListItem>
                </asp:DropDownList>
                &nbsp;
                <span class="style2">種類：</span>
                <asp:DropDownList ID="DDL_Type" runat="server" CssClass="style2" ForeColor="#0066FF">
                    <asp:ListItem Value="-1" Selected="True">全部</asp:ListItem>
                    <asp:ListItem Value="0">調撥</asp:ListItem>
                    <asp:ListItem Value="1">瑕疵</asp:ListItem>
                    <asp:ListItem Value="2">問題件</asp:ListItem>
                </asp:DropDownList>
                &nbsp;
                <span class="style2" style="color: #006666">特定需求單/傳票/採購單：</span>
                <asp:TextBox ID="txt_特定需求單" runat="server" CssClass="style2" Width="250px" ForeColor="#006666"></asp:TextBox>
                <br />
                <span class="style2">開始日期：</span>
                <asp:TextBox ID="txt_Start" runat="server" CssClass="style2" Width="100px"></asp:TextBox>
                <asp:CalendarExtender ID="ImgBtn_Date_CalendarExtender" runat="server" DaysModeTitleFormat="yyyy-MM-dd"
                    Enabled="True" Format="yyyy-MM-dd" PopupButtonID="Imgbtn_Date" TargetControlID="txt_Start"
                    TodaysDateFormat="yyyy-MM-dd">
                </asp:CalendarExtender>
                &nbsp;<asp:ImageButton ID="ImgBtn_Date" runat="server" ImageUrl="~/Image/Calendar.png" />
                &nbsp;
                <%--<br />--%>
                <span class="style2">結束日期：</span><asp:TextBox ID="txt_End" runat="server" CssClass="style2"
                    Width="100px"></asp:TextBox>
                <asp:CalendarExtender ID="ImgBtn_Date2_CalendarExtender" runat="server" DaysModeTitleFormat="yyyy-MM-dd"
                    Enabled="True" Format="yyyy-MM-dd" PopupButtonID="ImgBtn_Date2" TargetControlID="txt_End"
                    TodaysDateFormat="yyyy-MM-dd">
                </asp:CalendarExtender>
                &nbsp;<asp:ImageButton ID="ImgBtn_Date2" runat="server" ImageUrl="~/Image/Calendar.png" />
                &nbsp;

                <asp:Button ID="btn_Search" runat="server" CssClass="style2" OnClick="btn_Search_Click"
                    Text="查詢" />
                &nbsp;<asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txt_End"
                    ControlToValidate="txt_Start" CssClass="style3" ErrorMessage="結束日期必須大於開始日期" Operator="LessThanEqual"></asp:CompareValidator>
                    <asp:CompareValidator ID="CompareValidator3" runat="server" CssClass="style3"
                    ErrorMessage="日期格式錯誤" Operator="DataTypeCheck" ControlToValidate="txt_Start"
                    Type="Date"></asp:CompareValidator><asp:CompareValidator ID="CompareValidator4" runat="server" CssClass="style3"
                    ErrorMessage="日期格式錯誤" Operator="DataTypeCheck" ControlToValidate="txt_End" Type="Date"></asp:CompareValidator>
            </div>
        </asp:Panel>
        <hr /> 

        &nbsp;<asp:Label ID="lbl_Count" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>

        <asp:GridView ID="gv_List" runat="server" CellPadding="2" CellSpacing="2" CssClass="style4gv"
            ForeColor="#333333" GridLines="None" Width="1300px" PageSize="50" 
            EmptyDataText="無資料" onrowdatabound="gv_List_RowDataBound" 
            AutoGenerateColumns="False">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField HeaderText="序號" DataField="序號"/>
                <asp:BoundField HeaderText="需求單ID" DataField="需求單ID"/>
                <asp:BoundField HeaderText="種類" DataField="種類"/>
                <asp:BoundField HeaderText="產品編號" DataField="產品編號"/>
                <asp:BoundField HeaderText="預撥數量" DataField="預撥數量"/>
                <asp:BoundField HeaderText="實際數量" DataField="實際數量"/>
                <asp:BoundField HeaderText="申請人" DataField="申請人"/>
                <asp:BoundField HeaderText="申請時間" DataField="申請時間"/>
                <asp:BoundField HeaderText="狀態" DataField="狀態"/>
                <asp:BoundField HeaderText="審核人" DataField="審核人"/>
                <asp:TemplateField HeaderText="採購ID">
                    <ItemTemplate>
                        <asp:HyperLink ID="BId" runat="server" Text='<%# (Eval("採購ID").ToString() != "0") ? Eval("採購ID") : "未建立" %>' Target="_blank" 
                            NavigateUrl='<%# (Eval("採購ID").ToString() != "0") ? string.Format("http://erp03.obdesign.com.tw/admin/order/modifyorder.aspx?id={0}", Eval("採購ID")) : "" %>'></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="傳票ID" DataField="傳票ID"/>
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
        <hr />
        <br />
    </div>
    </form>
</body>
</html>
