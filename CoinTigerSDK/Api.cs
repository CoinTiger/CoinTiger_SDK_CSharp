// ************************************************************************** //
// 项目名称：CoinTigerSDK
// 项目描述：
// 类 名 称：Api、Account、Response 等
// 说    明：
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
//using Newtonsoft.Json;

namespace CoinTiger
{
    public static class Api
    {
        // 查询系统当前时间
        // 系统时间戳是以一个Int64整数表示的
        // 其值是 1970年1月1月00时00分00秒 算起的毫秒数
        public static Timestamp GetTimestamp()
        {
            string strResponse = Http.Get(V2.URL + "/timestamp");
            if (string.IsNullOrEmpty(strResponse))
                return null;

            Response response = Response.FromString(strResponse);
            if (response == null || response.code != "0" || response.msg != "suc" || string.IsNullOrEmpty(response.data))
                return null;

            Timestamp timestamp = Timestamp.FromString(response.data);
            if (timestamp == null || timestamp.system_current_time <= 0)
                return null;

            return timestamp;
        }

        // 查询cointiger站支持的所有币种
        // 币种是按交易分区来列举的，如：
        //   bitcny-partition
        //   btc-partition
        //   usdt-partition
        //   eth-partition
        public static Currencys GetCurrencys()
        {
            string strResponse = Http.Get(V2.URL + "/currencys");
            if (string.IsNullOrEmpty(strResponse))
                return null;

            Response response = Response.FromString(strResponse);
            if (response == null || response.code != "0" || response.msg != "suc" || string.IsNullOrEmpty(response.data))
                return null;

            Currencys currencys = Currencys.FromString(response.data);
            if (currencys == null || currencys.partitions == null)
                return null;

            return currencys;
        }

        // 前24小时行情
        // symbol: 交易对, 形如 tchbtc, ethbtc,btcbitcny,eosbtc...
        public static MarketDetail GetMarketDetail(string symbol)
        {
            string strResponse = Http.Get(V1.URL + "/market/detail", string.Format("symbol={0}", symbol));
            if (string.IsNullOrEmpty(strResponse))
                return null;

            Response response = Response.FromString(strResponse);
            if (response == null || response.code != "0" || response.msg != "suc" || string.IsNullOrEmpty(response.data))
                return null;

            MarketDetail marketDetail = MarketDetail.FromString(response.data);
            if (marketDetail == null || marketDetail.ts <= 0)
                return null;

            return marketDetail;
        }

        // 前24小时行情 (适用于行情展示平台使用)
        // 这个可以一次性把所有币种的都获取了
        public static PublicMarketDetail GetPublicMarketDetail()
        {
            string strResponse = Http.Get("https://www.cointiger.com/exchange/api/public/market/detail");
            if (string.IsNullOrEmpty(strResponse))
                return null;

            PublicMarketDetail publicMarketDetail = PublicMarketDetail.FromString(strResponse);
            if (publicMarketDetail == null || publicMarketDetail.items == null || publicMarketDetail.items.Count <= 0)
                return null;

            return publicMarketDetail;
        }

        // 深度盘口
        // symbol: 交易对, 形如 tchbtc, ethbtc,btcbitcny,eosbtc...
        // type  : Depth 类型, 深度有3个维度, "step0", "step1", "step2"
        public static MarketDepth GetMarketDepth(string symbol, string type)
        {
            string strResponse = Http.Get(V1.URL + "/market/depth", string.Format("symbol={0}&type={1}", symbol, type));
            if (string.IsNullOrEmpty(strResponse))
                return null;

            Response response = Response.FromString(strResponse);
            if (response == null || response.code != "0" || response.msg != "suc" || string.IsNullOrEmpty(response.data))
                return null;

            MarketDepth marketDepth = MarketDepth.FromString(response.data);
            if (marketDepth == null || marketDepth.ts <= 0)
                return null;

            return marketDepth;
        }

        // K线历史数据
        // symbol: 交易对, 形如 tchbtc, ethbtc,btcbitcny,eosbtc...
        // period: K线类型, 形如 1min,5min,15min,30min,60min,1day,1week,1month
        // size  : 获取数量, 取值范围[1,2000]
        public static MarketKLine GetMarketKLine(string symbol, string period, int size)
        {
            string strResponse = Http.Get(V1.URL + "/market/history/kline", string.Format("symbol={0}&period={1}&size={2}", symbol, period, size));
            if (string.IsNullOrEmpty(strResponse))
                return null;

            Response response = Response.FromString(strResponse);
            if (response == null || response.code != "0" || response.msg != "suc" || string.IsNullOrEmpty(response.data))
                return null;

            MarketKLine marketKLine = MarketKLine.FromString(response.data);
            if (marketKLine == null || string.IsNullOrEmpty(marketKLine.symbol))
                return null;

            return marketKLine;
        }

