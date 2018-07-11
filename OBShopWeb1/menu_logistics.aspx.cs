using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OBShopWeb
{
    public partial class menu_logistics : System.Web.UI.Page
    {
        #region 宣告

        private setup auth = new setup();
        private bool do_del_noproduct = false;

        string account;

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Session["Account"] = "jack";
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                    Response.End();
                }
                else
                {
                    account = Session["Account"].ToString();
                    //管理員或sophia
                    if (!auth.checkAuthority("administrator") && account != "sophia")
                    {
                        HL_logisticsScoreSetting.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion
    }
}