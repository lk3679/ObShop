<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="menu_storage2.aspx.cs" Inherits="OBShopWeb.menu_storage2" %>

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
        var url = "PDA/CreateStorageNew.aspx";
        window.open(url, target = "content");

        c1 = "#996699";
        c2 = "#999922";
    </script> 
</head>
<body style="background-color: #AA6699">
    <form id="form1" runat="server">
    <div style="padding: 2px; margin: 2px; background-color: #773366;">
        <span class="style5menuHead"><strong>[儲位管理]</strong></span>
        &nbsp;<asp:HyperLink ID="HL_CreateStorage" runat="server" NavigateUrl="~/PDA/CreateStorageNew.aspx"
            Target="content" CssClass="style5menu">建儲位</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_PrintStorage" runat="server" NavigateUrl="~/PDA/PrintStorageNew.aspx"
            Target="content" CssClass="style5menu">印儲位</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_StorageSearch" runat="server" NavigateUrl="~/PDA/StorageSearchEmpty.aspx"
            Target="content" CssClass="style5menu">空儲位</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_StorageInfoRange" runat="server" NavigateUrl="~/PDA/StorageInfoRangeNew.aspx"
            Target="content" CssClass="style5menu">儲位盤點報表</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_StorageReport" runat="server" NavigateUrl="~/PDA/StorageReport.aspx"
            Target="content" CssClass="style5menu">報表</asp:HyperLink>
        <%--&nbsp;<asp:HyperLink ID="HL_DiffHandle" runat="server" NavigateUrl="~/DiffHandle.aspx"
            Target="content" CssClass="style5menu">差異處理</asp:HyperLink>--%>
        &nbsp;<asp:HyperLink ID="HL_StorageInfoLog" runat="server" NavigateUrl="~/PDA/StorageInfoLog.aspx"
            Target="content" CssClass="style5menu">儲位記錄</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_StorageInOutLog" runat="server" NavigateUrl="~/PDA/StorageInOutLog.aspx"
            Target="content" CssClass="style5menu">盤點打銷紀錄</asp:HyperLink>
        <%--&nbsp;<asp:HyperLink ID="HL_SupplementList" runat="server" NavigateUrl="~/SupplementList.aspx"
            Target="content" CssClass="style5menu">補貨</asp:HyperLink>
        &nbsp;<asp:HyperLink ID="HL_MergeShelfList" runat="server" NavigateUrl="~/MergeShelfList.aspx"
            Target="content" CssClass="style5menu">合併清單</asp:HyperLink>--%>

    </div>
    </form>
</body>
</html>
