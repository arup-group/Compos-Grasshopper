using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ComposAPI;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPITests
{
  public class ObjectExtensionTest
  {



    [Fact]
    void DuplicateTest1()
    {
      Child child = new Child(1.0, 1, "a", TestEnum.Value1, new Force(1, ForceUnit.Kilonewton));
      Parent parent = new Parent(child);
      GrandParent grandParent = new GrandParent(parent);

      GrandParent duplicateGrandParent = grandParent.Duplicate() as GrandParent;


    }

  }

  enum TestEnum
  {
    Value1 = 1,
    Value2 = 2,
    Value3 = 3
  }

  internal class Child
  {
    private double D { get; set; }
    private int I { get; set; }
    private string S { get; set; }
    private TestEnum TestEnum { get; set; }
    private IQuantity Quantity { get; set; }

    internal Child() { }

    internal Child(double d, int i, string s, TestEnum testEnum, IQuantity quantity)
    {
      this.D = d;
      this.I = i;
      this.S = s;
      this.TestEnum = testEnum;
      this.Quantity = quantity;
    }
  }

  internal class Parent : Child
  {
    private Child Child { get; set; }

    internal Parent() { }

    internal Parent(Child child)
    {
      this.Child = child;
    }
  }

  internal class GrandParent : Parent
  {
    internal Parent Parent { get; set; }  

    internal GrandParent() { }  

    internal GrandParent(Parent parent)
    {
      this.Parent = parent;
    }
  }
}
