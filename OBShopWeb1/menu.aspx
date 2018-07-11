<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="menu.aspx.cs" Inherits="OBShopWeb.menu" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Expires" content="0"/> 
<meta http-equiv="Cache-Control" content="no-cache"/> 
<meta http-equiv="Pragma" content="no-cache"/> 
    <title>選單</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <style type="text/css">
        body 
        {
            margin-top: 10px;
            margin-left: 10px;
        }        
    </style>   
     
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/lockkey.js"></script>
    <script type="text/javascript">
        $("document").ready(function () {
            $(".style8menu a").click(function () {
                $(".style8menu a").css("background", "");
                $(this).css("background", "#FFFF99");
            });
        });
    </script>
</head>
<body style="background-color:#FFFFFF">
    <form id="form1" runat="server" >
    <div class="style6menu" >
        <div id="Logo" class="style6menu" 
            style="padding: 10px; border: 1px dashed Gray; left: 30px; background-color: #FFFFFF;">
            <img alt="OrangeBear" class="style7pic" src="Image/obshop_small.jpg" /><br />
            <asp:Label ID="lbl_Account" runat="server" CssClass="style3"></asp:Label>
            &nbsp;<asp:HyperLink ID="HL_logout" runat="server" NavigateUrl="~/logout.aspx" Target="_parent" 
            style="color: #006699" CssClass="style3">登出</asp:HyperLink>
            <br />
            <asp:Label ID="lbl_OLCount" runat="server" CssClass="style3" Visible="False"></asp:Label>
            <asp:Label ID="lbl_Zone" runat="server" EnableTheming="True" CssClass="style3" 
                Visible="False"></asp:Label>
        </div>
        <div class="style8menu" id="Menu" style="padding: 5px; border: 1px dashed Gray; background-color: #FFFFFF;">
            <asp:HyperLink ID="HL_SystemInfo" runat="server" CssClass="style2" 
                ForeColor="#006666" NavigateUrl="rmenu_default.aspx" Target="rmenu">Info</asp:HyperLink>
            &nbsp;
            <span ><strong style="text-align: center;font-size: large;">[主選單]</strong></span>
            <br />
            <asp:HyperLink ID="HL_ShipOut_Menu" runat="server" target="rmenu" 
            NavigateUrl="~/menu_shipout.aspx" CssClass="style8menu" ForeColor="#006699">出庫作業</asp:HyperLink>
            <br />
            <asp:HyperLink ID="HL_ShipIn_Menu" runat="server" target="rmenu" 
            NavigateUrl="~/menu_shipin.aspx" CssClass="style8menu" ForeColor="#006600">入庫作業</asp:HyperLink>
            <br />
            <asp:HyperLink ID="HL_Storage1" runat="server" target="rmenu" 
            NavigateUrl="~/menu_storage1.aspx" CssClass="style8menu" ForeColor="#990099">儲位作業</asp:HyperLink>
            <br />
            <asp:HyperLink ID="HL_Storage2" runat="server" target="rmenu" 
            NavigateUrl="~/menu_storage2.aspx" CssClass="style8menu" ForeColor="#EE0066">儲位管理</asp:HyperLink>
            <%--
            <br />
            <asp:HyperLink ID="HL_BusNO_Invoice_Menu" runat="server" target="rmenu" 
                NavigateUrl="~/menu_BusNO_Invoice.aspx" CssClass="style8menu">發票管理</asp:HyperLink>--%>
            <br />
            <asp:HyperLink ID="HL_menu_ship_manage1" runat="server" target="rmenu" 
                NavigateUrl="~/menu_ship_manage1.aspx" CssClass="style8menu" 
                ForeColor="#FF3300">物流管理</asp:HyperLink>
            <%--
            <br />
            <asp:HyperLink ID="HL_menu_ship_manage2" runat="server" target="rmenu" 
                NavigateUrl="~/menu_ship_manage2.aspx" CssClass="style8menu" 
                ForeColor="#AA5533">包裹管理</asp:HyperLink>--%>
            <br />
            <asp:HyperLink ID="HL_logistics_account_Menu" runat="server" target="rmenu" 
                NavigateUrl="~/menu_logistics.aspx" CssClass="style8menu" 
                ForeColor="#CC0000">人員管理</asp:HyperLink>
            <br />
            <asp:HyperLink ID="HL_system_Menu" runat="server" target="rmenu" 
                NavigateUrl="~/menu_system.aspx" CssClass="style8menu" 
                ForeColor="#333399">系統管理</asp:HyperLink>
            <br />
            <br />
            <asp:HyperLink ID="HL_pos_check_out2" runat="server" 
                NavigateUrl="~/pos_check_out.aspx" Target="_parent" CssClass="style8menu">收銀前台</asp:HyperLink>
            <br />
        </div>
    </div>
    </form>
</body>
</html>
