<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ActivitiesSetting.aspx.cs" Inherits="OBShopWeb.ActivitiesSetting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>折扣價設定</title>
    <link href="css/bootstrap.min.css" rel="stylesheet" />
     <link href="css/jquery-ui.css" rel="stylesheet" />

    <script src="js/jquery-1.9.1.js"></script>
     <script src="js/bootstrap.min.js"></script>
        <link href="css/jquery-ui.css" rel="stylesheet" />
    <link href="css/jquery-ui.theme.min.css" rel="stylesheet" />
    <script src="js/jquery-ui.min.js"></script>
    <script type="text/javascript">
        var ID = 0;
        var act = '<%=act %>';
        var ActType = "<%=Type  %>";
        var TypeName = "金額";
        var ActivityID = '<%=ActivityID%>';
        var DiscountNPriceChanged = '<%=DiscountNPriceChanged%>'
        var ActivityRight = '<%=ActivityRight%>'

        $(function () {

            if (DiscountNPriceChanged == "True") {
                $("#ImportBtn").prop('disabled', true);
                $("#RemoveAllBtn").prop('disabled', true);
                $("#ImportBtn").val("新品折N元品項一天只能異動一次");
                $(".deleteSerialID").removeClass("deleteSerialID")
            }
            //$('.sortable').sortable({
            //    items: 'tr:not(:first)'
            //});

            if (ActivityRight == "True") {
                $("#Tips").html("商控人員你好，你可以設定所有折扣")
            } else {
                $("#Tips").html("你不是商控人員，只可以設定新品折X元的折扣類型")

                if (ActType == "滿X元X%元" || ActType == "滿X件X%元") {
                    $("#EditBtn").prop("disabled", true);
                    $("#ImportBtn").prop("disabled", true);
                    $("#RemoveAllBtn").prop("disabled", true);
                    $(".deleteSerialID").removeClass("deleteSerialID")
                }
            }


            if (act=="add"||act == "edit") {
                ParseParamList();
                CheckType();
            }

            $(".Date").datepicker({ dateFormat: "yy/mm/dd" });

            var Height = 250;
            var Width = $(window).width() * 1/3;

            $("#Param").dialog({
                open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                title:"折扣條件",
                width: Width,
                height: Height,
                autoOpen: false,
                modal: true,
                buttons: {
                    "取消": function () {
                        $(this).dialog("close");
                        $("#Limit").val('');
                        $("#Discount").val('');
                    }, "確認": function () {


                        if (ActType != "新品折X元") {
                            var Discount = parseInt($("#Discount").val());
                            if ($("#Discount").val().length == 0) {
                                alert("折扣不可為空")
                                return;
                            }
                            if (Discount < 10 || Discount > 100) {
                                alert("折扣必須介於10到100")
                                return;
                            }
                        }
                       
                        AddNewParam();
                        $(this).dialog("close");
                        $("#Limit").val('');
                        $("#Discount").val('');
                    }
                }
            });

            $(".ParamOpen").click(function () {
                $("#Param").dialog("open");
            })

            //delete Activity
            $(".deleteActivityBtn").click(function () {
                var ActName = $(this).attr("data-name");
                var ActID = $(this).attr("data-actid");
                var r = confirm("確認要刪除\n" + ActName + "  \n這個折扣活動嗎?");
                if (r == true) {
                    var url = "?act=deleteActivity&ActivityID=" + ActID;
                    $.get(url, function (data) {
                        console.log(data);
                        var json = JSON.parse(data);
                        if (json.result) {
                            alert("刪除成功!!");
                            location.reload();
                        }
                        else {
                            alert("刪除失敗!!");
                        }
                    })
                }
            })

            //delete SerialID
            $(".deleteSerialID").click(function () {

                if (DiscountNPriceChanged == "True") {
                    alert("新品折N元品項一天只能異動一次");
                    return;
                }

                var Div = $(this).parent("div");
                var SerialID = Div.find("span").html()
                console.log(Div.find("span").html());
                var url = "?act=deleteSerialID&ActivityID=" + ActivityID + "&SerialID=" + SerialID;
                $.get(url, function (data) {
                    console.log(data);
                    var json = JSON.parse(data);
                    if (json.result) {
                        Div.remove();
                    }
                    else {
                        alert("刪除失敗!!");
                    }
                })
            })

            //delete Param
            $(document).on("click", ".deleteParam", function () {
                var ID = $(this).attr("id").replace("btn_", "");
                var DeleteID = "#Param_" + ID;
                $(DeleteID).remove();
                Counting();
            });


            $('input[name="ActType"],input[name="EditActType"]').change(function () {
                $(".Param2").html("折扣")
                $("#Param1").show();
                ActType = $(this).val();
                $("#ParamListString").val("[]");
                $("#ParamListSpan").html("");
                $("#EditParamListString").val("[]");
                $("#EditParamListSpan").html("");
                switch (ActType) {
                    case "滿X元X%元":
                        $("#unit").html("%")
                        $(".Param1").html("金額")
                        break;
                    case "滿X件X%元":
                        $("#Unit").html("%");
                        $(".Param1").html("件數")
                        break;
                    case "新品折X元":
                        $("#Unit").html("元")
                        $(".Param2").html("折價")
                        $("#Param1").hide();
                    default:
                }
            })
        })

        function AddNewParam() {

            if (ActType != "新品折X元") {
                var Limit = $("#Limit").val();
                var Discount = $("#Discount").val();
                var ParamName = "#ParamList";
                CheckType();

                if (act == "edit") {
                    ParamName = "#EditParamList";
                }

                if (Limit.length == 0 || Discount.length == 0) {
                    return
                }

                ID++;
                var content = $(ParamName + "Span").html();
                var html = '<span id="Param_' + ID + '" clsss="Param">' + TypeName + '：<span id="Limit_' + ID + '">' + Limit + '</span>&nbsp;&nbsp;折扣：<span id="Discount_' + ID + '">' + Discount + '%</span>&nbsp;&nbsp;&nbsp;&nbsp;<span><input id="btn_' + ID + '" type="button" class="btn btn-danger deleteParam" value="X" />&nbsp;&nbsp;</span></span>';
                $(ParamName + "Span").html(content + html);
                Counting();

            } else {
                var Discount = $("#Discount").val();
                var ParamName = "#ParamList";
                var ParamList = [];

                if (act == "edit") {
                    ParamName = "#EditParamList";
                }

                var html = '<span id="Param_' + ID + '" clsss="Param"> 新品折價：<span id="Discount_' + ID + '">' + Discount + '元</span>&nbsp;&nbsp;<input id="btn_' + ID + '" type="button" class="btn btn-danger deleteParam" value="X" />&nbsp;&nbsp;</span></span>';
                $(ParamName + "Span").html(html);

                $(ParamName + "Span").find(".btn").each(function () {
                    var Param = {};
                    var ID = $(this).attr("id").replace("btn_", "");
                    var Discount = parseInt($("#Discount_" + ID).html());
                    Param.Limit = 0;
                    Param.Discount = Discount;
                    ParamList.push(Param);
                })
                var ParamListString = JSON.stringify(ParamList);
                $(ParamName + "String").val(ParamListString);
                console.log($(ParamName + "String").val());
            }
            
        }

        function Counting() {
            var ParamList = [];
            var ParamName = "#ParamList";
            if (act == "edit") {
                ParamName = "#EditParamList";
            }

            $(ParamName+"Span").find(".btn").each(function () {
                var Param = {};
                var ID = $(this).attr("id").replace("btn_", "");
                var Limit = parseInt($("#Limit_" + ID).html());
                var Discount = parseInt($("#Discount_" + ID).html());
                Param.Limit = Limit;
                Param.Discount = Discount;
                ParamList.push(Param);
            })

            //排序
            ParamList.sort(function (a, b) {
                var a1 = a.Limit;
                var b1 = b.Limit;
                if (a1 == b1) return 0;
                return a1 > b1 ? 1 : -1;
            });

            var ParamListString = JSON.stringify(ParamList);
            $(ParamName + "String").val(ParamListString);

            console.log($(ParamName + "String").val());
        }

        function ParseParamList() {
            CheckType();
            var ParamListString = "[]";
            var ParamName = "#ParamList";
            if (act == "edit") {
                ParamName = "#EditParamList";
            }

            if ($(ParamName + "String").val().length > 0) {
                ParamListString = $(ParamName + "String").val();
            }
           
            var ParamList = jQuery.parseJSON(ParamListString);
            $.each(ParamList, function (key, value) {
                ID++;
                if (TypeName.length > 0) {
                    var content = $(ParamName + "Span").html();
                    var Limit = value.Limit;
                    var Discount = value.Discount;
                    var html = '<span id="Param_' + ID + '" clsss="Param">' + TypeName + '：<span id="Limit_' + ID + '">' + Limit + '</span>&nbsp;&nbsp;折扣：<span id="Discount_' + ID + '">' + Discount + '%</span>&nbsp;&nbsp;<span><input id="btn_' + ID + '" type="button" class="btn btn-danger deleteParam" value="X" />&nbsp;&nbsp;</span></span>';
                    $(ParamName + "Span").html(content + html);
                } else {
                    var content = $(ParamName + "Span").html();
                    var Discount = value.Discount;
                    var html = '<span id="Param_' + ID + '" clsss="Param"> 新品折價：<span id="Discount_' + ID + '">' + Discount + '元</span>&nbsp;&nbsp;<input id="btn_' + ID + '" type="button" class="btn btn-danger deleteParam" value="X" />&nbsp;&nbsp;</span></span>';
                    $(ParamName + "Span").html(html);
                }
               
               
            });
        }

        function CheckType() {

            if (act == "add") {
                ActType = $('input[name="ActType"]:checked').val()
            }

            if (act == "edit") {
                ActType = $('input[name="EditActType"]:checked').val()
            }
           
            if (ActType.indexOf("件") >= 0) {
                TypeName = "件數";
            } else {
                TypeName = "金額";
            }

            if (ActType.indexOf("新品") >= 0) {
                TypeName = "";
                $("#Unit").html("元")
                $(".Param2").html("折價")
                $("#Param1").hide();
            }

            $(".Param1").html(TypeName)
        }

    </script>
    <style type="text/css">
        .auto-style2 {
            width: 20%;
        }

        th{
           background-color: #073163;
           color: #F6F0F0;
        }
        ul{
             width: 90%;
        }

        ul li {
            border: 1px #cccccc solid;
            width: 200px;
            height:40px;
            float: left;
            list-style: none;
            font-size:12pt;
        }

    </style>
