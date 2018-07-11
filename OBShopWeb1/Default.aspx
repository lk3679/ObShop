<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OBShopWeb.Default" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Expires" content="0"/> 
<meta http-equiv="Cache-Control" content="no-cache"/> 
<meta http-equiv="Pragma" content="no-cache"/> 
    <title>OB嚴選門市系統</title>
    <link rel="Shortcut Icon" href="favicon.ico" />
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <style type="text/css">
        .style1
        {
            width: 180px;
            height: 145px;
        }
        #abc { position: relative; margin: 0 auto; width: 522px; 
text-align:left;
            top: 0px;
            left: 0px;
        }
        .Textbox {
            height: 50px;
            width: 180px;
            font-size: large;
        }
    </style>
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/json2.js"></script>
</head>
<body>
    <form id="form1" runat="server" defaultfocus="txt_ID">
    <br />
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <br />
    <div id="abc">
        <asp:Panel ID="Panel1" runat="server" BorderColor="Gray" 
            BorderStyle="Dashed" BorderWidth="1px" Width="600px" >
            &nbsp;
            <asp:Table ID="Tab_Login" runat="server" CellPadding="2" CellSpacing="2" HorizontalAlign="Center" style="font-family: 微軟正黑體" Width="550px">
                <asp:TableRow ID="TableRow1" BorderWidth="1" BorderColor="Gray" runat="server">
                    <asp:TableCell ID="TableCell1" runat="server">
                    <br />
                    <img alt="OrangeBear" class="style1" src="Image/obshop_small.jpg" />
                    <br />
                    <br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <span class="style3">線上人數：</span>
                    <asp:Label ID="lbl_Online" runat="server" Text="" CssClass="style3"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server" RowSpan="0">
                        <br />
                        <br />
                        <br />
                        <span style="height:50px; font-size:large;">帳號：</span><asp:TextBox ID="txt_ID" runat="server" CssClass="Textbox"></asp:TextBox>
                                <asp:TextBoxWatermarkExtender ID="txt_ID_TextBoxWatermarkExtender" 
                                    runat="server" Enabled="True" TargetControlID="txt_ID" WatermarkText="(帳號 / 物流編號)" WatermarkCssClass="style9watermark Textbox">
                                </asp:TextBoxWatermarkExtender>
                        <br />
                        <br />
                        <br />
                        <span style="height:50px; font-size:large;">密碼：</span><asp:TextBox ID="txt_PW" runat="server" style="margin-bottom: 0px" TextMode="Password" CssClass="Textbox"></asp:TextBox>
                        &nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btn_Login" runat="server" Text="登入" onclick="btn_Login_Click" style="font-family: 微軟正黑體; height:50px; font-size:large;" />
                        &nbsp;
                        <br />
                        <br />
                        <asp:CheckBox ID="ckbRememberMe" runat="server" Text=" 記住帳號" />
                        &nbsp;
                        <asp:Label ID="lbl_Message" runat="server" style="color: #FF0000"></asp:Label>
                        <br />
                        <br />
                        <br />
                        <span class="style2">IP位址：</span><asp:Label ID="lbl_CookieIP" runat="server" CssClass="style2" ForeColor="#0066CC"></asp:Label>
                        &nbsp;&nbsp;
                        <asp:HyperLink ID="HL_SetIP" runat="server" NavigateUrl="~/SetIP.aspx" Target="_blank" ForeColor="#006600">修改</asp:HyperLink>
                        <br />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </div>    
    </form>
</body>
</html>
