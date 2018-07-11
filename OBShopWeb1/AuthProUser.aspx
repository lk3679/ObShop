<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuthProUser.aspx.cs" Inherits="OBShopWeb.AuthProUser" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>WMS權限管理PRO-使用者新增/修改</title>
    <link rel="stylesheet" type="text/css" href="layout.css" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <div>
        <span class="style1">WMS權限管理PRO-使用者新增/修改</span>
        &nbsp;&nbsp;&nbsp;
        <asp:Label ID="lbl_FunctionCount" runat="server" CssClass="style2" Visible="False">20</asp:Label>
        <br /><br />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>

            <table cellpadding="8" cellspacing="2" class="style2" bgcolor="#CCCCCC" 
                style="border-style: dotted; border-color: #999999; table-layout: auto;" 
                    width="1200px">
            <tr>
                <td style="border: thin solid #FFFFFF; text-align: center; background-color: #0066FF; color: #FFFFFF;" >
                    <span class="style2">ID：</span>
                </td>
                <td style="text-align: center; border: thin solid #FFFFFF;">
                    <asp:Label ID="lbl_UserId" runat="server" ForeColor="#006666"></asp:Label>
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
                        onselectedindexchanged="RB_Type_SelectedIndexChanged" AutoPostBack="True">
                        <asp:ListItem Value="0">條碼</asp:ListItem>
                        <asp:ListItem Value="1" Selected="True">AD帳號</asp:ListItem>
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
                    <span class="style2">帳號：</span>
                </td>
                <td style="text-align: center; border: thin solid #FFFFFF;" colspan="1">
                    <asp:TextBox ID="txt_Account" runat="server" CssClass="style2" Width="200px" 
                        AutoPostBack="True" ForeColor="#0066FF" ontextchanged="txt_Account_TextChanged"></asp:TextBox>
                </td>
                <td style="border: thin solid #FFFFFF; text-align: center; background-color: #0066FF; color: #FFFFFF;">
                    <span class="style2">條碼：</span>
                </td>
                <td style="text-align: center; border: thin solid #FFFFFF;" colspan="3">
                    <asp:TextBox ID="txt_Barcode" runat="server" CssClass="style2" Width="180px" ForeColor="#0066FF" ReadOnly="True"></asp:TextBox>
                    &nbsp;
                    <asp:DropDownList ID="DDL_Barcode" runat="server" CssClass="style2" 
                        AutoPostBack="True" Width="200px" ForeColor="#0066FF" 
                        onselectedindexchanged="DDL_Barcode_SelectedIndexChanged">
                        <asp:ListItem Value="-1">未選擇</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td style="border: thin solid #FFFFFF; text-align: center; background-color: #0066FF; color: #FFFFFF;" >
                    <span class="style2">姓名：</span>
                </td>
                <td style="text-align: center; border: thin solid #FFFFFF;" >
                    <asp:Label ID="lbl_Name" runat="server" ForeColor="#006666"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="border: thin solid #FFFFFF; text-align: center; background-color: #0066FF; color: #FFFFFF;" >
                    <span class="style2">權限：</span>
                    <br />
                    <br />
                    <asp:Button ID="btn_CopyFunction" runat="server" Text="複製" CssClass="style2" 
                        Enabled="True" onclick="btn_CopyFunction_Click"/>
                </td>
                <td style="border: thin solid #FFFFFF;" colspan="7" bgcolor="White">
                    <asp:Panel ID="P_SearchFunction" runat="server">
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
                        <asp:Panel ID="P_G4" runat="server" Visible="False" >
                        <asp:Label ID="lbl_G4" runat="server" Text="G4" CssClass="style1"></asp:Label>
                        <br />
                        <table>
                        <tr>
                            <td>
                            <asp:CheckBox ID="CB_All_G4" runat="server" CssClass="style2" AutoPostBack="True" 
                                    oncheckedchanged="CB_All_CheckedChanged" Text="全選" ForeColor="#003399" 
                                    ToolTip="2" Checked="True"/>
                            </td>
                            <td>
                            <asp:CheckBoxList ID="CBList_G4" runat="server" CssClass="style2" 
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
                </td>
            </tr>
            <tr>
                <td style="border: thin solid #FFFFFF; text-align: center; background-color: #0066FF; color: #FFFFFF;">
                    <span class="style2">快選：</span>
                </td>
                <td style="text-align: left; border: thin solid #FFFFFF;" colspan="7">
                    <asp:CheckBox ID="CB_StrFunctionList" runat="server" CssClass="style2" 
                        Text="使用快速設定" oncheckedchanged="CB_StrFunctionList_CheckedChanged" AutoPostBack="True"/>
                    <asp:Panel ID="P_StrFunctionList" runat="server" Visible="False">
                        <asp:RadioButtonList ID="RB_StrFunctionList" runat="server" CssClass="style2" 
                            ForeColor="#006666" RepeatDirection="Horizontal" AutoPostBack="True"
                            onselectedindexchanged="RB_StrFunctionList_SelectedIndexChanged">
                            <asp:ListItem Value="0">同已設定帳號</asp:ListItem>
                            <asp:ListItem Value="1">自填</asp:ListItem>
                        </asp:RadioButtonList>
                        <asp:DropDownList ID="DDL_StrFunctionList" runat="server" CssClass="style2" 
                            Width="350px" ForeColor="#0066FF" Visible="False"></asp:DropDownList>
                        <asp:TextBox ID="txt_StrFunctionList" runat="server" CssClass="style2" 
                            Width="600px" ForeColor="#0066FF" Visible="False">F</asp:TextBox>
                        <br />
                    </asp:Panel>
                    <asp:TextBox ID="txt_CopyFunction" runat="server" Visible="False" CssClass="style2" Width="600px" ForeColor="#0066FF"></asp:TextBox>
                </td>
            </tr>
            </table>
            <asp:Button ID="btn_AddSubmit" runat="server" CssClass="style2" onclick="btn_AddSubmit_Click" Text="新增" Visible="False" />
            &nbsp;
            <asp:Button ID="btn_EditSubmit" runat="server" CssClass="style2" onclick="btn_EditSubmit_Click" Text="修改" Visible="False" />
            <asp:Label ID="lbl_Message" runat="server" CssClass="style3"></asp:Label>
            <hr />
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
        <hr />
        
        <br />
    </div>
    </form>
</body>
</html>
