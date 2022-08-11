using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComposAPI.Tests;
using UnitsNet;
using Xunit;
using ComposGHTests.Helpers;


namespace ComposAPI.Members.Tests
{
  public class MemberTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData("MEMBER-1")]
    public Member ConstructorTest1(string name)
    {
      // 2 create object instance with constructor
      IBeam beam = new Beam();
      IStud stud = new Stud();
      ISlab slab = new Slab();
      IList<ILoad> loads = new List<ILoad>() { new Load() };
      IDesignCode designCode = new DesignCode();
      IDesignCriteria designCriteria = new DesignCriteria();

      Member member = new Member(name, designCode, beam, stud, slab, loads, designCriteria);

      // 3 check that inputs are set in object's members
      Assert.Equal(name, member.Name);
      Assert.Equal("", member.GridReference);
      Assert.Equal("", member.Note);
      Assert.Equal(designCode, member.DesignCode);
      Assert.Equal(beam, member.Beam);
      Assert.Equal(slab, member.Slab);
      Assert.Equal(loads, member.Loads);
      Assert.Equal(designCriteria, member.DesignCriteria);

      return member;
    }

    [Fact]
    public void DuplicateTest()
    {
      // 1 create with constructor and duplicate
      Member original = ConstructorTest1("MEMBER-1");
      Member duplicate = (Member)original.Duplicate();

      // 2 check that duplicate has duplicated values
      ObjectExtensionTest.IsEqual(original, duplicate, true); // exclude testing GUIDs are equal

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
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
      IDesignCriteria designCriteria = new DesignCriteria();

      Member member = new Member(name, gridRef, note, designCode, beam, stud, slab, loads, designCriteria);

      // 3 check that inputs are set in object's members
      Assert.Equal(name, member.Name);
      Assert.Equal(gridRef, member.GridReference);
      Assert.Equal(note, member.Note);
      Assert.Equal(designCode, member.DesignCode);
      Assert.Equal(beam, member.Beam);
      Assert.Equal(slab, member.Slab);
      Assert.Equal(loads, member.Loads);
      Assert.Equal(designCriteria, member.DesignCriteria);
    }
  }
}
