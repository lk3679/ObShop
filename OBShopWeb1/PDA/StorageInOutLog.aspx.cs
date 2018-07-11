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
using POS_Library;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb.PDA
{
    public partial class StorageInOutLog : System.Web.UI.Page
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

                    HandleStartDate = txt_Start.Text.Trim();
                    HandleEndDate = txt_End.Text.Trim();

                    lbl_Count_盤點上架.Text = "";
                    lbl_Count_盤點打銷.Text = "";
                    lbl_Count_出貨打銷.Text = "";
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
            gv_List_總數量.DataSource = "";
            gv_List_總數量.DataBind();
            gv_List_盤點上架.DataSource = "";
            gv_List_盤點上架.DataBind();
            gv_List_盤點打銷.DataSource = "";
            gv_List_盤點打銷.DataBind();
            gv_List_出貨打銷.DataSource = "";
            gv_List_出貨打銷.DataBind();
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
            txt_productId.Text = "";
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
                productId = txt_productId.Text = txt_productId.Text.Trim();

                //日期參數
                SDay = DateTime.Parse(HandleStartDate);
                EDay = DateTime.Parse(HandleEndDate).AddDays(1);
                WithoutDate = CB_Date.Checked;

                SearchActionLog();
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
                var aa = sp.GetStorageActionLogByInventory(SDay, EDay, WithoutDate, productId, _areaId).ToList();

                int y1 = 1;
                var 盤點上架 = (from i in aa
                            where i.StorageDetailTypeId == 10
                            orderby i.LogDateTime descending
                            select new
                            {
                                序號 = y1++,
                                日期 = i.LogDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                產品編號 = i.ProductNumber,
                                數量 = i.Quantity,
                                來源儲位編號 = i.FromStorage,
                                目的儲位編號 = i.TargetStorage,
                                帳號 = (i.LogAccount == "_BOT_") ? "※系統※" : i.LogAccount,
                                狀態 = "盤點上架",
                            }).ToList();

                int y2 = 1;
                var 盤點打銷 = (from i in aa
                            where i.StorageDetailTypeId == 0
                            orderby i.LogDateTime descending
                            select new
                            {
                                序號 = y2++,
                                日期 = i.LogDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                產品編號 = i.ProductNumber,
                                數量 = i.Quantity,
                                來源儲位編號 = i.FromStorage,
                                目的儲位編號 = i.TargetStorage,
                                帳號 = (i.LogAccount == "_BOT_") ? "※系統※" : i.LogAccount,
                                狀態 = "盤點打銷",
                            }).ToList();

                int y3 = 1;
                var 出貨打銷 = (from i in aa
                            where (i.StorageDetailTypeId == 8 || i.StorageDetailTypeId == 2)
                            orderby i.LogDateTime descending
                            select new
                            {
                                序號 = y3++,
                                日期 = i.LogDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                產品編號 = i.ProductNumber,
                                數量 = i.Quantity,
                                來源儲位編號 = i.FromStorage,
                                目的儲位編號 = i.TargetStorage,
                                帳號 = (i.LogAccount == "_BOT_") ? "※系統※" : i.LogAccount,
                                狀態 = (i.StorageDetailTypeId == 8) ? "刪單打銷" : "建議儲位打銷",
                            }).ToList();

                var 總數量 = aa.Select(x => x.Quantity).Sum();
                var 盤點上架數 = 盤點上架.Select(x => x.數量).Sum();
                var 盤點打銷數 = 盤點打銷.Select(x => x.數量).Sum();
                var 出貨打銷數 = 出貨打銷.Select(x => x.數量).Sum();

                var 建議數 = 出貨打銷.Where(x => x.狀態 == "建議儲位打銷").Select(x => x.數量).Sum();
                var 刪單數 = 出貨打銷.Where(x => x.狀態 == "刪單打銷").Select(x => x.數量).Sum();


                if (CB_Search_盤點上架.Checked)
                {
                    gv_List_盤點上架.DataSource = 盤點上架;
                    gv_List_盤點上架.DataBind();
                    lbl_Count_盤點上架.Text = "資料筆數：" + 盤點上架.Count;
                }
                if (CB_Search_盤點打銷.Checked)
                {
                    gv_List_盤點打銷.DataSource = 盤點打銷;
                    gv_List_盤點打銷.DataBind();
                    lbl_Count_盤點打銷.Text = "資料筆數：" + 盤點打銷.Count;
                }
                if (CB_Search_出貨打銷.Checked)
                {
                    gv_List_出貨打銷.DataSource = 出貨打銷;
                    gv_List_出貨打銷.DataBind();
                    lbl_Count_出貨打銷.Text = "資料筆數：" + 出貨打銷.Count;
                }

                int[] arr = { 0 };

                int[] 總數表 = { 總數量, 盤點上架數, 盤點打銷數, 出貨打銷數 };

                var temp = (from i in arr
                            select new
                            {
                                總數量 = 總數表[0],
                                盤點上架數 = 總數表[1],
                                盤點打銷數 = 總數表[2],
                                出貨打銷數 = 總數表[3].ToString() + " (建議：" + 建議數 + ", 刪單：" + 刪單數 + ")",
                            }).ToList();

                gv_List_總數量.DataSource = temp;
                gv_List_總數量.DataBind();
                
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