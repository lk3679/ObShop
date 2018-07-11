using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Data.ExceptionSource;
using System.Xml;
using System.Web;
using POS_Library.Public;

namespace OBShopWeb.Poslib
{
    static public class Area
    {
        public static string WmsAreaXml(string type)
        {
            var result = "";
            switch (type)
            {
                case "Area": result = Utility.setup_Area; break;
                case "ShopType": result = Utility.setup_ShopType; break;
                case "PrintPageSize": result = Utility.setup_PrintPageSize; break;
                case "changeColor": result = Utility.setup_changeColor; break;
                case "backgroundColor": result = Utility.setup_backgroundColor; break;
            }

            return result;
        }
    }
}