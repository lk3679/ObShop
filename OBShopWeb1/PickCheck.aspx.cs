using System;
using System.Web.Configuration;
using POS_Library.ShopPos;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class PickCheck : System.Web.UI.Page
    {
        #region 宣告

        private String id, account, ticket_id, repository_id, union, ShipOutType;
        private String 績效type, EID, EName;
        private String floor_id, barcode_string, checkString;

        private setup auth = new setup();

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        private POS_Library.Public.MsgStatus MSListNew = new POS_Library.Public.MsgStatus();
        private POS_Library.Public.MsgStatus MSListNewFinal = new POS_Library.Public.MsgStatus();

        #endregion 宣告

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //撿貨type = 1
                //type = "1";

                //判斷帳號登入
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                    Response.End();
                }
                else
                {
                    lbl_Message.ForeColor = System.Drawing.Color.Red;
                    lbl_Message.Text = "";

                    //切換(2013-0517新增)
                    if (Request["shoptype"] != null)
                        Session["ShipOutType"] = ((Utility.Store)int.Parse(Request["shoptype"].ToString())).ToString();

                    if (Session["ShipOutType"] != null)
                    {
                        lbl_ShipOutType.Text = "(" + Session["ShipOutType"].ToString() + ")";

                        lbl_ShipOutType.BackColor = System.Drawing.Color.Orange;
                        績效type = "1";

                    }
                    else
                    {
                        lbl_ShipOutType.Text = "未選擇類別";
                    }

                    if (Request["union"] != null)
                        union = Request["union"].ToString();

                    account = Session["Account"].ToString();

                    if (Session["EID"] != null)
                        EID = Session["EID"].ToString();
                    if (Session["Name"] != null)
                        EName = Session["Name"].ToString();

                    //無貨回報撿貨確認
                    checkString = "";
                    if (Request["checkString"] != null)
                    {
                        checkString = Request["checkString"].ToString();
                        DoPickCheck();
                    }

                    ////帳號登入
                    //if (Session["logisticAccount"] == null)
                    //{
                    //    txt_PickCheck_NO.Enabled = false;
                    //}
                    ////物流barcode登入
                    //else
                    //{
                    //    txt_PickCheck_NO.Enabled = true;
                        txt_PickCheck_NO.Focus();
                    //}
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion Page_Load

        #region 主功能-撿貨

        /// <summary>
        /// txtbox內容改變
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txt_PickCheck_NO_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DoPickCheck();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 撿貨確認
        /// </summary>
        protected void DoPickCheck()
        {
            try
            {
                var shipDa = new POS_Library.ShopPos.ShipOutDA();
                //測試 000065457818
                id = txt_PickCheck_NO.Text.Trim();
                if (checkString != "")
                    id = checkString;

                if (string.IsNullOrEmpty(id))
                {
                    lbl_Message.Text = "未輸入條碼！";
                    return;
                }
                var inputPickNum = txt_PickCheck_NO.Text.Trim();
                var pick = Utility.GetPickNumRegex(inputPickNum);
                var num = pick.Number;
                var store = pick.Store;
                var pickType = pick.PickType;

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
                        if (int.Parse(num) <300)
                        {
                            lbl_Message.Text = num+"：資料錯誤，請洽系統管理員";
                        }
                        //調出+瑕疵(2015-0716新增)
                        if (pickType == 2 || pickType == 4)
                        {
                            ticket_id = lbl_Ticket_Id.Text = num;
                            lbl_Ship_Id.Text = "0";

                            MSListNewFinal = shipDa.GetPerformance(account, (int)POS_Library.Public.Utility.LogisticsType.調出撿貨確認, int.Parse(num), _areaId);

                            if (MSListNewFinal != null)
                            {
                                if (MSListNewFinal.Result == "1")
                                {
                                    lbl_Message.ForeColor = System.Drawing.Color.Green;
                                }
                                lbl_Message.Text = MSListNewFinal.Reason;
                            }
                        }
                        else// if (pickType == 0)
                        {
                            lbl_Ticket_Id.Text = "0";
                            lbl_Ship_Id.Text = num;

                            MSListNewFinal = shipDa.GetPerformanceSale(account, (int)POS_Library.Public.Utility.LogisticsType.撿貨, int.Parse(num), _areaId);

                            if (MSListNewFinal != null)
                            {
                                if (MSListNewFinal.Result == "1")
                                {
                                    lbl_Message.ForeColor = System.Drawing.Color.Green;
                                    lbl_Message.Text = "成功!";
                                }
                                else
                                {
                                    lbl_Message.Text = id + ", " + MSListNewFinal.Reason;
                                }
                            }
                        }

                        break;
                }

                txt_PickCheck_NO.Text = "";
            }
            catch (Exception ex)
            {
                Response.Write(id + ", " + "系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-撿貨

        #region 副功能(檢查String是否為數字)

        /// <summary>
        /// 檢查String是否為數字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        private static bool IsNumeric(object Expression)
        {
            bool isNum;

            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any,
                System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

            return isNum;
        }

        #endregion 副功能(檢查String是否為數字)
    }
}