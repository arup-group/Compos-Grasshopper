﻿using ComposAPI;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComposGH.Parameters
{
  public abstract class GH_OasysGoo<T> : GH_Goo<T>
  {
    public override string TypeName => typeof(T).Name.TrimStart(new char[] {'I'});
    public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
    public override bool IsValid => (this.Value == null) ? false : true;
    public override string IsValidWhyNot
    {
      get
      {
        if (IsValid) { return string.Empty; }
        return IsValid.ToString();
      }
    }
    public GH_OasysGoo(T item)
    {
      if (item == null)
        this.Value = item;
      else
        this.Value = (T)item.Duplicate();
    }
    public override string ToString()
    {
      if (Value == null)
        return "Null";
      else
        return "Compos " + TypeName + " {" + Value.ToString() + "}"; ;
    }

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of our custom class into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(T)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into our custom class.

      if (source == null) { return false; }

      //Cast from this type
      if (typeof(T).IsAssignableFrom(source.GetType()))
      {
        Value = (T)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}