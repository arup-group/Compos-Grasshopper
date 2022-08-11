using System;
using ComposAPI;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using Xunit;
using ComposGHTests.Helpers;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Grasshopper.Kernel.Types;

namespace ComposGHTests
{
  public class GH_OasysGooTest
  {
    [Theory]
    [InlineData(typeof(SupportsGoo), typeof(Supports))]
    //[InlineData(typeof(BeamGoo), typeof(Beam))]
    [InlineData(typeof(StudGoo), typeof(Stud))]
    public void ConstructorTest(Type gooType, Type wrapType)
    {
      // Supports
      object value = Activator.CreateInstance(wrapType);
      object[] parameters = { value };

      // GH_OasysGoo<ISupports> 
      object objectGoo = Activator.CreateInstance(gooType, parameters);

      Type typeSource = objectGoo.GetType();
      PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      foreach (PropertyInfo property in propertyInfo)
      {
        if (property.Name == "Value")
        {
          object objPropertyValue = property.GetValue(objectGoo, null);
          Duplicates.AreEqual(value, objPropertyValue);
        }
      }
    }
  }
}
