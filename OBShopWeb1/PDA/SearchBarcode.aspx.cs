using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb.PDA
{
    public partial class SearchBarcode : System.Web.UI.Page
    {
        #region 宣告

        CheckFormat CF = new CheckFormat();
        ShelfProcess sp = new ShelfProcess();

        String str_input, str_product;
        List<ShelfConfig> list = new List<ShelfConfig>();

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
                account = Session["Account"].ToString();

                lbl_Message.Text = "";
                lbl_ProductID.Text = "";
                lbl_Info.Text = "";
                txt_Input.Focus();
            }
        }

        #endregion

        #region 主功能-刷條碼

        /// <summary>
        /// txt改變(刷條碼)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txt_Input_TextChanged(object sender, EventArgs e)
        {
            try
            {
                str_input = txt_Input.Text.Trim();

                if (CF.CheckID(str_input, CheckFormat.FormatName.Storage))
                {
                    lbl_Message.Text = "請輸入產品名稱或條碼！";
                }
                else if (CF.CheckID(str_input, CheckFormat.FormatName.Product))
                {
                    Search(str_input, false);

                }
                else if (str_input != "")
                {
                    Search(str_input, true);
                }
                else
                {
                    lbl_Message.Text = "請輸入產品名稱或條碼！";
                }
                txt_Input.Text = "";
                txt_Input.Focus();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 用產編或barcode查詢
        /// </summary>
        /// <param name="input"></param>
        public void Search(String input, bool type)
        {
            String result = "";
            String mappingName = "";

            //產編查條碼
            if (type)
            {
                result = sp.GetProductBarcode(input);
            }
            //條碼查產編
            else
            {
                result = sp.GetProductNum(input);
            }

            lbl_ProductID.Text = input;

            if (!String.IsNullOrEmpty(result))
            {
                lbl_Info.Text = result + " " + mappingName;
            }
            else
            {
                lbl_Message.Text = "無此產品資料";
            }
        }

        #endregion
    }
}