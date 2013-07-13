#!/bin/sh

if [ ! -f bin/Release/Pidgeon.exe ] || [ ! -d bin/Release/modules ];then
  echo "ERROR: you need to build pidgeon first"
  exit 1
fi

echo "Creating directory structure in /usr/share"
if [ ! -d /usr/share/pidgeon ]; then
  mkdir /usr/share/pidgeon
fi

if [ -d /usr/share/pidgeon/skins ]; then
  echo "Cleaning old skins"
  rm -vr /usr/share/pidgeon/skins
fi

mkdir /usr/share/pidgeon/skins

if [ -d "/usr/share/pidgeon/modules" ]; then
  echo "Cleaning old modules"
  rm -vr "/usr/share/pidgeon/modules"
fi

mkdir "/usr/share/pidgeon/modules"

echo "Copying the binaries to /usr/share"

cp -v bin/Release/modules/* "/usr/share/pidgeon/modules"
cp -v bin/Release/skins/* "/usr/share/pidgeon/skins"
cp -v bin/Release/Pidgeon.exe "/usr/share/pidgeon"

echo "Creating a terminal launcher in /usr/bin"

if [ -f /usr/bin/pidgeon ]; then
  echo "WARNING: there is already existing pidgeon launcher in /usr/bin/pidgeon making backup"
  suffix=0
  while [ -f /tmp/pidgeon.$suffix ]
  do
      suffix=`expr $suffix + 1`
  done
  cp -v /usr/bin/pidgeon "/tmp/pidgeon.$suffix"
  rm -f /usr/bin/pidgeon
fi

echo "#!/bin/sh" > /usr/bin/pidgeon
echo "mono /usr/share/pidgeon/Pidgeon.exe \$*" >> /usr/bin/pidgeon
chmod a+x /usr/bin/pidgeon

echo "Everything was installed, you can launch pidgeon using \"pidgeon\""
