using Xunit;

namespace ComposAPI.Members.Tests
{
  public partial class DesignCodeTest
  {
    [Fact]
    public DesignOption TestDesignOptionConstructor()
    {
      // 1 setup input
      // use default values

      // 2 create object instance with constructor
      DesignOption designOption = new DesignOption();

      // 3 check that inputs are set in object's members
      Assert.True(designOption.ProppedDuringConstruction);
      Assert.False(designOption.InclSteelBeamWeight);
      Assert.False(designOption.InclThinFlangeSections);
      Assert.False(designOption.InclConcreteSlabWeight);
      Assert.False(designOption.ConsiderShearDeflection);

      // (optionally return object for other tests)
      return designOption;
    }

    [Fact]
    public void TestDesignOptionDuplicate()
    {
      // 1 create with constructor and duplicate
      DesignOption original = new DesignOption();
      DesignOption duplicate = (DesignOption)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.True(duplicate.ProppedDuringConstruction);
      Assert.False(duplicate.InclSteelBeamWeight);
      Assert.False(duplicate.InclThinFlangeSections);
      Assert.False(duplicate.InclConcreteSlabWeight);
      Assert.False(duplicate.ConsiderShearDeflection);

      // 3 make some changes to duplicate
      duplicate.ProppedDuringConstruction = false;
      duplicate.InclSteelBeamWeight = true;
      duplicate.InclThinFlangeSections = true;
      duplicate.InclConcreteSlabWeight = true;
      duplicate.ConsiderShearDeflection = true;

      // 4 check that duplicate has set changes
      Assert.False(duplicate.ProppedDuringConstruction);
      Assert.True(duplicate.InclSteelBeamWeight);
      Assert.True(duplicate.InclThinFlangeSections);
      Assert.True(duplicate.InclConcreteSlabWeight);
      Assert.True(duplicate.ConsiderShearDeflection);

      // 5 check that original has not been changed
      Assert.True(original.ProppedDuringConstruction);
      Assert.False(original.InclSteelBeamWeight);
      Assert.False(original.InclThinFlangeSections);
      Assert.False(original.InclConcreteSlabWeight);
      Assert.False(original.ConsiderShearDeflection);
    }
  }
}
