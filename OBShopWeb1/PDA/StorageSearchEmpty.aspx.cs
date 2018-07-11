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
    public partial class StorageSearchEmpty : System.Web.UI.Page
    {
        #region 宣告

        CheckFormat CF = new CheckFormat();
        ShelfProcess sp = new ShelfProcess();

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        String account;

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

                lbl_Content.Text = "";
                lbl_Num.Text = "";
                if (!IsPostBack)
                {
                    ddl_Floor.DataSource = sp.GetFloor(_areaId);
                    ddl_Floor.DataBind();
                    ddl_Floor_SelectedIndexChanged(ddl_Floor, null);
                }
            }
        }

        #endregion

        #region 主功能-查詢

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
                var storage = sp.GetEmptyStorageNew(_areaId, ddl_Floor.Text, ddl_Area.Text);

                if (storage != null && storage.Count > 0)
                {
                    int i = 0;
                    foreach (var data in storage)
                    {
                        if (!string.IsNullOrEmpty(lbl_Content.Text))
                        {
                            if (i % 2 == 0 && i != 0)
                                lbl_Content.Text = lbl_Content.Text + "<br />";
                            else
                                lbl_Content.Text = lbl_Content.Text + ", ";
                        }

                        lbl_Content.Text = lbl_Content.Text + CF.TransShelfIdToLabel(data);

                        i++;
                    }

                    //空儲位數量
                    lbl_Num.Text = storage.Count.ToString();
                }
                else
                {
                    lbl_Content.Text = "無";
                }
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

            ShelfProcess sp = new ShelfProcess();
            var washName = string.Format("{0}樓{1}區_空儲位", ddl_Floor.Text, ddl_Area.Text);
            var storage = sp.GetEmptyStorageNew(_areaId, ddl_Floor.Text, ddl_Area.Text).ToList();
            var storageCount = string.Format("空儲位【{0}】", storage.Count());
            string[] columns = new string[] { storageCount };
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

            int a = 0;
            foreach (var item in storage)
            {
                a++;
                row = sheet.CreateRow(a);

                row.CreateCell(0).SetCellValue(CF.TransShelfIdToLabel(item));
            }
            workbook.Write(ms);

            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + HttpUtility.UrlEncode(washName, System.Text.Encoding.UTF8) + ".xls"));
            Response.BinaryWrite(ms.ToArray());
            workbook = null;
            ms.Close();
            ms.Dispose();
        }

        #endregion
    }
}