using Xunit;

using static ComposGH.Parameters.ComposDesignCode;

namespace ComposGH.Parameters.Tests
{
  public class ComposDesignCodeTest
  {
    [Fact]
    public ComposDesignCode TestConstructor()
    {
      Code code = Code.EN1994_1_1_2004;
      NationalAnnex nationalAnnex = NationalAnnex.Generic;

      ComposDesignCode designCode = new ComposDesignCode(code, nationalAnnex);

      Assert.Equal(code, designCode.Design_Code);
      Assert.Equal(nationalAnnex, designCode.National_Annex);

      return designCode;
    }

    [Fact]
    public void TestDuplicate()
    {
      Code code = Code.EN1994_1_1_2004;
      NationalAnnex nationalAnnex = NationalAnnex.Generic;

      ComposDesignCode designCode = new ComposDesignCode(code, nationalAnnex);

      ComposDesignCode duplicate = designCode.Duplicate();
      duplicate.Design_Code = Code.BS5950_3_1_1990_Superseeded;
      duplicate.National_Annex = NationalAnnex.United_Kingdom;

      Assert.Equal(code, designCode.Design_Code);
      Assert.Equal(nationalAnnex, designCode.National_Annex);
    }
  }
}
