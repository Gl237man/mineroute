mkdir LUTtest
copy vqm2MNET\bin\Debug\*.exe LUTtest\
copy vqm2MNET\bin\Debug\*.dll LUTtest\
copy MnetLutDecomposite\bin\Debug\*.exe LUTtest\
copy MnetLutDecomposite\bin\Debug\*.dll LUTtest\
copy Mnetsynt3\bin\Debug\*.exe LUTtest\
copy Mnetsynt3\bin\Debug\*.dll LUTtest\
copy GenerateTestForAllLut\bin\Debug\*.exe LUTtest\
copy GenerateTestForAllLut\bin\Debug\*.dll LUTtest\
copy BinhlEmul\bin\Debug\*.exe LUTtest\
copy BinhlEmul\bin\Debug\*.dll LUTtest\
copy MnetLutOptimise\bin\Debug\*.exe LUTtest\
copy MnetLutOptimise\bin\Debug\*.dll LUTtest\
copy MNETLib.BinLib LUTtest\
del LUTtest\*.vshost.exe
copy Templates\V2\*.binhl LUTtest\
cd LUTtest
mkdir 0
mkdir 1
mkdir 2
mkdir 3
mkdir 4
mkdir 5
mkdir 6
mkdir 7
mkdir 8
mkdir 9
mkdir A
mkdir B
mkdir C
mkdir D
mkdir E
mkdir F
copy *.* 0\
copy *.* 1\
copy *.* 2\
copy *.* 3\
copy *.* 4\
copy *.* 5\
copy *.* 6\
copy *.* 7\
copy *.* 8\
copy *.* 9\
copy *.* A\
copy *.* B\
copy *.* C\
copy *.* D\
copy *.* E\
copy *.* F\
GenerateTestForAllLut.exe
