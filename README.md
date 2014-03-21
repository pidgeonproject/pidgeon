pidgeon
=======

Pidgeon is an open source IRC client


This branch is used to build ubuntu packages

HOWTO:
* Checkout branch in a clean directory: git clone to /tmp
* Merge master to ubuntu
* dch -i with changelog
* Commit the changelog and push
* Delete .git directory

`debuild -us -uc`

That will build the .deb package
