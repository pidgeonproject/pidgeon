#!/bin/sh

echo "Checking all required packages..."

if [ ! -f `which xbuild` ];then
	echo "mono-xbuild is not installed!"
	exit 1
fi

echo "Checking links"

if [ ! -L manualpages ];then
	ln -s ManualPages manualpages
fi

xbuild
