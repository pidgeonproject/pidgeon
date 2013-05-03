#!/bin/sh

echo "Creating modules"
mkdir "bin/Debug/modules"
original_path=`pwd`
echo "Building tab completion"
cd "modules/pidgeon_tc/pidgeon_tab/pidgeon_tab/"
xbuild && mv bin/Debug/pidgeon_tab.dll "$original_path/bin/Debug/modules/pidgeon_tc.pmod"
cd "$original_path"
echo "Creating network data"
cd "modules/NetworkInfo"
xbuild && mv bin/Debug/NetworkInfo.dll "$original_path/bin/Debug/modules/networkdata.pmod"
