// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：OrdersDetail
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.Text;

namespace CoinTiger
{
    // 当前委托、历史委托
    // 内含一个列表
    // 列表中每一项是一个OrderDetail
    public class OrdersDetail
    {
        public System.Collections.Generic.List<OrderDetail> items = null;

        public static OrdersDetail FromString(string strResponseData)
        {
            Json.Array array = Json.ToArray(strResponseData);
            if (array == null)
                return null;

            OrdersDetail ordersDetail = new OrdersDetail();
            ordersDetail.items = new System.Collections.Generic.List<OrderDetail>();
            foreach (string dataItem in array)
            {
                Json.Dictionary dataItemDict = Json.ToDictionary(dataItem);

                OrderDetail item = new OrderDetail();
                item.user_id = long.Parse(Json.GetAt(dataItemDict, "user_id"));
                item.symbol = Json.GetAt(dataItemDict, "symbol");
                item.id = long.Parse(Json.GetAt(dataItemDict, "id"));
                item.type = Json.GetAt(dataItemDict, "type");
                item.volume = double.Parse(Json.GetAt(dataItemDict, "volume"));
                item.price = double.Parse(Json.GetAt(dataItemDict, "price"));
                double.TryParse(Json.GetAt(dataItemDict, "avg_price"), out item.avg_price);
                int.TryParse(Json.GetAt(dataItemDict, "status"), out item.status);
                double.TryParse(Json.GetAt(dataItemDict, "deal_volume"), out item.deal_volume);
                item.deal_money = double.Parse(Json.GetAt(dataItemDict, "deal_money"));
                item.fee = double.Parse(Json.GetAt(dataItemDict, "fee"));
                item.source = int.Parse(Json.GetAt(dataItemDict, "source"));
                item.ctime = Int64.Parse(Json.GetAt(dataItemDict, "ctime"));
                item.mtime = Int64.Parse(Json.GetAt(dataItemDict, "mtime"));

                ordersDetail.items.Add(item);
            }

            return ordersDetail;
        }
    }
}
