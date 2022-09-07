using ComposAPI.Tests;
using Moq;
using System;
using System.Linq.Expressions;
using UnitsNet;
using UnitsNet.Units;
using Xunit;
using ComposGHTests.Helpers;


namespace ComposAPI.Members.Tests
{
  public partial class DesignCodeTest
  {
    [Fact]
    public CodeOptionsASNZ TestCodeOptionsConstructor()
    {
      // 1 setup input
      // empty constructor creates default AS/NZ values

      // 2 create object instance with constructor
      CodeOptionsASNZ codeOptions = new CodeOptionsASNZ();

      // 3 check that inputs are set in object's members
      Assert.False(codeOptions.ConsiderShrinkageDeflection);
      Assert.Equal(2.0, codeOptions.LongTerm.CreepCoefficient);
      Assert.Equal(2.0, codeOptions.ShortTerm.CreepCoefficient);

      // (optionally return object for other tests)
      return codeOptions;
    }
    [Fact]
    public void DuplicateASNZTest()
    {
      // 1 create with constructor and duplicate
      CodeOptionsASNZ original = new CodeOptionsASNZ();
      CodeOptionsASNZ duplicate = (CodeOptionsASNZ)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void TestCodeOptionsDuplicate()
    {
      // 1 create with constructor and duplicate
      CodeOptionsASNZ original = new CodeOptionsASNZ();
      CodeOptionsASNZ duplicate = (CodeOptionsASNZ)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.False(duplicate.ConsiderShrinkageDeflection);
      Assert.Equal(2.0, duplicate.LongTerm.CreepCoefficient);
      Assert.Equal(2.0, duplicate.ShortTerm.CreepCoefficient);

      // 3 make some changes to duplicate
      CreepShrinkageParametersASNZ shortTerm = new CreepShrinkageParametersASNZ();
      shortTerm.CreepCoefficient = 4.0;
      CreepShrinkageParametersASNZ longTerm = new CreepShrinkageParametersASNZ();
      longTerm.CreepCoefficient = 6.0;
      duplicate.ShortTerm = shortTerm;
      duplicate.LongTerm = longTerm;
      duplicate.ConsiderShrinkageDeflection = true;

      // 4 check that duplicate has set changes
      Assert.True(duplicate.ConsiderShrinkageDeflection);
      Assert.Equal(6.0, duplicate.LongTerm.CreepCoefficient);
      Assert.Equal(4.0, duplicate.ShortTerm.CreepCoefficient);

      // 5 check that original has not been changed
      Assert.False(original.ConsiderShrinkageDeflection);
      Assert.Equal(2.0, original.LongTerm.CreepCoefficient);
      Assert.Equal(2.0, original.ShortTerm.CreepCoefficient);
    }

    [Fact]
    public CodeOptionsEN TestEC4OptionsConstructor()
    {
      // 1 setup input
      // empty constructor creates default EC4 values

      // 2 create object instance with constructor
      CodeOptionsEN codeOptions = new CodeOptionsEN();
      CreepShrinkageParametersEN lt = (CreepShrinkageParametersEN)codeOptions.LongTerm;
      CreepShrinkageParametersEN st = (CreepShrinkageParametersEN)codeOptions.ShortTerm;

      // 3 check that inputs are set in object's members
      Assert.False(codeOptions.ApproxModularRatios);
      Assert.False(codeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      Assert.Equal(CementClass.N, codeOptions.CementType);
      Assert.Equal(1.1, lt.CreepCoefficient);
      Assert.Equal(28, lt.ConcreteAgeAtLoad);
      Assert.Equal(36500, lt.FinalConcreteAgeCreep);
      Assert.Equal(0.5, lt.RelativeHumidity.DecimalFractions);
      Assert.Equal(0.55, st.CreepCoefficient);
      Assert.Equal(1, st.ConcreteAgeAtLoad);
      Assert.Equal(36500, st.FinalConcreteAgeCreep);
      Assert.Equal(0.5, st.RelativeHumidity.DecimalFractions);

      // (optionally return object for other tests)
      return codeOptions;
    }
    [Fact]
    public void DuplicateEC4Test()
    {
      // 1 create with constructor and duplicate
      CodeOptionsEN original = new CodeOptionsEN();
      CodeOptionsEN duplicate = (CodeOptionsEN)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void TestEC4OptionsDuplicate()
    {
      // 1 create with constructor and duplicate
      CodeOptionsEN original = new CodeOptionsEN();
      CodeOptionsEN duplicate = (CodeOptionsEN)original.Duplicate();
      CreepShrinkageParametersEN lt = (CreepShrinkageParametersEN)duplicate.LongTerm;
      CreepShrinkageParametersEN st = (CreepShrinkageParametersEN)duplicate.ShortTerm;

      // 2 check that duplicate has duplicated values
      Assert.False(duplicate.ApproxModularRatios);
      Assert.False(duplicate.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      Assert.Equal(CementClass.N, duplicate.CementType);
      Assert.Equal(1.1, lt.CreepCoefficient);
      Assert.Equal(28, lt.ConcreteAgeAtLoad);
      Assert.Equal(36500, lt.FinalConcreteAgeCreep);
      Assert.Equal(0.5, lt.RelativeHumidity.DecimalFractions);
      Assert.Equal(0.55, st.CreepCoefficient);
      Assert.Equal(1, st.ConcreteAgeAtLoad);
      Assert.Equal(36500, st.FinalConcreteAgeCreep);
      Assert.Equal(0.5, st.RelativeHumidity.DecimalFractions);

      // 3 make some changes to duplicate
      CreepShrinkageParametersEN longTerm = new CreepShrinkageParametersEN();
      longTerm.CreepCoefficient = 4.0;
      longTerm.ConcreteAgeAtLoad = 45;
      longTerm.FinalConcreteAgeCreep = 500;
      longTerm.RelativeHumidity = new Ratio(0.3, RatioUnit.DecimalFraction);
      CreepShrinkageParametersEN shortTerm = new CreepShrinkageParametersEN();
      shortTerm.CreepCoefficient = 2.0;
      shortTerm.ConcreteAgeAtLoad = 2;
      shortTerm.FinalConcreteAgeCreep = 28;
      shortTerm.RelativeHumidity = new Ratio(0.95, RatioUnit.DecimalFraction);

      duplicate.LongTerm = longTerm;
      duplicate.ShortTerm = shortTerm;
      duplicate.ConsiderShrinkageDeflection = true;
      duplicate.ApproxModularRatios = true;
      duplicate.IgnoreShrinkageDeflectionForLowLengthToDepthRatios = true;
      duplicate.CementType = CementClass.S;

      // 4 check that duplicate has set changes
      CreepShrinkageParametersEN lt2 = (CreepShrinkageParametersEN)duplicate.LongTerm;
      CreepShrinkageParametersEN st2 = (CreepShrinkageParametersEN)duplicate.ShortTerm;
      Assert.True(duplicate.ApproxModularRatios);
      Assert.True(duplicate.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      Assert.Equal(CementClass.S, duplicate.CementType);
      Assert.Equal(4.0, lt2.CreepCoefficient);
      Assert.Equal(45, lt2.ConcreteAgeAtLoad);
      Assert.Equal(500, lt2.FinalConcreteAgeCreep);
      Assert.Equal(0.3, lt2.RelativeHumidity.DecimalFractions);
      Assert.Equal(2.0, st2.CreepCoefficient);
      Assert.Equal(2, st2.ConcreteAgeAtLoad);
      Assert.Equal(28, st2.FinalConcreteAgeCreep);
      Assert.Equal(0.95, st2.RelativeHumidity.DecimalFractions);

      // 5 check that original has not been changed
      CreepShrinkageParametersEN olt = (CreepShrinkageParametersEN)original.LongTerm;
      CreepShrinkageParametersEN ost = (CreepShrinkageParametersEN)original.ShortTerm;
      Assert.False(original.ApproxModularRatios);
      Assert.False(original.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      Assert.Equal(CementClass.N, original.CementType);
      Assert.Equal(1.1, olt.CreepCoefficient);
      Assert.Equal(28, olt.ConcreteAgeAtLoad);
      Assert.Equal(36500, olt.FinalConcreteAgeCreep);
      Assert.Equal(0.5, olt.RelativeHumidity.DecimalFractions);
      Assert.Equal(0.55, ost.CreepCoefficient);
      Assert.Equal(1, ost.ConcreteAgeAtLoad);
      Assert.Equal(36500, ost.FinalConcreteAgeCreep);
      Assert.Equal(0.5, ost.RelativeHumidity.DecimalFractions);
    }
  }
}
