<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pos_SalesTaxReport.aspx.cs" Inherits="OBShopWeb.pos_SalesTaxReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
      <title>門市營業稅報表</title>
    <link rel="stylesheet" href="css/bootstrap.min.css" />
    <link rel="stylesheet" href="css/bootstrap-datepicker.css" />
    <style>
        body { font-family:微軟正黑體; font-size:16px; }
        div.container-first { margin-top:30px; }
        div.container-padding { padding:20px; width:1000px; clear:both; }
        div.clear-both { clear:both; }
        div.float-left { float:left; padding:2px 0 2px 3px; }
        div.report-title { width:100px; text-align:left; padding:2px 0 2px 3px; }
        .table td:nth-child(1) { text-align:right }
        .col-md-2 { position:inherit; }
    </style>

    <script src="js/jquery-1.9.1.js"></script>
     <script src="js/bootstrap.min.js"></script>
    <script src="js/bootstrap-datepicker.js"></script>
    <script src="js/locales/bootstrap-datepicker.zh-TW.js"></script>

    <script>
        $(document).ready(function ()
        {
            $(".select-date input").datepicker({
                format: "yyyy-mm-dd",
                autoclose: true,
                language: "zh-TW"
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
   <div class="container-first container-padding">
            <fieldset>
                <legend>銷貨資料匯出報表：</legend>
                <div class="select-date clear-both">
                    <div class="float-left report-title">
                        <label>開立日期：</label>
                    </div>
                    <div class="float-left">
                        <asp:TextBox ID="tbxStartDateIssuance" runat="server" placeholder="yyyy-mm-dd" />~
                    <asp:TextBox ID="tbxEndDateIssuance" runat="server" placeholder="yyyy-mm-dd" />
                    </div>
                </div>
                <div class="select-date clear-both">
                    <div class="float-left report-title">
                        <label>異動日期：</label>
                    </div>
                    <div class="float-left">
                        <asp:TextBox ID="tbxUpdStartDateIssuance" runat="server" placeholder="yyyy-mm-dd" />~
                    <asp:TextBox ID="tbxUpdEndDateIssuance" runat="server" placeholder="yyyy-mm-dd" />
                    </div>
                </div>
                 <div class="clear-both">
                     <div class="float-left report-title">
                         <label>選擇作廢：</label>
                     </div>
                     <div class="float-left">
                         <asp:DropDownList ID="ddlIssuanceCancelType"  runat="server">
                             <asp:ListItem Text="含作廢" Value="cancelwith" Selected="True" />
                             <asp:ListItem Text="不含作廢" Value="cancelwithout" />
                             <asp:ListItem Text="僅作廢" Value="cancelonly" />
                         </asp:DropDownList>
                     </div>
                 </div>
                <div class="clear-both">
                    <div class="float-left report-title">
                        <label>資料匯出：</label>
                    </div>
                    <div class="float-left">
                       <asp:Button ID="btnInvoiceCreation" runat="server" Text="銷貨資料匯出" Style="width: 180px;" OnClick="btnInvoiceCreation_Click" />
                        <asp:Button ID="btnInvoiceTxtOutput" runat="server" Text="銷貨資料文字檔匯出" Style="width: 180px;" OnClick="btnInvoiceTxtOutput_Click" />
                    </div>
                </div>
            </fieldset>
        </div>




        <div class="container-padding"">
             <fieldset>
                 <legend>銷貨折讓資料匯出報表：</legend>
                 <div class="select-date clear-both">
                     <div class="float-left report-title">
                         <label>開立日期：</label>
                     </div>
                     <div class="float-left">
                         <asp:TextBox ID="tbxStartDateAllowence" runat="server" placeholder="yyyy-mm-dd" />~
                    <asp:TextBox ID="tbxEndDateAllowence" runat="server" placeholder="yyyy-mm-dd" />
                     </div>
                 </div>
                 <div class="select-date clear-both">
                     <div class="float-left report-title">
                         <label>異動日期：</label>
                     </div>
                     <div class="float-left">
                         <asp:TextBox ID="tbxUpdStartDateAllowence" runat="server" placeholder="yyyy-mm-dd" />~
                <asp:TextBox ID="tbxUpdEndDateAllowence" runat="server" placeholder="yyyy-mm-dd" />
                     </div>
                 </div>
                 <div class="clear-both">
                     <div class="float-left report-title">
                         <label>
                         資料匯出：</label> </div>
                     <div class="float-left">
                        <asp:Button ID="btnAllowenceCreation" runat="server" Text="銷貨折讓資料匯出" Style="width: 180px; display: initial" OnClick="btnAllowenceCreation_Click" />
                         <asp:Button ID="btnAllowenceTxtOutput" runat="server" Text="銷貨折讓文字檔匯出" Style="width: 180px; display: initial" OnClick="btnAllowenceTxtOutput_Click" />
                     </div>
                 </div>
             </fieldset>
        </div>


                <div class="container-padding">
            <asp:Label ID="lblLog" runat="server" ForeColor="Red" Text=""></asp:Label>
        </div>

    </form>
</body>
</html>
