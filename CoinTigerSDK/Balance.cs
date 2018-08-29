// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：Balance
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.Text;

namespace CoinTiger
{
    // 资金状况
    public class Balance
    {
        public class Item
        {
            public string coin = null;                      // 币种名称
            public double normal = 0.0;                     // 可用资金
            public double locK = 0.0;                       // 冻结资金
        };
        public System.Collections.Generic.List<Item> items = null;

        public static Balance FromString(string strResponseData)
        {
            Json.Array array = Json.ToArray(strResponseData);
            if (array == null)
                return null;

            Balance balance = new Balance();
            if (array.Count > 0)
            {
                balance.items = new System.Collections.Generic.List<Item>();
                for (int i = 0; i < array.Count; i++)
                {
                    Item item = new Item();
                    string strItem = Json.GetAt(array, i);
                    Json.Dictionary dict = Json.ToDictionary(strItem);
                    item.coin = Json.GetAt(dict, "coin");
                    item.normal = Convert.ToDouble(Json.GetAt(dict, "normal"));
                    item.locK = Convert.ToDouble(Json.GetAt(dict, "lock"));
                    balance.items.Add(item);
                }
            }

            return balance;
        }
    }
}
