
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG_X86)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;"
ASSEMBLY = bin/Debug/EDF2MNET.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../RouteUtils/bin/Debug/RouteUtils.dll
BUILD_DIR = bin/Debug

EDF2MNET_EXE_MDB_SOURCE=bin/Debug/EDF2MNET.exe.mdb
EDF2MNET_EXE_MDB=$(BUILD_DIR)/EDF2MNET.exe.mdb
ROUTEUTILS_DLL_SOURCE=../RouteUtils/bin/Debug/RouteUtils.dll
ROUTEUTILS_DLL_MDB_SOURCE=../RouteUtils/bin/Debug/RouteUtils.dll.mdb

endif

ifeq ($(CONFIG),RELEASE_X86)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/EDF2MNET.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../RouteUtils/bin/Release/RouteUtils.dll
BUILD_DIR = bin/Release

EDF2MNET_EXE_MDB=
ROUTEUTILS_DLL_SOURCE=../RouteUtils/bin/Debug/RouteUtils.dll
ROUTEUTILS_DLL_MDB_SOURCE=../RouteUtils/bin/Debug/RouteUtils.dll.mdb

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(EDF2MNET_EXE_MDB) \
	$(ROUTEUTILS_DLL) \
	$(ROUTEUTILS_DLL_MDB)  

BINARIES = \
	$(EDF2MNET)  


RESGEN=resgen2

ROUTEUTILS_DLL = $(BUILD_DIR)/RouteUtils.dll
ROUTEUTILS_DLL_MDB = $(BUILD_DIR)/RouteUtils.dll.mdb
EDF2MNET = $(BUILD_DIR)/edf2mnet

FILES = \
	Main.cs \
	AssemblyInfo.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	edf2mnet.in 

REFERENCES =  \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,ROUTEUTILS_DLL))
$(eval $(call emit-deploy-target,ROUTEUTILS_DLL_MDB))
$(eval $(call emit-deploy-wrapper,EDF2MNET,edf2mnet,x))


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
	$(call cp,$(EDF2MNET_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ROUTEUTILS_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ROUTEUTILS_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(bindir)'
	$(call cp,$(EDF2MNET),$(DESTDIR)$(bindir))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(EDF2MNET_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ROUTEUTILS_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ROUTEUTILS_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(EDF2MNET),$(DESTDIR)$(bindir))
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
