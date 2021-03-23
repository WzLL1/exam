using RandomGenerator;

namespace RandomDistributionUnitTestProject
{
    public class T1
    {
        [FromDistribution(typeof(NormalDistribution), 1, 2)]
        public double A { get; set; }
    }
}
