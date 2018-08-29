// ************************************************************************** //
// 项目名称：CoinTigerSDKTest
// 项目描述：配套CoinTigerSDK的测试工程
// 类 名 称：Program
// 说    明：实现对CoinTigerSDK的接口进行测试运行
// 作    者：Email@yangkaijin.cn
// 创建时间：2018-07-29
// 更新时间：2018-07-29
// ************************************************************************** //
using System;
using CoinTiger;

namespace CoinTigerSDKTester
{
    class Program
    {
        static void Main(string[] args)
        {
            // 可以把ApiKey和Secret填到这里编译进去，这样可以免去每次运行起来都会问要输入
            // 如果这里不改，则会在需要用到时提示输入
            // 注：只会问一遍，如果输错了，请关掉软件重新打开
            Account account = new Account();
            account.apikey = "KKK";
            account.secret = "SSS";

            while (true)
            {
                Console.WriteLine(new string('-', 79));
                Console.WriteLine("Menus:");
                Console.WriteLine("  1. GetTimestamp");
                Console.WriteLine("  2. GetCurrencys");
                Console.WriteLine("  3. GetMarketDetail");
                Console.WriteLine("  4. GetPublicMarketDetail");
                Console.WriteLine("  5. GetMarketDepth");
                Console.WriteLine("  6. GetMarketKLine");
                Console.WriteLine("  7. GetMarketTrade");
                Console.WriteLine("  8. GetBalance");
                Console.WriteLine("  9. GetOrderDetail");
                Console.WriteLine("  10.GetOrdersDetail");
                Console.WriteLine("  11.GetMatchResults");
                Console.WriteLine("  12.GetMakeDetail");
                Console.WriteLine("  13.PlaceOrder");
                Console.WriteLine("  14.CancelOrder");
                Console.WriteLine("  0. Exit");

                Console.Write("Select Munu: ");
                string select = Console.ReadLine();
                if (select == "0")
                    break;

                Console.WriteLine("");
                if (select == "1") TestGetTimestamp();
                if (select == "2") TestGetCurrencys();
                if (select == "3") TestGetMarketDetail();
                if (select == "4") TestGetPublicMarketDetail();
                if (select == "5") TestGetMarketDepth();
                if (select == "6") TestGetMarketKLine();
                if (select == "7") TestGetMarketTrade();
                if (select == "8") TestGetBalance(account);
                if (select == "9") TestGetOrderDetail(account);
                if (select == "10") TestGetOrdersDetail(account);
                if (select == "11") TestGetMatchResults(account);
                if (select == "12") TestGetMakeDetail(account);
                if (select == "13") TestPlaceOrder(account);
                if (select == "14") TestCancelOrder(account);

                // API上说明是说调用频率限制是 1秒内最多6次
                // 为保险计，此处每次间歇 350ms
                System.Threading.Thread.Sleep(350);
            }


            Console.WriteLine(new string('-', 79));
            Console.Write("Press Enter to Quit.  ");
            Console.ReadKey();
        }

        // 如果没有修改前面把ApiKey和Secret填到源码里编译，就从输入框要求输进来
        static void InsureAccount(Account account)
        {
            if (account.apikey == "KKK" || account.secret == "SSS")
            {
                Console.WriteLine("The following trading APIs need account:");
                Console.Write("  ApiKey = ");
                account.apikey = Console.ReadLine();
                Console.Write("  Secret = ");
                account.secret = Console.ReadLine();
                Console.WriteLine(new string('-', 79));
            }
        }

        static void TestGetTimestamp()
        {
            Timestamp timestamp = Api.GetTimestamp();
            if (timestamp == null)
            {
                Console.WriteLine("GetTimestamp Failed!");
                return;
            }

            Console.WriteLine("GetTimestamp Succeed:");
            Console.WriteLine("  system_current_time = {0}", timestamp.system_current_time);
        }

