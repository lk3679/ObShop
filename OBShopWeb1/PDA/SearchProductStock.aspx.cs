using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb.PDA
{
    public partial class SearchProductStock : System.Web.UI.Page
    {
        #region 宣告

        CheckFormat CF = new CheckFormat();
        ShelfProcess sp = new ShelfProcess();

        String str_input, str_product;
        List<ShelfConfig> list = new List<ShelfConfig>();
        List<ShelfConfig> GVUselist = new List<ShelfConfig>();

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        String account, temp_ProductID;

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

                temp_ProductID = lbl_ProductID.Text.Trim();
                lbl_Message.Text = "";
                lbl_ProductID.Text = "";
                lbl_Info1.Text = "";
                txt_Input.Focus();

                //有值代入參數轉Session
                if (Request["SearchID"] != null)
                {
                    Session["SearchID"] = Request["SearchID"].ToString();
                    Response.Redirect("SearchProduct.aspx");
                }
                else if (Session["SearchID"] != null)
                {
                    txt_Input.Text = Session["SearchID"].ToString();
                    Session["SearchID"] = null;
                    txt_Input_TextChanged(sender, e);
                }
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
                    lbl_Info1.Text = "";
                    str_product = sp.GetProductNum(str_input);

                    if (str_product == null || str_product == "")
                    {
                        lbl_Message.Text = "產品條碼不存在！";
                    }
                    else
                    {
                        Search(str_product);
                    }
                }
                else if (str_input != "")
                {
                    Search(str_input);
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
        public void Search(String input)
        {
            lbl_ProductID.Text = input;

            var list1 = sp.ShelfGroupList(input, int.Parse(ddl_Type.SelectedValue), _areaId, false).OrderBy(x => x.StorageTypeId).ToList();

            foreach (var data in list1)
            {
                lbl_Info1.Text += CF.TypeToName(data.StorageTypeId) + " x " + data.Quantity + "<br />";
            }

            GVUselist = list1;

            if (list1.Count == 0)
            {
                lbl_Message.Text = "目標儲位無此產品";
            }
        }

        #endregion

        #region ●產生XLS

        /// <summary>
        /// 產生XLS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_XLS_Click(object sender, EventArgs e)
        {
            txt_Input.Text = temp_ProductID;
            //先查出來
            txt_Input_TextChanged(sender, e);

            List<string> aa = new List<string>();
            aa.Add("(類型)_數量");
            foreach (var i in GVUselist)
            {
                aa.Add(CF.TypeToName(i.StorageTypeId) + " x " + i.Quantity);
            }

            gv_List.DataSource = aa;
            gv_List.DataBind();

            //有資料
            if (gv_List.Rows.Count > 0)
            {
                //再產生XLS
                CreateXLS(temp_ProductID);
            }
            else
            {
                lbl_Message.Text = "無資料，無法產生XLS!";
            }
        }

        /// <summary>
        /// CreateXLS
        /// </summary>
        protected void CreateXLS(string SearchFileName)
        {
            #region 設定

            //檔名
            string xls_typeName, xls_date, xls_filename;

            xls_typeName = SearchFileName;
            xls_date = DateTime.Today.ToString("yyyy-MMdd");
            xls_filename = xls_date + "_" + xls_typeName;

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