#!/bin/sh

if [ -d .git ];then
  git rev-list HEAD --count > "version.txt"
  git describe --always >> "version.txt"
fi
