using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.EntranceService;
using OBShopWeb.AuthService;
using System.Web.Configuration;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class setup : System.Web.UI.Page
    {
        WmsAuth wmsauth = new WmsAuth();

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        #region ※舊權限

        /// <summary>
        /// 確認權限(2012-0110 舊權限資料)
        /// </summary>
        /// <param name="account"></param>
        public void check(String account)
        {

            String[] kwArr = { "jack", "shalom", "michael", "gary", "mark_huang", "ryan", 
                               "awei", "bibo", "shumin", "tien", "show" };
            if (kwArr.Contains(account))
                Session["kw"] = true;

            String[] auditArr = { "hata", "jack", "mark", "noin", "ob3", "ob4", "ob7", "ob10", "ob14", "ob19", "ob23", "tob7", 
                                  "tob16", "tob17", "viky", "syoutei", "may", "minnie", "minshu", "tien", "kate", "lynnho", 
                                  "sally19841005", "lynn", "oliver", "yang", "amy", "shalom", "michael", "gary", "mark_huang", "ryan", 
                                  "awei", "bibo" };
            if (auditArr.Contains(account))
                Session["audit"] = true;

            String[] privilegeArr = { "hata", "jack", "mark", "noin", "yang", "viky", "minnie", "minshu", "shalom", "tien", "michael", "gary", "mark_huang", "ryan", 
                                      "awei", "bibo", "shumin", "tien", "show" };
            if (privilegeArr.Contains(account))
                Session["privilege"] = true;

            String[] logisticsArr = { "hata", "jack", "mark", "shalom", "minnie", "minshu", "tien", "wun", "viky", "may", 
                                      "bibo", "ada", "noin", "yang", "oliver", "orangebear_anne", "yanlu", "michael", "gary", "mark_huang", "ryan", 
                                      "awei", "shumin", "tien", "show" };
            if (logisticsArr.Contains(account))
                Session["logistics"] = true;

            String[] logistics_privArr = { "hata", "jack", "mark", "shalom", "minnie", "minshu", "tien", "noin", "yang", "michael", "gary", "mark_huang", "ryan", 
                                           "awei", "bibo", "shumin", "tien", "show" };
            if (logistics_privArr.Contains(account))
                Session["logistics_priv"] = true;

            String[] posArr = { "hata", "jack", "mark", "viky", "may", "aaa7221", "sweet", "K731122", "a2001miko", 
                                "aldova234", "minnie", "minshu", "tien", "chyo", "junko", "africa", "shalom", "michael", "gary", "mark_huang", "ryan", 
                                "awei", "bibo", "shumin", "tien", "show" };
            if (posArr.Contains(account))
                Session["pos"] = true;

            String[] promotion_auditArr = { "hata", "noin", "yang", "minnie", "minshu", "tien", "shalom", "michael", "gary", "mark_huang", "ryan", 
                                            "awei", "bibo", "shumin", "tien", "show" };
            if (promotion_auditArr.Contains(account))
                Session["promotion_audit"] = true;

            String[] logisticAccountArr = { "michael", "gary", "mark_huang", "ryan" };
            if (logisticAccountArr.Contains(account))
                Session["logisticAccount"] = true;
        }

        #endregion

        #region ※新權限

        /// <summary>
        /// 設定權限(2012-0110 新權限確認方式)
        /// </summary>
        /// <param name="account"></param>
        public void setAuthority(List<EntranceService.Authority> authority)
        {
            List<String> a = new List<String>();

            for (int i = 0; i < authority.Count; i++)
            {
                //五股權限
                if (authority[i].Type == 0)
                    a.Add(authority[i].Title);
            }

            Session["authority"] = a;
        }
        /// <summary>
        /// 判斷是否有權限(2012-0110 新權限確認方式)
        /// </summary>
        /// <param name="account"></param>
        public bool checkAuthority(String authority)
        {
            if (Session["authority"] != null)
            {
                List<String> a = (List<String>)Session["authority"];

                if (a.Contains(authority))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 判斷是否有權限資料
        /// </summary>
        /// <param name="account"></param>
        public bool ExistAuthority()
        {
            if (Session["authority"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region ※AuthPro權限(2014-0509新增)

        /// <summary>
        /// 查詢(2014-0509 新權限確認方式)
        /// </summary>
        /// <param name="account"></param>
        public void setAuthorityPro(string account, int type)
        {
            var temp = wmsauth.GetLoginUser(account, _areaId, type);

            if (temp != null)
            {
                List<String> a = temp.FunctionList.Select(x => x.功能編號.ToString()).ToList();

                Session["authorityPro"] = a;

                List<String> b = temp.FunctionList.Select(x => x.功能名稱.ToString()).ToList();

                Session["authorityProName"] = b;
            }
        }

        /// <summary>
        /// 判斷是否有權限(2014-0509 新權限確認方式)
        /// </summary>
        /// <param name="account"></param>
        public bool checkAuthorityPro(String authority)
        {
            if (Session["authorityPro"] != null)
            {
                List<String> a = (List<String>)Session["authorityPro"];
                List<String> b = (List<String>)Session["authorityProName"];

                if (a.Contains(authority) || b.Contains(authority) || b.Contains("系統管理員"))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 判斷是否有權限資料(2014-0509 新權限確認方式)
        /// </summary>
        /// <param name="account"></param>
        public bool ExistAuthorityPro()
        {
            if (Session["authorityPro"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}