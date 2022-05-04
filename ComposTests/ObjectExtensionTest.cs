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
  internal static class ChildMother
  {


    internal static Child CreateChild2()
    {
      Child child = new Child(2.0, 2, "b", TestEnum.Value2, new Force(2, ForceUnit.Kilonewton));

      return child;
    }
  }

  public class ObjectExtensionTest
  {



    [Fact]
    void DuplicateTest1()
    {
      Force quantity = new Force(1, ForceUnit.Kilonewton);
      Child child = new Child(1.0, 1, "a", TestEnum.Value1, quantity);

      GrandParent original = new GrandParent(new Parent(child));
      GrandParent duplicate = original.Duplicate() as GrandParent;

      //original.Parent.Child.D = -1.0;
      //original.Parent.Child.I = -1;
      //original.Parent.Child.S = "z";
      //original.Parent.Child.TestEnum = TestEnum.None;
      original.Parent.Child.Quantity = new Pressure(-1.0, PressureUnit.KilonewtonPerSquareMeter);

      //Assert.Equal(1.0, duplicate.Parent.Child.D);
      //Assert.Equal(1, duplicate.Parent.Child.I);
      //Assert.Equal("a", duplicate.Parent.Child.S);
      //Assert.Equal(TestEnum.Value1, duplicate.Parent.Child.TestEnum);
      Assert.Equal(quantity, duplicate.Parent.Child.Quantity);
    }

  }

  enum TestEnum
  {
    Value1 = 1,
    Value2 = 2,
    Value3 = 3,
    None = -1
  }

  public class Child
  {
    //internal double D { get; set; }
    //internal int I { get; set; }
    //internal string S { get; set; }
    //internal TestEnum TestEnum { get; set; }
    internal IQuantity Quantity { get; set; }

    public Child() { }

    internal Child(double d, int i, string s, TestEnum testEnum, IQuantity quantity)
    {
      //this.D = d;
      //this.I = i;
      //this.S = s;
      //this.TestEnum = testEnum;
      this.Quantity = quantity;
    }
  }

  public class Parent : Child
  {
    internal Child Child { get; set; }

    public Parent() { }

    internal Parent(Child child)
    {
      this.Child = child;
    }
  }

  public class GrandParent : Parent
  {
    internal Parent Parent { get; set; }

    public GrandParent() { }  

    internal GrandParent(Parent parent)
    {
      this.Parent = parent;
    }
  }
}
