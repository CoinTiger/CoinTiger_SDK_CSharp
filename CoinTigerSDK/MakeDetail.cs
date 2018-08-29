// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：MakeDetail
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.Text;

namespace CoinTiger
{
    // 某个订单的成交明细
    public class MakeDetail
    {
        public class Item
        {
            public long id = 0;             // 撮合ID
            public double volume = 0.0;     // 成交数量
            public double price = 0.0;      // 成交价格
            public string symbol = null;    // 交易对
            public string type = null;      // 委托类型  buy-market:市价买, sell-market:市价卖, buy-limit:限价买, sell-limit:限价卖
            public string source = null;    // 订单来源  api
            public long orderId = 0;        // 订单ID
            public long bid_user_id = 0;    // 买单用户ID（如果该订单为买单，该字段不为空）
            public long ask_user_id = 0;    // 卖单用户ID（如果该订单为卖单，该字段不为空）
            public double buy_fee = 0.0;    // 手续费
            public double sell_fee = 0.0;   // 手续费
            public Int64 created = 0;       // 成交时间
        };
        public System.Collections.Generic.List<Item> items = null;

        public static MakeDetail FromString(string strResponseData)
        {
            Json.Array array = Json.ToArray(strResponseData);
            if (array == null)
                return null;

            MakeDetail makeDetail = new MakeDetail();
            makeDetail.items = new System.Collections.Generic.List<Item>();
            foreach (string dataItem in array)
            {
                Json.Dictionary dataItemDict = Json.ToDictionary(dataItem);

                Item item = new Item();
                item.id = long.Parse(dataItemDict["id"]);
                item.volume = double.Parse(dataItemDict["volume"]);
                item.price = double.Parse(dataItemDict["price"]);
                item.symbol = dataItemDict["symbol"];
                item.type = dataItemDict["type"];
                item.source = dataItemDict["source"];
                item.orderId = long.Parse(dataItemDict["orderId"]);
                long.TryParse(Json.GetAt(dataItemDict, "bid_user_id"), out item.bid_user_id);
                long.TryParse(Json.GetAt(dataItemDict, "ask_user_id"), out item.ask_user_id);
                double.TryParse(Json.GetAt(dataItemDict, "buy_fee"), out item.buy_fee);
                double.TryParse(Json.GetAt(dataItemDict, "sell_fee"), out item.sell_fee);
                item.created = Int64.Parse(dataItemDict["created"]);

                makeDetail.items.Add(item);
            }

            return makeDetail;
        }
    }
}
