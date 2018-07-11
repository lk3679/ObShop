<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShipInVerify.aspx.cs" Inherits="OBShopWeb.ShipInVerify" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Cache-Control" content="no-cache" />
    <meta http-equiv="Pragma" content="no-cache" />
    <title>入庫確認</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/json2.js"></script>
    <script type="text/javascript">
    <!--
        function goMoreLack(type) {
            var pagename = (type == 1) ? "" : "Special";
            var Product_Id1 = document.getElementById("LB_Product_Id1");
            var selected = false;
            //選擇產品
            for (var i = 0; i < Product_Id1.length; i++) {
                if (Product_Id1.options[i].selected) {
                    selected = true;
                    var productid = $(Product_Id1[i]).text().split('_')[0];
                    var ticketid = $(Product_Id1[i]).val().split(',')[1];
                    var flowType = $(Product_Id1[i]).val().split(',')[2];
                    var quantity = $(Product_Id1[i]).val().split(',')[3];

                    if (ticketid != null && ticketid != "") {
                        var url = "";
                        url += "ShippingMoreLack" + pagename + ".aspx?ticket_id=" + ticketid + "&ProductId=" + productid + "&Q=" + quantity + "&flowType=" + flowType + "&r=" + Math.random();

                        window.open(url, target = "_blank");
                    }
                    else {
                        alert("無傳票號碼");
                    }
                    break;
                }
            }
            //選擇產品2(2014-0407新增)
            if (!selected) {
                var Product_Id2 = document.getElementById("LB_Product_Id2");
                for (var i = 0; i < Product_Id2.length; i++) {
                    if (Product_Id2.options[i].selected) {
                        selected = true;
                        var productid = $(Product_Id2[i]).text().split('_')[0];
                        var ticketid = $(Product_Id2[i]).val().split(',')[1];
                        var flowType = $(Product_Id2[i]).val().split(',')[2];
                        var quantity = $(Product_Id2[i]).val().split(',')[3];

                        if (ticketid != null && ticketid != "") {
                            var url = "";
                            url += "ShippingMoreLack" + pagename + ".aspx?ticket_id=" + ticketid + "&ProductId=" + productid + "&Q=" + quantity + "&flowType=" + flowType + "&r=" + Math.random();

                            window.open(url, target = "_blank");
                        }
                        else {
                            alert("無傳票號碼");
                        }
                        break;
                    }
                }
            }
            if (!selected)
                alert("未選擇回報產品");

            return false;
        }
    //-->
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div>
        <span class="style1"><strong>入庫確認</strong></span>
        &nbsp;&nbsp; 
        <br class="style2" />
        <br />
        <asp:Label ID="lbl_VerifyCheck_NO" runat="server" Text="產品條碼：" CssClass="style2"></asp:Label>
        <asp:TextBox ID="txt_VerifyCheck_NO" runat="server" CssClass="style2" AutoPostBack="true"
            OnTextChanged="txt_VerifyCheck_NO_TextChanged"></asp:TextBox>
        &nbsp; 
        <%--&nbsp;&nbsp;
        <span class="style2" style="color: #660066">※右欄請驗300件以內，以免系統執行逾時</span>--%>

        <span class="style2"></span><br class="style2" />
        <%--(0:取消最後一筆 1:確認 2:全部確認)--%><hr />
        <span class="style2">箱號：</span> &nbsp;
            <asp:Label ID="lblbox" runat="server" Style="color: #006699" CssClass="style2" ></asp:Label>
         &nbsp;&nbsp;
        <br />
         <span class="style2">傳票：</span> &nbsp;
            <asp:Label ID="lblTicketId" runat="server" Style="color: #006699" CssClass="style2" ></asp:Label>
         &nbsp;&nbsp;
        <br />
        <asp:LinkButton ID="LB_GoShippingMoreLack" runat="server" CssClass="style2" ForeColor="#FF6600"
            OnClientClick="return goMoreLack(1);">數量異常回報</asp:LinkButton>
        &nbsp;&nbsp;
       <%-- <asp:LinkButton ID="LB_GoShippingMoreLackSpecial" runat="server" CssClass="style2" ForeColor="#0066FF"
            OnClientClick="return goMoreLack(2);">【特殊】數量異常回報</asp:LinkButton>
        &nbsp;&nbsp;
        <asp:HyperLink ID="HL_SetWeight" runat="server" CssClass="style2" ForeColor="#9900CC"
             NavigateUrl="SetWeight.aspx" Target="_blank">重量異常秤重</asp:HyperLink>--%>
        <br />
        <hr />
        <asp:Label ID="lbl_Message" runat="server" CssClass="style3"></asp:Label>
        <br />
        <%--<span class="style3">上架箱號：
			<asp:Label ID="lbl_pid" runat="server" Style="color: #006699"></asp:Label>
		</span>
		<br />
		<hr />--%>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
            <table>
            <tr>
            <td>
            <asp:ListBox ID="LB_Product_Id1" runat="server" Width="250px" CssClass="style2" ForeColor="#0066CC"
                    DataTextField="ProductId" DataValueField="Barcode" Height="400px"></asp:ListBox>
            </td>
            <td>
            <asp:ListBox ID="LB_Product_Id2" runat="server" Width="250px" CssClass="style2" ForeColor="#006600"
                    DataTextField="ProductId" DataValueField="Barcode" Height="400px"></asp:ListBox>
            </td>
            </tr>
            <tr>
            <td>
            <span class="style2">品項數：</span>
            <asp:Label ID="lbl_Product_Id1num" runat="server" CssClass="style2" ForeColor="#006699"></asp:Label>
            </td>
            <td>
            <asp:Label ID="lbl_Product_Id2num" runat="server" CssClass="style2" ForeColor="#990099"></asp:Label>
            </td>
            </tr>
            </table>

            <br />
            </ContentTemplate>
        </asp:UpdatePanel>
       
    </div>
    <br /> 
    </form>
</body>
</html>
