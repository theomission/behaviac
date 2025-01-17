# GNU Make project makefile autogenerated by Premake
ifndef config
  config=debug64
endif

ifndef verbose
  SILENT = @
endif

CC = gcc
CXX = g++
AR = ar

ifndef RESCOMP
  ifdef WINDRES
    RESCOMP = $(WINDRES)
  else
    RESCOMP = windres
  endif
endif

ifeq ($(config),debug64)
  OBJDIR     = ../../intermediate/debug/linux/btunittest/x64
  TARGETDIR  = ../../bin
  TARGET     = $(TARGETDIR)/btunittest_debugstatic_linux_gmake.exe
  DEFINES   += -D_DEBUG -DDEBUG
  INCLUDES  += -I../../inc -I../../inc -I../../../../include -I../../test/btunittest
  ALL_CPPFLAGS  += $(CPPFLAGS) -MMD -MP $(DEFINES) $(INCLUDES)
  ALL_CFLAGS    += $(CFLAGS) $(ALL_CPPFLAGS) $(ARCH) -g -Wall -Wextra -Werror -ffast-math -m64 -Wno-invalid-offsetof -Wno-array-bounds -Wno-unused-local-typedefs -Wno-maybe-uninitialized -Woverloaded-virtual -Wnon-virtual-dtor -Wfloat-equal -finput-charset=UTF-8 -Wno-unused-parameter -Wno-unused-variable
  ALL_CXXFLAGS  += $(CXXFLAGS) $(ALL_CFLAGS) -fno-exceptions 
  ALL_RESFLAGS  += $(RESFLAGS) $(DEFINES) $(INCLUDES)
  ALL_LDFLAGS   += $(LDFLAGS) -L../../../../lib -L../../lib -m64 -L/usr/lib64
  LDDEPS    += ../../lib/libbehaviac_debugstatic_linux_gmake.a
  LIBS      += $(LDDEPS)
  LINKCMD    = $(CXX) -o $(TARGET) $(OBJECTS) $(RESOURCES) $(ARCH) $(ALL_LDFLAGS) $(LIBS) -lpthread
  define PREBUILDCMDS
  endef
  define PRELINKCMDS
  endef
  define POSTBUILDCMDS
  endef
endif

ifeq ($(config),release64)
  OBJDIR     = ../../intermediate/release/linux/btunittest/x64
  TARGETDIR  = ../../bin
  TARGET     = $(TARGETDIR)/btunittest_releasestatic_linux_gmake.exe
  DEFINES   += -DNDEBUG
  INCLUDES  += -I../../inc -I../../inc -I../../../../include -I../../test/btunittest
  ALL_CPPFLAGS  += $(CPPFLAGS) -MMD -MP $(DEFINES) $(INCLUDES)
  ALL_CFLAGS    += $(CFLAGS) $(ALL_CPPFLAGS) $(ARCH) -O2 -Wall -Wextra -Werror -ffast-math -m64 -Wno-strict-aliasing -Wno-invalid-offsetof -Wno-array-bounds -Wno-unused-local-typedefs -Wno-maybe-uninitialized -Woverloaded-virtual -Wnon-virtual-dtor -Wfloat-equal -finput-charset=UTF-8 -Wno-unused-parameter -Wno-unused-variable
  ALL_CXXFLAGS  += $(CXXFLAGS) $(ALL_CFLAGS) -fno-exceptions 
  ALL_RESFLAGS  += $(RESFLAGS) $(DEFINES) $(INCLUDES)
  ALL_LDFLAGS   += $(LDFLAGS) -L../../../../lib -L../../lib -s -m64 -L/usr/lib64
  LDDEPS    += ../../lib/libbehaviac_releasestatic_linux_gmake.a
  LIBS      += $(LDDEPS)
  LINKCMD    = $(CXX) -o $(TARGET) $(OBJECTS) $(RESOURCES) $(ARCH) $(ALL_LDFLAGS) $(LIBS) -lpthread
  define PREBUILDCMDS
  endef
  define PRELINKCMDS
  endef
  define POSTBUILDCMDS
  endef
endif

