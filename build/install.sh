#!/bin/sh

if [ "$DESTDIR" = "" ];then
    DESTDIR=/.
fi

if [ x"$1" != x ];then
    DESTDIR=$1
fi

echo "Installing pidgeon to $DESTDIR"

if [ ! -f bin/Release/Pidgeon.exe ] || [ ! -d bin/Release/modules ];then
  echo "ERROR: you need to build pidgeon first"
  exit 1
fi

echo "Creating directory structure in $DESTDIR/usr/share"
if [ ! -d "$DESTDIR/usr" ]; then
  mkdir "$DESTDIR/usr" || exit 1
fi
if [ ! -d "$DESTDIR/usr/share" ]; then
  mkdir "$DESTDIR/usr/share" || exit 1
fi
if [ ! -d "$DESTDIR/usr/share/pidgeon" ]; then
  mkdir "$DESTDIR/usr/share/pidgeon" || exit 1
fi

if [ -d "$DESTDIR/usr/share/pidgeon/skins" ]; then
  echo "Cleaning old skins"
  rm -vr "$DESTDIR/usr/share/pidgeon/skins" || exit 1
fi

mkdir "$DESTDIR/usr/share/pidgeon/skins" || exit 1

if [ -d "$DESTDIR/usr/share/pidgeon/modules" ]; then
  echo "Cleaning old modules"
  rm -vr "$DESTDIR/usr/share/pidgeon/modules" || exit 1
fi

mkdir "$DESTDIR/usr/share/pidgeon/modules" || exit 1

echo "Copying the binaries to /usr/share"

cp -v bin/Release/modules/* "$DESTDIR/usr/share/pidgeon/modules" || exit 1
cp -v bin/Release/skins/* "$DESTDIR/usr/share/pidgeon/skins" || exit 1
cp -v bin/Release/Pidgeon.exe "$DESTDIR/usr/share/pidgeon" || exit 1
cp -v bin/Release/Pidgeon.XML "$DESTDIR/usr/share/pidgeon" || exit 1
chmod a+rx "$DESTDIR/usr/share/pidgeon/" || exit 1
chmod a+rx "$DESTDIR/usr/share/pidgeon/skins" || exit 1
chmod a+rx "$DESTDIR/usr/share/pidgeon/modules" || exit 1
chmod -R a+r "$DESTDIR/usr/share/pidgeon" || exit 1

echo "Creating a terminal launcher in $DESTDIR/usr/bin"

if [ -f "$DESTDIR/usr/bin/pidgeon" ]; then
  echo "WARNING: there is already existing pidgeon launcher in $DESTDIR/usr/bin/pidgeon making backup"
  suffix=0
  while [ -f /tmp/pidgeon.$suffix ]
  do
      suffix=`expr $suffix + 1`
  done
  cp -v "$DESTDIR/usr/bin/pidgeon" "/tmp/pidgeon.$suffix"
  rm -f "$DESTDIR/usr/bin/pidgeon"
fi

echo "#!/bin/sh" > "$DESTDIR/usr/bin/pidgeon"
echo "mono $DESTDIR/usr/share/pidgeon/Pidgeon.exe \$*" >> "$DESTDIR/usr/bin/pidgeon"
chmod a+x "$DESTDIR/usr/bin/pidgeon"
if [ ! -d "$DESTDIR/usr/share/man" ];then
    mkdir "$DESTDIR/usr/share/man" || exit 1
fi
if [ ! -d "$DESTDIR/usr/share/man/man1" ];then
    mkdir "$DESTDIR/usr/share/man/man1"
fi
cp man/* "$DESTDIR/usr/share/man/man1" || exit 1
gzip "$DESTDIR/usr/share/man/man1/pidgeon.1" || exit 1

echo "Everything was installed, you can launch pidgeon using \"pidgeon\""
