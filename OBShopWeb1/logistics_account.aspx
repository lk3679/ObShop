<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="logistics_account.aspx.cs" Inherits="OBShopWeb.logistics_account" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>物流人員管理</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript">
        //刪除
    function delete_account(element, id) {
        if (confirm("是否刪除 " + id)) {
            element.disabled = true;
            var url = "?act=delete_account&id=" + id + "&r=" + Math.random();
            window.open(url, target = "_blank", "width=100, height=100");
        }
        return false;
    }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    
    <asp:Panel ID="P_list" runat="server">
        <div id="div_list" runat="server">
    
            <span class="style1"><strong>物流人員管理 </strong></span><br class="style3" />
            <asp:Button ID="btn_Add" runat="server" Text="新增" CssClass="style2" 
                onclick="btn_Add_Click" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <span class="style2" style="color: #003300">※特定帳號：</span>
            <asp:TextBox ID="txt_Name" runat="server" CssClass="style2"></asp:TextBox>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_Search" runat="server" Text="搜尋" CssClass="style2" onclick="btn_Search_Click" />            
            &nbsp;<hr />
            &nbsp;<asp:Label ID="lbl_Count" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>

            <asp:GridView ID="gv_Account" runat="server" 
                CellPadding="4" ForeColor="#333333" GridLines="None" CellSpacing="2" 
                CssClass="style4gv" Width="100%" 
                onpageindexchanging="gv_Account_PageIndexChanging" PageSize="20" 
                onrowdatabound="gv_Account_RowDataBound" AllowPaging="True">
                <AlternatingRowStyle BackColor="White" />

                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" 
                    HorizontalAlign="NotSet" />
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
    </asp:Panel>

    <asp:Panel ID="P_add" runat="server" Visible="False">
        <div id="div_add" runat="server">
        <span class="style1"><strong>物流人員管理-新增</strong></span><br class="style3" />

        <br />
        <span class="style2">帳號：</span>
        <asp:TextBox ID="txt_AddAccount" runat="server"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="btn_AddSubmit" runat="server" CssClass="style2" 
                onclick="btn_AddSubmit_Click" Text="確定" ViewStateMode="Enabled" />
        &nbsp;<asp:Button ID="btn_AddCancel" runat="server" CssClass="style2" 
                onclick="btn_Cancel_Click" Text="取消" />
            &nbsp;<asp:Label ID="lbl_AddMessage" runat="server"></asp:Label>
        </div>
    </asp:Panel>

    <asp:Panel ID="P_edit" runat="server" Visible="False">
        <div id="div_edit" runat="server">
        <span class="style1"><strong>物流人員管理-修改</strong></span><br class="style3" />
        <br />
            <span class="style2">ID：</span>
            <asp:Label ID="lbl_ID" runat="server" CssClass="style2"></asp:Label>
            <br />
            <span class="style2">條碼：</span>
            <asp:Label ID="lbl_barcode" runat="server" CssClass="style2"></asp:Label>
            <br />
            <span class="style2">帳號：</span>
            <asp:TextBox ID="txt_EditAccount" runat="server"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="btn_EditSubmit" runat="server" CssClass="style2" 
                onclick="btn_EditSubmit_Click" Text="儲存" />
            &nbsp;<asp:Button ID="btn_EditCancel" runat="server" CssClass="style2" 
                onclick="btn_Cancel_Click" Text="取消" />
            &nbsp;<asp:Label ID="lbl_EditMessage" runat="server"></asp:Label>
            <br />
        </div>
    </asp:Panel>

    </form>
</body>
</html>
