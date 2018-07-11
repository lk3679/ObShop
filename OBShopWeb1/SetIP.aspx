<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetIP.aspx.cs" Inherits="OBShopWeb.SetIP" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>修改Cookie IP</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <span class="style1">修改Cookie IP</span>
    <br />
    <br />
    <span class="style2">原始IP：</span><asp:Label ID="lbl_CookieIP" runat="server" 
            CssClass="style2" ForeColor="#0066CC"></asp:Label><br />
    <span class="style2">修改為：</span><asp:TextBox ID="txt_SetIP" runat="server" 
            CssClass="style2" Width="130px"></asp:TextBox>
    &nbsp;
    <asp:Button ID="btn_SetIP" runat="server" Text="修改" CssClass="style2" onclick="btn_SetIP_Click"/>
    &nbsp;
    <asp:Label ID="lbl_Message" runat="server" CssClass="style3" ></asp:Label>
    &nbsp;
    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" 
            runat="server" ErrorMessage="格式不符" CssClass="style3" 
            ControlToValidate="txt_SetIP" 
            ValidationExpression="^((?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))*$"></asp:RegularExpressionValidator>
    </div>
    </form>
</body>
</html>
