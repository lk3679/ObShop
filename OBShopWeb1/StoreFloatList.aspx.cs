using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;

namespace OBShopWeb
{
    public partial class StoreFloatList : System.Web.UI.Page
    {
        public double FloatsDiscountRate = 0;
        public List<string> FloatList = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            GetStoreFloats();
        }

        public void GetStoreFloats()
        {
            string XMLFile = HttpContext.Current.Server.MapPath("StoreFloats.xml");

            if (File.Exists(XMLFile) == false)
            { return; }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(XMLFile);
                XmlNode discountNode = doc.SelectSingleNode("Discount");
                XmlElement element = (XmlElement)discountNode;
                double DiscountRate = int.Parse(element.GetAttribute("Rate"));
                XmlNodeList SerialList = doc.DocumentElement.SelectNodes("/Discount/SerialID");
                FloatsDiscountRate = (double)(DiscountRate / 100);

                foreach (XmlNode node in SerialList)
                {
                    string SerialID = node.InnerText;
                    if (SerialID.Contains("-"))
                    {
                        FloatList.Add(SerialID);
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