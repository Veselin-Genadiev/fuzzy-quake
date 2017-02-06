using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Fuzzy;
using System.IO;
using FuzzyQuake.Lib.Utils;
using System.Device.Location;

namespace FuzzyQuake.Lib
{
    public class EarthQuakeInferenceSystem
    {
        private InferenceSystem quakeSystem;
        private Inputs inputs;
        private List<Rule> rules = new List<Rule>();

        public DateTime StartDate { get; set; }

        public EarthQuakeInferenceSystem() :
            this(DateTime.Now)
        {
        }

        public EarthQuakeInferenceSystem(DateTime startDate)
        {
            this.StartDate = startDate;
            this.BuildInferenceSystem();
        }

        /// <summary>
        /// Input CSV must have these field:
        /// time - UTC format
        /// latitude
        /// longitude
        /// depth (km)
        /// mag
        /// </summary>
        /// <param name="csvPath"></param>
        public void ProvideInput(string csvPath, float latitudeStart, float longitudeStart, float latitudeEnd, float longitudeEnd)
        {
            var lines = File.ReadAllLines(csvPath);
            var startCoordinate = new GeoCoordinate(latitudeStart, longitudeStart);
            var endCoordinate = new GeoCoordinate(latitudeEnd, longitudeEnd);

            lines = lines.Skip(1).Where(line =>
            {
                var values = line.Split(',');
                long time = DateTime.Parse(values[0]).Ticks;
                float latitude = float.Parse(values[1]);
                float longitude = float.Parse(values[2]);
                var coordinate = new GeoCoordinate(latitude, longitude);

                bool isDateRight = DateTime.Parse(values[0]) <= StartDate;
                bool isNearOrFair = (float)coordinate.MinDistanceToRectangle(startCoordinate, endCoordinate) / 1000 <= 40;
                return isDateRight && isNearOrFair;
            }).ToArray();

            inputs = new Inputs(lines.Length);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var values = line.Split(',');
                long time = DateTime.Parse(values[0]).Ticks;
                float latitude = float.Parse(values[1]);
                float longitude = float.Parse(values[2]);
                var coordinate = new GeoCoordinate(latitude, longitude);

                float depth = float.Parse(values[3]);
                float magnitude = float.Parse(values[4]);

                inputs.Times[i] = time;
                inputs.Distances[i] = (float)coordinate.MinDistanceToRectangle(startCoordinate, endCoordinate) / 1000;
                inputs.Depths[i] = depth;
                inputs.Magnitudes[i] = magnitude;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                var weekMagnitudes = inputs.Magnitudes.Where((d, index) => inputs.Times[index] < StartDate.Ticks &&
                    inputs.Times[index] > StartDate.AddDays(-7).Ticks);

                var weekDepths = inputs.Depths.Where((d, index) => inputs.Times[index] < StartDate.Ticks &&
                    inputs.Times[index] > StartDate.AddDays(-7).Ticks);

                inputs.WeekConcentrations[i] = weekMagnitudes.Count();
                inputs.MaxWeekMagnitudes[i] = weekMagnitudes.Count() == 0 ? 0 : weekMagnitudes.Max();
                inputs.MaxWeekDepths[i] = weekDepths.Count() == 0 ? 0 :
                    weekDepths.ElementAt(weekMagnitudes.ToList().IndexOf(inputs.MaxWeekMagnitudes[i]));

                var twoWeekMagnitudes = inputs.Magnitudes.Where((d, index) => inputs.Times[index] < StartDate.AddDays(-7).Ticks &&
                    inputs.Times[index] > StartDate.AddDays(-14).Ticks);

                var twoWeekDepths = inputs.Depths.Where((d, index) => inputs.Times[index] < StartDate.AddDays(-7).Ticks &&
                    inputs.Times[index] > StartDate.AddDays(-14).Ticks);

                inputs.TwoWeeksConcentrations[i] = twoWeekMagnitudes.Count();
                inputs.MaxTwoWeeksMagnitudes[i] = twoWeekMagnitudes.Count() == 0 ? 0 : twoWeekMagnitudes.Max();
                inputs.MaxTwoWeeksDepths[i] = twoWeekDepths.Count() == 0 ? 0 :
                    twoWeekDepths.ElementAt(twoWeekMagnitudes.ToList().IndexOf(inputs.MaxTwoWeeksMagnitudes[i]));

                var fiveYearMagnitudes = inputs.Magnitudes.Where((d, index) => inputs.Times[index] < StartDate.AddYears(-1).Ticks &&
                    inputs.Times[index] > StartDate.AddYears(-5).Ticks);

                var fiveYearDepths = inputs.Depths.Where((d, index) => inputs.Times[index] < StartDate.AddYears(-1).Ticks &&
                    inputs.Times[index] > StartDate.AddYears(-5).Ticks);

                inputs.FiveYearsConcentrations[i] = fiveYearMagnitudes.Count();
                inputs.MaxFiveYearsMagnitudes[i] = fiveYearMagnitudes.Count() == 0 ? 0 : fiveYearMagnitudes.Max();
                inputs.MaxFiveYearsDepths[i] = fiveYearDepths.Count() == 0 ? 0 :
                    fiveYearDepths.ElementAt(fiveYearMagnitudes.ToList().IndexOf(inputs.MaxFiveYearsMagnitudes[i]));

                var tenYearMagnitudes = inputs.Magnitudes.Where((d, index) => inputs.Times[index] < StartDate.AddYears(-5).Ticks &&
                    inputs.Times[index] > StartDate.AddYears(-10).Ticks);

                var tenYearDepths = inputs.Depths.Where((d, index) => inputs.Times[index] < StartDate.AddYears(-5).Ticks &&
                    inputs.Times[index] > StartDate.AddYears(-10).Ticks);

                inputs.TenYearsConcentrations[i] = tenYearMagnitudes.Count();
                inputs.MaxTenYearsMagnitudes[i] = tenYearMagnitudes.Count() == 0 ? 0 : tenYearMagnitudes.Max();
                inputs.MaxTenYearsDepths[i] = tenYearDepths.Count() == 0 ? 0 :
                    tenYearDepths.ElementAt(tenYearMagnitudes.ToList().IndexOf(inputs.MaxTenYearsMagnitudes[i]));

                var fiftyYearMagnitudes = inputs.Magnitudes.Where((d, index) => inputs.Times[index] < StartDate.AddYears(-10).Ticks &&
                    inputs.Times[index] > StartDate.AddYears(-50).Ticks);

                var fiftyYearDepths = inputs.Depths.Where((d, index) => inputs.Times[index] < StartDate.AddYears(-10).Ticks &&
                    inputs.Times[index] > StartDate.AddYears(-50).Ticks);

                inputs.FiftyYearsConcentrations[i] = fiftyYearMagnitudes.Count();
                inputs.MaxFiftyYearsMagnitudes[i] = fiftyYearMagnitudes.Count() == 0 ? 0 : fiftyYearMagnitudes.Max();
                inputs.MaxFiftyYearsDepths[i] = fiftyYearDepths.Count() == 0 ? 0 :
                    fiftyYearDepths.ElementAt(fiftyYearMagnitudes.ToList().IndexOf(inputs.MaxFiftyYearsMagnitudes[i]));

                var centuryMagnitudes = inputs.Magnitudes.Where((d, index) => inputs.Times[index] < StartDate.AddYears(-50).Ticks &&
                    inputs.Times[index] > StartDate.AddYears(-100).Ticks);

                var centuryDepths = inputs.Depths.Where((d, index) => inputs.Times[index] < StartDate.AddYears(-50).Ticks &&
                    inputs.Times[index] > StartDate.AddYears(-100).Ticks);

                inputs.CenturyConcentrations[i] = centuryMagnitudes.Count();
                inputs.MaxCenturyMagnitudes[i] = centuryMagnitudes.Count() == 0 ? 0 : centuryMagnitudes.Max();
                inputs.MaxCenturyDepths[i] = centuryDepths.Count() == 0 ? 0 :
                    centuryDepths.ElementAt(centuryMagnitudes.ToList().IndexOf(inputs.MaxCenturyMagnitudes[i]));
            }

