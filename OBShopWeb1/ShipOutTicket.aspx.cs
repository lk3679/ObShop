using System;
using System.Linq;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class ShipOutTicket : System.Web.UI.Page
    {
        #region 宣告

        //儲位所在地
        static private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        int store = POS_Library.Public.Utility.GetStore(_areaId);

        private LinkButton LBtn_Temp;
        private HyperLink HL_Temp;
        private Label lbl_Space;

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
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region ●主功能-查詢

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                bool isAudit = false;
                var selectType = int.Parse(DDL_Type.SelectedValue);
                //門市/福袋、瑕疵退廠調出查詢
                var shipDa = new POS_Library.ShopPos.ShipOutDA(); 
                var posTickets = shipDa.PosTickets(selectType, _areaId); 
                int si = 1;
                var temp = posTickets.OrderByDescending(x => x.TicketDate).Select(x => new
                {
                    序號 = si++,
                    傳票ID = x.TicketId,
                    出貨日期 = x.TicketDate.ToString("yyyy-MM-dd"),
                    印單日期 = x.VerifyDate == null ? "" : x.VerifyDate.Value.ToString("yyyy-MM-dd"),
                    驗貨日期 = x.AuditDate == null ? "" : x.AuditDate.Value.ToString("yyyy-MM-dd"),
                    驗貨者 = x.AuditAccount,
                    功能 = "",
                }).ToList();
                gv_List.DataSource = temp;
                gv_List.DataBind();

                lbl_Count.Text = "總筆數：" + gv_List.Rows.Count;
            }
            catch (Exception ex)
            {
                lbl_Message.Text = ex.Message;
            }
        }

        public enum EnumTicketFlowType
        {
            調出 = 1,
            瑕疵 = 6,
        }

        #endregion

        #region gv_List_RowDataBound

        /// <summary>
        /// gv_List_RowDataBound新增功能欄位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_List_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                var selectType = int.Parse(DDL_Type.SelectedValue);
                var pickType = 0;
                switch (selectType)
                {
                    case (int)EnumTicketFlowType.調出:
                        pickType = (int)POS_Library.Public.Utility.ShipPDF.寄倉調出;
                        break;

                    case (int)EnumTicketFlowType.瑕疵:
                        pickType = (int)POS_Library.Public.Utility.ShipPDF.瑕疵退倉;
                        break;

                    default:
                        Response.Write("種類不正確！");
                        break;
                }
                //若為DataRow則放入HyperLink
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem;
                    string gridTicketId = dataItem.GetType().GetProperty("傳票ID").GetValue(dataItem, null).ToString().Trim();
                    string gridDate = dataItem.GetType().GetProperty("出貨日期").GetValue(dataItem, null).ToString().Trim();
                    bool isVerifier = dataItem.GetType().GetProperty("印單日期").GetValue(dataItem, null).ToString().Trim() != "";
                    bool 驗貨 = dataItem.GetType().GetProperty("驗貨日期").GetValue(dataItem, null).ToString().Trim() != "";

                    HL_Temp = new HyperLink();
                    HL_Temp.Text = "明細";
                    HL_Temp.NavigateUrl = string.Format("ShipTicketDetail.aspx?tick={0}&areaId={1}", gridTicketId, _areaId);
                    HL_Temp.Target = "content";
                    e.Row.Cells[6].Controls.Add(HL_Temp);

                    //未驗貨才可印單
                    if (!驗貨)
                    {
                        //空白
                        lbl_Space = new Label();
                        lbl_Space.Text = "  ";
                        e.Row.Cells[6].Controls.Add(lbl_Space);

                        LBtn_Temp = new LinkButton();
                        LBtn_Temp.Text = "撿單";
                        LBtn_Temp.PostBackUrl = "";
                        LBtn_Temp.OnClientClick = string.Format("return CreateDoc(this, '{0}','{1}','{2}','{3}');", pickType, gridTicketId, _areaId, store);
                        e.Row.Cells[6].Controls.Add(LBtn_Temp);
                    }
                    if (isVerifier && !驗貨)
                    {
                        //空白
                        lbl_Space = new Label();
                        lbl_Space.Text = "  ";
                        e.Row.Cells[6].Controls.Add(lbl_Space);

                        LBtn_Temp = new LinkButton();
                        LBtn_Temp.Text = "再確認";
                        LBtn_Temp.PostBackUrl = "";
                        LBtn_Temp.OnClientClick = string.Format("return Verify(this, '{0}','{1}','{2}');", gridTicketId, _areaId, store);
                        e.Row.Cells[6].Controls.Add(LBtn_Temp);
                    }
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