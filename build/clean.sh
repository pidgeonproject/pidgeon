#!/bin/sh

cd Pidgeon
if [ -d obj ];then
rm -vrf obj
fi

if [ -d bin ];then
rm -vrf bin
fi

cd -

rm -vrf Extensions/*/bin
rm -vrf Extensions/*/obj
rm -vf pidgeon