            for (int i = 0; i < lines.Length; i++)
            {
                quakeSystem.SetInput("Date", inputs.Times[i]);
                quakeSystem.SetInput("Distance", inputs.Distances[i]);
                quakeSystem.SetInput("WeekConcentration", inputs.WeekConcentrations[i]);
                quakeSystem.SetInput("MaxWeekMagnitude", inputs.MaxWeekMagnitudes[i]);
                quakeSystem.SetInput("TwoWeeksConcentration", inputs.TwoWeeksConcentrations[i]);
                quakeSystem.SetInput("MaxTwoWeeksMagnitude", inputs.MaxTwoWeeksMagnitudes[i]);
                quakeSystem.SetInput("FiveYearsConcentration", inputs.FiveYearsConcentrations[i]);
                quakeSystem.SetInput("MaxFiveYearsMagnitude", inputs.MaxFiveYearsMagnitudes[i]);
                quakeSystem.SetInput("TenYearsConcentration", inputs.TenYearsConcentrations[i]);
                quakeSystem.SetInput("MaxTenYearsMagnitude", inputs.MaxTenYearsMagnitudes[i]);
                quakeSystem.SetInput("FiftyYearsConcentration", inputs.FiftyYearsConcentrations[i]);
                quakeSystem.SetInput("MaxFiftyYearsMagnitude", inputs.MaxFiftyYearsMagnitudes[i]);
                quakeSystem.SetInput("CenturyConcentration", inputs.CenturyConcentrations[i]);
                quakeSystem.SetInput("MaxCenturyMagnitude", inputs.MaxCenturyMagnitudes[i]);
                quakeSystem.SetInput("MaxWeekDepth", inputs.MaxWeekDepths[i]);
                quakeSystem.SetInput("MaxTwoWeeksDepth", inputs.MaxTwoWeeksDepths[i]);
                quakeSystem.SetInput("MaxFiveYearsDepth", inputs.MaxFiveYearsDepths[i]);
                quakeSystem.SetInput("MaxTenYearsDepth", inputs.MaxTenYearsDepths[i]);
                quakeSystem.SetInput("MaxFiftyYearsDepth", inputs.MaxFiftyYearsDepths[i]);
                quakeSystem.SetInput("MaxCenturyDepth", inputs.MaxCenturyDepths[i]);
            }
        }

