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
      TestObject grandChild = new TestObject(1.0, 1, "a", TestEnum.Value1, quantity, new List<TestObject>());
      TestObject original = new TestObject(new TestObject(grandChild));

      object o = original.Duplicate();
      TestObject duplicate = original.Duplicate() as TestObject;

      original.Children[0].Children[0].D = -1.0;
      original.Children[0].Children[0].I = -1;
      original.Children[0].Children[0].S = "z";
      original.Children[0].Children[0].TestEnum = TestEnum.None;
      original.Children[0].Children[0].Quantity = new Pressure(-1.0, PressureUnit.KilonewtonPerSquareMeter);

      Assert.Equal(1.0, duplicate.Children[0].Children[0].D);
      Assert.Equal(1, duplicate.Children[0].Children[0].I);
      Assert.Equal("a", duplicate.Children[0].Children[0].S);
      Assert.Equal(TestEnum.Value1, duplicate.Children[0].Children[0].TestEnum);
      Assert.Equal(quantity, duplicate.Children[0].Children[0].Quantity);
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
    internal double D { get; set; }
    internal int I { get; set; }
    internal string S { get; set; }
    internal TestEnum TestEnum { get; set; }
    internal IQuantity Quantity { get; set; }
    internal List<TestObject> Children { get; set; } = new List<TestObject>();

    public TestObject() { }

    public TestObject(TestObject child)
    {
      this.Children = new List<TestObject>() { child };
    }

    public TestObject(List<TestObject> children)
    {
      this.Children = children;
    }

    internal TestObject(double d, int i, string s, TestEnum testEnum, IQuantity quantity, List<TestObject> children)
    {
      this.D = d;
      this.I = i;
      this.S = s;
      this.TestEnum = testEnum;
      this.Quantity = quantity;
      this.Children = children;
    }
  }
}
