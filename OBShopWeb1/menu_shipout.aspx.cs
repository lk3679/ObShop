using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.AuthService;
using System.Web.Configuration;

namespace OBShopWeb
{
    public partial class menu_shipout : System.Web.UI.Page
    {
        #region 宣告

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                setup auth = new setup();

                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                    Response.End();
                }
                else
                {
                    HL_pos_check_out2.Visible = auth.checkAuthorityPro("15") && (Session["ClerkID"] != null);
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