        static void TestGetCurrencys()
        {
            Currencys currencys = Api.GetCurrencys();
            if (currencys == null)
            {
                Console.WriteLine("GetCurrencys Failed!");
                return;
            }

            Console.WriteLine("GetCurrencys Succeed:");
            foreach (Currencys.Partition partition in currencys.partitions)
            {
                Console.WriteLine("{0}", partition.name);
                foreach (Currency currency in partition.items)
                {
                    Console.WriteLine("  {0}\t{1}\t{2}  {3}\t{4}\t{5}\t{6}\t{7:E}\t{8}\t{9}\t{10}",
                        currency.baseCurrency,
                        currency.quoteCurrency,
                        currency.pricePrecision,
                        currency.amountPrecision,
                        currency.withdrawFeeMin,
                        currency.withdrawFeeMax,
                        currency.withdrawOneMin,
                        currency.withdrawOneMax,
                        currency.depthSelect.step0,
                        currency.depthSelect.step1,
                        currency.depthSelect.step2);
                }
            }
        }

        static void TestGetMarketDetail()
        {
            MarketDetail marketDetail = Api.GetMarketDetail("ethbtc");
            if (marketDetail == null)
            {
                Console.WriteLine("GetMarketDetail Failed!");
                return;
            }

            Console.WriteLine("GetMarketDetail Succeed:");
            Console.WriteLine("  symbol = {0}", marketDetail.symbol);
            Console.WriteLine("  ts     = {0}", marketDetail.ts);
            Console.WriteLine("  amount = {0}", marketDetail.amount);
            Console.WriteLine("  vol    = {0}", marketDetail.vol);
            Console.WriteLine("  high   = {0}", marketDetail.high);
            Console.WriteLine("  low    = {0}", marketDetail.low);
            Console.WriteLine("  rose   = {0}", marketDetail.rose);
            Console.WriteLine("  close  = {0}", marketDetail.close);
            Console.WriteLine("  open   = {0}", marketDetail.open);
        }

        static void TestGetMarketDepth()
        {
            MarketDepth marketDepth = Api.GetMarketDepth("ethbtc", "step0");
            if (marketDepth == null)
            {
                Console.WriteLine("GetMarketDepth Failed!");
                return;
            }

            Console.WriteLine("GetMarketDepth Succeed:");
            Console.WriteLine("  symbol = {0}", marketDepth.symbol);
            Console.WriteLine("  ts     = {0}", marketDepth.ts);

            Console.WriteLine("  buys:");
            for (int i = 0; i < marketDepth.buys.Count; i++)
            {
                MarketDepth.Item item = marketDepth.buys[i];
                Console.WriteLine("  {0}.\t{1}\t{2}",
                    i + 1,
                    item.price,
                    item.amount);
            }

            Console.WriteLine("  asks:");
            for (int i = 0; i < marketDepth.asks.Count; i++)
            {
                MarketDepth.Item item = marketDepth.asks[i];
                Console.WriteLine("  {0}.\t{1}\t{2}",
                    i + 1,
                    item.price,
                    item.amount);
            }
        }

        static void TestGetMarketKLine()
        {
            MarketKLine marketKLine = Api.GetMarketKLine("ethbtc", "5min", 24);
            if (marketKLine == null)
            {
                Console.WriteLine("GetMarketKLine Failed!");
                return;
            }

            Console.WriteLine("GetMarketKLine Succeed:");
            Console.WriteLine("  symbol = {0}", marketKLine.symbol);
            for (int i = 0; i < marketKLine.kline_data.Count; i++)
            {
                MarketKLine.Item item = marketKLine.kline_data[i];
                Console.WriteLine("  {0}.\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
                    i + 1,
                    item.id,
                    item.amount,
                    item.vol,
                    item.high,
                    item.low,
                    item.close,
                    item.open);
            }
        }

        static void TestGetMarketTrade()
        {
            MarketTrade marketTrade = Api.GetMarketTrade("ethbtc", 20);
            if (marketTrade == null)
            {
                Console.WriteLine("GetMarketTrade Failed!");
                return;
            }

            Console.WriteLine("GetMarketTrade Succeed:");
            Console.WriteLine("  symbol = {0}", marketTrade.symbol);
            for (int i = 0; i < marketTrade.trade_data.Count; i++)
            {
                MarketTrade.Item item = marketTrade.trade_data[i];
                Console.WriteLine("  {0}.\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
                    i + 1,
                    item.id,
                    item.side,
                    item.price,
                    item.vol,
                    item.amount,
                    item.ts,
                    item.ds);
            }
        }

