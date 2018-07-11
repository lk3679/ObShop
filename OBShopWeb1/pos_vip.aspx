<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pos_vip.aspx.cs" Inherits="OBShopWeb.pos_vip" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>橘熊門市系統</title>
    <link href="css/bootstrap.min.css" rel="stylesheet" />
       <link href="css/jquery-ui.css" rel="stylesheet" />
    <link href="css/jquery-ui.theme.min.css" rel="stylesheet" />
    <link href="layout.css" rel="stylesheet" />
    <link href="css/layout.css" rel="stylesheet" />
    <link href="css/orangebear.css" rel="stylesheet" />
    <script src="js/jquery-1.9.1.js"></script>
    <script src="js/jquery-ui.min.js"></script>
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
                text-align: center;
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
        var birthdayMonth = '<%=birthdayMonth %>';
        var birthdayDay = '<%=birthdayDay %>';
        var act = '<%=act %>';
        var clerk = JSON.parse(localStorage.getItem('clerk'));
        
        $(document).ready(function () {
            $("#input").focus();

            var Height = $(window).height() * 2 / 3;
            var Width = $(window).width() *4 / 5;

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
                var OrderID = $(this).attr("data-orderid");
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

            
            $("#Date_Month option").each(function () {
                if ($(this).val() == birthdayMonth) {
                    $(this).attr("selected", "selected");
                }
            });

            $("#Date_Day option").each(function () {
                if ($(this).val() == birthdayDay) {
                    $(this).attr("selected", "selected");
                }
            });


            $(".CancelVIP").click(function () {
                var VipID = $(this).attr("data-vip");
                var url = "?act=CancelVIP&vip_id="+VipID;
                var result = confirm("確定要停用這張卡嗎?");

                if (result) {
                    $.get(url, function (data) {
                        var obj = JSON.parse(data);
                        window.location.href = "pos_result.aspx?result=" + obj.result;
                    });
                }
              
            })

            $("#mobile").change(function () {
                console.log(act);
                if (act == "edit") {
                    return;
                }

                var mobile = $(this).val();
                var url = "?act=CheckMobile&mobile=" + mobile;
                $.get(url, function (data) {
                    var obj = JSON.parse(data);
                    console.log(obj)
                    if (obj.result) {
                        if (obj.HasTheSameVIP) {
                            alert("已有相同行動電話之會員，請改用補發卡號");
                            $("#SaveBtn").hide();
                        } else {
                            $("#SaveBtn").show();
                        }
                    }

                });
                
            })


            $("#Extend").click(function () {
                var vip_id = $(this).attr("data-vipid");
                var valid_date = $("#valid_date").html();
                ClerkID = clerk.id;
                var url = "?act=VipExtend&vip_id=" + vip_id + "&ClerkID=" + ClerkID + "&valid_date=" + valid_date;
                $.get(url, function (data) {
                    var obj = JSON.parse(data);
                    console.log(obj)
                    if (obj.result) {
                        alert("延長效期成功")
                        location.reload();
                    } else {
                        alert("延長效期失敗")
                    }

                });
            })
        });

        function scan() {
            var o = $("#input");
            var id = o.val();
            o.val("");

            if (id.match(/^VIP[A-Z0-9]{9}$/)) {
                window.location = "?vip_id=" + id;
            }
            else if (id.match(/^09\d+/)) {
                window.location = "?mobile=" + id;
            }
            else {
                alert("輸入錯誤！");
            }
        }
 
    </script>
