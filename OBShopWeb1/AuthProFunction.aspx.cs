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
    public partial class AuthProFunction : System.Web.UI.Page
    {
        #region 宣告

        EntranceClient EC = new EntranceClient();
        POS_Library.Public.WmsAuth wmsauth = new WmsAuth();
        int area, FunctionType, FId, GPId;
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
                    Response.Write(" <script> parent.document.location= 'logout.aspx?urlx=AuthProFunction.aspx' </script> ");
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
                    FunctionType = int.Parse(RB_Type.SelectedValue);
                    area = _areaId;

                    lbl_Message.Text = "";

                    act = (Request["act"] != null) ? Request["act"].ToString() : "none";
                    FId = (Request["FId"] != null) ? int.Parse(Request["FId"].ToString()) : -1;

                    if (!IsPostBack)
                    {
                        switch (act)
                        {
                            case "add":
                                area = -2;
                                ChangePanel(1);
                                Set_DDL_GroupId(-1);
                                btn_AddSubmit.Visible = true;
                                break;
                            case "edit":
                                ChangePanel(1);
                                GetEditUser(FId);
                                break;
                            default:
                                ChangePanel(2);
                                Set_DDL_GroupId(-1);
                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region ●取權限資料

        /// <summary>
        /// 取權限資料
        /// </summary>
        /// <param name="id"></param>
        protected void GetEditUser(int id)
        {
            var FT = wmsauth.GetFunction(id);
            if (FT != null)
            {
                HF_Guid.Value = FT.Guid;
                CB_Active.Checked = FT.Active;
                RB_Type.SelectedIndex = FT.Type;
                DDL_Area.SelectedIndex = 0;// user.Area + 1;

                txt_Title.Text = FT.Title;
                txt_Memo.Text = FT.Memo;
                txt_Comment.Text = FT.Comment;

                Set_DDL_GroupId(FT.GroupId);

                DDL_Area.Enabled = RB_Type.Enabled = false;

                lbl_FunctionId.Text = FId.ToString();
                btn_EditSubmit.Visible = true;
            }
            else
            {
                lbl_Message.Text = "權限ID錯誤!";
            }
        }

        #endregion

        #region ●讀取GroupId

        /// <summary>
        /// 設定DDL_GroupId
        /// </summary>
        /// <param name="id"></param>
        protected void Set_DDL_GroupId(int id)
        {
            area = P_Add_Edit.Visible ? _areaId : _areaId;
            var AllFT = wmsauth.GetAllFunctionList(area, -1);

            var FTGroup = AllFT.Where(x=>x.Type == 0).ToList();

            //判斷要處理哪個DDL
            var DDLGroup = P_Add_Edit.Visible ? DDL_GroupId : DDL_GroupId_Search;
            //清除
            DDLGroup.Items.Clear();

            if (area != -1 && FTGroup.Count > 0)
            {
                //加 全部 選項
                if (P_Search.Visible)
                {
                    LITemp = new ListItem();
                    LITemp.Value = "-1";
                    LITemp.Text = "全部";
                    DDLGroup.Items.Add(LITemp);
                }
                else
                {
                    if (RB_Type.SelectedValue == "0")
                        DDLGroup.Enabled = false;
                }
                int y = 1;
                //組成DDL
                foreach (var FG in FTGroup)
                {
                    LITemp = new ListItem();
                    LITemp.Value = FG.GroupId.ToString();
                    LITemp.Text = y++ + "." + "【" + FG.GroupId + "】【" + FG.Memo + "】";

                    if (id != -1 && id == FG.GroupId)
                    {
                        LITemp.Selected = true;
                    }
                    DDLGroup.Items.Add(LITemp);
                }
            }
        }

        #endregion

        #region 介面-倉庫切換/Panel控制

        /// <summary>
        /// 查詢Area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Area_Search_SelectedIndexChanged(object sender, EventArgs e)
        {
            Set_DDL_GroupId(-1);
        }

        /// <summary>
        /// 新增修改Area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Area_SelectedIndexChanged(object sender, EventArgs e)
        {
            Set_DDL_GroupId(-1);
        }

        /// <summary>
        /// 顯示/隱藏 編輯Panel
        /// </summary>
        /// <param name="choice"></param>
        protected void ChangePanel(int choice)
        {
            if (choice == 1)
            {
                P_Add_Edit.Visible = true;
                P_Search.Visible = false;
            }
            else
            {
                P_Add_Edit.Visible = false;
                P_Search.Visible = true;
            }
        }

        /// <summary>
        /// 切換類型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RB_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RB_Type.SelectedValue == "0")
                DDL_GroupId.Enabled = false;
            else
            {
                DDL_GroupId.Enabled = true;
                Set_DDL_GroupId(-1);
            }
        }

        #endregion

        #region 主功能-查詢

        /// <summary>
        /// 查詢-按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// 查詢
        /// </summary>
        protected void Search()
        {
            area = _areaId;
            FunctionType = int.Parse(DDL_Type_Search.SelectedValue);
            GPId = int.Parse(DDL_GroupId_Search.SelectedValue);

            var temp = wmsauth.GetAllFunctionList(area, FunctionType);

            var y = 1;
            var temp2 = (from i in temp
                         where (GPId == -1) ? true : i.GroupId == GPId
                         orderby i.GroupId, i.Id
                         select new
                         {
                             序號 = y++,
                             Id = i.Id,
                             有效 = i.Active,
                             類型 = TypeToName(i.Type, 0),
                             倉庫 = TypeToName(i.Area, 1),
                             群組ID = i.GroupId,
                             英文名稱 = i.Title,
                             中文名稱 = i.Memo,
                             備註 = i.Comment

                         }).ToList();

            gv_List.DataSource = temp2;
            gv_List.DataBind();

            lbl_Count.Text = "總筆數：" + temp2.Count;
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
                if (string.IsNullOrEmpty(txt_Title.Text) || string.IsNullOrEmpty(txt_Memo.Text) || int.Parse(DDL_Area.SelectedValue) == -1)
                {
                    lbl_Message.Text = "設定資料有誤(未填名稱/未選倉庫)";
                    return;
                }
                var Data = GetFunctionData("add");

                var result = wmsauth.AddFunction(Data);

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
                if (string.IsNullOrEmpty(txt_Title.Text) || string.IsNullOrEmpty(txt_Memo.Text) || int.Parse(DDL_Area.SelectedValue) == -1)
                {
                    lbl_Message.Text = "設定資料有誤(未填名稱/未選倉庫)";
                    return;
                }
                var Data = GetFunctionData("edit");

                var result = wmsauth.SetFunction(Data);

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
        protected WMS_Auth_Function GetFunctionData(string type)
        {
            var FGuid = (type == "add") ? Utility.GetGuidMD5() : HF_Guid.Value;
            var FId = (type == "add") ? 1 : int.Parse(lbl_FunctionId.Text);
            var FType = int.Parse(RB_Type.SelectedValue);
            var FGroupId = (type == "add" && RB_Type.SelectedValue == "0") ? 0 : int.Parse(DDL_GroupId.SelectedValue);
            var FTitle = txt_Title.Text;
            var FMemo = txt_Memo.Text;
            var FComment = txt_Comment.Text;
            var FArea = _areaId;
            var FActive = CB_Active.Checked;
            var FUpdateDate = DateTime.Now;
            var FCreateDate = DateTime.Now;

            var Data = new WMS_Auth_Function();
            Data.Guid = FGuid;
            Data.Id = FId;
            Data.Type = FType;
            Data.GroupId = FGroupId;
            Data.Title = FTitle;
            Data.Memo = FMemo;
            Data.Area = FArea;
            Data.Active = FActive;
            Data.Comment = FComment;
            Data.UpdateDate = FUpdateDate;
            Data.CreateDate = FCreateDate;

            return Data;
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
                    case 0: return "群組類別";
                    case 1: return "權限";
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

        #endregion
    }
}