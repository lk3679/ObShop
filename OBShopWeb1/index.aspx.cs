using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.Poslib;
using POS_Library.Public;

namespace OBShopWeb
{
    public partial class index : System.Web.UI.Page
    {
        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Page.Header.Title = Page.Header.Title.Split('-')[0] + "-" + ((Utility.Store)int.Parse(Area.WmsAreaXml("ShopType"))).ToString();

                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                    Response.End();
                }
                else
                {
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