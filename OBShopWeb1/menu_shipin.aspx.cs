using System;
using System.Web.Configuration;

namespace OBShopWeb
{
    public partial class menu_shipin : System.Web.UI.Page
    {
        #region 宣告

        private setup auth = new setup();

        #endregion 宣告

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                    Response.End();
                }
                else
                {
                    if (auth.checkAuthority("administrator"))
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion Page_Load
    }
}