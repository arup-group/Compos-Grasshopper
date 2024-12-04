using System.Collections.Generic;
using ComposAPI.Helpers;
using ComposGH.Helpers;
using ComposGHTests.Helper;
using OasysGH;
using OasysUnits;
using OasysUnits.Units;
using Xunit;
using static ComposAPI.ConcreteMaterial;

namespace ComposAPI.Slabs.Tests {
  public static class ConcreteMaterialMother {

    public static IConcreteMaterial CreateConcreteMaterial() {
      var dryDensity = new Density(2400, DensityUnit.KilogramPerCubicMeter);
      IERatio eRatio = new ERatio();
      return new ConcreteMaterial(ConcreteGrade.C30, WeightType.Normal, dryDensity, false, eRatio, new Ratio(0.33, RatioUnit.DecimalFraction));
    }
  }

  [Collection("ComposAPI Fixture collection")]
  public class ConcreteMaterialTest {

    // 1 setup inputs
    [Theory]
    // AS/NZ
    [InlineData(ConcreteGrade.C25, WeightType.Normal, DensityClass.NOT_APPLY, 2400, false, 0.33, -0.00085, false)]
    [InlineData(ConcreteGrade.C25, WeightType.Normal, DensityClass.NOT_APPLY, 2200, true, 0.33, -0.00085, false)]
    [InlineData(ConcreteGrade.C25, WeightType.Normal, DensityClass.NOT_APPLY, 2400, false, 0.33, -0.0003, true)]
    public void ASNZConstructorTest(ConcreteGrade grade, WeightType type, DensityClass densityClass, double dryDensityValue, bool userDensity, double imposedLoadPercentage, double shrinkageStrainValue, bool userStrain) {
      // 2 create object instance with constructor
      var dryDensity = new Density(dryDensityValue, DensityUnit.KilogramPerCubicCentimeter);
      var eRatio = new ERatio();
      var shrinkageStrain = new Strain(shrinkageStrainValue, StrainUnit.MilliStrain);
      var concreteMaterial = new ConcreteMaterial(grade, dryDensity, userDensity, eRatio, new Ratio(imposedLoadPercentage, RatioUnit.DecimalFraction), shrinkageStrain, userStrain);

      // 3 check that inputs are set in object's members
      Assert.Equal(grade.ToString(), concreteMaterial.Grade);
      Assert.Equal(type, concreteMaterial.Type);
      Assert.Equal(densityClass, concreteMaterial.Class);
      Assert.Equal(dryDensity, concreteMaterial.DryDensity);
      Assert.Equal(userDensity, concreteMaterial.UserDensity);
      Assert.Equal(eRatio, concreteMaterial.ERatio);
      Assert.Equal(imposedLoadPercentage, concreteMaterial.ImposedLoadPercentage.DecimalFractions);
      Assert.Equal(shrinkageStrain, concreteMaterial.ShrinkageStrain);
      Assert.Equal(userStrain, concreteMaterial.UserStrain);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(ConcreteGrade.C30, WeightType.Normal, DensityClass.NOT_APPLY, 2400, false, 33, -0.000325, false)]
    [InlineData(ConcreteGrade.C30, WeightType.Normal, DensityClass.NOT_APPLY, 2300, true, 33, -0.000325, false)]
    [InlineData(ConcreteGrade.C30, WeightType.LightWeight, DensityClass.NOT_APPLY, 1800, false, 33, -0.000325, false)]
    [InlineData(ConcreteGrade.C30, WeightType.LightWeight, DensityClass.NOT_APPLY, 1900, true, 33, -0.000325, false)]
    public void BritishConstructorTest(ConcreteGrade grade, WeightType type, DensityClass densityClass, double dryDensityValue, bool userDensity, double imposedLoadPercentage, double shrinkageStrainValue, bool userStrain) {
      // 2 create object instance with constructor
      var dryDensity = new Density(dryDensityValue, DensityUnit.KilogramPerCubicCentimeter);
      var eRatio = new ERatio();
      var shrinkageStrain = new Strain(shrinkageStrainValue, StrainUnit.Ratio);
      var concreteMaterial = new ConcreteMaterial(grade, type, dryDensity, userDensity, eRatio, new Ratio(imposedLoadPercentage, RatioUnit.Percent));

      // 3 check that inputs are set in object's members
      Assert.Equal(grade.ToString(), concreteMaterial.Grade);
      Assert.Equal(type, concreteMaterial.Type);
      Assert.Equal(densityClass, concreteMaterial.Class);
      Assert.Equal(dryDensity, concreteMaterial.DryDensity);
      Assert.Equal(userDensity, concreteMaterial.UserDensity);
      Assert.Equal(eRatio, concreteMaterial.ERatio);
      Assert.Equal(imposedLoadPercentage, concreteMaterial.ImposedLoadPercentage.Percent);
      Assert.Equal(shrinkageStrain.Ratio, concreteMaterial.ShrinkageStrain.Ratio, 6);
      Assert.Equal(userStrain, concreteMaterial.UserStrain);
    }

