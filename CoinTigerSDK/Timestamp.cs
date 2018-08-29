// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：Timestamp
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.Text;

namespace CoinTiger
{
    // 系统时间戳
    // 是以一个Int64整数表示的
    // 其值是 1970年1月1月00时00分00秒 算起的毫秒数
    public class Timestamp
    {
        public Int64 system_current_time = 0;

        public static Timestamp FromString(string strResponseData)
        {
            Json.Dictionary dict = Json.ToDictionary(strResponseData);
            if (dict == null)
                return null;

            Timestamp timestamp = new Timestamp();
            string system_current_time = Json.GetAt(dict, "system_current_time");
            if (string.IsNullOrEmpty(system_current_time))
                return null;

            timestamp.system_current_time = Convert.ToInt64(system_current_time);
            return timestamp;
        }
    }
}
