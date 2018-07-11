<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pos_stock_query.aspx.cs" Inherits="OBShopWeb.pos_stock_query" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <title>POS_庫存查詢</title>
   <link href="layout.css" rel="stylesheet" />
    <link href="css/layout.css" rel="stylesheet" />
    <link rel="Stylesheet" type="text/css" href="css/POS_style.css"/>

     <script src="js/jquery-1.9.1.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {

            $("#input").focus();

            GetName();
            $("body").click(function () {
                $("#input").focus();
            })

            $(".keyboicon_").click(function () {
                $(".KeyinFloat").show();
            });

            $(".ESC").click(function () {
                $("#TempInput").val("");
                $(".KeyinFloat").hide();
            });
               
            $(".number").click(function () {
                var  input=$("#TempInput").val();
                var num = $(this).html();
                input += num;
                $("#TempInput").val(input);

            })

            $(".enter").click(function () {
                $("#input").val($("#TempInput").val())
                $("#TempInput").val("");
                $(".KeyinFloat").hide();
            })

            $(".backspace").click(function () {
                var input = $("#TempInput").val();
                input = input.substring(0, input.length - 1);
                $("#TempInput").val(input);
            })


            $("#search").click(function () {
                scan();
            });

            $(".logout").click(function () {
                var clerk = {};
                localStorage.setItem('clerk', JSON.stringify(clerk));
                window.location.href = "logout.aspx";
            });

            $(".Product_Row").click(function () {
                var barcode = $(this).attr("data-barcode");
                var url = "pos_stock.aspx?barcode=" + barcode;
                window.open(url, "", ' width=1024,height=768,top=0, left=0');

            })

        });

        function scan() {
            var o = $("#input");
            var id = o.val();
            o.val("");

            if (id.match(/^\d{8}$/)) {
                window.location = "?key=" + id+"&QueryType=0";
            } else {
                window.location = "?key=" + id + "&QueryType=1";
            }
            
        }

        function GetName() {
            var LoginClerk = localStorage.getItem("clerk");
            if (LoginClerk == "{}" || LoginClerk == undefined) {
                //window.location.href = "logout.aspx";
                $("#ClerkNmae").html("無法顯示的名稱");
            } else {
                var clerk = JSON.parse(LoginClerk);
                ClerkNmae = clerk.name;
                ClerkID = clerk.id;
                //console.log(ClerkNmae)
                $("#ClerkNmae").html(ClerkNmae);
            }
            
        }
    </script>

</head>
<body>
    <div class="All">
   
   <!--最上面的頁面連結按鈕-->
<div class="Top_Menu">
            <ul>
                <li><a href="pos_check_out.aspx">交易結帳</a></li>
                <li><a href="pos_stock_query.aspx">庫存查詢</a></li>
                <li><a href="pos_order_query.aspx" onclick="window.open(this.href, '', 'width=1024,height=768,top=0, left=0'); return false;">交易查詢</a></li>
                <li><a href="ActivitiesSetting.aspx">販促設定</a></li>
                <li><a href="index.aspx">管理頁面</a></li>
                 <li><a href="http://www.obdesign.com.tw/" onclick="window.open(this.href, '', 'width=1024,height=768,top=0, left=0'); return false;">官網首頁</a></li>
            </ul>
        </div>

   <!--左邊結帳細目-->
  <div class="BIG_list">
   
        <form onsubmit="scan(); return false;">
   <div class="SearchDIV">
   <ul>
   <li style="float: left;">請輸入型號：<input name="TextBoxEmail1" type="text" size="25" id="input"> </li>
   <li style="float: left;"><span class="SAMIicon_" id="search">查詢</span></li>
   <li style="float: left;"><span style="color:#ff0000">   <%=QueryResultMsg %></span></li>
   </ul>
   
   </div>
   </form>
   
   
   <div class="SearchICON">
   <li class="keyboicon_">呼叫數字鍵盤</li>
   </div>
   
   
   
   <div style="clear:both;"></div><!--間容ie7的div浮動-->
   
   <div class="BIGpro_infoTIT">查詢結果</div>
   
   <div class="BIG2pro_infoDT">
     <table width="1000" border="0" cellspacing="0" cellpadding="0">
  <tbody>

     
     <tr>
         <td style="width: 10%;">型號</td>
         <td style="width: 10%;">條碼</td>
         <td style="width: 36%;">品名</td>
         <td style="width: 7%;">價格</td>
         <td style="width: 7%;">顏色</td>
         <td style="width: 7%;">尺寸</td>
         <td style="width: 7%;">B1</td>
         <td style="width: 7%;">展售</td>
         <td style="width: 7%;">總倉庫存</td>
    </tr>
          <%if (!string.IsNullOrEmpty(series_id))
            { %>
          <%foreach (System.Data.DataRow row in sd.stockDT.Rows)
            { %>
          <tr <%if (product_id.Trim() == row["ProductId"].ToString().Trim() || key.Trim() == row["BarCode"].ToString().Trim())
                {%>
              style="background-color: #FCEE55;" <%} %>  class="Product_Row" data-barcode="<%=row["BarCode"]%>">
              <td style="width: 10%;"><%=row["ProductId"]%></td>
              <td style="width:10%;"><%=row["BarCode"]%></td>
              <td style="width: 36%;"><%=sd.series_name %></td>
              <td style="width:7%;"><%=row["Price"] %></td>
              <td style="width: 7%;"><%=row["Color"]%></td>
              <td style="width: 7%;"><%=row["Size"]%></td>
              <td style="width: 7%;"><%=OBShopWeb.Poslib.Stock.GetB1Stock(row["ProductId"].ToString()) %></td>
              <td style="width: 7%;"><%=OBShopWeb.Poslib.Stock.GetShowStock(row["ProductId"].ToString()) %></td>
              <td style="width: 7%;"><%=OBShopWeb.Poslib.Stock.GetKWStockAllocation(row["ProductId"].ToString()) %></td>
          </tr>
          <% }  %>
      <%} %>
  </tbody>
</table>

     

   </div>
   

   
   



   <div style="clear:both;"></div><!--間容ie7的div浮動-->
   
   <div class="BIGbottom_info">
   <ul>
    <li>登入者：<span id="ClerkNmae"></span> <span class="logout">登出</span></li>    
   </ul>
   </div>
   
  <div>
   
   </div>
   
</div>
   
  <!--右邊鍵入區-->
        <div class="KeyinFloat" style="display:none;left:30%" >

            <ul>
                <li class="li-N"><input name="TextBoxEmail1" style="font-size:30px;" type="text" size="22" id="TempInput" /> </li>
                <li class="li-0 number">7</li>
                <li class="number">8</li>
                <li class="number">9</li>
                <li style="width: 70px;">─</li>
                <li style="width: 70px;">┼</li>
                <li class="li-0 number">4</li>
                <li class="number">5</li>
                <li class="number">6</li>
                <li style="width: 150px;" class="ESC">ESC</li>
                <li class="li-0 number">1</li>
                <li class="number">2</li>
                <li class="number">3</li>
                <li class="backspace"></li>
                <li class="li-0 number" style="width: 174px;">0</li>
                <li>‧</li>
                <li class="enter"></li>
            </ul>

        </div>
        <!--右邊鍵入區END-->


</div>
  

</body>
</html>
