mkdir LLCtest
copy LLC\bin\Debug\*.exe LLCtest\
copy LLC\bin\Debug\*.dll LLCtest\
copy MnetLutDecomposite\bin\Debug\*.exe LLCtest\
copy MnetLutDecomposite\bin\Debug\*.dll LLCtest\
copy Mnetsynt3\bin\Debug\*.exe LLCtest\
copy Mnetsynt3\bin\Debug\*.dll LLCtest\
copy Binhl2JsWE\bin\Debug\*.exe LLCtest\
copy Binhl2JsWE\bin\Debug\*.dll LLCtest\
copy MnetLutOptimise\bin\Debug\*.exe LLCtest\
copy MnetLutOptimise\bin\Debug\*.dll LLCtest\
copy RunLLCTest.cmd LLCtest\
copy MNETLib.BinLib LLCtest\
del LLCtest\*.vshost.exe
copy Templates\V2\*.binhl LLCtest\
copy TestTemplate\*.LLC LLCtest\
cd LLCtest
mkdir Comp
cd ..
copy Templates\comp\*.MNET LLCtest\Comp\
