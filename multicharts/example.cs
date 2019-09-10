/*
 * This is Multicharts strategy code example of communication with python server
 * Many things here might be not optimal because it's made only for example purpose
 * For sure this strategy will not work:)
 *
 * My russian telegram channel https://t.me/day0market
 * GitHub https://github.com/day0market/
 */

using System;
using Newtonsoft.Json; // Don't forget donload dll and add reference to it in Multicharts
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

namespace PowerLanguage.Strategy
{

    public class Quote
    {
        public double Open;
        public double High;
        public double Low;
        public double Close;
        public DateTime Datetime;

        public Quote(double open, double high, double low, double close, DateTime datetime)
        {
            this.Open = open;
            this.Close = close;
            this.High = high;
            this.Low = low;
            this.Datetime = datetime;

        }
    }

    public class StrategyParams
    {
        public double zig_zag_multiplier;
        public double merge_percent;
    }


    public class RequestData
    {
        public List<Quote> candles;
        public StrategyParams st_params;
    }



    public class LevelEntry
    {
        public double price;
    }




    public class Rest : SignalObject
    {
        public Rest(object _ctx) : base(_ctx)
        {
            param1 = 10;
            param2 = 0.1;
            param3 = 3;
        }
        private IOrderMarket buy_order;
        private IOrderPriced stop_loss;
        private IOrderPriced take_profit;




        [Input]
        public double param1 { get; set; }


        [Input]
        public double param2 { get; set; }

        [Input]
        public int param3 { get; set; }

        private Queue<Quote> quotesBuffer;

        private StrategyParams strategyParams;

        double buy_level;
        double day_high;
        bool orderSent;
        bool stopTakePlaced;
        int tradesCount;

        double stop_price;
        double take_price;

        protected override void Create()
        {
            // create variable objects, function objects, order objects etc.
            buy_order = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, EOrderAction.Buy));
            stop_loss = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "SL", EOrderAction.Sell));
            take_profit = OrderCreator.Limit(new SOrderParameters(Contracts.Default, "TP", EOrderAction.Sell));


            quotesBuffer = new Queue<Quote>();
            strategyParams = new StrategyParams();

        }
        protected override void StartCalc()
        {
            strategyParams.merge_percent = param2;
            strategyParams.zig_zag_multiplier = param1;

        }

        private string SendData(string myJson)
        {
            var url = "http://DESKTOP-EC18PUB:8000/multicharts"; // Replace it for your link

            var request = HttpWebRequest.Create(url);
            var byteData = Encoding.ASCII.GetBytes(myJson);
            request.ContentType = "application/json";
            request.Method = "POST";


            using (var stream = request.GetRequestStream())
            {
                stream.Write(byteData, 0, byteData.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            //Output.WriteLine(responseString); debug purpose
            return responseString;
        }


        private LevelEntry GetAllLevels()
        {
            if (Bars.CurrentBar < 12000)
            {
                return null;
            }

            var candles = new List<Quote>();
            for (int i = 1500; i >= 0; i--)
            {
                var q = new Quote(Bars.Open[i], Bars.High[i], Bars.Low[i], Bars.Close[i], Bars.Time[i]);
                candles.Add(q);
            }



            var reqData = new RequestData();
            reqData.candles = candles;
            reqData.st_params = strategyParams;

            string json = JsonConvert.SerializeObject(reqData);
            var resp = SendData(json);

            if (resp == null)
            {
                return null;
            }

            LevelEntry levels_entries = JsonConvert.DeserializeObject<LevelEntry>(resp);
            return levels_entries;

        }


        private double GetBuyLevel()
        {
            var allLevels = GetAllLevels();

            if (allLevels == null)
            {
                return double.NaN;
            }

            if (allLevels.price < 0)
            {
                return double.NaN;
            }


            return allLevels.price;

        }


        protected override void CalcBar()
        {


            if (Bars.High[0] > day_high)
            {
                day_high = Bars.High[0];
            }

            if (Bars.Time[0].Hour == 11 || Bars.Time[0].Hour == 16 || Bars.Time[0].Hour == 17)
            {

                if (!double.IsNaN(buy_level) && Bars.Close[0] > buy_level - 100 && Bars.Close[0] < buy_level && StrategyInfo.MarketPosition == 0)
                {
                    tradesCount++;
                    // Output.WriteLine(string.Format("Close: {0} Level: {1} Trades: {2} BarN: {3}", Bars.Close[0], buy_level, tradesCount, Bars.CurrentBar));
                    buy_order.Send();
                    stop_price = Bars.Low[0] - 200;
                    take_price = Bars.Close[0] + 5000;
                    return;

                }
            }

            take_profit.Send(take_price);
            stop_loss.Send(stop_price);

            if (Bars.LastBarInSession && StrategyInfo.MarketPosition != 0)
            {
                GenerateExitOnClose();
            }

            if (Bars.LastBarInSession)
            {
                //Output.WriteLine(string.Format("Buy price: {0}. Day High: {1}", buy_level, day_high));
                buy_level = GetBuyLevel();
                orderSent = false;
                day_high = double.NegativeInfinity;
            }
        }
    }
}