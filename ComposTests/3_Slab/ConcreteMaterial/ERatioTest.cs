using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ComposAPI.Tests
{
  public static class ERatioMother
  {
    public static ERatio CreateERatio()
    {
      return new ERatio(9.87, 28.72, 9.55, 27.55);
    }
  }

  public class ERatioTest
  {
    [Fact]
    public ERatio EmptyConstructorTest()
    {
      ERatio eRatio = new ERatio();

      Assert.False(eRatio.UserDefined);

      return eRatio;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(6, 18, 5.39)]
    public void Constructor1Test(double shortTerm, double longTerm, double vibration)
    {
      // 2 create object instance with constructor
      ERatio eRatio = new ERatio(shortTerm, longTerm, vibration);

      // 3 check that inputs are set in object's members
      Assert.Equal(shortTerm, eRatio.ShortTerm);
      Assert.Equal(longTerm, eRatio.LongTerm);
      Assert.Equal(vibration, eRatio.Vibration);
      Assert.Equal(double.NaN, eRatio.Shrinkage);
      Assert.True(eRatio.UserDefined);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(9.87, 28.72, 9.55, 27.55)]
    public void Constructor2Test(double shortTerm, double longTerm, double vibration, double shrinkage)
    {
      // 2 create object instance with constructor
      ERatio eRatio = new ERatio(shortTerm, longTerm, vibration, shrinkage);

      // 3 check that inputs are set in object's members
      Assert.Equal(shortTerm, eRatio.ShortTerm);
      Assert.Equal(longTerm, eRatio.LongTerm);
      Assert.Equal(vibration, eRatio.Vibration);
      Assert.Equal(shrinkage, eRatio.Shrinkage);
      Assert.True(eRatio.UserDefined);
    }

    [Theory]
    [InlineData(9.87, 28.72, 9.55, 27.55)]
    public void DuplicateTest(double shortTerm, double longTerm, double vibration, double shrinkage)
    {
      // 1 create with constructor and duplicate
      ERatio original = new ERatio(shortTerm, longTerm, vibration, shrinkage);
      ERatio? duplicate = original.Duplicate() as ERatio;

      // 2 check that duplicate has duplicated values
      Assert.Equal(original.ShortTerm, duplicate.ShortTerm);
      Assert.Equal(original.LongTerm, duplicate.LongTerm);
      Assert.Equal(original.Vibration, duplicate.Vibration);
      Assert.Equal(original.Shrinkage, duplicate.Shrinkage);
      Assert.Equal(original.UserDefined, duplicate.UserDefined);

      // 3 make some changes to duplicate
      duplicate.ShortTerm = 6;
      duplicate.LongTerm = 18;
      duplicate.Vibration = 5.39;
      duplicate.Shrinkage = 0;

      // 4 check that duplicate has set changes
      Assert.Equal(6, duplicate.ShortTerm);
      Assert.Equal(18, duplicate.LongTerm);
      Assert.Equal(5.39, duplicate.Vibration);
      Assert.Equal(0, duplicate.Shrinkage);

      // 5 check that original has not been changed
      Assert.Equal(shortTerm, original.ShortTerm);
      Assert.Equal(longTerm, original.LongTerm);
      Assert.Equal(vibration, original.Vibration);
      Assert.Equal(shrinkage, original.Shrinkage);
      Assert.True(original.UserDefined);
    }
  }
}
