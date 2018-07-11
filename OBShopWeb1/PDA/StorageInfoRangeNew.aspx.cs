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
    public partial class StorageInfoRangeNew : System.Web.UI.Page
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
                //P_AllCount.Visible = false;

                if (!IsPostBack)
                {
                    ddl_Floor.DataSource = sp.GetFloor(_areaId);
                    ddl_Floor.DataBind();
                    ddl_Floor_SelectedIndexChanged(ddl_Floor, null);
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
                if (txt_StartShelf.Text != "" && txt_EndShelf.Text != ""
                    && txt_StartShelf.Text.Length == 8 && txt_EndShelf.Text.Length == 8)
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
        /// 切換樓層
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddl_Floor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ddl_Area.DataSource = sp.GetAreaAll(_areaId);
                ddl_Area.DataBind();
                ddl_Area_SelectedIndexChanged(ddl_Area, null);
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// 切換區域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddl_Area_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                gv_List.DataSource = "";
                gv_List.DataBind();
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
                int i = 1;
                String head = ddl_Area.SelectedValue;
                int type = int.Parse(DDL_Type.SelectedValue);
                var aaa = sp.GetRangeSearchProduct(head + txt_StartShelf.Text.Trim(), head + txt_EndShelf.Text.Trim(), type, _areaId);

                //紀錄筆數
                numcount = aaa.Count;

                var temp = aaa.Select(
                    x => new
                    {
                        序號 = (i++).ToString(),
                        儲位 = x.ShelfId, //+ " 【" +  aaa.Where(y => y.ShelfId == x.ShelfId).Select(y => y.Quantity).Sum() + "】",
                        產品 = x.ProductNumber,
                        數量 = x.Quantity.ToString(),
                        可銷 = x.StatusName,
                        //款數 = aaa.Where(y => y.ShelfId == x.ShelfId).Count().ToString(),
                        //總數 = aaa.Where(y => y.ShelfId == x.ShelfId).Select(y => y.Quantity).Sum().ToString(),
                        //總數 = " 【" + aaa.Where(y => y.ShelfId == x.ShelfId).Select(y => y.Quantity).Sum() + "】",
                    }
                    ).ToList();

                String tempstr = "";
                String tempstr2 = "";
                int 貨架數 = 0;
                int 空白數 = 0;
                for (int a = 0; a < temp.Count; a++)
                {
                    //if (a == 0)
                    //{
                    //    tempstr = temp[a].儲位.Substring(0, 5);
                    //}
                    if (tempstr != temp[a].儲位.Substring(0, 5))
                    {
                        tempstr2 = temp[a].儲位;
                        tempstr = temp[a].儲位.Substring(0, 5);
                        //var empty = new { 序號 = "貨架", 儲位 = tempstr, 產品 = temp[a].款數.ToString(), 數量 = temp[a].總數, 款數 = "", 總數  = "" };
                        var empty = new { 序號 = "貨架", 儲位 = tempstr, 產品 = "", 數量 = "", 可銷 = "" };
                        temp.Insert(a, empty);
                        貨架數++;
                    }
                    else if (tempstr2 != temp[a].儲位 && temp[a - 1].序號 != "貨架")
                    {
                        tempstr2 = temp[a].儲位;
                        //var empty = new { 序號 = "貨架", 儲位 = tempstr, 產品 = temp[a].款數.ToString(), 數量 = temp[a].總數, 款數 = "", 總數  = "" };
                        var empty = new { 序號 = "", 儲位 = "", 產品 = "", 數量 = "", 可銷 = "" };
                        temp.Insert(a, empty);
                        空白數++;
                    }
                }

                gv_List.DataSource = temp;
                gv_List.DataBind();

                lbl_Count.Text = "總筆數: " + numcount + " , 貨架數: " + 貨架數;
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region ●主功能-查詢庫存可銷

        /// <summary>
        /// 取總庫存數/可銷數
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_GetAllCount_Click(object sender, EventArgs e)
        {
            try
            {
                P_AllCount.Visible = true;

                int[] arr = { 0 };

                int[] aa = sp.GetAllProductAndSellCount(_areaId);

                var temp = (from i in arr
                            select new
                            {
                                庫存總數 = aa[0],
                                可銷數 = aa[1],
                            }).ToList();

                gv_AllCount.DataSource = temp;
                gv_AllCount.DataBind();
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

            String head = ddl_Area.SelectedValue;

            var txtName = string.Format("{0}_{1}～{2}_盤點清單_【{3}筆】",
                DateTime.Now.ToString("yyyy-MMdd"), head + txt_StartShelf.Text.Trim(), head + txt_EndShelf.Text.Trim(), numcount);

            //string[] columns = new string[] { "序號", "儲位", "產品", "數量", "總數" };
            string[] columns = new string[] { "序號", "儲位", "產品", "數量", "可銷" };
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();

            // 新增試算表。
            var sheet = workbook.CreateSheet("工作表1");
            //加Column
            Row row = sheet.CreateRow(0);
            for (int i = 0; i < columns.Length; i++)
            {
                Cell cell = row.CreateCell(i);
                cell.SetCellValue(columns[i]);
            }

            for (int i = 0; i < gv_List.Rows.Count; i++)
            {
                row = sheet.CreateRow(i + 1);

                row.CreateCell(0).SetCellValue(gv_List.Rows[i].Cells[0].Text == "&nbsp;" ? "" : gv_List.Rows[i].Cells[0].Text);
                row.CreateCell(1).SetCellValue(gv_List.Rows[i].Cells[1].Text == "&nbsp;" ? "" : gv_List.Rows[i].Cells[1].Text);
                row.CreateCell(2).SetCellValue(gv_List.Rows[i].Cells[2].Text == "&nbsp;" ? "" : gv_List.Rows[i].Cells[2].Text);
                row.CreateCell(3).SetCellValue(gv_List.Rows[i].Cells[3].Text == "&nbsp;" ? "" : gv_List.Rows[i].Cells[3].Text);
                row.CreateCell(4).SetCellValue(gv_List.Rows[i].Cells[4].Text == "&nbsp;" ? "" : gv_List.Rows[i].Cells[4].Text);
                //row.CreateCell(5).SetCellValue(gv_List.Rows[i].Cells[5].Text);
            }
            workbook.Write(ms);

            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + HttpUtility.UrlEncode(txtName, System.Text.Encoding.UTF8) + ".xls"));
            Response.BinaryWrite(ms.ToArray());
            workbook = null;
            ms.Close();
            ms.Dispose();
        }

        #endregion

        #region 介面-合併儲存格/gv_List_RowDataBound

        /// <summary>
        /// 在PreRender時做RowSpan合併儲存格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_List_PreRender(object sender, EventArgs e)
        {
            try
            {
                //int i = 1;
                //foreach (GridViewRow row in gv_List.Rows)
                //{
                //    if (row.RowIndex != 0)
                //    {
                //        //與前i筆帳號一樣
                //        if (row.Cells[1].Text.Trim() == gv_List.Rows[(row.RowIndex - i)].Cells[1].Text.Trim())
                //        {
                //            GetCellByName(gv_List.Rows[(row.RowIndex - i)], "總數").RowSpan += 1;

                //            GetCellByName(row, "總數").Visible = false;

                //            i++;
                //        }

                //        //只有一筆
                //        else
                //        {
                //            i = 1;

                //            GetCellByName(row, "總數").RowSpan = 1;
                //        }
                //    }
                //    else
                //    {
                //        GetCellByName(row, "總數").RowSpan = 1;
                //    }
                //}
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

        /// <summary>
        /// gv_List_RowDataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_List_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.Cells[0].Text == "貨架")
                    {
                        e.Row.Cells[0].BackColor =
                        e.Row.Cells[1].BackColor =
                        e.Row.Cells[2].BackColor =
                        e.Row.Cells[3].BackColor =
                        e.Row.Cells[4].BackColor = System.Drawing.Color.DarkSlateGray;
                        e.Row.Cells[0].ForeColor =
                        e.Row.Cells[1].ForeColor = System.Drawing.Color.White;
                    }
                    else if (e.Row.Cells[0].Text != "&nbsp;")
                    {
                        e.Row.Cells[1].Text = CF.TransShelfIdToLabel(e.Row.Cells[1].Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 介面-合併儲存格/gv_List_RowDataBound

    }
}