#!/bin/sh

count=`git rev-list HEAD --count`
cat templates/build.cs | sed "s/%DATA%/$count/" > build_info.cs
