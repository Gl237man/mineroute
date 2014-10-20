mkdir MCtest
copy vqm2MNET\bin\Debug\*.exe MCtest\
copy vqm2MNET\bin\Debug\*.dll MCtest\
copy MnetLutDecomposite\bin\Debug\*.exe MCtest\
copy MnetLutDecomposite\bin\Debug\*.dll MCtest\
copy Mnetsynt3\bin\Debug\*.exe MCtest\
copy Mnetsynt3\bin\Debug\*.dll MCtest\
copy Binhl2JsWE\bin\Debug\*.exe MCtest\
copy Binhl2JsWE\bin\Debug\*.dll MCtest\
copy MnetLutOptimise\bin\Debug\*.exe MCtest\
copy MnetLutOptimise\bin\Debug\*.dll MCtest\
copy RunMCTest.cmd MCtest\
copy MNETLib.BinLib MCtest\
del MCtest\*.vshost.exe
copy Templates\V2\*.binhl MCtest\
copy TestTemplate\*.vqm MCtest\