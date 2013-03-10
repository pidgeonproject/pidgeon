

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=.

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG_X64)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4 -optimize- -debug "-define:DEBUG;TRACE"
ASSEMBLY = bin/x64/Debug/Pidgeon.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = winexe
PROJECT_REFERENCES = 
BUILD_DIR = bin/x64/Debug/

PIDGEON_EXE_MDB_SOURCE=bin/x64/Debug/Pidgeon.exe.mdb
PIDGEON_EXE_MDB=$(BUILD_DIR)/Pidgeon.exe.mdb
TCL84_DLL_SOURCE=tcl84.dll
PIDGEON_EXE_CONFIG_SOURCE=app.config

endif

ifeq ($(CONFIG),DEBUG_X86)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4 -optimize- -debug "-define:DEBUG;TRACE"
ASSEMBLY = bin/Debug/Pidgeon.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = winexe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug/

PIDGEON_EXE_MDB_SOURCE=bin/Debug/Pidgeon.exe.mdb
PIDGEON_EXE_MDB=$(BUILD_DIR)/Pidgeon.exe.mdb
TCL84_DLL_SOURCE=tcl84.dll
PIDGEON_EXE_CONFIG_SOURCE=app.config

endif

ifeq ($(CONFIG),RELEASE_X64)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4 -optimize+ "-define:TRACE"
ASSEMBLY = bin/x64/Release/Pidgeon.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = winexe
PROJECT_REFERENCES = 
BUILD_DIR = bin/x64/Release/

PIDGEON_EXE_MDB=
TCL84_DLL_SOURCE=tcl84.dll
PIDGEON_EXE_CONFIG_SOURCE=app.config

endif

ifeq ($(CONFIG),RELEASE_X86)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4 -optimize+ "-define:TRACE"
ASSEMBLY = bin/Release/Pidgeon.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = winexe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release/

PIDGEON_EXE_MDB=
TCL84_DLL_SOURCE=tcl84.dll
PIDGEON_EXE_CONFIG_SOURCE=app.config

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(PIDGEON_EXE_MDB) \
	$(TCL84_DLL) \
	$(PIDGEON_EXE_CONFIG)  

BINARIES = \
	$(PIDGEON)  


RESGEN=resgen2

TCL84_DLL = $(BUILD_DIR)/tcl84.dll
PIDGEON_EXE_CONFIG = $(BUILD_DIR)/Pidgeon.exe.config
PIDGEON = $(BUILD_DIR)/pidgeon

FILES = \
	Buffers.cs \
	Channel.cs \
	Commands.cs \
	Extension.cs \
	Forms/Channels.cs \
	Forms/Channels.Designer.cs \
	Configuration.cs \
	Core.cs \
	Forms/Connection.cs \
	Forms/Connection.Designer.cs \
	Forms/Help.cs \
	Forms/Help.Designer.cs \
	Forms/Logs.cs \
	Forms/Logs.Designer.cs \
	Forms/Main.cs \
	Forms/Main.Designer.cs \
	Forms/MicroChat.cs \
	Forms/MicroChat.Designer.cs \
	Forms/Notification.cs \
	Forms/Notification.Designer.cs \
	Forms/ScriptEdit.cs \
	Forms/ScriptEdit.Designer.cs \
	Forms/SearchItem.cs \
	Forms/SearchItem.Designer.cs \
	Forms/SettingsEd.cs \
	Forms/SettingsEd.Designer.cs \
	Forms/ShortcutBox.cs \
	Forms/ShortcutBox.Designer.cs \
	Forms/SkinEd.cs \
	Forms/SkinEd.Designer.cs \
	Ignoring.cs \
	IRC.cs \
	ListViewDB.cs \
	NetworkMode.cs \
	ProtocolXmpp.cs \
	Qt.cs \
	Services/Datagram.cs \
	Services/Loader.cs \
	Services/ResponsesSv.cs \
	RevisionInfo.cs \
	SBABOX/Line.cs \
	SBABOX/Link.cs \
	SBABOX/SBABox.cs \
	SBABOX/SBABox.Designer.cs \
	Forms/TrafficScanner.cs \
	Forms/TrafficScanner.Designer.cs \
	Forms/Warning.cs \
	Forms/Warning.Designer.cs \
	Hooks.cs \
	PidgeonList.cs \
	PidgeonList.Designer.cs \
	Forms/Recovery.cs \
	Forms/Recovery.Designer.cs \
	Forms/Channel_Info.cs \
	Forms/Channel_Info.Designer.cs \
	Forms/Updater.cs \
	Forms/Updater.Designer.cs \
	Services/ProtocolSv.cs \
	ScriptingCore.cs \
	Scrollback/Menu.cs \
	Scrollback/ProcessingLine.cs \
	Timer.cs \
	Window.cs \
	Window.Designer.cs \
	Messages.cs \
	Network.cs \
	Forms/Preferences.cs \
	Forms/Preferences.Designer.cs \
	DataProcessor.cs \
	Program.cs \
	Properties/AssemblyInfo.cs \
	Protocol.cs \
	ProtocolIrc.cs \
	ProtocolQuassel.cs \
	Scrollback/Scrollback.cs \
	Scrollback/Scrollback.Designer.cs \
	SkinCl.cs \
	TextBox.cs \
	TextBox.Designer.cs \
	User.cs \
	Properties/Resources.Designer.cs \
	Properties/Settings.Designer.cs 

