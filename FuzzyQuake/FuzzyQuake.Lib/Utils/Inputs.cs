using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyQuake.Lib.Utils
{
    public class InputOutputs
    {
        public InputOutputs(int n)
        {
            // Input
            this.Times = new long[n];
            this.Distances = new float[n];
            this.Depths = new float[n];
            this.Magnitudes = new float[n];
            this.WeekConcentrations = new float[n];
            this.MaxWeekMagnitudes = new float[n];
            this.TwoWeeksConcentrations = new float[n];
            this.MaxTwoWeeksMagnitudes = new float[n];
            this.FiveYearsConcentrations = new float[n];
            this.MaxFiveYearsMagnitudes = new float[n];
            this.TenYearsConcentrations = new float[n];
            this.MaxTenYearsMagnitudes = new float[n];
            this.FiftyYearsConcentrations = new float[n];
            this.MaxFiftyYearsMagnitudes = new float[n];
            this.CenturyConcentrations = new float[n];
            this.MaxCenturyMagnitudes = new float[n];
            this.MaxWeekDepths = new float[n];
            this.MaxTwoWeeksDepths = new float[n];
            this.MaxFiveYearsDepths = new float[n];
            this.MaxTenYearsDepths = new float[n];
            this.MaxFiftyYearsDepths = new float[n];
            this.MaxCenturyDepths = new float[n];

            this.Seismicities = new float[n];
            this.WeekSeismicities = new float[n];
            this.MonthSeismicities = new float[n];
        }

        // Input
        public long[] Times { get; set; }

        public float[] Distances { get; set; }

        public float[] Depths { get; set; }

        public float[] Magnitudes { get; set; }

        public float[] WeekConcentrations { get; set; }

        public float[] MaxWeekMagnitudes { get; set; }

        public float[] TwoWeeksConcentrations { get; set; }

        public float[] MaxTwoWeeksMagnitudes { get; set; }

        public float[] FiveYearsConcentrations { get; set; }

        public float[] MaxFiveYearsMagnitudes { get; set; }

        public float[] TenYearsConcentrations { get; set; }

        public float[] MaxTenYearsMagnitudes { get; set; }

        public float[] FiftyYearsConcentrations { get; set; }

        public float[] MaxFiftyYearsMagnitudes { get; set; }

        public float[] CenturyConcentrations { get; set; }

        public float[] MaxCenturyMagnitudes { get; set; }

        public float[] MaxWeekDepths { get; set; }

        public float[] MaxTwoWeeksDepths { get; set; }

        public float[] MaxFiveYearsDepths { get; set; }

        public float[] MaxTenYearsDepths { get; set; }

        public float[] MaxFiftyYearsDepths { get; set; }

        public float[] MaxCenturyDepths { get; set; }

        public float[] Seismicities { get; set; }

        public float[] WeekSeismicities { get; set; }

        public float[] MonthSeismicities { get; set; }
    }
}
