module test(sig1,sig2,sig4,sig3,counter,clk);
input [1:0]sig1;
input [1:0]sig2;
input [1:0]sig4;
input clk;
output reg [3:0]counter;
output reg [1:0]sig3;

always @(sig1,sig2,sig4)
begin
	sig3 = sig1 & sig2 | sig4;
end

always @(posedge clk)
begin 
   counter=counter+1;
end

endmodule