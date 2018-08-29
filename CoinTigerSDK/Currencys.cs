// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：Currency
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.Text;

namespace CoinTiger
{
    // cointiger支持的某个币种和信息
    public class Currency
    {
        public string baseCurrency = null;              // 基础币种
        public string quoteCurrency = null;             // 计价币种
        public int pricePrecision = 0;                  // 价格精度位数（0为个位）
        public int amountPrecision = 0;                 // 数量精度位数（0为个位）
        public double withdrawFeeMin = 0.0;             // 提币手续费最小值
        public double withdrawFeeMax = 0.0;             // 提币手续费最大值
        public double withdrawOneMin = 0.0;             // 单笔提现最小限制
        public double withdrawOneMax = 0.0;             // 单笔提现最大限制
        public DepthSelect depthSelect = null;          // 深度选择配置
        public class DepthSelect
        {
            public double step0 = 0.0;
            public double step1 = 0.0;
            public double step2 = 0.0;
        }
    }

    // cointiger站支持的所有币种
    // 币种是按交易分区来列举的，如：
    //   bitcny-partition
    //   btc-partition
    //   usdt-partition
    //   eth-partition
    public class Currencys
    {
        public class Partition
        {
            public string name = null;
            public System.Collections.Generic.List<Currency> items = null;
        };
        public System.Collections.Generic.List<Partition> partitions = null;

        public static Currencys FromString(string strResponseData)
        {
            Json.Dictionary dict = Json.ToDictionary(strResponseData);
            if (dict == null)
                return null;

            Currencys currencys = new Currencys();
            foreach (var part in dict)
            {
                Partition partition = new Partition();
                partition.name = part.Key;
                Json.Array array = Json.ToArray(part.Value);
                if (array != null)
                {
                    partition.items = new System.Collections.Generic.List<Currency>();
                    foreach (var it in array)
                    {
                        Json.Dictionary dictItem = Json.ToDictionary(it);
                        Currency item = new Currency();
                        item.baseCurrency = Json.GetAt(dictItem, "baseCurrency");
                        item.quoteCurrency = Json.GetAt(dictItem, "quoteCurrency");
                        item.pricePrecision = Convert.ToInt32(Json.GetAt(dictItem, "pricePrecision"));
                        item.amountPrecision = Convert.ToInt32(Json.GetAt(dictItem, "amountPrecision"));
                        item.withdrawFeeMin = Convert.ToDouble(Json.GetAt(dictItem, "withdrawFeeMin"));
                        item.withdrawFeeMax = Convert.ToDouble(Json.GetAt(dictItem, "withdrawFeeMax"));
                        item.withdrawOneMin = Convert.ToDouble(Json.GetAt(dictItem, "withdrawOneMin"));
                        item.withdrawOneMax = Convert.ToDouble(Json.GetAt(dictItem, "withdrawOneMax"));
                        Json.Dictionary dictDS = Json.ToDictionary(Json.GetAt(dictItem, "depthSelect"));
                        item.depthSelect = new Currency.DepthSelect();
                        item.depthSelect.step0 = Convert.ToDouble(Json.GetAt(dictDS, "step0"));
                        item.depthSelect.step1 = Convert.ToDouble(Json.GetAt(dictDS, "step1"));
                        item.depthSelect.step2 = Convert.ToDouble(Json.GetAt(dictDS, "step2"));
                        partition.items.Add(item);
                    }
                }

                if (currencys.partitions == null)
                {
                    currencys.partitions = new System.Collections.Generic.List<Partition>();
                }

                currencys.partitions.Add(partition);
            }

            return currencys;
        }
    }
}
