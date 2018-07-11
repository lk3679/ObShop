using Newtonsoft.Json;
using OBShopWeb.Poslib;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace OBShopWeb
{
    public partial class ActivitiesSetting : System.Web.UI.Page
    {
        public DataTable ActivitiesDT=new DataTable();
        public string act = "";
        public string ActivityID = "";
        public CheckOut.Clerk ck = new CheckOut.Clerk();
        public bool DiscountNPriceChanged = false;
        public bool ActivityRight = false;
        public string Type = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            Auth();
            act = Request["act"];
            ActivityID = string.IsNullOrEmpty(Request["ActivityID"]) ? "" : Request["ActivityID"];
            string SerialID = string.IsNullOrEmpty(Request["SerialID"]) ? "" : Request["SerialID"];            

            if (!IsPostBack)
            {
                switch (act)
                {
                    case "add":
                        ActivitiesDiv.Visible = false;
                        AddDiv.Visible = true;
                        EditDiv.Visible = false;
                        ActType.Items[0].Selected = true;
                        VIPOnlyRadioAdd.Items[0].Selected = true;
                        if (ActivityRight == false)
                        {
                            ActType.Items[0].Enabled = false;
                            ActType.Items[1].Enabled = false;
                            ActType.Items[2].Selected = true;
                        }
                        break;
                    case "edit":
                        ActivitiesDiv.Visible = false;
                        AddDiv.Visible = false;
                        EditDiv.Visible = true;
                        EditMode(ActivityID);
                        DiscountNPriceChanged = Activity.IsDiscountNPriceChanged(ActivityID);
                        break;
                    case "deleteActivity":
                        Activity.DeleteActivity(ActivityID);
                        break;
                    case "deleteSerialID":
                        Activity.DeleteActivitySerialID(ActivityID, SerialID,ck.ID);
                        break;
                    default:
                        ActivitiesDiv.Visible = true;
                        AddDiv.Visible = false;
                        EditDiv.Visible = false;
                        ActivitiesDT = Activity.GetAllActitivities();
                        break;
                }
            }
            
        }

        protected void SendBtn_Click(object sender, EventArgs e)
        {
            Auth();
            string ErrorMsg = CheckEmptyErrorString(act);
            if (ErrorMsg.Length > 0)
            {
                Result_lbl.Text = ErrorMsg;
                return;
            }

            bool CanApplyWithOthers = false;
            if (ActType.SelectedValue == "新品折X元")
            {
                CanApplyWithOthers = true;
            }

            bool Result = Activity.AddNewActivity(NameTextBox.Text, ActType.SelectedValue, StartDate.Text, EndDate.Text, ck.ID, ParamListString.Value, VIPOnlyRadioAdd.SelectedValue, CanApplyWithOthers);

            if (Result)
                Result_lbl.Text = "新增成功!";
            else
                Result_lbl.Text = "新增失敗!";

            NameTextBox.Text = "";
            StartDate.Text = "";
            EndDate.Text = "";
            ParamListString.Value = "";
        }

        protected void EditBtn_Click(object sender, EventArgs e)
        {
            Auth();
            string ErrorMsg = CheckEmptyErrorString(act);
            if (ErrorMsg.Length > 0)
            {
                Result_lbl.Text = ErrorMsg;
                return;
            }

            bool CanApplyWithOthers = false;
            if (EditActType.SelectedValue == "新品折X元")
            {
                CanApplyWithOthers = true;
            }

            string ActivityID = string.IsNullOrEmpty(Request["ActivityID"]) ? "" : Request["ActivityID"];
            bool Result = Activity.UpdateActivity(ActivityID, EditNameTextBox.Text, EditActType.SelectedValue, EditStartDate.Text, EditEndDate.Text, ck.ID, EditParamListString.Value, RadioButtonListActivityProductType.SelectedValue, VIPOnlyRadioEdit.SelectedValue, CanApplyWithOthers);

            if (Result)
                Result_lbl.Text = "更新成功!";
            else
                Result_lbl.Text = "更新失敗!";
        }

        protected void ImportBtn_Click(object sender, EventArgs e)
        {
            Auth();
            ImportSerialID();
        }

        protected void RemoveAllBtn_Click(object sender, EventArgs e)
        {
            Auth();
            string ActivityID = string.IsNullOrEmpty(Request["ActivityID"]) ? "" : Request["ActivityID"];
            bool result = Activity.DeleteAllSerialID(ActivityID);

            if (result)
                Result_lbl.Text = "刪除成功!";
            else
                Result_lbl.Text = "刪除失敗!";

            SerialIDListLoadData(ActivityID);
        }

        void EditMode(string ActivityID)
        {
            ActivitiesDT = Activity.GetAllActitivities();
            IEnumerable<DataRow> query = (from a in ActivitiesDT.AsEnumerable()
                                          where a["ActivityID"].ToString() == ActivityID
                                          select a).ToList();

            if (query.Count() > 0)
            {
                ActivitiesDT = query.CopyToDataTable();
                EditNameTextBox.Text = ActivitiesDT.Rows[0]["Name"].ToString();
                Type= ActivitiesDT.Rows[0]["Type"].ToString();
                string ActivityProductType = ActivitiesDT.Rows[0]["ActivityProductType"].ToString();
                bool VIPOnly = false;
                bool.TryParse(ActivitiesDT.Rows[0]["VIPOnly"].ToString(),out VIPOnly);

                if (VIPOnly)
                    VIPOnlyRadioEdit.Items[1].Selected = true;
                else
                    VIPOnlyRadioEdit.Items[0].Selected = true;

                foreach (ListItem item  in EditActType.Items)
                {
                    item.Enabled = false;
                    if (item.Value == Type)
                        item.Selected = true;
                }

                RadioButtonListActivityProductType.Items[0].Selected = true;
                if (Type == "新品折X元")
                {
                    RadioButtonListActivityProductType.Items[1].Enabled = false;
                }

                foreach (ListItem item in RadioButtonListActivityProductType.Items)
                {
                    if (item.Value == ActivityProductType)
                        item.Selected = true;
                }

                EditStartDate.Text = Convert.ToDateTime(ActivitiesDT.Rows[0]["StartDate"]).ToString("yyyy/MM/dd");
                EditEndDate.Text = Convert.ToDateTime(ActivitiesDT.Rows[0]["EndDate"]).ToString("yyyy/MM/dd");
                DateTime today = DateTime.Today;
                if (today > Convert.ToDateTime(ActivitiesDT.Rows[0]["EndDate"]))
                {
                    EditStartDate.Enabled = false;
                    EditEndDate.Enabled = false;
                }

                EditParamListString.Value = ActivitiesDT.Rows[0]["Parameters"].ToString();
                SerialIDListLoadData(ActivityID);
            }
        }

        protected void SerialDataLis_ItemDataBound(Object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemIndex > -1)
            {                   
                     Label SerialIDLabel=(Label)e.Item.FindControl("SerialIDLabel");
                     SerialIDLabel.Text = DataBinder.Eval(e.Item.DataItem, "ProductSerialID").ToString();
            }
        }

        public void SerialIDListLoadData(string ActivityID)
        {
            DataTable dt = Activity.GetSerialID(ActivityID);
            SerialDataList.DataSource = dt;
            SerialDataList.DataBind();
        }

        public string CheckEmptyErrorString(string act)
        {
            if (act == "add")
            {
                if (NameTextBox.Text == "")
                    return "名稱不可為空";
                if (StartDate.Text == "")
                    return "開始日期不可為空";
                if (EndDate.Text == "")
                    return "結束日期不可為空";
                if (Activity.ValidateDate(StartDate.Text) == false || Activity.ValidateDate(EndDate.Text) == false)
                   return   "日期格式錯誤";
                if (DateTime.Parse(EndDate.Text) < DateTime.Parse(StartDate.Text))
                    return "結束日期不可小於開始日期";
                if (Activity.CheckHasActivitySamePeroid(DateTime.Parse(StartDate.Text), DateTime.Parse(EndDate.Text), "", bool.Parse(VIPOnlyRadioAdd.SelectedValue), ActType.SelectedValue))
                    return "此期間已有同類型的折扣活動，請更改時間";
                if (ParamListString.Value == "[]")
                    return "折扣設定不可為空";
                else
                    return "";
            }
            else
            {
                if (EditNameTextBox.Text == "")
                    return "名稱不可為空";
                if (EditStartDate.Text == "")
                    return "開始日期不可為空";
                if (EditEndDate.Text == "")
                    return "結束日期不可為空";
                if (Activity.ValidateDate(EditStartDate.Text) == false || Activity.ValidateDate(EditEndDate.Text) == false)
                    return "日期格式錯誤";
                if (DateTime.Parse(EditEndDate.Text) < DateTime.Parse(EditStartDate.Text))
                    return "結束日期不可小於開始日期";
                if (Activity.CheckHasActivitySamePeroid(DateTime.Parse(EditStartDate.Text), DateTime.Parse(EditEndDate.Text), ActivityID, bool.Parse(VIPOnlyRadioEdit.SelectedValue), EditActType.SelectedValue))
                    return "此期間已有同類型的折扣活動，請更改時間";
                if (EditParamListString.Value == "[]")
                    return "折扣設定不可為空";
                else
                    return "";
            }
        }

        public void Auth()
        {
            if (!string.IsNullOrEmpty(Request["ClerkID"]))
            {
                Session["ClerkID"]= Request["ClerkID"];
            }

            if (Session["Account"] == null)
            {
                Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                Response.End();
            }
            else
            {
                ck.Name = Session["Account"].ToString();
                GetMemberRight();

                if (Session["ClerkID"] == null)
                {
                    Response.Write("閒置時間過久，請重新登入<br/>");
                    Response.Write("<a href=\"logout.aspx\">登出</a>");
                    Response.End();
                }
            }

            ck.ID = Session["ClerkID"].ToString();
            
        }

        #region 匯入折價商品系列編號

        public void ImportSerialID()
        {
            bool result = false;
            string[] SerialList = ImportTextBox.Text.Split('\n');
            string ActivityID = string.IsNullOrEmpty(Request["ActivityID"]) ? "" : Request["ActivityID"];

            if (HasAddSerialIDRight(SerialList) == false)
            {
                Result_lbl.Text = "新品折X元，商品類型不可超過30筆";
                return;
            }

            foreach (string Serial in SerialList)
            {
                string SerialID = Serial.Replace("\r", "").Trim(' ');

                if (!string.IsNullOrEmpty(SerialID))
                {
                    result = Activity.AddSerialID(ActivityID, SerialID, int.Parse(RadioButtonListActivityProductType.SelectedValue));
                }
                   
            }

            if (result)
            {
                //寫LOG
                Activity.AddLog(ActivityID, ck.ID);
                ImportTextBox.Text = "";
                Result_lbl.Text = "匯入成功!";
            }
            else
            {
                Result_lbl.Text = "匯入失敗!";
            }

            SerialIDListLoadData(ActivityID);
        }

        public bool HasAddSerialIDRight(string[] SerialList)
        {
            bool right = true;
            string ActivityProductType = Activity.FindActivityType(ActivityID);

            //新品折X元，商品類型不可超過30筆
            if (ActivityProductType == "新品折X元")
            {
                //計算現有筆數和匯入筆數是否超過30
                DataTable SerialIDList = Activity.GetSerialID(ActivityID);
                if (SerialIDList.Rows.Count + SerialList.Length > 60)
                {
                    right = false;
                }
            }

            return right;

        }
        
        #endregion


        public void GetMemberRight()
        {
            string XMLFile = HttpContext.Current.Server.MapPath("ActivityRight.xml");

            if (File.Exists(XMLFile) == false)
            { return; }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(XMLFile);
                XmlNodeList MemberList = doc.DocumentElement.SelectNodes("/MemberList/Member");

                foreach (XmlNode node in MemberList)
                {
                    string ID=node.SelectSingleNode("ID").InnerText;
                    string account = node.SelectSingleNode("Account").InnerText;
                    if (account.ToUpper() == ck.Name.ToUpper())
                    {
                        ck.ID = ID;
                        Session["ClerkID"] = ck.ID;
                        ActivityRight = true;
                    }
                }
            }
            catch (Exception e)
            {
                string errorMsg = e.ToString();
                Response.Write(errorMsg);
            }
        }       
    }
}