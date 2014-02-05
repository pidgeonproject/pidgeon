#!/bin/sh

if [ "$DESTDIR" = "" ];then
    DESTDIR=/.
fi

if [ ! -f bin/Release/Pidgeon.exe ] || [ ! -d bin/Release/modules ];then
  echo "ERROR: you need to build pidgeon first"
  exit 1
fi

echo "Creating directory structure in /usr/share"
if [ ! -d $DESTDIR/usr/share/pidgeon ]; then
  mkdir $DESTDIR/usr/share/pidgeon
fi

if [ -d $DESTDIR/usr/share/pidgeon/skins ]; then
  echo "Cleaning old skins"
  rm -vr $DESTDIR/usr/share/pidgeon/skins
fi

mkdir $DESTDIR/usr/share/pidgeon/skins

if [ -d "$DESTDIR/usr/share/pidgeon/modules" ]; then
  echo "Cleaning old modules"
  rm -vr "$DESTDIR/usr/share/pidgeon/modules"
fi

mkdir "$DESTDIR/usr/share/pidgeon/modules"

echo "Copying the binaries to /usr/share"

cp -v bin/Release/modules/* "$DESTDIR/usr/share/pidgeon/modules"
cp -v bin/Release/skins/* "$DESTDIR/usr/share/pidgeon/skins"
cp -v bin/Release/Pidgeon.exe "$DESTDIR/usr/share/pidgeon"
cp -v bin/Release/Pidgeon.XML "$DESTDIR/usr/share/pidgeon"
chmod a+rx "$DESTDIR/usr/share/pidgeon/"
chmod a+rx "$DESTDIR/usr/share/pidgeon/skins"
chmod a+rx "$DESTDIR/usr/share/pidgeon/modules"
chmod -R a+r "$DESTDIR/usr/share/pidgeon"

echo "Creating a terminal launcher in /usr/bin"

if [ -f $DESTDIR/usr/bin/pidgeon ]; then
  echo "WARNING: there is already existing pidgeon launcher in /usr/bin/pidgeon making backup"
  suffix=0
  while [ -f /tmp/pidgeon.$suffix ]
  do
      suffix=`expr $suffix + 1`
  done
  cp -v $DESTDIR/usr/bin/pidgeon "/tmp/pidgeon.$suffix"
  rm -f $DESTDIR/usr/bin/pidgeon
fi

echo "#!/bin/sh" > /usr/bin/pidgeon
echo "mono $DESTDIR/usr/share/pidgeon/Pidgeon.exe \$*" >> $DESTDIR/usr/bin/pidgeon
chmod a+x $DESTDIR/usr/bin/pidgeon

echo "Everything was installed, you can launch pidgeon using \"pidgeon\""
