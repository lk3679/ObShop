<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StoreFloatList.aspx.cs" Inherits="OBShopWeb.StoreFloatList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
        <link href="css/bootstrap.min.css" rel="stylesheet" />
     <link href="css/jquery-ui.css" rel="stylesheet" />
     <script src="js/bootstrap.min.js"></script>
    <style type="text/css">

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
            color:navy;
        }

    </style>
</head>
<body>
    <div style="margin-left:30px;margin-top:20px">
    <a href="ActivitiesSetting.aspx" class="btn btn-primary" style="line-height: 2.0">折扣活動清單</a>&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
    </div><br/><br />
 <div id="FloatDiv"  style="margin-left:30px">
            <span style="font-size:14pt">花車折扣：<strong style="color:red"><%=FloatsDiscountRate %></strong></span>
                <br /><br />
            <span style="font-size:14pt">花車商品系列編號：</span><br /><br />
            <ul>
            <% foreach (string SerialID in FloatList)
               { %>
                <li> <%=SerialID  %></li>
              
            <% } %>
            
        </ul>

            </div>
</body>
</html>
