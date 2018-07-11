using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using POS_Library.ShopPos;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class RequireList : System.Web.UI.Page
    {
        #region 宣告

        private String account;

        private setup auth = new setup();

        private System.Drawing.Color tempColor = System.Drawing.Color.DarkRed;
        private String str_tempbox = "";

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
                //else if (!auth.checkFunctionsAuthority("●調出需求單"))
                //{
                //    Response.Redirect("Privilege.aspx");
                //}
                else
                {
                    account = Session["Account"].ToString();
                    lbl_Count.Text = "";

                    if (!IsPostBack)
                    {
                        txt_Start.Text = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
                        txt_End.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion Page_Load

        #region 主功能-查詢

        /// <summary>
        /// 查詢按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 查詢
        /// </summary>
        protected void Search()
        {
            try
            {
                int status = int.Parse(DDL_Status.SelectedValue);
                int type = int.Parse(DDL_Type.SelectedValue);
                //特定需求單
                var 特定需求單 = txt_特定需求單.Text.Trim();

                RequireDA.RequireStatus BoxStatus = (status == -2) ? RequireDA.RequireStatus.全部 : (status == -1) ? RequireDA.RequireStatus.取消 :
                    (status == 1) ? RequireDA.RequireStatus.未建傳票 : RequireDA.RequireStatus.已建傳票;
                RequireDA.RequireType BoxType = (type == -1) ? RequireDA.RequireType.全部 :
                    (type == 0) ? RequireDA.RequireType.調撥 : (type == 1) ? RequireDA.RequireType.瑕疵 : RequireDA.RequireType.問題件;

                var temp = (特定需求單 == "") ? RequireDA.GetRequireAllStatusList(BoxStatus, BoxType, DateTime.Parse(txt_Start.Text), DateTime.Parse(txt_End.Text).AddDays(1), _areaId) :
                                          RequireDA.GetRequireOneStatusList(特定需求單, _areaId);

                #region ●需求單狀態

                int x = 1;

                var temp2 = (from i in temp
                             orderby i.申請時間 descending
                             select new
                             {
                                 序號 = x++,
                                 需求單ID = i.需求單ID,
                                 種類 = i.種類,
                                 產品編號 = i.產品編號,
                                 預撥數量 = i.預撥數量,
                                 實際數量 = i.實際數量,
                                 申請人 = i.申請人,
                                 申請時間 = i.申請時間.ToString("yyyy-MM-dd HH:mm"),
                                 採購ID = i.採購ID,
                                 傳票ID = (i.傳票ID != 0) ? i.傳票ID.ToString() : "未建立",
                                 審核人 = i.審核人 ?? "無",
                                 狀態 = (i.狀態 == -1) ? "取消" : (i.預撥數量 == 0) ? "移除" : 
                                        (i.狀態 == 0) ? "待審" : (i.狀態 >= 1 && i.狀態 < 10) ? "待建傳票" : "可印單",
                             }).ToList();

                gv_List.DataSource = temp2;
                gv_List.DataBind();

                #endregion ●需求單狀態

                var 已審需求單數 = temp2.Select(y => y.需求單ID).Distinct().Count();
                var 採購單數 = temp2.Select(y => y.採購ID).Distinct().Count();
                var 已審預撥數 = temp2.Where(y => y.審核人 != "無").Sum(y => y.預撥數量);
                var 未審預撥數 = temp2.Where(y => y.審核人 == "無").Sum(y => y.預撥數量);
                var 實際數 = temp2.Sum(y => y.實際數量);
                var 傳票數 = temp2.Where(y => y.傳票ID != "未建立").Select(y => y.傳票ID).Distinct().Count();
                var 總筆數 = temp2.Count();

                lbl_Count.Text = "總筆數: " + 總筆數 + ", " + "已審需求單數: " + 已審需求單數 + ", " + "採購單數: " + 採購單數 +
                    ", " + "傳票數: " + 傳票數 + ", " + "已審預撥數: " + 已審預撥數 + ", " + "未審預撥數: " + 未審預撥數 + ", " + "實際數: " + 實際數;
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-查詢

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
                //若為DataRow則放入HyperLink
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //箱號
                    var tempCell = GetCellByName(e.Row, "需求單ID");
                    if (str_tempbox != tempCell.Text)
                    {
                        str_tempbox = tempCell.Text;
                        tempColor = (tempColor == System.Drawing.Color.DarkGreen) ? System.Drawing.Color.DarkRed : System.Drawing.Color.DarkGreen;
                    }
                    tempCell.ForeColor = tempColor;
                }
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

        #endregion gv_List_RowDataBound

    }
}