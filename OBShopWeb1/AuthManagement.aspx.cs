using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using OBShopWeb.AuthService;
using OBShopWeb.ADVerifyService;

namespace OBShopWeb
{
    public partial class AuthManagement : System.Web.UI.Page
    {
        #region 宣告

        //龜山0
        int type = 0;
        ADVerifyClient ADVC = new ADVerifyClient();
        List<OBShopWeb.AuthService.AuthMapping> temp;

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            //清除訊息文字
            lbl_Message.Text = string.Empty;

            if (!IsPostBack)
            {
                GetAllData();
            }
        }

        #endregion

        #region 主功能-取得所有權限資料

        /// <summary>
        /// 取得所有權限資料
        /// </summary>
        protected void GetAllData()
        {
            //取得mapping資料
            var authService = new AuthClient();
            ViewState["mappings"] = temp = authService.GetAuthMappings(type).ToList();
            //List<OBShopWeb.AuthService.AuthMapping> aa = authService.GetAuthMappings(type);

            //取得function資料0預設為物流系統
            ViewState["functions"] = authService.GetAuthFunctions(type).OrderBy(x => x.Title).ToList();

            if (!CB_顯示姓名.Checked)
            {
                ltbMapping.DataSource = ViewState["mappings"];
                ltbMapping.DataBind();
            }
            else
            {
                ltbMapping.Items.Clear();

                //增加顯示姓名(2013-1104新增)-----------------------
                foreach (var i in temp)
                {
                    var ADI = ADVC.Verify("OBDesign.com.tw", i.Account, "");

                    ListItem aa = new ListItem();
                    aa.Value = i.Account;
                    aa.Text = i.Account + " (" + ADI.Fullname + ")";
                    ltbMapping.Items.Add(aa);
                }
                //----------------------------------------------
            }

            ltbFunction.DataSource = ViewState["functions"];
            ltbFunction.DataBind();
        }

        #endregion

        #region 主功能-增加AD/權限

        /// <summary>
        /// 新增AD帳號
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_AddAD_Click(object sender, EventArgs e)
        {
            var ad_service = new ADVerifyService.ADVerifyClient();
            var addaccount = txb_ADaccount.Text.Trim();

            var result = ad_service.Verify("OBdesign.com.tw", addaccount, null);
            if (result.VertifyState == ADVerifyService.State.NoAccount)
            {
                lbl_Message.Text = "無此AD帳號";
                txb_ADaccount.Text = string.Empty;
                return;
            }
            //2013-1009新增
            else if (result.VertifyState == ADVerifyService.State.Error)
            {
                lbl_Message.Text = "Server錯誤";
                txb_ADaccount.Text = string.Empty;
                return;
            }
            else if (result.VertifyState == ADVerifyService.State.NoPass)
            {
                var authService = new AuthClient();
                var mappings = (List<AuthMapping>)ViewState["mappings"];
                if (mappings.Count > 0 && mappings.FirstOrDefault(x => x.Account == addaccount) != null)
                {
                    lbl_Message.Text = "AD帳號已存在";
                    txb_ADaccount.Text = string.Empty;
                    return;
                }

                var mapping = new AuthMapping();
                mapping.Authoritys = new List<Authority>();
                mapping.Account = txb_ADaccount.Text.Trim().ToLower();
                mappings.Add(mapping);
                ViewState["mappings"] = mappings;

                if (!CB_顯示姓名.Checked)
                {
                    ltbMapping.DataSource = ViewState["mappings"];
                    ltbMapping.DataBind();
                }
                else
                {
                    //增加顯示姓名(2013-1104新增)-----------------------
                    ltbMapping.Items.Clear();

                    foreach (var i in mappings)
                    {
                        var ADI = ADVC.Verify("OBDesign.com.tw", i.Account, "");

                        ListItem aa = new ListItem();
                        aa.Value = i.Account;
                        aa.Text = i.Account + " (" + ADI.Fullname + ")";
                        ltbMapping.Items.Add(aa);
                    }
                    //----------------------------------------------
                }

                lbl_Message.Text = ADVC.Verify("OBDesign.com.tw", addaccount, "").Fullname;
                lbl_Message.Text += ", 加入清單成功，請選擇此帳號之權限!";

                txb_ADaccount.Text = string.Empty;
            }
        }

        #endregion

        #region 主功能-修改/儲存

        protected void btnModify_Click(object sender, EventArgs e)
        {
            var mappings = (List<AuthMapping>)ViewState["mappings"];
            var mapping = mappings.FirstOrDefault(x => x.Account == ltbMapping.SelectedValue);
            mapping.Authoritys.Clear();

            foreach (ListItem item in ltbFunction.Items)
            {
                var functions = (List<Authority>)ViewState["functions"];
                if (item.Selected)
                {
                    var func = functions.FirstOrDefault(x => x.Index == item.Value);
                    mapping.Authoritys.Add(func);
                }
            }
            ViewState["mappings"] = mappings;

            lbl_Message.Text = "已修改，完成操作後請按儲存!";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ////var functions = (List<Authority>)ViewState["functions"];
            //var mappings = (List<AuthMapping>) ViewState["mappings"];

            //var authService = new AuthClient();
            ////authService.WriteAuthFunctions(functions, 0);
            //authService.WriteAuthMappings(mappings, type);

            var skipCount = 10;
            //var functions = (List<Authority>)ViewState["functions"];
            var mappings = (List<AuthMapping>)ViewState["mappings"];

            var authService = new AuthClient();
            //authService.WriteAuthFunctions(functions, 0);
            //一次回存10個
            for (int i = 0; i < mappings.Count; i = i + 0)
            {
                var count = i;
                //判斷是否是最後一次 存剩下的
                if (mappings.Count - i < skipCount)
                    count = mappings.Count - i;
                else
                    count = skipCount;
                //WCF
                authService.WriteAuthMappings(mappings.Skip(i).Take(count).ToList(), type);
                //判斷是否是最後一次 跳出
                if (count < skipCount)
                    break;
                else
                    i = i + skipCount;
            }

            lbl_Message.Text = "儲存成功!";
        }

        #endregion

        #region 讀取/Bind

        protected void ltbFunction_DataBound(object sender, EventArgs e)
        {
            var functions = (List<Authority>)ViewState["functions"];

            foreach (ListItem item in ltbFunction.Items)
            {
                var firstOrDefault = functions.FirstOrDefault(x => x.Index == item.Value);
                if (firstOrDefault != null)
                {
                    var memo = firstOrDefault.Memo;
                    item.Text += "(" + memo + ")";
                }
            }
        }

        protected void ltbMapping_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mappings = (List<AuthMapping>)ViewState["mappings"];
            var mapping = mappings.FirstOrDefault(x => x.Account == ltbMapping.SelectedValue);
            ViewState["mapping"] = mapping;

            if (ckbAutoLoad.Checked)
                foreach (ListItem item in ltbFunction.Items)
                {
                    if (mapping.Authoritys.FirstOrDefault(x => x.Index == item.Value) != null)
                        item.Selected = true;
                    else
                        item.Selected = false;
                }
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            var mapping = (AuthMapping)ViewState["mapping"];
            foreach (ListItem item in ltbFunction.Items)
            {
                if (mapping.Authoritys.FirstOrDefault(x => x.Index == item.Value) != null)
                    item.Selected = true;
                else
                    item.Selected = false;
            }
        }

        protected void CB_顯示姓名_CheckedChanged(object sender, EventArgs e)
        {
            GetAllData();
        }

        #endregion

    }
}