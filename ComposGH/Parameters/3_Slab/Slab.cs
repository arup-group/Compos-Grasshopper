using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Documentation;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using Rhino.Collections;
using UnitsNet;
using UnitsNet.Units;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class SlabGoo : GH_Goo<ISlab>
  {
    #region constructors
    public SlabGoo()
    {
      this.Value = new Slab();
    }

    public SlabGoo(ISlab item)
    {
      if (item == null)
        item = new Slab();
      this.Value = item.Duplicate() as ISlab;
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public SlabGoo DuplicateGoo()
    {
      return new SlabGoo(this.Value == null ? new Slab() : this.Value.Duplicate() as ISlab);
    }
    #endregion

    #region properties
     public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "Slab";
    public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
    public override string IsValidWhyNot
    {
      get
      {
        if (IsValid) { return string.Empty; }
        return IsValid.ToString(); //Todo: beef this up to be more informative.
      }
    }
    public override string ToString()
    {
      if (this.Value == null)
        return "Null";
      else
        return "Compos " + this.TypeName + " {" + this.Value.ToString() + "}"; ;
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this instance of our custom class into some other type Q.            
      if (typeof(Q).IsAssignableFrom(typeof(Slab)))
      {
        if (this.Value == null)
          target = default;
        else
          target = (Q)(object)this.Value;
        return true;
      }

      target = default;
      return false;
    }

    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data into our custom class.
      if (source == null) { return false; }

      if (typeof(Slab).IsAssignableFrom(source.GetType()))
      {
        this.Value = (Slab)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
