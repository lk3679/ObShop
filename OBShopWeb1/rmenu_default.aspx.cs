using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using POS_Library.Public;

namespace OBShopWeb
{
    public partial class rmenu_default : System.Web.UI.Page
    {
        #region 宣告

        setup auth = new setup();

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            SetList();
        }

        #endregion

        #region 取得名單

        /// <summary>
        /// 取得名單
        /// </summary>
        protected void SetList()
        {
            try
            {
                var temp = (List<Utility.OnlinePerson>)Application["onlineList"];

                lbl_Online.Text = temp.Count.ToString();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 重整
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            SetList();
        }

        #endregion
    }
}