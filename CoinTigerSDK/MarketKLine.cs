// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：MarketKLine
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.Text;

namespace CoinTiger
{
    // K线历史数据
    public class MarketKLine
    {
        public class Item
        {
            public Int64 id = 0;        // 时间刻度起始值
            public double amount = 0.0; // 交易额
            public double vol = 0.0;    // 交易量
            public double high = 0.0;   // 最高价
            public double low = 0.0;    // 最低价
            public double close = 0.0;  // 收盘价
            public double open = 0.0;   // 开盘价
        };
        public string symbol = null;
        public System.Collections.Generic.List<Item> kline_data = null;

        public static MarketKLine FromString(string strResponseData)
        {
            Json.Dictionary dict = Json.ToDictionary(strResponseData);
            if (dict == null)
                return null;

            MarketKLine marketKLine = new MarketKLine();
            marketKLine.symbol = dict["symbol"];
            if (string.IsNullOrEmpty(marketKLine.symbol))
                return null;

            Json.Array kline_data = Json.ToArray(Json.GetAt(dict, "kline_data"));
            foreach (string dataItem in kline_data)
            {
                Json.Dictionary dataItemDist = Json.ToDictionary(dataItem);
                Item item = new Item();
                item.id = Int64.Parse(dataItemDist["id"]);
                item.amount = double.Parse(dataItemDist["amount"]);
                item.vol = double.Parse(dataItemDist["vol"]);
                item.high = double.Parse(dataItemDist["high"]);
                item.low = double.Parse(dataItemDist["low"]);
                item.close = double.Parse(dataItemDist["close"]);
                item.open = double.Parse(dataItemDist["open"]);

                if (marketKLine.kline_data == null)
                    marketKLine.kline_data = new System.Collections.Generic.List<Item>();

                marketKLine.kline_data.Add(item);
            }

            return marketKLine;
        }
    }
}
