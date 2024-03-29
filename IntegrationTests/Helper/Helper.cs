﻿using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Xunit;

namespace IntegrationTests {
  internal class Helper {

    public static GH_Component FindComponentInDocumentByGroup(GH_Document doc, string groupIdentifier) {
      foreach (IGH_DocumentObject obj in doc.Objects) {
        if (obj is GH_Group group) {
          if (group.NickName == groupIdentifier) {
            Guid componentguid = group.ObjectIDs[0];

            foreach (IGH_DocumentObject obj2 in doc.Objects) {
              if (obj2.InstanceGuid == componentguid) {
                return (GH_Component)obj2;
              }
            }
          }
        }
      }
      return null;
    }

    public static void TestNoRuntimeMessagesInDocument(GH_Document doc, GH_RuntimeMessageLevel runtimeMessageLevel, string exceptComponentNamed = "") {
      foreach (IGH_DocumentObject obj in doc.Objects) {
        if (obj is GH_Component comp) {
          if (comp.Name != exceptComponentNamed) {
            Assert.Empty(comp.RuntimeMessages(runtimeMessageLevel));
          }
        }
      }
    }
  }
}
