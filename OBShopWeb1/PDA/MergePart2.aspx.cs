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
    /// 移動產品
    /// 只限制有效入有效、有效入無效
    /// </summary>
    public partial class MergePart2 : System.Web.UI.Page
    {
        #region 宣告

        private ShelfProcess sp = new ShelfProcess();
        private CheckFormat CF = new CheckFormat();

        //儲位所在地
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

                txt_Input.Focus();
            }
        }

        #endregion Page_Load

        #region 主功能-儲位對儲位部份移動

        protected void txt_Input_TextChanged(object sender, EventArgs e)
        {
            ShelfProcess sp = new ShelfProcess();

            string str_input = txt_Input.Text.Trim();

            //檢查儲位條碼
            if (CF.CheckID(str_input, CheckFormat.FormatName.Storage))
            {
                int? shelfType = sp.CheckStorage(str_input, _areaId);
                if (shelfType == null)
                {
                    lbl_Message.Text = str_input + " 不存在，請設定！";
                    return;
                }
                //第一次一定要刷入儲位，檢查來源儲位是否已填入
                if (string.IsNullOrEmpty(lbl_FromStorage_NO.Text))
                {
                    //檢查來源，不可為入庫暫存檢查 + 不良 + 打消 
                    var fromType = shelfType.Value;
                    ViewState["fromType"] = fromType;
                    var strShelf = CF.CheckShelfType(fromType, "", (int)EnumData.MergeType.移動儲位); 
                    if (string.IsNullOrEmpty(strShelf))
                    {
                        Session["ShelfList"] = sp.GetStorageConfig(str_input, _areaId);
                        lbl_FromStorage_NO.Text = str_input;
                        lbl_FromStage_NO_Type.Text = CF.TypeToName(shelfType);
                        lbl_Message.Text = "";
                    }
                    else
                    {
                        lbl_Message.Text = strShelf;
                        return;
                    }
                }
                else
                {
                    if (str_input == lbl_FromStorage_NO.Text)
                    {
                        lbl_Message.Text = "不能和自己合併！";
                    }
                    //輸入目的儲位，不可為入庫暫存檢查
                    string From, Target;
                    From = lbl_FromStorage_NO.Text;
                    Target = str_input;
                    //檢查目的不可為暫存，可為打消、不良 
                    var fromType = int.Parse(ViewState["fromType"].ToString());
                    var targetType = shelfType.Value;
                    var strShelf = CF.CheckShelfType(fromType, targetType.ToString(), (int)EnumData.MergeType.移動儲位);
                    if (string.IsNullOrEmpty(strShelf))
                    {

                        lbl_TargetStorage_NO.Text = str_input;
                        lbl_TargetStorage_NO_Type.Text = CF.TypeToName(shelfType);
                        btn_Submit.Visible = true;   
                    }
                    else
                    {
                        lbl_Message.Text = strShelf;
                        return;
                    } 
                }
            }
            else if (string.IsNullOrEmpty(lbl_FromStorage_NO.Text))
            {
                lbl_Message.Text = "請先刷儲位條碼！";
            }
            else
            {
                //產編/條碼都可吃(2013-0426新增)------------
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

                string productName = temp;

                if (!string.IsNullOrEmpty(productName))
                {
                    List<ShelfConfig> list = Session["ShelfList"] as List<ShelfConfig>;
                    var 撿貨未確認 = sp.GetTempStorageDetail(lbl_FromStorage_NO.Text, productName, _areaId);
                    if (list != null && list.Count > 0)
                    {
                        #region 判斷內容數量與輸入數量

                        //取內容數量與輸入數量
                        int qua = (txt_Num.Text == "" || txt_Num.Text == "1") ? 1 : int.Parse(txt_Num.Text.Trim());
                        int NowProductNum = list.Where(x => x.ProductNumber == productName).Sum(x => x.Quantity);

                        //數量夠 = true
                        bool Numresult = (qua <= NowProductNum) ? true : false;

                        #endregion 判斷內容數量與輸入數量

                        //如果夠就扣數
                        if (Numresult)
                        {
                            bool result = false;

                            #region 輸入數量幾個就跑幾次(2013-0124修改)

                            //輸入數量幾個就跑幾次(2013-0121修改)
                            for (int i = 0; i < qua; i++)
                            {
                                result = false;

                                foreach (var data in list)
                                {
                                    if (data.ProductNumber == productName)
                                    {
                                        lbl_Product.Text = productName + "<br />" + lbl_Product.Text;
                                        //算目前件數
                                        lbl_CurrentNum.Text = (int.Parse(lbl_CurrentNum.Text) + 1).ToString();

                                        if (--data.Quantity == 0)
                                        {
                                            list.Remove(data);
                                        }

                                        lbl_Message.Text = "";
                                        result = true;
                                        break;
                                    }
                                }

                                if (!result)
                                {
                                    lbl_Message.Text = "儲位無此商品！1";
                                }
                            }
                            //處理完再塞回去(2014-0110新增)
                            Session["ShelfList"] = list;

                            #endregion 輸入數量幾個就跑幾次(2013-0124修改)

                            #region ●顯示可再移，未確認數量

                            lbl_Message.Text = "此產品可再移：" + (NowProductNum - qua) + " , 撿貨中：" + 撿貨未確認.Count;

                            #endregion ●顯示可再移，未確認數量
                        }
                        else
                        {
                            lbl_Message.Text = "此商品剩餘數量為(" + NowProductNum + ")！" + " , 撿貨中：" + 撿貨未確認.Count;
                        }
                    }
                    else
                    {
                        if (撿貨未確認.Count > 0)
                            lbl_Message.Text = "此商品剩餘數量為(0)！" + " , 撿貨中：" + 撿貨未確認.Count;
                        else
                            lbl_Message.Text = "儲位無商品！3";
                    }
                }
                else
                {
                    lbl_Message.Text = "產品條碼錯誤！4";
                }
            }
            txt_Num.Text = "";
            txt_Input.Text = "";
            txt_Input.Focus();
        }

        #endregion 主功能-儲位對儲位部份移動

        #region ●查詢儲位內容(2014-0221獨立出來)

        /// <summary>
        /// 查詢儲位內容
        /// 門市驗完貨就算過帳，所以移動時只要看是否還在檢貨中
        /// </summary>
        /// <param name="str_input"></param>
        protected bool SeachShelf(string str_input)
        {
            #region 顯示 撿貨未確認

            var 目前儲位內容 = sp.GetStorageConfig(str_input, _areaId);
            //Session["ShelfList"] = 目前儲位內容;
            var 目前儲位內容數 = 目前儲位內容 == null ? 0 : 目前儲位內容.Sum(x => x.Quantity);
            var 撿貨未確認 = sp.GetTempStorageDetail(str_input, "", _areaId).ToList();
            lbl_ShelfQuantity.Text = string.Format("可移：{0}, 撿貨中：{1}", 目前儲位內容數, 撿貨未確認.Count);
            //----------------------------------------------
            if (撿貨未確認.Count > 0)
            {
                lbl_Message.Text = "此儲位含有撿貨未確認的產品!!";
                return true;
            }
            else
            {
                return false;
            }

            #endregion 顯示 撿貨未確認/傳票未過帳
        }

        #endregion ●查詢儲位內容(2014-0221獨立出來)

        #region 主功能-確認移動產品

        /// <summary>
        /// 送出 確認移動產品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                string str_From = lbl_FromStorage_NO.Text;
                string str_Target = lbl_TargetStorage_NO.Text;
                int? fromShelfType = sp.CheckStorage(str_From, _areaId);
                if (fromShelfType == null)
                {
                    lbl_Message.Text = str_From + " 不存在 請設定！";
                }

                String[] productList = lbl_Product.Text.Split('<');
                List<String> productL = new List<String>();

                foreach (var data in productList)
                {
                    var newData = data.Replace("br />", "");
                    if (newData != "")
                        productL.Add(newData);
                }

                if (productL.Count > 0)
                {
                    //此功能不會是入庫
                    //但可能是無效儲位入有效儲位
                    var isImport = false;  
                    result = sp.MergeStorage(str_From, str_Target, productL, account, _areaId, isImport);

                    if (result.Result == "1")
                    {
                        lbl_Message.Text = result.Reason + " " + str_Target + " 上 " + productL.Count + " 件";

                        lbl_Product.Text = "";
                        lbl_CurrentNum.Text = "0";
                        lbl_FromStorage_NO.Text = (CB_LockFrom.Checked) ? lbl_FromStorage_NO.Text : "";
                        lbl_FromStage_NO_Type.Text = (CB_LockFrom.Checked) ? lbl_FromStage_NO_Type.Text : "";
                        lbl_TargetStorage_NO.Text = "";
                        lbl_TargetStorage_NO_Type.Text = "";

                        btn_Submit.Visible = false;

                        Session["ShelfList"] = null;
                        //鎖定的話要重讀(2013-0515新增)
                        if (CB_LockFrom.Checked)
                            Session["ShelfList"] = sp.GetStorageConfig(lbl_FromStorage_NO.Text, _areaId);
                    }
                    else
                    {
                        lbl_Product.Text = "";
                        lbl_Message.Text = "失敗! 請檢查<br >【" + str_From + "," + str_Target + "】<br >儲位內容是否正確!<br >" + result.Reason;
                    }
                }
                else
                {
                    lbl_Message.Text = "未刷任何產品!";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-確認移動產品
    }
}