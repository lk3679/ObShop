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
using System.Threading;
using OBShopWeb.Poslib;
using System.Configuration;

namespace OBShopWeb
{
    public partial class RequireAdd : System.Web.UI.Page
    {
        #region 宣告

        CheckFormat CF = new CheckFormat();
        ShelfProcess sp = new ShelfProcess();

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        private int _PrintPageSize = int.Parse(Area.WmsAreaXml("PrintPageSize"));
        private string _changeColor = Area.WmsAreaXml("changeColor");
        private string _changeColorOld = Area.WmsAreaXml("changeColor");
        private string _backgroundColor = Area.WmsAreaXml("backgroundColor");
        string IPaddress = HttpContext.Current.Request.UserHostAddress;
        String account;
        int numcount = 0;
        object temp = new object();
        DateTime SDay, EDay;
        String HandleStartDate, HandleEndDate, SerialId, ProductId, ShelfId, SearchType, templblCount;
        bool WithoutDate;

        List<serialtemp> slist = new List<serialtemp>();
        List<string> sOKlist = new List<string>();
        List<string> sYeslist = new List<string>();

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

                btn_Print.Text = "列印儲位明細(一張" + _PrintPageSize + "筆)，印單機：" + PosNumber.GetPrintMachineNo(IPaddress);

                templblCount = lbl_Count.Text;

                lblMsg.Text = "";
                lbl_Message.Text = "";
                lbl_Count.Text = "";
                lbl_ReportCount.Text = "";

                if (!IsPostBack)
                {
                    txt_Start.Text = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
                    txt_End.Text = DateTime.Today.ToString("yyyy-MM-dd");
                }

                HandleStartDate = txt_Start.Text.Trim();
                HandleEndDate = txt_End.Text.Trim();

                P_Report.Visible = (DDL_SearchType.SelectedItem.Text == "展售未上架");
                gv_List.Visible = (DDL_SearchType.SelectedItem.Text == "庫存銷售" || DDL_SearchType.SelectedItem.Text == "瑕疵" || DDL_SearchType.SelectedItem.Text == "問題件");
                
