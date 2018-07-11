<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pos_sale_analysis.aspx.cs" Inherits="OBShopWeb.pos_sale_analysis" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>門市銷售分析</title>

    <link rel="stylesheet" href="css/bootstrap.min.css" />
    <link rel="stylesheet" href="css/bootstrap-datepicker.css" />

    <script src="js/jquery-1.9.1.js"></script>
    <script src="js/bootstrap.min.js"></script>
    <script src="js/bootstrap-datepicker.js"></script>
    <script src="js/locales/bootstrap-datepicker.zh-TW.js"></script>
    <script src="js/jquery.tablesorter.js"></script>

    <style type="text/css">
        .colHeader2 {
            width: 57px;
        }
        .colHeader3 {
            width: 65px;
        }
        .colHeader4 {
            width: 100px;
        }
        .colHeader5 {
            width: 95px;
        }

        table.tablesorter thead tr .header {
            background-image: url(Image/tablesorter/bg.gif);
            background-repeat: no-repeat;
            background-position: center right;
            cursor: pointer;
        }
        table.tablesorter thead tr .headerSortUp {
            background-image: url(Image/tablesorter/asc.gif);
        }
        table.tablesorter thead tr .headerSortDown {
            background-image: url(Image/tablesorter/desc.gif);
        }

    </style>
    <script type="text/javascript">
        
        $(document).ready(function () {
            if ($("#HiddenShowSort").val()== '1') {
                $("#GVSearch").tablesorter();
            }
            $(".select-date").datepicker({
                format: "yyyy-mm-dd",
                autoclose: true,
                language: "zh-TW"
            });
        });
    </script>
</head>
<body>
    
    <form id="form1" runat="server">
        <div class="row col-md-5 col-md-offset-1" style="margin-bottom:10px">
            <h3>門市銷售報表</h3>
            <div class=" form-group">
                <label>開始日期：</label>
                <asp:TextBox ID="tbStartDate" CssClass="select-date" runat="server"></asp:TextBox>
                ~
                <label>結束日期：</label>
                <asp:TextBox ID="tbEndDate" CssClass="select-date" runat="server"></asp:TextBox>
            </div>
            <div class="form-group">
                <label>系列編號關鍵字：</label><asp:TextBox ID="tbSID" runat="server"></asp:TextBox>
            </div>
            <div class="form-group">
                <label>系列名稱關鍵字：</label><asp:TextBox ID="tbSName" runat="server"></asp:TextBox>
            </div>

            <asp:Button ID="btnSearch" runat="server" Text="查詢" CssClass="btn btn-default" OnClick="btnSearch_Click" />
            <asp:Button ID="btnSearchMore" runat="server" Text="進階查詢" CssClass="btn btn-default" OnClick="btnSearchMore_Click" />
        </div>
        
        <div class="row col-md-10 col-md-offset-1">
        <asp:GridView ID="GVSearch" AutoGenerateColumns="False" CssClass=" table table-bordered tablesorter" runat="server" style="width:0%" OnPreRender="GVSearch_PreRender" OnRowDataBound="GVSearch_RowDataBound" AllowSorting="True">
            <Columns>

                <asp:TemplateField HeaderText="排序" ItemStyle-CssClass="colHeader2">
                    <ItemTemplate>
                        <asp:Label ID="lbNo" runat="server" Text='<%# Container.DataItemIndex +1 %>' ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="圖片" >
                    <ItemTemplate>
                        <asp:Image ID="ImageSID" runat="server"  Height="100" Width="100" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="系列名稱" >
                    <ItemTemplate>
                        <asp:Label ID="lbSName" runat="server" ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="系列編號" ItemStyle-CssClass="colHeader4">
                    <ItemTemplate>
                        <asp:Label ID="lbSID" runat="server" CssClass="" ></asp:Label>
                        <asp:HyperLink ID="linkSID" runat="server" Target="_blank"  ></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="產品編號" >
                    <ItemTemplate>
                        <asp:Label ID="lbPID" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="尺寸" >
                    <ItemTemplate>
                        <asp:Label ID="lbSize" runat="server" ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="顏色" >
                    <ItemTemplate>
                        <asp:Label ID="lbColor" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="期間銷售量" ItemStyle-CssClass="colHeader5">
                    <ItemTemplate>
                        <asp:Label ID="lbSaleCount" runat="server" ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="庫存量" ItemStyle-CssClass="colHeader3">
                    <ItemTemplate>
                        <asp:Label ID="lbStorage" runat="server" ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="期間進貨量" ItemStyle-CssClass="colHeader5">
                    <ItemTemplate>
                        <asp:Label ID="lbPurchaseCount" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="周轉率" ItemStyle-CssClass="colHeader3">
                    <ItemTemplate>
                        <asp:Label ID="lbTurnover" runat="server" ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:HiddenField ID="HiddenCol" runat="server" />
        <asp:HiddenField ID="HiddenShowSort" runat="server" />
        </div>
   </form>
</body>
</html>
