using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Tests
{
  public class MeshReinforcementTest
  {
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
    [InlineData(35, ReinforcementMeshType.B385, false, "REBAR_WESH	MEMBER-1	B385	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.B503, false, "REBAR_WESH	MEMBER-1	B503	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.B785, false, "REBAR_WESH	MEMBER-1	B785	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.C283, false, "REBAR_WESH	MEMBER-1	C283	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.C385, false, "REBAR_WESH	MEMBER-1	C385	35.0000	PARALLEL\n")]
    [InlineData(35, ReinforcementMeshType.C503, true, "REBAR_WESH	MEMBER-1	C503	35.0000	PERPENDICULAR\n")]
    [InlineData(50, ReinforcementMeshType.C636, true, "REBAR_WESH	MEMBER-1	C636	50.0000	PERPENDICULAR\n")]
    [InlineData(100, ReinforcementMeshType.C785, true, "REBAR_WESH	MEMBER-1	C785	100.000	PERPENDICULAR\n")]
    public void ToCoaStringTest(double cover, ReinforcementMeshType meshType, bool rotated, string expected_coaString)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();
      IMeshReinforcement reinforcement = new MeshReinforcement(new Length(cover, units.Length), meshType, rotated);

      string coaString = reinforcement.ToCoaString("MEMBER-1", units);

      Assert.Equal(expected_coaString, coaString);
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
    [InlineData("REBAR_WESH	MEMBER-1	B385	35.0000	PARALLEL\n", 35, ReinforcementMeshType.B385, false)]
    [InlineData("REBAR_WESH	MEMBER-1	B503	35.0000	PARALLEL\n", 35, ReinforcementMeshType.B503, false)]
    [InlineData("REBAR_WESH	MEMBER-1	B785	35.0000	PARALLEL\n", 35, ReinforcementMeshType.B785, false)]
    [InlineData("REBAR_WESH	MEMBER-1	C283	35.0000	PARALLEL\n", 35, ReinforcementMeshType.C283, false)]
    [InlineData("REBAR_WESH	MEMBER-1	C385	35.0000	PARALLEL\n", 35, ReinforcementMeshType.C385, false)]
    [InlineData("REBAR_WESH	MEMBER-1	C503	35.0000	PERPENDICULAR\n", 35, ReinforcementMeshType.C503, true)]
    [InlineData("REBAR_WESH	MEMBER-1	C636	50.0000	PERPENDICULAR\n", 50, ReinforcementMeshType.C636, true)]
    [InlineData("REBAR_WESH	MEMBER-1	C785	100.000	PERPENDICULAR\n", 100, ReinforcementMeshType.C785, true)]
    public void FromCoaStringTest(string coaString, double expected_cover, ReinforcementMeshType expected_meshType, bool expected_rotated)
    {
      List<string> parameters = CoaHelper.Split(coaString);
      ComposUnits units = ComposUnits.GetStandardUnits();

      IMeshReinforcement reinforcement = MeshReinforcement.FromCoaString(parameters, units);

      Assert.Equal(expected_cover, reinforcement.Cover.Value);
      Assert.Equal(expected_meshType, reinforcement.MeshType);
      Assert.Equal(expected_rotated, reinforcement.Rotated);
    }
  }
}
