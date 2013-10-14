cp ./TestTemplate/*.edf ./test/
cd ./test/
mono ./EDF2MNET.exe tm
mono ./MNetSynt.exe tm
mono ./Binhl2JsWE.exe tm