        // 成交历史数据
        // symbol: 交易对, 形如 tchbtc, ethbtc,btcbitcny,eosbtc...
        // size  : 获取数量, 取值范围[1,2000]
        public static MarketTrade GetMarketTrade(string symbol, int size)
        {
            string strResponse = Http.Get(V1.URL + "/market/history/trade", string.Format("symbol={0}&size={1}", symbol, size));
            if (string.IsNullOrEmpty(strResponse))
                return null;

            Response response = Response.FromString(strResponse);
            if (response == null || response.code != "0" || response.msg != "suc" || string.IsNullOrEmpty(response.data))
                return null;

            MarketTrade marketTrade = MarketTrade.FromString(response.data);
            if (marketTrade == null || string.IsNullOrEmpty(marketTrade.symbol))
                return null;

            return marketTrade;
        }

        // 获取资金状况
        // account: 账户信息, 内含ApiKey及Secret
        // coin   : 选填, 如果不填返回所有币种, 形如 btc,eth... 需要小写
        public static Balance GetBalance(Account account, string coin)
        {
            // 排序：coin、time
            string strParam = string.IsNullOrEmpty(coin)
                ? string.Format("time={0}", GetServerTime())
                : string.Format("coin={0}&time={1}", coin, GetServerTime());
            string strSignedParam = string.Format("api_key={0}&{1}&sign={2}", account.apikey, strParam, Crypt.GetSignature(strParam, account.secret));
            string strResponse = Http.Get(V1.URL + "/user/balance", strSignedParam);
            if (string.IsNullOrEmpty(strResponse))
                return null;

            Response response = Response.FromString(strResponse);
            if (response == null || response.code != "0" || response.msg != "suc" || string.IsNullOrEmpty(response.data))
                return null;

            Balance balance = Balance.FromString(response.data);
            if (balance == null || balance.items == null)
                return null;

            return balance;
        }

        // 查询某个订单详情
        // symbol  : 交易对, 形如 btcbitcny, bchbtc, eoseth ...
        // order_id: 订单ID
        public static OrderDetail GetOrderDetail(Account account, string symbol, long order_id)
        {
            // 排序：order_id、symbol、time
            string strParam = string.Format("order_id={0}&symbol={1}&time={2}", order_id, symbol, GetServerTime());
            string strSignedParam = string.Format("api_key={0}&{1}&sign={2}", account.apikey, strParam, Crypt.GetSignature(strParam, account.secret));
            string strResponse = Http.Get(V2.URL + "/order/details", strSignedParam);
            if (string.IsNullOrEmpty(strResponse))
                return null;

            Response response = Response.FromString(strResponse);
            if (response == null || response.code != "0" || response.msg != "suc" || string.IsNullOrEmpty(response.data))
                return null;

            OrderDetail orderDetail = OrderDetail.FromString(response.data);
            if (orderDetail == null || orderDetail.id <= 0)
                return null;

            return orderDetail;
        }

