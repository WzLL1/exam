using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomGenerator;

namespace RandomDistributionUnitTestProject
{
    public class T3
    {
        [FromDistribution(typeof(NormalDistribution), 1, 2, 3)]
        public double WrongDistributionArguments { get; set; }
    }
}
