using System;
using System.Data;
using System.Linq;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class ShipInTicket : System.Web.UI.Page
    {
        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        private LinkButton LBtn_Temp;
        private HyperLink HL_Temp;
        private Label lbl_Space;

        private string account;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //判斷帳號登入
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                    Response.End();
                }
                else
                {
                    account = Session["Account"].ToString();
                    if (!IsPostBack)
                    {
                        SetTable();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                bool isVerify = false;
                var selectType = int.Parse(DDL_Type.SelectedValue);
                var shipDa = new POS_Library.ShopPos.ShipInDA();

                var posTickets = shipDa.PosTickets(isVerify, selectType, _areaId); 
                int si = 1;
                var temp = posTickets.OrderByDescending(x => x.TicketDate).Select(x => new
                {
                    序號 = si++,
                    傳票ID = x.TicketId,
                    箱號 = x.TicketBox,
                    出貨日期 = x.TicketDate.ToString("yyyy-MM-dd"), 
                    收貨者 = x.Consignee,
                    驗貨者 = x.VerifyAccount,
                    類型 = x.TicketType == 0 ? "進貨" : x.TicketType == 2?"調入":"不知名",
                    功能 = "",
                    TicketType = x.TicketType,
                }).ToList();
                gv_List.DataSource = temp;
                gv_List.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #region 設定

        /// <summary>
        /// 建立dt
        /// </summary>
        protected void SetTable()
        {
            try
            {
                DataTable dt = new DataTable("ALLData");
                //datatable
                dt.Columns.Add("序號", typeof(String));
                dt.Columns.Add("傳票ID", typeof(String));
                dt.Columns.Add("箱號", typeof(String));
                dt.Columns.Add("出貨日期", typeof(String));
                dt.Columns.Add("驗貨者", typeof(String));
                dt.Columns.Add("類型", typeof(String));
                dt.Columns.Add("功能", typeof(String));
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
         
        #endregion 設定

        #region gv_List_RowDataBound

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_List_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                e.Row.Cells[8].Visible = false;
                //若為DataRow則放入HyperLink
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem;
                    string gridTicketId = dataItem.GetType().GetProperty("傳票ID").GetValue(dataItem, null).ToString().Trim();
                    string gridBox = dataItem.GetType().GetProperty("箱號").GetValue(dataItem, null).ToString().Trim();
                    var consignee = dataItem.GetType().GetProperty("收貨者").GetValue(dataItem, null);
                    bool isVerifier = dataItem.GetType().GetProperty("驗貨者").GetValue(dataItem, null).ToString().Trim() != "";
                    int ticketType = int.Parse(dataItem.GetType().GetProperty("TicketType").GetValue(dataItem, null).ToString());

                    HL_Temp = new HyperLink();
                    HL_Temp.Text = "明細";
                    HL_Temp.NavigateUrl = string.Format("ShipTicketDetail.aspx?box={0}&areaId={1}", gridBox, _areaId);
                    HL_Temp.Target = "content";
                    e.Row.Cells[7].Controls.Add(HL_Temp);
                    if (!(ticketType == 0 && consignee==null))
                    {
                        //未驗貨
                        if (!isVerifier)
                        {
                            lbl_Space = new Label();
                            lbl_Space.Text = "  ";
                            e.Row.Cells[7].Controls.Add(lbl_Space);

                            LBtn_Temp = new LinkButton();
                            LBtn_Temp.Text = "驗貨";
                            LBtn_Temp.PostBackUrl = "";
                            LBtn_Temp.OnClientClick = string.Format("return Check(this, '{0}', '{1}', '{2}', '{3}');", gridTicketId, gridBox, _areaId, ticketType);
                            e.Row.Cells[7].Controls.Add(LBtn_Temp);
                        } 
                    } 
                    
                    //修改編號為連結
                    //HyperLink HL_id = new HyperLink();
                    //HL_id.Text = gridTicketId;
                    //HL_id.NavigateUrl = "http://erp03.obdesign.com.tw/admin/products/checked.aspx?SendID=" + gridTicketId;
                    //HL_id.Target = "_blank";
                    //e.Row.Cells[1].Text = "";
                    //e.Row.Cells[1].Controls.Add(HL_id);
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion gv_List_RowDataBound
    }
}