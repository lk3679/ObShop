using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Web.Configuration;
using POS_Library;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb.PDA
{
    public partial class StorageInfoRangeSeries : System.Web.UI.Page
    {
        #region 宣告

        CheckFormat CF = new CheckFormat();
        ShelfProcess sp = new ShelfProcess();

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        String account;
        int numcount = 0;
        object temp = new object();

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Account"] == null)
            {
                Response.Write(" <script> parent.document.location= '../logout.aspx' </script> ");
                Response.End();
            }
            else
            {
                account = Session["Name"].ToString();

                lbl_Message.Text = "";
                lbl_Count.Text = "";

                if (!IsPostBack)
                {
                }
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
                    Search();
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
        /// 執行查詢
        /// </summary>
        protected void Search()
        {
            try
            {
                int x = 1;
                var Series = txt_系列.Text.Trim();
                List<string> 系列 = Series.Replace(" ", "").Replace("\r\n",",").Split(',').ToList();

                int[] stype = { 0, 1, 2, 3, 4 };
                int[] stype2 = { 5 };

                var temp = sp.GetRangeSearchProductBySeries(系列, (CB_不良.Checked ? stype2 : stype), _areaId).ToList();

                var temp2 = (from i in temp
                             select new
                             {
                                 序號 = x++,
                                 產品編號 = i.ProductNumber,
                                 儲位編號 = i.ShelfId,
                                 數量 = i.Quantity,
                                 類型 = i.Id,
                             }).ToList();

                var 總件數 = temp2.Select(y => y.數量).Sum();
                var 總筆數 = numcount = temp2.Count;

                gv_List.DataSource = temp2;
                gv_List.DataBind();

                lbl_Count.Text = "系列數：" + 系列.Count + ", 總筆數: " + 總筆數 + ", 總件數: " + 總件數;
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
            btn_Submit_Click(sender, e);

            #region 設定

            var xls_filename = string.Format("{0}_系列盤點清單_【{1}筆】", DateTime.Now.ToString("yyyy-MMdd"), numcount);

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