using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using WMS_Library.ProductStorage;
//using WMS_Library.ProductStorage.DataModel;
//using WMS_Library.Public;
using System.Web.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using POS_Library;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb.PDA
{
    public partial class StorageReport : System.Web.UI.Page
    {
        #region 宣告

        setup auth = new setup();
        //bool result;

        CheckFormat CF = new CheckFormat();
        ShelfProcess sp = new ShelfProcess();

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        String account;

        //WCF執行結果
        List<MsgStatus> MSList = new List<MsgStatus>();
        List<ShelfConfig> SCList = new List<ShelfConfig>();

        String HandleType, HandleStartDate, HandleEndDate;

        Button btn_Handle;
        Button btn_Finish;
        LinkButton LBtn_Temp;
        HyperLink HL_Temp;
        String Function91 = "91", Function92 = "92", Function93 = "93", Function94 = "94", Function95 = "95",
            Function98 = "98", Function99 = "99", Function101 = "101", Function102 = "102", Function103 = "103",
            Function104 = "104", Function105 = "105", Function106 = "106", Function107 = "107", Function108 = "108",
            Function109 = "109", Function111 = "111", Function112 = "112";

        System.Drawing.Color tempColor = System.Drawing.Color.DarkRed;
        String str_tempbox = "";

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= '../logout.aspx' </script> ");
                    Response.End();
                }
                else
                {
                    if (!IsPostBack)
                    {
                        txt_Start.Text = DateTime.Today.ToString("yyyy-MM-dd");
                        txt_End.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    }

                    account = Session["Name"].ToString();

                    HandleType = DDL_Type.SelectedValue;
                    HandleStartDate = txt_Start.Text.Trim();
                    HandleEndDate = txt_End.Text.Trim();

                    lbl_Message.Text = "";
                    lbl_Count.Text = "";

                    String choice = DDL_Choice.SelectedValue;

                    #region ●日期隱藏(2013-0829新增)

                    //日期隱藏(2013-0829新增)
                    if (choice != Function94)
                    {
                        PL_Date.Visible = true;
                        CB_Date.Visible = true;

                        //無貨報表只能選一天
                        if (choice == Function95 || choice == Function103)
                        {
                            txt_End.Visible = false;
                            CB_Date.Enabled = CB_Date.Checked = false;
                        }
                    }
                    else
                    {
                        PL_Date.Visible = false;
                        CB_Date.Visible = false;
                        //btn_XLS.Visible = true;
                    }

                    #endregion

                    #region ●樓層(2013-0430新增)

                    //樓層(2013-0430新增)
                    if (choice != Function91 && choice != Function92 && choice != Function93 && choice != Function94
                        && choice != Function98 && choice != Function99 && choice != Function101 && choice != Function102
                        && choice != Function103 && choice != Function104 && choice != Function105 && choice != Function106
                        && choice != Function107 && choice != Function108 && choice != Function109 && choice != Function111 && choice != Function112)
                    {
                        ////查問題無貨儲位 自動勾 新儲位格式
                        //CB_NewFormat.Checked = (choice == Function5);
                        //if(DDL_Area.Items.Count == 0)
                        //    CB_NewFormat_CheckedChanged(sender, e);

                        //分新舊
                        if (CB_NewFormat.Checked)
                        {
                            lbl_Floor.Visible = true;
                            DDL_Floor.Visible = false;
                            lbl_Floor.Text = "&nbsp;區域：";
                            DDL_Area.Visible = true;
                        }
                        else
                        {
                            lbl_Floor.Visible = true;
                            DDL_Floor.Visible = true;
                            lbl_Floor.Text = "&nbsp;樓層/區域：";

                            if (DDL_Floor.SelectedValue != "-1")
                                DDL_Area.Visible = true;
                            else
                                DDL_Area.Visible = false;
                        }
                    }
                    else
                    {
                        lbl_Floor.Visible = false;
                        DDL_Floor.Visible = false;
                        DDL_Area.Visible = false;
                    }

                    #endregion

                    #region ●選擇百分比/差異處理狀態

                    //選擇百分比------------------------------
                    if (choice != Function92 && choice != Function93)
                    {
                        lbl_smaller.Visible = false;
                        DDL_Percent.Visible = false;
                    }
                    else
                    {
                        lbl_smaller.Visible = true;
                        DDL_Percent.Visible = true;
                    }

                    //差異處理狀態
                    if (choice != Function91)
                    {
                        lbl_Type.Visible = false;
                        lbl_Status.Visible = false;
                        DDL_Type.Visible = false;
                        DDL_Status.Visible = false;
                    }
                    else
                    {
                        lbl_Type.Visible = true;
                        lbl_Status.Visible = true;
                        DDL_Type.Visible = true;
                        DDL_Status.Visible = true;
                    }
                    //-----------------------------------------

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 介面

        #region ●切換

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
        /// 換樓層(2013-0430)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Floor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //分新舊
                if (DDL_Floor.SelectedValue != "-1" && !CB_NewFormat.Checked)
                {
                    DDL_Area.DataSource = sp.GetArea(_areaId, DDL_Floor.SelectedValue);
                    DDL_Area.DataBind();
                }
                else if (CB_NewFormat.Checked)
                {
                    DDL_Area.DataSource = sp.GetAreaAll(_areaId);
                    DDL_Area.DataBind();
                }
                else
                {
                    DDL_Area.Visible = false;
                }
                ListItem aa = new ListItem();
                aa.Text = "全部";
                aa.Value = "-1";
                aa.Selected = true;
                DDL_Area.Items.Add(aa);
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 切換新舊儲位格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CB_NewFormat_CheckedChanged(object sender, EventArgs e)
        {
            DDL_Floor_SelectedIndexChanged(sender, e);

            ClearGV();
        }

        /// <summary>
        /// 切目錄選單
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Choice_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearGV();

            if (DDL_Choice.SelectedValue == "107")
            {
                txt_Start.Text = DateTime.Today.Year + "-01-01";
                txt_End.Text = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            }

            if (DDL_Choice.SelectedValue == "107" || DDL_Choice.SelectedValue == "109")
            {
                btn_Search.Enabled = false;
            }
            else
            {
                btn_Search.Enabled = true;
            }
        }

        /// <summary>
        /// 清除GV
        /// </summary>
        protected void ClearGV()
        {
            gv_List.DataSource = "";
            gv_List.DataBind();
        }

        #endregion

        #endregion

        #region ●主功能-查詢

        #region ※查詢選擇

        /// <summary>
        /// 執行查詢
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                switch (DDL_Choice.SelectedValue)
                {
                    //儲位查詢 Search-----------------------
                    //普通
                    case "0": Search("0"); break;
                    //打銷
                    case "1": Search("9"); break;
                    //無貨
                    case "2": Search("10"); break;
                    //問題
                    case "3": Search("4"); break;
                    //不良
                    case "4": Search("5"); break;
                    //補貨
                    case "5": Search("2"); break;
                    //散貨
                    case "6": Search("1"); break;
                    //過季
                    case "7": Search("3"); break;

                    //暫存
                    case "8": Search("6"); break;
                    //問題暫存
                    case "9": Search("8"); break;
                    //不良暫存
                    case "10": Search("7"); break;
                    //海運暫存
                    case "11": Search("14"); break;
                    //換貨暫存
                    case "12": Search("15"); break;
                    //散貨暫存
                    case "13": Search("16"); break;
                    //調回暫存
                    case "14": Search("17"); break;
                    //預購暫存
                    case "15": Search("18"); break;
                    //--------------------------------------------

                    //無貨回報儲位列表
                    case "95": Search95(); break;
                    //入庫無差異無貨產品(2013-1033新增)
                    case "98": Search98(); break;
                    //Search101 無貨有庫存列表(2013-1205新增)
                    case "101": Search101(); break;
                    //Search102 盤點打銷出貨相關(2013-1205新增)
                    case "102": Search102(); break;
                    //Search103 出貨無貨列表(2013-1206新增)
                    case "103": Search103(); break;
                    //Search104 打銷產品列表(2014-0107新增)
                    case "104": Search104(); break;
                    //Search111 展售上架時間(2015-0331新增)
                    case "111": Search111(); break;
                    //Search112 總倉展售款式差異(2015-0408新增)
                    case "112": Search112(); break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region ●Search1 儲位查詢

        /// <summary>
        /// 儲位查詢
        /// </summary>
        protected void Search(String stype)
        {
            try
            {
                var str_floor = (CB_NewFormat.Checked) ? "" : DDL_Floor.SelectedValue;
                var str_area = (str_floor == "-1") ? "-1" : DDL_Area.SelectedValue;

                //新舊儲位格式(2013-0829修改)
                if (CB_NewFormat.Checked)
                {
                    SCList = sp.GetStorageByTypeNew(str_floor, str_area, int.Parse(stype), _areaId).ToList();

                    int x = 1;
                    var temp = (from i in SCList
                                //日期範圍
                                where ((CB_Date.Checked) ? true :
                                (i.Date >= DateTime.Parse(HandleStartDate) && i.Date <= DateTime.Parse(HandleEndDate).AddDays(1)))
                                &&
                                    //區域(2013-0430新增)
                                ((str_area == "-1" || str_area == "") ? true : i.StorageId.Substring(0, 1) == str_area)
                                //group i by i.ProductNumber into j
                                orderby i.Date descending
                                //orderby i.ProductNumber
                                select new
                                {
                                    序號 = x++,
                                    日期 = i.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                                    儲位編號 = i.StorageId,
                                    產品編號 = i.ProductNumber,
                                    數量 = 1,
                                }).ToList();

                    int y = 1;
                    var temp2 = temp.GroupBy(a => new { a.日期, a.產品編號, a.儲位編號 }).Select(b => new { 序號 = y++, b.Key.日期, b.Key.儲位編號, b.Key.產品編號, 數量 = b.Sum(bb => bb.數量) }).ToList();

                    gv_List.DataSource = temp2;
                    lbl_Count.Text = "資料筆數：" + temp2.Count + ", 總數量：" + temp2.Sum(c => c.數量).ToString();
                }
                else
                {
                    SCList = sp.GetStorageByType(str_floor, str_area, int.Parse(stype), _areaId).ToList();

                    int x = 1;
                    var temp = (from i in SCList
                                //日期範圍
                                where ((CB_Date.Checked) ? true :
                                (i.Date >= DateTime.Parse(HandleStartDate) && i.Date <= DateTime.Parse(HandleEndDate).AddDays(1)))
                                &&
                                    //樓層(2013-0430新增)
                                ((str_floor == "-1") ? true : i.StorageId.Substring(0, 1) == str_floor)
                                &&
                                    //區域(2013-0430新增)
                                ((str_area == "-1" || str_area == "") ? true : i.StorageId.Substring(1, 1) == str_area)
                                //group i by i.ProductNumber into j
                                orderby i.Date descending
                                //orderby i.ProductNumber
                                select new
                                {
                                    序號 = x++,
                                    日期 = i.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                                    儲位編號 = i.StorageId,
                                    產品編號 = i.ProductNumber,
                                    數量 = 1,
                                }).ToList();

                    int y = 1;
                    var temp2 = temp.GroupBy(a => new { a.日期, a.產品編號, a.儲位編號 }).Select(b => new { 序號 = y++, b.Key.日期, b.Key.儲位編號, b.Key.產品編號, 數量 = b.Sum(bb => bb.數量) }).ToList();

                    gv_List.DataSource = temp2;
                    lbl_Count.Text = "資料筆數：" + temp2.Count + ", 總數量：" + temp2.Sum(c => c.數量).ToString();
                }

                gv_List.DataBind();
                gv_List.Width = 700;
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region ●Search95 無貨回報儲位列表 (2013-1011新增)

        /// <summary>
        /// 無貨回報儲位列表 GetIssueStorage
        /// </summary>
        protected void Search95()
        {
            var temp = sp.GetIssueStorage(DateTime.Parse(HandleStartDate), DateTime.Parse(HandleStartDate), _areaId).Select(x => x.ShelfId).OrderBy(x => x);
            int si = 1;
            var temp2 = (from i in temp
                         //where (DDL_Area.Items.Count == 0 || DDL_Area.SelectedValue == "-1") ? true : i.Substring(0, 1) == DDL_Area.SelectedValue
                         where ((!CB_NewFormat.Checked && DDL_Floor.SelectedValue == "-1") || (CB_NewFormat.Checked && DDL_Area.SelectedValue == "-1")) ? true :
                         //新儲位格式
                         CB_NewFormat.Checked ? i.Substring(0, 1) == DDL_Area.SelectedValue :
                         //舊儲位格式 + Area有值
                         (DDL_Area.Items.Count > 0 && DDL_Area.SelectedValue != "-1") ? i.Substring(0, 1) == DDL_Floor.SelectedValue && i.Substring(1, 1) == DDL_Area.SelectedValue :
                         //(DDL_Area.Items.Count == 0 || DDL_Area.SelectedValue == "-1") 
                         i.Substring(0, 1) == DDL_Floor.SelectedValue
                         select new
                         {
                             序號 = si++,
                             回報無貨儲位 = i,
                         }).ToList();

            gv_List.DataSource = temp2;
            gv_List.DataBind();

            lbl_Count.Text = "總筆數：" + gv_List.Rows.Count;

            gv_List.Width = 500;
        }

        #endregion

        #region ●Search98 入庫無差異無貨產品(2013-1033新增)

        /// <summary>
        /// 入庫無差異無貨產品
        /// </summary>
        protected void Search98()
        {
            try
            {
                var temp = sp.GetIssueProductDiff(DateTime.Parse(HandleStartDate), DateTime.Parse(HandleEndDate), _areaId).ToList();

                int si = 1;
                var temp2 = temp.GroupBy(x => new { 產品編號 = x.ProductId }).Select(a => new
                {
                    序號 = si++,
                    產品編號 = a.Key.產品編號,
                    數量 = a.Count(),
                }).ToList();

                gv_List.DataSource = temp2;
                gv_List.DataBind();

                lbl_Count.Text = "總筆數：" + gv_List.Rows.Count;

                gv_List.Width = 300;
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region ●Search101 無貨有庫存列表(2013-1205新增)

        /// <summary>
        /// 無貨有庫存列表 GetIssueButHaveStock
        /// </summary>
        protected void Search101()
        {
            var temp = sp.GetIssueButHaveStock(DateTime.Parse(HandleStartDate), DateTime.Parse(HandleEndDate).AddDays(1), _areaId).ToList();
            int si = 1;
            var temp2 = (from i in temp
                         where i.產品狀態 != "入庫上架"
                         select new
                         {
                             序號 = si++,
                             產品編號 = i.產品編號,
                             儲位編號 = i.儲位編號,
                             產品狀態 = i.產品狀態,
                             刪單 = i.刪單? "是" : "否",
                         }).ToList();

            gv_List.DataSource = temp2;
            gv_List.DataBind();

            var 總筆數 = temp2.Count;
            var 產品數 = temp2.Select(x => x.產品編號).Distinct().Count();

            lbl_Count.Text = "總筆數：" + 總筆數 + ", 產品數：" + 產品數;

            gv_List.Width = 600;
        }

        #endregion

        #region ●Search102 盤點打銷與出貨相關(2013-1205新增)

        /// <summary>
        /// 盤點打銷與出貨相關 GetIssueButHaveStock
        /// </summary>
        protected void Search102()
        {
            var temp = sp.GetInventoryEffectShipOut(DateTime.Parse(HandleStartDate), DateTime.Parse(HandleEndDate).AddDays(1), _areaId).ToList();
            int si = 1;
            var temp2 = (from i in temp
                         orderby i.類別, i.產品編號
                         select new
                         {
                             序號 = si++,
                             產品編號 = i.產品編號,
                             出貨日期 = i.出貨日期.ToString("yyyy-MM-dd"),
                             來源儲位 = string.IsNullOrEmpty(i.來源儲位) ? "無" : i.來源儲位,
                             出貨暫存儲位 = i.出貨暫存儲位,
                             類別 = i.類別,
                         }).ToList();

            gv_List.DataSource = temp2;
            gv_List.DataBind();

            var 總筆數 = temp2.Count;
            var 產品數 = temp2.Select(x => x.產品編號).Distinct().Count();
            var 總包裹數 = temp2.Select(x => x.出貨暫存儲位).Distinct().Count();
            var 官網 = temp2.Where(x => x.類別 == "官網").Select(x => x.出貨暫存儲位).Distinct().Count();
            var 官網無貨 = temp2.Where(x => x.類別 == "官網" && x.來源儲位 == "無").Select(x => x.出貨暫存儲位).Distinct().Count();
            var 橘熊 = temp2.Where(x => x.類別 == "橘熊").Select(x => x.出貨暫存儲位).Distinct().Count();
            var 橘熊無貨 = temp2.Where(x => x.類別 == "橘熊" && x.來源儲位 == "無").Select(x => x.出貨暫存儲位).Distinct().Count();
            var 無貨包裹數 = temp2.Where(x => x.來源儲位 == "無").Select(x => x.出貨暫存儲位).Distinct().Count();

            lbl_Count.Text = "總筆數：" + 總筆數 + ", 產品數：" + 產品數 + ", 總包裹數：" + 總包裹數 +
                ", 官網：【" + 官網無貨 + " / " + 官網 + "】, 橘熊：【" + 橘熊無貨 + " / " + 橘熊 + "】, 無貨包裹數：" + 無貨包裹數;

            gv_List.Width = 800;
        }

        #endregion

        #region ●Search103 出貨無貨列表(2013-1206新增)

        /// <summary>
        /// 出貨無貨列表 GetIssueButHaveStock
        /// </summary>
        protected void Search103()
        {
            var temp = sp.GetIssueNoProduct(DateTime.Parse(HandleStartDate), DateTime.Parse(HandleEndDate), _areaId).ToList();
            int si = 1;
            var temp2 = (from i in temp
                         orderby i.類別, i.產品編號
                         select new
                         {
                             序號 = si++,
                             出貨日期 = i.出貨日期.ToString("yyyy-MM-dd"),
                             出貨暫存儲位 = i.出貨暫存儲位,
                             產品編號 = i.產品編號,
                             類別 = i.類別,
                         }).ToList();

            gv_List.DataSource = temp2;
            gv_List.DataBind();

            var 總筆數 = temp2.Count;
            var 產品數 = temp2.Select(x => x.產品編號).Distinct().Count();
            var 官網無貨 = temp2.Where(x => x.類別 == "官網").Select(x => x.出貨暫存儲位).Distinct().Count();
            var 橘熊無貨 = temp2.Where(x => x.類別 == "橘熊").Select(x => x.出貨暫存儲位).Distinct().Count();
            var 無貨包裹數 = temp2.Select(x => x.出貨暫存儲位).Distinct().Count();

            lbl_Count.Text = "總筆數：" + 總筆數 + ", 產品數：" + 產品數 +
                ", 官網：【" + 官網無貨 + "】, 橘熊：【" + 橘熊無貨 + "】, 無貨包裹數：" + 無貨包裹數;

            gv_List.Width = 800;
        }

        #endregion

        #region ●Search104 打銷產品列表(2014-0107新增)

        /// <summary>
        /// 打銷產品列表 GetDisappearProduct
        /// </summary>
        protected void Search104()
        {
            var shelf = POS_Library.Public.Utility.GetShelfData(9, _areaId);
            var temp = sp.GetDisappearProduct(DateTime.Parse(HandleStartDate), DateTime.Parse(HandleEndDate).AddDays(1), shelf.Shelf, _areaId).ToList();
            int si = 1;
            var temp2 = (from i in temp
                         orderby i.類別, i.產品編號
                         select new
                         {
                             序號 = si++,
                             產品編號 = i.產品編號,
                             類別 = i.類別,
                             數量 = i.數量
                         }).ToList();

            gv_List.DataSource = temp2;
            gv_List.DataBind();

            var 總筆數 = temp2.Count;
            var 產品數 = temp2.Select(x => x.產品編號).Distinct().Count();
            var 盤點打銷產品 = temp2.Where(x => x.類別 == "盤點打銷").Select(x => x.產品編號).Distinct().Count();
            var 盤點打銷數 = temp2.Where(x => x.類別 == "盤點打銷").Select(x => x.數量).Sum();
            var 入庫打銷產品 = temp2.Where(x => x.類別 == "入庫打銷").Select(x => x.產品編號).Distinct().Count();
            var 入庫打銷數 = temp2.Where(x => x.類別 == "入庫打銷").Select(x => x.數量).Sum();
            var 出貨打銷產品 = temp2.Where(x => x.類別 == "出貨打銷").Select(x => x.產品編號).Distinct().Count();
            var 出貨打銷數 = temp2.Where(x => x.類別 == "出貨打銷").Select(x => x.數量).Sum();
            var 調出打銷產品 = temp2.Where(x => x.類別 == "調出打銷").Select(x => x.產品編號).Distinct().Count();
            var 調出打銷數 = temp2.Where(x => x.類別 == "調出打銷").Select(x => x.數量).Sum();

            lbl_Count.Text = "總筆數：" + 總筆數 + ", 產品數：" + 產品數 + ", (產品數/總數) 盤點：" + 盤點打銷產品 + "/" + 盤點打銷數
                                + ", 入庫：" + 入庫打銷產品 + "/" + 入庫打銷數
                                + ", 出貨：" + 出貨打銷產品 + "/" + 出貨打銷數
                                + ", 調出：" + 調出打銷產品 + "/" + 調出打銷數;

            gv_List.Width = 800;
        }

        #endregion

        #region ●Search111 展售上架時間(2015-0331新增)

        /// <summary>
        /// 展售上架時間 GetSaleShelfDate
        /// </summary>
        protected void Search111()
        {
            var temp = sp.GetSaleShelfDate().ToList();
            int si = 1;
            var temp2 = (from i in temp
                         orderby i.系列編號
                         select new
                         {
                             序號 = si++,
                             系列編號 = i.系列編號,
                             產品編號 = i.產品編號,
                             時間 = i.時間.ToString("yyyy-MM-dd HH:mm:ss")
                         }).ToList();

            gv_List.DataSource = temp2;
            gv_List.DataBind();

            var 總筆數 = temp2.Count;
            var 產品數 = temp2.Select(x => x.產品編號).Distinct().Count();
            var 系列數 = temp2.Select(x => x.系列編號).Distinct().Count();

            lbl_Count.Text = "總筆數：" + 總筆數 + ", 產品數：" + 產品數 + ", 系列數：" + 系列數;

            gv_List.Width = 800;
        }

        #endregion

        #region ●Search112 總倉展售款式差異(2015-0408新增)

        /// <summary>
        /// 盤點打銷與出貨相關 GetDisappearProduct
        /// </summary>
        protected void Search112()
        {
            var temp = sp.GetDiffSerial().ToList();
            int si = 1;
            var temp2 = (from i in temp
                         select new
                         {
                             序號 = si++,
                             總倉款 = i.總倉款,
                             展售款 = i.展售款,
                         }).ToList();

            gv_List.DataSource = temp2;
            gv_List.DataBind();

            var 總倉款 = temp2.Select(x => x.總倉款).Distinct().Count();
            var 展售款 = temp2.Where(x=>x.展售款 != "").Select(x => x.展售款).Distinct().Count();

            lbl_Count.Text = "總倉款：" + 總倉款 + ", 展售款：" + 展售款;

            gv_List.Width = 400;
        }

        #endregion

        #endregion

        #region 副功能-判斷狀態

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

        #endregion

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
                    if (DDL_Choice.SelectedValue == Function91)
                    {
                        GetCellByName(e.Row, "Id").Visible = false;
                        str_Status = GetCellByName(e.Row, "處理狀態").Text.Trim();

                        if (e.Row.RowType == DataControlRowType.DataRow)
                        {
                        }
                    }
                    else if (DDL_Choice.SelectedValue == Function99 && e.Row.RowType == DataControlRowType.DataRow)
                    {
                        //系列編號
                        var tempCell = GetCellByName(e.Row, "系列編號");
                        if (str_tempbox != tempCell.Text)
                        {
                            str_tempbox = tempCell.Text;
                            tempColor = (tempColor == System.Drawing.Color.DarkGreen) ? System.Drawing.Color.DarkRed : System.Drawing.Color.DarkGreen;
                        }
                        tempCell.ForeColor = tempColor;

                        //產生連結
                        HL_Temp = new HyperLink();
                        HL_Temp.Target = "_blank";
                        HL_Temp.NavigateUrl = "http://erp03.obdesign.com.tw/admin/products/showdetail1.aspx?seriesID=" + tempCell.Text;
                        HL_Temp.Text = "修改";
                        GetCellByName(e.Row, "功能").Controls.Add(HL_Temp);
                    }
                    else if (DDL_Choice.SelectedValue == Function101 && e.Row.RowType == DataControlRowType.DataRow)
                    {
                        //系列編號
                        var tempCell = GetCellByName(e.Row, "產品編號");
                        if (str_tempbox != tempCell.Text)
                        {
                            str_tempbox = tempCell.Text;
                            tempColor = (tempColor == System.Drawing.Color.DarkGreen) ? System.Drawing.Color.DarkRed : System.Drawing.Color.DarkGreen;
                        }
                        tempCell.ForeColor = tempColor;

                        tempCell = GetCellByName(e.Row, "刪單");
                        tempCell.ForeColor = (tempCell.Text == "是") ? System.Drawing.Color.Red : tempCell.ForeColor;
                    }
                    else if (DDL_Choice.SelectedValue == Function102 && e.Row.RowType == DataControlRowType.DataRow)
                    {
                        //系列編號
                        var tempCell = GetCellByName(e.Row, "產品編號");
                        if (str_tempbox != tempCell.Text)
                        {
                            str_tempbox = tempCell.Text;
                            tempColor = (tempColor == System.Drawing.Color.DarkGreen) ? System.Drawing.Color.DarkRed : System.Drawing.Color.DarkGreen;
                        }
                        tempCell.ForeColor = tempColor;

                        tempCell = GetCellByName(e.Row, "來源儲位");
                        tempCell.ForeColor = (tempCell.Text == "無") ? System.Drawing.Color.Red : tempCell.ForeColor;
                    }
                    else if (DDL_Choice.SelectedValue == Function103 && e.Row.RowType == DataControlRowType.DataRow)
                    {
                        //系列編號
                        var tempCell = GetCellByName(e.Row, "出貨暫存儲位");
                        if (str_tempbox != tempCell.Text)
                        {
                            str_tempbox = tempCell.Text;
                            tempColor = (tempColor == System.Drawing.Color.DarkGreen) ? System.Drawing.Color.DarkRed : System.Drawing.Color.DarkGreen;
                        }
                        tempCell.ForeColor = tempColor;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region GetCellByName

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

        #endregion

        #region ●產生XLS(2013-0925修改)

        /// <summary>
        /// 產生XLS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_XLS_Click(object sender, EventArgs e)
        {
            //設定儲位內容之外的報表
            string[] functionTypelist = { Function91, Function92, Function93, Function94, Function95, Function98, 
                                            Function99, Function101, Function102, Function103, Function104, Function105, 
                                            Function106, Function107, Function108, Function109, Function111, Function112 };
            //可產生XLS
            string[] CanDoCreateXLS = { Function91, Function92, Function93, Function94, Function95, Function98, 
                                          Function99, Function101, Function102, Function103, Function104, Function105, 
                                          Function106, Function107, Function108, Function109, Function111, Function112 };
   
            //先查出來
            btn_Search_Click(sender, e);
            
            //有資料
            if (gv_List.Rows.Count > 0)
            {
                //再產生XLS
                if (CanDoCreateXLS.Contains(DDL_Choice.SelectedValue))
                    CreateXLS(DDL_Choice.SelectedItem.Text);
                else if (!functionTypelist.Contains(DDL_Choice.SelectedValue))
                    CreateXLS(DDL_Choice.SelectedItem.Text + "儲位");
            }
            else
            {
                lbl_Message.Text = "無資料，無法產生XLS!";
            }
        }

        /// <summary>
        /// CreateXLS
        /// </summary>
        protected void CreateXLS(string SearchFileName)
        {
            #region 設定

            //檔名
            string xls_typeName, xls_date, xls_filename;

            xls_typeName = SearchFileName;
            xls_date = (HandleStartDate == HandleEndDate || txt_End.Visible == false) ? DateTime.Parse(HandleStartDate).ToString("yyyy-MMdd") :
                DateTime.Parse(HandleStartDate).ToString("yyyy-MMdd") + "~" + DateTime.Parse(HandleEndDate).ToString("yyyy-MMdd");
            xls_filename = xls_date + "_" + xls_typeName;

            CreateXLS CX = new CreateXLS();
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();

            //要輸出的欄位
            int[] list = { };

            #endregion

            //呼叫產生XLS
            CX.DoCreateXLS(workbook, ms, list.ToList(), gv_List, false, "");

            //輸出
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + HttpUtility.UrlEncode(xls_filename, System.Text.Encoding.UTF8) + ".xls"));
            Response.BinaryWrite(ms.ToArray());
            workbook = null;
            ms.Close();
            ms.Dispose();
        }

        #endregion
    }
}