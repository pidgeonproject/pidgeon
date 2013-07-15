#!/bin/sh


if [ "$1" = "" ];then
    echo "Missing parameter"
    exit 1
fi

if [ "$2" = "" ];then
    echo "Missing parameter"
    exit 1
fi

echo "Creating modules"

if [ ! -d "bin/$2/modules" ];then
	mkdir "bin/$2/modules" || exit 1
fi
original_path=`pwd`
echo "Building tab completion"
cd "modules/pidgeon_tc/"
xbuild $1 && mv bin/$2/pidgeon_tab.dll "$original_path/bin/$2/modules/pidgeon_tc.pmod"
cd "$original_path"
echo "Creating network data"
cd "modules/NetworkInfo"
xbuild $1 && mv bin/$2/NetworkInfo.dll "$original_path/bin/$2/modules/networkdata.pmod"
cd "$original_path"
echo "Creating nc"
cd "modules/pidgeon_notice"
xbuild $1 && mv bin/$2/pidgeon_notice.dll "$original_path/bin/$2/modules/pidgeon_notice.pmod"
cd "$original_path"
echo "Creating diff"
cd "modules/TopicDiff"
xbuild $1 && mv bin/$2/TopicDiff.dll "$original_path/bin/$2/modules/TopicDiff.pmod"
cd "$original_path"
echo "Creating it"
cd "modules/IgnoredText"
xbuild $1 && mv bin/$2/IgnoredText.dll "$original_path/bin/$2/modules/IgnoredText.pmod"
cd "$original_path"
echo "Creating channel"
cd "modules/pidgeon_chan"
xbuild $1 && mv bin/$2/pidgeon_chan.dll "$original_path/bin/$2/modules/pidgeon_chan.pmod"
