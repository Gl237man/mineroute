using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnetsynt2
{
    class NetworkDataGroupObject
    {
        public Mnet MainNetwork;
        public int BaseSize;
        public List<RouteUtils.Cpoint> Cpoints;
        public int CurrentWireLayer;
        public int CurrentRealLayer;
        public RouteUtils.Wire[] MCWires;
        public char[,] WireMask;
        public int[] WireWeights;
    }
}
