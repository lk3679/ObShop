using OBShopWeb.Poslib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OBShopWeb
{
    public partial class SerilaNameManage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            DataTable ProductSerial = new DataTable();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  *FROM [PosClient].[dbo].[ProductSerial]");
            Dictionary<string, object> param = new Dictionary<string, object>();
            ProductSerial=DB.DBQuery(sb.ToString(), param, "PosClient");
            if (ProductSerial.Rows.Count > 0)
            {
                foreach(DataRow row in ProductSerial.Rows){
                    string SerialId = row["SerialId"].ToString();
                    string Name = GetSerialName(SerialId);
                    if (UpdateName(SerialId,Name))
                    {
                        Response.Write("更新ID" + SerialId + "成功，名稱為"+Name+"<br/>");
                    }
                    else
                    {
                        Response.Write("更新ID" + SerialId + "失敗!!<br/>");
                    }
                }
               
            }
           
        }

        public string  GetSerialName(string SerialId)
        {
            string Name = "";
            DataTable SerilalDT = new DataTable();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT [系列編號],系列名稱  FROM [orangebear].[dbo].[產品系列] a where a.系列編號=@SerialId ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("SerialId", SerialId);
            SerilalDT = DB.DBQuery(sb.ToString(), param, "orangebear");
            if (SerilalDT.Rows.Count > 0)
                Name = SerilalDT.Rows[0]["系列名稱"].ToString();

            return Name;
            
        }

        public bool  UpdateName(string SerialId,string Name)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("update [PosClient].[dbo].[ProductSerial]  set Name=@Name where SerialId=@SerialId  ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("SerialId", SerialId);
            param.Add("Name", Name);
           return DB.DBNonQuery(sb.ToString(), param, "PosClient");
        }
    }
}