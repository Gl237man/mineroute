#!/bin/bash
# Создание тестовой директории
ALTERA_BIN=/home/gl237/altera/13.0sp1/quartus/linux64/
MINEROUTE_PATH=/home/gl237/mineroute/mineroute/
mkdir test
cp ./Templates/*.binhl ./test/
cp ./Binhl2JsWE/bin/Debug/*.exe ./test/
cp ./vqm2MNET/bin/Debug/*.exe ./test/
cp ./MNetSynt/bin/Debug/*.exe ./test/
cp ./EDF2MNET/bin/Debug/*.exe ./test/
cp ./RouteUtils/bin/Debug/*.dll ./test/
export LD_LIBRARY_PATH=$ALTERA_BIN
$(ALTERA_BIN)quartus_cdb $(MINEROUTE_PATH)TestQuartusProject/test -vqm=$(MINEROUTE_PATH)test/test.vqm
