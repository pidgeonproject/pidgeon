#!/bin/sh

if [ -d obj ];then
rm -rf obj
fi

if [ -d bin ];then
rm -rf bin
fi

rm -rf modules/*/bin
rm -rf modules/*/obj
