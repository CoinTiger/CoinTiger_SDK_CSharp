// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：MarketTrade
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.Text;

namespace CoinTiger
{
    // 成交历史数据
    public class MarketTrade
    {
        public class Item
        {
            public int id = 0;          // 交易ID
            public string side = null;  // 买卖方向buy,sell
            public double price = 0.0;  // 单价
            public double vol = 0.0;    // 数量
            public double amount = 0.0; // 总额
            public double ts = 0.0;     // 数据产生时间
            public string ds = null;    // 格式化的数据产生时间
        };
        public string symbol = null;    // 交易对
        public System.Collections.Generic.List<Item> trade_data = null;

        public static MarketTrade FromString(string strResponseData)
        {
            Json.Dictionary dict = Json.ToDictionary(strResponseData);
            if (dict == null)
                return null;

            MarketTrade marketTrade = new MarketTrade();
            marketTrade.symbol = dict["symbol"];
            if (string.IsNullOrEmpty(marketTrade.symbol))
                return null;

            int size = int.Parse(dict["size"]);
            Json.Array trade_data = Json.ToArray(Json.GetAt(dict, "trade_data"));
            foreach (string dataItem in trade_data)
            {
                Json.Dictionary dataItemDist = Json.ToDictionary(dataItem);
                Item item = new Item();
                item.id = int.Parse(dataItemDist["id"]);
                item.side = dataItemDist["side"];
                item.price = double.Parse(dataItemDist["price"]);
                item.vol = double.Parse(dataItemDist["vol"]);
                item.amount = double.Parse(dataItemDist["amount"]);
                item.ts = Int64.Parse(dataItemDist["ts"]);
                item.ds = dataItemDist["ds"];

                if (marketTrade.trade_data == null)
                    marketTrade.trade_data = new System.Collections.Generic.List<Item>();

                marketTrade.trade_data.Add(item);
            }
            if (marketTrade.trade_data.Count != size)
                return null;

            return marketTrade;
        }
    }
}