OBJECTS := \
	$(OBJDIR)/behaviacsystem.o \
	$(OBJDIR)/BehaviacWorkspace.o \
	$(OBJDIR)/main.o \
	$(OBJDIR)/AgentArrayAccessTest.o \
	$(OBJDIR)/AgentNodeTest.o \
	$(OBJDIR)/CustomPropertyAgent.o \
	$(OBJDIR)/EmployeeParTestAgent.o \
	$(OBJDIR)/FSMAgentTest.o \
	$(OBJDIR)/HTNAgentHouse.o \
	$(OBJDIR)/HTNAgentHouseBase.o \
	$(OBJDIR)/HTNAgentTravel.o \
	$(OBJDIR)/ParTestAgent.o \
	$(OBJDIR)/ParTestAgentBase.o \
	$(OBJDIR)/ParTestRegNameAgent.o \
	$(OBJDIR)/PreconEffectorAgent.o \
	$(OBJDIR)/PreconEffectorTest.o \
	$(OBJDIR)/PropertyReadonlyAgent.o \
	$(OBJDIR)/UnitTestTypes.o \
	$(OBJDIR)/ArrayAccessTest.o \
	$(OBJDIR)/PreconEffectorUintTest.o \
	$(OBJDIR)/customizedtypes.o \
	$(OBJDIR)/extendreftype.o \
	$(OBJDIR)/fsmtestbase_0.o \
	$(OBJDIR)/fsmunittest.o \
	$(OBJDIR)/htnhouseunittest.o \
	$(OBJDIR)/htntravelunittest.o \
	$(OBJDIR)/testtraits.o \
	$(OBJDIR)/testtype.o \
	$(OBJDIR)/DecorationNodeUnitTest.o \
	$(OBJDIR)/EnterExitActionUnitTest.o \
	$(OBJDIR)/EventUnitTest.o \
	$(OBJDIR)/NodeUnitTest.o \
	$(OBJDIR)/ParallelNodeUnitTest.o \
	$(OBJDIR)/PredicateUnitTest.o \
	$(OBJDIR)/QueryUnitTest.o \
	$(OBJDIR)/btloadtest.o \
	$(OBJDIR)/btmethodtest.o \
	$(OBJDIR)/btunittest.o \
	$(OBJDIR)/parallelnodetest.o \
	$(OBJDIR)/probabilityselectortest.o \
	$(OBJDIR)/reflectionunittest.o \
	$(OBJDIR)/selectorlooptest.o \
	$(OBJDIR)/selectorstochastictest.o \
	$(OBJDIR)/sequencestochastictest.o \
	$(OBJDIR)/ParPropertyUnitTest.o \
	$(OBJDIR)/PropertyReadonlyUnitTest.o \

RESOURCES := \

SHELLTYPE := msdos
ifeq (,$(ComSpec)$(COMSPEC))
  SHELLTYPE := posix
endif
ifeq (/bin,$(findstring /bin,$(SHELL)))
  SHELLTYPE := posix
endif

.PHONY: clean prebuild prelink

all: $(TARGETDIR) $(OBJDIR) prebuild prelink $(TARGET)
	@:

$(TARGET): $(GCH) $(OBJECTS) $(LDDEPS) $(RESOURCES)
	@echo Linking btunittest
	$(SILENT) $(LINKCMD)
	$(POSTBUILDCMDS)

$(TARGETDIR):
	@echo Creating $(TARGETDIR)
ifeq (posix,$(SHELLTYPE))
	$(SILENT) mkdir -p $(TARGETDIR)
else
	$(SILENT) mkdir $(subst /,\\,$(TARGETDIR))
endif

$(OBJDIR):
	@echo Creating $(OBJDIR)
ifeq (posix,$(SHELLTYPE))
	$(SILENT) mkdir -p $(OBJDIR)
else
	$(SILENT) mkdir $(subst /,\\,$(OBJDIR))
endif

clean:
	@echo Cleaning btunittest
ifeq (posix,$(SHELLTYPE))
	$(SILENT) rm -f  $(TARGET)
	$(SILENT) rm -rf $(OBJDIR)
else
	$(SILENT) if exist $(subst /,\\,$(TARGET)) del $(subst /,\\,$(TARGET))
	$(SILENT) if exist $(subst /,\\,$(OBJDIR)) rmdir /s /q $(subst /,\\,$(OBJDIR))
endif

prebuild:
	$(PREBUILDCMDS)

prelink:
	$(PRELINKCMDS)

ifneq (,$(PCH))
.NOTPARALLEL: $(GCH) $(PCH)
$(GCH): $(PCH)
	@echo $(notdir $<)
	$(SILENT) $(CXX) -x c++-header $(ALL_CXXFLAGS) -MMD -MP $(DEFINES) $(INCLUDES) -o "$@" -MF "$(@:%.gch=%.d)" -c "$<"
endif

