using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmGenetyczny
{
    public class GenoTypeComparer : IComparer<Genotype>
    {
        public GenoTypeComparer()
        {
        }

        public int Compare(Genotype x, Genotype y)
        {
            if ( x.FunctionValue > y.FunctionValue)
                return 1;
            else if (x.FunctionValue < y.FunctionValue)
                return -1;
            else
                return 0;
        }
    }
}