    [Fact]
    public void DuplicateStdTest() {
      // 1 create with constructor and duplicate
      var original = (ConcreteMaterial)ConcreteMaterialMother.CreateConcreteMaterial();
      var duplicate = (ConcreteMaterial)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Theory]
    [InlineData(ConcreteGradeEN.C35_45, WeightType.Normal, DensityClass.NOT_APPLY, 2400, false, 0.33, -0.000325, false)]
    public void DuplicateTest(ConcreteGradeEN grade, WeightType type, DensityClass densityClass, double dryDensityValue, bool userDensity, double imposedLoadPercentage, double shrinkageStrainValue, bool userStrain) {
      // 1 create with constructor and duplicate
      var dryDensity = new Density(dryDensityValue, DensityUnit.KilogramPerCubicCentimeter);
      var eRatio = new ERatio(9.87, 28.72, 9.55, 27.55);
      var shrinkageStrain = new Strain(shrinkageStrainValue, StrainUnit.MilliStrain);
      var original = new ConcreteMaterial(grade, densityClass, dryDensity, userDensity, eRatio, new Ratio(imposedLoadPercentage, RatioUnit.DecimalFraction), shrinkageStrain, userStrain);
      var duplicate = (ConcreteMaterial)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(grade.ToString(), duplicate.Grade);
      Assert.Equal(type, duplicate.Type);
      Assert.Equal(densityClass, duplicate.Class);
      Assert.Equal(dryDensity, duplicate.DryDensity);
      Assert.Equal(userDensity, duplicate.UserDensity);
      Assert.Equal(9.87, duplicate.ERatio.ShortTerm);
      Assert.Equal(28.72, duplicate.ERatio.LongTerm);
      Assert.Equal(9.55, duplicate.ERatio.Vibration);
      Assert.Equal(27.55, duplicate.ERatio.Shrinkage);
      Assert.Equal(imposedLoadPercentage, duplicate.ImposedLoadPercentage.DecimalFractions);
      Assert.Equal(shrinkageStrain, duplicate.ShrinkageStrain);
      Assert.Equal(userStrain, duplicate.UserStrain);

      // 3 make some changes to duplicate
      duplicate.Grade = ConcreteGradeEN.LC30_33.ToString();
      duplicate.Type = WeightType.LightWeight;
      duplicate.Class = DensityClass.DC1601_1800;
      var duplicateDensity = new Density(1780, DensityUnit.KilogramPerCubicMeter);
      duplicate.DryDensity = duplicateDensity;
      duplicate.UserDensity = true;

      var duplicateERatio = (ERatio)duplicate.ERatio;

      duplicateERatio.ShortTerm = 1;
      duplicateERatio.LongTerm = 2;
      duplicateERatio.Vibration = 3;
      duplicateERatio.Shrinkage = 4;

      duplicate.ImposedLoadPercentage = new Ratio(0.5, RatioUnit.DecimalFraction);
      var duplicateStrain = new Strain(-0.0004, StrainUnit.MilliStrain);
      duplicate.ShrinkageStrain = duplicateStrain;
      duplicate.UserStrain = true;

      // 4 check that duplicate has set changes
      Assert.Equal(ConcreteGradeEN.LC30_33.ToString(), duplicate.Grade);
      Assert.Equal(WeightType.LightWeight, duplicate.Type);
      Assert.Equal(DensityClass.DC1601_1800, duplicate.Class);
      Assert.Equal(duplicateDensity, duplicate.DryDensity);
      Assert.True(duplicate.UserDensity);
      Assert.Equal(1, duplicate.ERatio.ShortTerm);
      Assert.Equal(2, duplicate.ERatio.LongTerm);
      Assert.Equal(3, duplicate.ERatio.Vibration);
      Assert.Equal(4, duplicate.ERatio.Shrinkage);
      Assert.Equal(0.5, duplicate.ImposedLoadPercentage.DecimalFractions);
      Assert.Equal(duplicateStrain, duplicate.ShrinkageStrain);
      Assert.True(duplicate.UserStrain);

      // 5 check that original has not been changed
      Assert.Equal(grade.ToString(), original.Grade);
      Assert.Equal(type, original.Type);
      Assert.Equal(densityClass, original.Class);
      Assert.Equal(dryDensity, original.DryDensity);
      Assert.Equal(userDensity, original.UserDensity);
      Assert.Equal(9.87, original.ERatio.ShortTerm);
      Assert.Equal(28.72, original.ERatio.LongTerm);
      Assert.Equal(9.55, original.ERatio.Vibration);
      Assert.Equal(27.55, original.ERatio.Shrinkage);
      Assert.Equal(imposedLoadPercentage, original.ImposedLoadPercentage.DecimalFractions);
      Assert.Equal(shrinkageStrain, original.ShrinkageStrain);
      Assert.Equal(userStrain, original.UserStrain);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(ConcreteGradeEN.C35_45, WeightType.Normal, DensityClass.NOT_APPLY, 2400, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.C35_45, WeightType.Normal, DensityClass.NOT_APPLY, 2200, true, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.C35_45, WeightType.Normal, DensityClass.NOT_APPLY, 2400, false, 0.33, -0.0003, true)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.LightWeight, DensityClass.DC801_1000, 1000, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.LightWeight, DensityClass.DC1001_1200, 1200, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.LightWeight, DensityClass.DC1201_1400, 1400, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.LightWeight, DensityClass.DC1401_1600, 1600, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.LightWeight, DensityClass.DC1601_1800, 1800, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.LightWeight, DensityClass.DC1801_2000, 2000, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.LightWeight, DensityClass.DC1801_2000, 1800, true, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.LightWeight, DensityClass.DC1801_2000, 1800, true, 0.33, -0.0003, true)]
    public void EuropeanConstructorTest(ConcreteGradeEN grade, WeightType type, DensityClass densityClass, double dryDensityValue, bool userDensity, double imposedLoadPercentage, double shrinkageStrainValue, bool userStrain) {
      // 2 create object instance with constructor
      var dryDensity = new Density(dryDensityValue, DensityUnit.KilogramPerCubicCentimeter);
      var eRatio = new ERatio();
      var shrinkageStrain = new Strain(shrinkageStrainValue, StrainUnit.MilliStrain);
      var concreteMaterial = new ConcreteMaterial(grade, densityClass, dryDensity, userDensity, eRatio, new Ratio(imposedLoadPercentage, RatioUnit.DecimalFraction), shrinkageStrain, userStrain);

      // 3 check that inputs are set in object's members
      Assert.Equal(grade.ToString(), concreteMaterial.Grade);
      Assert.Equal(type, concreteMaterial.Type);
      Assert.Equal(densityClass, concreteMaterial.Class);
      Assert.Equal(dryDensity, concreteMaterial.DryDensity);
      Assert.Equal(userDensity, concreteMaterial.UserDensity);
      Assert.Equal(eRatio, concreteMaterial.ERatio);
      Assert.Equal(imposedLoadPercentage, concreteMaterial.ImposedLoadPercentage.DecimalFractions);
      Assert.Equal(shrinkageStrain, concreteMaterial.ShrinkageStrain);
      Assert.Equal(userStrain, concreteMaterial.UserStrain);
    }

