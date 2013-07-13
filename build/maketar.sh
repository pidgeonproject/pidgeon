#!/bin/sh

if [ ! -f Pidgeon.csproj ];then
  echo "Your pwd needs to be root of source code, but it is: `pwd`"
  exit 1
fi

echo "Enter a version (for example 1.2.4.0)":
read v

if [ -d /tmp/pidgeon-$v.orig ];then
  echo "Remove /tmp/pidgeon-$v.orig before this"
  exit 1
fi

mkdir /tmp/p
