using Xunit;

using static ComposGH.Parameters.DesignCode;

namespace ComposGH.Parameters.Tests
{
  public class ComposDesignCodeTest
  {
    [Fact]
    public DesignCode TestConstructor()
    {
      Code code = Code.EN1994_1_1_2004;
      NationalAnnex nationalAnnex = NationalAnnex.Generic;

      DesignCode designCode = new DesignCode(code, nationalAnnex);

      Assert.Equal(code, designCode.Design_Code);
      Assert.Equal(nationalAnnex, designCode.National_Annex);

      return designCode;
    }

    [Fact]
    public void TestDuplicate()
    {
      Code code = Code.EN1994_1_1_2004;
      NationalAnnex nationalAnnex = NationalAnnex.Generic;

      DesignCode designCode = new DesignCode(code, nationalAnnex);

      DesignCode duplicate = designCode.Duplicate();
      duplicate.Design_Code = Code.BS5950_3_1_1990_Superseeded;
      duplicate.National_Annex = NationalAnnex.United_Kingdom;

      Assert.Equal(code, designCode.Design_Code);
      Assert.Equal(nationalAnnex, designCode.National_Annex);
    }
  }
}
