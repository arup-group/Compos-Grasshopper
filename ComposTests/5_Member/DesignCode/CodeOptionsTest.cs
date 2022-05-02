using Xunit;

namespace ComposAPI.Tests
{
  public partial class DesignCodeTest
  {
    [Fact]
    public CodeOptions TestCodeOptionsConstructor()
    {
      // 1 setup input
      // empty constructor creates default AS/NZ values

      // 2 create object instance with constructor
      CodeOptions codeOptions = new CodeOptions();

      // 3 check that inputs are set in object's members
      Assert.False(codeOptions.ConsiderShrinkageDeflection);
      Assert.Equal(2.0, codeOptions.LongTerm.CreepCoefficient);
      Assert.Equal(2.0, codeOptions.ShortTerm.CreepCoefficient);

      // (optionally return object for other tests)
      return codeOptions;
    }

    [Fact]
    public void TestCodeOptionsDuplicate()
    {
      // 1 create with constructor and duplicate
      CodeOptions original = new CodeOptions();
      CodeOptions duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.False(duplicate.ConsiderShrinkageDeflection);
      Assert.Equal(2.0, duplicate.LongTerm.CreepCoefficient);
      Assert.Equal(2.0, duplicate.ShortTerm.CreepCoefficient);

      // 3 make some changes to duplicate
      CreepShrinkageParameters shortTerm = new CreepShrinkageParameters();
      shortTerm.CreepCoefficient = 4.0;
      CreepShrinkageParameters longTerm = new CreepShrinkageParameters();
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
    public CodeOptions TestEC4OptionsConstructor()
    {
      // 1 setup input
      // empty constructor creates default EC4 values

      // 2 create object instance with constructor
      EC4Options codeOptions = new EC4Options();

      // 3 check that inputs are set in object's members
      Assert.False(codeOptions.ApproxModularRatios);
      Assert.False(codeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      Assert.Equal(EC4Options.CementClass.N, codeOptions.CementType);
      Assert.Equal(1.1, codeOptions.LongTerm.CreepCoefficient);
      Assert.Equal(28, codeOptions.LongTerm.ConcreteAgeAtLoad);
      Assert.Equal(36500, codeOptions.LongTerm.FinalConcreteAgeCreep);
      Assert.Equal(0.5, codeOptions.LongTerm.RelativeHumidity);
      Assert.Equal(0.55, codeOptions.ShortTerm.CreepCoefficient);
      Assert.Equal(1, codeOptions.ShortTerm.ConcreteAgeAtLoad);
      Assert.Equal(36500, codeOptions.ShortTerm.FinalConcreteAgeCreep);
      Assert.Equal(0.5, codeOptions.ShortTerm.RelativeHumidity);

      // (optionally return object for other tests)
      return codeOptions;
    }

    [Fact]
    public void TestEC4OptionsDuplicate()
    {
      // 1 create with constructor and duplicate
      EC4Options original = new EC4Options();
      EC4Options duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.False(duplicate.ApproxModularRatios);
      Assert.False(duplicate.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      Assert.Equal(EC4Options.CementClass.N, duplicate.CementType);
      Assert.Equal(1.1, duplicate.LongTerm.CreepCoefficient);
      Assert.Equal(28, duplicate.LongTerm.ConcreteAgeAtLoad);
      Assert.Equal(36500, duplicate.LongTerm.FinalConcreteAgeCreep);
      Assert.Equal(0.5, duplicate.LongTerm.RelativeHumidity);
      Assert.Equal(0.55, duplicate.ShortTerm.CreepCoefficient);
      Assert.Equal(1, duplicate.ShortTerm.ConcreteAgeAtLoad);
      Assert.Equal(36500, duplicate.ShortTerm.FinalConcreteAgeCreep);
      Assert.Equal(0.5, duplicate.ShortTerm.RelativeHumidity);

      // 3 make some changes to duplicate
      CreepShrinkageEuroCodeParameters longTerm = new CreepShrinkageEuroCodeParameters();
      longTerm.CreepCoefficient = 4.0;
      longTerm.ConcreteAgeAtLoad = 45;
      longTerm.FinalConcreteAgeCreep = 500;
      longTerm.RelativeHumidity = 0.3;
      CreepShrinkageEuroCodeParameters shortTerm = new CreepShrinkageEuroCodeParameters();
      shortTerm.CreepCoefficient = 2.0;
      shortTerm.ConcreteAgeAtLoad = 2;
      shortTerm.FinalConcreteAgeCreep = 28;
      shortTerm.RelativeHumidity = 0.95;

      duplicate.LongTerm = longTerm;
      duplicate.ShortTerm = shortTerm;
      duplicate.ConsiderShrinkageDeflection = true;
      duplicate.ApproxModularRatios = true;
      duplicate.IgnoreShrinkageDeflectionForLowLengthToDepthRatios = true;
      duplicate.CementType = EC4Options.CementClass.S;

      // 4 check that duplicate has set changes
      Assert.True(duplicate.ApproxModularRatios);
      Assert.True(duplicate.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      Assert.Equal(EC4Options.CementClass.S, duplicate.CementType);
      Assert.Equal(4.0, duplicate.LongTerm.CreepCoefficient);
      Assert.Equal(45, duplicate.LongTerm.ConcreteAgeAtLoad);
      Assert.Equal(500, duplicate.LongTerm.FinalConcreteAgeCreep);
      Assert.Equal(0.3, duplicate.LongTerm.RelativeHumidity);
      Assert.Equal(2.0, duplicate.ShortTerm.CreepCoefficient);
      Assert.Equal(2, duplicate.ShortTerm.ConcreteAgeAtLoad);
      Assert.Equal(28, duplicate.ShortTerm.FinalConcreteAgeCreep);
      Assert.Equal(0.95, duplicate.ShortTerm.RelativeHumidity);

      // 5 check that original has not been changed
      Assert.False(original.ApproxModularRatios);
      Assert.False(original.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      Assert.Equal(EC4Options.CementClass.N, original.CementType);
      Assert.Equal(1.1, original.LongTerm.CreepCoefficient);
      Assert.Equal(28, original.LongTerm.ConcreteAgeAtLoad);
      Assert.Equal(36500, original.LongTerm.FinalConcreteAgeCreep);
      Assert.Equal(0.5, original.LongTerm.RelativeHumidity);
      Assert.Equal(0.55, original.ShortTerm.CreepCoefficient);
      Assert.Equal(1, original.ShortTerm.ConcreteAgeAtLoad);
      Assert.Equal(36500, original.ShortTerm.FinalConcreteAgeCreep);
      Assert.Equal(0.5, original.ShortTerm.RelativeHumidity);
    }
  }
}
