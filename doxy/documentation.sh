#!/bin/sh

sh ./update.sh
version=`head -n 1 version.txt`
cat doxy/doxygen.conf | sed "s/%BUILD%/$version/" > doxygen.cf
doxygen doxygen.cf
