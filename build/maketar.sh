#!/bin/sh

if [ ! -f Pidgeon.csproj ];then
  echo "Your pwd needs to be root of source code, but it is: `pwd`"
  exit 1
fi

echo "Enter a version (for example 1.2.4.0)":
read v

temp=/tmp/pidgeon-$v.orig

if [ -d /tmp/pidgeon-$v.orig ];then
  echo "Remove $temp before this"
  exit 1
fi

mkdir $temp || exit 1

echo "Copying all source codes"
cp Pidgeon.csproj $temp
cp *sln $temp
cp *.cs $temp
cp -r modules $temp/modules
cp -r Core $temp/Core
cp -r Commands $temp/Commands
cp -r Resources $temp/Resources
cp -r Extensions $temp/Extensions
cp -r WinForms $temp/WinForms
cp -r Graphics $temp/Graphics
cp -r Forms $temp/Forms
cp -r ManualPages $temp/ManualPages
cp -r Protocols $temp/Protocols
cp -r Properties $temp/Properties
cp -r Scrollback $temp/Scrollback
cp -r RichTBox $temp/RichTBox
cp -r skins $temp/skins
echo "Copying makefiles"
cp -r build $temp/build
cp *sh $temp
cp Makefile $temp

echo "Creating a tarball"

cd /tmp
tar -zcf $temp.tar.gz pidgeon-$v.orig || exit 1

echo "Done, your tarball is at $temp.tar.gz"
