using System;
using System.Collections.Generic;

namespace vqm2MNET
{
        public enum PortType
        {
                    IN,
                    OUT
        }
        public class IOPort
        {
                public string Name;
                public string Connection;
                public PortType Ptype;

                public IOPort()
                {

                }
        }
}
