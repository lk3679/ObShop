using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.Poslib;
using Newtonsoft.Json;
using System.Text;

namespace OBShopWeb
{
    public partial class pos_vip : System.Web.UI.Page
    {
        public string act = "";
        public string vip_id = "";
        public string name = "";
        public string tel = "";
        public string mobile = "";
        public string email = "";
        public string Date_Month = "";
        public string Date_Day = "";
        public string new_vip_id = "";
        public string bonus = "";
        public string birthday = "";
        public string birthdayMonth = "";
        public string birthdayDay = "";
        public string last_birthday = "";
        public string valid_date = "";
        public string create_date = "";
        public DataTable VipInfoDT = new DataTable();
        public DataTable VipOrderDT = new DataTable();
        public int TotalOrderAmount = 0;
        public bool ShowExtend = false;
        public string ClerkID = "";
        public string PosNo = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            edie_vip_member.Visible = false;
            GetHttpRequest();        

            if (act == "_add")
            {
                if (mobile == "")
                {
                    string errorMsg = "開卡一定要填行動電話";
                    Response.Redirect("pos_result.aspx?result=" + false + "&errorMsg=" + errorMsg);
                    return;
                }

                bool HasTheSameVIP = VipMember.CheckHasTheSameVIP(mobile);
                if(HasTheSameVIP){
                    Response.Redirect("pos_result.aspx?result=" + false);
                }else{
                    birthday = string.Format("1999-{0}-{1}", Date_Month, Date_Day);
                    bool result = VipMember.add_pos_vip(vip_id, name, tel, mobile, email, birthday);
                    Response.Redirect("pos_result.aspx?result=" + result);
                }
                
            }
            else if (act == "_edit")
            {
                birthday = string.Format("1999-{0}-{1}", Date_Month, Date_Day);
                bool result=VipMember.edit_pos_vip(vip_id, name, tel, mobile, email, birthday);
                Response.Redirect("pos_result.aspx?result=" + result);
            }
            else if (act == "exchage")
            {
                bool result=VipMember.exchage_pos_vip(vip_id, new_vip_id);
                if (result)
                {
                    string msg = string.Format("新卡號:{0}", new_vip_id);
                    Log.Add(10, vip_id, msg, "VIP補卡", "", PosNo);
                }

                Response.Redirect("pos_result.aspx?result=" + result);
            }
            else if (act == "CancelVIP")
            {
                bool UpdateResult = VipMember.CancelVIP(vip_id);
                var result = new { result = UpdateResult };
                ShowResultOnPage(JsonConvert.SerializeObject(result));

            }else if(act=="CheckMobile"){

                bool HasTheSameVIP = VipMember.CheckHasTheSameVIP(mobile);
                var result = new { result = true, HasTheSameVIP = HasTheSameVIP };
                ShowResultOnPage(JsonConvert.SerializeObject(result));

            }
            else if (act == "GetOrderItem")
            {
                DataTable dt = VipMember.GetOrderItemByOrderID(Request["OrderID"]);
                string OrderItem = GetOrderItemString(dt);
                var result = new { result = dt.Rows.Count > 0, data = OrderItem };
                ShowResultOnPage(JsonConvert.SerializeObject(result));
            }
            else if (act == "VipExtend")
            {
                bool UpdateResult = VipMember.VipExtend(vip_id);
                if (UpdateResult)
                {
                    PosNoBiding();
                    Log.Add(9, vip_id, "上次的有效期限：" + valid_date, "VIP有效期限延長", ClerkID, PosNo);
                }
                var result = new { result = UpdateResult };
                ShowResultOnPage(JsonConvert.SerializeObject(result));
            }
            else if (vip_id != "")
            {
                vip_open.Visible = false;
                edie_vip_member.Visible = true;
                DataTable dt = VipMember.get_pos_vip(vip_id);
                
                if (dt.Rows.Count > 0)
                {
                    VipInfoDT = dt;
                    act = "edit";
                    SetValueFormDT(dt);
                    //判斷是否可以展期
                    if (VipMember.FindLastYearTotalAmountByVip(vip_id, valid_date) >= 5000)
                    { ShowExtend = true; }
                }
                else
                {
                    act = "add";
                }
            }
            else if (mobile != "")
            {
                vip_open.Visible = false;
                edie_vip_member.Visible = true;
                DataTable dt = VipMember.get_pos_vip_by_mobile(mobile);
                act = "edit";
                if (dt.Rows.Count > 0)
                {
                    VipInfoDT = dt;
                    SetValueFormDT(dt);
                    //判斷是否可以展期
                    if (VipMember.FindLastYearTotalAmountByVip(vip_id, valid_date) >= 5000)
                    { ShowExtend = true; }
                }
            }

