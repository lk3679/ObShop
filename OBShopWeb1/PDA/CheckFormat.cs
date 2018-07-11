using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using POS_Library.Public;
using POS_Library.ShopPos;
using OBShopWeb.Poslib;

namespace OBShopWeb.PDA
{
    public class CheckFormat
    {
        #region 宣告

        private ShelfProcess sp = new ShelfProcess();

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        #endregion 宣告

        #region 格式設定

        /// <summary>
        /// 格式代號
        /// </summary>
        public enum FormatName
        {
            Storage = 0,
            Product = 1,
            Number = 2,

            //分成In/Out(2013-0730修改)
            TInBox = 3,

            TOutBox = 4,
            HInBox = 5,
            HOutBox = 6
        }

        #endregion 格式設定

        #region 主功能-檢查格式

        /// <summary>
        /// 檢查格式
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CheckID(String ID, FormatName type)
        {
            try
            {
                switch (type)
                {
                    case FormatName.Storage:
                        if (Regex.IsMatch(ID, @"^([2-4]{1})([0-9A-Z]{1})([0-9A-Z]{2})([0-9A]{1})([0-9A]{1})$") || Regex.IsMatch(ID, @"^([A-Z]{1})([0-9]{8})$"))
                        {
                            return true;
                        }
                        break;

                    case FormatName.Product:
                        if (Regex.IsMatch(ID, @"^\d{8}$"))
                        {
                            return true;
                        }
                        break;

                    case FormatName.Number:
                        if (Regex.IsMatch(ID, @"^\d+$"))
                        {
                            return true;
                        }
                        break;
                    //分成In/Out(2013-0730修改)(2013-0910修改)
                    case FormatName.TInBox:
                        if (Regex.IsMatch(ID, @"^[H,A,S,D,E,F,G,K,L]([0-9]{11})$") || Regex.IsMatch(ID, @"^(\w)(\d{9})(T)$"))
                        {
                            return true;
                        }
                        break;

                    case FormatName.TOutBox:
                        if (Regex.IsMatch(ID, @"^[T]([0-9]{11})$") || Regex.IsMatch(ID, @"^(\w)(\d{9})([H,P,U])$"))
                        {
                            return true;
                        }
                        break;

                    case FormatName.HInBox:
                        if (Regex.IsMatch(ID, @"^[T,B,C]([0-9]{11})$") || Regex.IsMatch(ID, @"^(\w)(\d{9})(H)$"))
                        {
                            return true;
                        }
                        break;

                    case FormatName.HOutBox:
                        if (Regex.IsMatch(ID, @"^[H,A,S,B,C,D,E,F,G,K,L]([0-9]{11})$") || Regex.IsMatch(ID, @"^(\w)(\d{9})([T,H,P,U])$"))
                        {
                            return true;
                        }
                        break;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion 主功能-檢查格式

        #region 主功能-檢查格式(新)

        /// <summary>
        /// 檢查格式(新)
        /// </summary>
        /// <param name="fromShelfType"></param>
        /// <param name="targetType"></param>
        /// <param name="mergeType"></param>
        /// <returns></returns>
        public string CheckShelfType(int fromShelfType, string targetType, int mergeType)
        {
            string msg = string.Empty;
            switch (mergeType)
            {
                case (int)EnumData.MergeType.入庫上架:
                    if (string.IsNullOrEmpty(targetType))
                    {
                        var 來源暫存 = Utility.GetStorageTempAll().Contains(fromShelfType);
                        if (!來源暫存)
                        {
                            msg = ((Utility.StorageType)fromShelfType) + "，不能為來源儲位！<BR/>只可為：<BR/>『入庫暫存』儲位";
                        }
                    }
                    else
                    {
                        var targetShelfType = int.Parse(targetType);
                        var 目的無效 = Utility.GetStorageInvalidAll().Contains(targetShelfType);
                        if (目的無效 && (targetShelfType != (int)Utility.StorageType.寄倉儲位))
                        {
                            msg = string.Format("來源{0}不可入目的{1}儲位！<BR/>只可為：<BR/>『入庫暫存、寄倉』儲位", (Utility.StorageType)fromShelfType, (Utility.StorageType)targetShelfType);
                        }
                    }
                    break;

                case (int)EnumData.MergeType.合併儲位:
                case (int)EnumData.MergeType.移動儲位:
                    //主要暫存不可以上標準
                    if (string.IsNullOrEmpty(targetType))
                    {
                        //來源不可為 打銷儲位、不良儲位、問題儲位
                        //來源不可為 暫存
                        var 來源暫存 = Utility.GetStorageTempAll().Contains(fromShelfType);
                        if (mergeType == (int)EnumData.MergeType.移動儲位)
                        {
                            if (fromShelfType == (int)Utility.StorageType.打銷儲位|| fromShelfType == (int)Utility.StorageType.問題儲位 || 來源暫存)
                            {
                                msg = ((Utility.StorageType)fromShelfType) + "，不能為來源儲位！<BR/>不可為：<BR/>『打銷、問題、暫存』儲位";
                            }
                        }
                        else
                        {
                            if (fromShelfType == (int)Utility.StorageType.打銷儲位 || fromShelfType == (int)Utility.StorageType.不良儲位 || fromShelfType == (int)Utility.StorageType.問題儲位 )
                            {
                                msg = ((Utility.StorageType)fromShelfType) + "，不能為來源儲位！<BR/>不可為：<BR/>『打銷、不良、問題』儲位";
                            }
                        }
                        
                    }
                    else
                    {
                        //目的不可為 打銷儲位、問題儲位、暫存
                        //不良暫存儲位 只可移 不良儲位
                        //散貨暫存儲位 只可移 超散貨儲位
                        var targetShelfType = int.Parse(targetType);
                        var 目的暫存 = Utility.GetStorageTempAll().Contains(targetShelfType);
                        if ((targetShelfType == (int)Utility.StorageType.打銷儲位 || targetShelfType == (int)Utility.StorageType.問題儲位) || 目的暫存 ||
                            (fromShelfType == (int)Utility.StorageType.不良暫存儲位 && targetShelfType != (int)Utility.StorageType.不良儲位) ||
                            (fromShelfType == (int)Utility.StorageType.散貨暫存儲位 && targetShelfType != (int)Utility.StorageType.超散貨儲位))
                        {
                            msg = string.Format("來源{0}與目的{1}，不能移動儲位！<BR/>不可為：<BR/>『打銷、問題、暫存』儲位", (Utility.StorageType)fromShelfType, (Utility.StorageType)targetShelfType);
                        }
                    }
                    break;

                case (int)EnumData.MergeType.問題上架:
                    if (fromShelfType != (int)EnumData.StorageType.問題儲位)
                    {
                        msg = ((Utility.StorageType)fromShelfType) + "，不能為來源儲位！<BR/>只可為：<BR/>『問題』儲位";
                    }
                    break;

                case (int)EnumData.MergeType.盤點:
                case (int)EnumData.MergeType.盤單品:
                    //特定儲位可盤點
                    var 可盤點儲位 = Utility.GetStorageInventoryAll().Contains(fromShelfType);
                    if (!可盤點儲位)
                    {
                        msg = ((Utility.StorageType)fromShelfType) + "，不能為來源儲位！<BR/>只可為：<BR/>『標準、散貨、展售、補貨、過季<BR/>問題、不良、無貨、寄倉』儲位";
                    }
                    break;
                     //(int) POS_Library.ShopPos.EnumData.StorageType.標準儲位, (int) POS_Library.ShopPos.EnumData.StorageType.超散貨儲位,
                     //   (int) POS_Library.ShopPos.EnumData.StorageType.補貨儲位, (int) POS_Library.ShopPos.EnumData.StorageType.過季儲位,
                     //   (int) POS_Library.ShopPos.EnumData.StorageType.問題儲位, (int) POS_Library.ShopPos.EnumData.StorageType.不良儲位,
                     //   (int) POS_Library.ShopPos.EnumData.StorageType.無貨儲位, (int) POS_Library.ShopPos.EnumData.StorageType.展售儲位,
                     //   (int) POS_Library.ShopPos.EnumData.StorageType.寄倉儲位
            }
            return msg;
        }

        /// <summary>
        /// 檢查格式(新)
        /// </summary>
        /// <param name="fromShelfType"></param>
        /// <param name="targetType"></param>
        /// <param name="mergeType"></param>
        /// <returns></returns>
        public string CheckShelfTypeBak(int fromShelfType, string targetType, int mergeType)
        {
            string msg = string.Empty;
            switch (mergeType)
            {
                case (int)EnumData.MergeType.入庫上架:
                    if (string.IsNullOrEmpty(targetType))
                    {
                        var 來源暫存 = Utility.GetStorageTempAll().Contains(fromShelfType);
                        if (!來源暫存)
                        {
                            msg = ((Utility.StorageType)fromShelfType) + "，不能為來源儲位！";
                        }
                    }
                    else
                    {
                        var targetShelfType = int.Parse(targetType);
                        var 目的無效 = Utility.GetStorageInvalidAll().Contains(targetShelfType);
                        if (目的無效 && (targetShelfType != (int)Utility.StorageType.寄倉儲位))
                        {
                            msg = string.Format("來源{0}不可入目的{1}儲位！", (Utility.StorageType)fromShelfType, (Utility.StorageType)targetShelfType);
                        }
                    }
                    break;

                case (int)EnumData.MergeType.合併儲位:
                    //來源不可為 打銷儲位、不良儲位、問題儲位
                    //不良暫存儲位 只可移 不良儲位
                    //散貨暫存儲位 只可移 超散貨儲位
                    //
                    if (string.IsNullOrEmpty(targetType))
                    {
                        if (fromShelfType == (int)Utility.StorageType.打銷儲位 || fromShelfType == (int)Utility.StorageType.不良儲位)
                        {
                            msg = ((Utility.StorageType)fromShelfType) + "，不能為來源儲位！";
                        }
                    }
                    else
                    {
                        var targetShelfType = int.Parse(targetType);
                        var 來源暫存 = Utility.GetStorageTempAll().Contains(fromShelfType);
                        var 目的無效 = Utility.GetStorageInvalidAll().Contains(targetShelfType);
                        if (targetShelfType == (int)Utility.StorageType.問題儲位 || targetShelfType == (int)Utility.StorageType.打銷儲位 ||
                            (fromShelfType == (int)Utility.StorageType.不良暫存儲位 && targetShelfType != (int)Utility.StorageType.不良儲位) ||
                            (fromShelfType == (int)Utility.StorageType.散貨暫存儲位 && targetShelfType != (int)Utility.StorageType.超散貨儲位) ||
                            (fromShelfType == (int)EnumData.StorageType.標準暫存儲位) && 目的無效)
                        {
                            msg = string.Format("來源{0}與目的{1}，不能合併儲位！", (Utility.StorageType)fromShelfType, (Utility.StorageType)targetShelfType);
                        }
                    }
                    break;

                case (int)EnumData.MergeType.移動儲位:
                    //來源不可為暫存
                    //目的不可為暫存、問題不能移問題
                    if (string.IsNullOrEmpty(targetType))
                    {
                        var 來源暫存 = Utility.GetStorageTempAll().Contains(fromShelfType);
                        if (fromShelfType == (int)Utility.StorageType.打銷儲位 || 來源暫存)
                        {
                            msg = ((Utility.StorageType)fromShelfType) + "，不能為來源儲位！";
                        }
                    }
                    else
                    {
                        var targetShelfType = int.Parse(targetType);
                        var 目的暫存 = Utility.GetStorageTempAll().Contains(targetShelfType);
                        if ((targetShelfType == (int)Utility.StorageType.問題儲位 || targetShelfType == (int)Utility.StorageType.打銷儲位) || 目的暫存)
                        {
                            msg = string.Format("來源{0}與目的{1}，不能移動儲位！", (Utility.StorageType)fromShelfType, (Utility.StorageType)targetShelfType);
                        }
                    }
                    break;

                case (int)EnumData.MergeType.問題上架:
                    if (fromShelfType != (int)EnumData.StorageType.問題儲位)
                    {
                        msg = ((Utility.StorageType)fromShelfType) + "，不能為來源儲位！";
                    }
                    break;

                case (int)EnumData.MergeType.盤單品:
                    var 可盤點儲位 = Utility.GetStorageInventoryAll().Contains(fromShelfType);
                    if (!可盤點儲位)
                    {
                        msg = ((Utility.StorageType)fromShelfType) + "，不能為來源儲位！";
                    }
                    break;

                case (int)EnumData.MergeType.盤點:
                    var 無效2 = Utility.GetStorageInvalidAll().Contains(fromShelfType);
                    if (無效2)
                    {
                        msg = ((Utility.StorageType)fromShelfType) + "，不能為來源儲位！";
                    }
                    break;
            }
            return msg;
        }

        #endregion 主功能-檢查格式(新)

        #region 副功能-Type to Name

        /// <summary>
        /// 檢查合併的儲位條件
        /// </summary>
        /// <param name="From"></param>
        /// <param name="Target"></param>
        /// <returns></returns>
        public String TypeToName(int? type)
        {
            try
            {
                switch (type)
                {
                    case (int)EnumData.StorageType.標準儲位: return "(普通)";
                    case (int)EnumData.StorageType.超散貨儲位: return "(散貨)";
                    case (int)EnumData.StorageType.補貨儲位: return "(補貨)";
                    case (int)EnumData.StorageType.過季儲位: return "(過季)";
                    case (int)EnumData.StorageType.問題儲位: return "(問題)";
                    case (int)EnumData.StorageType.不良儲位: return "(不良)";
                    case (int)EnumData.StorageType.標準暫存儲位: return "(標準暫存)";
                    case (int)EnumData.StorageType.不良暫存儲位: return "(不良暫存)";
                    case (int)EnumData.StorageType.問題暫存儲位: return "(問題暫存)";
                    case (int)EnumData.StorageType.打銷儲位: return "(打銷)";
                    case (int)EnumData.StorageType.無貨儲位: return "(無貨)";
                    case (int)EnumData.StorageType.出貨暫存儲位: return "(出貨暫存)";
                    case (int)EnumData.StorageType.海運暫存儲位: return "(海運暫存)";
                    case (int)EnumData.StorageType.換貨暫存儲位: return "(換貨暫存)";
                    case (int)EnumData.StorageType.散貨暫存儲位: return "(散貨暫存)";
                    case (int)EnumData.StorageType.調回暫存儲位: return "(調回暫存)";
                    case (int)EnumData.StorageType.調出暫存儲位: return "(調出暫存)";
                    case (int)EnumData.StorageType.展售儲位: return "(展售)";
                    case (int)EnumData.StorageType.預購暫存: return "(預購暫存)";
                    case (int)EnumData.StorageType.寄倉儲位: return "(寄倉)";
                    default: return "(未設定種類)";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion 副功能-Type to Name

        #region 副功能-轉換儲位顯示格式

        /// <summary>
        /// 轉換儲位顯示格式 ID To Label
        /// </summary>
        /// <param name="From"></param>
        /// <param name="Target"></param>
        /// <returns></returns>
        public String TransShelfIdToLabel(String str)
        {
            try
            {
                //分9碼 或 其他
                var temp = (str.Length == 9) ? str.Substring(0, 3) + "-" + str.Substring(3, 2) + "-" + str.Substring(5, 2) + "-" + str.Substring(7, 2) : str;
                return temp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 轉換儲位顯示格式 Label To ID
        /// </summary>
        /// <param name="From"></param>
        /// <param name="Target"></param>
        /// <returns></returns>
        public String TransLabelToShelfId(String str)
        {
            try
            {
                return str.Replace("-", "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion 副功能-轉換儲位顯示格式
    }
}