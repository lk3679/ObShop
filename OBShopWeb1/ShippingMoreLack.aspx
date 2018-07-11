<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShippingMoreLack.aspx.cs" Inherits="OBShopWeb.ShippingMoreLack" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Cache-Control" content="no-cache" />
    <meta http-equiv="Pragma" content="no-cache" />
    <title>數量異常回報</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/json2.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <span class="style1"><strong>傳票數量異常回報</strong></span>
        <br />
        <br />
        <asp:Panel ID="Panel1" runat="server">
            <span class="style2">傳票：</span>
            <asp:Label ID="lbl_ticket_id" runat="server" Text="" CssClass="style2" ForeColor="#006666"
                Visible="False"></asp:Label>
            <asp:TextBox ID="txt_ticket_id" runat="server" Text="" CssClass="style2" ForeColor="#006666"
                Width="90px" Enabled="False"></asp:TextBox>
            &nbsp;&nbsp; <span class="style2">類別：</span>
            <asp:Label ID="lbl_FlowStatus" runat="server" Text="" CssClass="style2" ForeColor="#006666"></asp:Label>
            &nbsp;&nbsp;
            <asp:Label ID="lbl_Reason" runat="server" Text="Label" CssClass="style2" ForeColor="#880066"
                Visible="False">原因：</asp:Label>
            <asp:DropDownList ID="DDL_Reason" runat="server" CssClass="style2" ForeColor="#880066"
                Visible="False">
                <asp:ListItem>未選擇</asp:ListItem>
                <asp:ListItem>有破箱</asp:ListItem>
                <asp:ListItem>無破箱</asp:ListItem>
                <asp:ListItem>海關抽驗</asp:ListItem>
                <asp:ListItem>虎門短裝</asp:ListItem>
                <asp:ListItem>裝錯箱</asp:ListItem>
            </asp:DropDownList>
            <br />
            <span class="style2">原產品：</span>
            <asp:Label ID="lbl_P" runat="server" Text="" CssClass="style2" ForeColor="#006666"></asp:Label>
            &nbsp;&nbsp; <span class="style2">原數量：</span>
            <asp:Label ID="lbl_Q" runat="server" Text="" CssClass="style2" ForeColor="#006666"></asp:Label>
            <br />
            <span class="style2" style="color: #008000">產品編號：</span>
            <asp:TextBox ID="txt_More_ProdoctID" runat="server" CssClass="style2" Width="130px"></asp:TextBox>
            <span class="style2" style="color: #008000">數量：</span>
            <asp:TextBox ID="txt_More_ProdoctNum" runat="server" CssClass="style2" Width="50px"></asp:TextBox>
            &nbsp;&nbsp;
            <asp:Button ID="btn_temp" runat="server" Text="" CssClass="style2" />
            &nbsp;&nbsp; <span class="style2">
                <asp:Button ID="btn_Lack" runat="server" Text="缺少" CssClass="style2" OnClick="btn_Lack_Click" />
                &nbsp;/&nbsp;
                <asp:Button ID="btn_More" runat="server" Text="多出" CssClass="style2" OnClick="btn_More_Click" />
            </span>
        </asp:Panel>
        <hr />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Label ID="lbl_Message" runat="server" Text="" CssClass="style3"></asp:Label>
                <br />
                <span class="style2">產品：</span>
                <br />
                <asp:ListBox ID="LB_Product_Id1" runat="server" Width="200px" CssClass="style2" ForeColor="#0066CC"
                    DataTextField="ProductId" DataValueField="Barcode" Height="200px"></asp:ListBox>
                <asp:ListBox ID="LB_Product_Id2" runat="server" Width="200px" CssClass="style2" ForeColor="#006600"
                    DataTextField="ProductId" DataValueField="Barcode" Height="200px" Visible="False">
                </asp:ListBox>
                <br />
                <br />
                <asp:Button ID="btn_Send" runat="server" Text="確定回報" CssClass="style2" OnClientClick="return confirm('確定回報?')"
                    OnClick="btn_Send_Click" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
