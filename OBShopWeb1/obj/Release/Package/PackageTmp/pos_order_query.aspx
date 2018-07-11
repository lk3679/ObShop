<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pos_order_query.aspx.cs" Inherits="OBShopWeb.pos_order_query" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>訂單查詢</title>
    <script src="js/jquery-1.9.1.js"></script>
         <link href="css/jquery-ui.css" rel="stylesheet" />
    <link href="css/jquery-ui.theme.min.css" rel="stylesheet" />
    <script src="js/jquery-ui.min.js"></script>
      <%--<link href="layout.css" rel="stylesheet" />--%>
     <link href="css/layout.css" rel="stylesheet" />
    <style>
        .EU_DataTable {
            border-collapse: collapse;
            width: 100%;
        }

            .EU_DataTable tr th {
                background: #ccc;
                color: #454545;
                border: 1px solid #cccccc;
                /*padding: 4px 8px 4px 8px;*/
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
        $(function () {
            var Height = $(window).height() * 2/3;
            var Width = $(window).width() * 9/10;

            $("#OrderItem").dialog({
                open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                width: Width,
                height: Height,
                autoOpen: false,
                modal: true,
                buttons: {
                    "關閉": function () {
                        $(this).dialog("close");

                    }
                }
            });


            $(".OrderID").click(function () {
                var OrderID = $(this).html();
                var url = "?act=GetOrderItem&OrderID=" + OrderID;
                $.get(url, function (data) {
                    //console.log(data)
                    var json = JSON.parse(data);
                    if (json.result) {
                        $("#OrderItem").html(json.data);
                        $("#OrderItem").dialog('open');
                    }
                    else {
                        $("#OrderItem").html("查不到訂單明細");
                        $("#OrderItem").dialog('open');
                    }
                })
            })

            $(".CancelOrders").click(function () {
                var invoice = $(this).attr("data-invoice");
                var r = confirm("請確認發票\n" + invoice + "  \n未印出\n才可取消訂單");
                if (r == true) {
                    var OrderID = $(this).attr("data-orderid");
                    var url = "?act=CancelOrders&OrderID=" + OrderID;
                    $.get(url, function (data) {
                        var json = JSON.parse(data);
                        if (json.result) {
                            alert("作廢成功!!");
                            location.reload();
                        }
                        else {
                            
                            if (json.ErrorMsg == "尚未登入") {
                                alert("目前尚未登入\n請重新登入，再取消訂單");
                                window.location.href = "logout.aspx";
                            }

                            if (json.ErrorMsg == "新增失敗") {
                                alert("作廢失敗!!");
                            }
 
                        }
                    })
                } 
                
            })

        })
    </script>
</head>
<body>
    <h2 class="style1">交易查詢</h2>
            <form method="post">
                開始日期：<select name="startYear" id="startYear">
                    <%for (int i = FirstYear; i <= int.Parse(currentTime.ToString("yyyy")); i++)
                      { %>
                    <option label="<%=i %>" value="<%=i %>" <%if (startYear == i.ToString())
                                                              { %> selected="selected" <%} %>><%=i %></option>
                    <% } %>
                </select>
                <select name="startMonth" id="startMonth">
                    <%for (int i = 1; i <= 12; i++)
                      { %>
                    <%string MonthItem = i.ToString("D2"); %>
                    <option label="<%=MonthItem %>" value="<%=MonthItem %>" <%if (startMonth == MonthItem)
                                                                              { %> selected="selected" <%} %>><%=MonthItem %></option>
                    <% } %>
                </select>

                <select name="startDay" id="startDay">
                    <%for (int i = 1; i <= 31; i++)
                      { %>
                    <%string DayItem = i.ToString("D2"); %>
                    <option label="<%=DayItem %>" value="<%=DayItem %>" <%if (startDay == DayItem)
                                                                          { %> selected="selected" <%} %>><%=DayItem %></option>
                    <% } %>
                </select><br>
                結束日期：<select name="endYear" id="endYear">
                    <%for (int i = FirstYear; i <= int.Parse(currentTime.ToString("yyyy")); i++)
                      { %>
                    <option label="<%=i %>" value="<%=i %>" <%if (endYear == i.ToString())
                                                              { %> selected="selected" <%} %>><%=i %></option>
                    <% } %>
                </select>
                <select name="endMonth" id="endMonth">
                    <%for (int i = 1; i <= 12; i++)
                      { %>
                    <%string MonthItem = i.ToString("D2"); %>
                    <option label="<%=MonthItem %>" value="<%=MonthItem %>" <%if (endMonth == MonthItem)
                                                                              { %> selected="selected" <%} %>><%=MonthItem %></option>
                    <% } %>
                </select>
                <select name="endDay" id="endDay">
                    <%for (int i = 1; i <=31; i++)
                      { %>
                    <%string DayItem = i.ToString("D2"); %>
                    <option label="<%=DayItem %>" value="<%=DayItem %>" <%if (endDay == DayItem)
                                                                          { %> selected="selected" <%} %>><%=DayItem %></option>
                    <% } %>
                </select><br/>

                <hr/>
                <input type="submit" name="QueryType" value="訂單資料查詢" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input type="submit" name="QueryType" value="作廢訂單查詢" />
            </form>
            <hr/>

    <div id="container" class="EU_DataTable">
        <div id="content">
            <table class="style4gv">
                <tbody>
                    <tr>
                         <th></th>
                        <th>訂單編號</th>
                        <th>訂單狀態</th>
                        <th>金額</th>
                        <th>付款方式</th>
                        <th>POS機編號</th>
                        <th>結帳人</th>
                        <th>交易時間</th>
                        <th>發票號碼</th>
                        <th>取消訂單</th>
                    </tr>
                    <%
                        int RowNo = 0;
                         %>
                    <%foreach (System.Data.DataRow dr in OrderDT.Rows)
                      { %>
                    <tr>
                        <%string OrderStatus = "";
                          string CancelButton = "";
                          RowNo++;
                          int status = int.Parse(dr["Status"].ToString());

                          switch (status)
                          {
                              case 2:
                                  OrderStatus = "作廢";
                                  break;
                              case 3:
                                  OrderStatus = "待結";
                                  break;
                              case 4:
                                  OrderStatus = "系統自動取消";
                                  break;
                              case 5:
                                  OrderStatus = "手動取消";
                                  break;
                              case 6:
                                  OrderStatus = "折讓";
                                  break;
                              default:
                                  OrderStatus = "";
                                  CancelButton = "<a href=\"javascript:void(0)\" class=\"CancelOrders\" data-orderid=\"" + dr["OrderID"].ToString() + "  \"    data-invoice=\"" + GetInvoiceList(dr["InvoiceList"].ToString()).Replace("<br/>", "\n") + "\">取消訂單</a>";
                                  break;
                          }
                         
                        %>
                        <td><%= RowNo %></td>
                        <td><a href="javascript:void(0)" class="OrderID"><%=(dr["OrderID"].ToString() == "0") ? "" : dr["OrderID"].ToString() %></a></td>
                        <td><%if (OrderStatus.Length > 0)
                              { %>
                                 <%=OrderStatus %><br/>
                           <%} %>
                             <a href="pos_vip.aspx?vip_id=<%=dr["VipNo"].ToString().Trim()%>" ><%=dr["VipNo"].ToString() %></a>
                           </td>
                        <td style=" text-align: right"><%=dr["Amount"].ToString() %></td>
                        <td><%=int.Parse(dr["PayType"].ToString())==1?"<span style='color:green'>現金</span>":"<span style='color:blue'>刷卡</span>" %></td>
                        <td><%=dr["PosNo"].ToString() %></td>
                        <td><%=dr["Name"].ToString() %></td>
                        <td><%=DateTime.Parse(dr["OrderTime"].ToString()).ToString("yyyy/MM/dd HH:mm:ss") %></td>
                        <td><%= GetInvoiceList(dr["InvoiceList"].ToString()) %></td>
                        <td><%  if (Session["ClerkID"] != null)
                            { %><%=CancelButton %> <% } %></td>
                    </tr>
                    <% } %>


                    <tr>
                        <th></th>
                        <th></th>
                        <th>現金銷售</th>
                        <th style=" text-align: right"><%=TotalCash %></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>      
                        <th></th>                                        
                    </tr>
                      <tr>
                          <th></th>
                        <th></th>
                        <th>信用卡銷售</th>
                        <th style=" text-align: right"><%=TotalCredit %></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>  
                          <th></th>                                            
                    </tr>
                    <tr>
                        <th></th>
                        <th></th>
                        <th>銷售金額總計</th>
                        <th style=" text-align: right"><%=TotalAmount %></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>                                              
                    </tr>
                   <tr>
                       <th></th>
                        <th></th>
                        <th>銷售總件數</th>
                        <th style=" text-align: right"><%=SaleNum %></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th> 
                       <th></th>                                             
                    </tr>
                    <tr>
                        <th></th>
                        <th></th>
                        <th>現金作廢</th>
                        <th style=" text-align: right"><%=FailedTotalCash %></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th> 
                        <th></th>                                             
                    </tr>
                    <tr>
                        <th></th>
                        <th></th>
                        <th>信用卡作廢</th>
                        <th style="text-align: right"><%=FailedTotalCredit %></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                    </tr>
                    <tr>
                        <th></th>
                        <th></th>
                        <th>作廢金額總計</th>
                        <th style="text-align: right"><%=FailedTotalAmount %></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                    </tr>
                    <tr>
                        <th></th>
                        <th></th>
                        <th>作廢總件數</th>
                        <th style=" text-align: right"><%=FailedNum %></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>  
                        <th></th>                                            
                    </tr>

                </tbody>
            </table>
        </div>
    </div>

    <div id="OrderItem" style="display: none">
    </div>
</body>
</html>
