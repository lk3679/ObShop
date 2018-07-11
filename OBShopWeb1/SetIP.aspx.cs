using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OBShopWeb
{
    public partial class SetIP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_Message.Text = "";
            txt_SetIP.Focus();

            if (!IsPostBack)
            {
                //cookieIP(2012-1205新增)---------------------
                if (Request.Cookies["myip"] == null)
                {
                    var cookie = new HttpCookie("myip", HttpContext.Current.Request.UserHostAddress);
                    cookie.Expires = DateTime.Now.AddYears(1);
                    Response.Cookies.Add(cookie);

                    lbl_CookieIP.Text = HttpContext.Current.Request.UserHostAddress;
                }
                else
                {
                    lbl_CookieIP.Text = Request.Cookies["myip"].Value;
                }
                //---------------------------------------------
            }
        }
        /// <summary>
        /// 更改cookieIP & SessionIP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_SetIP_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_SetIP.Text.Trim() != "")
                {
                    var cookie = new HttpCookie("myip", txt_SetIP.Text.Trim());
                    cookie.Expires = DateTime.Now.AddYears(1);
                    Response.Cookies.Set(cookie);
                    Session["ip"] = txt_SetIP.Text.Trim();

                    lbl_Message.Text = "修改完成，請關閉此頁，重新整理操作頁面";
                }
                else
                {
                    lbl_Message.Text = "請輸入正確IP";
                }

            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }

        }
    }
}