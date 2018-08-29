// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：Order
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.Text;

namespace CoinTiger
{
    // 创建订单的返回信息
    // 只包含一个订单号
    public class Order
    {
        public long order_id = 0;

        public static Order FromString(string strResponseData)
        {
            Json.Dictionary dict = Json.ToDictionary(strResponseData);
            if (dict == null)
                return null;

            Order order = new Order();
            if (!long.TryParse(Json.GetAt(dict, "order_id"), out order.order_id))
                return null;

            return order;
        }
    }
}
