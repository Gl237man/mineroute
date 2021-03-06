
using System;

namespace LLC {



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int _bits = 3;
	public const int _hex = 4;
	public const int maxT = 38;

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
		} else SynErr(39);
		ID = tab.newConst(wide,tval);
	}

	void Expresion(out string ID) {
		ID = "";
		string LID = "";
		LogicOp(out LID);
		ID = LID; 
	}

	void LogicOp(out string ID) {
		ID = "";
		string RID = "";
		string LID = "";
		string OP ="";
		CompareOp(out LID);
		ID = LID;
		if (la.kind == 16 || la.kind == 17 || la.kind == 18) {
			LogicOpType(out OP);
			LogicOp(out RID);
		}
		if (OP !="")
		{
		ID = tab.NewBOP(OP);
		tab.NewWire(LID,ID,"O","A");
		tab.NewWire(RID,ID,"O","B");} 
	}

	void MultOpType(out string optype) {
		optype = "";
		if (la.kind == 6) {
			Get();
			optype = "MUL_";
		} else if (la.kind == 7) {
			Get();
			optype = "DIV_";
		} else SynErr(40);
	}

	void SumOpType(out string optype) {
		optype = "";
		if (la.kind == 8) {
			Get();
			optype = "ADD_";
		} else if (la.kind == 9) {
			Get();
			optype = "SUB_";
		} else SynErr(41);
	}

	void CompareOpType(out string optype) {
		optype = "";
		switch (la.kind) {
		case 10: {
			Get();
			optype = "EQ_";
			break;
		}
		case 11: {
			Get();
			optype = "NOTEQ_";
			break;
		}
		case 12: {
			Get();
			optype = "MORE_";
			break;
		}
		case 13: {
			Get();
			optype = "LESS_";
			break;
		}
		case 14: {
			Get();
			optype = "LESSEQ_";
			break;
		}
		case 15: {
			Get();
			optype = "MOREEQ_";
			break;
		}
		default: SynErr(42); break;
		}
	}

	void LogicOpType(out string optype) {
		optype = "";
		if (la.kind == 16) {
			Get();
			optype = "AND_";
		} else if (la.kind == 17) {
			Get();
			optype = "OR_";
		} else if (la.kind == 18) {
			Get();
			optype = "XOR_";
		} else SynErr(43);
	}

	void UnOpType(out string optype) {
		optype = "";
		if (la.kind == 19) {
			Get();
			optype = "NOT_";
		} else if (la.kind == 20) {
			Get();
			optype = "LL_";
		} else if (la.kind == 21) {
			Get();
			optype = "RR_";
		} else if (la.kind == 22) {
			Get();
			optype = "RRC_";
		} else if (la.kind == 23) {
			Get();
			optype = "LLC_";
		} else SynErr(44);
	}

	void CompareOp(out string ID) {
		ID = "";
		string RID = "";
		string LID = "";
		string OP ="";
		SumOp(out LID);
		ID = LID;
		if (StartOf(1)) {
			CompareOpType(out OP);
			CompareOp(out RID);
		}
		if (OP !="")
		{
		ID = tab.NewBOP(OP);
		tab.NewWire(LID,ID,"O","A");
		tab.NewWire(RID,ID,"O","B");} 
	}

	void SumOp(out string ID) {
		ID = "";
		string RID = "";
		string LID = "";
		string OP ="";
		MultOp(out LID);
		ID = LID;
		if (la.kind == 8 || la.kind == 9) {
			SumOpType(out OP);
			SumOp(out RID);
		}
		if (OP !="")
		{
		ID = tab.NewBOP(OP);
		tab.NewWire(LID,ID,"O","A");
		tab.NewWire(RID,ID,"O","B");} 
	}

	void MultOp(out string ID) {
		ID = "";
		string RID = "";
		string LID = "";
		string OP ="";
		Term(out LID);
		ID = LID;
		if (la.kind == 6 || la.kind == 7) {
			MultOpType(out OP);
			MultOp(out RID);
		}
		if (OP !="")
		{
		ID = tab.NewBOP(OP);
		tab.NewWire(LID,ID,"O","A");
		tab.NewWire(RID,ID,"O","B");} 
	}

	void Term(out string ID) {
		ID = "";
		string cID = "";
		if (la.kind == 2) {
			Const(out cID);
		} else if (la.kind == 1) {
			Ident(out cID);
		} else if (la.kind == 24) {
			Get();
			Expresion(out cID);
			Expect(25);
		} else if (StartOf(2)) {
			UNOp(out cID);
		} else SynErr(45);
		ID = cID;
	}

	void UNOp(out string ID) {
		ID = "";
		string LID = "";
		string OP ="";
		UnOpType(out OP);
		Term(out LID);
		ID = LID;
		if (OP !="")
		{   
		ID = tab.NewBOP(OP);
		tab.NewWire(LID,ID,"O","I");} 
	}

	void Assign() {
		string name;
		string ID;
		Ident(out name);
		string wto = name; 
		Expect(26);
		Expresion(out ID);
		string wfrom = ID;
		Expect(27);
		tab.NewWire(wfrom,wto,"O","I"); 
	}

	void PortDecl() {
		int wide = 1;
		string name;
		string type = "NAN";
		Expect(28);
		if (la.kind == 29) {
			Get();
			type = "IN";
		} else if (la.kind == 30) {
			Get();
			type = "OUT";
		} else SynErr(46);
		Ident(out name);
		while (la.kind == 31) {
			Get();
			Expect(2);
			wide = Convert.ToInt32(t.val); 
			Expect(32);
		}
		Expect(27);
		tab.NewPort(name,type,wide); 
	}

	void TrigDecl() {
		int wide = 1;
		string name;
		Expect(33);
		Ident(out name);
		while (la.kind == 31) {
			Get();
			Expect(2);
			wide = Convert.ToInt32(t.val); 
			Expect(32);
		}
		Expect(27);
		tab.NewTrig(name,wide); 
	}

	void WireDecl() {
		int wide = 1;
		string name;
		Expect(34);
		Ident(out name);
		while (la.kind == 31) {
			Get();
			Expect(2);
			wide = Convert.ToInt32(t.val); 
			Expect(32);
		}
		Expect(27);
		tab.NewWire(name,wide); 
	}

	void llc() {
		Expect(35);
		Expect(36);
		tab.OpenScope(); 
		while (StartOf(3)) {
			if (la.kind == 34) {
				WireDecl();
			} else if (la.kind == 28) {
				PortDecl();
			} else if (la.kind == 1) {
				Assign();
			} else {
				TrigDecl();
			}
		}
		Expect(37);
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
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x}

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
			case 6: s = "\"*\" expected"; break;
			case 7: s = "\"/\" expected"; break;
			case 8: s = "\"+\" expected"; break;
			case 9: s = "\"-\" expected"; break;
			case 10: s = "\"==\" expected"; break;
			case 11: s = "\"!=\" expected"; break;
			case 12: s = "\">\" expected"; break;
			case 13: s = "\"<\" expected"; break;
			case 14: s = "\"<=\" expected"; break;
			case 15: s = "\">=\" expected"; break;
			case 16: s = "\"&\" expected"; break;
			case 17: s = "\"|\" expected"; break;
			case 18: s = "\"^\" expected"; break;
			case 19: s = "\"!\" expected"; break;
			case 20: s = "\"LL\" expected"; break;
			case 21: s = "\"RR\" expected"; break;
			case 22: s = "\"RRC\" expected"; break;
			case 23: s = "\"LLC\" expected"; break;
			case 24: s = "\"(\" expected"; break;
			case 25: s = "\")\" expected"; break;
			case 26: s = "\"=\" expected"; break;
			case 27: s = "\";\" expected"; break;
			case 28: s = "\"PORT\" expected"; break;
			case 29: s = "\"IN\" expected"; break;
			case 30: s = "\"OUT\" expected"; break;
			case 31: s = "\"[\" expected"; break;
			case 32: s = "\"]\" expected"; break;
			case 33: s = "\"TRIG\" expected"; break;
			case 34: s = "\"WIRE\" expected"; break;
			case 35: s = "\"main\" expected"; break;
			case 36: s = "\"{\" expected"; break;
			case 37: s = "\"}\" expected"; break;
			case 38: s = "??? expected"; break;
			case 39: s = "invalid Const"; break;
			case 40: s = "invalid MultOpType"; break;
			case 41: s = "invalid SumOpType"; break;
			case 42: s = "invalid CompareOpType"; break;
			case 43: s = "invalid LogicOpType"; break;
			case 44: s = "invalid UnOpType"; break;
			case 45: s = "invalid Term"; break;
			case 46: s = "invalid PortDecl"; break;

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