;NSIS Modern User Interface
;Basic Example Script
;Written by Joost Verburg

;--------------------------------
;Include Modern UI

!include "MUI2.nsh"

;--------------------------------
;General

;Name and file
Name "Pidgeon"
OutFile "pidgeon_setup.exe"

!define APPNAME "Pidgeon"
!define DESCRIPTION "Pidgeon client"
!define COMPANYNAME "Pidgeon"
# These three must be integers
!define VERSIONMAJOR 1
!define VERSIONMINOR 2
!define VERSIONBUILD 0
# These will be displayed by the "Click here for support information" link in "Add/Remove Programs"
# It is possible to use "mailto:" links in here to open the email client
!define HELPURL "http://pidgeonclient.org/wiki" # "Support Information" link
!define ABOUTURL "http://pidgeonclient.org/wiki" # "Publisher" link

  ;Default installation folder
 InstallDir "$PROGRAMFILES\${APPNAME}"
 
  ;Request application privileges for Windows Vista
  RequestExecutionLevel admin

;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_LICENSE "C:\Users\petr.bena\Documents\Visual Studio 2010\gpl-3.0.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  
;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "Pidgeon" SecDummy
SectionIn RO
  SetOutPath "$INSTDIR"
  
  ;ADD YOUR OWN FILES HERE...
  file "tcl84.dll"
  file "Pidgeon.exe" 
  ;Store installation folder
  WriteUninstaller "$INSTDIR\Uninstall.exe"

SectionEnd


Section "Modules" SecModules
  SetOutPath "$INSTDIR"
  CreateDirectory $INSTDIR\modules
  file /oname=modules\pidgeon_tab.pmod "modules\pidgeon_tab.pmod"
SectionEnd

Section "Modules" SecFreenode
  SetOutPath "$INSTDIR"
  CreateDirectory $INSTDIR\modules
  file /oname=modules\freenode.pmod "modules\freenode.pmod"
SectionEnd
;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecModules ${LANG_ENGLISH} "This will install recommended modules for pidgeon, if you won't install this pidgeon will not have so many features."
  LangString DESC_SecFreenode ${LANG_ENGLISH} "This will install freenode module for pidgeon, that enables some extra features useful on freenode."
  LangString DESC_SecDummy ${LANG_ENGLISH} "This will install the pidgeon client."

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecDummy} $(DESC_SecDummy)
    !insertmacro MUI_DESCRIPTION_TEXT ${SecModules} $(DESC_SecModules)
    !insertmacro MUI_DESCRIPTION_TEXT ${SecFreenode} $(DESC_SecFreenode)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;ADD YOUR OWN FILES HERE...
  ;
	delete $INSTDIR\modules\pidgeon_tab.pmod
	delete $INSTDIR\tcl84.dll
	delete $INSTDIR\Pidgeon.exe
  Delete "$INSTDIR\Uninstall.exe"
  RMDir "$INSTDIR\modules"
  RMDir "$INSTDIR"

SectionEnd

