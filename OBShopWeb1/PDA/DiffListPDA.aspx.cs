using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using OBShopWeb.Poslib;
using POS_Library.ShopPos;

namespace OBShopWeb.PDA
{
    public partial class DiffListPDA : System.Web.UI.Page
    {
        #region 宣告

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        private String lack, more, product, account;
        private string[] array;

        #endregion 宣告

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

                if (!IsPostBack)
                {
                    if (Request.QueryString["storage"] != null)
                        lbl_Storage_NO.Text = Request.QueryString["storage"].ToString();

                    //Session(2013-0402修改)--------------------------------
                    if (Session["PDAlack"] != null)
                    {
                        lack = Session["PDAlack"].ToString();
                        Session["PDAlack"] = null;

                        array = lack.Split(',');

                        if (array.Length > 0)
                        {
                            lbl_Lack.Text = "缺少：<br />";
                            SetLabel(array, lbl_Lack);
                        }
                    }
                    if (Session["PDAmore"] != null)
                    {
                        more = Session["PDAmore"].ToString();
                        Session["PDAmore"] = null;

                        array = more.Split(',');

                        if (array.Length > 0)
                        {
                            lbl_More.Text = "多出：<br />";
                            SetLabel(array, lbl_More);
                        }
                    }
                    if (Session["PDAproduct"] != null)
                    {
                        product = Session["PDAproduct"].ToString();
                        Session["PDAproduct"] = null;

                        array = product.Split(',');

                        if (array.Length > 0)
                        {
                            lbl_Product.Text = "內容：<br />";
                            SetLabel(array, lbl_Product);
                        }
                    }
                    //------------------------------------------------------
                }
            }
        }

        #endregion Page_Load

        #region 副功能-串label

        /// <summary>
        /// 串label
        /// </summary>
        /// <param name="list"></param>
        /// <param name="lbl"></param>
        protected void SetLabel(string[] list, Label lbl)
        {
            try
            {
                foreach (var data in list)
                {
                    if (!string.IsNullOrEmpty(data))
                        lbl.Text += data + "<br />";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 副功能-串label

        #region 主功能-差異清單確認(調整庫存)

        /// <summary>
        /// 差異清單確認
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime date = DateTime.Now;
                btn_Submit.Enabled = false;
                var inventoryId = POS_Library.Public.Utility.GetGuidMD5();
                StartReport(date, inventoryId);

                bool result = true;

                #region 多的產品處理

                string more = lbl_More.Text.Replace("多出：<br />", "");
                string[] moreList = more.Split('<');
                List<string> moreProducts = new List<string>();
                foreach (var data in moreList)
                {
                    string productId = data.Replace("br />", "");

                    if (!string.IsNullOrEmpty(productId))
                    {
                        if (result)
                        {
                            moreProducts.Add(productId.Trim());
                        }
                    }
                }

                #endregion 多的產品處理

                #region 少的產品處理

                string lack = lbl_Lack.Text.Replace("缺少：<br />", "");
                string[] lackList = lack.Split('<');
                List<string> lackProducts = new List<string>();
                foreach (var data in lackList)
                {
                    string productId = data.Replace("br />", "");

                    if (!string.IsNullOrEmpty(productId))
                    {
                        lackProducts.Add(productId.Trim());
                    }
                }

                #endregion 少的產品處理

                //有差異才可以進 for 績效(2013-0925修改)
                if (moreProducts.Count > 0 || lackProducts.Count > 0)
                {
                    if ((moreProducts.Count + lackProducts.Count) > 100)
                    {
                        lbl_Message.Text = "差異總數量請在100件以內！";
                        return;
                    }

                    ShelfProcess shelfProcess = new ShelfProcess();
                    var msg = shelfProcess.盤點儲位差異(lbl_Storage_NO.Text, moreProducts, lackProducts, account, date, inventoryId, _areaId);
                    if (msg.Result != "1")
                    {
                        result = false;
                    }
                }

                if (result)
                {
                    lbl_Message.Text = "成功！";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 寫入盤點紀錄
        /// </summary>
        /// <returns></returns>
        protected bool StartReport(DateTime date, string inventoryId)
        {
            try
            {
                ShelfProcess sp = new ShelfProcess();
                sp.SetInventoryLog(lbl_Storage_NO.Text, account, date, inventoryId, _areaId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion 主功能-差異清單確認(調整庫存)
    }
}