using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBShopWeb.publics
{
	public class EnumTypes
	{
        public enum ShopType
        {
            拍賣 = 2,
            官網 = 3
        }

        public enum WorkType
        {
            撿貨 = 1,
            分貨 = 2,
            驗貨 = 3,
            包貨 = 4
        }
	}
}