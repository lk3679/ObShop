using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    /// <summary>
    /// 績效報表
    /// </summary>
    public partial class logistics_print : System.Web.UI.Page
    {
        #region 宣告

        private DataTable dt = new DataTable("ALLData");
        private List<POS_Library.ShopPos.DataModel.LogisticsReportModel> LPList = new List<POS_Library.ShopPos.DataModel.LogisticsReportModel>();
        private String zone;
        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

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
                    if (!IsPostBack)
                    {
                        txt_Start.Text = DateTime.Today.ToString("yyyy-MM-dd");
                        txt_End.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    }
                    zone = _areaId.ToString();

                    SetShipOutTable();
                }
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
                try
                {
                    //使用服務查詢
                    var job = new POS_Library.ShopPos.LogisticsAccount();
                    LPList = job.GetPrintLogistics(zone, txt_Start.Text, DateTime.Parse(txt_End.Text).AddDays(1).ToString("yyyy-MM-dd"));
                }
                catch (Exception ex)
                {
                    string a = POS_Library.Public.Utility.Stack.ExceptionMsg(ex);
                    Response.Write("系統發生錯誤 " + a);
                }

                if (LPList.Count > 0)
                {
                    for (int i = 0; i < LPList.Count; i++)
                    {
                        DataRow dr = dt.NewRow();
                        dr["帳號"] = LPList[i].Account;

                        //暫存帳號
                        String Account = "";
                        decimal score = 0;
                        int j = i + 1;
                        var logistics = new List<string>();
                        logistics.Add(((int)POS_Library.Public.Utility.LogisticsType.入庫確認).ToString());
                        logistics.Add(((int)POS_Library.Public.Utility.LogisticsType.入庫上架).ToString());
                        logistics.Add(((int)POS_Library.Public.Utility.LogisticsType.移動儲位).ToString());
                        logistics.Add(((int)POS_Library.Public.Utility.LogisticsType.調出驗貨確認).ToString());
                        logistics.Add(((int)POS_Library.Public.Utility.LogisticsType.盤點無條件上架).ToString());
                        logistics.Add(((int)POS_Library.Public.Utility.LogisticsType.盤點無條件打銷).ToString());

                        if (Account == "")
                        {
                            Account = LPList[i].Account;
                            score += (!logistics.Contains(LPList[i].TypeName)) ? LPList[i].ProductScore : LPList[i].ProductQuantity;
                        }
                        //帳號相同時把分數加總
                        while (j < LPList.Count && Account == LPList[j].Account)
                        {
                            score += (!logistics.Contains(LPList[i].TypeName)) ? LPList[j].ProductScore : LPList[j].ProductQuantity;
                            j++;
                        }
                        //加總完放入
                        dr["總分"] = score.ToString();
                        Account = "";
                        score = 0;
                        j = i;

                        dr["種類"] = ((POS_Library.Public.Utility.LogisticsType)int.Parse(LPList[i].TypeName)).ToString();
                        dr["項目"] = LPList[i].ProductItem;
                        dr["件數"] = LPList[i].ProductQuantity;
                        dr["分數"] = (!logistics.Contains(LPList[i].TypeName)) ? LPList[i].ProductScore : LPList[i].ProductQuantity;

                        dt.Rows.Add(dr);
                    }
                }
                gv_logistics.DataSource = dt;
                gv_logistics.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-產生報表

        #region 介面

        /// <summary>
        /// 在PreRender時做RowSpan合併儲存格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_logistics_PreRender(object sender, EventArgs e)
        {
            try
            {
                int i = 1;
                foreach (GridViewRow row in gv_logistics.Rows)
                {
                    if (row.RowIndex != 0)
                    {
                        //與前i筆帳號一樣
                        if (row.Cells[0].Text.Trim() == gv_logistics.Rows[(row.RowIndex - i)].Cells[0].Text.Trim())
                        {
                            gv_logistics.Rows[(row.RowIndex - i)].Cells[0].RowSpan += 1;
                            gv_logistics.Rows[(row.RowIndex - i)].Cells[5].RowSpan += 1;

                            //if (row.RowIndex == gv_logistics.Rows.Count - 1)
                            //{
                            //    gv_logistics.Rows[(row.RowIndex - i)].Cells[0].RowSpan += 1;
                            //    gv_logistics.Rows[(row.RowIndex - i)].Cells[5].RowSpan += 1;
                            //}
                            row.Cells[0].Visible = false;
                            row.Cells[5].Visible = false;

                            i++;
                        }
                        //只有一筆
                        else
                        {
                            i = 1;
                            row.Cells[0].RowSpan = 1;
                            row.Cells[5].RowSpan = 1;
                        }
                    }
                    else
                    {
                        row.Cells[0].RowSpan = 1;
                        row.Cells[5].RowSpan = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 加光棒效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_logistics_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                e.Row.Cells[0].Width = 200;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[5].ForeColor = System.Drawing.Color.ForestGreen;
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

        #endregion 介面
    }
}