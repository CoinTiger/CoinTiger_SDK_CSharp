// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：OrderDetail
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.Text;

namespace CoinTiger
{
    // 某个订单详情
    public class OrderDetail
    {
        //          参数名称                 是否必须  描述     取值范围
        public long id = 0;                 // true   订单id
        public long user_id = 0;            // true   账户ID
        public double volume = 0.0;         // true   订单数量
        public double deal_volume = 0.0;    // false  成交数量
        public double deal_money = 0.0;     // true   已成交金额
        public double fee = 0.0;            // true   手续费
        public double price = 0.0;          // true   限价单挂单价格
        public double avg_price = 0.0;      // *****  平均成交价格
        public int status = 0;              // false  订单状态  0或1:新订单, 2:完全成交, 3:部分成交, 4:已撤销, 6:异常订单
        public string type = null;          // true   订单类型  buy-market:市价买, sell-market:市价卖, buy-limit:限价买, sell-limit:限价卖
        public int source = 0;              // true   订单来源
        public string symbol = null;        // true   交易对    btcbitcny, eoseth, ethbtc ...
        public Int64 ctime = 0;             // true   订单创建时间
        public Int64 mtime = 0;             // true   最后成交时间

        public static OrderDetail FromString(string strResponseData)
        {
            Json.Dictionary dict = Json.ToDictionary(strResponseData);
            if (dict == null)
                return null;

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.id = long.Parse(Json.GetAt(dict, "id"));
            orderDetail.user_id = long.Parse(Json.GetAt(dict, "user_id"));
            orderDetail.volume = double.Parse(Json.GetAt(dict, "volume"));
            double.TryParse(Json.GetAt(dict, "deal_volume"), out orderDetail.deal_volume);
            orderDetail.deal_money = double.Parse(Json.GetAt(dict, "deal_money"));
            orderDetail.fee = double.Parse(Json.GetAt(dict, "fee"));
            orderDetail.price = double.Parse(Json.GetAt(dict, "price"));
            double.TryParse(Json.GetAt(dict, "avg_price"), out orderDetail.avg_price);
            int.TryParse(Json.GetAt(dict, "status"), out orderDetail.status);
            orderDetail.type = Json.GetAt(dict, "type");
            orderDetail.source = int.Parse(Json.GetAt(dict, "source"));
            orderDetail.symbol = Json.GetAt(dict, "symbol");
            orderDetail.ctime = Int64.Parse(Json.GetAt(dict, "ctime"));
            orderDetail.mtime = Int64.Parse(Json.GetAt(dict, "mtime"));

            return orderDetail;
        }
    }
}
