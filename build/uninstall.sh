#!/bin/sh

if [ "$DESTDIR" = "" ];then
    DESTDIR=/.
fi

if [ -f /usr/bin/pidgeon ];then
  echo "Removing a terminal launcher in /usr/bin"
  rm /usr/bin/pidgeon
fi

if [ ! -d $DESTDIR/usr/share/pidgeon ]; then
  echo "There is nothing to uninstall"
  exit 0
fi

echo "Removing everything in /usr/share/pidgeon"
rm -rv $DESTDIR/usr/share/pidgeon

echo "Everything was uninstalled, your system is pidgeon free"