$(OBJDIR)/behaviacsystem.o: ../../test/btunittest/behaviacsystem.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/BehaviacWorkspace.o: ../../test/btunittest/BehaviacWorkspace.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/main.o: ../../test/btunittest/main.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/AgentArrayAccessTest.o: ../../test/btunittest/Agent/AgentArrayAccessTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/AgentNodeTest.o: ../../test/btunittest/Agent/AgentNodeTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/CustomPropertyAgent.o: ../../test/btunittest/Agent/CustomPropertyAgent.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/EmployeeParTestAgent.o: ../../test/btunittest/Agent/EmployeeParTestAgent.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/FSMAgentTest.o: ../../test/btunittest/Agent/FSMAgentTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/HTNAgentHouse.o: ../../test/btunittest/Agent/HTNAgentHouse.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/HTNAgentHouseBase.o: ../../test/btunittest/Agent/HTNAgentHouseBase.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/HTNAgentTravel.o: ../../test/btunittest/Agent/HTNAgentTravel.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/ParTestAgent.o: ../../test/btunittest/Agent/ParTestAgent.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/ParTestAgentBase.o: ../../test/btunittest/Agent/ParTestAgentBase.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/ParTestRegNameAgent.o: ../../test/btunittest/Agent/ParTestRegNameAgent.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/PreconEffectorAgent.o: ../../test/btunittest/Agent/PreconEffectorAgent.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/PreconEffectorTest.o: ../../test/btunittest/Agent/PreconEffectorTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/PropertyReadonlyAgent.o: ../../test/btunittest/Agent/PropertyReadonlyAgent.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/UnitTestTypes.o: ../../test/btunittest/Agent/UnitTestTypes.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/ArrayAccessTest.o: ../../test/btunittest/ArrayAccessTest/ArrayAccessTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/PreconEffectorUintTest.o: ../../test/btunittest/attachments/PreconEffectorUintTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/customizedtypes.o: ../../test/btunittest/behaviac_generated/types/customizedtypes.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/extendreftype.o: ../../test/btunittest/ext/extendreftype.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/fsmtestbase_0.o: ../../test/btunittest/FSMTest/fsmtestbase_0.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/fsmunittest.o: ../../test/btunittest/FSMTest/fsmunittest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/htnhouseunittest.o: ../../test/btunittest/HTNTest/htnhouseunittest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/htntravelunittest.o: ../../test/btunittest/HTNTest/htntravelunittest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/testtraits.o: ../../test/btunittest/Meta/testtraits.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/testtype.o: ../../test/btunittest/Meta/testtype.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/DecorationNodeUnitTest.o: ../../test/btunittest/NodeTest/DecorationNodeUnitTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/EnterExitActionUnitTest.o: ../../test/btunittest/NodeTest/EnterExitActionUnitTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/EventUnitTest.o: ../../test/btunittest/NodeTest/EventUnitTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/NodeUnitTest.o: ../../test/btunittest/NodeTest/NodeUnitTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/ParallelNodeUnitTest.o: ../../test/btunittest/NodeTest/ParallelNodeUnitTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/PredicateUnitTest.o: ../../test/btunittest/NodeTest/PredicateUnitTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/QueryUnitTest.o: ../../test/btunittest/NodeTest/QueryUnitTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/btloadtest.o: ../../test/btunittest/Others/btloadtest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/btmethodtest.o: ../../test/btunittest/Others/btmethodtest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/btunittest.o: ../../test/btunittest/Others/btunittest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/parallelnodetest.o: ../../test/btunittest/Others/parallelnodetest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/probabilityselectortest.o: ../../test/btunittest/Others/probabilityselectortest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/reflectionunittest.o: ../../test/btunittest/Others/reflectionunittest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/selectorlooptest.o: ../../test/btunittest/Others/selectorlooptest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/selectorstochastictest.o: ../../test/btunittest/Others/selectorstochastictest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/sequencestochastictest.o: ../../test/btunittest/Others/sequencestochastictest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/ParPropertyUnitTest.o: ../../test/btunittest/ParPropertyTest/ParPropertyUnitTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

$(OBJDIR)/PropertyReadonlyUnitTest.o: ../../test/btunittest/ParPropertyTest/PropertyReadonlyUnitTest.cpp
	@echo $(notdir $<)
	$(SILENT) $(CXX) $(ALL_CXXFLAGS) $(FORCE_INCLUDE) -o "$@" -MF $(@:%.o=%.d) -c "$<"

-include $(OBJECTS:%.o=%.d)
ifneq (,$(PCH))
  -include $(OBJDIR)/$(notdir $(PCH)).d
endif
