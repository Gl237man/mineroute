using System;

namespace vqm2MNET
{
	public enum CellType
	{
		cycloneii_lcell_comb,
		cycloneii_lcell_ff
	}

	public class Cell
	{
		public string Name;
		public CellType CelType;
		public string dataa;
		public string datab;
		public string datac;
		public string datad;
		public string cin;
		public string combout;
		public string cout;
		public string lut_mask;
		public string sum_lutc_input;

		public string clk;
		public string datain;
		public string regout;
        public string sclr;

		public Cell (CellType CType)
		{
			CelType = CType;
		}
	}
}