        // 查询当前委托、历史委托
        // symbol: 交易对, 形如 btcbitcny, bchbtc, eoseth ...
        // types : 查询的订单类型组合, 使用','分割, buy-market:市价买, sell-market:市价卖, buy-limit:限价买, sell-limit:限价卖, 可省略, 默认表示全部
        // states: 查询的订单状态组合, 使用','分割, new:新订单, part_filled:部分成交, filled:完全成交, canceled:已撤销, expired:异常订单
        // from  : 查询起始ID（订单ID）, 配合direct使用, 可省略
        // direct: 查询方向, prev向前, next向后, 可省略, 默认next
        // size  : 查询记录大小, 最多一次查询50条数据, 可省略, 默认50
        public static OrdersDetail GetOrdersDetail(Account account, string symbol, string types, string states, int from = 0, int direct = 0, int size = 0)
        {
            // 排序：direct、from、size、states、symbol、time、types
            string strParam = string.Format("{0}{1}{2}{3}{4}{5}{6}",
                direct == 0 ? "" : string.Format("direct={0}&", direct > 0 ? "next" : "prev"),
                from == 0 ? "" : string.Format("from={0}&", from),
                size == 0 ? "" : string.Format("size={0}&", size),
                string.Format("states={0}", states),
                string.Format("&symbol={0}", symbol),
                string.Format("&time={0}", GetServerTime()),
                string.IsNullOrEmpty(types) ? "" : string.Format("&types={0}", types)
                );
            strParam = strParam.TrimStart('&'); // 有可能省略direct，此时会多出一个前导的'&'符
            string strSignedParam = string.Format("api_key={0}&{1}&sign={2}", account.apikey, strParam, Crypt.GetSignature(strParam, account.secret));
            string strResponse = Http.Get(V2.URL + "/order/orders", strSignedParam);
            if (string.IsNullOrEmpty(strResponse))
                return null;

            Response response = Response.FromString(strResponse);
            if (response == null || response.code != "0" || response.msg != "suc" || string.IsNullOrEmpty(response.data))
                return null;

            OrdersDetail ordersDetail = OrdersDetail.FromString(response.data);
            if (ordersDetail == null || ordersDetail.items == null)
                return null;

            return ordersDetail;
        }

        // 查询当前成交、历史成交
        // symbol   : 交易对, 形如 btcbitcny, bchbtc, rcneth ...
        // startDate: 查询开始日期, 起始日期和结束日志时间最大间隔7天
        // endDate  : 查询结束日期, 起始日期和结束日志时间最大间隔7天
        // from     : 查询起始ID（撮合ID）, 配合direct使用, 可省略
        // direct   : 查询方向, prev向前, next向后, 可省略
        // size     : 查询记录大小, 最多一次查询50条数据, 可省略, 默认50
        public static MatchResults GetMatchResults(Account account, string symbol, DateTime startDate, DateTime endDate, int from = 0, int direct = 0, int size = 0)
        {
            // 排序：direct、end-date、from、size、start-date、symbol、time
            string strParam = string.Format("{0}{1}{2}{3}{4}{5}{6}",
                direct == 0 ? "" : string.Format("direct={0}&", direct > 0 ? "next" : "prev"),
                string.Format("end-date={0}", endDate.ToString("yyyy-MM-dd")),
                from == 0 ? "" : string.Format("&from={0}", from),
                size == 0 ? "" : string.Format("&size={0}", size),
                string.Format("&start-date={0}", startDate.ToString("yyyy-MM-dd")),
                string.Format("&symbol={0}", symbol),
                string.Format("&time={0}", GetServerTime())
                );
            string strSignedParam = string.Format("api_key={0}&{1}&sign={2}", account.apikey, strParam, Crypt.GetSignature(strParam, account.secret));
            string strResponse = Http.Get(V2.URL + "/order/match_results", strSignedParam);
            if (string.IsNullOrEmpty(strResponse))
                return null;

            Response response = Response.FromString(strResponse);
            if (response == null || response.code != "0" || response.msg != "suc" || string.IsNullOrEmpty(response.data))
                return null;

            MatchResults matchResults = MatchResults.FromString(response.data);
            if (matchResults == null || matchResults.items == null)
                return null;

            return matchResults;
        }

        // 查询某个订单的成交明细
        // symbol  : 交易对, 形如 btcbitcny, bchbtc, eoseth...
        // order_id: 订单ID
        public static MakeDetail GetMakeDetail(Account account, string symbol, long order_id)
        {
            // 排序：order_id、symbol、time
            string strParam = string.Format("order_id={0}&symbol={1}&time={2}", order_id, symbol, GetServerTime());
            string strSignedParam = string.Format("api_key={0}&{1}&sign={2}", account.apikey, strParam, Crypt.GetSignature(strParam, account.secret));
            string strResponse = Http.Get(V2.URL + "/order/make_detail", strSignedParam);
            if (string.IsNullOrEmpty(strResponse))
                return null;

            Response response = Response.FromString(strResponse);
            if (response == null || response.code != "0" || response.msg != "suc" || string.IsNullOrEmpty(response.data))
                return null;

            MakeDetail makeDetail = MakeDetail.FromString(response.data);
            if (makeDetail == null || makeDetail.items == null)
                return null;

            return makeDetail;
        }

