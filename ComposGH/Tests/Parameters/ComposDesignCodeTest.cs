using Xunit;

using static ComposGH.Parameters.ComposDesignCode;

namespace ComposGH.Parameters.Tests
{
  public class ComposDesignCodeTest
  {
    [Fact]
    public ComposDesignCode TestConstructor()
    {
      // 1 setup input
      Code code = Code.EN1994_1_1_2004;
      NationalAnnex nationalAnnex = NationalAnnex.Generic;

      // 2 create object instance with constructor
      ComposDesignCode designCode = new ComposDesignCode(code, nationalAnnex);

      // 3 check that inputs are set in object's members
      Assert.Equal(code, designCode.Design_Code);
      Assert.Equal(nationalAnnex, designCode.National_Annex);

      // (optionally return object for other tests)
      return designCode;
    }

    [Fact]
    public void TestDuplicate()
    {
      Code code = Code.EN1994_1_1_2004;
      NationalAnnex nationalAnnex = NationalAnnex.Generic;

      // 1 create with constructor and duplicate
      ComposDesignCode original = new ComposDesignCode(code, nationalAnnex);
      ComposDesignCode duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(code, duplicate.Design_Code);
      Assert.Equal(nationalAnnex, duplicate.National_Annex);

      // 3 make some changes to duplicate
      duplicate.Design_Code = Code.BS5950_3_1_1990_Superseeded;
      duplicate.National_Annex = NationalAnnex.United_Kingdom;

      // 4 check that duplicate has set changes
      Assert.Equal(Code.BS5950_3_1_1990_Superseeded, duplicate.Design_Code);
      Assert.Equal(NationalAnnex.United_Kingdom, duplicate.National_Annex);

      // 5 check that original has not been changed
      Assert.Equal(code, original.Design_Code);
      Assert.Equal(nationalAnnex, original.National_Annex);
    }
  }
}