        static void TestGetPublicMarketDetail()
        {
            PublicMarketDetail publicMarketDetail = Api.GetPublicMarketDetail();
            if (publicMarketDetail == null)
            {
                Console.WriteLine("GetPublicMarketDetail Failed!");
                return;
            }

            Console.WriteLine("GetPublicMarketDetail Succeed:");
            int count = 0;
            foreach (var kv in publicMarketDetail.items)
            {
                count++;
                PublicMarketDetail.Item item = kv.Value;
                Console.WriteLine("  {0}.\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}",
                    count,
                    kv.Key,
                    item.id,
                    item.baseVolume,
                    item.quoteVolume,
                    item.percentChange,
                    item.last,
                    item.high24hr,
                    item.low24hr,
                    item.highestBid,
                    item.lowestAsk);
            }
        }

        static void TestGetBalance(Account account)
        {
            InsureAccount(account);

            Balance balance = Api.GetBalance(account, "");
            if (balance == null)
            {
                Console.WriteLine("GetBalance Failed!");
                return;
            }

            Console.WriteLine("GetBalance Succeed:");
            for (int i = 0; i < balance.items.Count; i++)
            {
                Balance.Item item = balance.items[i];
                Console.WriteLine("  {0}.\t{1}\t{2}\t{3}",
                    i + 1,
                    item.coin,
                    item.normal,
                    item.locK);
            }
        }

        static void TestGetOrderDetail(Account account)
        {
            InsureAccount(account);

            Console.WriteLine("To GetOrderDetail, please input symbol, such as btcbitcny, bchbtc, eoseth ...");
            Console.Write("  symbol   = ");
            string symbol = Console.ReadLine();
            Console.Write("  order_id = ");
            long order_id = long.Parse(Console.ReadLine());
            OrderDetail orderDetail = Api.GetOrderDetail(account, symbol, order_id);
            if (orderDetail == null)
            {
                Console.WriteLine("GetOrderDetail Failed!");
                return;
            }

            Console.WriteLine("GetOrderDetail Succeed:");
            Console.WriteLine("  {0}  {1}  {2}  {3}  {4}  {5}  {6}  {7}  {8}  {9}  {10}  {11}  {12}  {13}",
                orderDetail.user_id,
                orderDetail.symbol,
                orderDetail.id,
                orderDetail.type,
                orderDetail.volume,
                orderDetail.price,
                orderDetail.avg_price,
                orderDetail.status,
                orderDetail.deal_volume,
                orderDetail.deal_money,
                orderDetail.fee,
                orderDetail.source,
                orderDetail.ctime,
                orderDetail.mtime);
        }

        static void TestGetOrdersDetail(Account account)
        {
            InsureAccount(account);

            Console.WriteLine("To GetOrders, please input symbol, such as btcbitcny, bchbtc, eoseth ...");
            Console.Write("  symbol     = ");
            string symbol = Console.ReadLine();
            Console.WriteLine("  types      = all (default)");
            Console.WriteLine("  states     = new,part_filled,filled,canceled,expired");
            OrdersDetail ordersDetail = Api.GetOrdersDetail(account, symbol, null, "new,part_filled,filled,canceled,expired");
            if (ordersDetail == null)
            {
                Console.WriteLine("GetOrders Failed!");
                return;
            }

            Console.WriteLine("GetOrders Succeed:  ({0} items)", ordersDetail.items.Count);
            for (int i = 0; i < ordersDetail.items.Count; i++)
            {
                OrderDetail item = ordersDetail.items[i];
                Console.WriteLine("  {0}.\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}",
                    i + 1,
                    item.user_id,
                    item.symbol,
                    item.id,
                    item.type,
                    item.volume,
                    item.price,
                    item.avg_price,
                    item.status,
                    item.deal_volume,
                    item.deal_money,
                    item.fee,
                    item.source,
                    item.ctime,
                    item.mtime);
            }
        }

