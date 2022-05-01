using Xunit;
using ComposAPI.Member;
using static ComposAPI.Member.DesignCode;

namespace ComposAPI.Tests
{
  public partial class DesignCodeTest
  {
    [Fact]
    public DesignCode TestConstructor()
    {
      // 1 setup input
      Code code = Code.BS5950_3_1_1990_A1_2010;

      // 2 create object instance with constructor
      DesignCode designCode = new DesignCode(code);

      // 3 check that inputs are set in object's members
      Assert.Equal(code, designCode.Code);
      // designoptions
      Assert.True(designCode.DesignOptions.ProppedDuringConstruction);
      Assert.False(designCode.DesignOptions.InclSteelBeamWeight);
      Assert.False(designCode.DesignOptions.InclThinFlangeSections);
      Assert.False(designCode.DesignOptions.ConsiderShearDeflection);
      // safety factors
      Assert.Null(designCode.SafetyFactors.MaterialFactors);
      Assert.Null(designCode.SafetyFactors.LoadFactors);

      // (optionally return object for other tests)
      return designCode;
    }

    [Fact]
    public DesignCode TestASNZConstructor()
    {
      // 1 setup input
      // empty constructor creates default AS/NZ code

      // 2 create object instance with constructor
      ASNZS2327 designCode = new ASNZS2327();

      // 3 check that inputs are set in object's members
      Assert.Equal(Code.AS_NZS2327_2017, designCode.Code);
      // designoptions
      Assert.True(designCode.DesignOptions.ProppedDuringConstruction);
      Assert.False(designCode.DesignOptions.InclSteelBeamWeight);
      Assert.False(designCode.DesignOptions.InclThinFlangeSections);
      Assert.False(designCode.DesignOptions.ConsiderShearDeflection);
      // safety factors
      Assert.Null(designCode.SafetyFactors.MaterialFactors);
      Assert.Null(designCode.SafetyFactors.LoadFactors);
      // code options
      Assert.False(designCode.CodeOptions.ConsiderShrinkageDeflection);
      Assert.Equal(2.0, designCode.CodeOptions.LongTerm.CreepCoefficient);
      Assert.Equal(2.0, designCode.CodeOptions.ShortTerm.CreepCoefficient);

      // (optionally return object for other tests)
      return designCode;
    }