            LoadOrder();
            
        }

        #region HTTP相關程式
        public void GetHttpRequest()
        {
            if (!string.IsNullOrEmpty(Request["act"]))
                act = Request["act"];
            if (!string.IsNullOrEmpty(Request["vip_id"]))
                vip_id = Request["vip_id"];
            if (!string.IsNullOrEmpty(Request["name"]))
                name = Request["name"];
            if (!string.IsNullOrEmpty(Request["tel"]))
                tel = Request["tel"];
            if (!string.IsNullOrEmpty(Request["mobile"]))
                mobile = Request["mobile"];
            if (!string.IsNullOrEmpty(Request["email"]))
                email = Request["email"];
            if (!string.IsNullOrEmpty(Request["Date_Month"]))
                Date_Month = Request["Date_Month"];
            if (!string.IsNullOrEmpty(Request["Date_Day"]))
                Date_Day = Request["Date_Day"];
            if (!string.IsNullOrEmpty(Request["new_vip_id"]))
                new_vip_id = Request["new_vip_id"];
            if (!string.IsNullOrEmpty(Request["ClerkID"]))
                ClerkID = Request["ClerkID"];
            if (!string.IsNullOrEmpty(Request["valid_date"]))
                valid_date = Request["valid_date"];
        }

        public void ShowResultOnPage(string JsonResult)
        {
            Response.Clear();
            Response.Write(JsonResult);
            Response.Flush();
            Response.End();
        }

        public void PosNoBiding()
        {
            string IPaddress = Request.UserHostAddress;
            PosNo = PosNumber.GetPosNumberMapping(IPaddress);
  
        }


        #endregion
        
        public string covertshortDate(string LongDate)
        {
           return(!string.IsNullOrEmpty(LongDate))?DateTime.Parse(LongDate).ToString("yyyy-MM-dd"):"";
        }

        public void SetValueFormDT(DataTable dt)
        {
            vip_id = dt.Rows[0]["vip_id"].ToString();
            name = dt.Rows[0]["name"].ToString();
            bonus = dt.Rows[0]["bonus"].ToString();
            tel = dt.Rows[0]["tel"].ToString();
            mobile = dt.Rows[0]["mobile"].ToString();
            bonus = dt.Rows[0]["bonus"].ToString();
            birthday = covertshortDate(dt.Rows[0]["birthday"].ToString());
            birthdayMonth = DateTime.Parse(birthday).ToString("MM");
            birthdayDay = DateTime.Parse(birthday).ToString("dd");
            last_birthday = covertshortDate(dt.Rows[0]["last_birthday"].ToString());
            if (last_birthday == "")
            { last_birthday = "1970-01-01"; }
            create_date = covertshortDate(dt.Rows[0]["create_date"].ToString());
            valid_date = covertshortDate(dt.Rows[0]["valid_date"].ToString());
        }

        public void LoadOrder()
        {
            if (vip_id.Length == 0)
                return;

            VipOrderDT = VipMember.GetOrderByVIP(vip_id);
           
            foreach (DataRow row in VipOrderDT.Rows)
            {
                int Amount = int.Parse(row["Amount"].ToString());
                TotalOrderAmount += Amount;
            }
        }

        public string GetOrderItemString(DataTable dt)
        {
            string HtmlTag = "<div id=\"content\"><h2>交易明細</h2><table class=\"EU_DataTable\"><tbody>";
            HtmlTag += "<tr><th></th><th>交易序號</th><th>產品編號</th><th>產品條碼</th><th>產品名稱</th><th>件數</th><th>價格</th><th>小計</th></tr>";
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                HtmlTag += "<tr>";
                HtmlTag += "<td>" + i + "</td>";
                HtmlTag += "<td>" + dr["OrderID"].ToString() + "</td>";
                HtmlTag += "<td>" + dr["ProductId"].ToString() + "</td>";
                HtmlTag += "<td>" + dr["BarCode"].ToString() + "</td>";
                HtmlTag += "<td>" + dr["Name"].ToString() + "</td>";
                HtmlTag += "<td>" + dr["Quantity"].ToString() + "</td>";
                HtmlTag += "<td>" + dr["Price"].ToString() + "</td>";
                HtmlTag += "<td>" + dr["Amount"].ToString() + "</td>";
                HtmlTag += "</tr>";
            }
            HtmlTag += "</tbody></table></div>";
            return HtmlTag;
        }

    }
}