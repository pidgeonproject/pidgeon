pidgeon
=======

Pidgeon is an open source IRC client

How to build pidgeon
=====================

On Windows:
 - You need to install GTK# for .Net http://download.xamarin.com/Installer/gtk-sharp-2.12.20.msi
 - Then just open the project in visual studio and hit build

On linux:
 - First of all, mono isn't able to execute windows batch, there is a linux version called update.sh, you need to run it from the root folder at least once so that version.txt is created, otherwise you won't be able to build the app, you should run this to update the version number in about dialog
 - You should be able to open the project in monodevelop and build, however it may come with some warning that some classes already exist, that happens because some files which were created with stetic were later converted to manual style and were removed from stetic, however it still attempts to recreate the .cs file for these. Some of these files are still in repository just blanked, some are deleted. If stetic recreate them, blank them, save them and remove these files from interface folder
 - Hit build or

```
# create version.txt
./update.sh
# build
xbuild
```

Compiling extensions:
You will probably want to install at least tab completion, for that you first need to build pidgeon. Then open the extension you want to build and ensure that Pidgeon reference is existing, if it's not simply replace it with existing reference to Pidgeon.exe so that you can build the extension.

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
