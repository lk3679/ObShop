using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Web.Configuration;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    /// <summary>
    /// 績效報表(區間)
    /// </summary>
    public partial class logistics_print2 : System.Web.UI.Page
    {
        #region 宣告

        private DataTable dt = new DataTable("ALLData");
        private List<POS_Library.ShopPos.DataModel.LogisticsReportModel> LRList = new List<POS_Library.ShopPos.DataModel.LogisticsReportModel>();

        private String zone;
        private String time1, time2;
        private String preType = "";
        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        #endregion 宣告

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Session["Account"] = "jack";
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                    Response.End();
                }
                else
                {
                    lbl_Count.Text = "";

                    if (!IsPostBack)
                    {
                        txt_Start.Text = DateTime.Today.ToString("yyyy-MM-dd");
                        //txt_Start.Text = "2011-11-28";
                        txt_End.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    }
                    zone = _areaId.ToString();

                    lbl_Time.Text = "";

                    //SetShipOutTable();
                }
                //if (!IsPostBack)
                //{
                //    txt_Start.Text = DateTime.Today.ToString("yyyy-MM-dd");
                //    //txt_Start.Text = "2011-11-28";
                //    txt_End.Text = DateTime.Today.ToString("yyyy-MM-dd");
                //}
                //if (Request["zone"] != null)
                //    zone = Request["zone"].ToString();
                //else
                //    zone = "1";

                //lbl_Time.Text = "";
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion Page_Load

        #region 設定

        /// <summary>
        /// 建立dt
        /// </summary>
        protected void SetShipOutTable()
        {
            try
            {
                //datatable
                dt.Columns.Add("帳號", typeof(String));
                dt.Columns.Add("種類", typeof(String));
                dt.Columns.Add("項目", typeof(String));
                dt.Columns.Add("件數", typeof(String));
                dt.Columns.Add("分數", typeof(String));
                dt.Columns.Add("總分", typeof(String));
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 設定

        #region 主功能-產生報表

        /// <summary>
        /// 查詢按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                //時間設定
                if (DDL_Time1.SelectedValue == "24")
                {
                    time1 = " 23:59:59";
                }
                else
                {
                    time1 = " " + DDL_Time1.SelectedValue + ":00:00";
                }

                if (DDL_Time2.SelectedValue == "24")
                {
                    time2 = " 23:59:59";
                }
                else
                {
                    time2 = " " + DDL_Time2.SelectedValue + ":00:00";
                }

                lbl_Time.Text = "統計區間：" + txt_Start.Text + time1 + " ~ " + txt_End.Text + time2;

                SearchPrint();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 產生報表
        /// </summary>
        protected void SearchPrint()
        {
            try
            {
                //使用服務查詢
                var job = new POS_Library.ShopPos.LogisticsAccount();
                if (DDL_Type.SelectedValue == "1+")
                {
                    LRList = job.GetPrintLogisticsForPick(zone, txt_Start.Text + time1, txt_End.Text + time2);
                    if (LRList.Count > 0)
                    {
                        var aa = (from i in LRList
                                  orderby i.Account, i.Group
                                  select new
                                  {
                                      帳號 = i.Account,
                                      種類 = ((POS_Library.Public.Utility.LogisticsType)int.Parse(i.TypeName)).ToString(),
                                      區域 = i.Group,
                                      項目 = i.ProductItem,
                                      件數 = i.ProductQuantity,
                                      分數 = decimal.Round(i.ProductScore, 2, MidpointRounding.AwayFromZero) 
                                  }).ToList();
                        if (CB_Sort.Checked)
                            aa = aa.OrderBy(x => x.區域).ToList();
                        gv_logistics.DataSource = aa;

                        var 總分數 = aa.Select(x => x.分數).Sum();
                        var 總筆數 = aa.Count;

                        lbl_Count.Text = "總筆數：" + 總筆數 + ", 總分數：" + 總分數;
                    }
                }
                else
                {
                    LRList = job.GetPrintLogistics(zone, txt_Start.Text + time1, txt_End.Text + time2);
                    if (LRList.Count > 0)
                    {
                        var aa = (from i in LRList
                                  where (DDL_Type.SelectedIndex == 0) ? true :
                                  ((DDL_Type.SelectedValue == "3") ? (i.TypeName == DDL_Type.SelectedValue || i.TypeName == "103") : i.TypeName == DDL_Type.SelectedValue)
                                  orderby int.Parse(i.TypeName), i.ProductScore descending
                                  select new
                                  {
                                      帳號 = i.Account,
                                      種類 = ((POS_Library.Public.Utility.LogisticsType)int.Parse(i.TypeName)).ToString(),
                                      項目 = i.ProductItem,
                                      件數 = i.ProductQuantity,
                                      分數 = decimal.Round(i.ProductScore, 2, MidpointRounding.AwayFromZero) 
                                  }).ToList();
                        gv_logistics.DataSource = aa;

                        var 總分數 = decimal.Round(aa.Select(x => x.分數).Sum(), 2, MidpointRounding.AwayFromZero);
                        var 總筆數 = aa.Count;

                        lbl_Count.Text = "總筆數：" + 總筆數 + ", 總分數：" + 總分數;
                    }
                }
                gv_logistics.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-產生報表

        #region 主功能-匯出XLS

        /// <summary>
        /// 匯出XLS按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_XLS_Click(object sender, EventArgs e)
        {
            btn_Search_Click(sender, e);

            CreateXLS("result_" + txt_Start.Text + time1 + "~" + txt_End.Text + time2);
        }

        /// <summary>
        /// CreateXLS
        /// </summary>
        protected void CreateXLS(string SearchFileName)
        {
            #region 設定

            //檔名
            string xls_filename;
            xls_filename = SearchFileName;

            CreateXLS CX = new CreateXLS();
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();

            //要輸出的欄位
            int[] list = { };

            #endregion

            //呼叫產生XLS
            CX.DoCreateXLS(workbook, ms, list.ToList(), gv_logistics, false, "");

            //輸出
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + HttpUtility.UrlEncode(xls_filename, System.Text.Encoding.UTF8) + ".xls"));
            Response.BinaryWrite(ms.ToArray());
            workbook = null;
            ms.Close();
            ms.Dispose();
        }

        #endregion 主功能-匯出XLS

        #region 介面

        /// <summary>
        /// 加光棒效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_logistics_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (DDL_Type.SelectedValue == "1+")
                {
                    e.Row.Cells[0].Width = 200;
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        e.Row.Cells[5].ForeColor = System.Drawing.Color.ForestGreen;
                        if (CB_Sort.Checked)
                        {
                            if (e.Row.Cells[2].Text.Trim() != preType)
                            {
                                for (int i = 0; i <= 5; i++)
                                {
                                    e.Row.Cells[i].ForeColor = System.Drawing.Color.DarkRed;
                                }
                            }

                            preType = e.Row.Cells[2].Text.Trim();
                        }
                        else
                        {
                            if (e.Row.Cells[0].Text.Trim() != preType)
                            {
                                for (int i = 0; i <= 5; i++)
                                {
                                    e.Row.Cells[i].ForeColor = System.Drawing.Color.DarkRed;
                                }
                            }

                            preType = e.Row.Cells[0].Text.Trim();
                        }
                    }
                }
                else
                {
                    e.Row.Cells[0].Width = 200;
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        e.Row.Cells[4].ForeColor = System.Drawing.Color.ForestGreen;

                        if (e.Row.Cells[1].Text.Trim() != preType)
                        {
                            for (int i = 0; i <= 4; i++)
                            {
                                e.Row.Cells[i].ForeColor = System.Drawing.Color.Blue;
                            }
                        }

                        preType = e.Row.Cells[1].Text.Trim();
                    }
                }

                //光棒效果
                //判定row的型態是資料行
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    GvLightBar.lightbar(e, 1);
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 切換DDL/顯示排序勾選
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DDL_Type.SelectedValue == "1+")
            {
                CB_Sort.Visible = true;
            }
            else
            {
                CB_Sort.Checked = CB_Sort.Visible = false;
            }
        }

        #endregion 介面
    }
}