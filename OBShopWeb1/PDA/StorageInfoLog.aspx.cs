using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using POS_Library;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb.PDA
{
    public partial class StorageInfoLog : System.Web.UI.Page
    {
        #region 宣告

        setup auth = new setup();
        //bool result;

        CheckFormat CF = new CheckFormat();
        ShelfProcess sp = new ShelfProcess();

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        String account;

        String ticketId, shipoutId, productId, shelfId, shopType;
        String HandleStartDate, HandleEndDate;
        DateTime SDay, EDay;
        bool WithoutDate;
        //權限用
        bool admincheck = false;

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= '../logout.aspx?urlx=PDA/StorageInfoLog.aspx' </script> ");
                    Response.End();
                }
                else
                {
                    if (!IsPostBack)
                    {
                        txt_Start.Text = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
                        txt_End.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    }

                    account = Session["Name"].ToString();
                    CB_ShowStorageId.Visible = auth.checkAuthority("administrator");
                    admincheck = CB_ShowStorageId.Checked;

                    HandleStartDate = txt_Start.Text.Trim();
                    HandleEndDate = txt_End.Text.Trim();

                    lbl_Count_ActionLog.Text = "";
                    lbl_Count_ExportLog.Text = "";
                    lbl_Count_ImportLog.Text = "";
                    lbl_Count_IssueReport.Text = "";
                    lbl_Count_StorageDetail.Text = "";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region ●介面

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
        /// 切目錄選單
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Choice_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearGV();
        }

        /// <summary>
        /// 清除GV
        /// </summary>
        protected void ClearGV()
        {
            gv_List_ActionLog.DataSource = "";
            gv_List_ActionLog.DataBind();
            gv_List_ExportLog.DataSource = "";
            gv_List_ExportLog.DataBind();
            gv_List_ImportLog.DataSource = "";
            gv_List_ImportLog.DataBind();
            gv_List_IssueReport.DataSource = "";
            gv_List_IssueReport.DataBind();
            gv_List_StorageDetail.DataSource = "";
            gv_List_StorageDetail.DataBind();
        }

        /// <summary>
        /// 清除TxtBox按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Clear_Txt_Click(object sender, EventArgs e)
        {
            ClearTxtBox();
        }

        /// <summary>
        /// 清除TxtBox
        /// </summary>
        protected void ClearTxtBox()
        {
            txt_ticketId.Text = "";
            txt_shipoutId.Text = "";
            txt_productId.Text = "";
            txt_shelfId.Text = "";
            DDL_shoptype.SelectedIndex = 0;
        }

        #endregion

        #region ●主功能-查詢(ActionLog/ExportLog/ImportLog/IssueReport/StorageDetail)

        #region ※執行查詢按鈕

        /// <summary>
        /// 執行查詢按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                //清空gv
                ClearGV();

                //查詢值
                ticketId = txt_ticketId.Text = txt_ticketId.Text.Trim();
                shipoutId = txt_shipoutId.Text = txt_shipoutId.Text.Trim();
                productId = txt_productId.Text = txt_productId.Text.Trim();
                shelfId = txt_shelfId.Text = txt_shelfId.Text.Trim();
                shopType = DDL_shoptype.SelectedValue;

                //日期參數
                SDay = DateTime.Parse(HandleStartDate);
                EDay = DateTime.Parse(HandleEndDate).AddDays(1);
                WithoutDate = CB_Date.Checked;

                //ActionLog
                if ((!String.IsNullOrEmpty(ticketId) || !String.IsNullOrEmpty(shipoutId) ||
                    !String.IsNullOrEmpty(productId) || !String.IsNullOrEmpty(shelfId)) &&
                    CB_Search_ActionLog.Checked)
                    SearchActionLog();
                //ExportLog
                if ((!String.IsNullOrEmpty(ticketId) || !String.IsNullOrEmpty(shipoutId) || !String.IsNullOrEmpty(productId)) &&
                    CB_Search_ExportLog.Checked)
                    SearchExportLog();
                //ImportLog
                if ((!String.IsNullOrEmpty(ticketId) || !String.IsNullOrEmpty(productId)) &&
                    CB_Search_ImportLog.Checked)
                    SearchImportLog();
                //IssueReport
                if ((!String.IsNullOrEmpty(ticketId) || !String.IsNullOrEmpty(shipoutId) || !String.IsNullOrEmpty(productId) || !String.IsNullOrEmpty(shelfId)) &&
                    CB_Search_IssueReport.Checked)
                    SearchIssueReport();
                //StorageDetail
                if ((!String.IsNullOrEmpty(productId) || !String.IsNullOrEmpty(shelfId)) &&
                    CB_Search_StorageDetail.Checked)
                    SearchStorageDetail();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region ○查詢ActionLog

        /// <summary>
        /// 查詢ActionLog
        /// </summary>
        protected void SearchActionLog()
        {
            try
            {
                var aa = sp.GetStorageActionLogByAll(SDay, EDay, WithoutDate, ticketId, shipoutId, productId, shelfId, shopType, _areaId).ToList();

                int x = 1;
                var temp = (from i in aa
                            orderby i.LogDateTime descending
                            select new
                            {
                                序號 = x++,
                                日期 = i.LogDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                產品編號 = i.ProductNumber,
                                數量 = (i.Quantity > 0) ? i.Quantity : -1 * i.Quantity, //(i.IsImport) ? i.Quantity : -1 * i.Quantity,
                                來源儲位編號 = i.FromStorage,
                                目的儲位編號 = i.TargetStorage,
                                帳號 = (i.LogAccount == "_BOT_") ? "※系統※" : i.LogAccount,
                                狀態 =
                                (i.TargetStorage == Utility.GetShelfData((int)EnumData.StorageType.無貨儲位, _areaId).Shelf) ? "無貨刪單-有貨" :
                                (i.TargetStorage == Utility.GetShelfData((int)EnumData.StorageType.打銷儲位, _areaId).Shelf && (i.FromStorage.StartsWith("export"))) ? 
                                ((i.StorageDetailTypeId == 8 && i.StorageTypeId == 9 && i.IsImport == true) ? "無貨刪單-打銷" : "建議儲位-打銷") :
                                (i.TargetStorage == Utility.GetShelfData((int)EnumData.StorageType.打銷儲位, _areaId).Shelf) ? "盤點-無條件打銷" :
                                (i.TargetStorage == Utility.GetShelfData((int)EnumData.StorageType.不良儲位, _areaId).Shelf) ? "不良品" :
                                (i.TargetStorage == Utility.GetShelfData((int)EnumData.StorageType.問題暫存儲位, _areaId).Shelf) ? "特殊回報上架" :
                                //分"印單扣數"是否為重出(2014-0121新增)
                                (i.StorageDetailTypeId == 4 && i.LogAccount == "_BOT_") ? 
                                ((i.FromStorage == Utility.GetShelfData((int)EnumData.StorageType.無貨儲位, _areaId).Shelf) ? "印單扣數(重出)" : "印單扣數") :
                                (i.StorageDetailTypeId == 4 && i.LogAccount != "_BOT_") ? "建議儲位" :
                                //分"撿貨確認"是否為重出(2014-0121新增)
                                (i.StorageDetailTypeId == 5) ?
                                ((i.FromStorage == Utility.GetShelfData((int)EnumData.StorageType.無貨儲位, _areaId).Shelf) ? "撿貨確認(重出)" : "撿貨確認") :
                                (i.FromStorage == "-") ? "盤點-無條件上架" :
                                //將上架/過帳/暫存回報差異-多少 區分開來 (2014-0123修改)
                                (i.StorageDetailTypeId == 6 && i.LogAccount == "_BOT_") ? "暫存回報差異" + (i.IsImport ? "-多" : "-少") :
                                (i.StorageDetailTypeId == 6) ? "驗貨入暫存" :
                                (i.StorageDetailTypeId == 7) ? "上架" :
                                //調出驗貨確認(2015-0211新增)
                                (i.StorageDetailTypeId == 11) ? "驗貨確認" :
                                (i.StorageDetailTypeId == 12) ? "上架(過帳)" :
                                (i.TargetStorage.StartsWith("export")) ? "出貨扣數" :
                                (i.TargetStorage == "-") ? "清除資料" : "移動產品",
                            }).ToList();

                gv_List_ActionLog.DataSource = temp;
                lbl_Count_ActionLog.Text = "資料筆數：" + temp.Count;

                gv_List_ActionLog.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region ○查詢ExportLog

        /// <summary>
        /// 查詢ExportLog
        /// </summary>
        protected void SearchExportLog()
        {
            try
            {
                var aa = sp.GetStorageExportLogByAll(SDay, EDay, WithoutDate, ticketId, shipoutId, productId, shopType, _areaId).ToList();

                int x = 1;
                var temp = (from i in aa
                            orderby i.ShipOutDate descending
                            select new
                            {
                                序號 = x++,
                                出貨日期 = i.ShipOutDate.ToString("yyyy-MM-dd"),
                                產品編號 = i.ProductId,
                                數量 = 1,
                                來源儲位編號 = i.FromShelfId,
                                目的儲位編號 = i.ToShelfId,
                                無貨 = i.IsLack,
                                樓層 = i.FromShelfFloor,
                                區域 = i.FromShelfArea,
                                撿貨區 = i.FromShelfGroup,
                                狀態 = i.Status,
                                刪單 = i.IsCancel.Value ? "是" : "否",
                                類別 = ((Utility.Store)i.OtherKey).ToString(),
                            }).ToList();

                gv_List_ExportLog.DataSource = temp;
                lbl_Count_ExportLog.Text = "資料筆數：" + temp.Count;

                gv_List_ExportLog.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region ○查詢ImportLog

        /// <summary>
        /// 查詢ImportLog
        /// </summary>
        protected void SearchImportLog()
        {
            try
            {
                var aa = sp.GetStorageImportLogByAll(SDay, EDay, WithoutDate, ticketId, productId, _areaId).ToList();

                int x = 1;
                var temp = (from i in aa
                            orderby i.Date descending
                            select new
                            {
                                序號 = x++,
                                日期 = i.Date.ToString("yyyy-MM-dd HH:mm"),
                                傳票號碼 = i.TicketId,
                                產品編號 = i.ProductId,
                                應驗數量 = i.Quantity,
                                已上架數量 = i.NowQuantity,
                                狀態 = (i.FlowStatus == 0) ? "未驗" : (i.FlowStatus == 1) ? "已驗" : (i.FlowStatus == 2) ? "上架(有效)" : "入帳(有效)",
                                暫存儲位編號 = (!admincheck) ? i.ShelfId : (String.IsNullOrEmpty(i.ShelfId)) ? "" : i.ShelfId + ", " + i.ToStorageId,
                                備註 = i.Comment
                            }).ToList();

                gv_List_ImportLog.DataSource = temp;
                lbl_Count_ImportLog.Text = "資料筆數：" + temp.Count;

                gv_List_ImportLog.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region ○查詢IssueReport

        /// <summary>
        /// 查詢IssueReport
        /// </summary>
        protected void SearchIssueReport()
        {
            try
            {
                var aa = sp.GetStorageIssueReportByAll(SDay, EDay, WithoutDate, ticketId, shipoutId, productId, shelfId, shopType, _areaId).ToList();

                int x = 1;
                var temp = (from i in aa
                            orderby i.CreateDate descending
                            select new
                            {
                                序號 = x++,
                                日期 = i.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                                傳票號碼 = i.TicketId,
                                出貨序號 = i.ShipOutId,
                                最後來源 = (i.BoxNum == "無貨") ? "印單無貨" : i.BoxNum, 
                                產品編號 = i.ProductId,
                                數量 = (i.IsQuantity) ? i.Quantity : -1 * i.Quantity,
                                買家帳號 = i.Account,
                                回報人 = i.CreateAuditor,
                                類別 = EnumData.GetFlowStatusName(i.FlowStatus),
                                狀態 = (!String.IsNullOrEmpty(i.FinishAuditor) ? "已結案" : !String.IsNullOrEmpty(i.HandleAuditot) ? "待處理" : "待結案"),
                                備註 = i.Comment,
                            }).ToList();

                gv_List_IssueReport.DataSource = temp;
                lbl_Count_IssueReport.Text = "資料筆數：" + temp.Count;

                gv_List_IssueReport.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region ○查詢StorageDetail

        /// <summary>
        /// 查詢StorageDetail
        /// </summary>
        protected void SearchStorageDetail()
        {
            try
            {
                var aa = sp.GetStorageDetailByAll(SDay, EDay, WithoutDate, productId, shelfId, _areaId).ToList();

                int x = 1;
                var temp = (from i in aa
                            orderby i.Date descending
                            select new
                            {
                                序號 = x++,
                                最後移動日期 = i.Date.ToString("yyyy-MM-dd HH:mm"),
                                產品編號 = i.ProductId,
                                數量 = i.Quantity,
                                儲位編號 = (!admincheck) ? i.ShelfId : (String.IsNullOrEmpty(i.ShelfId)) ? "" : i.ShelfId + ", " + i.StorageId,
                                儲位類別 = i.StorageType,
                                狀態 = i.Status,
                                備註 = i.Comment,
                            }).ToList();

                gv_List_StorageDetail.DataSource = temp;
                lbl_Count_StorageDetail.Text = "資料筆數：" + temp.Count;

                gv_List_StorageDetail.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #endregion
    }
}