        // 创建订单
        // symbol: 交易对, 形如 tchbtc,ltcbtc,ethbitcny ...
        // price : 下单价格
        // volume: 下单数量
        // side  : 买卖方向 买BUY 卖SELL
        // type  : 1:限价交易, 2:市价交易
        public static Order PlaceOrder(Account account, string symbol, double price, double volume, string side, int type)
        {
            // 排序：price、side、symbol、time、type、volume
            string strParam = string.Format("price={0}&side={1}&symbol={2}&time={3}&type={4}&volume={5}",
                price.ToString("F15").TrimEnd('0'), // 下单价格
                side.ToUpper(),                     // 买卖方向 买BUY 卖SELL
                symbol,                             // 交易对 tchbtc, ltcbtc, ethbitcny...
                GetServerTime(),                    // 当前时间戳
                type,                               // 1 ：限价交易，2：市价交易
                volume.ToString("F0"));             // 下单数量
            string strSignedParam = string.Format("api_key={0}&{1}&sign={2}", account.apikey, strParam, Crypt.GetSignature(strParam, account.secret));
            string strResponse = Http.Post(V2.URL + "/order", strSignedParam);
            if (string.IsNullOrEmpty(strResponse))
                return null;

            Response response = Response.FromString(strResponse);
            if (response == null || response.code != "0" || response.msg != "suc" || string.IsNullOrEmpty(response.data))
                return null;

            Order order = Order.FromString(response.data);
            if (order == null || order.order_id < 0)
                return null;

            return order;
        }

        // 取消订单
        // symbol  : 交易对, 形如 tchbtc,ltcbtc,ethbitcny ...
        // order_id: 订单ID
        public static bool CancelOrder(Account account, string symbol, long order_id)
        {
            // 排序：order_id、symbol、time
            string strParam = string.Format("order_id={0}&symbol={1}&time={2}", order_id, symbol, GetServerTime());
            string strSignedParam = string.Format("api_key={0}&{1}&sign={2}", account.apikey, strParam, Crypt.GetSignature(strParam, account.secret));
            string strResponse = Http.Delete(V1.URL + "/order", strSignedParam);
            if (string.IsNullOrEmpty(strResponse))
                return false;

            Response response = Response.FromString(strResponse);
            if (response == null || response.code != "0" || response.msg != "suc" || string.IsNullOrEmpty(response.data))
                return false;

            return true;
        }

        #region 服务器时间

        // 需要账户和签名的API接口，都需要带time参数
        // 应尽量对齐使用服务器时间
        // 但为了减少不必要的向服务器询问时间的操作
        // 把某一次问服务器得到的时间记下来，并记下当时的本地时间
        // 由此可以随时推知服务器的当前时间
        public static Int64 GetServerTime()
        {
            if (timeServer == 0)
            {
                Timestamp timestamp = GetTimestamp();
                if (timestamp != null && timestamp.system_current_time > 0)
                {
                    timeServer = timestamp.system_current_time;
                    timeLocal = GetLocalTime();
                }
            }

            Int64 tLocal = GetLocalTime();
            return timeServer + (tLocal - timeLocal);
        }

        // 读当前时间, in ms, from 1970-01-01 00:00:00.000
        public static Int64 GetLocalTime()
        {
            return System.DateTime.UtcNow.Ticks / 10000 - Jan1st1970Ms;
        }

        // 接口中使用的各处时间戳，是以 1970-01-01 00:00:00.000 起的毫秒数来表示的
        // 此处提供函数转换为 System.DateTime
        public static DateTime TimestampToDataTime(Int64 ts)
        {
            System.DateTime dt = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            dt = dt + System.TimeSpan.FromMilliseconds(ts);
            return dt;
        }

        private static Int64 timeServer = 0;
        private static Int64 timeLocal = 0;
        private static Int64 Jan1st1970Ms = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks / 10000;

        #endregion
    }

    public static class V1
    {
        private static string url = "https://api.cointiger.pro/exchange/trading/api";
        public static string URL
        {
            get { return url; }
        }
    }

    public static class V2
    {
        private static string url = "https://api.cointiger.pro/exchange/trading/api/v2";
        public static string URL
        {
            get { return url; }
        }
    }

    // CoinTiger账户
    // 读取操作账户信息时需要使用
    public class Account
    {
        public string name = null;      // 使用时可以关联一个账户名称，如 13812345678
        public string apikey = null;    // ApiKey, 登陆CoinTiger后从个人信息的API管理里申请
        public string secret = null;    // Secret, 登陆CoinTiger后从个人信息的API管理里申请
    }