        public string EvaluateSeismicity()
        {
            try
            {
                return GetSeismicityLabel(quakeSystem.Evaluate("Seismicity"));
            }
            catch
            {
                return "Low";
            }
        }

        public string EvaluateWeekSeismicity()
        {
            try
            {
                return GetSeismicityLabel(quakeSystem.Evaluate("WeekSeismicity"));
            }
            catch
            {
                return "Low";
            }
        }

        public string EvaluateMonthSeismicity()
        {
            try
            {
                return GetSeismicityLabel(quakeSystem.Evaluate("MonthSeismicity"));
            }
            catch
            {
                return "Low";
            }
        }

        private string GetSeismicityLabel(float output)
        {
            if (output < 4)
            {
                return "Low";
            }
            else if (output < 6)
            {
                return "Medium";
            }
            else if (output < 9)
            {
                return "Strong";
            }
            else
            {
                return "Great";
            }
        }

        private void BuildInferenceSystem()
        {
            #region Add linguistic variables
            // Input
            var lvDistance = GetDistanceVariable();
            var lvDate = GetDateVariable();

            // Output
            var lvSeismicEnv = GetSeismicityVariable("Seismicity");
            var lvWeekSeismicEnv = GetSeismicityVariable("WeekSeismicity");
            var lvMonthSeismicEnv = GetSeismicityVariable("MonthSeismicity");

            // Input Calculated average earthquake concetration (near only)
            var lvWeekConcentration = GetConcentrationVariable("WeekConcentration", StartDate.AddDays(-7), StartDate);
            var lvTwoWeeksConcentration = GetConcentrationVariable("TwoWeeksConcentration", StartDate.AddDays(-14), StartDate.AddDays(-7));
            var lvFiveYearsConcentration = GetConcentrationVariable("FiveYearsConcentration", StartDate.AddYears(-5), StartDate.AddDays(-14));
            var lvTenYearsConcentration = GetConcentrationVariable("TenYearsConcentration", StartDate.AddYears(-10), StartDate.AddYears(-5));
            var lvFiftyYearsConcentration = GetConcentrationVariable("FiftyYearsConcentration", StartDate.AddYears(-50), StartDate.AddYears(-10));
            var lvCenturyConcentration = GetConcentrationVariable("CenturyConcentration", StartDate.AddYears(-100), StartDate.AddYears(-50));

            // Input Calculated maximum earthquake magnitude
            var lvMaxWeekMagnitude = GetMagnitudeVariable("MaxWeekMagnitude");
            var lvMaxTwoWeeksMagnitude = GetMagnitudeVariable("MaxTwoWeeksMagnitude");
            var lvMaxFiveYearsMagnitude = GetMagnitudeVariable("MaxFiveYearsMagnitude");
            var lvMaxTenYearsMagnitude = GetMagnitudeVariable("MaxTenYearsMagnitude");
            var lvMaxFiftyYearsMagnitude = GetMagnitudeVariable("MaxFiftyYearsMagnitude");
            var lvMaxCenturyMagnitude = GetMagnitudeVariable("MaxCenturyMagnitude");

            // Input Calculated biggest earthquake depth
            var lvMaxWeekDepth = GetDepthVariable("MaxWeekDepth");
            var lvMaxTwoWeeksDepth = GetDepthVariable("MaxTwoWeeksDepth");
            var lvMaxFiveYearsDepth = GetDepthVariable("MaxFiveYearsDepth");
            var lvMaxTenYearsDepth = GetDepthVariable("MaxTenYearsDepth");
            var lvMaxFiftyYearsDepth = GetDepthVariable("MaxFiftyYearsDepth");
            var lvMaxCenturyDepth = GetDepthVariable("MaxCenturyDepth");

            Database fuzzyDB = new Database();
            fuzzyDB.AddVariable(lvDistance);
            fuzzyDB.AddVariable(lvDate);
            fuzzyDB.AddVariable(lvSeismicEnv);
            fuzzyDB.AddVariable(lvWeekSeismicEnv);
            fuzzyDB.AddVariable(lvMonthSeismicEnv);

            fuzzyDB.AddVariable(lvWeekConcentration);
            fuzzyDB.AddVariable(lvTwoWeeksConcentration);
            fuzzyDB.AddVariable(lvFiveYearsConcentration);
            fuzzyDB.AddVariable(lvTenYearsConcentration);
            fuzzyDB.AddVariable(lvFiftyYearsConcentration);
            fuzzyDB.AddVariable(lvCenturyConcentration);

            fuzzyDB.AddVariable(lvMaxWeekMagnitude);
            fuzzyDB.AddVariable(lvMaxTwoWeeksMagnitude);
            fuzzyDB.AddVariable(lvMaxFiveYearsMagnitude);
            fuzzyDB.AddVariable(lvMaxTenYearsMagnitude);
            fuzzyDB.AddVariable(lvMaxFiftyYearsMagnitude);
            fuzzyDB.AddVariable(lvMaxCenturyMagnitude);

            fuzzyDB.AddVariable(lvMaxWeekDepth);
            fuzzyDB.AddVariable(lvMaxTwoWeeksDepth);
            fuzzyDB.AddVariable(lvMaxFiveYearsDepth);
            fuzzyDB.AddVariable(lvMaxTenYearsDepth);
            fuzzyDB.AddVariable(lvMaxFiftyYearsDepth);
            fuzzyDB.AddVariable(lvMaxCenturyDepth);
            #endregion

            quakeSystem = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000), new MinimumNorm(), new MaximumCoNorm());

            #region Construct rules
            // Current seismicity rules
            string nearAndSoonClause = "Distance IS Near AND (Date IS Week OR Date IS TwoWeeks)";
            string tooStrongMagnitudeClause = "(MaxWeekMagnitude IS Great OR MaxWeekMagnitude IS Major)";
            string strongMagnitudeClause = "MaxWeekMagnitude IS Strong";
            string moderateMagnitudeClause = "MaxWeekMagnitude IS Moderate";
            string lightMagnitudeClause = "MaxWeekMagnitude IS Light";
            string belowLightMagnitudeClause = "(MaxWeekMagnitude IS Light OR MaxWeekMagnitude IS Minor OR MaxWeekMagnitude IS Micro)";
            string shallowDepthClause = "MaxWeekDepth IS Shallow";
            string notShallowDepthClause = "MaxWeekDepth IS NOT Shallow";
            string fairlyDeepClause = "(MaxWeekDepth IS Fairly OR MaxWeekDepth is Deep)";
            string veryDeepClause = "MaxWeekDepth IS VeryDeep";

            // near events
            rules.Add(quakeSystem.NewRule("Rule 1", "IF " + nearAndSoonClause +
                " AND " + tooStrongMagnitudeClause + " THEN Seismicity IS Great"));

            rules.Add(quakeSystem.NewRule("Rule 2", "IF " + nearAndSoonClause +
                " AND "+ strongMagnitudeClause + " AND " + shallowDepthClause + " THEN Seismicity is Great"));

            rules.Add(quakeSystem.NewRule("Rule 3", "IF " + nearAndSoonClause +
                " AND " + strongMagnitudeClause + " AND " + fairlyDeepClause + " THEN Seismicity is Strong"));

            rules.Add(quakeSystem.NewRule("Rule 4", "IF " + nearAndSoonClause +
                " AND " + strongMagnitudeClause + " AND " + veryDeepClause + " THEN Seismicity is Strong"));

            rules.Add(quakeSystem.NewRule("Rule 5", "IF " + nearAndSoonClause +
                " AND " + moderateMagnitudeClause + " AND " + shallowDepthClause + " THEN Seismicity is Strong"));

            rules.Add(quakeSystem.NewRule("Rule 6", "IF " + nearAndSoonClause +
                " AND " + moderateMagnitudeClause + " AND "+ notShallowDepthClause + " THEN Seismicity is Medium"));

            rules.Add(quakeSystem.NewRule("Rule 7", "IF " + nearAndSoonClause +
                " AND " + lightMagnitudeClause + " AND " + shallowDepthClause + " THEN Seismicity is Medium"));

            rules.Add(quakeSystem.NewRule("Rule 8", "IF " + nearAndSoonClause +
                " AND " + belowLightMagnitudeClause + " AND " + notShallowDepthClause + " THEN Seismicity is Low"));

            // far events
            string fairAndSoonClause = "Distance IS Fair AND (Date IS Week OR Date is TwoWeeks)";
            rules.Add(quakeSystem.NewRule("Rule 9", "IF " + fairAndSoonClause + " AND " +
                tooStrongMagnitudeClause + " THEN Seismicity IS Great"));

            rules.Add(quakeSystem.NewRule("Rule 10", "IF " + fairAndSoonClause + " AND " + strongMagnitudeClause +
                " AND " + shallowDepthClause + " THEN Seismicity IS Strong"));

            rules.Add(quakeSystem.NewRule("Rule 11", "IF " + fairAndSoonClause + " AND " + strongMagnitudeClause +
                " AND " + fairlyDeepClause + " THEN Seismicity IS Strong"));

            rules.Add(quakeSystem.NewRule("Rule 12", "IF " + fairAndSoonClause + " AND " + strongMagnitudeClause +
                " AND " + veryDeepClause + " THEN Seismicity IS Medium"));

            rules.Add(quakeSystem.NewRule("Rule 13", "IF " + fairAndSoonClause + " AND " + moderateMagnitudeClause +
                " AND " + shallowDepthClause + " THEN Seismicity IS Medium"));

            rules.Add(quakeSystem.NewRule("Rule 14", "IF " + fairAndSoonClause + " AND " + moderateMagnitudeClause +
                " AND " + notShallowDepthClause + " THEN Seismicity IS Low"));

            rules.Add(quakeSystem.NewRule("Rule 15", "IF " + fairAndSoonClause + " AND " +
                "(" + lightMagnitudeClause + " OR " + belowLightMagnitudeClause + ")" +
                " THEN Seismicity IS Low"));

            // Predict next week - subsiding
            string soonClause = "(Date IS Week OR Date IS TwoWeeks)";
            rules.Add(quakeSystem.NewRule("Rule 16", "IF " + soonClause + " AND (WeekConcentration IS Big OR TwoWeeksConcentration IS Big) " +
                "AND (MaxWeekMagnitude IS Great OR MaxTwoWeeksMagnitude IS Great OR " +
                     "MaxWeekMagnitude IS Major OR MaxTwoWeeksMagnitude IS Major)" +
                     " THEN WeekSeismicity IS Strong"));

            rules.Add(quakeSystem.NewRule("Rule 17", "IF " + soonClause + " AND (WeekConcentration IS Big OR TwoWeeksConcentration IS Big) " +
                "AND (MaxWeekMagnitude IS Strong OR MaxTwoWeeksMagnitude IS Strong OR " +
                     "MaxWeekMagnitude IS Moderate OR MaxTwoWeeksMagnitude IS Moderate)" +
                     " THEN WeekSeismicity IS Medium"));

            rules.Add(quakeSystem.NewRule("Rule 18", "IF " + soonClause + " AND (WeekConcentration IS Big OR TwoWeeksConcentration IS Big) " +
                "AND (MaxWeekMagnitude IS Light OR MaxTwoWeeksMagnitude IS Light OR " +
                     "MaxWeekMagnitude IS Minor OR MaxTwoWeeksMagnitude IS Minor OR " +
                     "MaxWeekMagnitude IS Micro OR MaxTwoWeeksMagnitude IS Micro)" +
                     " THEN WeekSeismicity IS Low"));

            // Predict next month
            string upToMonthClause = "(Date IS Week OR Date IS TwoWeeks OR Date IS Month)";
            rules.Add(quakeSystem.NewRule("Rule 19", "IF " + upToMonthClause + " AND FiveYearsConcentration IS Medium AND TenYearsConcentration IS Medium " +
                "AND (MaxFiveYearsMagnitude IS Strong AND MaxTenYearsMagnitude IS Strong OR " +
                     "MaxFiveYearsMagnitude IS Moderate AND MaxTenYearsMagnitude IS Moderate)" +
                     " THEN MonthSeismicity IS Strong"));

            rules.Add(quakeSystem.NewRule("Rule 20", "IF " + upToMonthClause + " AND FiveYearsConcentration IS Medium AND TenYearsConcentration IS Medium " +
                "AND MaxFiveYearsMagnitude IS Light AND MaxTenYearsMagnitude IS Light" +
                     " THEN MonthSeismicity IS Medium"));

            rules.Add(quakeSystem.NewRule("Rule 21", "IF " + upToMonthClause + " AND FiveYearsConcentration IS Medium AND TenYearsConcentration IS Medium " +
                "AND (MaxFiveYearsMagnitude IS Minor OR MaxFiveYearsMagnitude IS Micro) AND " +
                    "(MaxTenYearsMagnitude IS Minor OR MaxTenYearsMagnitude IS Micro)" +
                     " THEN MonthSeismicity IS Low"));

            string upToYearClause = "(Date IS Week OR Date IS TwoWeeks OR Date IS Month OR " +
                "Date IS ThreeMonths OR Date IS SixMonths OR Date IS Year)";

            rules.Add(quakeSystem.NewRule("Rule 22", "IF " + upToYearClause + " AND FiftyYearsConcentration IS Small AND CenturyConcentration IS Small " +
                "AND (MaxFiftyYearsMagnitude IS Great AND MaxCenturyMagnitude IS Great OR " +
                     "MaxFiftyYearsMagnitude IS Major AND MaxCenturyMagnitude IS Major)" +
                     " THEN MonthSeismicity IS Great"));

            rules.Add(quakeSystem.NewRule("Rule 23", "IF " + upToYearClause +
                " AND NOT (MaxFiftyYearsMagnitude IS Great AND MaxCenturyMagnitude IS Great OR " +
                          "MaxFiftyYearsMagnitude IS Major AND MaxCenturyMagnitude IS Major)" +
                     " THEN MonthSeismicity IS Low"));
            #endregion
        }

        private LinguisticVariable GetConcentrationVariable(string period, DateTime dateFrom, DateTime dateTo)
        {
            var offset = dateTo - dateFrom;
            int days = offset.Days;
            float bigPercent = 0.04f;
            float mediumPercent = 0.02f;
            float smallPercent = 0.01f;


            FuzzySet smallConcentration = new FuzzySet("Small", new TrapezoidalFunction(0, 0, smallPercent * days, mediumPercent * days));
            FuzzySet mediumConcentration = new FuzzySet("Medium", new TrapezoidalFunction(smallPercent * days, mediumPercent * days, bigPercent * days));
            FuzzySet bigConcentration = new FuzzySet("Big", new TrapezoidalFunction(mediumPercent * days, bigPercent * days, TrapezoidalFunction.EdgeType.Left));

            LinguisticVariable lvConcentration = new LinguisticVariable(period, 0, float.MaxValue);

            lvConcentration.AddLabel(smallConcentration);
            lvConcentration.AddLabel(mediumConcentration);
            lvConcentration.AddLabel(bigConcentration);

            return lvConcentration;
        }

        private LinguisticVariable GetDistanceVariable()
        {
            FuzzySet distanceNear = new FuzzySet("Near", new TrapezoidalFunction(0, 0, 15, 22));
            FuzzySet distanceFair = new FuzzySet("Fair", new TrapezoidalFunction(20, 30, 40));

            LinguisticVariable lvDistance = new LinguisticVariable("Distance", 0, 80);
            lvDistance.AddLabel(distanceNear);
            lvDistance.AddLabel(distanceFair);

            return lvDistance;
        }

        private LinguisticVariable GetDateVariable()
        {
            var weekBehind = StartDate.AddDays(-7);
            var twoWeeksBehind = StartDate.AddDays(-14);
            var monthBehind = StartDate.AddMonths(-1);
            var threeMonthBehind = StartDate.AddMonths(-3);
            var sixMonthBehind = StartDate.AddMonths(-6);
            var yearBehind = StartDate.AddYears(-1);
            var twoYearBehind = StartDate.AddYears(-2);
            var fiveYearBehind = StartDate.AddYears(-5);
            var tenYearBehind = StartDate.AddYears(-10);
            var twentyYearBehind = StartDate.AddYears(-20);
            var fiftyYearBehind = StartDate.AddYears(-50);
            var centuryBehind = StartDate.AddYears(-100);

            FuzzySet dateWeek = new FuzzySet("Week", new TrapezoidalFunction(0, 0, weekBehind.Ticks, weekBehind.AddDays(1).Ticks));
            FuzzySet dateTwoWeeks = new FuzzySet("TwoWeeks", new TrapezoidalFunction(weekBehind.AddDays(-1).Ticks, weekBehind.Ticks, twoWeeksBehind.Ticks, twoWeeksBehind.AddDays(1).Ticks));
            FuzzySet dateMonth = new FuzzySet("Month", new TrapezoidalFunction(twoWeeksBehind.AddDays(-2).Ticks, twoWeeksBehind.Ticks, monthBehind.Ticks, monthBehind.AddDays(2).Ticks));
            FuzzySet dateThreeMonths = new FuzzySet("ThreeMonths", new TrapezoidalFunction(monthBehind.AddDays(-7).Ticks, monthBehind.Ticks, threeMonthBehind.Ticks, threeMonthBehind.AddDays(7).Ticks));
            FuzzySet dateSixMonths = new FuzzySet("SixMonths", new TrapezoidalFunction(threeMonthBehind.AddDays(-14).Ticks, threeMonthBehind.Ticks, sixMonthBehind.Ticks, sixMonthBehind.AddDays(14).Ticks));
            FuzzySet dateYear = new FuzzySet("Year", new TrapezoidalFunction(sixMonthBehind.AddMonths(-1).Ticks, sixMonthBehind.Ticks, yearBehind.Ticks, yearBehind.AddMonths(1).Ticks));
            FuzzySet dateTwoYears = new FuzzySet("TwoYears", new TrapezoidalFunction(yearBehind.AddMonths(-2).Ticks, yearBehind.Ticks, twoYearBehind.Ticks, twoYearBehind.AddMonths(2).Ticks));
            FuzzySet dateFiveYears = new FuzzySet("FiveYears", new TrapezoidalFunction(twoYearBehind.AddMonths(-3).Ticks, twoYearBehind.Ticks, fiveYearBehind.Ticks, fiveYearBehind.AddMonths(3).Ticks));
            FuzzySet dateTenYears = new FuzzySet("TenYears", new TrapezoidalFunction(fiveYearBehind.AddMonths(-6).Ticks, fiveYearBehind.Ticks, tenYearBehind.Ticks, tenYearBehind.AddMonths(6).Ticks));
            FuzzySet dateTwentyYears = new FuzzySet("TwentyYears", new TrapezoidalFunction(tenYearBehind.AddYears(-1).Ticks, tenYearBehind.Ticks, twentyYearBehind.Ticks, twentyYearBehind.AddYears(1).Ticks));
            FuzzySet dateFiftyYears = new FuzzySet("FiftyYears", new TrapezoidalFunction(twentyYearBehind.AddYears(-2).Ticks, twentyYearBehind.Ticks, fiftyYearBehind.Ticks, fiftyYearBehind.AddYears(2).Ticks));
            FuzzySet dateCentury = new FuzzySet("Century", new TrapezoidalFunction(fiftyYearBehind.AddYears(-5).Ticks, fiftyYearBehind.Ticks, centuryBehind.Ticks, centuryBehind.AddYears(5).Ticks));

            LinguisticVariable lvDate = new LinguisticVariable("Date", 0, DateTime.MaxValue.Ticks);
            lvDate.AddLabel(dateWeek);
            lvDate.AddLabel(dateTwoWeeks);
            lvDate.AddLabel(dateMonth);
            lvDate.AddLabel(dateThreeMonths);
            lvDate.AddLabel(dateSixMonths);
            lvDate.AddLabel(dateYear);
            lvDate.AddLabel(dateTwoYears);
            lvDate.AddLabel(dateFiveYears);
            lvDate.AddLabel(dateTenYears);
            lvDate.AddLabel(dateTwentyYears);
            lvDate.AddLabel(dateFiftyYears);
            lvDate.AddLabel(dateCentury);

            return lvDate;
        }

        private LinguisticVariable GetMagnitudeVariable(string magType)
        {
            FuzzySet microMagnitude = new FuzzySet("Micro", new TrapezoidalFunction(0, 1, 1, 2.2f));
            FuzzySet minorMagnitude = new FuzzySet("Minor", new TrapezoidalFunction(1.8f, 3, 3, 4.2f));
            FuzzySet lightMagnitude = new FuzzySet("Light", new TrapezoidalFunction(3.8f, 4.5f, 4.5f, 5.2f));
            FuzzySet moderateMagnitude = new FuzzySet("Moderate", new TrapezoidalFunction(4.8f, 5.5f, 5.5f, 6.2f));
            FuzzySet strongMagnitude = new FuzzySet("Strong", new TrapezoidalFunction(5.8f, 6.5f, 6.5f, 7.2f));
            FuzzySet majorMagnitude = new FuzzySet("Major", new TrapezoidalFunction(6.8f, 7.5f, 7.5f, 8.2f));
            FuzzySet greatMagnitude = new FuzzySet("Great", new TrapezoidalFunction(7.8f, 8, 12, 12));

            LinguisticVariable lvMagnitude = new LinguisticVariable(magType, 0, 12);

            lvMagnitude.AddLabel(microMagnitude);
            lvMagnitude.AddLabel(minorMagnitude);
            lvMagnitude.AddLabel(lightMagnitude);
            lvMagnitude.AddLabel(moderateMagnitude);
            lvMagnitude.AddLabel(strongMagnitude);
            lvMagnitude.AddLabel(majorMagnitude);
            lvMagnitude.AddLabel(greatMagnitude);

            return lvMagnitude;
        }

        private LinguisticVariable GetDepthVariable(string depthType)
        {
            FuzzySet shallowDepth = new FuzzySet("Shallow", new TrapezoidalFunction(0, 0, 10, 10.5f));
            FuzzySet fairlyDepth = new FuzzySet("Fairly", new TrapezoidalFunction(9.5f, 10, 20, 20.5f));
            FuzzySet deepDepth = new FuzzySet("Deep", new TrapezoidalFunction(19.5f, 20, 40, 40.5f));
            FuzzySet veryDeepDepth = new FuzzySet("VeryDeep", new TrapezoidalFunction(39.5f, 40, float.MaxValue, float.MaxValue));

            LinguisticVariable lvDepth = new LinguisticVariable(depthType, 0, float.MaxValue);

            lvDepth.AddLabel(shallowDepth);
            lvDepth.AddLabel(fairlyDepth);
            lvDepth.AddLabel(deepDepth);
            lvDepth.AddLabel(veryDeepDepth);

            return lvDepth;
        }

        private LinguisticVariable GetSeismicityVariable(string seismicityType)
        {
            FuzzySet lowEnv = new FuzzySet("Low", new TrapezoidalFunction(1, 1, 4, 4.2f));
            FuzzySet mediumEnv = new FuzzySet("Medium", new TrapezoidalFunction(3.8f, 4, 8, 8.2f));
            FuzzySet strongEnv = new FuzzySet("Strong", new TrapezoidalFunction(6.8f, 7, 10, 10.2f));
            FuzzySet greatEnv = new FuzzySet("Great", new TrapezoidalFunction(7.8f, 8, 12, 12));

            LinguisticVariable lvSeismicEnv = new LinguisticVariable(seismicityType, 1, 12);

            lvSeismicEnv.AddLabel(lowEnv);
            lvSeismicEnv.AddLabel(mediumEnv);
            lvSeismicEnv.AddLabel(strongEnv);
            lvSeismicEnv.AddLabel(greatEnv);

            return lvSeismicEnv;
        }
    }
}
