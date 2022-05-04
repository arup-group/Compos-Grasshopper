﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ComposAPI
{
  public static class ObjectExtension
  {
    public static object Duplicate(this object objSource)
    {
      // get the type of source object and create a new instance of that type
      Type typeSource = objSource.GetType();
      object objTarget = Activator.CreateInstance(typeSource);
      // get all the properties of source object type
      PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      // assign all source property to taget object's properties
      foreach (PropertyInfo property in propertyInfo)
      {
        // check whether property can be written to
        if (property.CanWrite)
        {
          object value = property.GetValue(objSource, null);
          Type propertyType;
          if (value != null)
            propertyType = value.GetType();
          else
            propertyType = property.PropertyType;

          // check whether property type is value type, enum or string type
          if (propertyType.IsValueType || propertyType.IsEnum || propertyType.Equals(typeof(System.String)))
          {
            property.SetValue(objTarget, property.GetValue(objSource, null), null);
          }
          // else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
          else
          {
            object objPropertyValue = property.GetValue(objSource, null);
            if (objPropertyValue == null)
            {
              property.SetValue(objTarget, null, null);
            }
            else
            {
              property.SetValue(objTarget, objPropertyValue.Duplicate(), null);
            }
          }
        }
      }
      return objTarget;
    }
  }

}
