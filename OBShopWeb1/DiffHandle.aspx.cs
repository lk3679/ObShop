using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OBShopWeb.PDA; 
using POS_Library;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class DiffHandle : System.Web.UI.Page
    {
        #region 宣告

        private setup auth = new setup();

        //bool result;

        private CheckFormat CF = new CheckFormat();
        private ShelfProcess sp = new ShelfProcess();

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        private String account;

        //WCF執行結果
        private List<MsgStatus> MSList = new List<MsgStatus>();

        private List<ShelfConfig> SCList = new List<ShelfConfig>();

        private Button btn_Cancel;
        private Button btn_Handle;
        private Button btn_Confirm, btn_Confirm2, btn_Confirm3;
        private Button btn_Finish;
        private LinkButton LBtn_Temp;
        private DataControlFieldCell TempCell;
        private HyperLink HL_Temp;

        private POS_Library.DB.IssueReport issue;
        private String HandleType, HandleStartDate, HandleEndDate, HandleTicketID;

        private MsgStatus result;

        private bool SearchTicket = false;
        private bool Pos = false;

        #endregion 宣告

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Request["Pos"] != null && Request["Pos"].ToString() == "4")
                {
                    lbl_Pos.Visible = Pos = true;
                    Page.Header.Title += lbl_Pos.Text;
                    DDL_Type.Enabled = !Pos;
                }

                if (Session["Account"] == null)
                {
                    string url = " <script> parent.document.location= 'logout.aspx" + (Pos ? "?urlx=DiffHandle.aspx?Pos=4" : "") + "' </script> ";
                    Response.Write(url);
                    Response.End();
                }
                else
                {
                    if (!IsPostBack)
                    {
                        txt_Start.Text = DateTime.Today.ToString("yyyy-MM-dd");
                        txt_End.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    }
                    account = Session["Account"].ToString();
                    lbl_Message.Text = "";
                    lbl_Count.Text = "";

                    HandleType = DDL_Type.SelectedValue;
                    HandleStartDate = txt_Start.Text.Trim();
                    HandleEndDate = txt_End.Text.Trim();
                    HandleTicketID = txt_Ticket.Text.Trim();
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion Page_Load

        #region 介面

        /// <summary>
        /// gv換頁設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_List_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                //gv_List.PageIndex = e.NewPageIndex;
                //gv_List.DataSource = (DataTable)ViewState["dt"];
                //gv_List.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

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

        #endregion 介面

        #region ●主功能-查詢
         protected enum EnumSearchType
         {
             傳票查詢 =1,
             日期查詢 = 2,

         }
        /// <summary>
        /// 查詢入庫調出差異化
        /// </summary>
        protected void Search(int searchType)
        {
            try
            {
                int storeType = POS_Library.Public.Utility.GetStoreForShop(_areaId);
                var temp = POS_Library.ShopPos.Storage.GetIssueReports("", storeType).ToList();
                var ticketId = txt_Ticket.Text.Trim();
                var sDate = DateTime.Parse(txt_Start.Text.Trim());
                var eDate = DateTime.Parse(txt_End.Text.Trim());
                //類別
                var type = DDL_Type.SelectedValue;
                switch (searchType)
                {
                    case 1: //傳票號碼
                        temp = temp.Where(x => x.TicketId == int.Parse(ticketId)).ToList();
                        break;
                          
                    case 2:

                        #region 類別

                        switch (type)
                        {
                            case "-1":
                                temp = temp.Where(x => x.FlowStatus != 12 && x.FlowStatus != 1).ToList();
                                break;

                            case "1":
                                temp = temp.Where(x => x.FlowStatus == 1 || x.FlowStatus == 6 || x.FlowStatus == 9).ToList();
                                break;

                            case "2":
                                temp = temp.Where(x => x.FlowStatus == 2 || (x.FlowStatus == 11 && x.Comment == "海運")).ToList();
                                break;

                            case "11":
                                temp = temp.Where(x => x.FlowStatus == 11 && x.Comment != "海運").ToList();
                                break;

                            case "9":
                                temp = temp.Where(x => x.FlowStatus == 3 || x.FlowStatus == 4 || x.FlowStatus == 5 || x.FlowStatus == 9).ToList();
                                break;

                            default:
                                temp = temp.Where(x => x.FlowStatus == int.Parse(type)).ToList();
                                break;
                        }

                        #endregion 類別

                        #region 狀態

                        if (DDL_Status.SelectedValue != "-1")
                        {
                            switch (DDL_Status.SelectedValue)
                            {
                                case "待處理":
                                    temp = temp.Where(x => !string.IsNullOrEmpty(x.CreateAuditor) && string.IsNullOrEmpty(x.HandleAuditot) && string.IsNullOrEmpty(x.FinishAuditor)).ToList();
                                    break;

                                case "已處理":
                                    temp = temp.Where(x => !string.IsNullOrEmpty(x.HandleAuditot) && string.IsNullOrEmpty(x.FinishAuditor)).ToList();
                                    break;

                                case "已結案":
                                    temp = temp.Where(x => !string.IsNullOrEmpty(x.FinishAuditor)).ToList();
                                    break;
                            }
                        }

                        #endregion 狀態

                        #region 日期範圍

                        if (!CB_Date.Checked)
                        {
                            temp = temp.Where(x => x.CreateDate >= sDate && x.CreateDate <= eDate.AddDays(1)).ToList();
                        }
                        else
                        {
                            temp = temp.Where(x => x.CreateDate >= DateTime.Now.AddMonths(-6) && x.CreateDate <= DateTime.Now.AddDays(1)).ToList();
                        }

                        #endregion 日期範圍

                        break;
                }

                var temp2 = (from i in temp
                             orderby i.CreateDate descending
                             select new
                             {
                                 Id = i.Id,
                                 類別 = (i.FlowStatus == 1) ? EnumData.FlowType.出貨.ToString() :
                                     (i.FlowStatus == 2) ? EnumData.FlowType.海運.ToString() :
                                     (i.FlowStatus == 3) ? EnumData.FlowType.寄倉.ToString() :
                                     (i.FlowStatus == 4) ? EnumData.FlowType.版借出.ToString() :
                                     (i.FlowStatus == 5) ? EnumData.FlowType.瑕疵.ToString() :
                                     (i.FlowStatus == 6) ? "補撿" :
                                     (i.FlowStatus == 7) ? EnumData.FlowType.拆單重出.ToString() :
                                     (i.FlowStatus == 8) ? EnumData.FlowType.換貨.ToString() :
                                     (i.FlowStatus == 9) ? EnumData.FlowType.調出.ToString() :
                                     (i.FlowStatus == 11) ? (i.Comment == "海運") ? "海運" : "調回" :
                                     (i.FlowStatus == 12) ? EnumData.FlowType.強制移無貨儲位.ToString() :
                                     (i.FlowStatus == 13) ? EnumData.FlowType.台直.ToString() :
                                     (i.FlowStatus == 16) ? EnumData.FlowType.門市調出.ToString() :
                                     (i.FlowStatus == 17) ? EnumData.FlowType.福袋打銷.ToString() :
                                     (i.FlowStatus == 18) ? EnumData.FlowType.台組進貨.ToString() :
                                     (i.FlowStatus == 19) ? EnumData.FlowType.門市調回.ToString() :
                                     (i.FlowStatus == 20) ? EnumData.FlowType.版還.ToString() :
                                     (i.FlowStatus == 21) ? EnumData.FlowType.門市進貨.ToString() :
                                     (i.FlowStatus == 28) ? EnumData.FlowType.門市進貨.ToString() :
                                     (i.FlowStatus == 99) ? EnumData.FlowType.出貨結案.ToString() : "其他",
                                 傳票 = i.TicketId,
                                 箱號 = i.BoxNum + ((i.FlowStatus != 1) ? ((i.FlowStatus == 2 || i.FlowStatus == 18) ? "" :
                                         string.IsNullOrEmpty(i.BoxNum) ? "" :
                                         (i.FlowStatus == 9) ? "" : "(暫存)") : ""),  
                                 調出廠商 = "", //Utility.GetOneTicketOutById(i.TicketId),
                                 帳號 = i.Account,
                                 出貨序號 = i.ShipOutId,
                                 產品編號 = i.ProductId, 
                                 數量 = ((i.IsQuantity == true) ? "" : "-") + i.Quantity,
                                 回報人 = i.CreateAuditor,
                                 回報日期 = i.CreateDate.ToString("yyyy/MM/dd HH:mm"),
                                 處理人 = (i.HandleAuditot == "_BOT_") ? "系統" : i.HandleAuditot,
                                 處理日期 = i.HandleDate.HasValue ? i.HandleDate.Value.ToString("yyyy/MM/dd HH:mm") : "",
                                 結案人 = (i.FinishAuditor == "_BOT_") ? "系統" : i.FinishAuditor,
                                 結案日期 = i.FinishDate.HasValue ? i.FinishDate.Value.ToString("yyyy/MM/dd HH:mm") : "",
                                 //待回報,已處理,已結案
                                 處理狀態 = getStatus(i.CreateAuditor, i.HandleAuditot, i.FinishAuditor),
                                 備註 = ((i.FlowStatus == 2 && (i.Comment != "" && i.Comment != null)) ? i.Comment : ""),
                                 備註HU = "",//sp.GetComment(i.Id, (int)EnumData.ServiceType.傳票確認),
                                 功能 = "",
                             }).ToList();

                gv_List.DataSource = temp2;
                gv_List.DataBind();

                lbl_Count.Text = "資料筆數：" + gv_List.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion ●主功能-查詢

        #region 副功能-判斷狀態

        /// <summary>
        /// 判斷狀態
        /// </summary>
        /// <param name="CreateAuditor"></param>
        /// <param name="HandleAuditot"></param>
        /// <param name="FinishAuditor"></param>
        /// <returns></returns>
        protected String getStatus(String CreateAuditor, String HandleAuditot, String FinishAuditor)
        {
            try
            {
                if (FinishAuditor == null || FinishAuditor == "")
                {
                    if (HandleAuditot == null || HandleAuditot == "")
                    {
                        if (CreateAuditor != null && CreateAuditor != "")
                        {
                            return "待處理";
                        }
                        else
                        {
                            return "無回報人";
                        }
                    }
                    else
                    {
                        return "已處理";
                    }
                }
                else
                {
                    return "已結案";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
                return "錯誤";
            }
        }

        /// <summary>
        /// NameToType
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        protected int NameToType(String Type)
        {
            switch (Type)
            {
                case "出貨": return 1;
                case "海運": return 2;
                case "寄倉": return 3;
                case "版借出": return 4;
                case "瑕疵": return 5;
                case "調出": return 9;
                case "調回": return 11;
                case "台直": return 13;
                default: return 0;
            }
        }

        #endregion 副功能-判斷狀態

        #region ●主功能-取消回報/處理/傳票確認/結案

        #region ○取消回報

        /// <summary>
        /// 取消回報
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;

                foreach (GridViewRow row in gv_List.Rows)
                {
                    Button btnTest = GetCellByName(row, "功能").FindControl("btn_Cancel") as Button;

                    //判斷是否是按下按鈕的那一行
                    if (btnTest == btn)
                    {
                        issue = new POS_Library.DB.IssueReport();

                        //傳票
                        //TempCell = GetCellByName(row, "傳票");
                        //issue.TicketId = int.Parse(TempCell.Text);
                        //代入參數(2013-0313修改)
                        issue.Id = btnTest.Attributes["ticket"];

                        if (issue.FlowStatus == (int)EnumData.FlowType.調出 || issue.FlowStatus == (int)EnumData.FlowType.門市進貨)//門市台組退倉
                        {
                            if (issue.TicketId == null)
                            {
                                lbl_Message.Text = "傳票不可為空!!";
                                return;
                            }
                        }
                        //取消回報
                        POS_Library.ShopPos.Storage.SetExportTicket(issue.Id, _areaId);
                        var result = sp.UpdateIssueReportsEnd(issue.Id, account, DateTime.Now, (int)POS_Library.ShopPos.EnumData.FlowType.補貨確認);

                        if (result != null)
                        {
                            lbl_Message.Text = result.Reason;

                            //成功則改狀態
                            if (result.Result == "1")
                            {
                                btnTest.Visible = false;
                                Button btnTemp = GetCellByName(row, "功能").FindControl("btn_Finish") as Button;
                                btnTemp.Visible = true;

                                GetCellByName(row, "處理狀態").Text = "已處理";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion ○取消回報

        #region ○處理

        /// <summary>
        /// 處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Handle_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;

                foreach (GridViewRow row in gv_List.Rows)
                {
                    Button btnTest = GetCellByName(row, "功能").FindControl("btn_Handle") as Button;

                    //判斷是否是按下按鈕的那一行
                    if (btnTest == btn)
                    {
                        issue = new POS_Library.DB.IssueReport();

                        //傳票
                        //TempCell = GetCellByName(row, "傳票");
                        //issue.TicketId = int.Parse(TempCell.Text);
                        //代入參數(2013-0313修改)
                        issue.TicketId = int.Parse(btnTest.Attributes["ticket"]);

                        //帳號
                        TempCell = GetCellByName(row, "帳號");
                        issue.Account = (TempCell.Text == "" || TempCell.Text == "&nbsp;") ? "" : TempCell.Text;

                        //出貨序號
                        TempCell = GetCellByName(row, "出貨序號");
                        if (TempCell.Text != "" && TempCell.Text != "&nbsp;")
                        {
                            issue.ShipOutId = int.Parse(TempCell.Text);
                        }

                        //產品編號
                        TempCell = GetCellByName(row, "產品編號");
                        issue.ProductId = TempCell.Text;

                        //數量/多缺
                        TempCell = GetCellByName(row, "數量");
                        if (int.Parse(TempCell.Text) > 0)
                        {
                            issue.Quantity = int.Parse(TempCell.Text);
                            issue.IsQuantity = true;
                        }
                        else
                        {
                            issue.Quantity = int.Parse(TempCell.Text) * -1;
                            issue.IsQuantity = false;
                        }

                        //倉庫類別
                        issue.ShopType = (int)POS_Library.Public.Utility.Store.橘熊;

                        //回報人
                        TempCell = GetCellByName(row, "回報人");
                        issue.HandleAuditot = TempCell.Text;

                        //回報日期
                        TempCell = GetCellByName(row, "回報日期");
                        issue.HandleDate = DateTime.Parse(TempCell.Text);

                        //處理人
                        issue.HandleAuditot = account;

                        //處理日期
                        issue.HandleDate = DateTime.Now;

                        //類別
                        TempCell = GetCellByName(row, "類別");
                        issue.FlowStatus = NameToType(TempCell.Text);

                        //類別
                        TempCell = GetCellByName(row, "Id");
                        issue.Id = TempCell.Text;
                        if (issue.FlowStatus == (int)EnumData.FlowType.調出)
                        {
                            if (issue.TicketId == null)
                            {
                                lbl_Message.Text = "調出傳票不可為空!!";
                                return;
                            }
                        }

                        //寫入issueReport
                        result = sp.UpdateIssueReports(issue);

                        if (result != null)
                        {
                            lbl_Message.Text = result.Reason;

                            //成功則改狀態
                            if (result.Result == "1")
                            {
                                btnTest.Visible = false;
                                Button btnTemp = GetCellByName(row, "功能").FindControl("btn_Finish") as Button;
                                btnTemp.Visible = true;
                                GetCellByName(row, "處理狀態").Text = "已處理";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion ○處理

       
 
        #region ○結案

        /// <summary>
        /// 結案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Finish_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;

                foreach (GridViewRow row in gv_List.Rows)
                {
                    Button btnTest = GetCellByName(row, "功能").FindControl("btn_Finish") as Button;

                    //判斷是否是按下按鈕的那一行
                    if (btnTest == btn)
                    {
                        issue = new POS_Library.DB.IssueReport();

                        //傳票
                        //TempCell = GetCellByName(row, "傳票");
                        //issue.TicketId = int.Parse(TempCell.Text);
                        //代入參數(2013-0313修改)
                        issue.TicketId = int.Parse(btnTest.Attributes["ticket"]);

                        //帳號
                        TempCell = GetCellByName(row, "帳號");
                        issue.Account = (TempCell.Text == "" || TempCell.Text == "&nbsp;") ? "" : TempCell.Text;

                        //出貨序號
                        TempCell = GetCellByName(row, "出貨序號");
                        if (TempCell.Text != "" && TempCell.Text != "&nbsp;")
                        {
                            issue.ShipOutId = int.Parse(TempCell.Text);
                        }

                        //產品編號
                        TempCell = GetCellByName(row, "產品編號");
                        issue.ProductId = TempCell.Text;

                        //數量/多缺
                        TempCell = GetCellByName(row, "數量");
                        if (int.Parse(TempCell.Text) > 0)
                        {
                            issue.Quantity = int.Parse(TempCell.Text);
                            issue.IsQuantity = true;
                        }
                        else
                        {
                            issue.Quantity = int.Parse(TempCell.Text) * -1;
                            issue.IsQuantity = false;
                        }

                        //倉庫類別
                        issue.ShopType = POS_Library.Public.Utility.GetStoreForShop(_areaId);

                        //回報人
                        TempCell = GetCellByName(row, "回報人");
                        issue.HandleAuditot = TempCell.Text;

                        //回報日期
                        TempCell = GetCellByName(row, "回報日期");
                        issue.HandleDate = DateTime.Parse(TempCell.Text);

                        //處理人
                        issue.HandleAuditot = account;

                        //處理日期
                        issue.HandleDate = DateTime.Now;

                        //結案人
                        TempCell = GetCellByName(row, "結案人");
                        issue.FinishAuditor = account;

                        //結案日期
                        TempCell = GetCellByName(row, "結案日期");
                        issue.FinishDate = DateTime.Now;

                        //類別
                        TempCell = GetCellByName(row, "類別");
                        issue.FlowStatus = NameToType(TempCell.Text);

                        //類別
                        TempCell = GetCellByName(row, "Id");
                        issue.Id = TempCell.Text;
                        if (issue.FlowStatus == (int)EnumData.FlowType.調出)
                        {
                            if (issue.TicketId == null)
                            {
                                lbl_Message.Text = "調出傳票不可為空!!";
                                return;
                            }
                            else
                            {
                                //SetDelTicket(issue.TicketId.Value, issue.ProductId);
                            }
                        }

                        //寫入issueReport
                        result = sp.UpdateIssueReportsEnd(issue);
                        lbl_Message.Text = "結案成功!!";
                        if (result != null)
                        {
                            lbl_Message.Text = result.Reason;

                            //成功則改狀態
                            if (result.Result == "1")
                            {
                                btnTest.Visible = false;
                                GetCellByName(row, "處理狀態").Text = "已結案";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion ○結案
         
        #endregion ●主功能-取消回報/處理/傳票確認/結案

        #region gv_List_RowDataBound

        /// <summary>
        /// gv_List_RowDataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_List_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                String str_Status;

                if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
                {
                    GetCellByName(e.Row, "備註HU").Visible = false;
                    //GetCellByName(e.Row, "處理狀態").Visible = false;
                    GetCellByName(e.Row, "Id").Visible = false;
                    str_Status = GetCellByName(e.Row, "處理狀態").Text.Trim();

                    TempCell = GetCellByName(e.Row, "類別");

                    if ((HandleType == "-1" && !SearchTicket) || HandleType == "1")
                    {
                        GetCellByName(e.Row, "功能").Visible = false;
                    }
                    else
                    {
                        GetCellByName(e.Row, "功能").Visible = true;
                    }

                    var temp類別 = GetCellByName(e.Row, "類別").Text;

                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        if (temp類別 == "門市進貨")
                        {
                            //備註整理(2013-1007新增)================================
                            string HuComment = GetCellByName(e.Row, "備註HU").Text;
                            TempCell = GetCellByName(e.Row, "備註");
                            TempCell.Text += ((HuComment == "&nbsp;") ? ((TempCell.Text == "&nbsp;") ? "" : " HU:無") : " HU:" + HuComment);
                            //=======================================================

                            if (str_Status == "待處理")
                            {
                                btn_Cancel = (Button)GetCellByName(e.Row, "功能").FindControl("btn_Cancel");
                                btn_Cancel.Visible = true;
                                btn_Handle = (Button)GetCellByName(e.Row, "功能").FindControl("btn_Handle");
                                btn_Handle.Visible = false;
                            }
                            else if (str_Status == "已處理")
                            {
                                //btn_Confirm = (Button)GetCellByName(e.Row, "功能").FindControl("btn_Confirm");
                                //btn_Confirm.Visible = true;
                                btn_Finish = (Button)GetCellByName(e.Row, "功能").FindControl("btn_Finish");
                                btn_Finish.Visible = true;
                            }
                            else if (str_Status == "已結案")
                            {
                                //GetCellByName(e.Row, "功能").Text = "已結案";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion gv_List_RowDataBound

        #region 主功能-查傳票

        /// <summary>
        /// 查傳票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Ticket_Search_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(HandleTicketID))
                {
                    SearchTicket = true;
                    Search((int)EnumSearchType.傳票查詢);
                }
                else
                {
                    lbl_Message.Text = "傳票號碼不可為空!";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-查傳票
         

        #region 主功能-查詢全部

        /// <summary>
        /// 執行查詢
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                Search((int)EnumSearchType.日期查詢);
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-查詢全部

        #region 匯出XLS(2013-0930新增)

        /// <summary>
        /// XLS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Xls_Click(object sender, EventArgs e)
        {
            CreateXLS();
        }

        /// <summary>
        /// CreateXLS
        /// </summary>
        protected void CreateXLS()
        {
            #region 設定

            //檔名
            string xls_typeName, xls_date, xls_filename;

            xls_date = DateTime.Today.ToString("yyyy-MMdd");

            xls_typeName = "TW差異處理";
            xls_filename = xls_date + "_" + xls_typeName;

            CreateXLS CX = new CreateXLS();
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();

            //要輸出的欄位
            int[] list = { };

            #endregion 設定

            if (gv_List.Rows.Count > 0)
            {
                foreach (GridViewRow i in gv_List.Rows)
                {
                    var tempCell = GetCellByName(i, "傳票");
                    System.Web.UI.WebControls.HyperLink HL = tempCell.FindControl("TicketId") as System.Web.UI.WebControls.HyperLink;
                    tempCell.Text = HL.Text;
                }

                //呼叫產生XLS
                CX.DoCreateXLS(workbook, ms, list.ToList(), gv_List, false, "");

                //輸出
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + HttpUtility.UrlEncode(xls_filename, System.Text.Encoding.UTF8) + ".xls"));
                Response.BinaryWrite(ms.ToArray());
                workbook = null;
                ms.Close();
                ms.Dispose();
            }
        }

        #endregion 匯出XLS(2013-0930新增)
    }
}