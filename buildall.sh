#!/bin/sh

echo "Creating modules"
if [ ! -d "bin/Debug/modules" ];then
	mkdir "bin/Debug/modules" || exit 1
fi
original_path=`pwd`
echo "Building tab completion"
cd "modules/pidgeon_tc/pidgeon_tab/pidgeon_tab/"
xbuild && mv bin/Debug/pidgeon_tab.dll "$original_path/bin/Debug/modules/pidgeon_tc.pmod"
cd "$original_path"
echo "Creating network data"
cd "modules/NetworkInfo"
xbuild && mv bin/Debug/NetworkInfo.dll "$original_path/bin/Debug/modules/networkdata.pmod"
cd "$original_path"
echo "Creating nc"
cd "modules/pidgeon_notice/pidgeon_notice"
xbuild && mv bin/Debug/pidgeon_notice.dll "$original_path/bin/Debug/modules/pidgeon_notice.pmod"
cd "$original_path"
echo "Creating diff"
cd "modules/TopicDiff/TopicDiff"
xbuild && mv bin/Debug/TopicDiff.dll "$original_path/bin/Debug/modules/TopicDiff.pmod"
cd "$original_path"
echo "Creating it"
cd "modules/IgnoredText"
xbuild && mv bin/Debug/IgnoredText.dll "$original_path/bin/Debug/modules/IgnoredText.pmod"
cd "$original_path"
echo "Creating channel"
cd "modules/pidgeon_chan/pidgeon_chan"
xbuild && mv bin/Debug/pidgeon_chan.dll "$original_path/bin/Debug/modules/pidgeon_chan.pmod"
