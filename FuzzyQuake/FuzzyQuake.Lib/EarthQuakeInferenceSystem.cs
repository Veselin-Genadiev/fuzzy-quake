using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Fuzzy;

namespace FuzzyQuake.Lib
{
    public class EarthQuakeInferenceSystem
    {
        InferenceSystem quakeSystem;

        public DateTime StartDate { get; set; }

        public EarthQuakeInferenceSystem()
        {
            this.BuildInferenceSystem();
            this.StartDate = DateTime.Now;
        }

        public void BuildInferenceSystem()
        {
            var lvDistance = GetDistanceVariable();
            var lvDate = GetDateVariable();
            var lvMagnitude = GetMagnitudeVariable();
            var lvDepth = GetDepthVariable();
            var lvSeismicEnv = GetSeismicEnvironmentVariable();

            Database fuzzyDB = new Database();
            fuzzyDB.AddVariable(lvDistance);
            fuzzyDB.AddVariable(lvDate);
            fuzzyDB.AddVariable(lvMagnitude);
            fuzzyDB.AddVariable(lvDepth);
            fuzzyDB.AddVariable(lvSeismicEnv);

            quakeSystem = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000));
        }

        private LinguisticVariable GetDistanceVariable()
        {
            FuzzySet distanceVeryNear = new FuzzySet("VeryNear", new TrapezoidalFunction(0, 0, 10));
            FuzzySet distanceNear = new FuzzySet("Near", new TrapezoidalFunction(8, 14, 20));
            FuzzySet distanceFair = new FuzzySet("Fair", new TrapezoidalFunction(12, 18, 24));
            FuzzySet distanceFar = new FuzzySet("Far", new TrapezoidalFunction(18, 24, 30));
            FuzzySet distanceVeryFar = new FuzzySet("VeryFar", new TrapezoidalFunction(30, 30, float.MaxValue));

            LinguisticVariable lvDistance = new LinguisticVariable("Distance", 0, float.MaxValue);
            lvDistance.AddLabel(distanceVeryNear);
            lvDistance.AddLabel(distanceNear);
            lvDistance.AddLabel(distanceFair);
            lvDistance.AddLabel(distanceFar);
            lvDistance.AddLabel(distanceVeryFar);

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

        private LinguisticVariable GetMagnitudeVariable()
        {
            FuzzySet microMagnitude = new FuzzySet("Micro", new TrapezoidalFunction(0, 0, 2, 2.2f));
            FuzzySet minorMagnitude = new FuzzySet("Minor", new TrapezoidalFunction(1.8f, 2, 4, 4.2f));
            FuzzySet lightMagnitude = new FuzzySet("Light", new TrapezoidalFunction(3.8f, 4, 5, 5.2f));
            FuzzySet moderateMagnitude = new FuzzySet("Moderate", new TrapezoidalFunction(4.8f, 5, 6, 6.2f));
            FuzzySet strongMagnitude = new FuzzySet("Strong", new TrapezoidalFunction(5.8f, 6, 7, 7.2f));
            FuzzySet majorMagnitude = new FuzzySet("Major", new TrapezoidalFunction(6.8f, 7, 8, 8.2f));
            FuzzySet greatMagnitude = new FuzzySet("Great", new TrapezoidalFunction(7.8f, 8, 12, 12));

            LinguisticVariable lvMagnitude = new LinguisticVariable("Magnitude", 0, 12);

            lvMagnitude.AddLabel(microMagnitude);
            lvMagnitude.AddLabel(minorMagnitude);
            lvMagnitude.AddLabel(lightMagnitude);
            lvMagnitude.AddLabel(moderateMagnitude);
            lvMagnitude.AddLabel(strongMagnitude);
            lvMagnitude.AddLabel(majorMagnitude);
            lvMagnitude.AddLabel(greatMagnitude);

            return lvMagnitude;
        }

        private LinguisticVariable GetDepthVariable()
        {
            FuzzySet shallowDepth = new FuzzySet("Shallow", new TrapezoidalFunction(0, 0, 10, 10.5f));
            FuzzySet fairlyDepth = new FuzzySet("Fairly", new TrapezoidalFunction(9.5f, 10, 20, 20.5f));
            FuzzySet deepDepth = new FuzzySet("Deep", new TrapezoidalFunction(19.5f, 20, 40, 40.5f));
            FuzzySet veryDeepDepth = new FuzzySet("VeryDeep", new TrapezoidalFunction(39.5f, 40, float.MaxValue, float.MaxValue));

            LinguisticVariable lvDepth = new LinguisticVariable("Depth", 0, float.MaxValue);

            lvDepth.AddLabel(shallowDepth);
            lvDepth.AddLabel(fairlyDepth);
            lvDepth.AddLabel(deepDepth);
            lvDepth.AddLabel(veryDeepDepth);

            return lvDepth;
        }

        private LinguisticVariable GetSeismicEnvironmentVariable()
        {
            FuzzySet insignificantEnv = new FuzzySet("Insignificant", new TrapezoidalFunction(0, 0, 4, 4.2f));
            FuzzySet mediumEnv = new FuzzySet("Medium", new TrapezoidalFunction(3.8f, 4, 8, 8.2f));
            FuzzySet strongEnv = new FuzzySet("Strong", new TrapezoidalFunction(6.8f, 7, 10, 10.2f));
            FuzzySet greatEnv = new FuzzySet("Great", new TrapezoidalFunction(7.8f, 8, 12, 12));

            LinguisticVariable lvSeismicEnv = new LinguisticVariable("SeismicEnvironment", 1, 12);

            lvSeismicEnv.AddLabel(insignificantEnv);
            lvSeismicEnv.AddLabel(mediumEnv);
            lvSeismicEnv.AddLabel(strongEnv);
            lvSeismicEnv.AddLabel(greatEnv);

            return lvSeismicEnv;
        }
    }
}
