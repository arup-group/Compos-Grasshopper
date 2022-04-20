using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// are we sure about this namespace?
using static ComposGH.Parameters.DesignCode;

namespace ComposGH.Parameters.Tests
{
  [TestClass]
  public class ComposDesignCodeTest
  {
    [TestMethod]
    public void TestConstructor()
    {
      Code code = Code.EN1994_1_1_2004;
      NationalAnnex nationalAnnex = NationalAnnex.Generic;

      DesignCode designCode = new DesignCode(code, nationalAnnex);

      Assert.AreEqual(code, designCode.Design_Code);
      Assert.AreEqual(nationalAnnex, designCode.National_Annex);
    }
  }
}
