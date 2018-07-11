<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetSerailProductByProductID.aspx.cs" Inherits="OBShopWeb.GetSerailProductByProductID" %>
<!DOCTYPE html>

<div class="pro_infoDT" style="width:600px;height:400px">
<table class="SerialList" style="width:600px"  border="0" cellspacing="0" cellpadding="0">
                    <tbody>
                        <tr>
                            <td style="width: 30px;"></td>
                            <td style="width: 80px;">款號</td>
                            <td style="width: 240px;">款名</td>
                            <td style="width: 48px;">顏色</td>
                            <td style="width: 48px;">尺寸</td>
                            <td style="width: 48px;">單價</td>
                            <td style="width: 38px;">B1</td>
                             <td style="width: 38px;">展售</td>
                        </tr>
                        <%int Counter = 0; %>
                        <%foreach(OBShopWeb.Poslib.CheckOutProduct Product in ProductList) {%>

                           <%Counter++; %>
                    <tr  id="<%=Product.barcode %>_barcode"  class="SerialItem"> 
                        <td style="width: 30px; "> <%=Counter %></td>
                        <td style="width: 80px;" class="itemNo"><%=Product.product_id %></td>
                        <td style="width: 240px; "><%=Product.series_name %></td>
                        <td style="width: 48px; " class="color"><%=Product.color %></td>
                        <td style="width: 48px; "><%=Product.size %></td>
                        <td style="width: 48px; " class="price"><%=Product.price %></td>
                        <td style="width: 38px; " id="BA0385-5-M_SkuQty"><%=Product.StkQty %></td>
                        <td style="width: 38px;" id="BA0385-5-M_ShowQty"><%=Product.ShowQty %></td>
                        <%  } %>
                    </tr>
                    </tbody>
                </table>
    </div>
