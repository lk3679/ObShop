<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuthProFunction.aspx.cs" Inherits="OBShopWeb.AuthProFunction" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>WMS權限管理PRO-權限新增/修改</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <div>
        <span class="style1">WMS權限管理PRO-權限新增/修改</span>
        &nbsp;&nbsp;&nbsp;
        <asp:Label ID="lbl_FunctionCount" runat="server" CssClass="style2" Visible="False">20</asp:Label>
        <br /><br />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
            <asp:Panel ID="P_Add_Edit" runat="server" Visible="False">
            <table cellpadding="8" cellspacing="2" class="style2" bgcolor="#CCCCCC" 
                style="border-style: dotted; border-color: #999999; table-layout: auto;" width="1200px">
            <tr>
                <td style="border: thin solid #FFFFFF; text-align: center; background-color: #0066FF; color: #FFFFFF;" >
                    <span class="style2">ID：</span>
                </td>
                <td style="text-align: center; border: thin solid #FFFFFF;">
                    <asp:Label ID="lbl_FunctionId" runat="server" ForeColor="#006666"></asp:Label>
                    <asp:HiddenField ID="HF_Guid" runat="server" />
                </td>
                <td style="border: thin solid #FFFFFF; text-align: center; background-color: #0066FF; color: #FFFFFF;">
                    <span class="style2">有效：</span>
                </td>
                <td style="text-align: center; border: thin solid #FFFFFF;">
                    <asp:CheckBox ID="CB_Active" runat="server" Checked="True" />
                </td>
                <td style="border: thin solid #FFFFFF; text-align: center; background-color: #0066FF; color: #FFFFFF;" >
                    <span class="style2">類型：</span>
                </td>
                <td style="text-align: center; border: thin solid #FFFFFF;">
                    <asp:RadioButtonList ID="RB_Type" runat="server" RepeatDirection="Horizontal" 
                        onselectedindexchanged="RB_Type_SelectedIndexChanged" AutoPostBack="True" >
                        <asp:ListItem Value="0">群組類別</asp:ListItem>
                        <asp:ListItem Value="1" Selected="True">權限</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td style="border: thin solid #FFFFFF; text-align: center; background-color: #0066FF; color: #FFFFFF;" >
                    <span class="style2">倉庫：</span>
                </td>
                <td style="text-align: center; border: thin solid #FFFFFF;">
                    <asp:DropDownList ID="DDL_Area" runat="server" CssClass="style2" 
                        ForeColor="#0066FF" AutoPostBack="True" onselectedindexchanged="DDL_Area_SelectedIndexChanged">
                        <%--<asp:ListItem Value="-1">未選擇</asp:ListItem>--%>
                        <asp:ListItem Value="3">門市</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="border: thin solid #FFFFFF; text-align: center; background-color: #0066FF; color: #FFFFFF;" >
                    <span class="style2">群組ID：</span>
                </td>
                <td style="text-align: center; border: thin solid #FFFFFF;" colspan="1">
                    <asp:DropDownList ID="DDL_GroupId" runat="server" CssClass="style2" ForeColor="#0066FF">
                    </asp:DropDownList>
                </td>
                <td style="border: thin solid #FFFFFF; text-align: center; background-color: #0066FF; color: #FFFFFF;" >
                    <span class="style2">英文名稱：</span>
                </td>
                <td style="text-align: center; border: thin solid #FFFFFF;" colspan="1">
                    <asp:TextBox ID="txt_Title" runat="server" CssClass="style2" Width="150px" ForeColor="#0066FF"></asp:TextBox>
                </td>
                <td style="border: thin solid #FFFFFF; text-align: center; background-color: #0066FF; color: #FFFFFF;" >
                    <span class="style2">中文名稱：</span>
                </td>
                <td style="text-align: center; border: thin solid #FFFFFF;" colspan="1">
                    <asp:TextBox ID="txt_Memo" runat="server" CssClass="style2" Width="150px" ForeColor="#0066FF"></asp:TextBox>
                </td>
                <td style="border: thin solid #FFFFFF; text-align: center; background-color: #0066FF; color: #FFFFFF;" >
                    <span class="style2">備註：</span>
                </td>
                <td style="border: thin solid #FFFFFF;">
                    <asp:TextBox ID="txt_Comment" runat="server" CssClass="style2" Width="200px" ForeColor="#0066FF"></asp:TextBox>
                </td>
            </tr>
            </table>
            <asp:Button ID="btn_AddSubmit" runat="server" CssClass="style2" onclick="btn_AddSubmit_Click" Text="新增" Visible="False" />
            &nbsp;
            <asp:Button ID="btn_EditSubmit" runat="server" CssClass="style2" onclick="btn_EditSubmit_Click" Text="修改" Visible="False" />
            <asp:Label ID="lbl_Message" runat="server" CssClass="style3"></asp:Label>
            <hr />
            </asp:Panel>
            <asp:Panel ID="P_Search" runat="server" Visible="False">
                
                <span class="style2">倉庫：</span>
                <asp:DropDownList ID="DDL_Area_Search" runat="server" CssClass="style2" 
                    ForeColor="#0066FF" AutoPostBack="True" onselectedindexchanged="DDL_Area_Search_SelectedIndexChanged">
                    <asp:ListItem Value="3" Selected="True">門市</asp:ListItem>
                </asp:DropDownList>
                &nbsp;&nbsp;&nbsp;
                <span class="style2">群組：</span>
                <asp:DropDownList ID="DDL_GroupId_Search" runat="server" CssClass="style2" ForeColor="#0066FF">
                </asp:DropDownList>
                &nbsp;&nbsp;&nbsp;
                <span class="style2">種類：</span>
                <asp:DropDownList ID="DDL_Type_Search" runat="server" CssClass="style2" ForeColor="#0066FF">
                    <asp:ListItem Value="-1">全部</asp:ListItem>
                    <asp:ListItem Value="0">群組類別</asp:ListItem>
                    <asp:ListItem Value="1">權限</asp:ListItem>
                </asp:DropDownList>
                &nbsp;&nbsp;
                <asp:Button ID="btn_Search" runat="server" CssClass="style2" onclick="btn_Search_Click" Text="查詢"/>
                <fieldset class="style2">
                <legend class="style1" style="color: #006666; border-color: #003366">權限清單</legend>
                        <%--<span class="style1">使用者名單</span>--%>
                        <%--<asp:Button ID="btn_Add" runat="server" CssClass="style2" Text="新增使用者" OnClick="btn_Add_Click" ForeColor="#990000" />--%>
                        <asp:HyperLink ID="HL_ADD" runat="server" CssClass="style2" Text="※新增權限" 
                            NavigateUrl="AuthProFunction.aspx?act=add" Target="_blank" ForeColor="#0066CC"></asp:HyperLink>
                        &nbsp;&nbsp;
                        <asp:Label ID="lbl_Count" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>
                        <asp:GridView ID="gv_List" runat="server" CellPadding="2" CellSpacing="2" CssClass="style4gv"
                            ForeColor="#333333" GridLines="None" Width="1400px" AutoGenerateColumns="False">
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                            <asp:BoundField HeaderText="序號" DataField="序號" />
                            <asp:BoundField HeaderText="Id" DataField="Id" />
                            <asp:BoundField HeaderText="類型" DataField="類型" />
                            <asp:BoundField HeaderText="倉庫" DataField="倉庫" />
                            <asp:BoundField HeaderText="群組ID" DataField="群組ID" />
                            <asp:BoundField HeaderText="英文名稱" DataField="英文名稱" />
                            <asp:BoundField HeaderText="中文名稱" DataField="中文名稱" />
                            <asp:BoundField HeaderText="備註" DataField="備註" />
                            <asp:TemplateField HeaderText="有效">
                                <ItemTemplate>
                                    <asp:CheckBox ID="CB_有效" runat="server" Checked='<%#Eval("有效")%>' Enabled="False" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--<asp:BoundField HeaderText="建立時間" DataField="建立時間" />
                            <asp:BoundField HeaderText="更新時間" DataField="更新時間" />--%>
                            <asp:TemplateField HeaderText="功能">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HL_Edit" runat="server" Target="_blank" Text="修改" 
                                        NavigateUrl='<%# string.Format("AuthProFunction.aspx?act=edit&FId={0}", Eval("Id")) %>'></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            </Columns>

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
                
                </fieldset>
            </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
        <hr />
        <br />
    </div>
    </form>
</body>
</html>
