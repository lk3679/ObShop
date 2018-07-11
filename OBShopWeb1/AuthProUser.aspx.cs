using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using POS_Library.Public;
using POS_Library.DB;
using OBShopWeb.EntranceService;
using System.Net;
using OBShopWeb.AuthService;
using OBShopWeb.ADVerifyService;
using System.Collections.Specialized;
using System.Web.Configuration;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class AuthProUser : System.Web.UI.Page
    {
        #region 宣告

        EntranceClient EC = new EntranceClient();
        POS_Library.Public.WmsAuth wmsauth = new WmsAuth();
        int area, AccountType, userId;
        string account, act;

        Label lblTemp;
        CheckBox CBTemp;
        CheckBoxList CBListTemp;
        ListItem LITemp;
        Panel PLTemp;
        System.Drawing.Color tempColor = System.Drawing.Color.DarkRed;

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        setup auth = new setup();

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx?urlx=AuthPro.aspx' </script> ");
                    Response.End();
                }
                //新權限(2014-0512新增)
                else if (!auth.checkAuthorityPro("2"))
                {
                    Response.Redirect("Privilege.aspx");
                }
                else
                {
                    account = Session["Account"].ToString();
                    AccountType = int.Parse(RB_Type.SelectedValue);
                    area = _areaId;

                    lbl_Message.Text = "";

                    act = (Request["act"] != null) ? Request["act"].ToString() : "none";
                    userId = (Request["userId"] != null) ? int.Parse(Request["userId"].ToString()) : -1;

                    if (!IsPostBack)
                    {
                        switch (act)
                        {
                            case "add":
                                area = -2;
                                SetCheckBoxList();
                                RB_Type_Change();
                                Set_DDL_Barcode();
                                btn_AddSubmit.Visible = true;
                                break;
                            case "edit":
                                GetEditUser(userId);
                                break;
                            default: break;
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 取使用者資料
        /// </summary>
        /// <param name="id"></param>
        protected void GetEditUser(int id)
        {
            var temp = wmsauth.GetAllUserFunctionList(-1, -1, id).FirstOrDefault();
            if (temp != null)
            {
                var user = temp.User;
                CB_Active.Checked = user.Active;
                RB_Type.SelectedIndex = user.Type;
                DDL_Area.SelectedIndex = 0;// user.Area + 1;

                txt_Account.Text = user.Account;
                txt_Barcode.Text = user.Barcode;
                lbl_Name.Text = user.Name;
                txt_Account.Enabled = txt_Barcode.Enabled =
                DDL_Area.Enabled = RB_Type.Enabled = DDL_Barcode.Enabled = false;

                var FList = temp.FunctionList;
                var FListGroup = FList.Select(x => x.GroupId).Distinct().ToList();
                var FListId = FList.Select(x => x.功能編號.ToString()).ToList();

                //查詢權限
                SetCheckBoxList();
                //先清除所有選擇
                Set_CB_Check(false);

                foreach (var gg in FListGroup)
                {
                    var aa = div_SearchFunction.FindControl("CBList_G" + gg) as CheckBoxList;
                    if (aa != null)
                    {
                        var bb = aa.Items;

                        //確認是否勾選
                        foreach (var item in bb)
                        {
                            var cc = item as ListItem;
                            var tempstr = cc.Text.Split('.')[0];
                            cc.Selected = (FListId.Contains(tempstr)) ? true : cc.Selected;
                        }

                        //改完再看CB_All要不要打勾
                        Set_CB_All_Check(aa);
                    }
                }

                lbl_UserId.Text = userId.ToString();
                btn_EditSubmit.Visible = true;
            }
            else
            {
                lbl_Message.Text = "使用者ID錯誤!";
            }
        }

        #endregion

        #region 介面-組權限/輸入物流人員

        #region ●組權限清單

        /// <summary>
        /// 組權限清單
        /// </summary>
        protected void SetCheckBoxList()
        {
            area = _areaId;
            var templist = wmsauth.GetAllFunctionList(area, -1);

            var groupFather = templist.Where(x => x.Type == 0).ToList();
            var groupChild = templist.Where(x => x.Type == 1).ToList();

            //先全改成隱藏
            var AllFCount = int.Parse(lbl_FunctionCount.Text);
            for (int i = 0; i < AllFCount; i++)
            {
                PLTemp = form1.FindControl("P_G" + i) as Panel;
                if (PLTemp != null)
                    PLTemp.Visible = false;
            }
            //再將有的改為顯示
            if (groupFather.Count > 0)
            {
                #region ●建出權限列表

                foreach (var gp in groupFather)
                {
                    var GID = gp.GroupId;
                    var GName = gp.Memo;
                    tempColor = (tempColor == System.Drawing.Color.DarkGreen) ? System.Drawing.Color.DarkRed : System.Drawing.Color.DarkGreen;

                    PLTemp = form1.FindControl("P_G" + GID) as Panel;
                    if (PLTemp != null)
                    {
                        PLTemp.Visible = gp.Active;
                        lblTemp = div_SearchFunction.FindControl("lbl_G" + GID) as Label;
                        lblTemp.Text = GID + "." + GName;

                        //CBTemp = div_SearchFunction.FindControl("CB_All_G" + GID) as CheckBox;
                        //CBTemp.Text = GID + "全選";

                        var gptemp = groupChild.Where(x => x.GroupId == gp.GroupId).ToList();
                        CBListTemp = div_SearchFunction.FindControl("CBList_G" + GID) as CheckBoxList;
                        CBListTemp.ForeColor = tempColor;
                        CBListTemp.Items.Clear();
                        foreach (var item in gptemp)
                        {
                            LITemp = new ListItem();
                            LITemp.Selected = true;
                            LITemp.Text = item.Id + "." + item.Memo;
                            LITemp.Value = item.Id + "," + item.GroupId + "," + item.Type + "," + item.Title;

                            CBListTemp.Items.Add(LITemp);
                        }
                    }
                }

                #endregion
            }
        }

        #endregion

        #region ●查詢物流人員

        /// <summary>
        /// 查詢物流人員
        /// </summary>
        protected void GetLogistics()
        {
            Set_DDL_Barcode();

            if (DDL_Area.SelectedValue != "-1")
            {
                var zone = _areaId.ToString();
                var temp = wmsauth.LoadLogistics(zone).OrderBy(x => x.account);

                List<ListItem> list = new List<ListItem>();
                foreach (var ii in temp)
                {
                    LITemp = new ListItem();
                    LITemp.Text = ii.account;
                    LITemp.Value = ii.barcode;
                    DDL_Barcode.Items.Add(LITemp);
                }
                DDL_Barcode.Enabled = true;
            }
        }

        /// <summary>
        /// 清空或設定DDL_Barcode
        /// </summary>
        protected void Set_DDL_Barcode()
        {
            //DDL清空重新查詢
            DDL_Barcode.Items.Clear();
            LITemp = new ListItem();
            LITemp.Text = "未選擇";
            LITemp.Value = "-1";
            DDL_Barcode.Items.Add(LITemp);

            txt_Barcode.Text = lbl_Name.Text = "";
            DDL_Barcode.Enabled = false;
        }

        #endregion

        #region ※切換按鈕

        /// <summary>
        /// 切換快速設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RB_StrFunctionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt_StrFunctionList.Visible =
            DDL_StrFunctionList.Visible = false;
            
            if (RB_StrFunctionList.SelectedValue == "0")
            {
                DDL_StrFunctionList.Visible = true;
                DDL_StrFunctionList.Items.Clear();

                var AllUser = wmsauth.GetAllUserList(area, AccountType);

                if (AllUser.Count > 0)
                {
                    foreach (var user in AllUser)
                    {
                        LITemp = new ListItem();
                        LITemp.Text = user.Name + " (" + ((user.Type == 0) ? user.Barcode : user.Account) + ")";
                        LITemp.Value = user.FunctionList;
                        DDL_StrFunctionList.Items.Add(LITemp);
                    }
                }
            }
            else
            {
                txt_StrFunctionList.Visible = true;
            }
        }

        /// <summary>
        /// 切換速填
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CB_StrFunctionList_CheckedChanged(object sender, EventArgs e)
        {
            P_StrFunctionList.Visible = CB_StrFunctionList.Checked;
        }

        /// <summary>
        /// 切換類別RB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RB_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                RB_Type_Change();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// 切換類別
        /// </summary>
        protected void RB_Type_Change()
        {
            if (RB_Type.SelectedValue == "0")
            {
                txt_Barcode.Text = txt_Account.Text = lbl_Name.Text = "";
                txt_Account.Enabled = false;
                txt_Barcode.Enabled = true;
                DDL_Barcode.Enabled = true;

                GetLogistics();
            }
            else if (RB_Type.SelectedValue == "1")
            {
                txt_Barcode.Text = txt_Account.Text = lbl_Name.Text = "";
                txt_Account.Enabled = true;
                txt_Account.Focus();
                txt_Barcode.Enabled = false;
                DDL_Barcode.Enabled = false;

                Set_DDL_Barcode();
            }
        }

        /// <summary>
        /// 切換物流條碼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Barcode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (DDL_Barcode.SelectedItem.Value != "-1")
                {
                    lbl_Name.Text = DDL_Barcode.SelectedItem.Text;
                    txt_Barcode.Text = DDL_Barcode.SelectedItem.Value;
                }
                else
                {
                    lbl_Name.Text = txt_Barcode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 切換倉庫：組權限/查詢物流人員
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Area_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txt_Account.Text = lbl_Name.Text = txt_Barcode.Text = "";
                //有選擇倉庫
                if (DDL_Area.SelectedValue != "-1")
                {
                    //組權限列表
                    SetCheckBoxList();

                    //如果類型是條碼就取物流人員清單
                    if (RB_Type.SelectedValue == "0")
                    {
                        GetLogistics();
                    }
                    else
                    {
                        txt_Account.Enabled = true;
                    }
                }
                else
                {
                    //先全改成隱藏
                    var AllFCount = int.Parse(lbl_FunctionCount.Text);
                    for (int i = 0; i < AllFCount; i++)
                    {
                        PLTemp = form1.FindControl("P_G" + i) as Panel;
                        if (PLTemp != null)
                            PLTemp.Visible = false;
                    }
                    txt_Account.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 全選權限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CB_All_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var aa = sender as CheckBox;
                var bb = div_SearchFunction.FindControl("CBList_G" + aa.ID.Substring(aa.ID.Length - 1, 1)) as CheckBoxList;

                if (bb != null)
                {
                    foreach (var i in bb.Items)
                    {
                        var cc = i as ListItem;
                        cc.Selected = aa.Checked;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 全Group改成全不選/全選
        /// </summary>
        /// <param name="ischecked"></param>
        protected void Set_CB_Check(bool ischecked)
        {
            var AllFCount = int.Parse(lbl_FunctionCount.Text);
            for (int i = 0; i < AllFCount; i++)
            {
                var aa = div_SearchFunction.FindControl("CB_All_G" + i) as CheckBox;
                if (aa != null)
                {
                    aa.Checked = ischecked;
                    var bb = div_SearchFunction.FindControl("CBList_G" + i) as CheckBoxList;
                    if (bb != null)
                    {
                        foreach (var ii in bb.Items)
                        {
                            var cc = ii as ListItem;
                            cc.Selected = ischecked;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 切換選擇權限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CBList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var aa = sender as CheckBoxList;
                Set_CB_All_Check(aa);
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 判斷全選是否要打勾
        /// </summary>
        /// <param name="aa"></param>
        protected void Set_CB_All_Check(CheckBoxList aa)
        {
            var result = true;
            foreach (var i in aa.Items)
            {
                var cc = i as ListItem;
                if (!cc.Selected)
                    result = false;
            }

            var dd = div_SearchFunction.FindControl("CB_All_G" + aa.ID.Substring(aa.ID.Length - 1, 1)) as CheckBox;
            if (dd != null)                
                dd.Checked = result;
        }


        #endregion

        #endregion

        #region 介面-輸入/驗證AD帳號

        /// <summary>
        /// 輸入帳號
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txt_Account_TextChanged(object sender, EventArgs e)
        {
            try
            {
                verifyAccount();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// AD驗證
        /// </summary>
        protected void verifyAccount()
        {
            var ad_service = new ADVerifyService.ADVerifyClient();
            var addaccount = txt_Account.Text = txt_Account.Text.Trim();
            //清空重新查詢
            Set_txt_Account(false, "", "");
            var result = ad_service.Verify("OBdesign.com.tw", addaccount, null);
            if (result.VertifyState == ADVerifyService.State.NoAccount)
            {
                lbl_Message.Text = "無此AD帳號";
                txt_Account.Text = string.Empty;
                return;
            }
            else if (result.VertifyState == ADVerifyService.State.Error)
            {
                lbl_Message.Text = "Server錯誤";
                txt_Account.Text = string.Empty;
                return;
            }
            else if (result.VertifyState == ADVerifyService.State.NoPass)
            {
                var mappings = wmsauth.GetUser(addaccount, area, AccountType);
                if (mappings.Count > 0)
                {
                    lbl_Message.Text = "AD帳號已存在";
                    txt_Account.Text = string.Empty;
                    return;
                }

                ADVerifyClient ADVC = new ADVerifyClient();
                var ADI = ADVC.Verify("OBDesign.com.tw", addaccount, "");

                lbl_Message.Text = ADI.Fullname;
                lbl_Message.Text += ", 帳號正常，請選擇此帳號之權限!";
                //設定
                Set_txt_Account(true, addaccount, ADI.Fullname);
            }
        }

        /// <summary>
        /// 清空或設定txt_Account
        /// </summary>
        /// <param name="IsOK"></param>
        /// <param name="account"></param>
        /// <param name="Name"></param>
        protected void Set_txt_Account(bool IsOK, string account, string Name)
        {
            if (IsOK)
            {
                lbl_Name.Text = Name;
                txt_Account.Text = account;
            }
            else
            {
                lbl_Name.Text = txt_Barcode.Text = "";
            }
        }

        #endregion

        #region 副功能-轉值

        /// <summary>
        /// TypeToName
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="ChangeType"></param>
        /// <returns></returns>
        protected String TypeToName(int Type, int ChangeType)
        {
            if (ChangeType == 0)
            {
                switch (Type)
                {
                    case 0: return "條碼";
                    case 1: return "AD";
                    default: return "其他";
                }
            }
            else
            {
                switch (Type)
                {
                    case 0: return "龜山";
                    case 1: return "虎門";
                    case 2: return "門市";
                    case 3: return "門市旗艦";
                    case 4: return "門市市府店";
                    default: return "其他";
                }
            }

        }

        /// <summary>
        /// NameToType
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        protected int NameToType(String Name, int ChangeType)
        {
            if (ChangeType == 0)
            {
                switch (Name)
                {
                    case "條碼": return 0;
                    case "AD": return 1;
                    default: return 0;
                }
            }
            else
            {
                switch (Name)
                {
                    case "龜山": return 0;
                    case "虎門": return 1;
                    case "門市": return 2;
                    case "門市旗艦": return 3;
                    case "門市市府店": return 4;
                    default: return 0;
                }
            }
        }

        /// <summary>
        /// ListToString(權限list轉名稱)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected string ListToString(List<WmsAuth.FunctionInfo> list)
        {
            var result = "";
            foreach (var i in list)
            {
                result += i.功能編號 + "." + i.功能名稱 + ", ";
            }
            return result;
        }

        #endregion

        #region 副功能-複製權限String

        /// <summary>
        /// 複製權限String
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_CopyFunction_Click(object sender, EventArgs e)
        {
            try
            {
                var temp = GetSelectFunctionList();

                if (temp == "F")
                {
                    lbl_Message.Text = "設定資料有誤(未選權限)";
                    return;
                }
                txt_CopyFunction.Visible = true;
                txt_CopyFunction.Text = temp;
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 主功能-新增/修改

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_AddSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(lbl_Name.Text) || int.Parse(DDL_Area.SelectedValue) == -1)
                {
                    lbl_Message.Text = "設定資料有誤(未填人員/未選倉庫)";
                    return;
                }
                var Data = GetUserData("add");
                if (Data.FunctionList == "F")
                {
                    lbl_Message.Text = "設定資料有誤(未選權限)";
                    return;
                }
                var result = wmsauth.AddUserFunctionList(Data);

                lbl_Message.Text = result.Reason;

                if (result.Result == "1")
                {
                    //btn_AddSubmit.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_EditSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(lbl_Name.Text) || int.Parse(DDL_Area.SelectedValue) == -1)
                {
                    lbl_Message.Text = "設定資料有誤(未填人員/未選倉庫)";
                    return;
                }
                var Data = GetUserData("edit");
                if (Data.FunctionList == "F")
                {
                    lbl_Message.Text = "設定資料有誤(未選權限)";
                    return;
                }
                var result = wmsauth.SetUserFunctionList(Data);

                lbl_Message.Text = result.Reason;

                if (result.Result == "1")
                {
                    //btn_EditSubmit.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 串權限選擇結果
        /// </summary>
        /// <returns></returns>
        protected WMS_Auth_Mapping GetUserData(string type)
        {
            var UserId = (type == "add") ? 1 : int.Parse(lbl_UserId.Text);
            var UserType = int.Parse(RB_Type.SelectedValue);
            var UserAccount = txt_Account.Text;
            var UserBarcode = txt_Barcode.Text;
            var UserName = lbl_Name.Text;
            var UserArea = _areaId;
            var UserActive = CB_Active.Checked;
            var UserFunctionList = GetSubmitFunctionList();
            var UserUpdateDate = DateTime.Now;
            var UserCreateDate = DateTime.Now;

            var Data = new WMS_Auth_Mapping();
            Data.Id = UserId;
            Data.Type = UserType;
            Data.Account = string.IsNullOrEmpty(UserAccount) ? null : UserAccount;
            Data.Barcode = string.IsNullOrEmpty(UserBarcode) ? null : UserBarcode;
            Data.Name = UserName;
            Data.Area = UserArea;
            Data.Active = UserActive;
            Data.FunctionList = UserFunctionList;
            Data.UpdateDate = UserUpdateDate;
            Data.CreateDate = UserCreateDate;

            return Data;
        }

        /// <summary>
        /// 串權限選擇結果
        /// </summary>
        /// <returns></returns>
        protected string GetSelectFunctionList()
        {
            var result = "F";
            var AllFCount = int.Parse(lbl_FunctionCount.Text);
            for (int i = 0; i < AllFCount; i++)
            {
                var bb = div_SearchFunction.FindControl("CBList_G" + i) as CheckBoxList;

                if (bb != null)
                {
                    foreach (var ii in bb.Items)
                    {
                        var cc = ii as ListItem;
                        var tempstr = cc.Text.Split('.')[0];
                        if (cc.Selected)
                            result += "," + tempstr;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 串權限選擇結果(一般勾選/快選)
        /// </summary>
        /// <returns></returns>
        protected string GetSubmitFunctionList()
        {
            var result = "";

            if (CB_StrFunctionList.Checked)
            {
                //同已設定帳號
                if (RB_StrFunctionList.SelectedValue == "0")
                {
                    result = DDL_StrFunctionList.SelectedValue;
                }
                //自填
                else
                {
                    result = txt_StrFunctionList.Text;
                }
            }
            else
            {
                result = GetSelectFunctionList();
            }

            return result;
        }

        #endregion
    }
}