                P_SerialList.Visible = !CB_自動查儲位.Checked;
                
            }
        }

        #endregion

        #region ●主功能-查詢儲位內容

        /// <summary>
        /// 查詢按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_系列.Text.Trim()))
                {
                    SearchShelf();
                }
                else
                {
                    lbl_Message.Text = "請輸入正確範圍";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
                
        /// <summary>
        /// 執行查詢儲位產品
        /// </summary>
        protected void SearchShelf()
        {
            try
            {
                int x = 1;
                var Series = txt_系列.Text.Trim();
                List<string> 產品 = Series.Replace(" ", "").Replace("\r\n", ",").Replace("\n", ",").Split(',').ToList();

                CB_不良.Checked = (DDL_SearchType.SelectedItem.Text == "瑕疵" || DDL_SearchType.SelectedItem.Text == "問題件");
                int[] stype = { 0, 1 };
                int[] stype2 = { 5 };

                var temp = sp.GetRangeSearchProductByPId(產品, (CB_不良.Checked ? stype2 : stype), _areaId).ToList();

                var temp2 = (from i in temp
                             select new
                             {
                                 序號 = x++,
                                 產品編號 = i.ProductNumber,
                                 儲位編號 = i.ShelfId,
                                 數量 = i.Quantity,
                                 類型 = i.Id,
                             }).ToList();

                var 產品數 = temp2.Select(y => y.產品編號).Distinct().Count();
                var 總件數 = temp2.Select(y => y.數量).Sum();
                var 總筆數 = numcount = temp2.Count;

                gv_FList.DataSource = temp2;
                gv_FList.DataBind();

                lbl_FCount.Text = "產品數：" + 產品數 + ", 總筆數: " + 總筆數 + ", 總件數: " + 總件數;
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 匯出XLS

        /// <summary>
        /// XLS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnXls_Click(object sender, EventArgs e)
        {
            //先查詢
            btn_查詢報表_Click(sender, e);

            #region 設定

            var xls_filename = string.Format("{0}_儲位明細_【{1}筆】", DateTime.Now.ToString("yyyy-MMdd"), gv_FList.Rows.Count);

            CreateXLS CX = new CreateXLS();
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();

            //要輸出的欄位
            int[] list = { };

            #endregion

            //呼叫產生XLS
            CX.DoCreateXLS(workbook, ms, list.ToList(), gv_FList, false, "");

            //輸出
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + HttpUtility.UrlEncode(xls_filename, System.Text.Encoding.UTF8) + ".xls"));
            Response.BinaryWrite(ms.ToArray());
            workbook = null;
            ms.Close();
            ms.Dispose();
        }

        /// <summary>
        /// 產生報表SKU明細XLS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Xls2_Click(object sender, EventArgs e)
        {
            //清空gv
            gv_Report.DataSource = "";
            gv_Report.DataBind();

            //先查詢系列(為了過濾銷售總數)
            SearchFirst();

            var temp系列 = sp.GetSaleReportBySerial(ShelfId, SerialId, ProductId, SDay, EDay, int.Parse(SearchType), WithoutDate).ToList();

            var temp2系列 = (from i in temp系列
                           where ((DDL_SaleRange.SelectedValue == "0") ? i.銷售 == 0 :
                                   (DDL_SaleRange.SelectedValue == "1") ? i.銷售 >= 1 && i.銷售 <= 10 :
                                   (DDL_SaleRange.SelectedValue == "2") ? i.銷售 >= 10 && i.銷售 <= 20 :
                                   (DDL_SaleRange.SelectedValue == "3") ? i.銷售 >= 20 && i.銷售 <= 30 :
                                   (DDL_SaleRange.SelectedValue == "4") ? i.銷售 >= 30 && i.銷售 <= 50 :
                                   (DDL_SaleRange.SelectedValue == "5") ? i.銷售 >= 50 : true)
                                 &&
                                 (!CB_過濾在途回途.Checked || (i.回途庫存 == 0 && i.在途庫存 == 0))
                         select new
                         {
                             系列編號 = i.系列編號,
                         }).ToList();

            //再查詢產品並join出所需系列
            var tempSKU = sp.GetSaleReportByProduct(ShelfId, SerialId, ProductId, SDay, EDay, int.Parse(SearchType), WithoutDate).ToList();

            int x = 1;
            var temp2SKU = (from i in temp2系列
                            join ii in tempSKU on i.系列編號 equals ii.系列編號
                            where (!CB_過濾在途回途.Checked || (ii.回途庫存 == 0 && ii.在途庫存 == 0))
                         select new
                         {
                             序號 = x++,
                             系列編號 = ii.系列編號,
                             型號 = ii.型號,
                             品名 = ii.品名,
                             顏色 = ii.顏色,
                             尺寸 = ii.尺寸,
                             展售庫存 = ii.展售庫存,
                             補貨庫存 = ii.補貨庫存,
                             一般庫存 = ii.一般庫存,
                             暫存庫存 = ii.暫存庫存,
                             在途庫存 = ii.在途庫存,
                             回途庫存 = ii.回途庫存,
                             瑕疵庫存 = ii.瑕疵庫存,
                             銷售 = ii.銷售,
                             售價 = ii.售價,
                             上架日 = ii.上架日 ?? "無",
                         }).ToList();

            gv_Report.DataSource = temp2SKU;
            gv_Report.DataBind();

            #region 設定

            var xls_filename = string.Format("{0}_{1}_【{2}筆】", DateTime.Now.ToString("yyyy-MMdd") + "-【" + HandleStartDate + "~" + HandleEndDate + "】" 
                                            , DDL_SearchType.SelectedItem.Text, gv_Report.Rows.Count);

            CreateXLS CX = new CreateXLS();
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();

            //要輸出的欄位
            int[] list = { };

            #endregion

            //呼叫產生XLS
            CX.DoCreateXLS(workbook, ms, list.ToList(), gv_Report, false, "");

            //輸出
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + HttpUtility.UrlEncode(xls_filename, System.Text.Encoding.UTF8) + ".xls"));
            Response.BinaryWrite(ms.ToArray());
            workbook = null;
            ms.Close();
            ms.Dispose();
        }

        #endregion

        #region ●主功能-查報表

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_查詢報表_Click(object sender, EventArgs e)
        {
            CB_不良.Checked = (DDL_SearchType.SelectedItem.Text == "瑕疵" || DDL_SearchType.SelectedItem.Text == "問題件");
            SearchFirst();

            if (DDL_SearchType.SelectedItem.Text == "展售未上架")
                Search1();
            else
                Search2();

        }

        /// <summary>
        /// 查詢前先處理
        /// </summary>
        public void SearchFirst()
        {
            //查詢值
            SerialId = txt_SerialId.Text = txt_SerialId.Text.Trim();
            ProductId = txt_ProductId.Text = txt_ProductId.Text.Trim();
            ShelfId = txt_ShelfId.Text = txt_ShelfId.Text.Trim();
            SearchType = DDL_SearchType.SelectedValue;

            //日期參數
            SDay = DateTime.Parse(HandleStartDate);
            EDay = DateTime.Parse(HandleEndDate).AddDays(1);
            WithoutDate = CB_Date.Checked;
        }

        //系列排序
        public class serialtemp
        {
            public string 系列;
            public int 排序;
        }

        /// <summary>
        /// 查SKU
        /// </summary>
        public void Search1()
        {
            #region ●查SKU

            //清空gv
            gv_Report.DataSource = "";
            gv_Report.DataBind();

            var temp = sp.GetSaleReportByProduct(ShelfId, SerialId, ProductId, SDay, EDay, int.Parse(SearchType), WithoutDate).ToList();
            //過濾下架產品(2015-0212新增)
            var 下架產品 = sp.GetMoveFromSaleStock(DateTime.Today).ToList();

            var temp2 = (from i in temp
                         where ((CB_過濾補貨.Checked) ? i.展售庫存 == 0 && i.補貨庫存 == 0 : true)
                                 //&& //過濾下架產品(2015-0212新增)
                                 //(!CB_過濾展售下架.Checked || !下架產品.Contains(i.型號))
                                 &&
                                 ((DDL_SaleRange.SelectedValue == "0") ? i.銷售 == 0 :
                                 (DDL_SaleRange.SelectedValue == "1") ? i.銷售 >= 1 && i.銷售 <= 10 :
                                 (DDL_SaleRange.SelectedValue == "2") ? i.銷售 >= 10 && i.銷售 <= 20 :
                                 (DDL_SaleRange.SelectedValue == "3") ? i.銷售 >= 20 && i.銷售 <= 30 :
                                 (DDL_SaleRange.SelectedValue == "4") ? i.銷售 >= 30 && i.銷售 <= 50 :
                                 (DDL_SaleRange.SelectedValue == "5") ? i.銷售 >= 50 : true)
                                 &&
                                 (!CB_過濾在途回途.Checked || (i.回途庫存 == 0 && i.在途庫存 == 0))
                         select i).ToList();

            //過濾下架產品(2015-0212新增)
            if (CB_過濾展售下架.Checked)
                temp2 = (from i in temp2
                         join ii in 下架產品 on i.型號 equals ii into q
                         from ii in q.DefaultIfEmpty()
                         where ii == null
                         select i).ToList();


            #region ●將SKU展售有庫存或有銷售 整個系列排序提前

            slist = new List<serialtemp>();
            sOKlist = new List<string>();

            foreach (var item in temp2)
            {
                if (!sOKlist.Contains(item.系列編號))
                {
                    serialtemp stemp = new serialtemp();
                    stemp.系列 = item.系列編號;
                    stemp.排序 = 1;
                    if (item.展售庫存 > 0 || item.銷售 > 0)
                    {
                        slist.Add(stemp);
                        sOKlist.Add(item.系列編號);
                        sYeslist.Add(item.系列編號);
                    }
                }
            }

            foreach (var item in temp2)
            {
                if (!sOKlist.Contains(item.系列編號))
                {
                    serialtemp stemp = new serialtemp();
                    stemp.系列 = item.系列編號;
                    stemp.排序 = 2;
                    slist.Add(stemp);
                    sOKlist.Add(item.系列編號);
                }
            }

            #endregion

            int x = 1;
            var temp3 = (from i in temp2
                         join ii in slist on i.系列編號 equals ii.系列
                         orderby ii.排序, i.型號
                         select new
                         {
                             序號 = x++,
                             系列編號 = i.系列編號,
                             型號 = i.型號,
                             品名 = i.品名,
                             顏色 = i.顏色,
                             尺寸 = i.尺寸,
                             展售庫存 = i.展售庫存,
                             補貨庫存 = i.補貨庫存,
                             一般庫存 = i.一般庫存,
                             暫存庫存 = i.暫存庫存,
                             在途庫存 = i.在途庫存,
                             回途庫存 = i.回途庫存,
                             瑕疵庫存 = i.瑕疵庫存,
                             銷售 = i.銷售,
                             售價 = i.售價,
                             上架日 = i.上架日 ?? "無",
                         }).ToList();

            gv_Report.DataSource = temp3;
            gv_Report.DataBind();

            lbl_ReportCount.Text = "總筆數：" + temp3.Count + ", 優先上架系列數：" + sYeslist.Count;

            #endregion
        }

        /// <summary>
        /// 查系列
        /// </summary>
        public void Search2()
        {
            #region ●查系列

            gv_List.DataSource = "";
            gv_List.DataBind();

            var temp = sp.GetSaleReportBySerial(ShelfId, SerialId, ProductId, SDay, EDay, int.Parse(SearchType), WithoutDate).ToList();

            int x = 1;
            var temp2 = (from i in temp
                         where ((DDL_SaleRange.SelectedValue == "0") ? i.銷售 == 0 :
                                 (DDL_SaleRange.SelectedValue == "1") ? i.銷售 >= 1 && i.銷售 <= 10 :
                                 (DDL_SaleRange.SelectedValue == "2") ? i.銷售 >= 10 && i.銷售 <= 20 :
                                 (DDL_SaleRange.SelectedValue == "3") ? i.銷售 >= 20 && i.銷售 <= 30 :
                                 (DDL_SaleRange.SelectedValue == "4") ? i.銷售 >= 30 && i.銷售 <= 50 :
                                 (DDL_SaleRange.SelectedValue == "5") ? i.銷售 >= 50 : true)
                                 &&
                                 (!CB_過濾在途回途.Checked || (i.回途庫存 == 0 && i.在途庫存 == 0))
                         select new
                         {
                             序號 = x++,
                             系列編號 = i.系列編號,
                             品名 = i.品名,
                             展售庫存 = i.展售庫存,
                             一般庫存 = i.一般庫存,
                             暫存庫存 = i.暫存庫存,
                             在途庫存 = i.在途庫存,
                             回途庫存 = i.回途庫存,
                             瑕疵庫存 = i.瑕疵庫存,
                             銷售 = i.銷售,
                             售價 = i.售價,
                             營業額 = i.銷售 * i.售價,
                         }).ToList();

            gv_List.DataSource = temp2;
            gv_List.DataBind();

            var 系列數 = temp2.Count();
            var 展售數 = temp2.Select(y => y.展售庫存).Sum();
            var 一般數 = temp2.Select(y => y.一般庫存).Sum();
            var 暫存數 = temp2.Select(y => y.暫存庫存).Sum();
            var 在途數 = temp2.Select(y => y.在途庫存).Sum();
            var 瑕疵數 = temp2.Select(y => y.瑕疵庫存).Sum();
            var 銷售數 = temp2.Select(y => y.銷售).Sum();
            var 總營業額 = temp2.Select(y => y.營業額).Sum();

            lbl_Count.Text = "系列數：" + 系列數 + ", 展售數: " + 展售數 + ", 一般數: " + 一般數 + ", 暫存數: " + 暫存數 + ", 在途數: " + 在途數 +
                            ", 瑕疵數: " + 瑕疵數 + ", 銷售數: " + 銷售數 + ", 總營業額: " + 總營業額;

            CB_不良.Checked = (DDL_SearchType.SelectedItem.Text == "瑕疵");

            #endregion
        }

        #endregion

        #region ●主功能-匯出勾選產品

        /// <summary>
        /// 匯出勾選產品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_匯出勾選產品_Click(object sender, EventArgs e)
        {
            try 
            {
                var tempstr = "";

                if (DDL_SearchType.SelectedItem.Text == "庫存銷售" || DDL_SearchType.SelectedItem.Text == "瑕疵" || DDL_SearchType.SelectedItem.Text == "問題件")
                {
                    tempstr = GetCheckList1(gv_List);
                    btn_ADD.Enabled = !string.IsNullOrEmpty(tempstr);
                }
                else if (DDL_SearchType.SelectedItem.Text == "展售未上架")
                {
                    tempstr = GetCheckList2(gv_Report);
                    //btn_ADD.Enabled = false;
                }

                txt_系列.Text = tempstr;
                //btn_Print.Visible = false;

                if (!string.IsNullOrEmpty(tempstr) && CB_自動查儲位.Checked)
                {
                    SearchShelf();
                    //btn_Print.Visible = (gv_FList.Rows.Count > 0 && DDL_SearchType.SelectedItem.Text == "展售未上架");
                    btnXls.Visible = (gv_FList.Rows.Count > 0 && DDL_SearchType.SelectedItem.Text == "展售未上架");
                }
                else
                {
                    lbl_FCount.Text = "";
                    gv_FList.DataSource = "";
                    gv_FList.DataBind();
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 匯出產品(一般調出/瑕疵/問題件)
        /// </summary>
        /// <param name="gv"></param>
        public string GetCheckList1(GridView gv)
        {
            var tempstr = "";

            foreach (GridViewRow row in gv.Rows)
            {
                GridView gv_Detail = GetCellByName(row, "備註").FindControl("gv_Detail") as GridView;
                Panel P_Detail = GetCellByName(row, "備註").FindControl("P_Detail") as Panel;
                CheckBox CBSelect = GetCellByName(row, "備註").FindControl("CB_Detail_Select") as CheckBox;

                foreach (GridViewRow drow in gv_Detail.Rows)
                {
                    CheckBox CBtemp = GetCellByName(drow, "選擇").FindControl("CB_Detail") as CheckBox;
                    if (CBtemp.Checked)
                        tempstr += GetCellByName(drow, "型號").Text + "\r\n";

                }
            }

            return tempstr;
        }

        /// <summary>
        /// 匯出產品(展售未上架)
        /// </summary>
        /// <param name="gv"></param>
        public string GetCheckList2(GridView gv)
        {
            var tempstr = "";

            foreach (GridViewRow drow in gv.Rows)
            {
                CheckBox CBtemp = GetCellByName(drow, "選擇").FindControl("CB_Detail") as CheckBox;
                if (CBtemp.Checked)
                    tempstr += GetCellByName(drow, "型號").Text + "\r\n";
            }

            return tempstr;
        }

        /// <summary>
        /// gv_Detail_RowDataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_Detail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox CBtemp = GetCellByName(e.Row, "選擇").FindControl("CB_Detail") as CheckBox;
                //展售在途有數不給選---------------
                var 展售 = int.Parse(GetCellByName(e.Row, "展售").Text);
                var 在途 = int.Parse(GetCellByName(e.Row, "在途").Text);
                var 瑕疵 = int.Parse(GetCellByName(e.Row, "瑕疵").Text);

                if (DDL_SearchType.SelectedItem.Text == "庫存銷售")
                {
                    if ((展售 > 0 || 在途 > 0) && !CB_強制勾選.Checked)
                    {
                        CBtemp.Enabled = false;
                    }
                    else
                    {
                        CBtemp.Enabled = true;
                    }
                }
                else if (DDL_SearchType.SelectedItem.Text == "瑕疵" || DDL_SearchType.SelectedItem.Text == "問題件")
                {
                    if (瑕疵 > 0 || CB_強制勾選.Checked)
                    {
                        CBtemp.Enabled = true;
                    }
                    else
                    {
                        CBtemp.Enabled = false;
                    }
                }
                //---------------------------------
            }

            //光棒效果
            GvLightBar.lightbar(e, 3);
        }


        #endregion

        #region ●主功能-查詢Detail/全選/全不選

        /// <summary>
        /// Grid_RowCommand
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                var found = false;
                string Id = e.CommandArgument.ToString();

                if (!string.IsNullOrEmpty(Id))
                {
                    switch (e.CommandName)
                    {
                        case "Detail":

                            foreach (GridViewRow row in gv_List.Rows)
                            {
                                if (found)
                                    break;
                                var tempID = GetCellByName(row, "系列編號").Text;

                                //判斷是否是按下按鈕的那一行
                                if (tempID == Id)
                                {
                                    GridView gv_Detail = GetCellByName(row, "備註").FindControl("gv_Detail") as GridView;
                                    Panel P_Detail = GetCellByName(row, "備註").FindControl("P_Detail") as Panel;

                                    //if (!P_Detail.Visible)
                                    if (gv_Detail.Rows.Count == 0)
                                    {
                                        SearchFirst();
                                        var 報表s = sp.GetSaleReportByProduct(ShelfId, Id, "", SDay, EDay, int.Parse(SearchType), WithoutDate).ToList();

                                        int x = 1;
                                        var temp2 = (from i in 報表s
                                                     where (!CB_過濾在途回途.Checked || (i.回途庫存 == 0 && i.在途庫存 == 0))
                                                     select new
                                                     {
                                                         序號 = x++,
                                                         型號 = i.型號,
                                                         顏色 = i.顏色,
                                                         尺寸 = i.尺寸,
                                                         展售庫存 = i.展售庫存,
                                                         一般庫存 = i.一般庫存,
                                                         暫存庫存 = i.暫存庫存,
                                                         在途庫存 = i.在途庫存,
                                                         回途庫存 = i.回途庫存,
                                                         瑕疵庫存 = i.瑕疵庫存,
                                                         銷售 = i.銷售,
                                                         上架日 = i.上架日 ?? "無",
                                                     }).ToList();

                                        gv_Detail.DataSource = temp2;
                                        gv_Detail.DataBind();
                                    }

                                    P_Detail.Visible = !P_Detail.Visible;
                                    lbl_Count.Text = templblCount;
                                    found = true;
                                }
                            }

                            break;

                        case "CBDetail":
                            //CheckBox CBTemp = sender as CheckBox;
                            

                            foreach (GridViewRow row in gv_List.Rows)
                            {
                                if (found)
                                    break;
                                var tempID = GetCellByName(row, "系列編號").Text;

                                //判斷是否是按下按鈕的那一行
                                if (tempID == Id)
                                {
                                    GridView gv_Detail = GetCellByName(row, "備註").FindControl("gv_Detail") as GridView;
                                    Panel P_Detail = GetCellByName(row, "備註").FindControl("P_Detail") as Panel;
                                    CheckBox CBSelect = GetCellByName(row, "備註").FindControl("CB_Detail_Select") as CheckBox;
                                    CBSelect.Checked = !CBSelect.Checked;

                                    foreach (GridViewRow drow in gv_Detail.Rows)
                                    {
                                        CheckBox CBtemp = GetCellByName(drow, "選擇").FindControl("CB_Detail") as CheckBox;
                                        //展售在途有數不給選---------------
                                        var 展售 = int.Parse(GetCellByName(drow, "展售").Text);
                                        var 在途 = int.Parse(GetCellByName(drow, "在途").Text);
                                        var 瑕疵 = int.Parse(GetCellByName(drow, "瑕疵").Text);
                                        
                                        //---------------------------------

                                            if (DDL_SearchType.SelectedItem.Text == "庫存銷售")
                                            {
                                                if ((展售 == 0 && 在途 == 0) || CB_強制勾選.Checked)
                                                    CBtemp.Checked = CBSelect.Checked;
                                            }
                                            else if (DDL_SearchType.SelectedItem.Text == "瑕疵" || DDL_SearchType.SelectedItem.Text == "問題件")
                                            {
                                                if (瑕疵 > 0 || CB_強制勾選.Checked)
                                                    CBtemp.Checked = CBSelect.Checked;
                                            }
                                    }
                                    found = true;
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
                lbl_Message.Text = "讀取LOG記錄錯誤!!";
            }
        }

        /// <summary>
        /// 展售未上架全選/全不選
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CB_ReportSelect_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow drow in gv_Report.Rows)
            {
                CheckBox CBtemp = GetCellByName(drow, "選擇").FindControl("CB_Detail") as CheckBox;
                CBtemp.Checked = CB_ReportSelect.Checked;
            }
        }

        #endregion 主功能-查詢Detail/全選/全不選

        #region ●主功能-新增需求單

        /// <summary>
        /// 新增需求單 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_ADD_Click(object sender, EventArgs e)
        {
            try
            {
                //抓取目前儲位清單
                List<product> producttemp = new List<product>();
                foreach (GridViewRow row in gv_FList.Rows)
                {
                    var one = new product();
                    one.productid = GetCellByName(row, "產品編號").Text;
                    TextBox 調出數 = GetCellByName(row, "調出數").FindControl("txt_num") as TextBox;
                    one.quantity = (調出數 != null) ? int.Parse(調出數.Text) : 0;

                    //數量防呆
                    if (one.quantity <= 0)
                    {
                        lblMsg.Text = one.productid + " 調出數量不正確!";
                        return;
                    }

                    producttemp.Add(one);
                }
                //統計總數
                var temp = (from i in producttemp
                            group i by new { i.productid } into g
                            select new product
                            {
                                productid = g.Key.productid,
                                quantity = g.Sum(x => x.quantity),
                            }).ToList();

                //轉成List<string>
                List<string> ProductList = new List<string>();
                foreach (var item in temp)
                {
                    ProductList.Add(item.productid + "," + item.quantity);
                }

                //分為 一般調出 /瑕疵 /問題件
                var requireType = 0;
                if (DDL_SearchType.SelectedItem.Text == "瑕疵")
                    requireType = 1;
                else if (DDL_SearchType.SelectedItem.Text == "問題件")
                    requireType = 2;

                //新增需求單
                var result = RequireDA.SetRequireProduct(ProductList, requireType, account, _areaId);
                lblMsg.Text = result.Reason;

                if (result.Result == "1")
                {
                    btn_ADD.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        private class product
        {
            public string productid;
            public int quantity;
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
            txt_SerialId.Text = "";
            txt_ProductId.Text = "";
        }

        #region ○RowDataBound

        protected void gv_Report_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //判定row的型態是資料行
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (DDL_SearchType.SelectedItem.Text == "展售未上架")
                {
                    //優先上架打勾/變色
                    var temp = GetCellByName(e.Row, "系列編號");
                    if (sYeslist.Contains(temp.Text))
                    {
                        temp.ForeColor = System.Drawing.Color.Red;

                        CheckBox CBtemp = GetCellByName(e.Row, "選擇").FindControl("CB_Detail") as CheckBox;
                        CBtemp.Checked = true;
                    }
                }
            }

            //光棒效果
            GvLightBar.lightbar(e, 1);
        }

        protected void gv_List_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            //光棒效果
            GvLightBar.lightbar(e, 2);
        }

        protected void gv_FList_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            //光棒效果
            GvLightBar.lightbar(e, 1);
        }

        #endregion

        #endregion

        #region ●主功能-列印儲位明細

        /// <summary>
        /// 列印儲位明細
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Print_Click(object sender, EventArgs e)
        {
            try
            {
                lblMsg.Text = "";
                //列印儲位明細
                var p = new OBShopWeb.Poslib.Print();
                //抓取目前儲位清單
                List<TicketShelfTemp> pickList = new List<TicketShelfTemp>();
                List<TicketShelfTemp> One = new List<TicketShelfTemp>();
                foreach (GridViewRow row in gv_FList.Rows)
                {
                    var one = new TicketShelfTemp();
                    one.ProductId = GetCellByName(row, "產品編號").Text;
                    TextBox 調出數 = GetCellByName(row, "調出數").FindControl("txt_num") as TextBox;
                    one.Quantity = (調出數 != null) ? int.Parse(調出數.Text) : 0;
                    one.Division = GetCellByName(row, "儲位編號").Text;

                    //數量防呆
                    if (one.Quantity < 0)
                    {
                        lblMsg.Text = one.ProductId + " 調出數量不正確!";
                        return;
                    }

                    if(one.Quantity > 0)
                        pickList.Add(one);
                }
                //測試500
                //pickList = pickList.Take(120).ToList();

                var listCount = pickList.Count;
                var xxi = 0;
                //web.config設定每頁幾筆
                var pernum = _PrintPageSize;
                while (xxi * pernum < listCount)
                {
                    One = pickList.Skip(xxi * pernum).Take(pernum).ToList();
                    xxi++;
                    p.PrintPickDetail(One, "", "儲位明細" + xxi.ToString("D2"));
                    Thread.Sleep(1000);
                }

                lblMsg.Text = "列印成功!";
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

    }
}