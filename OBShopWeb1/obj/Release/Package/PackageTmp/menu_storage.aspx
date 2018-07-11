<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="menu_storage.aspx.cs" Inherits="OBShopWeb.menu_storage" %>

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
        var url = "Choice.aspx";
        window.open(url, target = "content");

        c1 = "#AA6699";
        c2 = "#999922";
    </script>
</head>
<body style="background-color: #AA6699">
    <form id="form1" runat="server">
    <div style="padding: 2px; margin: 2px; background-color: #773366;">
        <span class="style5menuHead"><strong>[儲位作業]</strong></span>
        <asp:HyperLink ID="HL_Inventory" runat="server" NavigateUrl="~/PDA/InventoryNum.aspx"
            Target="content" CssClass="style5menu">盤點</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_InventoryOne" runat="server" NavigateUrl="~/PDA/InventoryNumOne.aspx"
            Target="content" CssClass="style5menu">盤單品</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_Merge" runat="server" NavigateUrl="~/PDA/Merge.aspx"
            Target="content" CssClass="style5menu">批量合併</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_MergeOneQ" runat="server" NavigateUrl="~/PDA/MergeOneQ.aspx"
            Target="content" CssClass="style5menu">問題上架</asp:HyperLink>
        <%--&nbsp;<asp:HyperLink ID="HL_MergeOne2" runat="server" NavigateUrl="~/PDA/MergeOne2.aspx"
            Target= "content" CssClass="style5menu" Visible="False">逐件合併</asp:HyperLink>--%>
        &nbsp;<asp:HyperLink ID="HL_MergePart" runat="server" NavigateUrl="~/PDA/MergePart.aspx"
            Target="content" CssClass="style5menu">部份合併</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_MergePart2" runat="server" NavigateUrl="~/PDA/MergePart2.aspx"
            Target="content" CssClass="style5menu">移動產品</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_StorageSearch" runat="server" NavigateUrl="~/PDA/StorageSearch.aspx"
            Target="content" CssClass="style5menu">空儲位</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_StorageInfo" runat="server" NavigateUrl="~/PDA/StorageInfo.aspx"
            Target="content" CssClass="style5menu">儲位內容</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_SearchProduct" runat="server" NavigateUrl="~/PDA/SearchProduct.aspx"
            Target="content" CssClass="style5menu">產品儲位</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_SearchBarcode" runat="server" NavigateUrl="~/PDA/SearchBarcode.aspx"
            Target="content" CssClass="style5menu">查條碼</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_CreateStorage" runat="server" NavigateUrl="~/PDA/CreateStorage.aspx"
            Target="content" CssClass="style5menu">建儲位</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_PrintStorage" runat="server" NavigateUrl="~/PDA/PrintStorage.aspx"
            Target="content" CssClass="style5menu">印儲位</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_StorageReport" runat="server" NavigateUrl="~/PDA/StorageReport.aspx"
            Target="content" CssClass="style5menu">報表</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_DiffHandle" runat="server" NavigateUrl="~/DiffHandle.aspx"
            Target="content" CssClass="style5menu">差異處理</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_SupplementList" runat="server" NavigateUrl="~/SupplementList.aspx"
            Target="content" CssClass="style5menu">補貨</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/PDA/MergeSplitFloor.aspx"
            Target="content" CssClass="style5menu">分樓</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/MergeShelfList.aspx"
            Target="content" CssClass="style5menu">合併清單</asp:HyperLink>
    </div>
    </form>
</body>
</html>