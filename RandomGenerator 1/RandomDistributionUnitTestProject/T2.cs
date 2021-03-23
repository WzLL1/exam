using System.ComponentModel;
using RandomGenerator;

namespace RandomDistributionUnitTestProject
{
    public class T2
    {
        [FromDistribution(typeof(NormalDistribution), -1, 2)]
        public double A { get; set; }

        public double B = 42;

        public double C { get; set; }

        [FromDistribution(typeof(NormalDistribution))]
        public double D { get; set; }
    }
}
