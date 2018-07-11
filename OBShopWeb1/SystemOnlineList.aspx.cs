using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class SystemOnlineList : System.Web.UI.Page
    {
        #region 宣告

        setup auth = new setup();

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_Shop.Text = "-" + ((Utility.Store)int.Parse(Area.WmsAreaXml("ShopType"))).ToString();

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

                int x = 1;
                var temp2 = (from i in temp
                             select new
                             {
                                 序號 = x++,
                                 帳號 = auth.checkAuthority("administrator") ? i.Account : i.Account.Substring(0, 1) + "***" + i.Account.Substring(i.Account.Length - 1, 1),
                                 姓名 = i.Name,
                                 IP位址 = auth.checkAuthority("administrator") ? i.IP : "*.*." + i.IP.Split('.')[2] + "." + i.IP.Split('.')[3],
                                 登入時間 = i.Time,
                             }
                            ).ToList();

                switch (DDL_OrderBy.SelectedValue)
                {
                    case "0": temp2 = temp2.OrderBy(i => i.登入時間).ToList(); break;
                    case "1": temp2 = temp2.OrderBy(i => i.帳號).ToList(); break;
                    case "2": temp2 = temp2.OrderBy(i => i.姓名).ToList(); break;
                    case "3": temp2 = temp2.OrderBy(i => i.IP位址).ToList(); break;
                }

                gv_List.DataSource = temp2;
                gv_List.DataBind();

                lbl_Count.Text = "線上人數：" + gv_List.Rows.Count;

                Page.Header.Title = "(" + gv_List.Rows.Count + ") " + Page.Header.Title.Split(' ')[1];
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