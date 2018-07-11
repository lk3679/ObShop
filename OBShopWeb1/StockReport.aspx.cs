using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using POS_Library.ShopPos;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace OBShopWeb
{
    public partial class StockReport : System.Web.UI.Page
    {
        #region 宣告

        bool paging = true;
        System.Drawing.Color tempColor = System.Drawing.Color.DarkRed;
        String str_tempbox = "";

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_Count.Text = "";
        }

        #endregion

        #region ●查詢

        public void Search()
        {
            Storage ST = new Storage();
            var temp = ST.GetStockAllocation(txt_SerialId.Text.Trim(), txt_ProductId.Text.Trim());

            var xi = 1;
            var temp2 = (from i in temp
                         where (DDL_Type.SelectedValue == "一般") || ((i.Quantity + i.TempQuantity) != i.RealQuantity)
                         select new
                         {
                             序號 = xi++,
                             系列 = i.SerialId,
                             產品編號 = i.ProductId,
                             總數 = i.RealQuantity,
                             有效 = i.Quantity,
                             無效 = i.TempQuantity,
                             預購 = i.PreQuantity,
                             更新日期 = i.LastModifyDate.ToString("yyyy-MM-dd HH:mm"),
                             最後上架日 = i.InStockDate.HasValue ? i.InStockDate.Value.ToString("yyyy-MM-dd HH:mm") : "無",
                         }).ToList();

            gv_List.DataSource = temp2;

            var 系列數 = temp2.Select(x => x.系列).Distinct().Count();
            var 產品數 = temp2.Select(x => x.產品編號).Distinct().Count();
            var 總數 = temp2.Sum(x => x.總數);
            var 有效總數 = temp2.Sum(x => x.有效);
            var 無效總數 = temp2.Sum(x => x.無效);
            var 預購總數 = temp2.Sum(x => x.預購);

            lbl_Count.Text = "系列數：" + 系列數 + ", 產品數：" + 產品數 + ", 總數：" + 總數 +
                ", 有效總數：" + 有效總數 + ", 無效總數：" + 無效總數 + ", 預購總數：" + 預購總數;

            gv_List.AllowPaging = CB_分頁.Checked && paging;
            gv_List.PageSize = int.Parse(DDL_單頁筆數.SelectedValue);
        }

        protected void btn_查詢_Click(object sender, EventArgs e)
        {
            Search();

            gv_List.DataBind();

        }

        #endregion

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

        protected void gv_List_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //若為DataRow則放入HyperLink
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //箱號
                var tempCell = GetCellByName(e.Row, "系列");
                if (str_tempbox != tempCell.Text)
                {
                    str_tempbox = tempCell.Text;
                    tempColor = (tempColor == System.Drawing.Color.DarkGreen) ? System.Drawing.Color.DarkRed : System.Drawing.Color.DarkGreen;
                }
                tempCell.ForeColor = tempColor;
            }

            //光棒效果
            GvLightBar.lightbar(e, 2);
        }

        /// <summary>
        /// 換頁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_List_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            Search();
            gv_List.PageIndex = e.NewPageIndex;
            gv_List.DataBind();
        }

        #endregion

        #region 匯出XLS

        /// <summary>
        /// XLS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Xls_Click(object sender, EventArgs e)
        {
            paging = false;
            //先查詢
            btn_查詢_Click(sender, e);

            #region 設定

            var xls_filename = string.Format("{0}_庫存報表_【{1}筆】", DateTime.Now.ToString("yyyy-MMdd"), gv_List.Rows.Count);

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