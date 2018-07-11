using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OBShopWeb
{
    public partial class SystemInfo : System.Web.UI.Page
    {
        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var pDa = new POS_Library.ShopPos.ProductChangeDA();
                var result = pDa.GetPriceProductChange();
                //更新不須異動異動
                var ckProduct = result.Where(x => x.Print).ToList();
                if (ckProduct.Any())
                {
                    linkBtnProductChange.Text = "目前有產品價格被異動！共" + ckProduct.Count + "筆";
                }
            }
        }

        #endregion
         
    }
}