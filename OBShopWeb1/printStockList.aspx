<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="printStockList.aspx.cs" Inherits="OBShopWeb.printStockList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 242px;
            text-align: center;
        }
        .auto-style2 {
            font-size: xx-large;
        }
        ul
{
    list-style-type: none;
}
        span{
            text-align:center;width:100px;display: inline-block;display:-moz-inline-box;
        }

    </style>
    </head>
<body style="margin-left:0px">
    <%--<ul style="width:52mm;margin-left:0px">
        <li style="text-align:center;font-size:12pt">交易明細</li>
        <li><br/></li>
       <li style="text-align:center;font-size:8pt">檢貨單<br/></li>
         <%foreach(OBShopWeb.Poslib.CheckOutProduct Co in CoList){ %>
            <li style="font-size:8pt"><%=Co.barcode %>&nbsp;&nbsp;<%=Co.product_id %>&nbsp;X&nbsp;<%=Co.quantity%></li>
            <% } %> 
         <li><br/></li>
        <li style="font-size:6pt">交易時間： <%=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") %></li>
         <li style="font-size:6pt">交易序號： </li>
    </ul>--%>
    <img src="<%=ImgUrl %>" /><form id="form1" runat="server">
        
&nbsp;<table style='width: 52mm;'>
        <tr style="height:8mm">
            <td colspan='3' style='text-align: center; font-size: 14px;height'>交易明細
            </td>
        </tr>
        <tr style="height:8mm">
            <td colspan='3' style='text-align: center; font-size: 8px'>檢貨單</td>
        </tr>
        <%foreach (OBShopWeb.Poslib.CheckOutProduct Co in CoList)
          { %>
        <tr style="height:4mm">
            <td style='font-size: 8px; text-align: center;'><%=Co.barcode %></td>
            <td style='font-size: 8px; text-align: center;'><%=Co.product_id %></td>
            <td style='font-size: 8px; text-align: center;'>X <%=Co.quantity%></td>
        </tr>
        <% } %>
        <tr style="height:4mm">
            <td colspan='3' style='text-align: center; font-size: 8px'><br/></td>
        </tr>
        <tr style="height:4mm;text-align: center;">
            <td style='font-size: 8px;'>交易時間：</td><td colspan='2' style='font-size: 8px;text-align: left; '> <%=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") %></td>
        </tr>
        <tr style="height:4mm;text-align: center;">
            <td style='font-size: 8px;'>交易序號：</td><td colspan='2' style='font-size: 8px;' ></td>
        </tr>

    </table>


        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="產生PDF" />
       
        <asp:DropDownList ID="lbPrinter" runat="server">
        </asp:DropDownList>
    </form>
</body>
</html>
