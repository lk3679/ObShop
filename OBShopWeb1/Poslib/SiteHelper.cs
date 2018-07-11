using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace OBShopWeb.Poslib
{
    public class SiteHelper
    {
        static public object GetCache(string CacheId)
        {
            object objCache = System.Web.HttpRuntime.Cache.Get(CacheId);
            return objCache;
        }

        /// <summary>
        /// 寫入 Cache 資料 ( 預設 60 秒 )
        /// </summary>
        /// <param name="CacheId"></param>
        /// <param name="objCache"></param>
        static public void SetCache(string CacheId, object objCache)
        {
            if (WebConfigurationManager.AppSettings["CacheDurationSeconds"] != null)
            {
                SetCache(CacheId, objCache,
                    Convert.ToInt32(WebConfigurationManager.AppSettings["CacheDurationSeconds"]));
            }
            else
            {
                SetCache(CacheId, objCache, 60);
            }
        }

        static public void SetCache(string CacheId, object objCache, int cacheDurationSeconds)
        {
            if (objCache != null)
            {
                System.Web.HttpRuntime.Cache.Insert(
                    CacheId,
                    objCache,
                    null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    new TimeSpan(0, 0, cacheDurationSeconds),
                    System.Web.Caching.CacheItemPriority.High,
                    null);
            }
        }
    }
}