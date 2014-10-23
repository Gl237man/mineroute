
using System;

namespace LLC {



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int _bits = 3;
	public const int _hex = 4;
	public const int maxT = 21;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

const int // types
	  undef = 0, integer = 1, boolean = 2;

	const int // object kinds
	  var = 0, proc = 1;


	public SymbolTable   tab;
	public CodeGenerator gen;



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void Ident(out string name) {
		Expect(1);
		name = t.val; 
	}

	void Const(out string ID) {
		int wide;
		int tval = 0; 
		Expect(2);
		wide = Convert.ToInt32(t.val);
		if (la.kind == 5) {
			Get();
			Expect(2);
			tval = Convert.ToInt32(t.val);
		} else if (la.kind == 3) {
			Get();
			tval = tab.bitConv(t.val);
		} else if (la.kind == 4) {
			Get();
			tval = tab.hexConv(t.val);
		} else SynErr(22);
		ID = tab.newConst(wide,tval);
	}

	void Expresion(out string ID) {
		ID = "";
		string cID = "";
		string Tleft = "";
		string Tright = "";
		string boptype = "";
		if (la.kind == 2) {
			Const(out cID);
			ID = cID;
		} else if (la.kind == 1) {
			Ident(out cID);
		} else SynErr(23);
		ID = cID;
		if (StartOf(1)) {
			BOp(out boptype);
			Expresion(out Tright);
		}
		if (boptype !="")
		{
		ID = tab.NewBOP(boptype);
		tab.NewWire(cID,ID,"O0","I0");
		tab.NewWire(Tright,ID,"O0","I1");} 
	}

	void BOp(out string optype) {
		optype = "";
		if (la.kind == 6) {
			Get();
			optype = "ADD";
		} else if (la.kind == 7) {
			Get();
			optype = "SUB";
		} else if (la.kind == 8) {
			Get();
			optype = "MUL";
		} else if (la.kind == 9) {
			Get();
			optype = "DIV";
		} else SynErr(24);
	}

	void Assign() {
		string name;
		string ID;
		Ident(out name);
		string wto = name; 
		Expect(10);
		Expresion(out ID);
		string wfrom = ID;
		Expect(11);
		tab.NewWire(wfrom,wto,"O0","I0"); 
	}

	void PortDecl() {
		int wide = 1;
		string name;
		string type = "NAN";
		Expect(12);
		if (la.kind == 13) {
			Get();
			type = "IN";
		} else if (la.kind == 14) {
			Get();
			type = "OUT";
		} else SynErr(25);
		Ident(out name);
		while (la.kind == 15) {
			Get();
			Expect(2);
			wide = Convert.ToInt32(t.val); 
			Expect(16);
		}
		Expect(11);
		tab.NewPort(name,type,wide); 
	}

	void WireDecl() {
		int wide = 1;
		string name;
		Expect(17);
		Ident(out name);
		while (la.kind == 15) {
			Get();
			Expect(2);
			wide = Convert.ToInt32(t.val); 
			Expect(16);
		}
		Expect(11);
		tab.NewWire(name,wide); 
	}

	void llc() {
		Expect(18);
		Expect(19);
		tab.OpenScope(); 
		while (la.kind == 1 || la.kind == 12 || la.kind == 17) {
			if (la.kind == 17) {
				WireDecl();
			} else if (la.kind == 12) {
				PortDecl();
			} else {
				Assign();
			}
		}
		Expect(20);
		tab.CloseScope(); 
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		llc();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "number expected"; break;
			case 3: s = "bits expected"; break;
			case 4: s = "hex expected"; break;
			case 5: s = "\"#i\" expected"; break;
			case 6: s = "\"+\" expected"; break;
			case 7: s = "\"-\" expected"; break;
			case 8: s = "\"*\" expected"; break;
			case 9: s = "\"/\" expected"; break;
			case 10: s = "\"=\" expected"; break;
			case 11: s = "\";\" expected"; break;
			case 12: s = "\"PORT\" expected"; break;
			case 13: s = "\"IN\" expected"; break;
			case 14: s = "\"OUT\" expected"; break;
			case 15: s = "\"[\" expected"; break;
			case 16: s = "\"]\" expected"; break;
			case 17: s = "\"WIRE\" expected"; break;
			case 18: s = "\"main\" expected"; break;
			case 19: s = "\"{\" expected"; break;
			case 20: s = "\"}\" expected"; break;
			case 21: s = "??? expected"; break;
			case 22: s = "invalid Const"; break;
			case 23: s = "invalid Expresion"; break;
			case 24: s = "invalid BOp"; break;
			case 25: s = "invalid PortDecl"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}