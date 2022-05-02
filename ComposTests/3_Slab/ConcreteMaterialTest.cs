﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oasys.Units;
using UnitsNet;
using UnitsNet.Units;
using Xunit;
using static ComposAPI.ConcreteMaterial;

namespace ComposAPI.Tests
{
  public class ConcreteMaterialTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(ConcreteGrade.C30, WeightType.Normal, DensityClass.None, 2400, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGrade.C30, WeightType.Normal, DensityClass.None, 2300, true, 0.33, -0.000325, false)]
    [InlineData(ConcreteGrade.C30, WeightType.Light, DensityClass.None, 1800, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGrade.C30, WeightType.Light, DensityClass.None, 1900, true, 0.33, -0.000325, false)]
    public ConcreteMaterial TestBritishConstructor(ConcreteGrade grade, WeightType type, DensityClass densityClass, double dryDensityValue, bool userDensity, double imposedLoadPercentage, double shrinkageStrainValue, bool userStrain)
    {
      // 2 create object instance with constructor
      Density dryDensity = new Density(dryDensityValue, DensityUnit.KilogramPerCubicCentimeter);
      ERatio eRatio = new ERatio();
      Strain shrinkageStrain = new Strain(shrinkageStrainValue, StrainUnit.MilliStrain);
      ConcreteMaterial concreteMaterial = new ConcreteMaterial(grade, type, dryDensity, userDensity, eRatio, imposedLoadPercentage);

      // 3 check that inputs are set in object's members
      Assert.Equal(grade.ToString(), concreteMaterial.Grade);
      Assert.Equal(type, concreteMaterial.Type);
      Assert.Equal(densityClass, concreteMaterial.Class);
      Assert.Equal(dryDensity, concreteMaterial.DryDensity);
      Assert.Equal(userDensity, concreteMaterial.UserDensity);
      Assert.Equal(eRatio, concreteMaterial.ERatio);
      Assert.Equal(imposedLoadPercentage, concreteMaterial.ImposedLoadPercentage);
      Assert.Equal(shrinkageStrain, concreteMaterial.ShrinkageStrain);
      Assert.Equal(userStrain, concreteMaterial.UserStrain);

      // (optionally return object for other tests)
      return concreteMaterial;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(ConcreteGradeEN.C35_45, WeightType.Normal, DensityClass.None, 2400, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.C35_45, WeightType.Normal, DensityClass.None, 2200, true, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.C35_45, WeightType.Normal, DensityClass.None, 2400, false, 0.33, -0.0003, true)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.Light, DensityClass.DC801_1000, 1000, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.Light, DensityClass.DC1001_1200, 1200, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.Light, DensityClass.DC1201_1400, 1400, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.Light, DensityClass.DC1401_1600, 1600, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.Light, DensityClass.DC1601_1800, 1800, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.Light, DensityClass.DC1801_2000, 2000, false, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.Light, DensityClass.DC1801_2000, 1800, true, 0.33, -0.000325, false)]
    [InlineData(ConcreteGradeEN.LC30_33, WeightType.Light, DensityClass.DC1801_2000, 1800, true, 0.33, -0.0003, true)]
    public ConcreteMaterial TestEuropeanConstructor(ConcreteGradeEN grade, WeightType type, DensityClass densityClass, double dryDensityValue, bool userDensity, double imposedLoadPercentage, double shrinkageStrainValue, bool userStrain)
    {
      // 2 create object instance with constructor
      Density dryDensity = new Density(dryDensityValue, DensityUnit.KilogramPerCubicCentimeter);
      ERatio eRatio = new ERatio();
      Strain shrinkageStrain = new Strain(shrinkageStrainValue, StrainUnit.MilliStrain);
      ConcreteMaterial concreteMaterial = new ConcreteMaterial(grade, densityClass, dryDensity, userDensity, eRatio, imposedLoadPercentage, shrinkageStrain, userStrain);

      // 3 check that inputs are set in object's members
      Assert.Equal(grade.ToString(), concreteMaterial.Grade);
      Assert.Equal(type, concreteMaterial.Type);
      Assert.Equal(densityClass, concreteMaterial.Class);
      Assert.Equal(dryDensity, concreteMaterial.DryDensity);
      Assert.Equal(userDensity, concreteMaterial.UserDensity);
      Assert.Equal(eRatio, concreteMaterial.ERatio);
      Assert.Equal(imposedLoadPercentage, concreteMaterial.ImposedLoadPercentage);
      Assert.Equal(shrinkageStrain, concreteMaterial.ShrinkageStrain);
      Assert.Equal(userStrain, concreteMaterial.UserStrain);

      // (optionally return object for other tests)
      return concreteMaterial;
    }

    // 1 setup inputs
    [Theory]
    // HKSUOS
    [InlineData(ConcreteGrade.C35, WeightType.Normal, DensityClass.None, 2450, false, 0.33, 0.0, false)]
    [InlineData(ConcreteGrade.C35, WeightType.Normal, DensityClass.None, 2400, true, 0.33, 0.0, false)]
    public ConcreteMaterial TestHKSUOSConstructor(ConcreteGrade grade, WeightType type, DensityClass densityClass, double dryDensityValue, bool userDensity, double imposedLoadPercentage, double shrinkageStrainValue, bool userStrain)
    {
      // 2 create object instance with constructor
      Density dryDensity = new Density(dryDensityValue, DensityUnit.KilogramPerCubicCentimeter);
      ERatio eRatio = new ERatio();
      Strain shrinkageStrain = new Strain(shrinkageStrainValue, StrainUnit.MilliStrain);
      ConcreteMaterial concreteMaterial = new ConcreteMaterial(grade, dryDensity, userDensity, eRatio, imposedLoadPercentage);

      // 3 check that inputs are set in object's members
      Assert.Equal(grade.ToString(), concreteMaterial.Grade);
      Assert.Equal(type, concreteMaterial.Type);
      Assert.Equal(densityClass, concreteMaterial.Class);
      Assert.Equal(dryDensity, concreteMaterial.DryDensity);
      Assert.Equal(userDensity, concreteMaterial.UserDensity);
      Assert.Equal(eRatio, concreteMaterial.ERatio);
      Assert.Equal(imposedLoadPercentage, concreteMaterial.ImposedLoadPercentage);
      Assert.Equal(shrinkageStrain, concreteMaterial.ShrinkageStrain);
      Assert.Equal(userStrain, concreteMaterial.UserStrain);

      // (optionally return object for other tests)
      return concreteMaterial;
    }

    // 1 setup inputs
    [Theory]
    // AS/NZ 
    [InlineData(ConcreteGrade.C25, WeightType.Normal, DensityClass.None, 2400, false, 0.33, -0.00085, false)]
    [InlineData(ConcreteGrade.C25, WeightType.Normal, DensityClass.None, 2200, true, 0.33, -0.00085, false)]
    [InlineData(ConcreteGrade.C25, WeightType.Normal, DensityClass.None, 2400, false, 0.33, -0.0003, true)]
    public ConcreteMaterial TestASNZConstructor(ConcreteGrade grade, WeightType type, DensityClass densityClass, double dryDensityValue, bool userDensity, double imposedLoadPercentage, double shrinkageStrainValue, bool userStrain)
    {
      // 2 create object instance with constructor
      Density dryDensity = new Density(dryDensityValue, DensityUnit.KilogramPerCubicCentimeter);
      ERatio eRatio = new ERatio();
      Strain shrinkageStrain = new Strain(shrinkageStrainValue, StrainUnit.MilliStrain);
      ConcreteMaterial concreteMaterial = new ConcreteMaterial(grade, dryDensity, userDensity, eRatio, imposedLoadPercentage, shrinkageStrain, userStrain);

      // 3 check that inputs are set in object's members
      Assert.Equal(grade.ToString(), concreteMaterial.Grade);
      Assert.Equal(type, concreteMaterial.Type);
      Assert.Equal(densityClass, concreteMaterial.Class);
      Assert.Equal(dryDensity, concreteMaterial.DryDensity);
      Assert.Equal(userDensity, concreteMaterial.UserDensity);
      Assert.Equal(eRatio, concreteMaterial.ERatio);
      Assert.Equal(imposedLoadPercentage, concreteMaterial.ImposedLoadPercentage);
      Assert.Equal(shrinkageStrain, concreteMaterial.ShrinkageStrain);
      Assert.Equal(userStrain, concreteMaterial.UserStrain);

      // (optionally return object for other tests)
      return concreteMaterial;
    }

    [Theory]
    [InlineData(ConcreteGradeEN.C35_45, WeightType.Normal, DensityClass.None, 2400, false, 0.33, -0.000325, false)]
    public void TestDuplicate(ConcreteGradeEN grade, WeightType type, DensityClass densityClass, double dryDensityValue, bool userDensity, double imposedLoadPercentage, double shrinkageStrainValue, bool userStrain)
    {
      // 1 create with constructor and duplicate
      Density dryDensity = new Density(dryDensityValue, DensityUnit.KilogramPerCubicCentimeter);
      ERatio eRatio = new ERatio(9.87, 28.72, 9.55, 27.55);
      Strain shrinkageStrain = new Strain(shrinkageStrainValue, StrainUnit.MilliStrain);
      ConcreteMaterial original = new ConcreteMaterial(grade, densityClass, dryDensity, userDensity, eRatio, imposedLoadPercentage, shrinkageStrain, userStrain);
      ConcreteMaterial duplicate = original.Duplicate();

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
      Assert.Equal(imposedLoadPercentage, duplicate.ImposedLoadPercentage);
      Assert.Equal(shrinkageStrain, duplicate.ShrinkageStrain);
      Assert.Equal(userStrain, duplicate.UserStrain);

      // 3 make some changes to duplicate
      duplicate.Grade = ConcreteGradeEN.LC30_33.ToString();
      duplicate.Type = WeightType.Light;
      duplicate.Class = DensityClass.DC1601_1800;
      Density duplicateDensity = new Density(1780, DensityUnit.KilogramPerCubicMeter);
      duplicate.DryDensity = duplicateDensity;
      duplicate.UserDensity = true;
      duplicate.ERatio.ShortTerm = 1;
      duplicate.ERatio.LongTerm = 2;
      duplicate.ERatio.Vibration = 3;
      duplicate.ERatio.Shrinkage = 4;
      duplicate.ImposedLoadPercentage = 0.5;
      Strain duplicateStrain = new Strain(-0.0004, StrainUnit.MilliStrain);
      duplicate.ShrinkageStrain = duplicateStrain;
      duplicate.UserStrain = true;

      // 4 check that duplicate has set changes
      Assert.Equal(ConcreteGradeEN.LC30_33.ToString(), duplicate.Grade);
      Assert.Equal(WeightType.Light, duplicate.Type);
      Assert.Equal(DensityClass.DC1601_1800, duplicate.Class);
      Assert.Equal(duplicateDensity, duplicate.DryDensity);
      Assert.True(duplicate.UserDensity);
      Assert.Equal(1, duplicate.ERatio.ShortTerm);
      Assert.Equal(2, duplicate.ERatio.LongTerm);
      Assert.Equal(3, duplicate.ERatio.Vibration);
      Assert.Equal(4, duplicate.ERatio.Shrinkage);
      Assert.Equal(0.5, duplicate.ImposedLoadPercentage);
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
      Assert.Equal(imposedLoadPercentage, original.ImposedLoadPercentage);
      Assert.Equal(shrinkageStrain, original.ShrinkageStrain);
      Assert.Equal(userStrain, original.UserStrain);
    }
  }
}