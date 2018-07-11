<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RequireAdd.aspx.cs" Inherits="OBShopWeb.RequireAdd" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>新增調出需求單</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <%--<script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/lightbar.js"></script>
    <script type="text/javascript">
        d1 = "#DDDDDD";
    </script>--%>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <div>
    <span class="style1"><strong>新增調出需求單</strong></span>
    &nbsp;&nbsp;
    <span class="style2" style="color: #006600"><strong>(可查詢儲位：散貨、標準)</strong></span>
    &nbsp;&nbsp;
    
    <asp:CheckBox ID="CB_自動查儲位" runat="server" Text="自動查儲位" CssClass="style2" 
            ForeColor="#9900CC" Checked="True" Visible="False" />
    &nbsp;&nbsp;
    <asp:Button ID="btn_Xls2" runat="server" Text="銷售報表XLS" CssClass="style2" onclick="btn_Xls2_Click" 
         BackColor="#FF9933" BorderColor="Red" BorderStyle="Outset" BorderWidth="3px" ForeColor="#CC0000" />
    <br />
    <br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>

    <span class="style1">1.查詢庫存銷售報表</span>
    <br />
    <fieldset class="style2" style="color: #003300" >
    <legend>報表查詢</legend>
    
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
    <asp:ImageButton ID="ImgBtn_Date2" runat="server" ImageUrl="~/Image/Calendar.png" />
    &nbsp;&nbsp;
    <span class="style2" style="color: #9900CC">銷售數量：</span>
            <asp:DropDownList ID="DDL_SaleRange" runat="server" CssClass="style2" 
            ForeColor="#6600CC">
                <asp:ListItem Value="-1">不限制</asp:ListItem>
                <asp:ListItem Value="0">0</asp:ListItem>
                <asp:ListItem Value="1" Selected="True">1~10</asp:ListItem>
                <asp:ListItem Value="2">10~20</asp:ListItem>
                <asp:ListItem Value="3">20~30</asp:ListItem>
                <asp:ListItem Value="4">30~50</asp:ListItem>
                <asp:ListItem Value="5">50以上</asp:ListItem>
            </asp:DropDownList>
    &nbsp;&nbsp;
    <span class="style2">查詢類別：</span>
            <asp:DropDownList ID="DDL_SearchType" runat="server" CssClass="style2" 
                ForeColor="#006600">
                <asp:ListItem Value="0">庫存銷售</asp:ListItem>
                <asp:ListItem Value="1">瑕疵</asp:ListItem>
                <asp:ListItem Value="2">問題件</asp:ListItem>
                <asp:ListItem Value="3">展售未上架</asp:ListItem>
            </asp:DropDownList>
    &nbsp;&nbsp;
    <asp:CheckBox ID="CB_過濾補貨" runat="server" Checked="True" Text="過濾已補貨(未上架用)" ForeColor="#FF3300" />
    &nbsp;&nbsp;
    <asp:CheckBox ID="CB_過濾展售下架" runat="server" Checked="False" Text="過濾展售下架(當日)" ForeColor="#FF3300" />
    &nbsp;&nbsp;
    <asp:CheckBox ID="CB_過濾在途回途" runat="server" Checked="True" Text="過濾在途/回途" ForeColor="#0066CC" />
    &nbsp;&nbsp;
    <asp:CheckBox ID="CB_強制勾選" runat="server" Checked="False" Text="強制勾選" ForeColor="#0066CC" />
    <br />
    <span class="style2">系列：</span>
    <asp:TextBox ID="txt_SerialId" runat="server" CssClass="style2" Width="120px" ForeColor="#0066FF"></asp:TextBox>
    &nbsp;
    <span class="style2">產品編號：</span>
    <asp:TextBox ID="txt_ProductId" runat="server" CssClass="style2" Width="120px" ForeColor="#0066FF"></asp:TextBox>
    &nbsp;
    <span class="style2">儲位編號：</span>
    <asp:TextBox ID="txt_ShelfId" runat="server" CssClass="style2" Width="120px" ForeColor="#0066FF"></asp:TextBox>
    &nbsp;&nbsp;&nbsp;
    <asp:Button ID="btn_Clear_Txt" runat="server" CssClass="style2" OnClick="btn_Clear_Txt_Click" Text="清除輸入" />
    &nbsp;&nbsp;&nbsp;
    <asp:Button ID="btn_查詢報表" runat="server" Text="查詢報表" CssClass="style2" 
            onclick="btn_查詢報表_Click"/>
    &nbsp;
    <asp:CheckBox ID="CB_Date" runat="server" CssClass="style2" Text="不限銷售日期範圍" Checked="False" />
    <br />
    <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToValidate="txt_Start"
        CssClass="style3" ErrorMessage="開始日期格式錯誤" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
    &nbsp;<asp:CompareValidator ID="CompareValidator4" runat="server" ControlToValidate="txt_End"
        CssClass="style3" ErrorMessage="結束日期格式錯誤" Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
    &nbsp;<asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txt_End"
        ControlToValidate="txt_Start" CssClass="style3" ErrorMessage="結束日期必須大於開始日期" Operator="LessThanEqual"></asp:CompareValidator>
    <hr />
    <asp:Panel ID="P_Report" runat="server">
    <asp:CheckBox ID="CB_ReportSelect" runat="server" Text="全選/全不選" AutoPostBack="True" oncheckedchanged="CB_ReportSelect_CheckedChanged" />
    &nbsp;&nbsp;<asp:Label ID="lbl_ReportCount" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>
    <asp:GridView ID="gv_Report" runat="server" CellPadding="4" CellSpacing="2" 
        CssClass="style4gv" ForeColor="#333333" GridLines="None" Width="1600px" 
        EmptyDataText="無資料" AutoGenerateColumns="False" onrowdatabound="gv_Report_RowDataBound">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:TemplateField HeaderText="選擇" ItemStyle-Width="50px">
                <ItemTemplate>
                    <asp:CheckBox ID="CB_Detail" runat="server" Checked="False" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="序號" DataField="序號" ItemStyle-Width="50px" />
            <asp:BoundField HeaderText="系列編號" DataField="系列編號" />
            <asp:BoundField HeaderText="品名" DataField="品名" ItemStyle-Width="450px"/>
            <asp:BoundField HeaderText="型號" DataField="型號" />
            <asp:BoundField HeaderText="顏色" DataField="顏色"/>
            <asp:BoundField HeaderText="尺寸" DataField="尺寸"/>
            <asp:BoundField HeaderText="展售" DataField="展售庫存"/>
            <asp:BoundField HeaderText="補貨" DataField="補貨庫存"/>
            <asp:BoundField HeaderText="一般" DataField="一般庫存" />
            <asp:BoundField HeaderText="暫存" DataField="暫存庫存"/>
            <asp:BoundField HeaderText="在途" DataField="在途庫存" />
            <asp:BoundField HeaderText="回途" DataField="回途庫存" />
            <asp:BoundField HeaderText="瑕疵" DataField="瑕疵庫存" />
            <asp:BoundField HeaderText="銷售" DataField="銷售" />
            <asp:BoundField HeaderText="售價" DataField="售價" />
            <asp:BoundField HeaderText="最後上架日" DataField="上架日" />
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
    </asp:Panel>
    <asp:Label ID="lbl_Count" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>
        <asp:GridView ID="gv_List" runat="server" CellSpacing="1" CellPadding="4" style="font-family: 微軟正黑體;text-align: center;"
            ForeColor="#333333" GridLines="None" Width="1700px" EmptyDataText="無資料" 
            AutoGenerateColumns="False" OnRowCommand="Grid_RowCommand" 
            EnableModelValidation="True" onrowdatabound="gv_List_RowDataBound">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:BoundField HeaderText="序號" DataField="序號" ItemStyle-Width="50px" />
                <asp:BoundField HeaderText="系列編號" DataField="系列編號" />
                <asp:BoundField HeaderText="品名" DataField="品名" ItemStyle-Width="450px"/>
                <asp:BoundField HeaderText="展售" DataField="展售庫存"/>
                <asp:BoundField HeaderText="一般" DataField="一般庫存" />
                <asp:BoundField HeaderText="暫存" DataField="暫存庫存"/>
                <asp:BoundField HeaderText="在途" DataField="在途庫存" />
                <asp:BoundField HeaderText="回途" DataField="回途庫存" />
                <asp:BoundField HeaderText="瑕疵" DataField="瑕疵庫存" />
                <asp:BoundField HeaderText="銷售" DataField="銷售" />
                <asp:BoundField HeaderText="售價" DataField="售價" />
                <asp:TemplateField HeaderText="備註" ItemStyle-Width="700px">
                    <ItemTemplate>
                        <asp:LinkButton ID="Detail" CommandName="Detail" CommandArgument='<%#Eval("系列編號")%>'
                            runat="server" Text='<%# string.Format("{0} SKU清單", Eval("系列編號")) %>'></asp:LinkButton>
                                                
                            <asp:Panel ID="P_Detail" runat="server" Visible="False">
                                <asp:CheckBox ID="CB_Detail_Select" runat="server" Text="全選" Visible="False" />
                                <asp:LinkButton ID="Detail_Select" CommandName="CBDetail" CommandArgument='<%#Eval("系列編號")%>' runat="server" Text="全選/全不選"></asp:LinkButton>
                                    <asp:GridView ID="gv_Detail" runat="server" CellSpacing="1" CellPadding="2" style="font-family: 微軟正黑體;text-align: center;"
                                        ForeColor="#333333" GridLines="None" Width="100%" EmptyDataText="無資料" AutoGenerateColumns="False" onrowdatabound="gv_Detail_RowDataBound">
                                        <Columns>
                                            <asp:TemplateField HeaderText="選擇" ItemStyle-Width="50px">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="CB_Detail" runat="server"/>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField HeaderText="型號" DataField="型號" />
                                            <asp:BoundField HeaderText="顏色" DataField="顏色"/>
                                            <asp:BoundField HeaderText="尺寸" DataField="尺寸"/>
                                            <asp:BoundField HeaderText="展售" DataField="展售庫存"/>
                                            <asp:BoundField HeaderText="一般" DataField="一般庫存" />
                                            <asp:BoundField HeaderText="暫存" DataField="暫存庫存"/>
                                            <asp:BoundField HeaderText="在途" DataField="在途庫存" />
                                            <asp:BoundField HeaderText="回途" DataField="回途庫存" />
                                            <asp:BoundField HeaderText="瑕疵" DataField="瑕疵庫存" />
                                            <asp:BoundField HeaderText="銷售" DataField="銷售" />
                                            <asp:BoundField HeaderText="最後上架日" DataField="上架日" />
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
    <asp:Button ID="btn_匯出勾選產品" runat="server" Text="匯出勾選產品" CssClass="style2" 
            onclick="btn_匯出勾選產品_Click" />
    </fieldset>

    <br />
    <hr />
    <asp:Panel ID="P_SerialList" runat="server">
        <span class="style2" style="color: #006600">●產品編號(用換行或用","分隔) ※請注意是否有全形空白</span>
        <br />
        <asp:TextBox ID="txt_系列" runat="server" CssClass="style2" MaxLength="200" 
                Width="281px" ForeColor="#0066FF" Height="375px" TextMode="MultiLine"></asp:TextBox>
        &nbsp;
        <asp:Button ID="btn_Submit" runat="server" Text="檢查儲位產品" CssClass="style2" onclick="btn_Submit_Click"/>
        &nbsp;
        <asp:CheckBox ID="CB_不良" runat="server" Text="只查不良儲位" CssClass="style2" ForeColor="#9900CC" />
        &nbsp;
        <asp:Label ID="lbl_Message" runat="server" CssClass="style3" Text="" ></asp:Label>
    </asp:Panel>

    <hr />
    <br />
    <span class="style1">2.確認儲位產品→新增需求單</span>
    <br />
    &nbsp;<asp:Label ID="lbl_FCount" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>

    <asp:GridView ID="gv_FList" runat="server" CellPadding="4" CellSpacing="2" 
        CssClass="style4gv" ForeColor="#333333" GridLines="None" Width="600px" 
        EmptyDataText="無資料" AutoGenerateColumns="False" 
            onrowdatabound="gv_FList_RowDataBound">
        <Columns>
            <asp:BoundField HeaderText="序號" DataField="序號" ItemStyle-Width="50px">
            <ItemStyle Width="50px" />
            </asp:BoundField>
            <asp:BoundField HeaderText="產品編號" DataField="產品編號" />
            <asp:BoundField HeaderText="儲位編號" DataField="儲位編號" />
            <asp:BoundField HeaderText="儲位數量" DataField="數量" />
            <asp:BoundField HeaderText="類型" DataField="類型"/>
            <asp:TemplateField HeaderText="調出數">
                <ItemTemplate>
                    <asp:TextBox ID="txt_num" runat="server" CssClass="style2" Width="50px" Text ='<%#Eval("數量")%>' ForeColor="#0066CC"></asp:TextBox>
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
    
    &nbsp;&nbsp;
    <asp:Button ID="btn_ADD" runat="server" Text="確定新增調出需求單" CssClass="style2" 
            onclick="btn_ADD_Click" Enabled="False"/>
    
    <asp:Label ID="lblMsg" runat="server" CssClass="style3" Text=""></asp:Label>
    </ContentTemplate>
    </asp:UpdatePanel>
    &nbsp;&nbsp;
    <asp:Button ID="btnXls" runat="server" Text="匯出XLS" CssClass="style2" onclick="btnXls_Click" 
         BackColor="#FF9933" BorderColor="Red" BorderStyle="Outset" BorderWidth="3px" ForeColor="#CC0000" />
    &nbsp;&nbsp;
    <asp:Button ID="btn_Print" runat="server" Text="列印儲位明細(一張50筆)" CssClass="style2" onclick="btn_Print_Click"/>
    <hr />
    </div>
    </form>
</body>
</html>