using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using POS_Library.Public;
using POS_Library.ShopPos.DataModel;
using POS_Library.ShopPos;
using OBShopWeb.PDA;
using System.Web.Configuration;
using OBShopWeb.Poslib;


namespace OBShopWeb
{
    public partial class OrderList : System.Web.UI.Page
    {
        #region 宣告

        CheckFormat CF = new CheckFormat();
        ShelfProcess sp = new ShelfProcess();
        ShipOutDA SO = new ShipOutDA();

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        private bool testmode = false;
        String account;
        int numcount = 0;
        object temp = new object();
        DateTime SDay, EDay;
        String HandleStartDate, HandleEndDate, SerialId, ProductId, SearchType;
        bool WithoutDate;

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
                account = Session["Name"].ToString();

                lbl_Message.Text = "";
                lbl_Count.Text = "";

                if (!IsPostBack)
                {
                    txt_Start.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    txt_End.Text = DateTime.Today.ToString("yyyy-MM-dd");
                }

                HandleStartDate = txt_Start.Text.Trim();
                HandleEndDate = txt_End.Text.Trim();

                if (!IsPostBack)
                    Search();
            }
        }

        #endregion

        #region ●主功能-查詢

        /// <summary>
        /// 按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_查詢_Click(object sender, EventArgs e)
        {
            Search();
        }

        /// <summary>
        /// 查詢清單
        /// </summary>
        void Search()
        {
            //日期參數
            SDay = DateTime.Parse(HandleStartDate);
            EDay = DateTime.Parse(HandleEndDate);
            WithoutDate = false;
            var temp1 = SO.GetOrderDetails("-1", SDay, EDay, WithoutDate).ToList();

            var temp2 = (from i in temp1
                         where !CB_未撿貨確認.Checked || (string.IsNullOrEmpty(i.Verifier) && i.Status == 1)
                         group i by new { i.OrderId, i.OrderTime, i.PosNo, i.ClerkName, i.Amount, i.InvoiceNo, i.Verifier, i.Status, i.PickType } into g1
                         orderby g1.Key.OrderId descending
                         select new
                         {
                             出貨序號 = g1.Key.OrderId,
                             交易時間 = g1.Key.OrderTime.Value.ToString("yyyy-MM-dd HH:mm"),
                             收銀機號 = g1.Key.PosNo,
                             收銀員 = g1.Key.ClerkName,
                             總金額 = g1.Key.Amount,
                             產品數量 = g1.Sum(y => y.Quantity).ToString(),
                             發票號碼 = g1.Key.InvoiceNo,
                             撿貨者 = string.IsNullOrEmpty(g1.Key.Verifier) ? "未確認" : g1.Key.Verifier,
                             訂單狀態 = (g1.Key.Status == 1) ? "正常" : "取消",
                             PDF = 組路徑(g1.Key.OrderId, g1.Key.OrderTime.Value, g1.Key.PickType.Value)
                         }).ToList();

            int x = 1;
            var temp3 = temp2.Select(i => new { i.出貨序號, i.交易時間, i.收銀機號, i.收銀員, i.總金額, i.產品數量, i.撿貨者, i.訂單狀態, i.PDF }).Distinct().Select(i => new
            {
                序號 = x++,
                i.出貨序號,
                i.交易時間,
                i.收銀機號,
                i.收銀員,
                i.總金額,
                i.產品數量,
                發票號碼 = string.Join(",", temp2.Where(y => y.出貨序號 == i.出貨序號).Select(y => y.發票號碼).ToList()),
                i.撿貨者,
                i.訂單狀態,
                i.PDF
            }).ToList();

            gv_List.DataSource = temp3;
            gv_List.DataBind();

            var 有效金額 = temp3.Where(i => i.訂單狀態 == "正常").Sum(i => i.總金額);
            var 有效件數 = temp3.Where(i => i.訂單狀態 == "正常").Sum(i => int.Parse(i.產品數量));
            var 無效金額 = temp3.Where(i => i.訂單狀態 == "取消").Sum(i => i.總金額);
            var 無效件數 = temp3.Where(i => i.訂單狀態 == "取消").Sum(i => int.Parse(i.產品數量));

            lbl_Count.Text = "總筆數：" + temp2.Count;
            lbl_Count.Text += ", 金額/件數：【有效】" + 有效金額 + " / " + 有效件數;
            lbl_Count.Text += ", 【無效】" + 無效金額 + " / " + 無效件數;
        }

        /// <summary>
        /// gv_Detail_RowDataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_List_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton CBtemp = GetCellByName(e.Row, "功能").FindControl("Print") as LinkButton;

                var 撿貨者 = GetCellByName(e.Row, "撿貨者").Text;
                var 訂單狀態 = GetCellByName(e.Row, "訂單狀態").Text;

                if (撿貨者 != "未確認" || 訂單狀態 == "取消")
                {
                    CBtemp.Visible = false;
                }
                else
                {
                    CBtemp.Visible = true;
                }

                //---------------------------------
            }

            //光棒效果
            GvLightBar.lightbar(e, 2);
        }

        #endregion

        #region ●主功能-查詢Detail/補印

        /// <summary>
        /// Grid_RowCommand
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string Id = e.CommandArgument.ToString();

                if (!string.IsNullOrEmpty(Id))
                {
                    switch (e.CommandName)
                    {
                        case "Detail":
                            //日期參數
                            SDay = DateTime.Parse(HandleStartDate);
                            EDay = DateTime.Parse(HandleEndDate);
                            WithoutDate = true;
                            foreach (GridViewRow row in gv_List.Rows)
                            {
                                var tempID = GetCellByName(row, "出貨序號").Text;

                                //判斷是否是按下按鈕的那一行
                                if (tempID == Id)
                                {
                                    GridView gv_Detail = GetCellByName(row, "功能").FindControl("gv_Detail") as GridView;
                                    Panel P_Detail = GetCellByName(row, "功能").FindControl("P_Detail") as Panel;

                                    if (gv_Detail.Rows.Count == 0)
                                    {
                                        var temp1 = SO.GetOrderDetails(Id, SDay, EDay, WithoutDate).ToList();

                                        int x = 1;
                                        var temp2 = (from i in temp1
                                                     select new
                                                     {
                                                         序號 = x++,
                                                         型號 = i.ProductId,
                                                         條碼 = i.Barcode,
                                                         數量 = i.Quantity.ToString(),
                                                     }).ToList();

                                        gv_Detail.DataSource = temp2;
                                        gv_Detail.DataBind();
                                    }

                                    P_Detail.Visible = !P_Detail.Visible;
                                }
                            }

                            break;

                        case "Print":
                            foreach (GridViewRow row in gv_List.Rows)
                            {
                                var tempID = GetCellByName(row, "出貨序號").Text;

                                //判斷是否是按下按鈕的那一行
                                if (tempID == Id)
                                {
                                    var 發票號碼 = GetCellByName(row, "發票號碼").Text;
                                    var store = POS_Library.Public.Utility.GetStore(_areaId);
                                    var order = SO.ShipOut(int.Parse(Id), store);
                                    //印單不扣數
                                    //var PrintPickListsShelf = SO.PickDetailCk(int.Parse(Id), (int)Utility.ShipPDF.出貨, store);
                                    //印單扣數 //order[0].TicketType存是否扣展售資訊
                                    var PrintPickListsShelf = SO.PrintPickListsShelf(order, int.Parse(Id), order[0].TicketType, store, _areaId);

                                    //是否印撿貨單(2015-0813修改)
                                    if (SystemSettings.GetNeedPrintPickSheet() == true)
                                    {
                                        Print p = new Print();
                                        p.PrintPickList(PrintPickListsShelf, 發票號碼, "交易明細(補印)");
                                    }
                                    else
                                    {
                                        var SO2 = new POS_Library.ShopPos.ShipOutDA();
                                        var MSListNewFinal = SO2.GetPerformanceSale(account, (int)POS_Library.Public.Utility.LogisticsType.撿貨, int.Parse(Id), _areaId);
                                    }
                                }
                            }
                            break;
                    }
                }
                else
                {
                    lbl_Message.Text = "無資料!!";
                }
            }
            catch (Exception ex)
            {
                lbl_Message.Text = "查詢錯誤!!" + ex.Message;
            }
        }



        #endregion 主功能-查詢Detail/全選/全不選

        #region 介面

        /// <summary>
        /// GetCellByName
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="CellName"></param>
        /// <returns></returns>
        public DataControlFieldCell GetCellByName(GridViewRow Row, String CellName)
        {
            try
            {
                foreach (DataControlFieldCell Cell in Row.Cells)
                {
                    if (Cell.ContainingField.ToString() == CellName)
                        return Cell;
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 重整
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if(CB_Timer.Checked)
                Search();
        }

        protected void gv_Detail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //光棒效果
            GvLightBar.lightbar(e, 3);
        }

        /// <summary>
        /// 產生PDF路徑(印單機不能使用時)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="date"></param>
        /// <param name="pickType"></param>
        /// <returns></returns>
        public string 組路徑(int num, DateTime date, int pickType)
        {
            string Date = DateTime.Now.ToString("yyyyMMdd");
            var store = Utility.GetStoreForShop(_areaId);
            var barcode = num.ToString() + store.ToString().PadLeft(2, '0') + _areaId.ToString().PadLeft(2, '0') + pickType.ToString().PadLeft(2, '0');

            return "\\\\POS01\\PDF_Pick" + _areaId.ToString() + (testmode ? "_Test" : "") + "\\PickList_" + date.ToString("yyyyMMdd") + "_" + barcode + ".pdf";
        }

        #endregion
    }
}