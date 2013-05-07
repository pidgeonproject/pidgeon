REM if defined ProgramFiles(x86) (
PATH=%PATH%;%ProgramFiles(x86)%\Git\bin
REM ) else (
REM PATH=%PATH%;%ProgramFiles%\Git\bin
REM )
cd %1
git rev-list HEAD --count > %2
git describe --always >> %2
