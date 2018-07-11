using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class GetSerailProductByProductID : System.Web.UI.Page
    {
        public List<CheckOutProduct> ProductList;

        protected void Page_Load(object sender, EventArgs e)
        {
            string ProductID = Request["ProductID"];
            string PosNo="1";
            ProductList=CheckOut.GetTheSameSerialItemByProductID(ProductID, PosNo);
            
        }
    }
}