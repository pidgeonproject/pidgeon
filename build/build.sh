#!/bin/sh

p="/p:Configuration=Release /verbosity:quiet"
folder=Release

if [ "$1" = "--debug" ]; then
    folder=Debug
    p="/p:Configuration=Debug"
fi

echo "Checking all required packages..."

if [ "`which xbuild`" = "" ];then
	echo "mono-xbuild is not installed!"
	exit 1
fi

echo "Checking links"

if [ ! -L manualpages ];then
	ln -s ManualPages manualpages
fi

if [ "`uname`" = "Linux" ];then
	if [ ! -f Configuration.cs.orig ];then
		mv Configuration.cs Configuration.cs.orig
		cp Configuration.unix Configuration.cs
	fi
fi

./update.sh || exit 1

if [ ! -f version.txt ];then
	echo "Error! unable to create a version file!"
	exit 1
fi

xbuild $p || exit 1

if [ ! -d bin/Debug ];then
    mkdir bin/Debug
fi

if [ ! -f "bin/Debug/Pidgeon.exe" ];then
    cp bin/Release/Pidgeon.exe bin/Debug
fi

if [ ! -f pidgeon ];then
	echo "#!/bin/sh" >> pidgeon
	echo "mono bin/$folder/Pidgeon.exe \$*" >> pidgeon
	chmod a+x pidgeon
fi

if [ -f build/buildall.sh ];then
	build/buildall.sh "$p" $folder
	else
	echo "Warning: there is no extension configuration present"
fi

if [ -f Configuration.cs.orig ]; then
	mv Configuration.cs.orig Configuration.cs
fi

if [ -d skins ];then
    if [ ! -d bin/$folder/skins ];then
        mkdir bin/$folder/skins
    fi

    cp skins/* bin/$folder/skins
fi

echo "Everything was built, you can start pidgeon by typing"
echo "./pidgeon"
