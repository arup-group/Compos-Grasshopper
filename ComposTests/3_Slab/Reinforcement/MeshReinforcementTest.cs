﻿using System.Collections.Generic;
using ComposAPI.Helpers;
using ComposGH.Helpers;
using ComposGHTests.Helper;
using OasysGH;
using OasysUnits;
using Xunit;

namespace ComposAPI.Slabs.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class MeshReinforcementTest {

    // 1 setup inputs
    [Theory]
    [InlineData(35)]
    public MeshReinforcement ConstructorTest1(double cover) {
      var units = ComposUnits.GetStandardUnits();

      // 2 create object instance with constructor
      var mesh = new MeshReinforcement(new Length(cover, units.Length));

      // 3 check that inputs are set in object's members
      Assert.Equal(cover, mesh.Cover.Value);
      Assert.Equal(ReinforcementMeshType.A393, mesh.MeshType);
      Assert.False(mesh.Rotated);

      return mesh;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(0, ReinforcementMeshType.A142, false)]
    [InlineData(10, ReinforcementMeshType.A193, true)]
    [InlineData(20, ReinforcementMeshType.A252, false)]
    [InlineData(30, ReinforcementMeshType.A393, true)]
    [InlineData(35, ReinforcementMeshType.A98, false)]
    public void ConstructorTest2(double cover, ReinforcementMeshType meshType, bool rotated) {
      var units = ComposUnits.GetStandardUnits();

      // 2 create object instance with constructor
      var mesh = new MeshReinforcement(new Length(cover, units.Length), meshType, rotated);

      // 3 check that inputs are set in object's members
      Assert.Equal(cover, mesh.Cover.Value);
      Assert.Equal(meshType, mesh.MeshType);
      Assert.Equal(rotated, mesh.Rotated);
    }

    [Fact]
    public void DuplicateTest() {
      // 1 create with constructor and duplicate
      MeshReinforcement original = ConstructorTest1(30);
      var duplicate = (MeshReinforcement)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Theory]
    [InlineData("REBAR_WESH	MEMBER-1	A142	35.0000	PARALLEL\n", 35, ReinforcementMeshType.A142, false)]
    [InlineData("REBAR_WESH	MEMBER-1	A193	35.0000	PARALLEL\n", 35, ReinforcementMeshType.A193, false)]
    [InlineData("REBAR_WESH	MEMBER-1	A252	35.0000	PARALLEL\n", 35, ReinforcementMeshType.A252, false)]
    [InlineData("REBAR_WESH	MEMBER-1	A393	35.0000	PARALLEL\n", 35, ReinforcementMeshType.A393, false)]
    [InlineData("REBAR_WESH	MEMBER-1	A98	35.0000	PARALLEL\n", 35, ReinforcementMeshType.A98, false)]
    [InlineData("REBAR_WESH	MEMBER-1	B1131	35.0000	PARALLEL\n", 35, ReinforcementMeshType.B1131, false)]
    [InlineData("REBAR_WESH	MEMBER-1	B196	35.0000	PARALLEL\n", 35, ReinforcementMeshType.B196, false)]
    [InlineData("REBAR_WESH	MEMBER-1	B283	35.0000	PARALLEL\n", 35, ReinforcementMeshType.B283, false)]
    [InlineData("REBAR_WESH	MEMBER-1	B385	35.0000	PARALLEL\n", 35, ReinforcementMeshType.B385, false)]
    [InlineData("REBAR_WESH	MEMBER-1	B503	35.0000	PARALLEL\n", 35, ReinforcementMeshType.B503, false)]
    [InlineData("REBAR_WESH	MEMBER-1	B785	35.0000	PARALLEL\n", 35, ReinforcementMeshType.B785, false)]
    [InlineData("REBAR_WESH	MEMBER-1	C283	35.0000	PARALLEL\n", 35, ReinforcementMeshType.C283, false)]
    [InlineData("REBAR_WESH	MEMBER-1	C385	35.0000	PARALLEL\n", 35, ReinforcementMeshType.C385, false)]
    [InlineData("REBAR_WESH	MEMBER-1	C503	35.0000	PERPENDICULAR\n", 35, ReinforcementMeshType.C503, true)]
    [InlineData("REBAR_WESH	MEMBER-1	C636	50.0000	PERPENDICULAR\n", 50, ReinforcementMeshType.C636, true)]
    [InlineData("REBAR_WESH	MEMBER-1	C785	100.000	PERPENDICULAR\n", 100, ReinforcementMeshType.C785, true)]
    public void FromCoaStringTest(string coaString, double expected_cover, ReinforcementMeshType expected_meshType, bool expected_rotated) {
      List<string> parameters = CoaHelper.Split(coaString);
      var units = ComposUnits.GetStandardUnits();

      IMeshReinforcement reinforcement = MeshReinforcement.FromCoaString(parameters, units);

      Assert.Equal(expected_cover, reinforcement.Cover.Value);
      Assert.Equal(expected_meshType, reinforcement.MeshType);
      Assert.Equal(expected_rotated, reinforcement.Rotated);
    }

    [Theory]
    [InlineData(35, ReinforcementMeshType.A142, false, "REBAR_WESH	MEMBER-1	A142	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.A193, false, "REBAR_WESH	MEMBER-1	A193	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.A252, false, "REBAR_WESH	MEMBER-1	A252	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.A393, false, "REBAR_WESH	MEMBER-1	A393	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.A98, false, "REBAR_WESH	MEMBER-1	A98	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.B1131, false, "REBAR_WESH	MEMBER-1	B1131	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.B196, false, "REBAR_WESH	MEMBER-1	B196	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.B283, false, "REBAR_WESH	MEMBER-1	B283	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.B385, false, "REBAR_WESH	MEMBER-1	B385	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.B503, false, "REBAR_WESH	MEMBER-1	B503	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.B785, false, "REBAR_WESH	MEMBER-1	B785	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.C283, false, "REBAR_WESH	MEMBER-1	C283	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.C385, false, "REBAR_WESH	MEMBER-1	C385	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.C503, true, "REBAR_WESH	MEMBER-1	C503	35.0000	PERPENDICULAR\n")]
    [InlineData(50, ReinforcementMeshType.C636, true, "REBAR_WESH	MEMBER-1	C636	50.0000	PERPENDICULAR\n")]
    [InlineData(100, ReinforcementMeshType.C785, true, "REBAR_WESH	MEMBER-1	C785	100.000	PERPENDICULAR\n")]
    public void ToCoaStringTest(double cover, ReinforcementMeshType meshType, bool rotated, string expected_coaString) {
      var units = ComposUnits.GetStandardUnits();
      IMeshReinforcement reinforcement = new MeshReinforcement(new Length(cover, units.Length), meshType, rotated);

      string coaString = reinforcement.ToCoaString("MEMBER-1", units);

      Assert.Equal(expected_coaString, coaString);
    }
  }
}
