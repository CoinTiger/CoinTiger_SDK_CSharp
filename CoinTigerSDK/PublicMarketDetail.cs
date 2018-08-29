// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：PublicMarketDetail
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.Text;

namespace CoinTiger
{
    // 前24小时行情 (适用于行情展示平台使用)
    public class PublicMarketDetail
    {
        public class Item
        {
            public Int64 id = 0;                // 唯一标识
            public double baseVolume = 0.0;     // 交易量
            public double quoteVolume = 0.0;    // 交易额
            public double percentChange = 0.0;  // 涨跌幅
            public double last = 0.0;           // 最新价
            public double high24hr = 0.0;       // 24小时内最高价
            public double low24hr = 0.0;        // 24小时内最低价
            public double highestBid = 0.0;     // 买一价格
            public double lowestAsk = 0.0;      // 卖一价格
        };
        public System.Collections.Generic.Dictionary<string, Item> items;

        public static PublicMarketDetail FromString(string strResponseData)
        {
            Json.Dictionary dict = Json.ToDictionary(strResponseData);
            if (dict == null || dict.Count <= 0)
                return null;

            PublicMarketDetail publicMarketDetail = new PublicMarketDetail();
            publicMarketDetail.items = new System.Collections.Generic.Dictionary<string, Item>();
            foreach (var kv in dict)
            {
                Json.Dictionary detailItemDict = Json.ToDictionary(kv.Value);

                Item item = new Item();
                item.id = Int64.Parse(detailItemDict["id"]);
                item.baseVolume = double.Parse(detailItemDict["baseVolume"]);
                item.quoteVolume = double.Parse(detailItemDict["quoteVolume"]);
                item.percentChange = double.Parse(detailItemDict["percentChange"]);
                item.last = double.Parse(detailItemDict["last"]);
                item.high24hr = double.Parse(detailItemDict["high24hr"]);
                item.low24hr = double.Parse(detailItemDict["low24hr"]);
                item.highestBid = double.Parse(detailItemDict["highestBid"]);
                item.lowestAsk = double.Parse(detailItemDict["lowestAsk"]);
                publicMarketDetail.items.Add(kv.Key, item);
            }

            return publicMarketDetail;
        }
    }
}
