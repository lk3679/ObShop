<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateStorageNew.aspx.cs" Inherits="OBShopWeb.PDA.CreateStorageNew" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>新增儲位-新儲位格式</title>
    <link rel="stylesheet" type="text/css" href="../layout.css" />
	<script type="text/javascript" src="../js/jquery.js"></script>
	<script type="text/javascript" src="../js/json2.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <span class="style1"><strong>新增儲位-新儲位格式</strong></span>
        <br />
        <br />
        <span class="style2"><strong>起始儲位名稱：
		<asp:TextBox ID="txt_StartStorage" runat="server" CssClass="style2" MaxLength="6" Visible="False"></asp:TextBox><br />
        <asp:DropDownList ID="DDL_Area" runat="server" CssClass="style2" ForeColor="#0066FF" AutoPostBack="True">
            <asp:ListItem>A</asp:ListItem>
            <asp:ListItem>B</asp:ListItem>
            <asp:ListItem>C</asp:ListItem>
            <asp:ListItem>D</asp:ListItem>
            <asp:ListItem>E</asp:ListItem>
            <asp:ListItem>F</asp:ListItem>
            <asp:ListItem>G</asp:ListItem>
            <asp:ListItem>H</asp:ListItem>
            <asp:ListItem>I</asp:ListItem>
            <asp:ListItem>J</asp:ListItem>
            <asp:ListItem>K</asp:ListItem>
            <asp:ListItem>L</asp:ListItem>
            <asp:ListItem>M</asp:ListItem>
            <asp:ListItem>N</asp:ListItem>
            <asp:ListItem>O</asp:ListItem>
            <asp:ListItem>P</asp:ListItem>
            <asp:ListItem>Q</asp:ListItem>
            <asp:ListItem>R</asp:ListItem>
            <asp:ListItem>S</asp:ListItem>
            <asp:ListItem>T</asp:ListItem>
            <asp:ListItem>U</asp:ListItem>
            <asp:ListItem>V</asp:ListItem>
            <asp:ListItem>W</asp:ListItem>
            <asp:ListItem>X</asp:ListItem>
            <asp:ListItem>Y</asp:ListItem>
            <asp:ListItem>Z</asp:ListItem>
        </asp:DropDownList>
        &nbsp;
        <asp:DropDownList ID="DDL_D1" runat="server" CssClass="style2" AutoPostBack="True">
        </asp:DropDownList>
        &nbsp;-
        <asp:DropDownList ID="DDL_D2" runat="server" CssClass="style2" AutoPostBack="True">
        </asp:DropDownList>
        &nbsp;-
        <asp:DropDownList ID="DDL_D3" runat="server" CssClass="style2" AutoPostBack="True">
        </asp:DropDownList>
        &nbsp;-
        <asp:DropDownList ID="DDL_D4" runat="server" CssClass="style2" AutoPostBack="True">
        </asp:DropDownList>
                        
        </strong></span>
        &nbsp;
        <span class="style2"><strong>單層數量：
		<asp:TextBox ID="txt_Number" runat="server" CssClass="style2" Width="50px" MaxLength="5" ForeColor="#006600">4</asp:TextBox>
        &nbsp;
        儲位類型：
        <asp:DropDownList ID="ddl_Type" runat="server" CssClass="style2" ForeColor="#006600">
            <asp:ListItem Text="普通" Value="0"></asp:ListItem>
            <asp:ListItem Text="散貨" Value="1"></asp:ListItem>
            <asp:ListItem Text="補貨" Value="2"></asp:ListItem>
            <%--<asp:ListItem Text="過季" Value="3"></asp:ListItem>--%>
            <asp:ListItem Text="問題" Value="4"></asp:ListItem>
            <asp:ListItem Text="不良" Value="5"></asp:ListItem>
            <asp:ListItem Text="標準暫存" Value="6"></asp:ListItem>
            <asp:ListItem Text="不良暫存" Value="7"></asp:ListItem>
