using System;
using System.Collections.Generic;

namespace OBShopWeb.publics
{
    public class Ship
    {
        public static List<ShippingModel> GetShippingBase(EnumTypes.ShopType type)
        {
            var shippings = new List<ShippingModel>();
            switch (type)
            {
                case EnumTypes.ShopType.拍賣:
                    shippings.Add(CreateShippingModel(type, "Y拍掛號包裹", "0"));
                    shippings.Add(CreateShippingModel(type, "Y拍黑貓宅即便", "2"));
                    shippings.Add(CreateShippingModel(type, "Y拍7-11取貨", "12"));
                    shippings.Add(CreateShippingModel(type, "Y拍中華郵政國際包裹", "11"));
                    shippings.Add(CreateShippingModel(type, "虎門(A0)", "17"));
                    shippings.Add(CreateShippingModel(type, "自取(A1)", "401"));
                    shippings.Add(CreateShippingModel(type, "自取(橘熊)", "402"));
                    shippings.Add(CreateShippingModel(type, "自取(員購)", "403"));
                    shippings.Add(CreateShippingModel(type, "購物中心快捷", "7"));
                    shippings.Add(CreateShippingModel(type, "購物中心郵寄", "19"));
                    shippings.Add(CreateShippingModel(type, "購物中心黑貓", "29"));
                    shippings.Add(CreateShippingModel(type, "購物中心 7-11", "37"));
                    shippings.Add(CreateShippingModel(type, "購物中心全家", "55"));
                    shippings.Add(CreateShippingModel(type, "超級商城郵寄", "20"));
                    shippings.Add(CreateShippingModel(type, "超級商城黑貓", "21"));
                    shippings.Add(CreateShippingModel(type, "超級商城 7-11取貨", "32"));
                    shippings.Add(CreateShippingModel(type, "MOMO 7-11取貨", "31"));
                    shippings.Add(CreateShippingModel(type, "MOMO 黑貓", "38"));
                    shippings.Add(CreateShippingModel(type, "7NET 黑貓", "39"));
                    shippings.Add(CreateShippingModel(type, "7NET 7-11取貨", "34"));
                    shippings.Add(CreateShippingModel(type, "Y拍便利達康", "51"));
                    shippings.Add(CreateShippingModel(type, "超級商城全家", "52"));
                    shippings.Add(CreateShippingModel(type, "黑貓代收", "54"));
                    break;

                case EnumTypes.ShopType.官網:
                    shippings.Add(CreateShippingModel(type, "官網黑貓宅即便", "2"));
                    shippings.Add(CreateShippingModel(type, "官網7-11取貨", "12"));
                    shippings.Add(CreateShippingModel(type, "官網便利達康", "50"));
                    shippings.Add(CreateShippingModel(type, "官網黑貓代收", "53"));
                    shippings.Add(CreateShippingModel(type, "官網國買晉越", "62"));
                    break;

                default:
                    throw new Exception("error code");
                    break;
            }
            return shippings;
        }

        /// <summary>
        /// 取得工作類別的績效編碼
        /// </summary>
        /// <param name="type">工作類別</param>
        /// <param name="shop">商店種類</param>
        /// <returns></returns>
        public static int GetPerformanceCode(EnumTypes.WorkType type, EnumTypes.ShopType shop)
        {
            var result = (int)type;
            if (shop == EnumTypes.ShopType.官網)
                result += 100;
            return result;
        }

        /// <summary>
        /// 產生ShippingModel
        /// </summary>
        /// <param name="shopcode">shoptype code</param>
        /// <param name="name">名稱</param>
        /// <param name="value">參數</param>
        /// <returns></returns>
        private static ShippingModel CreateShippingModel(EnumTypes.ShopType shop, string name, string value)
        {
            return new ShippingModel() { ShopType = shop, Name = name, Value = value };
        }
    }
}