using System;
using System.Collections.Generic;

namespace vqm2MNET
{
	public class Module
	{

		public string Name;
		public List<Cell> Cells;
		public List<Wire> Wires;
		public List<IOPort> Ports;
		public Module ()
		{
			Cells = new List<Cell>();
			Wires = new List<Wire>();
			Ports = new List<IOPort>();
		}
	}
}