</head>
<body>
    <div style="margin-left:30px;margin-top:20px">
    <a href="ActivitiesSetting.aspx" class="btn btn-primary" style="line-height: 2.0">折扣活動清單</a>&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
    <a href="ActivitiesSetting.aspx?act=add" class="btn btn-primary" style="line-height: 2.0">新增折扣活動</a> &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
    <a href="StoreFloatList.aspx" class="btn btn-primary" style="line-height: 2.0">花車折扣</a> 
        &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
    <a href="pos_check_out.aspx" class="btn btn-success" style="line-height: 2.0">返回結帳頁</a><br/>
        <asp:Label ID="Result_lbl" runat="server" ForeColor="Red"></asp:Label><br /><br />
        <span style="color:blue" id="Tips"></span><br />
        <span style="color:green">※新品折N元必須匯入商品系列編號才會生效，最多不得超過30款，非VIP和VIP都適用此折扣※</span><br />
        
    </div>

    <form id="form1" runat="server">
        <div id="ActivitiesDiv" runat="server" style="margin-left:30px">
            <%if(ActivitiesDT.Rows.Count>0){ %>
            <table class="table table-bordered  sortable" style="width: 90%">
                    <tr>
                        <th>編輯</th>
                        <th>刪除</th>
                        <th>促銷活動名稱</th>
                        <th>類型</th>
                        <th>開始日期</th>
                        <th>結束日期</th>
                        <th>身分限定</th>
                    </tr>
                    <%foreach(System.Data.DataRow row in ActivitiesDT.Rows){ %>
                      <tr>
                          <td><a  class="btn btn-primary" href="?act=edit&ActivityID=<%=row["ActivityID"].ToString() %>">編輯</a></td>
                          <td>
                          <a  class="btn btn-danger deleteActivityBtn"  href="javascript:void(0)" data-actid="<%=row["ActivityID"].ToString() %>" data-name="<%=row["Name"].ToString() %>" >刪除</a></td>
                        <td><%=row["Name"] %></td>
                        <td><%=row["Type"] %></td>
                         <td><%=Convert.ToDateTime(row["StartDate"]).ToString("yyyy/MM/dd") %></td>
                        <td><%=Convert.ToDateTime(row["EndDate"]).ToString("yyyy/MM/dd") %></td>
                          <td><%if (bool.Parse(row["CanApplyWithOthers"].ToString())){ %>
                               <span style="color:blue">所有客入</span>
                              <%}else{ %>
                              <%=bool.Parse(row["VIPOnly"].ToString())?"<span style='color:red'>僅限VIP</span>":"<span style='color:green'>非VIP</span>" %>
                              <%} %>
                          </td>
                    </tr>
                    <%} %>
                </table>
            <%} %>
        </div>

        <div id="AddDiv" runat="server" style="margin-left:30px">

            <table class="table table-bordered" style="width: 80%">
                <tbody>
                    <tr>
                        <th>活動名稱</th>
                        <td>
                            <asp:TextBox ID="NameTextBox" runat="server" Style="width: 60%"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>活動類型</th>
                        <td>
                             <asp:RadioButtonList ID="ActType" runat="server" RepeatColumns="5" RepeatDirection="Horizontal">
                                <asp:ListItem Text="滿X元X%元" Value="滿X元X%元"></asp:ListItem>
                                <asp:ListItem Text="滿X件X%元" Value="滿X件X%元"></asp:ListItem>
                                 <asp:ListItem Text="新品折X元" Value="新品折X元"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <th>開始日期</th>
                        <td>
                            <asp:TextBox ID="StartDate" runat="server" CssClass="Date"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>結束日期</th>
                        <td>
                            <asp:TextBox ID="EndDate" runat="server" CssClass="Date"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>折扣條件</th>
                        <td><span id="ParamListSpan"></span>
                            <input class="btn ParamOpen" id="AddParam" type="button" value="+" />
                        <asp:HiddenField ID="ParamListString" runat="server" />
                            </td>
                    </tr>
                    <tr>
                        <th>折扣限定</th>
                        <td> <asp:RadioButtonList ID="VIPOnlyRadioAdd" runat="server" RepeatColumns="5" RepeatDirection="Horizontal">
                                <asp:ListItem Text="非VIP" Value="False"></asp:ListItem>
                                <asp:ListItem Text="VIP限定" Value="True"></asp:ListItem>
                            </asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="SendBtn" runat="server" Text="新增" CssClass="btn btn-success" OnClick="SendBtn_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                        
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>


        <div id="EditDiv" runat="server" style="margin-left:30px">

            <table class="table table-bordered" style="width: 80%">
                <tbody>
                    <tr>
                        <th>活動名稱</th>
                        <td>
                            <asp:TextBox ID="EditNameTextBox" runat="server" Style="width: 60%"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>活動類型</th>
                        <td>
                            <asp:RadioButtonList ID="EditActType" runat="server" RepeatColumns="5" RepeatDirection="Horizontal">
                                <asp:ListItem Text="滿X元X%元" Value="滿X元X%元"></asp:ListItem>
                                <asp:ListItem Text="滿X件X%元" Value="滿X件X%元"></asp:ListItem>
                                <asp:ListItem Text="新品折X元" Value="新品折X元"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <th>開始日期</th>
                        <td>
                            <asp:TextBox ID="EditStartDate" runat="server" CssClass="Date"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>結束日期</th>
                        <td>
                            <asp:TextBox ID="EditEndDate" runat="server" CssClass="Date"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>折扣條件</th>
                        <td>
                            <span id="EditParamListSpan"></span>
                            <input class="btn ParamOpen" id="EditParam" type="button" value="+" />
                            <asp:HiddenField ID="EditParamListString" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th>折扣限定</th>
                        <td> <asp:RadioButtonList ID="VIPOnlyRadioEdit" runat="server" RepeatColumns="5" RepeatDirection="Horizontal">
                                <asp:ListItem Text="非VIP" Value="False"></asp:ListItem>
                                <asp:ListItem Text="VIP限定" Value="True"></asp:ListItem>
                            </asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="EditBtn" runat="server" Text="更新" CssClass="btn btn-warning" OnClick="EditBtn_Click" />
                        </td>
                    </tr>
                </tbody>
            </table>


            <table class="table table-bordered" style="width: 80%">
                <tbody>
                    <tr>
                        <td class="auto-style2">
                            <asp:Button ID="ImportBtn" runat="server" Text="匯入系列編號"  class="btn" OnClick="ImportBtn_Click" />&nbsp;&nbsp;
                            <br />
                            <br />

                            <asp:TextBox ID="ImportTextBox" runat="server" Width="240px" Height="500px" TextMode="MultiLine"></asp:TextBox>
       
                        </td>
                        <td> <div>
                            <asp:Button ID="RemoveAllBtn" runat="server" Text="全部清空" class="btn btn-danger" OnClick="RemoveAllBtn_Click" />
                              <p class="text-success">※若無商品系列明細，則全館商品皆適用此折扣</p></div>
                            <asp:RadioButtonList ID="RadioButtonListActivityProductType" runat="server" RepeatColumns="5" RepeatDirection="Horizontal">
                                 <asp:ListItem Text="以下系列適用" Value="1"></asp:ListItem>
                                <asp:ListItem Text="以下系列不適用" Value="0"></asp:ListItem>
                            </asp:RadioButtonList>
                          
                            <div style="height:480px;width:100%;overflow-y:auto" class="form-horizontal well">
                                <asp:DataList ID="SerialDataList" runat="server"  RepeatColumns="6" RepeatDirection="Horizontal"  OnItemDataBound="SerialDataLis_ItemDataBound" Height="44px" Width="171px">
                                    <ItemTemplate>
                                        <div style="width:100px"><a href="javascript:void(0)" class="btn btn-danger deleteSerialID">X</a>
                                            <asp:Label ID="SerialIDLabel" runat="server" Text="SerialID"></asp:Label>
                                        </div>
                                        &nbsp;&nbsp;&nbsp;
                                    </ItemTemplate>
                                </asp:DataList>
                                
                            </div>

                        </td>
                    </tr>
                </tbody>
            </table>

        </div>


        <div id="Param">
            <div id="Param1">
            <span class="Param1">金額</span>：<input id="Limit" type="text" /><br />
                </div>
            <br />
            <span class="Param2">折扣</span>：<input id="Discount" type="text" /> &nbsp;<span id="Unit">%</span><br />
            
        </div>
    </form>
</body>
</html>
