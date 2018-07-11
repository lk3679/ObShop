using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using POS_Library.DB;
using POS_Library.Public;

namespace OBShopWeb
{
    public partial class NoProductCheck : System.Web.UI.Page
    {
        #region 宣告

        private string account;
        private string auditor = string.Empty;
        string PageKey;

        #endregion 宣告

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
                    auditor = account = Session["Account"].ToString();
                    PageKey = Request["PageKey"] ?? "";

                    if (!IsPostBack)
                    {
                        if (Request["pickNum"] == null)
                        {
                            throw new Exception("資料錯誤！");
                        }
                        Default();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        public void Default()
        {
            #region reuqes

            //單號
            var pickNum = Request["pickNum"];
            //產品狀態
            var productStatus = int.Parse(Request["productStatus"]);
            ViewState["productStatus"] = productStatus;
            //產品資訊
            var noProductData = Request["noProductData"];
            var shipAccount = Request["account"];
            //產品原因
            var reason = int.Parse(Request["reason"]);

            lblPickNum.Text = pickNum;
            var pick = Utility.GetPickNumRegex(pickNum);
            var num = pick.Number;
            var store = pick.Store;
            var areaId = pick.Area;
            var pickType = pick.PickType;

            var data = noProductData.Split(',');
            var fromShelf = data[0];
            var productId = data[1];
            var quantity = int.Parse(data[2]);
            var lackQuantity = data[3];

            #endregion reuqes

            lbl_Message.Text = "";
            string comment = "印單回報:" + ((POS_Library.ShopPos.NoProduct.EnumReportStatus)productStatus).ToString() + "，原因:" + ((POS_Library.ShopPos.NoProduct.EnumReportReason)reason).ToString();

            lbl_Quantity.Text = lackQuantity;
            lbl_Account.Text = "";// shipAccount;
            lbl_Product.Text = productId;
            lbl_Shelf.Text = fromShelf;

            //組連結(2014-0325新增)
            //SetHLink(lbl_TicketId.Text, shipId, productId, storeType.ToString());

            //檢查是否有回報過
            var issue = POS_Library.ShopPos.NoProduct.GetIssueReport(int.Parse(num), store, pickType).Where(x => x.ProductId == productId && x.FlowStatus != (int)POS_Library.ShopPos.EnumData.FlowType.補貨確認).ToList();
            //回報未結案
            var notFinish = issue.Count(x => string.IsNullOrEmpty(x.FinishAuditor));
            var issueFinish = issue.Count(x => x.FinishAuditor != null);
             
            lblComment.Text = comment;  
            if ((pickType == (int)Utility.ShipPDF.寄倉調出))
            {
                lblNumName.Text = "傳票號碼：";
                lblNum.Text = num;
                lbl_Allot.Text = "調出";
                if (fromShelf == "無貨")
                {
                    lblMsg.Text = "此產品已回報完畢不可再回報！！！";
                    btn_Report.Visible = false;
                }
                else
                {
                    lblMsg.Text = "此調出單不可有建議儲位！！！";
                }
                return;
            }
            else
            {
                lblNumName.Text = "出貨號碼：";
                lbl_Allot.Text = "出貨";
                lblNum.Text = num;
            }
            //未結案條件：
            //1.完全無回報紀錄
            //2.有回報紀錄但未結案
            //3.補貨完成可重新回報
            //if (issue.Count == 0 || notFinish > 0)
            //{

            #region 開始建議
             
            var nowProduct = POS_Library.ShopPos.Storage.GetNowProduct(fromShelf, productId, areaId, productStatus);
            nowProduct = nowProduct.Take(int.Parse(lackQuantity)).ToList();
            var soldList = nowProduct.Select(x => new
            {
                ShelfTypeName = x.Storage.StorageType.Name,
                NewStorage = x.Storage.ShelfId
            }).ToList();
            //是否有建議到儲位
            if (soldList.Any() || fromShelf == "無貨")
            {

                btn_Report.Visible = false;
                lblComment.Text = "撿貨回報建議";
                gv_List.DataSource = soldList.Select(x => new
                {
                    儲位類型 = x.ShelfTypeName,
                    建議儲位 = x.NewStorage
                }).ToList();
                gv_List.DataBind();

                #region 補貨與舊儲位目前應該沒用到

                foreach (GridViewRow iRow in gv_List.Rows)
                {
                    Button tb = (Button)iRow.FindControl("Btn_Execution");
                    //撿查建議儲位是否為舊儲位
                    //var old = Regex.Match(iRow.Cells[0].Text, @"(【舊儲位】.*.區)(\w.*)").Groups[2].Value;
                    //撿查目前儲位是否為補貨儲位
                    var storages = POS_Library.ShopPos.Storage.GetStorage(fromShelf, areaId);
                    if (storages != null)
                    {
                        if (storages.StorageTypeId == (int)POS_Library.ShopPos.EnumData.StorageType.補貨儲位)
                        {
                            tb.Enabled = true;
                            btn_Report.Visible = false;
                        }
                    }
                    else if ((Regex.IsMatch(soldList.First().NewStorage, @"(【舊儲位】.*.區)") && fromShelf == soldList.First().NewStorage) || Regex.Match(soldList.First().NewStorage, @"(【舊儲位】.*.區)(\w.*)").Groups[2].Value == fromShelf)
                    {
                        //暫時開放
                        tb.Enabled = false;
                        lblMsg.Text = "無建議儲位";
                    }
                }

                #endregion 補貨與舊儲位目前應該沒用到

                //暫時開放
                //btn_Report.Visible = true;
                //}
                //else
                //{
                //lbl_Quantity.Text = soldList.Count.ToString();
                if (lackQuantity != soldList.Count.ToString())
                {
                    lblMsg.Text = "缺" + (int.Parse(lackQuantity) - soldList.Count) + "件";
                }

                //}
            }
            else
            {

                btn_Report.Visible = true;
                if (fromShelf == "無貨")
                {
                    lblMsg.Text = "此人無建議儲位且回報過！！";
                }
                else
                {
                    lblMsg.Text = "此人無建議儲位！！";
                }
            }

            #endregion 開始建議

            //}
            //else
            //{
            //    lblMsg.Text = "此人已結案！不可再回報！";
            //}
        }

        #endregion Page_Load

        #region 主功能-執行回報確認(有建議儲位)

        /// <summary>
        ///有建議儲位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                var pick = Utility.GetPickNumRegex(lblPickNum.Text);
                var num = int.Parse(pick.Number);
                var store = pick.Store;
                var areaId = pick.Area;
                var pickType = pick.PickType;
                int productStatus = int.Parse(ViewState["productStatus"].ToString());
                var ticketId = 0;
                var shipId = 0;
                if (pickType == (int)Utility.ShipPDF.寄倉調出)
                {
                    ticketId = num;
                }
                else
                {
                    shipId = num;
                }

                var shelf = lbl_Shelf.Text.Trim();
                var productId = lbl_Product.Text.Trim();
                string comment = lblComment.Text;

                string newShelf = e.CommandArgument.ToString();
                var isIssue = (shelf == "無貨");
                var issueId = "補檢確認用";
                switch (e.CommandName)
                {
                    case "btnEdit":

                        //檢查來原儲位是否為新儲位
                        var oldStorage = POS_Library.ShopPos.Storage.GetStorage(shelf, pick.Area);
                        bool fromNewStorage = oldStorage != null ? true : false;

                        //檢查建議儲位是否為新儲位
                        var newStorage = POS_Library.ShopPos.Storage.GetStorage(newShelf, pick.Area);
                        bool targetNewStorage = newStorage != null ? true : false;
                        //////儲位名稱：無貨   原因：撿貨  沒有無貨回報
                        //////儲位名稱：無貨   原因：補撿建議  有無貨回報
                        if (targetNewStorage)
                        {
                            //來源是否無貨
                            if (!fromNewStorage)
                            {
                                //來源無貨
                                int expType = (int)POS_Library.ShopPos.EnumData.ExportType.無貨建議儲位;
                                if (Regex.IsMatch(shelf, @"補貨"))
                                {
                                    expType = (int)POS_Library.ShopPos.EnumData.ExportType.補貨建議儲位;
                                }
                                //無貨換新儲位(將建議儲位交換 )
                                POS_Library.ShopPos.NoProduct.無貨換新儲位(newShelf, ticketId, shipId, store, productId, account, pick.Area, shelf, expType, isIssue, comment, issueId);

                                lblMsg.Text = "回報成功!";
                            }
                            else
                            {
                                //來源有儲位(需打銷)
                                int expType = (int)POS_Library.ShopPos.EnumData.ExportType.撿貨建議儲位;
                                if (isIssue)
                                {
                                    expType = (int)POS_Library.ShopPos.EnumData.ExportType.無貨建議儲位;
                                }
                                 
                                if (comment == "未回報" || comment == "回報錯誤")
                                {
                                    expType = (int)POS_Library.ShopPos.EnumData.ExportType.驗貨建議儲位;
                                }
                                POS_Library.ShopPos.NoProduct.新儲位換新儲位(newShelf, ticketId, shipId, store, productId, account, pick.Area, shelf, expType, productStatus);
                            }
                            lblMsg.Text = "建議成功!";
                        }
                        else
                        {
                            lblMsg.Text = "無效目的儲位!";
                        }
                        break;

                    case "btnCancel":

                        break;
                }
                var tb = (Button)e.CommandSource;
                tb.Enabled = false;
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-執行回報確認(有建議儲位)

        #region 主功能-執行無貨回報確認(按鈕)(刪單)

        /// <summary>
        /// 確定無貨回報
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Report_Click(object sender, EventArgs e)
        {
            try
            {
                int productStatus = int.Parse(ViewState["productStatus"].ToString());
                var pick = Utility.GetPickNumRegex(lblPickNum.Text);
                var num = int.Parse(pick.Number);
                var store = pick.Store;
                var areaId = pick.Area;
                var pickType = pick.PickType;
                string oldShelf = lbl_Shelf.Text;
                string productId = lbl_Product.Text;
                int lackquantity = int.Parse(lbl_Quantity.Text);
                string account = lbl_Account.Text;
                string comment = lblComment.Text;
                var issurReports = POS_Library.ShopPos.NoProduct.GetIssueReport(num, store, pickType).Where(x => x.FinishDate == null && x.FlowStatus != (int)POS_Library.ShopPos.EnumData.FlowType.補貨確認).ToList();
                var expTicketLogs = POS_Library.ShopPos.NoProduct.GetExpTicketLogs(num, store, pickType).Where(x => x.ProductId == productId).ToList();

                if (Regex.IsMatch(oldShelf, @"無貨"))
                {
                    var expNoProductQ = expTicketLogs.Count(x => string.IsNullOrEmpty(x.FromShelfId));
                    //未結案+目前回報數量 不可大於 目前是無貨數量
                    if ((issurReports.Count + lackquantity) > expNoProductQ)
                    {
                        lblMsg.Text = "此問題已回報!!";
                        return;
                    }
                }
                else
                {
                    if ((issurReports.Count + lackquantity) > expTicketLogs.Count)
                    {
                        //if (issurReports.First().Comment == "瑕疵")
                        //{
                        //    var shelf = WMS_Library.Public.Utility.GetShelfData((int)WMS_Library.ProductStorage.EnumData.StorageType.不良儲位, int_treasurytype);
                        //    lblMsg.Text = "此問題已回報!!請將此產品放置不良儲位【" + shelf.Shelf + "】";
                        //}
                        //else
                        //{
                        //    var shelf = WMS_Library.Public.Utility.GetShelfData((int)WMS_Library.ProductStorage.EnumData.StorageType.無貨儲位, int_treasurytype);
                        //    lblMsg.Text = "此問題已回報!!請將此產品放置無貨儲位【" + shelf.Shelf + "】";
                        //}
                        lblMsg.Text = "此問題已回報!!";
                        return;
                    }
                }

                //檢查來原儲位是否為新儲位
                var oldStorage = POS_Library.ShopPos.Storage.GetStorage(oldShelf, areaId);
                bool fromNewStorage = oldStorage != null ? true : false;
                var error = false;
                if (expTicketLogs.Count > 0)
                {
                    //for (int i = 1; i <= quantity; i++)
                    //{
                    //新增無貨
                    var issue = new IssueReport();
                    var issueFlow = 0;
                    var ticketId = 0;
                    var shipId = 0;
                    if (pickType == (int)Utility.ShipPDF.寄倉調出)
                    {
                        ticketId = num;
                        issueFlow = (int)POS_Library.ShopPos.EnumData.FlowType.調出;
                    }
                    else
                    {
                        shipId = num;
                        issue.TicketId = num;
                        issue.ShipOutId = num;
                        issueFlow = (int)POS_Library.ShopPos.EnumData.FlowType.出貨;
                    }

                    //var issFlowType = POS_Library.ShopPos.EnumData.FlowType.出貨;
                    //if ((productStatus == (int)POS_Library.ShopPos.NoProduct.EnumReportStatus.瑕疵))
                    //{
                    //    issFlowType = POS_Library.ShopPos.EnumData.FlowType.瑕疵;
                    //}
                    issue.Id = POS_Library.Public.Utility.GetGuidMD5();
                    issue.TicketId = ticketId;
                    issue.ShipOutId = shipId;
                    issue.BoxNum = fromNewStorage ? oldShelf : "無貨";
                    issue.ProductId = productId;
                    issue.Quantity = lackquantity;
                    issue.IsQuantity = false;
                    issue.ShopType = store;
                    issue.Account = "";
                    issue.CreateAuditor = auditor;
                    issue.CreateDate = DateTime.Now;
                    issue.Comment = comment;

                    //調出/出貨
                    issue.FlowStatus = issueFlow;

                    var result = POS_Library.ShopPos.NoProductDA.SetDiff(issue, num, pickType,  areaId, fromNewStorage, auditor, productStatus);

                    lblMsg.Text = result.Reason;
                    //有出錯標記
                    if (result.Result == "0")
                        error = true;
                }
                if (error)
                {
                    lblMsg.Text += " 此問題回報發生錯誤，請重新回報!!";
                }
                else
                {
                    btn_Report.Visible = false;
                    if (pickType == (int)Utility.ShipPDF.寄倉調出 && PageKey == "ShipOutVerify")
                    {
                        var url = "ShipOutVerify.aspx?tick=" + num + "&area=" + areaId + "&store=" + store;
                        Page.RegisterClientScriptBlock("checkinput", @"<script>alert('回報成功!'); window.open('" + url + "','_self');</script>");
                        //}
                    }
                }
                
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-執行無貨回報確認(按鈕)(刪單)

        #region 組連結(建議歷史記錄)(2014-0325新增)

        /// <summary>
        /// 組連結(建議歷史記錄)(2014-0325新增)
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="shipId"></param>
        /// <param name="productId"></param>
        protected void SetHLink(string ticketId, string shipId, string productId, string shoptype)
        {
            var url = "";
            url = "NoProductCheckHistory.aspx?ticketId=" + ticketId + "&shipId=" + shipId + "&productId=" + productId + "&shoptype=" + shoptype;
            HL_無貨建議記錄.NavigateUrl = url;
        }

        #endregion 組連結(建議歷史記錄)(2014-0325新增)
    }
}