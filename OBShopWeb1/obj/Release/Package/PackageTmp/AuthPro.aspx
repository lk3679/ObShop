<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuthPro.aspx.cs" Inherits="OBShopWeb.AuthPro" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>WMS權限管理PRO</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <div>
        <span class="style1">WMS權限管理PRO</span>
        &nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HL_AuthProFunction" runat="server" CssClass="style2" Text="※權限管理頁面" 
              NavigateUrl="AuthProFunction.aspx" Target="_blank" ForeColor="#FF3300"></asp:HyperLink>
        &nbsp;&nbsp;&nbsp;
        <asp:Label ID="lbl_FunctionCount" runat="server" CssClass="style2" Visible="False">20</asp:Label>
        <br /><br />
        <span class="style2">帳號/條碼/名稱：</span>
        <asp:TextBox ID="txt_Search" runat="server" CssClass="style2" AutoCompleteType="Disabled" Width="200px"></asp:TextBox>
        &nbsp;
        &nbsp;
        <span class="style2">倉庫：</span>
        <asp:DropDownList ID="DDL_Area" runat="server" CssClass="style2" 
            ForeColor="#006600" AutoPostBack="True" 
            onselectedindexchanged="DDL_Area_SelectedIndexChanged">
            <%--<asp:ListItem Value="-1">全部</asp:ListItem>--%>
            <asp:ListItem Value="3" Selected="True">門市</asp:ListItem>
        </asp:DropDownList>
        &nbsp;
        &nbsp;
        <span class="style2">類別：</span>
        <asp:DropDownList ID="DDL_AccountType" runat="server" CssClass="style2" ForeColor="#006600">
            <asp:ListItem Value="-1">全部</asp:ListItem>
            <asp:ListItem Value="0">物流條碼</asp:ListItem>
            <asp:ListItem Value="1">AD帳號</asp:ListItem>
        </asp:DropDownList>
        &nbsp;
        <asp:Button ID="btn_Search" runat="server" CssClass="style2" onclick="btn_Search_Click" Text="查詢" />
        &nbsp;
        <asp:Label ID="lbl_Message" runat="server" CssClass="style3"></asp:Label>
        <hr />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
            <asp:CheckBox ID="CB_ShowFunctionPanel" runat="server" CssClass="style2" 
                    AutoPostBack="True" Checked="False" Text="顯示權限列表" ForeColor="#0033CC" 
                    oncheckedchanged="CB_ShowFunctionPanel_CheckedChanged" />
            <fieldset class="style2">
            <legend class="style1" style="color: #006666; border-color: #003366">權限列表</legend>
                <asp:Panel ID="P_SearchFunction" runat="server" Visible="False" >
                    <div id="div_SearchFunction" runat="server">
                    <asp:Panel ID="P_G0" runat="server" Visible="False" >
                        <asp:Label ID="lbl_G0" runat="server" Text="G0" CssClass="style1"></asp:Label>
                        <br />
                        <table>
                        <tr>
                            <td>
                            <asp:CheckBox ID="CB_All_G0" runat="server" CssClass="style2" AutoPostBack="True" 
                                    oncheckedchanged="CB_All_CheckedChanged" Text="全選" ForeColor="#003399" 
                                    ToolTip="1" Checked="True"/>
                            </td>
                            <td>
                            <asp:CheckBoxList ID="CBList_G0" runat="server" CssClass="style2" 
                                    RepeatDirection="Horizontal" AutoPostBack="True" 
                                    onselectedindexchanged="CBList_SelectedIndexChanged" >
                            </asp:CheckBoxList>
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="P_G1" runat="server" Visible="False" >
                        <asp:Label ID="lbl_G1" runat="server" Text="G1" CssClass="style1"></asp:Label>
                        <br />
                        <table>
                        <tr>
                            <td>
                            <asp:CheckBox ID="CB_All_G1" runat="server" CssClass="style2" AutoPostBack="True" 
                                    oncheckedchanged="CB_All_CheckedChanged" Text="全選" ForeColor="#003399" 
                                    ToolTip="1" Checked="True"/>
                            </td>
                            <td>
                            <asp:CheckBoxList ID="CBList_G1" runat="server" CssClass="style2" 
                                    RepeatDirection="Horizontal" AutoPostBack="True" 
                                    onselectedindexchanged="CBList_SelectedIndexChanged" >
                            </asp:CheckBoxList>
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="P_G2" runat="server" Visible="False" >
                        <asp:Label ID="lbl_G2" runat="server" Text="G2" CssClass="style1"></asp:Label>
                        <br />
                        <table>
                        <tr>
                            <td>
                            <asp:CheckBox ID="CB_All_G2" runat="server" CssClass="style2" AutoPostBack="True" 
                                    oncheckedchanged="CB_All_CheckedChanged" Text="全選" ForeColor="#003399" 
                                    ToolTip="2" Checked="True"/>
                            </td>
                            <td>
                            <asp:CheckBoxList ID="CBList_G2" runat="server" CssClass="style2" 
                                    RepeatDirection="Horizontal" AutoPostBack="True" 
                                    onselectedindexchanged="CBList_SelectedIndexChanged" >
                            </asp:CheckBoxList>
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="P_G3" runat="server" Visible="False" >
                        <asp:Label ID="lbl_G3" runat="server" Text="G3" CssClass="style1"></asp:Label>
                        <br />
                        <table>
                        <tr>
                            <td>
                            <asp:CheckBox ID="CB_All_G3" runat="server" CssClass="style2" AutoPostBack="True" 
                                    oncheckedchanged="CB_All_CheckedChanged" Text="全選" ForeColor="#003399" 
                                    ToolTip="2" Checked="True"/>
                            </td>
                            <td>
                            <asp:CheckBoxList ID="CBList_G3" runat="server" CssClass="style2" 
                                    RepeatDirection="Horizontal" AutoPostBack="True" 
                                    onselectedindexchanged="CBList_SelectedIndexChanged" >
                            </asp:CheckBoxList>
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="P_G5" runat="server" Visible="False" >
                        <asp:Label ID="lbl_G5" runat="server" Text="G5" CssClass="style1"></asp:Label>
                        <br />
                        <table>
                        <tr>
                            <td>
                            <asp:CheckBox ID="CB_All_G5" runat="server" CssClass="style2" AutoPostBack="True" 
                                    oncheckedchanged="CB_All_CheckedChanged" Text="全選" ForeColor="#003399" 
                                    ToolTip="2" Checked="True"/>
                            </td>
                            <td>
                            <asp:CheckBoxList ID="CBList_G5" runat="server" CssClass="style2" 
                                    RepeatDirection="Horizontal" AutoPostBack="True" 
                                    onselectedindexchanged="CBList_SelectedIndexChanged" >
                            </asp:CheckBoxList>
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="P_G6" runat="server" Visible="False" >
                    <asp:Label ID="lbl_G6" runat="server" Text="G6" CssClass="style1"></asp:Label>
                    <br />
                    <table>
                    <tr>
                        <td>
                        <asp:CheckBox ID="CB_All_G6" runat="server" CssClass="style2" AutoPostBack="True" 
                                oncheckedchanged="CB_All_CheckedChanged" Text="全選" ForeColor="#003399" 
                                ToolTip="2" Checked="True"/>
                        </td>
                        <td>
                        <asp:CheckBoxList ID="CBList_G6" runat="server" CssClass="style2" 
                                RepeatDirection="Horizontal" AutoPostBack="True" 
                                onselectedindexchanged="CBList_SelectedIndexChanged" >
                        </asp:CheckBoxList>
                        </td>
                    </tr>
                    </table>
                    </asp:Panel>
                    <asp:Panel ID="P_G7" runat="server" Visible="False" >
                    <asp:Label ID="lbl_G7" runat="server" Text="G7" CssClass="style1"></asp:Label>
                    <br />
                    <table>
                    <tr>
                        <td>
                        <asp:CheckBox ID="CB_All_G7" runat="server" CssClass="style2" AutoPostBack="True" 
                                oncheckedchanged="CB_All_CheckedChanged" Text="全選" ForeColor="#003399" 
                                ToolTip="2" Checked="True"/>
                        </td>
                        <td>
                        <asp:CheckBoxList ID="CBList_G7" runat="server" CssClass="style2" 
                                RepeatDirection="Horizontal" AutoPostBack="True" 
                                onselectedindexchanged="CBList_SelectedIndexChanged" >
                        </asp:CheckBoxList>
                        </td>
                    </tr>
                    </table>
                    </asp:Panel>
                    <asp:Panel ID="P_G8" runat="server" Visible="False" >
                    <asp:Label ID="lbl_G8" runat="server" Text="G8" CssClass="style1"></asp:Label>
                    <br />
                    <table>
                    <tr>
                        <td>
                        <asp:CheckBox ID="CB_All_G8" runat="server" CssClass="style2" AutoPostBack="True" 
                                oncheckedchanged="CB_All_CheckedChanged" Text="全選" ForeColor="#003399" 
                                ToolTip="2" Checked="True"/>
                        </td>
                        <td>
                        <asp:CheckBoxList ID="CBList_G8" runat="server" CssClass="style2" 
                                RepeatDirection="Horizontal" AutoPostBack="True" 
                                onselectedindexchanged="CBList_SelectedIndexChanged" >
                        </asp:CheckBoxList>
                        </td>
                    </tr>
                    </table>
                    </asp:Panel>
                    <asp:Panel ID="P_G9" runat="server" Visible="False" >
                    <asp:Label ID="lbl_G9" runat="server" Text="G9" CssClass="style1"></asp:Label>
                    <br />
                    <table>
                    <tr>
                        <td>
                        <asp:CheckBox ID="CB_All_G9" runat="server" CssClass="style2" AutoPostBack="True" 
                                oncheckedchanged="CB_All_CheckedChanged" Text="全選" ForeColor="#003399" 
                                ToolTip="2" Checked="True"/>
                        </td>
                        <td>
                        <asp:CheckBoxList ID="CBList_G9" runat="server" CssClass="style2" 
                                RepeatDirection="Horizontal" AutoPostBack="True" 
                                onselectedindexchanged="CBList_SelectedIndexChanged" >
                        </asp:CheckBoxList>
                        </td>
                    </tr>
                    </table>
                    </asp:Panel>
                    <asp:Panel ID="P_G10" runat="server" Visible="False" >
                    <asp:Label ID="lbl_G10" runat="server" Text="G10" CssClass="style1"></asp:Label>
                    <br />
                    <table>
                    <tr>
                        <td>
                        <asp:CheckBox ID="CB_All_G10" runat="server" CssClass="style2" AutoPostBack="True" 
                                oncheckedchanged="CB_All_CheckedChanged" Text="全選" ForeColor="#003399" 
                                ToolTip="2" Checked="True"/>
                        </td>
                        <td>
                        <asp:CheckBoxList ID="CBList_G10" runat="server" CssClass="style2" 
                                RepeatDirection="Horizontal" AutoPostBack="True" 
                                onselectedindexchanged="CBList_SelectedIndexChanged" >
                        </asp:CheckBoxList>
                        </td>
                    </tr>
                    </table>
                    </asp:Panel>
                    </div>
                </asp:Panel>
            </fieldset>
            
            <br />
            <hr />
            <fieldset class="style2">
            <legend class="style1" style="color: #006666; border-color: #003366">使用者名單</legend>
                <asp:Panel ID="P_UserList" runat="server">
                    <div id="div_UserList" runat="server">
                        <%--<span class="style1">使用者名單</span>--%>
                        <%--<asp:Button ID="btn_Add" runat="server" CssClass="style2" Text="新增使用者" OnClick="btn_Add_Click" ForeColor="#990000" />--%>
                        <asp:HyperLink ID="HL_ADD" runat="server" CssClass="style2" Text="※新增使用者" 
                            NavigateUrl="AuthProUser.aspx?act=add" Target="_blank" ForeColor="#0066CC"></asp:HyperLink>
                        &nbsp;&nbsp;
                        <asp:Label ID="lbl_Count_UserList" runat="server" CssClass="style3" ForeColor="#6600CC"></asp:Label>
                        <asp:GridView ID="gv_UserList" runat="server" CellPadding="2" CellSpacing="2" CssClass="style4gv"
                            ForeColor="#333333" GridLines="None" Width="1400px" AutoGenerateColumns="False">
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                            <asp:BoundField HeaderText="序號" DataField="序號" />
                            <asp:BoundField HeaderText="Id" DataField="Id" />
                            <asp:BoundField HeaderText="類別" DataField="類別" />
                            <asp:BoundField HeaderText="倉庫" DataField="倉庫" />
                            <asp:BoundField HeaderText="帳號" DataField="帳號" />
                            <asp:BoundField HeaderText="條碼" DataField="條碼" />
                            <asp:BoundField HeaderText="姓名" DataField="姓名" />
                            <asp:TemplateField HeaderText="有效">
                                <ItemTemplate>
                                    <asp:CheckBox ID="CB_有效" runat="server" Checked='<%#Eval("有效")%>' Enabled="False" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="權限" DataField="權限" />
                            <asp:BoundField HeaderText="建立時間" DataField="建立時間" />
                            <asp:BoundField HeaderText="更新時間" DataField="更新時間" />
                            <asp:TemplateField HeaderText="功能">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HL_Edit" runat="server" Target="_blank" Text="修改" 
                                        NavigateUrl='<%# string.Format("AuthProUser.aspx?act=edit&userId={0}", Eval("Id")) %>'></asp:HyperLink>
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
                        
                    </div>
                </asp:Panel>
            </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
        <hr />
        
        <br />
    </div>
    </form>
</body>
</html>
