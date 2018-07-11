using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OBShopWeb.Poslib;
using System.Text;
using Newtonsoft.Json;

namespace OBShopWeb
{
    public partial class pos_number_setting : System.Web.UI.Page
    {
        public DataTable PosNumberDT;

        protected void Page_Load(object sender, EventArgs e)
        {
            PosNumberDT=GetPOSNoData();
            LabelyourIP.Text = Request.UserHostAddress;
            string act=(string.IsNullOrEmpty(Request["act"]))?"":Request["act"];

            if (act == "DeletePosNo")
            {
                bool DeleteResult = DeletePOSNo(Request["IP"]);
                var result = new { result = DeleteResult };
                ShowResultOnPage(JsonConvert.SerializeObject(result));
            }
        }

        public void ShowResultOnPage(string JsonResult)
        {
            Response.Clear();
            Response.Write(JsonResult);
            Response.Flush();
            Response.End();
        }

        public DataTable GetPOSNoData()
        {
            string sql = "select a.ClientIP,a.PosNo,a.InvoiceMachineNo,a.PrintMachineNo from PosNumberMapping a ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            return dt;
        }

        public DataTable GetPOSNoDataByIP(string IP)
        {
            string sql = "select a.ClientIP,a.PosNo,a.InvoiceMachineNo,a.PrintMachineNo from PosNumberMapping a where  a.ClientIP=@IP ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("IP", IP);
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            return dt;
        }

        public bool DeletePOSNo(string IP)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Delete from PosNumberMapping where ClientIP=@IP ");
            string sql = sb.ToString();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("IP", IP);
            return DB.DBNonQuery(sql, param, "PosClient");
        }

        public bool InsertMachineNo(string IP, string PosNo, string MachineNo, string PrintMachineNo)
        {
            if (GetPOSNoDataByIP(IP).Rows.Count > 0)
                return false;

            StringBuilder sb = new StringBuilder();
            sb.Append("Insert into PosNumberMapping(ClientIP,PosNo,InvoiceMachineNo,PrintMachineNo)  ");
            sb.Append("values(@IP,@PosNo,@MachineNo,@PrintMachineNo)");
            string sql = sb.ToString();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("IP", IP);
            param.Add("PosNo", PosNo);
            param.Add("MachineNo", MachineNo);
            param.Add("PrintMachineNo", PrintMachineNo);
            return DB.DBNonQuery(sql, param, "PosClient");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            bool result = false;
            if (CheckEmpty() == true){
                result = InsertMachineNo(LabelyourIP.Text, TextBoxPOSNo.Text, TextBoxMachineNo.Text, TextBoxppPrinter.Text);
                Response.Redirect(Request.RawUrl);
            }
            else
            {
                LabelResult.Text = "資料不可為空";
                return;
            }
                
            if (result)
                LabelResult.Text = "新增成功";
            else
                LabelResult.Text = "新增失敗";
        }

        private bool CheckEmpty()
        {
            if (LabelyourIP.Text == "")
                return false;
            if (TextBoxPOSNo.Text == "")
                return false;
            if (TextBoxMachineNo.Text == "")
                return false;
            else
                return true;
        }
    }
}