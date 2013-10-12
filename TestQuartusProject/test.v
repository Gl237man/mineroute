module test(sig1,sig2,sig4,sig3);
input [1:0]sig1;
input [1:0]sig2;
input [1:0]sig4;
output reg [1:0]sig3;

always @(sig1,sig2,sig4)
begin
	sig3 = sig1 & sig2 | sig4;
end

endmodule