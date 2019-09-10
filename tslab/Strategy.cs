/* 
 * Example how TsLab can communicate with python app
 * My russian telegram channel https://t.me/day0market
 * GitHub https://github.com/day0market/
 */

using System;
using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Script.Optimization;


namespace TSLab.Example
{

    public class Strategy : IExternalScript
    {
        public OptimProperty DataLen = new OptimProperty(30, 10, 100, 5);
        public OptimProperty LongEntryThreshold = new OptimProperty(0.2, -1, 1, 0.1);
        public OptimProperty ShortEntryThreshold = new OptimProperty(-0.2, -1, 1, 0.1);

        private PyWorker pyWorker = new PyWorker("http://DESKTOP-EC18PUB:8000/tslab"); // insert here your link from python server      


        private double GetFromPY(double[] prices)
        {
            var val = pyWorker.GetPyResult(prices);
            return val;
        }



        public virtual void Execute(IContext ctx, ISecurity source)
        {
            double[] pyData = new double[DataLen]; // Array of data we want to send to Python App
            var indicatorValues = new List<double>();


            for (int i = 0; i < DataLen; i++)
            {
                // Fill first `DataLen` elements for indicator and full python array
                pyData[i] = source.ClosePrices[i];
                indicatorValues.Add(0);
            }

            //--------------------------------------------------------------------------------
            #region Trading cycle


            int barsCount = source.Bars.Count;
            int lastEntryBar = 0;

            for (int bar = DataLen; bar < barsCount; bar++)
            {
                //Create new array. Copy DataLen -1 elements from old array and insert new datapoint. 
                // We always have DataLen last points of data
                double[] newData = new double[DataLen];
                Array.Copy(pyData, 1, newData, 0, DataLen - 1);
                newData[DataLen - 1] = source.ClosePrices[bar];
                pyData = newData;

                var val = GetFromPY(pyData); // Send to python and get calculated value

                // Insert calculated value if it's not NaN
                if (double.IsNaN(val))
                {
                    indicatorValues.Add(0);
                }
                else
                {
                    indicatorValues.Add(val);
                }

                // Some dummy trade logic
                IPosition LongPos = source.Positions.GetLastActiveForSignal("LN", bar);
                if (LongPos == null)
                {

                    if (val < LongEntryThreshold)
                    {
                        source.Positions.BuyAtPrice(bar + 1, 1, source.ClosePrices[bar], "LN");
                        lastEntryBar = bar + 1;
                    }

                }
                else
                {

                    if (val > ShortEntryThreshold)
                        LongPos.CloseAtPrice(bar + 1, source.ClosePrices[bar], "LX");
                }

            }


            #endregion

            //--------------------------------------------------------------------------------
            #region Draw charts   

            IGraphPane mainPane = ctx.CreateGraphPane("Main", "");

            //Candlestick
            IGraphList CandleChart = mainPane.AddList(
                "Candle Chart",
                string.Format("Symbol:  [{0}]", source.Symbol),
                source, CandleStyles.BAR_CANDLE, CandleFillStyle.Decreasing,
                true, -14685179, PaneSides.RIGHT
                );

            source.ConnectSecurityList(CandleChart);
            CandleChart.AlternativeColor = -262137;
            CandleChart.Autoscaling = true;
            mainPane.UpdatePrecision(PaneSides.RIGHT, source.Decimals);

            //Python indicator
            IGraphList ind = mainPane.AddList(
                "Indicator", indicatorValues, ListStyles.LINE, 0xa000a0,
                LineStyles.SOLID, PaneSides.RIGHT);

            mainPane.Visible = true;
            mainPane.HideLegend = false;
            mainPane.SizePct = 100;
            ind.AlternativeColor = -6153042;
            ind.Autoscaling = true;
            mainPane.UpdatePrecision(PaneSides.RIGHT, source.Decimals);
            #endregion

        }

        private static IList<double> GetClosePrices(ISecurity source)
        {
            return source.ClosePrices;
        }
    }
}