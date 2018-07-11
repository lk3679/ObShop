using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using POS_Library.Public;
using POS_Library.DB;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class AuthPro : System.Web.UI.Page
    {
        #region 宣告

        POS_Library.Public.WmsAuth wmsauth = new WmsAuth();
        int area, AccountType;
        string account, searchbox;

        Label lblTemp;
        CheckBox CBTemp;
        CheckBoxList CBListTemp;
        ListItem LITemp;
        Panel PLTemp;
        System.Drawing.Color tempColor = System.Drawing.Color.DarkRed;
        //儲位所在地
        static private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

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
                    if (!IsPostBack)
                    {
                        SetCheckBoxList();
                    }

                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 主功能-查詢使用者清單

        /// <summary>
        /// 查詢使用者清單-按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            Search();
        }
        /// <summary>
        /// 查詢使用者清單
        /// </summary>
        protected void Search()
        {
            try
            {
                lbl_Count_UserList.Text = "";

                AccountType = int.Parse(DDL_AccountType.SelectedValue);
                area = _areaId;

                searchbox = txt_Search.Text.Trim();

                var typeList = GetSearchFunctionList();
                var temp = wmsauth.GetAllUserFunctionList(area, AccountType, -1);
                int y = 1;
                var temp2 = (from i in temp
                             where 
                             //比對帳號/條碼/姓名
                             (string.IsNullOrEmpty(searchbox) ? true :
                             ((!string.IsNullOrEmpty(i.User.Account) && i.User.Account.Contains(searchbox))
                             || (!string.IsNullOrEmpty(i.User.Barcode) && i.User.Barcode.Contains(searchbox))
                             || (!string.IsNullOrEmpty(i.User.Name) && i.User.Name.Contains(searchbox))))
                             //比對權限
                             && ((from aaa in i.FunctionList join bbb in typeList on aaa.功能編號 equals bbb select aaa).ToList().Count > 0) 
                             select new
                             {
                                 序號 = y++,
                                 Id = i.User.Id,
                                 類別 = TypeToName(i.User.Type, 0),
                                 倉庫 = TypeToName(i.User.Area, 1),
                                 帳號 = i.User.Account,
                                 條碼 = i.User.Barcode,
                                 姓名 = i.User.Name,
                                 有效 = i.User.Active,
                                 權限 = ListToString(i.FunctionList),
                                 建立時間 = i.User.CreateDate.ToString("yyy-MM-dd HH:mm"),
                                 更新時間 = i.User.UpdateDate.ToString("yyy-MM-dd HH:mm")
                             }).ToList();

                gv_UserList.DataSource = temp2;
                gv_UserList.DataBind();

                lbl_Count_UserList.Text = "資料筆數：" + temp2.Count;
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 串查詢權限清單
        /// </summary>
        /// <returns></returns>
        protected List<int> GetSearchFunctionList()
        {
            List<int> result = new List<int>();
            var AllFCount = int.Parse(lbl_FunctionCount.Text);
            for (int i = 0; i < AllFCount; i++)
            {
                CBListTemp = form1.FindControl("CBList_G" + i) as CheckBoxList;
                if (CBListTemp != null)
                {
                    foreach (var item in CBListTemp.Items)
                    {
                        var aa = item as ListItem;
                        if (aa.Selected)
                            result.Add(int.Parse(aa.Value.Split(',')[0]));
                    }
                }
            }
            return result;
        }

        #endregion

        #region 介面

        /// <summary>
        /// 組權限清單
        /// </summary>
        protected void SetCheckBoxList()
        {
            //div_SearchFunction.Controls.Clear();
            area = _areaId;
            var templist = wmsauth.GetAllFunctionList(area, -1);

            var groupFather = templist.Where(x => x.Type == 0).ToList();
            var groupChild = templist.Where(x => x.Type != 0).ToList();

            //先全改成隱藏
            var AllFCount = int.Parse(lbl_FunctionCount.Text);
            for (int i = 0; i < AllFCount;i++)
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

        /// <summary>
        /// 顯示/隱藏權限列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CB_ShowFunctionPanel_CheckedChanged(object sender, EventArgs e)
        {
            P_SearchFunction.Visible = CB_ShowFunctionPanel.Checked;
        }

        /// <summary>
        /// 切換倉庫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Area_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SetCheckBoxList();
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

                foreach (var i in bb.Items)
                {
                    var cc = i as ListItem;
                    cc.Selected = aa.Checked;
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CBList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var aa = sender as CheckBoxList;
                var result = true;
                foreach (var i in aa.Items)
                {
                    var cc = i as ListItem;
                    if (!cc.Selected)
                        result = false;
                }

                var dd = div_SearchFunction.FindControl("CB_All_G" + aa.ID.Substring(aa.ID.Length - 1, 1)) as CheckBox;
                dd.Checked = result;
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
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
    }
}