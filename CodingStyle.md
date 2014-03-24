This document is intended for pidgeon developers, it describes how the developers
should write the source code.

== C# code ==

The source code written in C# is formatted using the Allman (also known as ISO)
style: see https://en.wikipedia.org/wiki/Indent_style#Allman_style

Example source code:

int Foo(int Bar)
{
    if (Bar == 2)
    {
        return -2;
    }
    while (Bar > 0)
    {
        Bar--;
    }
    return 0;
}

this style has huge advantage that brackets match vertically, so that you can
easily see if starting bracket matches the ending one.  On other hand it
consumes more line than, for example K and R style.

=== Spaces ===

There is always an empty line between two functions:

int Bluh()
{
    return 0;
}

int Bleh()
{
    return 2;
}

Wrong:

int Bluh (  ) {
  return 0;
}
int Bleh ( )
{ return 2; }

=== Comments ===

Use them everywhere when you feel they should be used :-) especially for code
that may be hard to understand by others.

Every function and class should be documented according to doxygen format
http://www.stack.nl/~dimitri/doxygen/manual/docblocks.html

Please respect code of others, don't change the code of others without
consulting that with them, there may be specific reasons why something
is written in a way it's written.

== Naming ==

=== Suffixes ===

Every function member variable is suffixed with _ so that it never collide with
instance member variables for example:

class Foo
{
    int Number;
    int Boo()
    {
        int Number_ = 5;
        return this.Number + Number_;
    }
}
    

=== CamelCase ===

Every variable and function should be named using camel case, for example:

int SomeLocalVariable;
void ThisFunctionDoesALotOfStuff();

IMPORTANT: leading character is capital not lower case

=== Indentations ===

Use 4 spaces, no tabs because different IDE's can handle tabs in different way

=== this.x.bla() vs x.bla() ===

If you are working with local object defined for whole class, use always
this->object so that it's clear on first sight that you aren't working with
local object (local to function) neither static object, it makes it easier to
read the code and prevent conflict with other variables or function with same
name defined globally for huggle.

Some IDE's display member variables in different color, but some IDE's don't.
Using "this" can make it clear to anyone who read the code.

Another reason for this, is that if there was ever implemented a global variable
or function with same name, it will likely not collide with your code.

=== Wrapping ===
The code should be wrapped to 160 characters, so if there is a line that has
more than 160 wrap it.

=== Order ===

In every class static functions go on first place, right after them are static
variables, then there is a space and follows constructors, destructor and
functions with variables.

Using this order in every class declaration (.h file) will make it easier to find
where certain stuff is. It is recommended to order everything alphabetically too.
