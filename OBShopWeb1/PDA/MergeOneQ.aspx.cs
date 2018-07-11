using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using POS_Library.Public;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using OBShopWeb.Poslib;

namespace OBShopWeb.PDA
{
    /// <summary>
    /// 問題上架
    /// 如從問題上架移入一般儲位需建傳票
    /// </summary>
    public partial class MergeOneQ : System.Web.UI.Page
    {
        #region 宣告

        private ShelfProcess sp = new ShelfProcess();
        private CheckFormat CF = new CheckFormat();

        //台灣倉庫 0  虎門倉庫 1
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        private String account;
        private MsgStatus result;

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
                lbl_Message.Text = "";

                if (IsPostBack)
                    txt_Input.Focus();
                //else
                //    txt_AllNum.Focus();
            }
        }

        #endregion Page_Load

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
                string str_input = txt_Input.Text.Trim();
                //檢查儲位條碼
                if (CF.CheckID(str_input, CheckFormat.FormatName.Storage))
                {
                    #region 儲位條碼

                    int? shelfType = sp.CheckStorage(str_input, _areaId);
                    if (shelfType == null)
                    {
                        lbl_Message.Text = str_input + " 不存在，請設定！";
                    }
                    else
                    {
                        //第一次一定要刷入儲位，檢查來源儲位是否已填入
                        if (string.IsNullOrEmpty(lbl_TargetStorage_NO.Text))
                        {

                            var fromType = shelfType.Value; 
                            var strShelf = CF.CheckShelfType(fromType, "", (int)POS_Library.ShopPos.EnumData.MergeType.問題上架); 
                            if (!string.IsNullOrEmpty(strShelf))
                            {
                                lbl_Message.Text = strShelf;
                                return;
                            }
                            else
                            {
                                var shelfDetail = sp.GetStorageConfig(str_input, _areaId);
                                if (shelfDetail == null)
                                {
                                    lbl_Message.Text = "空儲位";
                                }
                                else
                                {
                                    lbl_Message.Text = "";
                                }
                                lbl_TargetStorage_NO.Text = str_input;
                                lbl_TargetStorage_NO_Type.Text = CF.TypeToName(shelfType);
                            }
                        }
                        else
                        {
                            lbl_Message.Text = "儲位已輸入！";
                        }
                    }

                    #endregion 儲位條碼
                }
                else if (string.IsNullOrEmpty(lbl_TargetStorage_NO.Text))
                {
                    lbl_Message.Text = "請先刷儲位條碼！";
                }
                else
                {
                    #region 產品編號

                    string temp = "";
                    if (CF.CheckID(str_input, CheckFormat.FormatName.Product))
                    {
                        temp = sp.GetProductNum(str_input);
                    }
                    else
                    {
                        string ttt = sp.GetProductBarcode(str_input);
                        if (!string.IsNullOrEmpty(ttt))
                            temp = str_input;
                    }

                    string productName = temp;
                    if (!string.IsNullOrEmpty(productName))
                    {
                        #region 輸入數量幾個就跑幾次(2013-0121修改)

                        //沒輸入就當做1個
                        int qua = (txt_Num.Text == "" || txt_Num.Text == "1") ? 1 : int.Parse(txt_Num.Text.Trim());

                        //輸入數量幾個就跑幾次(2013-0121修改)
                        for (int i = 0; i < qua; i++)
                        {
                            hide_ProductBarcode.Value += str_input + ",";

                            if (lbl_Product.Text != "")
                                lbl_Product.Text = productName + "<br />" + lbl_Product.Text;
                            else
                                lbl_Product.Text = productName;

                            //算目前件數
                            lbl_CurrentNum.Text = (int.Parse(lbl_CurrentNum.Text) + 1).ToString();
                        }

                        #endregion 輸入數量幾個就跑幾次(2013-0121修改)
                    }
                    else
                    {
                        lbl_Message.Text = "產品條碼錯誤！";
                    }

                    #endregion 產品編號
                }

                txt_Num.Text = "";
                txt_Input.Text = "";
                txt_Input.Focus();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// txt改變(刷條碼)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txt_Input_TextChanged1(object sender, EventArgs e)
        {
            try
            {
                ////須有填入件數 才可開始執行
                //if (txt_AllNum.Text.Trim() == "")
                //{
                //    lbl_Message.Text = "未填入總件數！";
                //}
                //else if (!CF.CheckID(txt_AllNum.Text.Trim(), CheckFormat.FormatName.Number))
                //{
                //    lbl_Message.Text = "件數請填數字！";
                //}
                //else
                //{
                string str_input = txt_Input.Text.Trim();

                if (CF.CheckID(str_input, CheckFormat.FormatName.Storage))
                {
                    int? type = sp.CheckStorage(str_input, _areaId);

                    if (type == null)
                    {
                        lbl_Message.Text = str_input + " 不存在請設定！";
                    }
                    else if (type != 4)
                    {
                        lbl_Message.Text = str_input + " 不屬於問題儲位！";
                    }
                    else
                    {
                        Session["ShelfList"] = sp.GetStorageConfig(str_input, _areaId);
                        lbl_TargetStorage_NO.Text = str_input;
                        if (Session["ShelfList"] == null)
                        {
                            lbl_Message.Text = "空儲位";
                        }
                    }
                }
                else if (str_input != "")
                {
                    //if (lbl_CurrentNum.Text != txt_AllNum.Text.Trim())
                    //{
                    //先輸入儲位
                    if (!String.IsNullOrEmpty(lbl_TargetStorage_NO.Text))
                    {
                        //產編/條碼都可吃(2013-0410新增)------------
                        String temp = "";

                        if (CF.CheckID(str_input, CheckFormat.FormatName.Product))
                        {
                            temp = sp.GetProductNum(str_input);
                        }
                        else
                        {
                            String ttt = sp.GetProductBarcode(str_input);
                            if (!String.IsNullOrEmpty(ttt))
                                temp = str_input;
                        }

                        String productName = temp;
                        //------------------------------------------

                        if (!String.IsNullOrEmpty(productName))
                        {
                            #region 輸入數量幾個就跑幾次(2013-0121修改)

                            //沒輸入就當做1個
                            int qua = (txt_Num.Text == "" || txt_Num.Text == "1") ? 1 : int.Parse(txt_Num.Text.Trim());

                            ////數量正確
                            //if (qua + int.Parse(lbl_CurrentNum.Text) <= int.Parse(txt_AllNum.Text.Trim()))
                            //{
                            //輸入數量幾個就跑幾次(2013-0121修改)
                            for (int i = 0; i < qua; i++)
                            {
                                hide_ProductBarcode.Value += str_input + ",";

                                if (lbl_Product.Text != "")
                                    lbl_Product.Text = productName + "<br />" + lbl_Product.Text;
                                else
                                    lbl_Product.Text = productName;

                                //算目前件數
                                lbl_CurrentNum.Text = (int.Parse(lbl_CurrentNum.Text) + 1).ToString();

                                ////判斷件數是否相符 換色
                                //if (lbl_CurrentNum.Text == txt_AllNum.Text.Trim())
                                //{
                                //    lbl_CurrentNum.ForeColor = System.Drawing.Color.Blue;
                                //    btn_Submit.Visible = true;
                                //}
                                //else
                                //{
                                //    lbl_CurrentNum.ForeColor = System.Drawing.Color.OrangeRed;
                                //    btn_Submit.Visible = false;
                                //}
                            }
                            //}
                            //else
                            //{
                            //    lbl_Message.Text = "超過總件數！";
                            //}

                            #endregion 輸入數量幾個就跑幾次(2013-0121修改)
                        }
                        else
                        {
                            lbl_Message.Text = "產品條碼錯誤！";
                        }
                    }
                    else
                    {
                        lbl_Message.Text = "請先刷儲位條碼！";
                    }
                    //}
                    //else
                    //{
                    //    lbl_Message.Text = "已達輸入件數！";
                    //}
                }
                //}
                txt_Num.Text = "";
                txt_Input.Text = "";
                txt_Input.Focus();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-刷條碼

        #region 主功能-確認上架

        /// <summary>
        /// 確定上架
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                ////判斷件數是否相符 換色
                //if (lbl_CurrentNum.Text == txt_Num.Text)
                //{
                lbl_CurrentNum.ForeColor = System.Drawing.Color.Blue;

                String[] product = lbl_Product.Text.Split('<');
                var productList = new List<ImportClass.ProductData>();

                for (int i = 0; i < product.Length; i++)
                {
                    var data = new ImportClass.ProductData();
                    data.Name = product[i].Replace("br />", "");
                    data.Quantity = 1;
                    productList.Add(data);
                }

                //執行合併
                result = sp.SetShelfProduct(lbl_TargetStorage_NO.Text, productList, account, true, _areaId, (int)POS_Library.ShopPos.EnumData.StorageDetailType.入庫無條件上架);

                if (result.Result == "1")
                {
                    lbl_Message.Text = "共 " + product.ToList().Count + " 件 上架成功！";
                    //清空
                    lbl_Product.Text = "";
                    lbl_TargetStorage_NO.Text = "";
                    lbl_TargetStorage_NO_Type.Text = "";
                    lbl_CurrentNum.Text = "0";
                    txt_Num.Text = "";
                }
                else
                {
                    lbl_Message.Text = result.Reason;
                }
                //}
                //else
                //{
                //    lbl_CurrentNum.ForeColor = System.Drawing.Color.OrangeRed;
                //    lbl_Message.Text = "件數不符！";
                //}
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-確認上架
    }
}