#!/bin/sh

if [ -d obj ];then
rm -vrf obj
fi

if [ -d bin ];then
rm -vrf bin
fi

rm -vrf modules/*/bin
rm -vrf modules/*/obj
rm -vf pidgeon
