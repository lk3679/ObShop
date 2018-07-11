<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Choice.aspx.cs" Inherits="OBShopWeb.Choice" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Expires" content="0"/> 
<meta http-equiv="Cache-Control" content="no-cache"/> 
<meta http-equiv="Pragma" content="no-cache"/> 
    <title>選擇出貨類別</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <style type="text/css">
        .style1
        {
            width: 187px;
            height: 145px;
        }
        #abc { position: relative; margin: 0 auto; width: 520px; text-align:left }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <br />
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        <br />
    <div id="abc">
        <asp:Panel ID="Panel1" runat="server" BorderColor="Gray" 
            BorderStyle="Dashed" BorderWidth="1px" HorizontalAlign="Center">
        </asp:Panel>
    </div>    
    </form>
</body>
</html>
