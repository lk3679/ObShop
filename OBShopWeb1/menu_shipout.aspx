<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="menu_shipout.aspx.cs" Inherits="OBShopWeb.menu_shipout" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Cache-Control" content="no-cache" />
    <meta http-equiv="Pragma" content="no-cache" />
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
    </style>
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/lockkey.js"></script>
    <script type="text/javascript" src="js/menu.js"></script>
    <script type="text/javascript">
        var url = "blank.aspx";
        window.open(url, target = "content");

        c1 = "#336699";
        c2 = "#999922";
    </script>
</head>
<body style="background-color: #336699">
    <form id="form1" runat="server">
    <div style="padding: 2px; margin: 2px; background-color: #003366;">
        <span class="style5menuHead"><strong>[出庫作業]</strong></span> 
        &nbsp;<asp:HyperLink ID="HL_OrderList" runat="server" NavigateUrl="~/OrderList.aspx"
            Target="content" CssClass="style5menu">出貨清單</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_ShipOutTicket" runat="server" NavigateUrl="~/ShipOutTicket.aspx" 
            Target="content" CssClass="style5menu">調出</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_NoProduct" runat="server" NavigateUrl="~/NoProduct.aspx"
            Target="content" CssClass="style5menu" >問題回報</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_pick" runat="server" NavigateUrl="~/PickCheck.aspx" Target="content"
            CssClass="style5menu">撿貨確認</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_pos_check_out2" runat="server" NavigateUrl="~/pos_check_out.aspx"
            Target="_parent" CssClass="style5menu">收銀前台</asp:HyperLink>
        
        <%--&nbsp;<asp:HyperLink ID="HL_pos_vip1" runat="server" NavigateUrl="~/pos_vip.aspx?act=add"
            Target="content" CssClass="style5menu">VIP卡開卡</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_pos_vip2" runat="server" NavigateUrl="~/pos_vip.aspx?act=query"
            Target="content" CssClass="style5menu">VIP卡查詢</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_pos_transaction" runat="server" NavigateUrl="~/pos_transaction.aspx"
            Target="content" CssClass="style5menu">交易查詢</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_pos_check_out" runat="server" NavigateUrl="~/pos_check_out.aspx"
            Target="content" CssClass="style5menu">結帳</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_pos_stock" runat="server" NavigateUrl="~/pos_stock.aspx"
            Target="content" CssClass="style5menu">庫存查詢</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_pos_barcode" runat="server" NavigateUrl="~/pos_barcode.pdf"
            Target="_blank" CssClass="style5menu">結帳條碼</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_pos_promotion" runat="server" NavigateUrl="~/pos_promotion.aspx"
            Target="content" CssClass="style5menu">活動設定</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_pos_vip_setup" runat="server" NavigateUrl="~/pos_vip_setup.aspx"
            Target="content" CssClass="style5menu">VIP設定</asp:HyperLink>--%>
    </div>
    </form>
</body>
</html>