using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Documentation;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Collections;
using Rhino.Geometry;
using UnitsNet;
using UnitsNet.Units;
using Oasys.Units;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class ConcreteMaterialGoo : GH_Goo<ConcreteMaterial>
  {
    #region constructors
    public ConcreteMaterialGoo()
    {
      this.Value = new ConcreteMaterial();
    }
    public ConcreteMaterialGoo(ConcreteMaterial item)
    {
      if (item == null)
        item = new ConcreteMaterial();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public ConcreteMaterialGoo DuplicateGoo()
    {
      return new ConcreteMaterialGoo(this.Value == null ? new ConcreteMaterial() : this.Value.Duplicate());
    }
    #endregion

    #region properties
     public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "Concrete Material";
    public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
    public override string IsValidWhyNot
    {
      get
      {
        if (IsValid) { return string.Empty; }
        return IsValid.ToString(); // todo: beef this up to be more informative
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
      if (typeof(Q).IsAssignableFrom(typeof(ConcreteMaterial)))
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
      // This function is called when Grasshopper needs to convert other data  into our custom class.
      if (source == null) { return false; }

      // Cast from GsaMaterial
      if (typeof(ConcreteMaterial).IsAssignableFrom(source.GetType()))
      {
        this.Value = (ConcreteMaterial)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