<%--            <asp:ListItem Text="問題暫存" Value="8"></asp:ListItem>
            <asp:ListItem Text="海運暫存" Value="14"></asp:ListItem>
            <asp:ListItem Text="換貨暫存" Value="15"></asp:ListItem>
            <asp:ListItem Text="散貨暫存" Value="16"></asp:ListItem>
            <asp:ListItem Text="調回暫存" Value="17"></asp:ListItem>--%>
            <asp:ListItem Text="寄倉" Value="21"></asp:ListItem>
        </asp:DropDownList>
        &nbsp;
        儲位容量：
		<asp:TextBox ID="txt_Volume" runat="server" CssClass="style2" Width="80px" MaxLength="8" ForeColor="#006600"></asp:TextBox>
        &nbsp;
        撿貨群組：
        <asp:DropDownList ID="DDL_Group1" runat="server" CssClass="style2" ForeColor="#0066FF" AutoPostBack="True">
            <asp:ListItem>A</asp:ListItem>
            <asp:ListItem>B</asp:ListItem>
            <asp:ListItem>C</asp:ListItem>
            <asp:ListItem>D</asp:ListItem>
            <asp:ListItem>E</asp:ListItem>
            <asp:ListItem>F</asp:ListItem>
            <asp:ListItem>G</asp:ListItem>
            <asp:ListItem>H</asp:ListItem>
            <asp:ListItem>I</asp:ListItem>
            <asp:ListItem>J</asp:ListItem>
            <asp:ListItem>K</asp:ListItem>
            <asp:ListItem>L</asp:ListItem>
            <asp:ListItem>M</asp:ListItem>
            <asp:ListItem>N</asp:ListItem>
            <asp:ListItem>O</asp:ListItem>
            <asp:ListItem>P</asp:ListItem>
            <asp:ListItem>Q</asp:ListItem>
            <asp:ListItem>R</asp:ListItem>
            <asp:ListItem>S</asp:ListItem>
            <asp:ListItem>T</asp:ListItem>
            <asp:ListItem>U</asp:ListItem>
            <asp:ListItem>V</asp:ListItem>
            <asp:ListItem>W</asp:ListItem>
            <asp:ListItem>X</asp:ListItem>
            <asp:ListItem>Y</asp:ListItem>
            <asp:ListItem>Z</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="DDL_Group2" runat="server" CssClass="style2" ForeColor="#0066FF" AutoPostBack="True">
            <asp:ListItem>1</asp:ListItem>
            <asp:ListItem>2</asp:ListItem>
            <asp:ListItem>3</asp:ListItem>
            <asp:ListItem>4</asp:ListItem>
            <asp:ListItem>5</asp:ListItem>
            <asp:ListItem>6</asp:ListItem>
            <asp:ListItem>7</asp:ListItem>
            <asp:ListItem>8</asp:ListItem>
            <asp:ListItem>9</asp:ListItem>
        </asp:DropDownList>
		<asp:TextBox ID="txtGroupName" runat="server" CssClass="style2" Width="30px" 
            MaxLength="2" ForeColor="#006600" Visible="False"></asp:TextBox>
        &nbsp;
        <asp:Button ID="btn_Execution" runat="server" Text="產生" CssClass="style2" onclick="btn_Execution_Click" />
        &nbsp;
        <asp:CheckBox ID="CB_A" runat="server" CssClass="style2" Text="產生到A" Visible="False"/>
        <asp:CheckBox ID="CB_OneFormat" runat="server" CssClass="style2" Text="規則產生"/>
        <br />
        【
        <asp:Label ID="lbl_ShlefId" runat="server" CssClass="style2" 
            ForeColor="#993399"></asp:Label>
        &nbsp;】
        </strong></span>
        <hr />
        <asp:Label ID="lbl_Message" runat="server" Text="" CssClass="style3"></asp:Label>
        <asp:GridView ID="gv_List" runat="server" CellPadding="2" CellSpacing="2" CssClass="style4gv"
                          ForeColor="#333333" GridLines="None" 
                          OnRowDataBound="gv_List_RowDataBound" Width="100%" 
                          AutoGenerateColumns="False">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:TemplateField HeaderText="儲位名稱">
                        <ItemTemplate>
                            <asp:TextBox ID="txt_StorageId" runat="server" Text='<%# Bind("儲位名稱")%>' MaxLength="12"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txt_StorageId" 
                            ErrorMessage="格式錯誤！" ForeColor="Red"
                            ValidationExpression="^([A-Z]{1})([0-9]{2})-([0-9]{2})-([0-9]{2})-([0-9]{2})$"></asp:RegularExpressionValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="儲位類別">
                        <ItemTemplate>
                            <asp:DropDownList ID="ddl_StorageType" runat="server" CssClass="style2">
                                <asp:ListItem Text="普通" Value="0"></asp:ListItem>
                                <asp:ListItem Text="散貨" Value="1"></asp:ListItem>
                                <asp:ListItem Text="補貨" Value="2"></asp:ListItem>
                                <%--<asp:ListItem Text="過季" Value="3"></asp:ListItem>--%>
                                <asp:ListItem Text="問題" Value="4"></asp:ListItem>
                                <asp:ListItem Text="不良" Value="5"></asp:ListItem>
                                <asp:ListItem Text="標準暫存" Value="6"></asp:ListItem>
                                <asp:ListItem Text="不良暫存" Value="7"></asp:ListItem>
                    <%--            <asp:ListItem Text="問題暫存" Value="8"></asp:ListItem>
                                <asp:ListItem Text="海運暫存" Value="14"></asp:ListItem>
                                <asp:ListItem Text="換貨暫存" Value="15"></asp:ListItem>
                                <asp:ListItem Text="散貨暫存" Value="16"></asp:ListItem>
                                <asp:ListItem Text="調回暫存" Value="17"></asp:ListItem>--%>
                                <asp:ListItem Text="寄倉" Value="21"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="儲位容量">
                        <ItemTemplate>
                            <asp:TextBox ID="txt_StorageVolume" runat="server" Text='<%# Bind("儲位容量")%>' MaxLength="8"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txt_StorageVolume" 
                            ErrorMessage="格式錯誤！" ForeColor="Red"
                            ValidationExpression="^\d+"></asp:RegularExpressionValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="撿貨群組">
                        <ItemTemplate>
                            <asp:TextBox ID="txt_StorageGroupName" runat="server" Text='<%# Bind("撿貨群組")%>' MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txt_StorageGroupName" 
                            ErrorMessage="格式錯誤！" ForeColor="Red"
                            ValidationExpression="^[A-Z][1-9]"></asp:RegularExpressionValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White"  />
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
            <asp:Button ID="btn_Save" runat="server" Text="儲存" CssClass="style2" onclick="btn_Save_Click" />
    </div>
    </form>
</body>
</html>
