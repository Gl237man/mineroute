#!/bin/bash
# Создание тестовой директории
ALTERA_BIN=/home/gl237/altera/13.0sp1/quartus/linux64/
MINEROUTE_PATH=/home/gl237/mineroute/mineroute/
mkdir test -v
cp ./Templates/*.binhl ./test/ -v
cp ./Binhl2JsWE/bin/Debug/*.exe ./test/ -v
cp ./vqm2MNET/bin/Debug/*.exe ./test/ -v
cp ./MNetSynt/bin/Debug/*.exe ./test/ -v
cp ./EDF2MNET/bin/Debug/*.exe ./test/ -v
cp ./RouteUtils/bin/Debug/*.dll ./test/ -v
export LD_LIBRARY_PATH=${ALTERA_BIN}
${ALTERA_BIN}quartus_cdb ${MINEROUTE_PATH}TestQuartusProject/test -vqm=${MINEROUTE_PATH}test/test.vqm
