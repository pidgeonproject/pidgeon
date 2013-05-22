#!/bin/sh

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

sh update.sh

if [ ! -f version.txt ];then
	echo "Error! unable to create a version file!"
	exit 1
fi

xbuild || exit 1

if [ ! -f pidgeon ];then
	echo "#!/bin/sh" >> pidgeon
	echo 'mono bin/Debug/Pidgeon.exe $*' >> pidgeon
	chmod a+x pidgeon
fi

if [ -f buildall.sh ];then
	sh buildall.sh
	else
	echo "Warning: there is no extension configuration present"
fi

if [ -f Configuration.cs.orig ]; then
	mv Configuration.cs.orig Configuration.cs
fi

if [ -d skins ];then
    if [ ! -d bin/Debug/skins ];then
        mkdir bin/Debug/skins
    fi

    cp skins/* bin/Debug/skins
fi

echo "Everything was built, you can start pidgeon by typing"
echo "./pidgeon"
