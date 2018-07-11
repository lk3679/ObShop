<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="logistics_print2.aspx.cs" Inherits="OBShopWeb.logistics_print2" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>績效報表(區間)</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/jquery.blockUI.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#btn_Search').click(function () {
                $.blockUI({
                    message: $('<h4 style="text-align:center"><img src="image/loading4.gif" /> <br/><br/>loading...</h4>'),
                    css: {
                        top: ($(window).height()) / 2 + 'px',
                        left: ($(window).width()) / 3 + 'px',
                        color: '#fff',
                        background: 'none',
                        border: '0px',
                        opacity: 0.6
                    }
                });
            });
        }); 
    </script>

</head>
<body >
    <form id="form1" runat="server">
    <div>
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        <span class="style1"><strong>績效報表(區間)</strong></span>&nbsp;&nbsp;
        <asp:Label ID="lbl_Time" runat="server" Text="" CssClass="style3" ForeColor="#008800"></asp:Label><br class="style2" /><br />
        <span class="style2">開始日期：</span><asp:TextBox ID="txt_Start" runat="server" 
            Width="100px" CssClass="style2"></asp:TextBox>
        <asp:CalendarExtender ID="ImgBtn_Date_CalendarExtender" runat="server" 
            DaysModeTitleFormat="yyyy-MM-dd" Enabled="True" Format="yyyy-MM-dd" 
            PopupButtonID="Imgbtn_Date" TargetControlID="txt_Start" 
            TodaysDateFormat="yyyy-MM-dd">
        </asp:CalendarExtender>
        &nbsp;<asp:ImageButton ID="ImgBtn_Date" runat="server" ImageUrl="~/Image/Calendar.png" />
        &nbsp;<span class="style2">起始時間：</span>
        &nbsp;<asp:DropDownList ID="DDL_Time1" runat="server" CssClass="style2">
            <asp:ListItem Selected="True" Value="00">0</asp:ListItem>
            <asp:ListItem Value="01">1</asp:ListItem>
            <asp:ListItem Value="02">2</asp:ListItem>
            <asp:ListItem Value="03">3</asp:ListItem>
            <asp:ListItem Value="04">4</asp:ListItem>
            <asp:ListItem Value="05">5</asp:ListItem>
            <asp:ListItem Value="06">6</asp:ListItem>
            <asp:ListItem Value="07">7</asp:ListItem>
            <asp:ListItem Value="08">8</asp:ListItem>
            <asp:ListItem Value="09">9</asp:ListItem>
            <asp:ListItem>10</asp:ListItem>
            <asp:ListItem>11</asp:ListItem>
            <asp:ListItem>12</asp:ListItem>
            <asp:ListItem>13</asp:ListItem>
            <asp:ListItem>14</asp:ListItem>
            <asp:ListItem>15</asp:ListItem>
            <asp:ListItem>16</asp:ListItem>
            <asp:ListItem>17</asp:ListItem>
            <asp:ListItem>18</asp:ListItem>
            <asp:ListItem>19</asp:ListItem>
            <asp:ListItem>20</asp:ListItem>
            <asp:ListItem>21</asp:ListItem>
            <asp:ListItem>22</asp:ListItem>
            <asp:ListItem>23</asp:ListItem>
            <asp:ListItem>24</asp:ListItem>
        </asp:DropDownList>
        &nbsp;<asp:CompareValidator ID="CompareValidator1" runat="server" 
            ControlToCompare="txt_End" ControlToValidate="txt_Start" CssClass="style3" 
            ErrorMessage="結束日期必須大於開始日期" Operator="LessThanEqual"></asp:CompareValidator>
        <br />
            <span class="style2">結束日期：</span><asp:TextBox ID="txt_End" runat="server" 
            Width="100px" CssClass="style2"></asp:TextBox>
        <asp:CalendarExtender ID="ImgBtn_Date2_CalendarExtender" runat="server" 
            DaysModeTitleFormat="yyyy-MM-dd" Enabled="True" Format="yyyy-MM-dd" 
            PopupButtonID="ImgBtn_Date2" TargetControlID="txt_End" 
            TodaysDateFormat="yyyy-MM-dd">
        </asp:CalendarExtender>
        &nbsp;<asp:ImageButton ID="ImgBtn_Date2" runat="server" ImageUrl="~/Image/Calendar.png" />
        &nbsp;<span class="style2">結束時間：</span>
        &nbsp;<asp:DropDownList ID="DDL_Time2" runat="server" CssClass="style2">
            <asp:ListItem Value="00">0</asp:ListItem>
            <asp:ListItem Value="01">1</asp:ListItem>
            <asp:ListItem Value="02">2</asp:ListItem>
            <asp:ListItem Value="03">3</asp:ListItem>
            <asp:ListItem Value="04">4</asp:ListItem>
            <asp:ListItem Value="05">5</asp:ListItem>
            <asp:ListItem Value="06">6</asp:ListItem>
            <asp:ListItem Value="07">7</asp:ListItem>
            <asp:ListItem Value="08">8</asp:ListItem>
            <asp:ListItem Value="09">9</asp:ListItem>
            <asp:ListItem>10</asp:ListItem>
            <asp:ListItem>11</asp:ListItem>
            <asp:ListItem>12</asp:ListItem>
            <asp:ListItem>13</asp:ListItem>
            <asp:ListItem>14</asp:ListItem>
            <asp:ListItem>15</asp:ListItem>
            <asp:ListItem>16</asp:ListItem>
            <asp:ListItem>17</asp:ListItem>
            <asp:ListItem>18</asp:ListItem>
            <asp:ListItem>19</asp:ListItem>
            <asp:ListItem>20</asp:ListItem>
            <asp:ListItem>21</asp:ListItem>
            <asp:ListItem>22</asp:ListItem>
            <asp:ListItem>23</asp:ListItem>
            <asp:ListItem Selected="True">24</asp:ListItem>
        </asp:DropDownList>
        &nbsp;<asp:CompareValidator ID="CompareValidator2" runat="server" 
            ControlToCompare="DDL_Time2" ControlToValidate="DDL_Time1" CssClass="style3" 
            ErrorMessage="結束時間必須大於開始時間" Operator="LessThanEqual"></asp:CompareValidator>
        <br /> 
        <span class="style2">種類：</span>
        <asp:DropDownList ID="DDL_Type" runat="server" CssClass="style2" 
            AutoPostBack="True" onselectedindexchanged="DDL_Type_SelectedIndexChanged">
            <asp:ListItem Value="0">全部</asp:ListItem>
            <asp:ListItem Value="1">撿貨</asp:ListItem>
            <asp:ListItem Value="1+">撿貨(分區)</asp:ListItem>
            <%--          <asp:ListItem Value="2">分貨</asp:ListItem>
            <asp:ListItem Value="3">驗貨</asp:ListItem>
            <asp:ListItem Value="4">包貨</asp:ListItem>
            <asp:ListItem Value="5">點台組</asp:ListItem>
            <asp:ListItem Value="6">貼條碼</asp:ListItem>--%>
            <asp:ListItem Value="7">確認</asp:ListItem>
            <%--<asp:ListItem Value="8">上架</asp:ListItem>--%>
            <asp:ListItem Value="50">入庫確認</asp:ListItem>
            <asp:ListItem Value="51">入庫上架</asp:ListItem>
            <asp:ListItem Value="52">移動儲位</asp:ListItem>
            <%--<asp:ListItem Value="53">調出撿貨確認(10/15之前)</asp:ListItem> 
            <asp:ListItem Value="56">調出驗貨確認(10/15之前)</asp:ListItem> --%>
            <asp:ListItem Value="57">調出撿貨確認</asp:ListItem> 
            <asp:ListItem Value="58">調出驗貨確認</asp:ListItem> 
            <asp:ListItem Value="54">盤點無條件上架</asp:ListItem>
            <asp:ListItem Value="55">盤點無條件打銷</asp:ListItem>
        </asp:DropDownList>
        &nbsp;&nbsp;
        <asp:CheckBox ID="CB_Sort" runat="server" CssClass="style2" Text="依區域排序" 
            Visible="False" ForeColor="#006600" />
        &nbsp;&nbsp;
        <asp:Button ID="btn_Search" runat="server" Text="查詢" CssClass="style2" onclick="btn_Search_Click" />
        &nbsp;&nbsp;<asp:Button ID="btn_XLS" runat="server" Text="匯出XLS" CssClass="style2" 
            onclick="btn_XLS_Click" />
        <br />
        <hr />
        &nbsp;<asp:Label ID="lbl_Count" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>

        <asp:GridView ID="gv_logistics" runat="server" 
            CellPadding="2" ForeColor="#333333" GridLines="None" CellSpacing="3" 
            Cssclass="style4gv" Width="100%" onrowdatabound="gv_logistics_RowDataBound">
            <AlternatingRowStyle BackColor="#FFFBD6" />
            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
            <SortedAscendingCellStyle BackColor="#FDF5AC" />
            <SortedAscendingHeaderStyle BackColor="#4D0000" />
            <SortedDescendingCellStyle BackColor="#FCF6C0" />
            <SortedDescendingHeaderStyle BackColor="#820000" />
        </asp:GridView>
        
        <hr />
    </div>
    </form>
</body>
</html>