DATA_FILES = 

RESOURCES = \
	Forms/Channels.resx,Client.Channels.resources \
	Forms/Channel_Info.resx,Client.Channel_Info.resources \
	Forms/Connection.resx,Client.Connection.resources \
	Forms/Help.resx,Client.Help.resources \
	Forms/Logs.resx,Client.Forms.Logs.resources \
	Forms/Main.resx,Client.Main.resources \
	Forms/MicroChat.resx,Client.MicroChat.resources \
	Forms/Notification.resx,Client.Notification.resources \
	Forms/Preferences.resx,Client.Preferences.resources \
	Forms/ScriptEdit.resx,Client.Forms.ScriptEdit.resources \
	Forms/SearchItem.resx,Client.SearchItem.resources \
	Forms/SettingsEd.resx,Client.SettingsEd.resources \
	Forms/ShortcutBox.resx,Client.ShortcutBox.resources \
	Forms/SkinEd.resx,Client.SkinEd.resources \
	SBABOX/SBABox.resx,Client.SBABox.resources \
	Forms/TrafficScanner.resx,Client.TrafficScanner.resources \
	Forms/Updater.resx,Client.Updater.resources \
	Forms/Warning.resx,Client.Warning.resources \
	PidgeonList.resx,Client.PidgeonList.resources \
	Forms/Recovery.resx,Client.Recovery.resources \
	Window.resx,Client.Window.resources \
	Properties/Resources.resx,Client.Properties.Resources.resources \
	Scrollback/Scrollback.resx,Client.Scrollback.resources \
	TextBox.resx,Client.TextBox.resources \
	version.txt,Client.version.txt 

EXTRAS = \
	app.config \
	Properties/Settings.settings \
	Languages/cs_czech.txt \
	Languages/en_english.txt \
	Resources/icon.png \
	ManualPages/Connect.txt \
	ManualPages/Oper.txt \
	ManualPages/PidgeonMan.txt \
	ManualPages/PidgeonModule.txt \
	ManualPages/PidgeonUptime.txt \
	ManualPages/Server.txt \
	Resources/darknetwork.png \
	Pigeon_clip_art_hight.ico \
	Resources/Pigeon_clip_art_hight.png \
	tcl84.dll \
	Resources/Image2.png \
	Resources/Image2.bmp \
	Resources/50px-IRC_icon.png \
	Resources/Image1.bmp \
	Resources/irc_channel.ico \
	Resources/regular.bmp \
	Resources/Image1.jpg \
	pidgeon.in 

REFERENCES =  \
	System \
	System.Web \
	System.Data \
	System.Drawing \
	System.Windows.Forms \
	System.Xml \
	System.Xml.Linq

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

#Targets
all-local: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES)  $(top_srcdir)/config.make



$(eval $(call emit-deploy-target,TCL84_DLL))
$(eval $(call emit-deploy-target,PIDGEON_EXE_CONFIG))
$(eval $(call emit-deploy-wrapper,PIDGEON,pidgeon,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


$(ASSEMBLY_MDB): $(ASSEMBLY)
$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	make install-satellite-assemblies prefix=$(prefix)
	mkdir -p '$(DESTDIR)$(libdir)/$(PACKAGE)'
	$(call cp,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(PIDGEON_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(TCL84_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(PIDGEON_EXE_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(bindir)'
	$(call cp,$(PIDGEON),$(DESTDIR)$(bindir))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(PIDGEON_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(TCL84_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(PIDGEON_EXE_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(PIDGEON),$(DESTDIR)$(bindir))
	make post-uninstall-local-hook prefix=$(prefix)
