// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：MatchResults
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.Text;

namespace CoinTiger
{
    // 当前成交、历史成交
    public class MatchResults
    {
        public class Item
        {
            public long id = 0;         // 撮合ID
            public double price = 0.0;  // 委托价格
            public double volume = 0.0; // 成交数量
            public double fee = 0.0;    // 手续费
            public long orderId = 0;    // 订单ID
            public string symbol = null;// 交易对    btcbitcny, bchbtc, eoseth ...
            public string type = null;  // 委托类型  buy-market:市价买, sell-market:市价卖, buy-limit:限价买, sell-limit:限价卖
            public int status = 0;      // 订单状态  0或1:新订单, 2:完全成交, 3:部分成交, 4:已撤销, 6:异常订单
            public Int64 mtime = 0;     // 成交时间
            public string source = null;// 订单来源  api
        };
        public System.Collections.Generic.List<Item> items = null;

        public static MatchResults FromString(string strResponseData)
        {
            Json.Array array = Json.ToArray(strResponseData);
            if (array == null)
                return null;

            MatchResults matchResults = new MatchResults();
            matchResults.items = new System.Collections.Generic.List<Item>();
            foreach (string dataItem in array)
            {
                Json.Dictionary dataItemDict = Json.ToDictionary(dataItem);

                Item item = new Item();
                item.id = long.Parse(Json.GetAt(dataItemDict, "id"));
                item.price = double.Parse(Json.GetAt(dataItemDict, "price"));
                item.volume = double.Parse(Json.GetAt(dataItemDict, "volume"));
                item.fee = double.Parse(Json.GetAt(dataItemDict, "fee"));
                item.orderId = long.Parse(Json.GetAt(dataItemDict, "orderId"));
                item.symbol = Json.GetAt(dataItemDict, "symbol");
                item.type = Json.GetAt(dataItemDict, "type");
                item.status = int.Parse(Json.GetAt(dataItemDict, "status"));
                item.mtime = Int64.Parse(Json.GetAt(dataItemDict, "mtime"));
                item.source = Json.GetAt(dataItemDict, "source");

                matchResults.items.Add(item);
            }

            return matchResults;
        }
    }
}
