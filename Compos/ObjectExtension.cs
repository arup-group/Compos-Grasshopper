using System;
using System.Collections;
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

      // return here if source is struct
      if (typeSource.IsValueType)
      {
        objTarget = objSource;
        return objTarget;
      }

      // get all the properties of source object type
      PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      // assign all source property to taget object's properties
      foreach (PropertyInfo property in propertyInfo)
      {
        // check whether property can be written to
        if (property.CanWrite)
        {
          object objPropertyValue;
          Type propertyType = property.PropertyType;
          try
          {
            objPropertyValue = property.GetValue(objSource, null);

            // check wether property is an interface
            if (propertyType.IsInterface)
            {
              if (objPropertyValue != null)
                propertyType = objPropertyValue.GetType();
            }

            // check wether property is an enumerable
            if (typeof(IEnumerable).IsAssignableFrom(propertyType) && !typeof(string).IsAssignableFrom(propertyType))
            {
              if (objPropertyValue == null)
              {
                property.SetValue(objTarget, null, null);
              }
              else
              {
                IEnumerable<object> enumerable = ((IEnumerable)objPropertyValue).Cast<object>();
                Type enumrableType = enumerable.GetType().GetGenericArguments()[0];

                // if type is a struct, we have to check the actual list items
                if (enumrableType.ToString() is "System.Object")
                {
                  if (enumerable.Any())
                  {
                    enumrableType = enumerable.First().GetType();
                  }
                }

                Type genericListType = typeof(List<>).MakeGenericType(enumrableType);

                IList list = (IList)Activator.CreateInstance(genericListType);
                using (var enumerator = enumerable.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    list.Add(enumerator.Current.Duplicate());
                  }
                }
                property.SetValue(objTarget, list, null);
              }
            }

            // check whether property type is value type, enum or string type
            else if (propertyType.IsValueType || propertyType.IsEnum || propertyType.Equals(typeof(System.String)))
            {
              property.SetValue(objTarget, objPropertyValue, null);
            }
            // else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
            else
            {
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
          catch (TargetParameterCountException ex)
          {
            propertyType = property.PropertyType;
          }
        }
      }
      return objTarget;
    }
  }

}
