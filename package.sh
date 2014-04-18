#!/bin/sh

debs="-us -uc"
if [ ! -d .git ];then
    echo "This command can be run from git repository only"
    exit 1
fi

if [ x"$1" = "x--source" ]; then
    echo "Building only source"
    debs="-sa -S"
fi

echo "Press enter to start make of debian package"
read x

echo "Making the target package"

temp=/tmp/pidgeon_pkg
suffix=0

while [ -d "$temp" ]
do
    temp="/tmp/pidgeon_pkg.$suffix"
    suffix=`expr $suffix + 1`
done

mkdir $temp || exit 1
cp -r . $temp/pidgeon || exit 1
cd "$temp/pidgeon" || exit 1

echo "Doing sanity checks"

if [ -d ".git" ];then
    cd Pidgeon || exit 1
    sh update.sh || exit 1
    cd .. || exit 1
    echo "Removing .git folder"
    rm -rf .git || exit 1
fi

if [ -d "Pidgeon/bin" ];then
    echo "Cleaning the binaries"
    if [ ! -f "Makefile" ];then
        cp Makefile.in Makefile || exit 1
    fi
    make clean || exit 1
    rm Makefile || exit 1
    echo "Everything cleaned"
fi

if [ -f "Makefile" ];then
    echo "Deleting old makefile"
    rm Makefile || exit 1
fi

echo "Running update"
cd Pidgeon || exit 1
./update.sh || exit 1
cd .. || exit 1

gpg -k
echo "Do you want to use key? if yes provide the key here or hit enter"
read priv

if [ x"$priv" != x ];then
    echo "Using $priv"
    priv="-k$priv"
fi
echo "Running debuild $priv $debs"
debuild $priv $debs || exit 1

echo "Packages were built in $temp"
ls ../*.deb

