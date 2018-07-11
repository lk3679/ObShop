<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderList.aspx.cs" Inherits="OBShopWeb.OrderList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>出貨清單狀態(補印單)</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/lightbar2.js"></script>
    <script type="text/javascript">
        d2 = "#DDDDDD";
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    
    <div>
    <span class="style1"><strong>出貨清單狀態(補印單)</strong></span>
    &nbsp;&nbsp;
    <%--<span class="style2" style="color: #9900CC"><strong>(每分鐘刷新一次)</strong></span>--%>
    <br />
    <br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <asp:Timer ID="Timer1" runat="server" Interval="60000" ontick="Timer1_Tick">
    </asp:Timer>
    <br />
    <span class="style2">銷售日期範圍：</span>
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
    
    <asp:Button ID="btn_查詢" runat="server" Text="查詢" CssClass="style2" onclick="btn_查詢_Click"/>
    &nbsp;
    &nbsp;
    <asp:CheckBox ID="CB_未撿貨確認" runat="server" CssClass="style2" ForeColor="#006600" Text="未撿貨確認" Checked="True" />
    &nbsp;&nbsp;
    <asp:CheckBox ID="CB_Timer" runat="server" CssClass="style2" ForeColor="#6600CC" Text="每分鐘自動刷新" Checked="True" />
    &nbsp;
    <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text=""></asp:Label>

    <br />
    <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToValidate="txt_Start"
        CssClass="style3" ErrorMessage="開始日期格式錯誤" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
    &nbsp;<asp:CompareValidator ID="CompareValidator4" runat="server" ControlToValidate="txt_End"
        CssClass="style3" ErrorMessage="結束日期格式錯誤" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
    &nbsp;<asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txt_End"
        ControlToValidate="txt_Start" CssClass="style3" ErrorMessage="結束日期必須大於開始日期" Operator="LessThanEqual"></asp:CompareValidator>
    <hr />
    
    <asp:Label ID="lbl_Count" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>
        <asp:GridView ID="gv_List" runat="server" CellSpacing="1" CellPadding="4" style="font-family: 微軟正黑體;text-align: center;"
            ForeColor="#333333" GridLines="None" Width="1500px" EmptyDataText="無資料" 
            AutoGenerateColumns="False" OnRowCommand="Grid_RowCommand" 
            EnableModelValidation="True" onrowdatabound="gv_List_RowDataBound">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:BoundField HeaderText="序號" DataField="序號" ItemStyle-Width="50px" />
                <asp:BoundField HeaderText="出貨序號" DataField="出貨序號" />
                <asp:BoundField HeaderText="交易時間" DataField="交易時間" ItemStyle-Width="150px"/>
                <asp:BoundField HeaderText="收銀機號" DataField="收銀機號"/>
                <asp:BoundField HeaderText="收銀員" DataField="收銀員" />
                <asp:BoundField HeaderText="總金額" DataField="總金額" />
                <asp:BoundField HeaderText="產品數量" DataField="產品數量" />
                <asp:BoundField HeaderText="發票號碼" DataField="發票號碼" />
                <asp:BoundField HeaderText="撿貨者" DataField="撿貨者" />
                <asp:BoundField HeaderText="訂單狀態" DataField="訂單狀態" />
                <asp:TemplateField HeaderText="功能" ItemStyle-Width="400px">
                    <ItemTemplate>
                        <asp:LinkButton ID="Detail" CommandName="Detail" CommandArgument='<%#Eval("出貨序號")%>'
                            runat="server" Text='<%# string.Format("{0} 明細", Eval("出貨序號")) %>'></asp:LinkButton>
                        &nbsp;&nbsp;
                        <asp:LinkButton ID="Print" CommandName="Print" CommandArgument='<%#Eval("出貨序號")%>'
                            runat="server" Text='<%# string.Format("{0} 補印撿貨單", Eval("出貨序號")) %>'></asp:LinkButton>
                        &nbsp;&nbsp;
                        <asp:HyperLink ID="PDF" runat="server" Text='<%# string.Format("PDF") %>' NavigateUrl='<%#Eval("PDF")%>' Target="_blank" Visible="False"></asp:HyperLink>                                                
                            <asp:Panel ID="P_Detail" runat="server" Visible="False">
                                     <asp:GridView ID="gv_Detail" runat="server" CellSpacing="1" CellPadding="2" style="font-family: 微軟正黑體;text-align: center;"
                                        ForeColor="#333333" GridLines="None" Width="100%" EmptyDataText="無資料" AutoGenerateColumns="False" onrowdatabound="gv_Detail_RowDataBound">
                                        <Columns>
                                            <asp:BoundField HeaderText="序號" DataField="序號" />
                                            <asp:BoundField HeaderText="型號" DataField="型號" />
                                            <asp:BoundField HeaderText="條碼" DataField="條碼"/>
                                            <asp:BoundField HeaderText="數量" DataField="數量"/>
                                        </Columns>
                                        <AlternatingRowStyle BackColor="White" />
                                        <EditRowStyle BackColor="#7C6F57" />
                                        <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                        <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                                        <RowStyle BackColor="#E3EAEB" />
                                        <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                </asp:GridView>
                            </asp:Panel>          
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EditRowStyle BackColor="#999999" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
    </asp:GridView>

    </ContentTemplate>
    </asp:UpdatePanel>
    <hr />
    </div>
    </form>
</body>
</html>
