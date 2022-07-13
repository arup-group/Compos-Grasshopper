using System;
using System.Collections;
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
    public static void Equals(object objA, object objB)
    {
      Type typeA = objA.GetType();
      Type typeB = objB.GetType();

      PropertyInfo[] propertyInfoA = typeA.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      PropertyInfo[] propertyInfoB = typeB.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

      for (int i = 0; i < propertyInfoA.Length; i++)
      {
        PropertyInfo propertyA = propertyInfoA[i];
        PropertyInfo propertyB = propertyInfoB[i];

        if (!propertyA.CanWrite && !propertyB.CanWrite)
          continue;
        else if (!propertyA.CanWrite || !propertyB.CanWrite)
          Assert.Equal(objA, objB);

        object objPropertyValueA;
        object objPropertyValueB;
        Type propertyTypeA = propertyA.PropertyType;
        Type propertyTypeB = propertyB.PropertyType;

        try
        {
          objPropertyValueA = propertyA.GetValue(objA, null);
          objPropertyValueB = propertyB.GetValue(objB, null);

          // check wether property is an interface
          if (propertyTypeA.IsInterface)
          {
            if (objPropertyValueA != null)
              propertyTypeA = objPropertyValueA.GetType();
          }
          if (propertyTypeB.IsInterface)
          {
            if (objPropertyValueB != null)
              propertyTypeB = objPropertyValueB.GetType();
          }

          // check wether property is an enumerable
          if (typeof(IEnumerable).IsAssignableFrom(propertyTypeA) && !typeof(string).IsAssignableFrom(propertyTypeA))
          {
            if (typeof(IEnumerable).IsAssignableFrom(propertyTypeB) && !typeof(string).IsAssignableFrom(propertyTypeB))
            {
              if (objPropertyValueA == null || objPropertyValueB == null)
              {
                Assert.Equal(objPropertyValueA, objPropertyValueB);
              }
              else
              {
                IEnumerable<object> enumerableA = ((IEnumerable)objPropertyValueA).Cast<object>();
                IEnumerable<object> enumerableB = ((IEnumerable)objPropertyValueB).Cast<object>();

                Type[] asdfasdf = enumerableA.GetType().GetGenericArguments();
                Type[] asdfsdfdsf = enumerableB.GetType().GetGenericArguments();

                Type enumrableTypeA = null;
                Type enumrableTypeB = null;
                if (enumerableA.GetType().GetGenericArguments().Length > 0)
                  enumrableTypeA = enumerableA.GetType().GetGenericArguments()[0];
                if (enumerableB.GetType().GetGenericArguments().Length > 0)
                  enumrableTypeB = enumerableB.GetType().GetGenericArguments()[0];
                Assert.Equal(enumrableTypeA, enumrableTypeB);

                // if type is a struct, we have to check the actual list items
                // this will fail if list is actually of type "System.Object"..
                if (enumrableTypeA.ToString() is "System.Object")
                {
                  if (enumerableA.Any())
                    enumrableTypeA = enumerableA.First().GetType();
                  else
                    continue; // can´t get type of struct in empty list? 
                }
                if (enumrableTypeB.ToString() is "System.Object")
                {
                  if (enumerableB.Any())
                    enumrableTypeB = enumerableB.First().GetType();
                  else
                    continue; // can´t get type of struct in empty list? 
                }

                Type genericListTypeA = typeof(List<>).MakeGenericType(enumrableTypeA);
                Type genericListTypeB = typeof(List<>).MakeGenericType(enumrableTypeB);
                Assert.Equal(genericListTypeA, genericListTypeB);

                var enumeratorB = enumerableB.GetEnumerator();

                using (var enumeratorA = enumerableA.GetEnumerator())
                {
                  while (enumeratorA.MoveNext())
                  {
                    Assert.True(enumeratorB.MoveNext());
                    ObjectExtensionTest.Equals(enumeratorA.Current, enumeratorB.Current);
                  }
                }
              }
            }
            else
            {
              Assert.Equal(objPropertyValueA, objPropertyValueB);
            }
          }
          // check whether property type is value type, enum or string type
          else if (propertyTypeA.IsValueType || propertyTypeA.IsEnum || propertyTypeA.Equals(typeof(System.String)))
          {
            Assert.Equal(objPropertyValueA, objPropertyValueB);
          }
          else if (objPropertyValueA == null || objPropertyValueB == null)
          {
            Assert.Equal(objPropertyValueA, objPropertyValueB);
          }
          else
          // property type is object/complex type, so need to recursively call this method until the end of the tree is reached
          {
            ObjectExtensionTest.Equals(objPropertyValueA, objPropertyValueB);
          }
        }
        catch (TargetParameterCountException ex)
        {
          propertyTypeA = propertyA.PropertyType;
        }
      }
    }

    [Fact]
    public void DuplicateTest1()
    {
      Force quantity = new Force(1, ForceUnit.Kilonewton);
      Force force = new Force(2, ForceUnit.Decanewton);
      IList<IQuantity> iQuantities = new List<IQuantity>() { Force.Zero, new Length(100, LengthUnit.Millimeter) };
      IList<Length> structs = new List<Length>() { Length.Zero, new Length(100, LengthUnit.Millimeter) };

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
      duplicate.Children[0].Children[0].IQuantities = new List<IQuantity>() { Force.Zero, new Length(100, LengthUnit.Millimeter) };
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

    [Fact]
    public void EqualityTest()
    {
      Force quantity = new Force(1, ForceUnit.Kilonewton);
      Force force = new Force(2, ForceUnit.Decanewton);
      IList<IQuantity> iQuantities = new List<IQuantity>() { Force.Zero, new Length(100, LengthUnit.Millimeter) };
      IList<Length> structs = new List<Length>() { Length.Zero, new Length(100, LengthUnit.Millimeter) };

      TestObject grandChild = new TestObject(true, 1.0, 1, "a", TestEnum.Value1, quantity, force, new List<TestObject>(), iQuantities, structs);
      TestObject original = new TestObject(new TestObject(grandChild));

      TestObject duplicate = original.Duplicate() as TestObject;

      ObjectExtensionTest.Equals(original, duplicate);
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
    internal IList<TestObject> Children { get; set; } = new List<TestObject>();
    internal IList<IQuantity> IQuantities { get; set; } = new List<IQuantity>();
    internal IList<Length> Structs { get; set; } = new List<Length>();

    public TestObject() { }

    public TestObject(TestObject child)
    {
      this.Children = new List<TestObject>() { child };
    }

    public TestObject(IList<TestObject> children)
    {
      this.Children = children;
    }

    internal TestObject(bool b, double d, int i, string s, TestEnum testEnum, IQuantity quantity, Force force, IList<TestObject> children, IList<IQuantity> iQuantities, IList<Length> structs)
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
