using System.Collections.Generic;
using ComposGH.Helpers;
using ComposGHTests.Helpers;
using OasysGH;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposAPI.Loads.Tests {
  public partial class LoadTest {

    [Fact]
    public void DuplicateMemberTest() {
      // 1 create with constructor and duplicate
      Load original = TestMemberLoadConstructor(100, "MEMBER-2", MemberLoad.SupportSide.Right);
      var duplicate = (Load)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void MemberLoadLeftFromCoaStringTest() {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      var units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;

      // Arrange
      string coaString = "LOAD	MEMBER-1	Member load	MEMBER-2	Left	1.50000\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      var memberLoad1 = (MemberLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Member load	MEMBER-2	Left	1.50000
      Assert.Equal(1.5, memberLoad1.Position.As(lengthUnit));
      Assert.Equal("MEMBER-2", memberLoad1.MemberName);
      Assert.Equal(MemberLoad.SupportSide.Left, memberLoad1.Support);
      Assert.Equal(LoadType.MemberLoad, memberLoad1.Type);
    }

    [Fact]
    public void MemberLoadLeftFromCoaStringTestPercentage() {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      var units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;

      // Arrange
      string coaString = "LOAD	MEMBER-1	Member load	MEMBER-2	Left	1.50000%\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      var memberLoad1 = (MemberLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Member load	MEMBER-2	Left	1.50000
      Assert.Equal(1.5, memberLoad1.Position.As(RatioUnit.Percent));
      Assert.Equal("MEMBER-2", memberLoad1.MemberName);
      Assert.Equal(MemberLoad.SupportSide.Left, memberLoad1.Support);
      Assert.Equal(LoadType.MemberLoad, memberLoad1.Type);
    }

    [Fact]
    public void MemberLoadLeftToCoaStringTest() {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Member load	MEMBER-2	Left	0.150000\n";
      Load load = TestMemberLoadConstructor(150, "Member-2", MemberLoad.SupportSide.Left); // pos units in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void MemberLoadLeftToCoaStringTestPercentage() {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Member load	MEMBER-2	Left	0.150000%\n";
      Load load = TestMemberLoadConstructorPercentage(0.15, "Member-2", MemberLoad.SupportSide.Left); // pos units in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void MemberLoadRightFromCoaStringTest() {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      var units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;

      // Arrange
      string coaString = "LOAD	MEMBER-1	Member load	MEMBER-2	Right	3.00000\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      var memberLoad2 = (MemberLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Member load	MEMBER-2	Right	3.00000
      Assert.Equal(3, memberLoad2.Position.As(lengthUnit));
      Assert.Equal("MEMBER-2", memberLoad2.MemberName);
      Assert.Equal(MemberLoad.SupportSide.Right, memberLoad2.Support);
      Assert.Equal(LoadType.MemberLoad, memberLoad2.Type);
    }

    [Fact]
    public void MemberLoadRightFromCoaStringTestPercentage() {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      var units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;

      // Arrange
      string coaString = "LOAD	MEMBER-1	Member load	MEMBER-2	Right	3.00000%\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      var memberLoad2 = (MemberLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Member load	MEMBER-2	Right	3.00000
      Assert.Equal(3, memberLoad2.Position.As(RatioUnit.Percent));
      Assert.Equal("MEMBER-2", memberLoad2.MemberName);
      Assert.Equal(MemberLoad.SupportSide.Right, memberLoad2.Support);
      Assert.Equal(LoadType.MemberLoad, memberLoad2.Type);
    }

    [Fact]
    public void MemberLoadRightToCoaStringTest() {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Member load	MEMBER-2	Right	0.00250000\n";
      Load load = TestMemberLoadConstructor(2.5, "Member-2", MemberLoad.SupportSide.Right); // pos units in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void MemberLoadRightToCoaStringTestPercentage() {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Member load	MEMBER-2	Right	0.00250000%\n";
      Load load = TestMemberLoadConstructorPercentage(0.0025, "Member-2", MemberLoad.SupportSide.Right); // pos units in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(100, "MEMBER-2", MemberLoad.SupportSide.Right)]
    [InlineData(4000, "MEMBER-1", MemberLoad.SupportSide.Left)]
    public Load TestMemberLoadConstructor(double position, string name, MemberLoad.SupportSide side) {
      LengthUnit length = LengthUnit.Millimeter;

      // 2 create object instance with constructor
      var load = new MemberLoad(name, side, new Length(position, length));

      // 3 check that inputs are set in object's members
      Assert.Equal(position, load.Position.As(LengthUnit.Millimeter));
      Assert.Equal(name, load.MemberName);
      Assert.Equal(side, load.Support);
      Assert.Equal(LoadType.MemberLoad, load.Type);

      return load;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(100, "MEMBER-2", MemberLoad.SupportSide.Right)]
    [InlineData(4000, "MEMBER-1", MemberLoad.SupportSide.Left)]
    public Load TestMemberLoadConstructorPercentage(double position, string name, MemberLoad.SupportSide side) {
      // 2 create object instance with constructor
      var load = new MemberLoad(name, side, new Ratio(position, RatioUnit.Percent));

      // 3 check that inputs are set in object's members
      Assert.Equal(position, load.Position.As(RatioUnit.Percent));
      Assert.Equal(name, load.MemberName);
      Assert.Equal(side, load.Support);
      Assert.Equal(LoadType.MemberLoad, load.Type);

      return load;
    }
  }
}
