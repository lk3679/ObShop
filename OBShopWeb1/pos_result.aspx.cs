using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class pos_result : System.Web.UI.Page
    {
        public bool result;
        public string resultMsg = "";
        public string errorMsg = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["errorMsg"]))
            {
                errorMsg=Request["errorMsg"];
            }

            if (!string.IsNullOrEmpty(Request["result"]))
            {
                result = bool.Parse(Request["result"]);
                resultMsg = result ? "執行成功" : "執行失敗";

                if (errorMsg.Length > 0)
                {
                    errorMsg = "失敗原因：" + errorMsg;
                }
            }
            else
            {
                resultMsg = "回傳結果為空值";
            }

        }
    }
}