    [Theory]
    [InlineData("SLAB_CONCRETE_MATERIAL	MEMBER-1	C35/45	NORMAL	CODE_DENSITY	2400.00	NOT_APPLY	0.330000	CODE_E_RATIO	CODE_STRAIN\n", "C35_45", DensityClass.NOT_APPLY, 2400, false, 0, 0, 0, 0, false, 33, -0.000325, false)] // EN Normal
    [InlineData("SLAB_CONCRETE_MATERIAL	MEMBER-1	C35/45	NORMAL	CODE_DENSITY	2400.00	NOT_APPLY	0.330000	CODE_E_RATIO	USER_STRAIN	-0.000300000\n", "C35_45", DensityClass.NOT_APPLY, 2400, false, 0, 0, 0, 0, false, 33, -0.0003, true)] // EN Normal User Strain
    [InlineData("SLAB_CONCRETE_MATERIAL	MEMBER-1	LC35/38	LIGHT	CODE_DENSITY	2000.00	1801_2000	0.330000	CODE_E_RATIO	CODE_STRAIN\n", "LC35_38", DensityClass.DC1801_2000, 2000, false, 0, 0, 0, 0, false, 33, -0.0005, false)] // EN Light
    [InlineData("SLAB_CONCRETE_MATERIAL	MEMBER-1	LC35/38	LIGHT	USER_DENSITY	1000.00	0.330000	CODE_E_RATIO	CODE_STRAIN\n", "LC35_38", DensityClass.NOT_APPLY, 1000, true, 0, 0, 0, 0, false, 33, -0.0005, false)] // EN Light User Density
    public void FromCoaStringTest(string coaString, string expected_grade, DensityClass expected_densityClass, double expected_dryDensity, bool expected_userDensity, double expected_shortTerm, double expected_longTerm, double expected_vibration, double expected_shrinkage, bool expected_userDefined, double expected_imposedLoadPercentage, double expected_shrinkageStrain, bool expected_userStrain) {
      var units = ComposUnits.GetStandardUnits();

      List<string> parameters = CoaHelper.Split(coaString);
      IConcreteMaterial material = ConcreteMaterial.FromCoaString(parameters, units);

      Assert.Equal(expected_grade, material.Grade);
      Assert.Equal(expected_densityClass, material.Class);
      Assert.Equal(expected_dryDensity, material.DryDensity.Value);
      Assert.Equal(expected_userDensity, material.UserDensity);
      Assert.Equal(expected_userDefined, material.ERatio.UserDefined);
      if (!expected_userDefined) {
        Assert.Equal(expected_shortTerm, material.ERatio.ShortTerm);
        Assert.Equal(expected_longTerm, material.ERatio.LongTerm);
        Assert.Equal(expected_vibration, material.ERatio.Vibration);
        Assert.Equal(expected_shrinkage, material.ERatio.Shrinkage);
      }
      Assert.Equal(expected_imposedLoadPercentage, material.ImposedLoadPercentage.Percent);
      Assert.Equal(expected_userStrain, material.UserStrain);
      if (expected_userStrain) {
        Assert.Equal(expected_shrinkageStrain, material.ShrinkageStrain.Value);
      }
    }

