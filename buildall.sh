#!/bin/sh

echo "Creating modules"
mkdir bin/Debug/modules
original_path=`pwd`
echo "Building tab completion"
cd modules/pidgeon_tc/pidgeon_tab/pidgeon_tab/
xbuild || exit 1
mv bin/Debug/pidgeon_tc.dll $original_path/bin/Debug/modules/pidgeon_tc.pmod
cd $original_path
