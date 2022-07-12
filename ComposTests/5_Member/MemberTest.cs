using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using Xunit;

namespace ComposAPI.Members.Tests
{
  public class MemberTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData("MEMBER-1")]
    public void ConstructorTest1(string name)
    {
      // 2 create object instance with constructor
      IBeam beam = new Beam();
      IStud stud = new Stud();
      ISlab slab = new Slab();
      IList<ILoad> loads = new List<ILoad>() { new Load() };
      IDesignCode designCode = new DesignCode();

      Member member = new Member(name, designCode, beam, stud, slab, loads);

      // 3 check that inputs are set in object's members
      Assert.Equal(name, member.Name);
      Assert.Equal("", member.GridReference);
      Assert.Equal("", member.Note);
      Assert.Equal(designCode, member.DesignCode);
      Assert.Equal(beam, member.Beam);
      Assert.Equal(slab, member.Slab);
      Assert.Equal(loads, member.Loads);
    }

    // 1 setup inputs
    [Theory]
    [InlineData("MEMBER-1", "Grid Reference", "Note")]
    public void ConstructorTest2(string name, string gridRef, string note)
    {
      // 2 create object instance with constructor
      IBeam beam = new Beam();
      IStud stud = new Stud();
      ISlab slab = new Slab();
      IList<ILoad> loads = new List<ILoad>() { new Load() };
      IDesignCode designCode = new DesignCode();

      Member member = new Member(name, gridRef, note, designCode, beam, stud, slab, loads);

      // 3 check that inputs are set in object's members
      Assert.Equal(name, member.Name);
      Assert.Equal(gridRef, member.GridReference);
      Assert.Equal(note, member.Note);
      Assert.Equal(designCode, member.DesignCode);
      Assert.Equal(beam, member.Beam);
      Assert.Equal(slab, member.Slab);
      Assert.Equal(loads, member.Loads);
    }
  }
}
