<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="menu_BusNO_Invoice.aspx.cs" Inherits="OBShopWeb.menu_BusNO_Invoice" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Expires" content="0"/> 
<meta http-equiv="Cache-Control" content="no-cache"/> 
<meta http-equiv="Pragma" content="no-cache"/> 
    <title></title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <style type="text/css">
        body 
        {
            margin-top: 10px;
            margin-left: 10px;
            margin-right: 0px;
            margin-bottom: 0px;            
        }        
        .style8menu
        {
            color: #FFFFFF;
        }
    </style>
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/lockkey.js"></script>
    <script type="text/javascript" src="js/menu.js"></script>
    <script type="text/javascript">
        var url = "Choice.aspx";
        window.open(url, target = "content");

        c1 = "#666666";
        c2 = "#999922";
    </script>
</head>
<body style="background-color:#666666">
    <form id="form1" runat="server">
    <div style="padding: 2px; margin: 2px; background-color: #000000;">
    
        <span class="style5menuHead">
        <strong>[發票管理] </strong>
        </span>
     
        <asp:HyperLink ID="HL_BusNo" runat="server" NavigateUrl="~/BusNo.aspx" 
            Target= "content" CssClass="style5menu">統一編號設定</asp:HyperLink>
&nbsp;<asp:HyperLink ID="HL_invoice" runat="server" NavigateUrl="~/Invoice.aspx" 
            Target= "content" CssClass="style5menu">發票機設定</asp:HyperLink>
&nbsp;<asp:HyperLink ID="HL_BusNo_mapping" runat="server" NavigateUrl="~/BusNo_mapping.aspx" 
            Target= "content" CssClass="style5menu">發票機對應</asp:HyperLink>
&nbsp;<asp:HyperLink ID="HL_print_invoice" runat="server" NavigateUrl="~/PrintInvoice.aspx" 
            Target= "content" CssClass="style5menu">發票補印</asp:HyperLink>
&nbsp;<asp:HyperLink ID="HL_no_invoice" runat="server" NavigateUrl="~/NoInvoice.aspx" 
            Target= "content" CssClass="style5menu" Visible="False">發票回填</asp:HyperLink>
    
        <asp:Menu ID="Menu1" runat="server" 
            ForeColor="White" Orientation="Horizontal" StaticSubMenuIndent="16px" 
            CssClass="style8menu" Visible="False">
            <Items>
                <asp:MenuItem Text="統一編號設定" Value="統一編號設定" NavigateUrl="~/BusNo.aspx" Target= "content"></asp:MenuItem>
                <asp:MenuItem Text="發票機設定" Value="發票機設定" NavigateUrl="~/Invoice.aspx" Target= "content"></asp:MenuItem>
                <asp:MenuItem Text="發票機對應" Value="發票機對應" NavigateUrl="~/BusNo_mapping.aspx" Target= "content"></asp:MenuItem>
                <asp:MenuItem Text="發票補印" Value="發票補印" NavigateUrl="~/PrintInvoice.aspx" Target= "content"></asp:MenuItem>
                <asp:MenuItem Text="發票回填" Value="發票回填" NavigateUrl="~/NoInvoice.aspx" Target= "content"></asp:MenuItem>
            </Items>
        </asp:Menu>
    
    </div>
    </form>
</body>
</html>
