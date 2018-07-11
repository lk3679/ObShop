<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pos_number_setting.aspx.cs" Inherits="OBShopWeb.pos_number_setting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        table {
            border-collapse: collapse;
        }

        table, th, td {
            border: 1px solid black;
        }
    </style>
     <script src="js/jquery-1.9.1.js"></script>
    <script type="text/javascript">
        $(function () {
            $(".DeletePOSNo").click(function () {
                var IP = $(this).attr("data-IP");
                var url = "?act=DeletePosNo&IP=" + IP;
                //console.log(IP);
                $.get(url, function (data) {
                    console.log(data);
                    var json = JSON.parse(data);
                    console.log(json);
                    if (json.result == true) {
                        alert("刪除成功!")
                        location.reload();
                    } else {
                        alert("刪除失敗")
                    }

                });
            })
        })
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>

        <table width="50%" border="0" cellspacing="0" >
  <tbody>
     <tr>
     <td style="width:10%;">編輯</td>
     <td style="width:10%;">刪除</td>
     <td style="width:20%;">電腦IP</td>
     <td style="width:20%;">POS編號</td>
     <td style="width:20%;">發票機號</td>
         <td style="width:20%;">印表機號</td>
    </tr>
        <%foreach(System.Data.DataRow row in PosNumberDT.Rows){ %>
       <tr>
              <td style="width: 10%;"></td>
              <td style="width: 10%;"><a href="#" data-IP="<%=row["ClientIP"]%>" class="DeletePOSNo">刪除</a></td>
              <td style="width: 20%;"><%=row["ClientIP"]%></td>
              <td style="width: 20%;"><%=row["PosNo"]%></td>
              <td style="width: 20%;"><%=row["InvoiceMachineNo"]%></td>
              <td style="width: 20%;"><%=row["PrintMachineNo"]%></td>
          </tr>
        <% } %>
        </tbody>
</table>
        <br />
        <br />
    </div>
        你的IP:
        <asp:Label ID="LabelyourIP" runat="server" Text=""></asp:Label>
        <br />
        <br />
        POS號碼：<asp:TextBox ID="TextBoxPOSNo" runat="server"></asp:TextBox>
        <br />
        <br />
        機器編號：<asp:TextBox ID="TextBoxMachineNo" runat="server"></asp:TextBox>
        <br />
        <br />
        印表機編號：<asp:TextBox ID="TextBoxppPrinter" runat="server"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" Text="新增資料" OnClick="Button1_Click" />
        <br />
        <br />
        <br />
        <asp:Label ID="LabelResult" runat="server" Text="" ForeColor="Red"></asp:Label>
    </form>
</body>
</html>
