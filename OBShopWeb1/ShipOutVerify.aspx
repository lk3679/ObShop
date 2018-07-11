<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShipOutVerify.aspx.cs" Inherits="OBShopWeb.ShipOutVerify" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Expires" content="0"/> 
<meta http-equiv="Cache-Control" content="no-cache"/> 
<meta http-equiv="Pragma" content="no-cache"/> 

    <title>調出確認</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/json2.js"></script>
    <script type="text/javascript">
    <!--  
        function scan() {

            var Product_Id1 = document.getElementById("LB_Product_Id1");
            var Product_Id2 = document.getElementById("LB_Product_Id2");
            var pid = document.getElementById("lbl_pid");
            var message = document.getElementById("lbl_Message");
            var lbl_ticket_id = document.getElementById("lbl_ticket_id");

            var lbl_ticketDetailId = "";
            var packageId = document.getElementById("lbl_packageId");
            var VerifyCheck_NO = document.getElementById("txt_VerifyCheck_NO");
            var id = VerifyCheck_NO.value;
            VerifyCheck_NO.value = "";

            $(message).text("");

            if (id.match(/^S\d{5,6}$/)) {
                $(pid).text(id.toString());
                //pid.innerText = id.toString();
            }
            //輸入0 回上一步
            else if (id.match(/^0{12}$/) || id.match(/^0$/)) {
                if (Product_Id2.length > 0) {
                    var num = Product_Id2.length - 1;
                    Product_Id1.appendChild(Product_Id2[0]);
                }
            }
            //輸入1 送出確認
            else if (id.match(/^0{10}17$/) || id.match(/^1$/)) {
                if (Product_Id2.length == 0) {
                    $(message).text("未驗任何商品！");
                }
                else {
                    var num = Product_Id2.length;
                    for (var j = 0; j < num; j++) {
                        if (j > 0) {
                            lbl_ticketDetailId += ",";
                        }
                        lbl_ticketDetailId += $(Product_Id2[j]).attr("value2");
                    }
                    var url = "";
                    url += "?act=verify&ticket_id=" + $(lbl_ticket_id).text();
                    url += "&ticketDetailId=" + lbl_ticketDetailId + "&package_id=" + $(pid).text() + "&r=" + Math.random();
                    window.open(url, target = "_self");
                }
            }
            //輸入2 全部確認
            else if (id.match(/^0{10}24$/) || id.match(/^2$/)) {
                var num = Product_Id1.length;
                for (var j = 0; j < num; j++) {
                    Product_Id2.appendChild(Product_Id1[0]);
                }
                VerifyCheck_NO.value = "";
            }
            //8碼barcode 驗商品
            else if (id.match(/^\d{8}$/)) {
                var find = false;
                var num = Product_Id1.length;
                for (var j = 0; j < num; j++) {

                    if (Product_Id1[j].value == id) {
                        Product_Id2.insertBefore(Product_Id1[j], Product_Id2[0]);
                        find = true;
                        break;
                    }
                }
                if (find == true) {

                }
                else {
                    $(message).text("這個商品不是這張傳票的！");
                }
            }
            else {
                $(message).text("這個商品不是這張傳票的！");
            }
        }
    //-->
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
    
        <span class="style1"><strong>調出確認</strong></span><br class="style2" />
        <br />
        <asp:Label ID="lbl_VerifyCheck_NO" runat="server" Text="產品條碼：" 
            CssClass="style2"></asp:Label>
        
        <asp:TextBox ID="txt_VerifyCheck_NO" runat="server" CssClass="style2" 
            ontextchanged="txt_VerifyCheck_NO_TextChanged" ></asp:TextBox>
    
        <span class="style2">&nbsp;(0:取消最後一筆 1:確認 2:全部確認)</span><br class="style2" />
    <hr />
        <span class="style2">傳票ID：&nbsp;
        <asp:Label ID="lbl_ticket_id" runat="server" style="color: #006699"></asp:Label>
        </span>&nbsp;&nbsp;
        <asp:LinkButton ID="LB_GoShippingMoreLack" runat="server" CssClass="style2" 
                ForeColor="#FF6600" onclick="LB_GoShippingMoreLack_Click" >數量異常回報</asp:LinkButton> 
        <br />
    <hr />
        <asp:Label ID="lbl_Message" runat="server" CssClass="style3"></asp:Label>
        <br />
        <hr />
<%--        <asp:ListBox ID="LB_Product_Id1" runat="server" Width="200px" CssClass="style2" DataTextField="ProductId"
            DataValueField="Barcode" ForeColor="#0066CC" Height="400px"></asp:ListBox>
        <asp:ListBox ID="LB_Product_Id2" runat="server" Width="200px" CssClass="style2" DataTextField="ProductId"
            DataValueField="Barcode" ForeColor="#006600" Height="400px"></asp:ListBox>--%>
        <asp:ListBox ID="LB_Product_Id1" runat="server" Width="200px" CssClass="style2" ForeColor="#0066CC" Height="400px"></asp:ListBox>
        <asp:ListBox ID="LB_Product_Id2" runat="server" Width="200px" CssClass="style2" ForeColor="#006600" Height="400px"></asp:ListBox>
        <br />
    </div>
    <asp:Panel ID="Panel2" runat="server" Visible="False">
    <asp:Label ID="Label2" runat="server"></asp:Label>
        &nbsp;<asp:Label ID="Label3" runat="server"></asp:Label>
        &nbsp;<asp:Label ID="Label4" runat="server"></asp:Label>
        &nbsp;<asp:Label ID="Label5" runat="server"></asp:Label>
    </asp:Panel> 
    </form>
</body>
</html>
