

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=.

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG_X86)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4 -optimize- -debug "-define:DEBUG;" "-main:Client.Program"
ASSEMBLY = bin/Debug/Pidgeon.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = winexe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

PIDGEON_EXE_MDB_SOURCE=bin/Debug/Pidgeon.exe.mdb
PIDGEON_EXE_MDB=$(BUILD_DIR)/Pidgeon.exe.mdb

endif

ifeq ($(CONFIG),RELEASE_X86)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4 -optimize- "-main:Client.Program"
ASSEMBLY = bin/Release/Pidgeon.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = winexe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

PIDGEON_EXE_MDB=

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(PIDGEON_EXE_MDB)  

BINARIES = \
	$(PIDGEON)  


RESGEN=resgen2

PIDGEON = $(BUILD_DIR)/pidgeon

FILES = \
	gtk-gui/generated.cs \
	Commands/Commands.cs \
	Commands/Commands_IRC.cs \
	Commands/Commands_Services.cs \
	Commands/Commands_System.cs \
	GTK/MessageBox.cs \
	GTK/PidgeonForm.cs \
	RichTBox/Classes.cs \
	RichTBox/Data.cs \
	RichTBox/RichTBox_Core.cs \
	RichTBox/Searching.cs \
	Scrollback/LineLogs.cs \
	Scrollback/Menu.cs \
	Scrollback/Parser.cs \
	Scrollback/ProcessingLine.cs \
	Scrollback/Scrollback.cs \
	Buffers.cs \
	Channel.cs \
	Configuration.cs \
	Extension.cs \
	Hooks.cs \
	Ignoring.cs \
	Messages.cs \
	Network.cs \
	NetworkData.cs \
	NetworkMode.cs \
	Program.cs \
	Protocol.cs \
	RevisionInfo.cs \
	ScriptingCore.cs \
	Scrollback/ScrollbackLine.cs \
	SkinCl.cs \
	Timer.cs \
	Core/Config_Core.cs \
	Core/Core.cs \
	Core/Core_IO.cs \
	Core/Core_Modules.cs \
	Protocols/ProtocolIrc.cs \
	Protocols/ProtocolQuassel.cs \
	Protocols/ProtocolXmpp.cs \
	Protocols/Qt.cs \
	Protocols/Services/Buffer.cs \
	Protocols/Services/Datagram.cs \
	Protocols/Services/ProtocolSv.cs \
	Protocols/Services/ResponsesSv.cs \
	Protocols/irc/ChannelIRC.cs \
	Protocols/irc/IRC.cs \
	Protocols/irc/NetworkIRC.cs \
	Forms/Main.cs \
	gtk-gui/Client.Forms.Main.cs \
	User.cs \
	Forms/Preferences.cs \
	gtk-gui/Client.Forms.Preferences.cs \
	Forms/Channels.cs \
	Forms/TrafficScanner.cs \
	gtk-gui/Client.Forms.TrafficScanner.cs \
	Properties/AssemblyInfo.cs \
	Properties/Resources.Designer.cs \
	Properties/Settings.Designer.cs \
	Graphics/PidgeonList.cs \
	Forms/Connection.cs \
	gtk-gui/Client.Forms.Connection.cs \
	Forms/SearchItem.cs \
	Forms/Channel_Info.cs \
	Forms/MicroChat.cs \
	Forms/ScriptEdit.cs \
	Graphics/TextBox.cs \
	Forms/Help.cs \
	gtk-gui/Client.Forms.Help.cs \
	WinForms/Recovery.cs \
	WinForms/Recovery.Designer.cs \
	Forms/Notification.cs \
	WinForms/Updater.cs \
	WinForms/Updater.Designer.cs \
	Forms/ConfigFile.cs \
	Forms/NetworkDB.cs \
	gtk-gui/Client.Forms.NetworkDB.cs \
	Graphics/Window/Window.cs \
	Graphics/Window/Window_UserList.cs 

DATA_FILES = 

RESOURCES = \
	gtk-gui/gui.stetic \
	ManualPages/Connect.txt,Client.ManualPages.Connect.txt \
	ManualPages/Oper.txt,Client.ManualPages.Oper.txt \
	ManualPages/PidgeonMan.txt,Client.ManualPages.PidgeonMan.txt \
	ManualPages/PidgeonModule.txt,Client.ManualPages.PidgeonModule.txt \
	ManualPages/PidgeonUptime.txt,Client.ManualPages.PidgeonUptime.txt \
	ManualPages/Server.txt,Client.ManualPages.Server.txt \
	ManualPages/ServicesClear.txt,Client.ManualPages.ServicesClear.txt \
	Properties/Resources.resx,Client.Properties.Resources.resources \
	Languages/en_english.txt,Client.Languages.en_english.txt \
	Languages/cs_czech.txt,Client.Languages.cs_czech.txt \
	Resources/arrow.png,Client.Resources.arrow.png \
	Resources/pigeon_clip_art_hight.ico,Client.Resources.pigeon_clip_art_hight.ico \
	Resources/at.png,Client.Resources.at.png \
	Resources/darknetwork.png,Client.Resources.darknetwork.png \
	Resources/exclamation\ mark.png,Client.Resources.exclamation\ mark.png \
	Resources/hash.png,Client.Resources.hash.png \
	Resources/Pigeon_clip_art_hight.png,Client.Resources.Pigeon_clip_art_hight.png \
	Resources/Pigeon_clip_art_hight_mini.png,Client.Resources.Pigeon_clip_art_hight_mini.png \
	version.txt,Client.version.txt \
	Resources/icon.png,Client.Resources.icon.png \
	Resources/icon_hash.png,Client.Resources.icon_hash.png 

EXTRAS = \
	Resources/50px-IRC_icon.png \
	Resources/Image1.bmp \
	Resources/Image1.jpg \
	Resources/Image2\ (1).bmp \
	Resources/Image2.bmp \
	Resources/Image2.png \
	Resources/Letter\ S.png \
	Resources/irc_channel.ico \
	Resources/regular.bmp \
	Properties/Settings.settings \
	Properties/Resources.resources \
	Graphics/Window \
	pidgeon.in 

REFERENCES =  \
	System \
	-pkg:gtk-sharp-2.0 \
	Mono.Posix \
	System.Drawing \
	System.Data \
	System.Windows.Forms \
	System.Xml \
	System.Web \
	-pkg:gtk-dotnet-2.0 \
	-pkg:glib-sharp-2.0 \
	-pkg:glade-sharp-2.0

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

#Targets
all-local: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES)  $(top_srcdir)/config.make



$(eval $(call emit-deploy-wrapper,PIDGEON,pidgeon,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

# Targets for Custom commands
DEBUG|X86_BeforeBuild:
	(cd $(srcdir) && update.cmd )


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
	mkdir -p '$(DESTDIR)$(bindir)'
	$(call cp,$(PIDGEON),$(DESTDIR)$(bindir))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(PIDGEON_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(PIDGEON),$(DESTDIR)$(bindir))
	make post-uninstall-local-hook prefix=$(prefix)
