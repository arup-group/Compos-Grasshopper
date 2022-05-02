using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino;
using Grasshopper.Documentation;
using Rhino.Collections;
using UnitsNet;
using UnitsNet.Units;
using System.Globalization;
using System.IO;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class BeamSectionGoo : GH_Goo<BeamSection>
  {
    #region constructors
    public BeamSectionGoo()
    {
      this.Value = new BeamSection();
    }
    public BeamSectionGoo(BeamSection item)
    {
      if (item == null)
        item = new BeamSection();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public BeamSectionGoo DuplicateGoo()
    {
      return new BeamSectionGoo(Value == null ? new BeamSection() : Value.Duplicate());
    }
    #endregion

    #region properties
     public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "Beam Section";
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
      if (Value == null)
        return "Null";
      else
        return "Compos " + TypeName + " {" + Value.ToString() + "}"; ;
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of our custom class into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(BeamSection)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(string)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.SectionDescription;
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

      //Cast from custom type
      if (typeof(BeamSection).IsAssignableFrom(source.GetType()))
      {
        Value = (BeamSection)source;
        return true;
      }

      //Cast from string
      if (GH_Convert.ToString(source, out string mystring, GH_Conversion.Both))
      {
        Value = new BeamSection(mystring);
        return true;
      }
      return false;
    }
    #endregion
  }
}
