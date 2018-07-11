using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OBShopWeb
{
    public partial class menu_ship_manage : System.Web.UI.Page
    {
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
                    setup auth = new setup();
                    //庫存調整(查詢)
                    if (auth.checkAuthority("adjuststock"))
                    {
                        HL_stockAdjust.Visible = true;
                    }
                    else
                    {
                        HL_stockAdjust.Visible = false;
                    }
                    //無貨刪單
                    if (auth.checkAuthority("del_noproduct"))
                    {
                        HL_noProduct.Visible = true;
                    }
                    else
                    {
                        HL_noProduct.Visible = false;
                    }
                    //撿貨誤差(2013-0510新增)
                    if (auth.checkAuthority("mistake_report"))
                    {
                        HL_MistakeReport.Visible = true;
                    }
                    else
                    {
                        HL_MistakeReport.Visible = false;
                    }

                    //HL_storage.NavigateUrl += "?Account=" + Session["Account"].ToString() + "&Name=" + Session["Name"].ToString();
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