    [Fact]
    public DesignCode TestEC4Constructor()
    {
      // 1 setup input
      // empty constructor creates default EN1994-1-1 code

      // 2 create object instance with constructor
      EN1994 designCode = new EN1994();

      // 3 check that inputs are set in object's members
      Assert.Equal(Code.EN1994_1_1_2004, designCode.Code);
      Assert.Equal(NationalAnnex.Generic, designCode.NationalAnnex);
      // designoptions
      Assert.True(designCode.DesignOptions.ProppedDuringConstruction);
      Assert.False(designCode.DesignOptions.InclSteelBeamWeight);
      Assert.False(designCode.DesignOptions.InclThinFlangeSections);
      Assert.False(designCode.DesignOptions.ConsiderShearDeflection);
      // safety factors
      Assert.Null(designCode.SafetyFactors.MaterialFactors);
      Assert.Null(designCode.SafetyFactors.LoadFactors);
      Assert.Equal(LoadCombination.Equation6_10, designCode.SafetyFactors.LoadCombination);
      // code options
      Assert.False(designCode.CodeOptions.ApproxModularRatios);
      Assert.False(designCode.CodeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      Assert.Equal(EC4Options.CementClass.N, designCode.CodeOptions.CementType);
      Assert.Equal(1.1, designCode.CodeOptions.LongTerm.CreepCoefficient);
      Assert.Equal(28, designCode.CodeOptions.LongTerm.ConcreteAgeAtLoad);
      Assert.Equal(36500, designCode.CodeOptions.LongTerm.FinalConcreteAgeCreep);
      Assert.Equal(0.5, designCode.CodeOptions.LongTerm.RelativeHumidity);
      Assert.Equal(0.55, designCode.CodeOptions.ShortTerm.CreepCoefficient);
      Assert.Equal(1, designCode.CodeOptions.ShortTerm.ConcreteAgeAtLoad);
      Assert.Equal(36500, designCode.CodeOptions.ShortTerm.FinalConcreteAgeCreep);
      Assert.Equal(0.5, designCode.CodeOptions.ShortTerm.RelativeHumidity);

      // (optionally return object for other tests)
      return designCode;
    }

    [Fact]
    public void TestDuplicate()
    {
      Code code = Code.HKSUOS_2005;

      // 1 create with constructor and duplicate
      DesignCode original = new DesignCode(code);
      DesignCode duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(code, duplicate.Code);
      // designoptions
      Assert.True(duplicate.DesignOptions.ProppedDuringConstruction);
      Assert.False(duplicate.DesignOptions.InclSteelBeamWeight);
      Assert.False(duplicate.DesignOptions.InclThinFlangeSections);
      Assert.False(duplicate.DesignOptions.ConsiderShearDeflection);
      // safety factors
      Assert.Null(duplicate.SafetyFactors.MaterialFactors);
      Assert.Null(duplicate.SafetyFactors.LoadFactors);

      // 3 make some changes to duplicate
      duplicate.Code = Code.BS5950_3_1_1990_Superseeded;

      // 4 check that duplicate has set changes
      Assert.Equal(Code.BS5950_3_1_1990_Superseeded, duplicate.Code);
      Assert.False(object.ReferenceEquals(duplicate.DesignOptions, original.DesignOptions));
      Assert.False(object.ReferenceEquals(duplicate.SafetyFactors, original.SafetyFactors));

      // 5 check that original has not been changed
      Assert.Equal(code, original.Code);
    }

    [Fact]
    public void TestASNZDuplicate()
    {
      // 1 create with constructor and duplicate
      ASNZS2327 original = new ASNZS2327();
      ASNZS2327 duplicate = (ASNZS2327)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(Code.AS_NZS2327_2017, duplicate.Code);
      // designoptions
      Assert.True(duplicate.DesignOptions.ProppedDuringConstruction);
      Assert.False(duplicate.DesignOptions.InclSteelBeamWeight);
      Assert.False(duplicate.DesignOptions.InclThinFlangeSections);
      Assert.False(duplicate.DesignOptions.ConsiderShearDeflection);
      // safety factors
      Assert.Null(duplicate.SafetyFactors.MaterialFactors);
      Assert.Null(duplicate.SafetyFactors.LoadFactors);
      // code options
      Assert.False(duplicate.CodeOptions.ConsiderShrinkageDeflection);
      Assert.Equal(2.0, duplicate.CodeOptions.LongTerm.CreepCoefficient);
      Assert.Equal(2.0, duplicate.CodeOptions.ShortTerm.CreepCoefficient);

      // 3 check that duplicate has differnt memory address than original
      Assert.False(object.ReferenceEquals(duplicate.DesignOptions, original.DesignOptions));
      Assert.False(object.ReferenceEquals(duplicate.SafetyFactors, original.SafetyFactors));
      Assert.False(object.ReferenceEquals(duplicate.CodeOptions, original.CodeOptions));
    }

    [Fact]
    public void TestEC4Duplicate()
    {
      // 1 create with constructor and duplicate
      EN1994 original = new EN1994();
      EN1994 duplicate = (EN1994)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(Code.EN1994_1_1_2004, duplicate.Code);
      Assert.Equal(NationalAnnex.Generic, duplicate.NationalAnnex);
      // designoptions
      Assert.True(duplicate.DesignOptions.ProppedDuringConstruction);
      Assert.False(duplicate.DesignOptions.InclSteelBeamWeight);
      Assert.False(duplicate.DesignOptions.InclThinFlangeSections);
      Assert.False(duplicate.DesignOptions.ConsiderShearDeflection);
      // safety factors
      Assert.Null(duplicate.SafetyFactors.MaterialFactors);
      Assert.Null(duplicate.SafetyFactors.LoadFactors);
      Assert.Equal(LoadCombination.Equation6_10, duplicate.SafetyFactors.LoadCombination);
      // code options
      Assert.False(duplicate.CodeOptions.ApproxModularRatios);
      Assert.False(duplicate.CodeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      Assert.Equal(EC4Options.CementClass.N, duplicate.CodeOptions.CementType);
      Assert.Equal(1.1, duplicate.CodeOptions.LongTerm.CreepCoefficient);
      Assert.Equal(28, duplicate.CodeOptions.LongTerm.ConcreteAgeAtLoad);
      Assert.Equal(36500, duplicate.CodeOptions.LongTerm.FinalConcreteAgeCreep);
      Assert.Equal(0.5, duplicate.CodeOptions.LongTerm.RelativeHumidity);
      Assert.Equal(0.55, duplicate.CodeOptions.ShortTerm.CreepCoefficient);
      Assert.Equal(1, duplicate.CodeOptions.ShortTerm.ConcreteAgeAtLoad);
      Assert.Equal(36500, duplicate.CodeOptions.ShortTerm.FinalConcreteAgeCreep);
      Assert.Equal(0.5, duplicate.CodeOptions.ShortTerm.RelativeHumidity);

      // 3 make some changes to duplicate
      duplicate.NationalAnnex = NationalAnnex.United_Kingdom;

      // 4 check that duplicate has set changes
      Assert.Equal(NationalAnnex.United_Kingdom, duplicate.NationalAnnex);

      // 5 check that duplicate has differnt memory address than original
      Assert.False(object.ReferenceEquals(duplicate.DesignOptions, original.DesignOptions));
      Assert.False(object.ReferenceEquals(duplicate.SafetyFactors, original.SafetyFactors));
      Assert.False(object.ReferenceEquals(duplicate.CodeOptions, original.CodeOptions));
    }
  }
}
