<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="OBShopWeb.index" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Cache-Control" content="no-cache" />
    <meta http-equiv="Pragma" content="no-cache" />
    <title>OB嚴選門市系統</title>
    <link rel="Shortcut Icon" href="favicon.ico" />
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/lockkey.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //            $(frames['menu']).click(function () {
            //                var menuWidth = this.frameElement.parentNode.attributes['cols'].value;
            //                alert(menuWidth);
            //                if (menuWidth == '15,*')
            //                    menuWidth = '180,*';
            //                else
            //                    menuWidth = '15,*';
            //                this.frameElement.parentNode.attributes['cols'].value = menuWidth;
            //            });
        });
    </script>
</head>
<frameset cols="180,*" frameborder="0">

    <form id="form1" runat="server"> 
　<frame src="menu.aspx" name="menu" frameborder="0" noresize="1" scrolling="no">

　<frameset rows="70,*"><%--55--%>

　　<frame src="rmenu_default.aspx" name="rmenu" scrolling="no" frameborder="0" noresize="0">

　　<frame src="SystemInfo.aspx" name="content" frameborder="0" noresize="0">
    </form>
    </frameset>
</frameset>
</html>