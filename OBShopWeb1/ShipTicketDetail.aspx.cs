using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OBShopWeb
{
    public partial class ShipTicketDetail : System.Web.UI.Page
    {
        #region 宣告

        string account, box;
        int ticketId, areaId;

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            account = Session["Account"].ToString();
            box = Request["box"] != null ? Request["box"].Trim() : "";
            ticketId = Request["tick"] != null ? int.Parse(Request["tick"].ToString()) : 0;
            areaId = int.Parse(Request["areaId"].Trim());

            Search();

        }

        #endregion

        #region ●主功能-查詢

        public void Search()
        {
            try
            {
                var temp = new List<POS_Library.ShopPos.DataModel.ImportClass.TicketDetailModel>();
                if (!string.IsNullOrEmpty(box))
                {
                    var shipDa = new POS_Library.ShopPos.ShipInDA();
                    temp = shipDa.GetPosBoxDetail(box, areaId);
                }
                else if (ticketId != 0)
                {
                    var shipDa = new POS_Library.ShopPos.ShipOutDA();
                    temp = shipDa.GetTicketDetail(ticketId, areaId, false, true);
                }
                int x = 1;
                var temp2 = (from i in temp
                             select new
                             {
                                 序號 = x++,
                                 傳票ID = i.TicketId,
                                 產品編號 = i.ProductId,
                                 數量 = i.Quantity

                             }).ToList();

                gv_List.DataSource = temp2;
                gv_List.DataBind();

                var 總筆數 = temp2.Count;
                var 傳票數 = temp2.Select(y => y.傳票ID).Distinct().Count();
                var 總件數 = temp2.Sum(y => y.數量);

                lbl_Count.Text = "總筆數: " + 總筆數 + ", 傳票數: " + 傳票數 + ", 總件數: " + 總件數;
            }
            catch (Exception ex)
            {
                lbl_Count.Text = ex.Message;
            }
        }

        #endregion

        #region gv_List_RowDataBound

        protected void gv_List_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GvLightBar.lightbar(e, 2);
        }

        #endregion
    }
}