</head>
<body>
    <div id="container" class="style2">
        <div id="content">
            <div id="vip_open" runat="server">
                <h2 class="style1">VIP卡<%if (act.Contains("add")) { Response.Write("開卡"); } else { Response.Write("查詢"); } %></h2>
                <form onsubmit="scan(); return false;">
                    <input type="text" id="input" />&nbsp;(輸入『VIP卡號』)<br/>
                </form>
            </div>

            <%if(VipInfoDT.Rows.Count>0){ %>
        <table class="EU_DataTable">
            <tbody>
                <tr>
                    <th></th>
                    <th>VIP ID</th>
                    <th>姓名</th>
                    <th>電話</th>
                    <th>行動電話</th>
                    <th>email</th>
                    <th>紅利點數</th>
                    <th>生日</th>
                    <th>VIP有效日期</th>
                    <th>開卡時間</th>
                    <th>狀態</th>
                </tr>
                <%foreach(System.Data.DataRow row in VipInfoDT.Rows) {%>
                <tr>
                    <td><a href="?vip_id=<%=row["vip_id"].ToString()  %>" >選取</a> &nbsp;&nbsp;&nbsp;
                        <%if (row["mobile"].ToString().Length>0){%><a href="javascript:void(0)" class="CancelVIP" data-vip="<%=row["vip_id"].ToString() %>" > 停用</a><%} %></td>
                    <td><%=row["vip_id"].ToString() %></td>
                    <td><%=row["name"].ToString() %></td>
                    <td><%=row["tel"].ToString() %></td>
                    <td><%=row["mobile"].ToString() %></td>
                    <td><%=row["email"].ToString() %></td>
                    <td><%=row["bonus"].ToString() %></td>
                    <td><%=covertshortDate(row["birthday"].ToString()) %></td>
                    <td><%=covertshortDate(row["valid_date"].ToString()) %></td>
                    <td><%=covertshortDate(row["create_date"].ToString()) %></td>
                    <td><%=DateTime.Parse(row["valid_date"].ToString())>DateTime.Now?"<span style='color:green'>未到期</span>":"<span style='color:red'>已過期</span>" %></td>
                </tr>
                <%} %>
            </tbody>
        </table>

        <%} %>

            <div id="edie_vip_member" runat="server">
                <h2 class="style1"><%if (act.Contains("add")) { Response.Write("VIP卡開卡"); } else { Response.Write("修改 VIP 會員資料"); } %></h2>
                <form method="post" action="?act=_<%=act %>&vip_id=<%=vip_id %>">
                    <table class="table table-bordered tablesorter" style="width:60%">
                        <tbody>
                            <tr>
                                <td>VIP ID</td>
                                <td><%=vip_id %></td>
                            </tr>
                            <tr>
                                <td>姓名</td>
                                <td>
                                    <input type="text" name="name" value="<%=name %>" /></td>
                            </tr>
                            <tr>
                                <td>電話</td>
                                <td>
                                    <input type="text" name="tel" value="<%=tel %>" /></td>
                            </tr>
                            <tr>
                                <td>行動電話</td>
                                <td>
                                    <input type="text" id="mobile" name="mobile" value="<%=mobile %>" /><%if (act=="_add"){ %><span style="color:red">(必填)</span><%} %></td>
                            </tr>
                            <tr>
                                <td>email</td>
                                <td>
                                    <input type="text"  name="email" value="<%=email %>" size="40" /></td>
                            </tr>
                            <tr>
                                <td>紅利</td>
                                <td>
                                    <%=bonus %><input type="hidden" name="bonus" value="<%=bonus %>" /></td>
                            </tr>

                            <tr>
                                <td>生日</td>
                                <td>
                                    <select name="Date_Month" id="Date_Month">
                                        <%for (int i = 1; i <= 12; i++)
                                          { %>
                                        <%string MonthItem = i.ToString("D2"); %>
                                        <option label="<%=MonthItem %>" value="<%=MonthItem %>"><%=MonthItem %></option>
                                        <%} %>
                                    </select>
                                    <select name="Date_Day" id="Date_Day">
                                        <%for (int i = 1; i <= 31; i++)
                                          { %>
                                        <%string DayItem = i.ToString("D2"); %>
                                        <option label="<%=DayItem %>" value="<%=DayItem %>"><%=DayItem %></option>
                                        <%} %>
                                    </select>
                                    (上一次使用壽星折扣: <%= last_birthday%>)
                                </td>
                            </tr>
                            <tr>
                                <td>VIP有效日期</td>
                                <td id="valid_date"><%= valid_date%></td>
                            </tr>
                            <tr>
                                <td>開卡時間 </td>
                                <td><%= create_date%></td>
                            </tr>

                        </tbody>
                    </table>
                    <input class="btn btn-default" type="submit" value="儲存"  id="SaveBtn"  />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <input class="btn btn-default" type="button" value="取消編輯" onclick="javascript: history.back();"/>&nbsp;&nbsp;&nbsp;&nbsp;
                    <%if(ShowExtend){ %>
                     <input class="btn btn-default" type="button" value="延長VIP時限"  id="Extend" data-vipid="<%=vip_id %>" />
                    <%} %>
                </form>

                <hr/>
                <%if (act.Contains("add")==false) { %>
                <form method="post" action="?act=exchage&vip_id=<%=vip_id %>">
                    新卡號:
                <input type="text" name="new_vip_id" value=""/><br />
                    <input class="btn btn-default" type="submit" value="補發"/>
                </form>
                <%} %>
            </div>

            <%if (VipOrderDT.Rows.Count > 0)
              {
                  int RowNo = 0;
                  %>
            <h2 class="style1">訂單紀錄</h2>
            <h4>訂單總金額：<%=TotalOrderAmount %>元</h4>
            <table class="EU_DataTable">
                <tbody>
                    <tr>
                        <th>編號</th>
                        <th>訂單編號</th>
                        <th>金額</th>
                        <th>付款方式</th>
                        <th>POS機編號</th>
                        <th>結帳人</th>
                        <th>交易時間</th>
                        <th>發票號碼</th>
                        <th>使用紅利</th>
                        <th>交易明細</th>
                    </tr>
                    <%foreach (System.Data.DataRow row in VipOrderDT.Rows) {
                          RowNo++;
                          %>
                    <tr>
                        <td><%=RowNo %></td>
                        <td><%=row["OrderID"].ToString() %></td>
                        <td><%=row["Amount"].ToString() %></td>
                        <td><%=row["PayType"].ToString() %></td>
                        <td><%=row["PosNo"].ToString() %></td>
                        <td><%=row["account"].ToString() %></td>
                        <td><%=row["OrderTime"].ToString() %></td>
                        <td><%=row["InvoiceNo"].ToString() %></td>
                        <td><%=row["BonusUsed"].ToString() %></td>
                        <td><a href="javascript:void(0)" class="OrderID" data-orderid="<%=row["OrderID"].ToString() %>">查看</a></td>
                    </tr>
                    <%} %>
                </tbody>
            </table>
            <%} %>
        </div>
        
    </div>
    

    <div id="OrderItem" ></div>
</body>
</html>