    // API接口的通用返回消息
    // code: 返回码, 0表示成功, 其他为错误码
    // msg : suc表示成功
    // data: 实际载荷
    public class Response
    {
        public string code = null;
        public string msg = null;
        public string data = null;
        public static Response FromString(string strResponse)
        {
            Json.Dictionary dict = Json.ToDictionary(strResponse);
            if (dict == null)
                return null;

            Response res = new Response();
            res.code = Json.GetAt(dict, "code");
            res.msg = Json.GetAt(dict, "msg");
            res.data = Json.GetAt(dict, "data");
            return res;
        }
    }

    #region 内部支持函数

    // 封装Http接口，使用到的有三类：
    // GET
    // POST
    // DELETE
    internal static class Http
    {
        public static string Get(string strUrl, string strData = null)
        {
            string strResponse = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl + (string.IsNullOrEmpty(strData) ? "" : "?") + strData);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamResponse, Encoding.ASCII);
                strResponse = streamReader.ReadToEnd();
                streamReader.Close();
                streamResponse.Close();
            }
            catch (WebException ex)
            {
                string strError = ex.Message;
                return null;
            }

            return strResponse;
        }

        public static string Post(string strUrl, string strData)
        {
            string strResult = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = Encoding.ASCII.GetByteCount(strData);
                Stream streamRequest = request.GetRequestStream();
                StreamWriter streamWriter = new StreamWriter(streamRequest, Encoding.ASCII);
                streamWriter.Write(strData);
                streamWriter.Close();
                streamRequest.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamResponse, Encoding.ASCII);
                strResult = streamReader.ReadToEnd();
                streamReader.Close();
                streamResponse.Close();
            }
            catch (WebException ex)
            {
                string strError = ex.Message;
                return null;
            }

            return strResult;
        }

        public static string Delete(string strUrl, string strData)
        {
            string strResponse = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl + (string.IsNullOrEmpty(strData) ? "" : "?") + strData);
                request.Method = "DELETE";
                request.ContentType = "text/html;charset=UTF-8";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamResponse, Encoding.ASCII);
                strResponse = streamReader.ReadToEnd();
                streamReader.Close();
                streamResponse.Close();
            }
            catch (WebException ex)
            {
                string strError = ex.Message;
                return null;
            }

            return strResponse;
        }
    }

    // 封装签名计算过程
    internal static class Crypt
    {
        // 计算签名
        // param : 形如 key1=value1&key2=value2&...
        // secret: cointiger上申请到的Secret
        // 签名规则：
        // sign=HMAC-SHA512(key1+value1+key2+value2+secret)
        // 所有参数的key按字母升序排列，为避免排序问题，key只能使用小写字母
        // 注：参数中，api_key不算签名参数
        public static string GetSignature(string param, string secret)
        {
            // 去掉 & 和 = 号，拼上 secret，得到待签名串
            // 为简单计，虽然规则要求key要按字母序排列
            // 但考虑到每个API需要的参数是固定的
            // 所以我们可以在拼装format的时候就按字母序排好
            param = param.Replace("&", "");
            param = param.Replace("=", "");
            param = param + secret;

            byte[] bytesSecret = Encoding.ASCII.GetBytes(secret);
            byte[] bytesParam = Encoding.ASCII.GetBytes(param);
            HMACSHA512 hmacsha512 = new HMACSHA512(bytesSecret);
            byte[] bytesSignedParam = hmacsha512.ComputeHash(bytesParam);

            string signedParam = BitConverter.ToString(bytesSignedParam).Replace("-", "").ToLower();
            return signedParam;
        }
    }

    // 封闭Json的解析
    // 注：
    // 此处只是实现了简单的Jgon字典和列表的解析
    // 鉴于API返回信息都比较规整简洁，这样的实现已经够用了
    // 如果要更详尽的解析支持，可以在这里面替换成 Newtonsoft.Json 库来实现
    // 另一方面，考虑到应用到Android时，添加 Newtonsoft.Json 是一定的负担，此处默认提供简洁版的实现
    internal static class Json
    {
        public class Dictionary : System.Collections.Generic.Dictionary<string, string> { };
        public static Dictionary ToDictionary(string strText)
        {
            strText = strText.Trim();
            Debug.Assert(!string.IsNullOrEmpty(strText));
            if (strText[0] != '{' || strText[strText.Length - 1] != '}')
                return null;

            Dictionary dict = new Dictionary();
            strText = strText.Substring(1, strText.Length - 2); // remove '{' & '}'
            int begin = 0;
            int end = strText.Length;
            while (begin < end)
            {
                int beginKey = begin;
                int endKey = beginKey;
                MatchStack stackKey = new MatchStack();
                while (endKey < end && (!stackKey.IsEmpty() || strText[endKey] != ':'))
                {
                    stackKey.Push(strText[endKey]);
                    endKey++;
                }
                Debug.Assert(endKey < end);
                string key = strText.Substring(beginKey, endKey - beginKey).Trim();
                Debug.Assert(key.Length < 2 || (key[0] == '"') == (key[key.Length - 1] == '"'));
                if (key.Length >= 2 && key[0] == '"' && key[key.Length - 1] == '"')
                    key = key.Substring(1, key.Length - 2);

                int beginValue = endKey + 1;
                int endValue = beginValue;
                MatchStack stackValue = new MatchStack();
                while (endValue < end && (!stackValue.IsEmpty() || strText[endValue] != ','))
                {
                    stackValue.Push(strText[endValue]);
                    endValue++;
                }
                string value = strText.Substring(beginValue, endValue - beginValue).Trim();
                Debug.Assert(value.Length < 2 || (value[0] == '"') == (value[value.Length - 1] == '"'));
                if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
                    value = value.Substring(1, value.Length - 2);

                begin = endValue + 1;

                Debug.Assert(!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value));
                dict.Add(key, value);
            }

            return dict;
        }

        public static string GetAt(Dictionary dict, string key)
        {
            if (dict == null)
                return null;
            if (!dict.ContainsKey(key))
                return null;
            return dict[key];
        }

        public class Array : System.Collections.Generic.List<string> { };
        public static Array ToArray(string strText)
        {
            strText = strText.Trim();
            Debug.Assert(!string.IsNullOrEmpty(strText));
            if (strText[0] != '[' || strText[strText.Length - 1] != ']')
                return null;

            Array array = new Array();
            strText = strText.Substring(1, strText.Length - 2); // remove '[' & ']'
            int begin = 0;
            int end = strText.Length;
            while (begin < end)
            {
                int beginItem = begin;
                int endItem = beginItem;
                MatchStack stackKey = new MatchStack();
                while (endItem < end && (!stackKey.IsEmpty() || strText[endItem] != ','))
                {
                    stackKey.Push(strText[endItem]);
                    endItem++;
                }
                string item = strText.Substring(beginItem, endItem - beginItem).Trim();
                Debug.Assert(item.Length < 2 || (item[0] == '"') == (item[item.Length - 1] == '"'));
                if (item.Length >= 2 && item[0] == '"' && item[item.Length - 1] == '"')
                    item = item.Substring(1, item.Length - 2);

                begin = endItem + 1;

                Debug.Assert(!string.IsNullOrEmpty(item));
                array.Add(item);
            }

            return array;
        }

        public static string GetAt(Array array, int index)
        {
            if (array == null)
                return null;
            if (index < 0 || index >= array.Count)
                return null;
            return array[index];
        }

        // 匹配同级的左右括号，[] {}
        // 不考虑有语法错误的串
        // 不考虑实现内容中包含了括号
        public class MatchStack
        {
            private string strStack;
            public bool IsEmpty() { return string.IsNullOrEmpty(strStack); }
            public void Push(char ch)
            {
                if (ch != '\"' && ch != '{' && ch != '}' && ch != '[' && ch != ']')
                    return;

                Debug.Assert(!string.IsNullOrEmpty(strStack) || ch != '}' && ch != ']');
                if (string.IsNullOrEmpty(strStack) && ch != '}' && ch != ']')
                {
                    strStack += ch;
                    return;
                }

                if (ch == '\"')
                {
                    if (strStack[strStack.Length - 1] == '\"')
                        strStack = strStack.Substring(0, strStack.Length - 1);
                    else
                        strStack += ch;
                }
                if (ch == '{' || ch == '[')
                {
                    strStack += ch;
                }
                if (ch == '}')
                {
                    Debug.Assert(strStack[strStack.Length - 1] == '{');
                    if (strStack[strStack.Length - 1] == '{')
                        strStack = strStack.Substring(0, strStack.Length - 1);
                }
                if (ch == ']')
                {
                    Debug.Assert(strStack[strStack.Length - 1] == '[');
                    if (strStack[strStack.Length - 1] == '[')
                        strStack = strStack.Substring(0, strStack.Length - 1);
                }
            }
        }
    }

    #endregion
}
