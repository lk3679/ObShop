using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.PDA;
using POS_Library.Public;

namespace OBShopWeb
{
    public partial class NoProduct : System.Web.UI.Page
    {
        #region 宣告

        private string account;
        string PageKey;

        #endregion 宣告

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //判斷帳號登入
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                    Response.End();
                }
                else
                {
                    account = Session["Account"].ToString();
                    PageKey = Request["PageKey"] ?? "";

                    lbl_Message.ForeColor = System.Drawing.Color.Red;
                    lbl_Message.Text = "";
                    if (!IsPostBack)
                    {
                        //pickBarCode  => ticket:未知，area 1碼，store 1碼，pickType 2碼
                        if (Request["pickNum"] != null)
                        {
                            txt_PickCheck_NO.Visible = false;
                            lbl_PickCheck_NO.Visible = false;
                            var pickNum = Request["pickNum"];
                            var pick = Utility.GetPickNumRegex(pickNum);
                            var num = pick.Number;
                            var store = pick.Store;
                            var pickType = pick.PickType;
                            lblPickNo.Text = pickNum;
                            if (!IsPostBack)
                            {
                                ShipOutDetail(pickType, int.Parse(num), Request["productid"], store);
                            }
                        }
                        else
                        {
                            //手動輸入
                            txt_PickCheck_NO.Enabled = true;
                            txt_PickCheck_NO.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion Page_Load

        public void ShipOutDetail(int pickType, int num, string productid, int store)
        {
            try
            {
                var ticketShelfTempList = new List<POS_Library.ShopPos.DataModel.TicketShelfTemp>();
                var shipDa = new POS_Library.ShopPos.ShipOutDA();
                ticketShelfTempList = shipDa.PickDetailCk(num, pickType, store);
                if (!string.IsNullOrEmpty(productid))
                {
                    ticketShelfTempList = ticketShelfTempList.Where(x => x.ProductId == productid).ToList();
                }
                ticketShelfTempList = ticketShelfTempList.Select(x => new POS_Library.ShopPos.DataModel.TicketShelfTemp
                {
                    Ticket = x.Ticket,
                    ShipId = x.ShipId,
                    Account = x.Account,
                    ShipNo = x.ShipNo,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    Repository = x.Repository,
                    Division = x.Division,
                    Floor = x.Floor,
                    GuestId = x.GuestId,
                    NowQuantity = x.NowQuantity,
                    LackQuantity = x.LackQuantity
                }).ToList();
                //產生無貨表供勾選
                if (ticketShelfTempList.Count > 0)
                {
                    int x = 1;
                    var result = (from i in ticketShelfTempList
                                  orderby i.Division
                                  select new
                                  {
                                      序號 = x++,
                                      傳票號碼 = i.Ticket,
                                      訂單編號 = i.GuestId,
                                      商品編號 = i.ProductId,
                                      倉別 = i.Repository,
                                      儲位名稱 = i.Division,
                                      應有數量 = i.NowQuantity,
                                      現有數量 = i.NowQuantity,
                                      短缺數量 = i.LackQuantity,
                                      樓層 = i.Floor,
                                      GuestId = i.GuestId,
                                      出貨序號 = i.ShipId,
                                      貨運單號 = i.ShipNo,
                                      帳號 = i.Account,
                                  }).ToList();
                    if (result.Count == 0)
                    {
                        lbl_Message.Text = "此無貨撿貨單已全部處理完成！請重新列印撿貨單！！";
                    }
                    gv_List.DataSource = result;
                    gv_List.DataBind();
                }
                else
                {
                    lbl_Message.Text = "無資料！"; 
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #region 主功能-執行無貨回報(找建議儲位)(按鈕)

        protected void Btn_Execution_Click(object sender, EventArgs e)
        {
            try
            { 
                Button btn = sender as Button;
                CheckFormat CF = new CheckFormat();
                foreach (GridViewRow row in gv_List.Rows)
                {
                    Button btnTest = row.Cells[0].FindControl("Btn_Execution") as Button;

                    //判斷是否是按下按鈕的那一行
                    if (btnTest == btn)
                    {
                        var txtTemp = row.Cells[4].FindControl("LackNum") as TextBox;

                        if (CF.CheckID(txtTemp.Text, CheckFormat.FormatName.Number))
                        {
                            string lackQuantity = txtTemp.Text.Trim();
                            string originalQuantity = row.Cells[3].Text.Trim();
                             
                            //缺少數量 <= 應有數量
                            if (int.Parse(lackQuantity) == 0)
                            {
                                lbl_Message.Text = "數量不可為0！";
                            }
                            else if (int.Parse(lackQuantity) <= int.Parse(originalQuantity))
                            {
                                var productStatus = RB_Flaw.SelectedValue;
                                //儲位名稱,商品編號,應有數量,短缺數量
                                string noProductData = string.Format("{0},{1},{2},{3}", row.Cells[1].Text.Trim(), row.Cells[2].Text.Trim(), row.Cells[3].Text.Trim(), lackQuantity);
                                string account = row.Cells[9].Text.Trim();
                                string reason = DDL_Reason.SelectedValue;
                                var url = "NoProductCheck.aspx?pickNum=" + lblPickNo.Text + "&productStatus=" + productStatus + "&noProductData=" + noProductData + "&account=" + account + "&reason=" + reason + "&PageKey=" + PageKey; 
                                Page.RegisterClientScriptBlock("checkinput", @"<script>window.open('" + url + "','_self');</script>"); 
                            }
                            else
                            {
                                lbl_Message.Text = "缺少數量 超過 應有數量！";
                            }
                        }
                        else
                        {
                            lbl_Message.Text = "請輸入正確數字！";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-執行無貨回報(找建議儲位)(按鈕)

        #region 主功能-判斷撿貨單號or出貨明細號

        /// <summary>
        /// 判斷是撿貨單號or出貨明細號
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txt_PickCheck_NO_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txt_PickCheck_NO.Text))
                {
                    lbl_Message.Text = "未輸入條碼！";
                    return;
                }
                var inputPickNum = txt_PickCheck_NO.Text.Trim();
                var pick = Utility.GetPickNumRegex(inputPickNum);
                var num = pick.Number;
                var store = pick.Store;
                var pickType = pick.PickType;
                lblPickNo.Text = inputPickNum;
                POS_Library.ShopPos.JobPerformance JB = new POS_Library.ShopPos.JobPerformance();
                //判斷績效
                var isPerformanceUp = JB.IsPerformancePOS(int.Parse(num), pickType);

                switch (isPerformanceUp)
                {
                    case true:  //錯誤的傳票號碼
                        lbl_Message.Text = "此撿貨單已結案!";  
                        break;

                    case false:   //成功
                        lbl_Message.ForeColor = System.Drawing.Color.Green;
                        lbl_Message.Text = "此撿貨單未結案!";
                         
                        //鎖鎖鎖鎖(2013-0927新增)
                        //CB_Allot.Enabled = false;

                        ShipOutDetail(pickType, int.Parse(num), "", store);
                        break;
                }

                txt_PickCheck_NO.Text = "";
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-判斷撿貨單號or出貨明細號

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
                //e.Row.Cells[6].Visible = false;
                e.Row.Cells[8].Visible = false;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion gv_List_RowDataBound
    }
}