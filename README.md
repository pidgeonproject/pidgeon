pidgeon
=======

Pidgeon is an open source IRC client

How to build pidgeon
=====================

On Windows:
 - You need to install GTK# for .Net http://download.xamarin.com/Installer/gtk-sharp-2.12.20.msi
 - Then just open the project in visual studio and hit build

On linux:
```
# enter the folder where you downloaded the source code and execute
./configure
make
#or in order to install pidgeon system wise
make && sudo make install
#to uninstall
sudo make uninstall
```

Compiling extensions:
----------------------

Note: makefile script for linux will build all default extensions for you, this is only needed on windows and mac

You will probably want to install at least tab completion, for that you first need to build pidgeon. Then:

* Open the project file for extension you want to build
* Ensure that Pidgeon reference is existing (pidgeon.exe needs to be there)
* If it's not, simply replace it with existing reference to Pidgeon.exe so that you can build the extension.
* Hit build, the .dll file will be produced, you need to rename it to .pmod file
* Copy the file to folder where your have pidgeon /extensions and restart pidgeon

How to install pidgeon
=======================
Download installation package: http://pidgeonclient.org/wiki/Download

How to get help
================
Use our bugzilla tracker: http://pidgeonclient.org/bugzilla

Or: irc://irc.tm-irc.org/#pidgeon

How to participate
====================
See http://pidgeonclient.org/wiki/Faq there are some tips. Everyone is welcome to join this project.
