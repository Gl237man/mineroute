using System;
using System.Collections.Generic;

namespace vqm2MNET
{
	public class Wire
	{
		public string Name;
		public List<Connection> Connections;

		public Wire ()
		{
			Connections = new List<Connection>();
		}
	}
}