    [Theory]
    [InlineData("SLAB_CONCRETE_MATERIAL	MEMBER-1	C35/45	NORMAL	CODE_DENSITY	2400.00	NOT_APPLY	0.330000	CODE_E_RATIO	CODE_STRAIN\n", "C35_45", DensityClass.NOT_APPLY, 2400, false, 0, 0, 0, 0, false, 33, -0.000325, false)] // EN Normal
    [InlineData("SLAB_CONCRETE_MATERIAL	MEMBER-1	C35/45	NORMAL	CODE_DENSITY	2400.00	NOT_APPLY	0.330000	CODE_E_RATIO	USER_STRAIN	-0.000300000\n", "C35_45", DensityClass.NOT_APPLY, 2400, false, 0, 0, 0, 0, false, 33, -0.0003, true)] // EN Normal User Strain
    [InlineData("SLAB_CONCRETE_MATERIAL	MEMBER-1	LC35/38	LIGHT	CODE_DENSITY	2000.00	1801_2000	0.330000	CODE_E_RATIO	CODE_STRAIN\n", "LC35_38", DensityClass.DC1801_2000, 2000, false, 0, 0, 0, 0, false, 33, -0.0005, false)] // EN Light
    [InlineData("SLAB_CONCRETE_MATERIAL	MEMBER-1	LC35/38	LIGHT	USER_DENSITY	1000.00	0.330000	CODE_E_RATIO	CODE_STRAIN\n", "LC35_38", DensityClass.NOT_APPLY, 1000, true, 0, 0, 0, 0, false, 33, -0.0005, false)] // EN Light User Density
    public void FromCoaStringTestEN(string coaString, string expected_grade, DensityClass expected_densityClass, double expected_dryDensity, bool expected_userDensity, double expected_shortTerm, double expected_longTerm, double expected_vibration, double expected_shrinkage, bool expected_userDefined, double expected_imposedLoadPercentage, double expected_shrinkageStrain, bool expected_userStrain) {
      var units = ComposUnits.GetStandardUnits();

      List<string> parameters = CoaHelper.Split(coaString);
      IConcreteMaterial material = ConcreteMaterial.FromCoaString(parameters, units);

      Assert.Equal(expected_grade, material.Grade);
      Assert.Equal(expected_densityClass, material.Class);
      Assert.Equal(expected_dryDensity, material.DryDensity.Value);
      Assert.Equal(expected_userDensity, material.UserDensity);
      Assert.Equal(expected_userDefined, material.ERatio.UserDefined);
      if (!expected_userDefined) {
        Assert.Equal(expected_shortTerm, material.ERatio.ShortTerm);
        Assert.Equal(expected_longTerm, material.ERatio.LongTerm);
        Assert.Equal(expected_vibration, material.ERatio.Vibration);
        Assert.Equal(expected_shrinkage, material.ERatio.Shrinkage);
      }
      Assert.Equal(expected_imposedLoadPercentage, material.ImposedLoadPercentage.Percent);
      Assert.Equal(expected_userStrain, material.UserStrain);
      if (expected_userStrain) {
        Assert.Equal(expected_shrinkageStrain, material.ShrinkageStrain.Value);
      }
    }

