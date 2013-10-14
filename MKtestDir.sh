mkdir test
cp ./Templates/*.binhl ./test/
cp ./Binhl2JsWE/bin/Debug/*.exe ./test/
cp ./vqm2MNET/bin/Debug/*.exe ./test/
cp ./MNetSynt/bin/Debug/*.exe ./test/
cp ./EDF2MNET/bin/Debug/*.exe ./test/
cp ./RouteUtils/bin/Debug/*.dll ./test/
export LD_LIBRARY_PATH=/home/gl237/altera/13.0sp1/quartus/linux64/
/home/gl237/altera/13.0sp1/quartus/linux64/quartus_cdb /home/gl237/mineroute/mineroute/TestQuartusProject/test -vqm=/home/gl237/mineroute/mineroute/test/test.vqm
