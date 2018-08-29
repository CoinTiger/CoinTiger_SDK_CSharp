// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：MarketDetail
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.Text;

namespace CoinTiger
{
    // 前24小时行情
    public class MarketDetail
    {
        public string symbol = null;    // 交易对
        public double amount = 0.0;     // 交易额
        public double vol = 0.0;        // 交易量
        public double high = 0.0;       // 最高价
        public double low = 0.0;        // 最低价
        public double rose = 0.0;       // 涨幅
        public double close = 0.0;      // 收盘价
        public double open = 0.0;       // 开盘价
        public Int64 ts = 0;            // 数据产生时间，单位：毫秒

        public static MarketDetail FromString(string strResponseData)
        {
            Json.Dictionary dict = Json.ToDictionary(strResponseData);
            if (dict == null)
                return null;

            MarketDetail marketDetail = new MarketDetail();
            marketDetail.symbol = dict["symbol"];
            if (string.IsNullOrEmpty(marketDetail.symbol))
                return null;

            Json.Dictionary trade_ticker_data = Json.ToDictionary(Json.GetAt(dict, "trade_ticker_data"));
            marketDetail.ts = Int64.Parse(trade_ticker_data["ts"]);

            Json.Dictionary tick = Json.ToDictionary(trade_ticker_data["tick"]);
            marketDetail.amount = double.Parse(tick["amount"]);
            marketDetail.vol = double.Parse(tick["vol"]);
            marketDetail.high = double.Parse(tick["high"]);
            marketDetail.low = double.Parse(tick["low"]);
            marketDetail.rose = double.Parse(tick["rose"]);
            marketDetail.close = double.Parse(tick["close"]);
            marketDetail.open = double.Parse(tick["open"]);

            return marketDetail;
        }
    }
}
