<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuthManagement.aspx.cs"
    Inherits="OBShopWeb.AuthManagement" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>TW-權限管理</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <div>
        <span class="style1"><strong>權限管理</strong></span>
        &nbsp;&nbsp;
        <asp:HyperLink ID="HL_SystemOnlineList" runat="server" CssClass="style2" 
        NavigateUrl="SystemOnlineList.aspx" ForeColor="#FF6600" Target="_blank">※系統Online名單※</asp:HyperLink>
        &nbsp;&nbsp;
        <asp:HyperLink ID="HL_SystemOnlineReport" runat="server" CssClass="style2" 
        NavigateUrl="SystemOnlineReport.aspx" ForeColor="#0066CC" Target="_blank">※所有系統Online名單※</asp:HyperLink>
        <br /><br />
        <asp:TextBox ID="txb_ADaccount" runat="server" CssClass="style2" AutoCompleteType="Disabled" Width="200px"></asp:TextBox>
        &nbsp;
        <asp:Button ID="btn_AddAD" runat="server" CssClass="style2" Text="新增AD帳號" OnClick="btn_AddAD_Click" ForeColor="#990000" />
        &nbsp;
        <asp:Button ID="btnSave" runat="server" CssClass="style2" onclick="btnSave_Click" Text="儲存" />
        &nbsp;
        <asp:CheckBox ID="ckbAutoLoad" runat="server" Checked="True" CssClass="style2" Text="選擇帳號自動載入權限" />
        &nbsp;
        <asp:CheckBox ID="CB_顯示姓名" runat="server" Checked="False" CssClass="style2" ForeColor="#9900CC"
            Text="顯示姓名" AutoPostBack="True" oncheckedchanged="CB_顯示姓名_CheckedChanged"/>
        <br />
        <hr />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table>
                    <tr>
                        <td>
                            <asp:ListBox ID="ltbMapping" runat="server" CssClass="style2" DataTextField="Account"
                                DataValueField="Account" Rows="20" Width="250px" Height="500px" AutoPostBack="True" 
                                OnSelectedIndexChanged="ltbMapping_SelectedIndexChanged" ForeColor="#006600">
                            </asp:ListBox>
                        </td>
                        <td>
                            <asp:Button ID="btnLoad" runat="server" Text="&gt;&gt;" CssClass="style2" 
                                onclick="btnLoad_Click" />
                            <br />
                            <br />
                            <asp:Button ID="btnModify" runat="server" CssClass="style2" 
                                onclick="btnModify_Click" Text="&lt;&lt;" />
                        </td>
                        <td>
                            <asp:ListBox ID="ltbFunction" runat="server" CssClass="style2" DataTextField="Title"
                                DataValueField="Index" Rows="20" Width="400px" Height="500px" SelectionMode="Multiple" 
                                ondatabound="ltbFunction_DataBound" ForeColor="#003366"></asp:ListBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
        <hr />
        <asp:Label ID="lbl_Message" runat="server" CssClass="style3"></asp:Label>
        <br />
    </div>
    </form>
</body>
</html>