    // 1 setup inputs
    [Theory]
    // HKSUOS
    [InlineData(ConcreteGrade.C35, WeightType.Normal, DensityClass.NOT_APPLY, 2450, false, 0.33, 0.0, false)]
    [InlineData(ConcreteGrade.C35, WeightType.Normal, DensityClass.NOT_APPLY, 2400, true, 0.33, 0.0, false)]
    public void HKSUOSConstructorTest(ConcreteGrade grade, WeightType type, DensityClass densityClass, double dryDensityValue, bool userDensity, double imposedLoadPercentage, double shrinkageStrainValue, bool userStrain) {
      // 2 create object instance with constructor
      var dryDensity = new Density(dryDensityValue, DensityUnit.KilogramPerCubicCentimeter);
      var eRatio = new ERatio();
      var shrinkageStrain = new Strain(shrinkageStrainValue, StrainUnit.MilliStrain);
      var concreteMaterial = new ConcreteMaterial(grade, dryDensity, userDensity, eRatio, new Ratio(imposedLoadPercentage, RatioUnit.DecimalFraction));

      // 3 check that inputs are set in object's members
      Assert.Equal(grade.ToString(), concreteMaterial.Grade);
      Assert.Equal(type, concreteMaterial.Type);
      Assert.Equal(densityClass, concreteMaterial.Class);
      Assert.Equal(dryDensity, concreteMaterial.DryDensity);
      Assert.Equal(userDensity, concreteMaterial.UserDensity);
      Assert.Equal(eRatio, concreteMaterial.ERatio);
      Assert.Equal(imposedLoadPercentage, concreteMaterial.ImposedLoadPercentage.DecimalFractions);
      Assert.Equal(shrinkageStrain, concreteMaterial.ShrinkageStrain);
      Assert.Equal(userStrain, concreteMaterial.UserStrain);
    }

