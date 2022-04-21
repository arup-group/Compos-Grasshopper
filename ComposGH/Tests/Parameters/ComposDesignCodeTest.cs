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

      // create with constructor and duplicate
      ComposDesignCode original = new ComposDesignCode(code, nationalAnnex);
      ComposDesignCode duplicate = original.Duplicate();
      
      // check that duplicate has duplicated values
      Assert.Equal(code, duplicate.Design_Code);
      Assert.Equal(nationalAnnex, duplicate.National_Annex);

      // make some changes to duplicate
      duplicate.Design_Code = Code.BS5950_3_1_1990_Superseeded;
      duplicate.National_Annex = NationalAnnex.United_Kingdom;

      // check that duplicate has set changes
      Assert.Equal(Code.BS5950_3_1_1990_Superseeded, duplicate.Design_Code);
      Assert.Equal(NationalAnnex.United_Kingdom, duplicate.National_Annex);

      // check that original keeps original values
      Assert.Equal(code, original.Design_Code);
      Assert.Equal(nationalAnnex, original.National_Annex);
    }
  }
}
