using ComposAPI.Helpers;
using ComposAPI.Tests;
using System.Collections.Generic;
using Xunit;
using ComposGHTests.Helpers;
using OasysGH;

namespace ComposAPI.Members.Tests
{
  public partial class DesignCodeTest
  {
    [Fact]
    public DesignCode ConstructorTest()
    {
      // 1 setup input
      Code code = Code.BS5950_3_1_1990_A1_2010;

      // 2 create object instance with constructor
      DesignCode designCode = new DesignCode(code);

      // 3 check that inputs are set in object's members
      Assert.Equal(code, designCode.Code);
      // designoptions
      Assert.True(designCode.DesignOption.ProppedDuringConstruction);
      Assert.False(designCode.DesignOption.InclSteelBeamWeight);
      Assert.False(designCode.DesignOption.InclThinFlangeSections);
      Assert.False(designCode.DesignOption.ConsiderShearDeflection);
      // safety factors
      Assert.Null(designCode.SafetyFactors.MaterialFactors);
      Assert.Null(designCode.SafetyFactors.LoadFactors);

      return designCode;
    }
    [Fact]
    public void DuplicateDCTest()
    {
      // 1 create with constructor and duplicate
      DesignCode original = ConstructorTest();
      DesignCode duplicate = (DesignCode)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void BSToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "DESIGN_OPTION	MEMBER-2	BS5950-3.1:1990+A1:2010	PROPPED	BEAM_WEIGHT_NO	SLAB_WEIGHT_NO	SHEAR_DEFORM_NO	THIN_SECTION_NO	2.00000	2.00000\n";
      DesignCode dc = new DesignCode(Code.BS5950_3_1_1990_A1_2010);
      // Act
      string coaString = dc.ToCoaString("MEMBER-2");
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void BSssToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "DESIGN_OPTION	MEMBER-1	BS5950-3.1:1990 (superseded)	PROPPED	BEAM_WEIGHT_NO	SLAB_WEIGHT_NO	SHEAR_DEFORM_NO	THIN_SECTION_NO	2.00000	2.00000\n";
      DesignCode dc = new DesignCode(Code.BS5950_3_1_1990_Superseded);
      // Act
      string coaString = dc.ToCoaString("MEMBER-1");
      // Assert
      Assert.Equal(expected_coaString, coaString);
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
      Assert.True(designCode.DesignOption.ProppedDuringConstruction);
      Assert.False(designCode.DesignOption.InclSteelBeamWeight);
      Assert.False(designCode.DesignOption.InclThinFlangeSections);
      Assert.False(designCode.DesignOption.ConsiderShearDeflection);
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
    public void DuplicateDCASNZTest()
    {
      // 1 create with constructor and duplicate
      DesignCode original = TestASNZConstructor();
      DesignCode duplicate = (DesignCode)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void ASNZToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "DESIGN_OPTION	MEMBER-7	AS/NZS2327:2017	PROPPED	BEAM_WEIGHT_NO	SLAB_WEIGHT_NO	SHEAR_DEFORM_NO	THIN_SECTION_NO	2.00000	2.00000\n";
      DesignCode dc = TestASNZConstructor();
      // Act
      string coaString = dc.ToCoaString("MEMBER-7");
      // Assert
      Assert.Equal(expected_coaString, coaString);
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
      Assert.True(designCode.DesignOption.ProppedDuringConstruction);
      Assert.False(designCode.DesignOption.InclSteelBeamWeight);
      Assert.False(designCode.DesignOption.InclThinFlangeSections);
      Assert.False(designCode.DesignOption.ConsiderShearDeflection);
      // safety factors
      Assert.Null(designCode.SafetyFactors.MaterialFactors);
      //Assert.Null(designCode.SafetyFactors.LoadCombinationFactors);
      Assert.Equal(LoadCombination.Equation6_10, designCode.SafetyFactors.LoadCombinationFactors.LoadCombination);
      // code options
      CreepShrinkageParametersEN lt = (CreepShrinkageParametersEN)designCode.CodeOptions.LongTerm;
      CreepShrinkageParametersEN st = (CreepShrinkageParametersEN)designCode.CodeOptions.ShortTerm;
      Assert.False(designCode.CodeOptions.ApproxModularRatios);
      Assert.False(designCode.CodeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      Assert.Equal(CementClass.N, designCode.CodeOptions.CementType);
      Assert.Equal(1.1, lt.CreepCoefficient);
      Assert.Equal(28, lt.ConcreteAgeAtLoad);
      Assert.Equal(36500, lt.FinalConcreteAgeCreep);
      Assert.Equal(0.5, lt.RelativeHumidity.DecimalFractions);
      Assert.Equal(0.55, st.CreepCoefficient);
      Assert.Equal(1, st.ConcreteAgeAtLoad);
      Assert.Equal(36500, st.FinalConcreteAgeCreep);
      Assert.Equal(0.5, st.RelativeHumidity.DecimalFractions);

      // (optionally return object for other tests)
      return designCode;
    }
    [Fact]
    public void DuplicateDCEC4Test()
    {
      // 1 create with constructor and duplicate
      DesignCode original = TestEC4Constructor();
      DesignCode duplicate = (DesignCode)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void EC4ToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "DESIGN_OPTION	MEMBER-4	EN1994-1-1:2004	PROPPED	BEAM_WEIGHT_NO	SLAB_WEIGHT_NO	SHEAR_DEFORM_NO	THIN_SECTION_NO	2.00000	2.00000\nEC4_DESIGN_OPTION\tMEMBER-4\tSHRINKAGE_DEFORM_EC4_NO\tIGNORE_SHRINKAGE_DEFORM_NO\tAPPROXIMATE_E_RATIO_NO\tGeneric\tCLASS_N\t1.10000\t0.550000\t28.0000\t1.00000\t36500.0\t36500.0\t50.0000\t50.0000\nEC4_LOAD_COMB_FACTORS\tMEMBER-4\tEC0_6_10\t1.00000\t1.00000\t1.00000\t1.00000\n";
      DesignCode dc = TestEC4Constructor();
      // Act
      string coaString = dc.ToCoaString("MEMBER-4");
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void EC4FromCoaStringTest()
    {
      // Arrange
      string coaString = "DESIGN_OPTION	MEMBER-4	EN1994-1-1:2004	PROPPED	BEAM_WEIGHT_NO	SLAB_WEIGHT_NO	SHEAR_DEFORM_NO	THIN_SECTION_NO	2.00000	2.00000\nEC4_DESIGN_OPTION\tMEMBER-4\tSHRINKAGE_DEFORM_EC4_NO\tIGNORE_SHRINKAGE_DEFORM_NO\tAPPROXIMATE_E_RATIO_NO\tGeneric\tCLASS_N\t1.10000\t0.550000\t28.0000\t1.00000\t36500.0\t36500.0\t50.0000\t50.0000\nEC4_LOAD_COMB_FACTORS\tMEMBER-4\tEC0_6_10\t1.35000\t1.35000\t1.50000\t1.50000\n";

      IDesignCode expected_dc = TestEC4Constructor();
      // Act
      IDesignCode actual = EN1994.FromCoaString(coaString, "MEMBER-4", ComposUnits.GetStandardUnits());
      // Assert
      Duplicates.AreEqual(expected_dc, actual);
    }

    [Fact]
    public void HK05ToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "DESIGN_OPTION	MEMBER-5	HKSUOS:2005	PROPPED	BEAM_WEIGHT_NO	SLAB_WEIGHT_NO	SHEAR_DEFORM_NO	THIN_SECTION_NO	2.00000	2.00000\n";
      DesignCode dc = new DesignCode(Code.HKSUOS_2005);
      // Act
      string coaString = dc.ToCoaString("MEMBER-5");
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void HK11ToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "DESIGN_OPTION	MEMBER-6	HKSUOS:2011	PROPPED	BEAM_WEIGHT_NO	SLAB_WEIGHT_NO	SHEAR_DEFORM_NO	THIN_SECTION_NO	2.00000	2.00000\n";
      DesignCode dc = new DesignCode(Code.HKSUOS_2011);
      // Act
      string coaString = dc.ToCoaString("MEMBER-6");
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void DuplicateTest()
    {
      Code code = Code.HKSUOS_2005;

      // 1 create with constructor and duplicate
      DesignCode original = new DesignCode(code);
      DesignCode duplicate = (DesignCode)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(code, duplicate.Code);
      // designoptions
      Assert.True(duplicate.DesignOption.ProppedDuringConstruction);
      Assert.False(duplicate.DesignOption.InclSteelBeamWeight);
      Assert.False(duplicate.DesignOption.InclThinFlangeSections);
      Assert.False(duplicate.DesignOption.ConsiderShearDeflection);
      // safety factors
      Assert.Null(duplicate.SafetyFactors.MaterialFactors);
      Assert.Null(duplicate.SafetyFactors.LoadFactors);

      // 3 make some changes to duplicate
      duplicate.Code = Code.BS5950_3_1_1990_Superseded;

      // 4 check that duplicate has set changes
      Assert.Equal(Code.BS5950_3_1_1990_Superseded, duplicate.Code);
      Assert.False(object.ReferenceEquals(duplicate.DesignOption, original.DesignOption));
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
      Assert.True(duplicate.DesignOption.ProppedDuringConstruction);
      Assert.False(duplicate.DesignOption.InclSteelBeamWeight);
      Assert.False(duplicate.DesignOption.InclThinFlangeSections);
      Assert.False(duplicate.DesignOption.ConsiderShearDeflection);
      // safety factors
      Assert.Null(duplicate.SafetyFactors.MaterialFactors);
      Assert.Null(duplicate.SafetyFactors.LoadFactors);
      // code options
      Assert.False(duplicate.CodeOptions.ConsiderShrinkageDeflection);
      Assert.Equal(2.0, duplicate.CodeOptions.LongTerm.CreepCoefficient);
      Assert.Equal(2.0, duplicate.CodeOptions.ShortTerm.CreepCoefficient);

      // 3 check that duplicate has differnt memory address than original
      Assert.False(object.ReferenceEquals(duplicate.DesignOption, original.DesignOption));
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
      Assert.True(duplicate.DesignOption.ProppedDuringConstruction);
      Assert.False(duplicate.DesignOption.InclSteelBeamWeight);
      Assert.False(duplicate.DesignOption.InclThinFlangeSections);
      Assert.False(duplicate.DesignOption.ConsiderShearDeflection);
      // safety factors
      Assert.Null(duplicate.SafetyFactors.MaterialFactors);
      //Assert.Null(duplicate.SafetyFactors.LoadCombinationFactors);
      Assert.Equal(LoadCombination.Equation6_10, duplicate.SafetyFactors.LoadCombinationFactors.LoadCombination);
      // code options
      CreepShrinkageParametersEN lt = (CreepShrinkageParametersEN)duplicate.CodeOptions.LongTerm;
      CreepShrinkageParametersEN st = (CreepShrinkageParametersEN)duplicate.CodeOptions.ShortTerm;
      Assert.False(duplicate.CodeOptions.ApproxModularRatios);
      Assert.False(duplicate.CodeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      Assert.Equal(CementClass.N, duplicate.CodeOptions.CementType);
      Assert.Equal(1.1, lt.CreepCoefficient);
      Assert.Equal(28, lt.ConcreteAgeAtLoad);
      Assert.Equal(36500, lt.FinalConcreteAgeCreep);
      Assert.Equal(0.5, lt.RelativeHumidity.DecimalFractions);
      Assert.Equal(0.55, st.CreepCoefficient);
      Assert.Equal(1, st.ConcreteAgeAtLoad);
      Assert.Equal(36500, st.FinalConcreteAgeCreep);
      Assert.Equal(0.5, st.RelativeHumidity.DecimalFractions);

      // 3 make some changes to duplicate
      duplicate.NationalAnnex = NationalAnnex.United_Kingdom;

      // 4 check that duplicate has set changes
      Assert.Equal(NationalAnnex.United_Kingdom, duplicate.NationalAnnex);

      // 5 check that duplicate has differnt memory address than original
      Assert.False(object.ReferenceEquals(duplicate.DesignOption, original.DesignOption));
      Assert.False(object.ReferenceEquals(duplicate.SafetyFactors, original.SafetyFactors));
      Assert.False(object.ReferenceEquals(duplicate.CodeOptions, original.CodeOptions));
    }

    [Theory]
    [InlineData("BS5950_3_1_1990_Superseded", false, false, false, false, false, "MEMBER_TITLE	MEMBER-1		B/tf=15    Change in direction > 11 degrees\nDESIGN_OPTION	MEMBER-1	BS5950-3.1:1990 (superseded)	UNPROPPED	BEAM_WEIGHT_NO	SLAB_WEIGHT_NO	SHEAR_DEFORM_NO	THIN_SECTION_NO	2.00000	2.00000\n")]
    [InlineData("BS5950_3_1_1990_A1_2010", true, false, false, false, false, "MEMBER_TITLE	MEMBER-2		B/tf=17.5    Change in direction < 10 degrees\nDESIGN_OPTION	MEMBER-2	BS5950-3.1:1990+A1:2010	PROPPED	BEAM_WEIGHT_NO	SLAB_WEIGHT_NO	SHEAR_DEFORM_NO	THIN_SECTION_NO	2.00000	2.00000\n")]
    [InlineData("EN1994_1_1_2004", true, true, false, false, false, "MEMBER_TITLE	MEMBER-3		B/tf=17.5    Change in direction > 10 degrees\nDESIGN_OPTION	MEMBER-3	EN1994-1-1:2004	PROPPED	BEAM_WEIGHT_YES	SLAB_WEIGHT_NO	SHEAR_DEFORM_NO	THIN_SECTION_NO	2.00000	2.00000\n")]
    [InlineData("EN1994_1_1_2004", false, false, true, true, false, "MEMBER_TITLE	MEMBER-4		B/tf=20    Change in direction < 9 degrees\nDESIGN_OPTION	MEMBER-4	EN1994-1-1:2004	UNPROPPED	BEAM_WEIGHT_NO	SLAB_WEIGHT_YES	SHEAR_DEFORM_YES	THIN_SECTION_NO	2.00000	2.00000\n")]
    [InlineData("HKSUOS_2005", false, false, false, false, true, "MEMBER_TITLE	MEMBER-5		B/tf=20    Change in direction > 9 degrees\nDESIGN_OPTION	MEMBER-5	HKSUOS:2005	UNPROPPED	BEAM_WEIGHT_NO	SLAB_WEIGHT_NO	SHEAR_DEFORM_NO	THIN_SECTION_YES	2.00000	2.00000\n")]
    [InlineData("HKSUOS_2011", true, true, true, true, true, "MEMBER_TITLE	MEMBER-6		B/tf=22.5    Change in direction < 8 degrees\nDESIGN_OPTION	MEMBER-6	HKSUOS:2011	PROPPED	BEAM_WEIGHT_YES	SLAB_WEIGHT_YES	SHEAR_DEFORM_YES	THIN_SECTION_YES	2.00000	2.00000\n")]
    [InlineData("AS_NZS2327_2017", false, false, false, false, false, "MEMBER_TITLE	MEMBER-7		B/tf=22.5    Change in direction > 8 degrees\nDESIGN_OPTION	MEMBER-7	AS/NZS2327:2017	UNPROPPED	BEAM_WEIGHT_NO	SLAB_WEIGHT_NO	SHEAR_DEFORM_NO	THIN_SECTION_NO	1.00000	3.00000\n", 1, 3)]
    public void TestFileCoaStringForDesignCode(string expected_Code, bool expected_ProppedDuringConstruction, bool expected_InclSteelBeamWeight, bool expected_InclConcreteSlabWeight, bool expected_ConsiderShearDeflection, bool expected_InclThinFlangeSections, string coaString, double expected_LongTermCreep = 0, double expected_ShortTermCreep = 0)
    {
      // Arrange 
      ComposUnits units = ComposUnits.GetStandardUnits();
      List<string> parameters = CoaHelper.Split(coaString);

      // Act
      IDesignCode desingCode = DesignCode.FromCoaString(coaString, parameters[1], units);

      // Assert
      Assert.Equal(expected_Code, desingCode.Code.ToString());
      Assert.Equal(expected_ProppedDuringConstruction, desingCode.DesignOption.ProppedDuringConstruction);
      Assert.Equal(expected_InclSteelBeamWeight, desingCode.DesignOption.InclSteelBeamWeight);
      Assert.Equal(expected_InclConcreteSlabWeight, desingCode.DesignOption.InclConcreteSlabWeight);
      Assert.Equal(expected_ConsiderShearDeflection, desingCode.DesignOption.ConsiderShearDeflection);
      Assert.Equal(expected_InclThinFlangeSections, desingCode.DesignOption.InclThinFlangeSections);

      if(desingCode is ASNZS2327 aSNZS)
      {
        Assert.Equal(expected_LongTermCreep, aSNZS.CodeOptions.LongTerm.CreepCoefficient);
        Assert.Equal(expected_ShortTermCreep, aSNZS.CodeOptions.ShortTerm.CreepCoefficient);
      }
    }
  }
}
