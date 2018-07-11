<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pos_stock.aspx.cs" Inherits="OBShopWeb.pos_stock" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>橘熊門市系統</title>
  <%--  <link href="layout.css" rel="stylesheet" />--%>
    <link href="css/layout.css" rel="stylesheet" />
    <link href="css/orangebear.css" rel="stylesheet" />
    <script type="text/javascript" src="js/jquery.js"></script>
    <style>
        a {
            text-decoration: none;
            color: #DC691F;
        }

        .EU_DataTable {
            border-collapse: collapse;
            width: 100%;
        }

            .EU_DataTable tr th {
                background: #5F2121;
                color: #FFFFFF;
                padding: 4px 8px 4px 8px;
                font-family: Arial, Helvetica, sans-serif;
                font-size: 15px;
                text-transform: capitalize;
                font-weight: bold;
            }

            .EU_DataTable tr:nth-child(2n+2) {
                background-color: #f3f4f5;
            }

            .EU_DataTable tr:nth-child(2n+1) td {
                background-color: #d6dadf;
                color: #454545;
            }

            .EU_DataTable tr td {
                padding: 3px 8px 3px 8px;
                color: #454545;
                font-family: Arial, Helvetica, sans-serif;
                font-size: 15px;
                border: 1px solid #cccccc;
                vertical-align: middle;
                text-align: center;
                font-weight: bold;
            }

                .EU_DataTable tr td:first-child {
                    text-align: center;
                }

            .EU_DataTable a {
                text-decoration: none;
                color: #C98526;
            }

                .EU_DataTable a:hover {
                    color: #e0802f;
                }

    </style>

    <script type="text/javascript">
<!--
    $(document).ready(function () {
        $("#input").focus();

        $("body").click(function () {
            $("#input").focus();
        })


    });

    function scan() {
        var o = $("#input");
        var id = o.val();
        o.val("");

        if (id.match(/^\d{8}$/)) {
            window.location = "?barcode=" + id;
        }else{
            window.location = "?product_id=" + id;
        }
        
    }
    //-->
    </script>
</head>
<body>
    <div id="container" class="style2">
        <div id="content">
            <h2 class="style1">橘熊門市查詢系統</h2>
            <form onsubmit="scan(); return false;">
                <input type="text" id="input"/>&nbsp;(輸入『產品編號』或是『產品條碼』)<br>
            </form>
            <hr>
            <%if (!string.IsNullOrEmpty(series_id))
              { %>

            <h2>系列編號: <%=sd.series_id %></h2>
            <h3>系列名稱: <%=sd.series_name %></h3>
            <%--<h3><a href="http://www.obdesign.com.tw/product.aspx?seriesID=<%=sd.series_id %>" onclick="window.open(this.href, '', 'width=1024,height=768,top=0, left=0'); return false;">查看更多商品圖片</a></h3>--%>
            <%if(sd.stockDT.Rows.Count>0){ %>
            <table class="EU_DataTable">
                <tr>
                    <th>產品編號</th>
                    <th>產品條碼</th>
                    <th>尺寸</th>
                    <th>顏色</th>
                    <th>價格</th>
                    <th>門市庫存</th>
                </tr>
                <%foreach (System.Data.DataRow row in  sd.stockDT.Rows)
                  { %>

                <tr <%if (product_id.Trim() == row["product_id"].ToString().Trim() || barcode.Trim() == row["barcode"].ToString().Trim())
                      {%> class="mark" <%} %> >
                    <td><%=row["product_id"]%></td>
                    <td><%=row["barcode"]%></td>
                    <td><%=row["size"]%></td>
                    <td><%=row["color"]%></td>
                    <td><%=row["price"]%></td>
                    <td><%=OBShopWeb.Poslib.Stock.GetB1Stock(row["product_id"].ToString()) %></td>
                </tr>
                <% }  %>
               
            </table>
            <%} %>
            <img src="http://image.obdesign.com.tw/<%=sd.img1 %>" /><br />
            <img src="http://image.obdesign.com.tw/<%=sd.img2 %>" /><br />
            <img src="http://image.obdesign.com.tw/<%=sd.img3 %>" /><br />
             <img src="http://image.obdesign.com.tw/<%=sd.img4 %>" /><br />
            <img src="http://image.obdesign.com.tw/<%=sd.img5 %>" /><br />
            <img src="http://image.obdesign.com.tw/<%=sd.img6 %>" /><br />
             <img src="http://image.obdesign.com.tw/<%=sd.img7 %>" /><br />
            <img src="http://image.obdesign.com.tw/<%=sd.img8 %>" /><br />

             <% }  %>
        </div>
    </div>
</body>
</html>