    [Theory]
    [InlineData(ConcreteGrade.C35, WeightType.Normal, 2400, false, 0, 0, 0, 0, false, 33, "SLAB_CONCRETE_MATERIAL	MEMBER-1	C35	NORMAL	CODE_DENSITY	2400.00	NOT_APPLY	0.330000	CODE_E_RATIO	CODE_STRAIN\n")] // BS Normal
    [InlineData(ConcreteGrade.C50, WeightType.LightWeight, 2200, true, 0, 0, 0, 0, false, 33, "SLAB_CONCRETE_MATERIAL	MEMBER-1	C50	LIGHT	USER_DENSITY	2200.00	0.330000	CODE_E_RATIO	CODE_STRAIN\n")] // BS User Density
    [InlineData(ConcreteGrade.C45, WeightType.Normal, 2400, false, 1, 2, 3, 0, true, 30, "SLAB_CONCRETE_MATERIAL	MEMBER-1	C45	NORMAL	CODE_DENSITY	2400.00	NOT_APPLY	0.300000	USER_E_RATIO	1.00000	2.00000	3.00000	0.000000	CODE_STRAIN\n")] // BS User ERatio
    [InlineData(ConcreteGrade.C35, WeightType.Normal, 2450, false, 0, 0, 0, 0, false, 33, "SLAB_CONCRETE_MATERIAL	MEMBER-1	C35	NORMAL	CODE_DENSITY	2450.00	NOT_APPLY	0.330000	CODE_E_RATIO	CODE_STRAIN\n")] // HKSUOS Normal
    [InlineData(ConcreteGrade.C40, WeightType.Normal, 2400, false, 0, 0, 0, 0, false, 33, "SLAB_CONCRETE_MATERIAL	MEMBER-1	C40	NORMAL	CODE_DENSITY	2400.00	NOT_APPLY	0.330000	CODE_E_RATIO	CODE_STRAIN\n")] // ASNZ Normal
    public void ToCoaStringTest(ConcreteGrade grade, WeightType type, double dryDensity, bool userDensity, double shortTerm, double longTerm, double vibration, double shrinkage, bool userDefined, double imposedLoadPercentage, string expected_coaString) {
      var eRatio = new ERatio {
        ShortTerm = shortTerm,
        LongTerm = longTerm,
        Vibration = vibration,
        Shrinkage = shrinkage,
        UserDefined = userDefined
      };
      var concreteMaterial = new ConcreteMaterial(grade, type, new Density(dryDensity, DensityUnit.KilogramPerCubicMeter), userDensity, eRatio, new Ratio(imposedLoadPercentage, RatioUnit.Percent));
      string coaString = concreteMaterial.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }

    [Theory]
    [InlineData(ConcreteGradeEN.C35_45, DensityClass.NOT_APPLY, 2400, false, 0, 0, 0, 0, false, 33, -0.000325, false, "SLAB_CONCRETE_MATERIAL	MEMBER-1	C35/45	NORMAL	CODE_DENSITY	2400.00	NOT_APPLY	0.330000	CODE_E_RATIO	CODE_STRAIN\n")] // EN Normal
    [InlineData(ConcreteGradeEN.C35_45, DensityClass.NOT_APPLY, 2400, false, 0, 0, 0, 0, false, 33, -0.0003, true, "SLAB_CONCRETE_MATERIAL	MEMBER-1	C35/45	NORMAL	CODE_DENSITY	2400.00	NOT_APPLY	0.330000	CODE_E_RATIO	USER_STRAIN	-0.000300000\n")] // EN Normal User Strain
    [InlineData(ConcreteGradeEN.LC35_38, DensityClass.DC1801_2000, 2000, false, 0, 0, 0, 0, false, 33, -0.0005, false, "SLAB_CONCRETE_MATERIAL	MEMBER-1	LC35/38	LIGHT	CODE_DENSITY	2000.00	1801_2000	0.330000	CODE_E_RATIO	CODE_STRAIN\n")] // EN Light
    [InlineData(ConcreteGradeEN.LC35_38, DensityClass.NOT_APPLY, 1000, true, 0, 0, 0, 0, false, 33, -0.0005, false, "SLAB_CONCRETE_MATERIAL	MEMBER-1	LC35/38	LIGHT	USER_DENSITY	1000.00	0.330000	CODE_E_RATIO	CODE_STRAIN\n")] // EN Light User Density
    public void ToCoaStringTestEN(ConcreteGradeEN grade, DensityClass densityClass, double dryDensity, bool userDensity, double shortTerm, double longTerm, double vibration, double shrinkage, bool userDefined, double imposedLoadPercentage, double shrinkageStrain, bool userStrain, string expected_coaString) {
      var eRatio = new ERatio {
        ShortTerm = shortTerm,
        LongTerm = longTerm,
        Vibration = vibration,
        Shrinkage = shrinkage,
        UserDefined = userDefined
      };
      var concreteMaterial = new ConcreteMaterial(grade, densityClass, new Density(dryDensity, DensityUnit.KilogramPerCubicMeter), userDensity, eRatio, new Ratio(imposedLoadPercentage, RatioUnit.Percent), new Strain(shrinkageStrain, StrainUnit.Ratio), userStrain);
      string coaString = concreteMaterial.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }
  }
}
