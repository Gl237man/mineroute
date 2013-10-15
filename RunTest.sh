#!/bin/bash
#  Запуск теста на синтез из edf
# edf сгенерирован с использованием IcarusVerilog
cp ./TestTemplate/*.edf ./test/
cd ./test/
mono ./EDF2MNET.exe tm
mono ./MNetSynt.exe tm
mono ./Binhl2JsWE.exe tm
