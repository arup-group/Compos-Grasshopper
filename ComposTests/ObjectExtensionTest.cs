using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Tests
{
  public class ObjectExtensionTest
  {
    [Fact]
    public void DuplicateTest1()
    {
      Force quantity = new Force(1, ForceUnit.Kilonewton);
      Force force = new Force(2, ForceUnit.Decanewton);
      List<IQuantity> iQuantities = new List<IQuantity>() { Force.Zero, new Length(100, LengthUnit.Millimeter) };
      List<Length> structs = new List<Length>() { Length.Zero, new Length(100, LengthUnit.Millimeter) };

      TestObject grandChild = new TestObject(true, 1.0, 1, "a", TestEnum.Value1, quantity, force, new List<TestObject>(), iQuantities, structs);
      TestObject original = new TestObject(new TestObject(grandChild));

      TestObject duplicate = original.Duplicate() as TestObject;

      duplicate.Children[0].Children[0].B = false;
      duplicate.Children[0].Children[0].D = -1.0;
      duplicate.Children[0].Children[0].I = -1;
      duplicate.Children[0].Children[0].S = "z";
      duplicate.Children[0].Children[0].TestEnum = TestEnum.None;
      duplicate.Children[0].Children[0].IQuantity = new Pressure(-1.0, PressureUnit.KilonewtonPerSquareMeter);
      duplicate.Children[0].Children[0].Force = new Force(-1.0, ForceUnit.Dyn);
      duplicate.Children[0].Children[0].IQuantities.AddRange(new List<IQuantity>() { Force.Zero, new Length(100, LengthUnit.Millimeter) });
      duplicate.Children[0].Children[0].IQuantities.RemoveAt(0);

      Assert.Equal(true, original.Children[0].Children[0].B);
      Assert.Equal(1.0, original.Children[0].Children[0].D);
      Assert.Equal(1, original.Children[0].Children[0].I);
      Assert.Equal("a", original.Children[0].Children[0].S);
      Assert.Equal(TestEnum.Value1, original.Children[0].Children[0].TestEnum);
      Assert.Equal(quantity, original.Children[0].Children[0].IQuantity);
      Assert.Equal(force, original.Children[0].Children[0].Force);
      Assert.Equal(iQuantities, original.Children[0].Children[0].IQuantities);
      Assert.Equal(structs, original.Children[0].Children[0].Structs);
    }
  }

  enum TestEnum
  {
    Value1 = 1,
    Value2 = 2,
    Value3 = 3,
    None = -1
  }

  public class TestObject
  {
    internal bool B { get; set; }
    internal double D { get; set; }
    internal int I { get; set; }
    internal string S { get; set; }
    internal TestEnum TestEnum { get; set; }
    internal IQuantity IQuantity { get; set; }
    internal Force Force { get; set; }
    internal List<TestObject> Children { get; set; } = new List<TestObject>();
    internal List<IQuantity> IQuantities { get; set; } = new List<IQuantity>();
    internal List<Length> Structs { get; set; } = new List<Length>();

    public TestObject() { }

    public TestObject(TestObject child)
    {
      this.Children = new List<TestObject>() { child };
    }

    public TestObject(List<TestObject> children)
    {
      this.Children = children;
    }

    internal TestObject(bool b, double d, int i, string s, TestEnum testEnum, IQuantity quantity, Force force, List<TestObject> children, List<IQuantity> iQuantities, List<Length> structs)
    {
      this.B = b;
      this.D = d;
      this.I = i;
      this.S = s;
      this.TestEnum = testEnum;
      this.IQuantity = quantity;
      this.Force = force;
      this.Children = children;
      this.IQuantities = iQuantities;
      this.Structs = structs;
    }
  }
}
