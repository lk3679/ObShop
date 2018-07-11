using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace OBShopWeb.publics
{
    public class ShippingModel : BaseModel
    {
        public ShippingModel()
        {

        }

        public EnumTypes.ShopType ShopType { get; set; }
    }
}
