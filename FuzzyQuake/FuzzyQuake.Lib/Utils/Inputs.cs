using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyQuake.Lib.Utils
{
    public class Inputs
    {
        public Inputs(int n)
        {
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
        }

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
    }
}
