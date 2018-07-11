<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockReport.aspx.cs" Inherits="OBShopWeb.StockReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>庫存報表</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <div>
    <span class="style1">庫存報表</span>
    &nbsp;&nbsp;
    <asp:Button ID="btn_Xls" runat="server" Text="報表XLS" CssClass="style2" onclick="btn_Xls_Click" 
         BackColor="#FF9933" BorderColor="Red" BorderStyle="Outset" BorderWidth="3px" ForeColor="#CC0000" />
    <br />
    <br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>

    <span class="style2">系列：</span>
    <asp:TextBox ID="txt_SerialId" runat="server" CssClass="style2" Width="120px" ForeColor="#0066FF"></asp:TextBox>
    <span class="style2">產品編號：</span>
    <asp:TextBox ID="txt_ProductId" runat="server" CssClass="style2" Width="120px" ForeColor="#0066FF"></asp:TextBox>
    &nbsp;&nbsp;&nbsp;
    <asp:Button ID="btn_Clear_Txt" runat="server" CssClass="style2" OnClick="btn_Clear_Txt_Click" Text="清除輸入" />
    &nbsp;&nbsp;&nbsp;
    <asp:Button ID="btn_查詢" runat="server" Text="查詢" CssClass="style2" onclick="btn_查詢_Click"/>
    &nbsp;&nbsp;&nbsp;
    <asp:CheckBox ID="CB_分頁" runat="server" CssClass="style2" ForeColor="#006600" Text="分頁" Checked="True" />
    &nbsp;&nbsp;&nbsp;
    <span class="style2">單頁筆數：</span>
    <asp:DropDownList ID="DDL_單頁筆數" runat="server" CssClass="style2" ForeColor="#006600">
        <asp:ListItem Selected="True">500</asp:ListItem>
        <asp:ListItem>2000</asp:ListItem>
        <asp:ListItem>5000</asp:ListItem>
    </asp:DropDownList>
    &nbsp;&nbsp;&nbsp;
    <span class="style2">種類：</span>
    <asp:DropDownList ID="DDL_Type" runat="server" CssClass="style2" ForeColor="#006600">
        <asp:ListItem Selected="True" Value="一般">一般</asp:ListItem>
        <asp:ListItem Value="異常">異常</asp:ListItem>
    </asp:DropDownList>
    <hr />
        
    <asp:Label ID="lbl_Count" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>
        <asp:GridView ID="gv_List" runat="server" CellSpacing="1" CellPadding="4" style="font-family: 微軟正黑體;text-align: center;"
            ForeColor="#333333" GridLines="None" Width="1200px" EmptyDataText="無資料" 
            AutoGenerateColumns="False" onrowdatabound="gv_List_RowDataBound" PagerSettings-Position="TopAndBottom" 
            PageSize="500" onpageindexchanging="gv_List_PageIndexChanging">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
            <asp:BoundField HeaderText="序號" DataField="序號" ItemStyle-Width="50px" />
            <asp:BoundField HeaderText="系列" DataField="系列" />
            <asp:BoundField HeaderText="產品編號" DataField="產品編號"/>
            <asp:BoundField HeaderText="總數" DataField="總數" />
            <asp:BoundField HeaderText="有效" DataField="有效"/>
            <asp:BoundField HeaderText="無效" DataField="無效"/>
            <asp:BoundField HeaderText="預購" DataField="預購"/>
            <asp:BoundField HeaderText="更新日期" DataField="更新日期"/>
            <asp:BoundField HeaderText="最後上架日" DataField="最後上架日" />
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
    </div>
    </form>
</body>
</html>
