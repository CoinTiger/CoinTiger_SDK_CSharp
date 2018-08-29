// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：MarketDepth
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.Text;

namespace CoinTiger
{
    // 深度盘口
    public class MarketDepth
    {
        public class Item
        {
            public double price = 0.0;  // 成交价
            public double amount = 0.0; // 成交量
        };
        public string symbol = null;                                // 交易对
        public System.Collections.Generic.List<Item> buys = null;   // 买盘，按price降序
        public System.Collections.Generic.List<Item> asks = null;   // 卖盘, 按price升序
        public Int64 ts = 0;                                        // 消息生成时间，单位：毫秒

        public static MarketDepth FromString(string strResponseData)
        {
            Json.Dictionary dict = Json.ToDictionary(strResponseData);
            if (dict == null)
                return null;

            MarketDepth marketDepth = new MarketDepth();
            marketDepth.symbol = dict["symbol"];
            if (string.IsNullOrEmpty(marketDepth.symbol))
                return null;

            Json.Dictionary depth_data = Json.ToDictionary(Json.GetAt(dict, "depth_data"));
            marketDepth.ts = Int64.Parse(depth_data["ts"]);

            Json.Dictionary tick = Json.ToDictionary(depth_data["tick"]);
            Json.Array buys = Json.ToArray(tick["buys"]);
            Json.Array asks = Json.ToArray(tick["asks"]);
            foreach (string buyItem in buys)
            {
                Json.Array buyItemArray = Json.ToArray(buyItem);
                Item item = new Item();
                item.price = double.Parse(buyItemArray[0]);
                item.amount = double.Parse(buyItemArray[1]);

                if (marketDepth.buys == null)
                    marketDepth.buys = new System.Collections.Generic.List<Item>();

                marketDepth.buys.Add(item);
            }
            foreach (string askItem in asks)
            {
                Json.Array askItemArray = Json.ToArray(askItem);
                Item item = new Item();
                item.price = double.Parse(askItemArray[0]);
                item.amount = double.Parse(askItemArray[1]);

                if (marketDepth.asks == null)
                    marketDepth.asks = new System.Collections.Generic.List<Item>();

                marketDepth.asks.Add(item);
            }

            return marketDepth;
        }
    }
}
