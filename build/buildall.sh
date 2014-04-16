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

if [ ! -d "Pidgeon/bin/$2/modules" ];then
	mkdir "Pidgeon/bin/$2/modules" || exit 1
fi
original_path=`pwd`/Pidgeon
cd Extensions || exit 1
echo "Creating search"
echo "Building tab completion"
cd "SearchText/"
xbuild $1 && mv bin/$2/SearchData.dll "$original_path/bin/$2/modules/search.pmod"
cd -
echo "Creating tab completion"
cd "pidgeon_tc/"
xbuild $1 && mv bin/$2/pidgeon_tab.dll "$original_path/bin/$2/modules/pidgeon_tc.pmod"
cd -
echo "Creating network data"
cd "NetworkInfo"
xbuild $1 && mv bin/$2/NetworkInfo.dll "$original_path/bin/$2/modules/networkdata.pmod"
cd -
echo "Creating notice"
cd "pidgeon_notice"
xbuild $1 && mv bin/$2/pidgeon_notice.dll "$original_path/bin/$2/modules/pidgeon_notice.pmod"
cd -
echo "Creating diff"
cd "TopicDiff"
xbuild $1 && mv bin/$2/TopicDiff.dll "$original_path/bin/$2/modules/TopicDiff.pmod"
cd -
echo "Creating it"
cd "IgnoredText"
xbuild $1 && mv bin/$2/IgnoredText.dll "$original_path/bin/$2/modules/IgnoredText.pmod"
cd -
echo "Creating channel"
cd "pidgeon_chan"
xbuild $1 && mv bin/$2/pidgeon_chan.dll "$original_path/bin/$2/modules/pidgeon_chan.pmod"