        static void TestGetMatchResults(Account account)
        {
            InsureAccount(account);

            Console.WriteLine("To GetMatchResults, please input symbol, such as btcbitcny, bchbtc, rcneth ...");
            Console.Write("  symbol     = ");
            string symbol = Console.ReadLine();
            DateTime startData = DateTime.Now.Date - new TimeSpan(7, 0, 0, 0);
            DateTime endData = DateTime.Now.Date;
            Console.WriteLine("  start-date = {0:yyyy-MM-dd}", startData);
            Console.WriteLine("  end-date   = {0:yyyy-MM-dd}", endData);
            MatchResults matchResults = Api.GetMatchResults(account, symbol, startData, endData);
            if (matchResults == null)
            {
                Console.WriteLine("GetMatchResults Failed!");
                return;
            }

            Console.WriteLine("GetMatchResults Succeed:  ({0} items)", matchResults.items.Count);
            for (int i = 0; i < matchResults.items.Count; i++)
            {
                MatchResults.Item item = matchResults.items[i];
                Console.WriteLine("  {0}.\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}",
                    i + 1,
                    item.id,
                    item.price,
                    item.volume,
                    item.fee,
                    item.orderId,
                    item.symbol,
                    item.type,
                    item.status,
                    item.mtime,
                    item.source);
            }
        }

        static void TestGetMakeDetail(Account account)
        {
            InsureAccount(account);

            Console.WriteLine("To GetMakeDetail, please input symbol, such as btcbitcny, bchbtc, eoseth ...");
            Console.Write("  symbol   = ");
            string symbol = Console.ReadLine();
            Console.Write("  order_id = ");
            long order_id = long.Parse(Console.ReadLine());
            MakeDetail makeDetail = Api.GetMakeDetail(account, symbol, order_id);
            if (makeDetail == null)
            {
                Console.WriteLine("GetMakeDetail Failed!");
                return;
            }

            Console.WriteLine("GetMakeDetail Succeed:  ({0} items)", makeDetail.items.Count);
            for (int i = 0; i < makeDetail.items.Count; i++)
            {
                MakeDetail.Item item = makeDetail.items[i];
                Console.WriteLine("  {0}.\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}",
                    i + 1,
                    item.id,
                    item.volume,
                    item.price,
                    item.symbol,
                    item.type,
                    item.source,
                    item.orderId,
                    Math.Max(item.bid_user_id, item.ask_user_id),
                    Math.Max(item.buy_fee, item.sell_fee),
                    item.created);
            }
        }

        static void TestPlaceOrder(Account account)
        {
            InsureAccount(account);

            Console.WriteLine("PlaceOrder:");
            Console.WriteLine("  1.Buy");
            Console.WriteLine("  2.Sell");
            Console.WriteLine("  0.Exit");
            string select = Console.ReadLine();
            if (select != "1" && select != "2")
                return;

            Console.WriteLine("");
            Console.WriteLine(select == "1" ? "To Buy:" : "To Sell:");
            Console.Write("  symbol = ");
            string symbol = Console.ReadLine();
            Console.Write("  price  = ");
            string price = Console.ReadLine();
            Console.Write("  volume = ");
            string volume = Console.ReadLine();
            Console.Write("  type   = ");
            string type = Console.ReadLine();
            Order order = Api.PlaceOrder(account, symbol, double.Parse(price), double.Parse(volume), select == "1" ? "BUY" : "SELL", int.Parse(type));
            if (order == null)
            {
                Console.WriteLine("PlaceOrder Failed!");
                return;
            }

            Console.WriteLine("PlaceOrder Succeed:");
            Console.WriteLine("  order_id = {0}", order.order_id);
        }

        static void TestCancelOrder(Account account)
        {
            InsureAccount(account);

            Console.WriteLine("To CancelOrder, please input symbol, such as btcbitcny, bchbtc, eoseth ...");
            Console.Write("  symbol   = ");
            string symbol = Console.ReadLine();
            Console.Write("  order_id = ");
            long order_id = long.Parse(Console.ReadLine());
            bool res = Api.CancelOrder(account, symbol, order_id);
            Console.WriteLine("CancelOrder {0}", res ? "Succeed." : "Failed!");
        }
    }
}
