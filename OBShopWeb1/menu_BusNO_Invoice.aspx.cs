using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.AuthService;

namespace OBShopWeb
{
    public partial class menu_BusNO_Invoice : System.Web.UI.Page
    {
        #region 宣告

        String ShipOutType;
        setup auth = new setup();

        #endregion

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
                    //統一編號HL顯示
                    if (Session["privilege"] != null || auth.checkAuthority("privilege"))
                        HL_BusNo.Visible = true;
                    else
                        HL_BusNo.Visible = false;

                    ShipOutType = Session["ShipOutType"].ToString();

                    ////補印發票HL顯示
                    //if (ShipOutType == "官網")
                    //{
                    //    HL_print_invoice.NavigateUrl = "http://workplat.obdesign.com.tw/kw_busno2.aspx";
                    //    HL_print_invoice.Target = "_blank";
                    //}
                    //else if (ShipOutType == "橘熊")
                    //{
                    //    HL_print_invoice.NavigateUrl = "~/PrintInvoice.aspx";
                    //    HL_print_invoice.Target = "content";
                    //}
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