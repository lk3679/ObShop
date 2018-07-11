using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace OBShopWeb
{
    public partial class logisticsScoreSetting : System.Web.UI.Page
    {
        #region 宣告

        private setup auth = new setup();
        private bool do_del_noproduct = false;

        string account;

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Account"] == null)
            {
                Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                Response.End();
            }
            else
            {
                account = Session["Account"].ToString();
                lbl_Msg.Text = "";
                //管理員或sophia
                if (!auth.checkAuthority("administrator") && account != "sophia")
                {
                    Response.Redirect("Privilege.aspx");
                }

                if (!IsPostBack)
                {
                    DefaultLoad();
                }
            }
        }

        #endregion

        #region 主功能-設定

        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Send_Click(object sender, EventArgs e)
        {
            if (gv_logistics.Rows.Count == 0)
                return;

            string fail = "";
            foreach (GridViewRow iRow in gv_logistics.Rows)
            {
                TextBox tb = (TextBox)iRow.FindControl("txtScore");

                var newScore = decimal.Parse(tb.Text);
                if (decimal.Parse(iRow.Cells[1].Text) != newScore)
                {
                    var job = new POS_Library.ShopPos.LogisticsAccount();
                    var setting = job.SetLogisticsSetting(iRow.Cells[0].Text, newScore);
                    if (!setting)
                    {
                        fail += iRow.Cells[0].Text + ",";
                    }

                }
            }

            DefaultLoad();
            lbl_Msg.Text = (string.IsNullOrEmpty(fail)) ? " 設定成功!" : fail + " 設定失敗! ";

        }

        #endregion

        #region 主功能-查詢

        /// <summary>
        /// 查詢
        /// </summary>
        protected void DefaultLoad()
        {
            var job = new POS_Library.ShopPos.LogisticsAccount();
            var setting = job.GetLogisticsSetting().Select(x => new
            {
                Name = x.GroupName,
                oldScore = x.Score,
                newScore = x.Score
            }).Distinct().ToList();
            gv_logistics.DataSource = setting;
            gv_logistics.DataBind();
        }

        #endregion

        #region 介面

        /// <summary>
        /// 加光棒效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_logistics_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //光棒效果
                //判定row的型態是資料行
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    GvLightBar.lightbar(e, 1);
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