using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System.Drawing;
using Grasshopper;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// GeometryGoo wrapper class, makes sure our custom class can be used and displayed in Grasshopper.
  /// </summary>
  public class MemberGoo : GH_GeometricGoo<IMember>, IGH_PreviewData
  {
    //public LineCurve Line { get; set; }
    //LengthUnit LengthUnit { get; set; }

    #region constructors
    public MemberGoo()
    {
      this.Value = new Member();
    }
    public MemberGoo(IMember item)
    {
      if (item == null)
        item = new Member();
      this.Value = item; //.Duplicate() as SafetyFactors;
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public MemberGoo DuplicateGoo()
    {
      return new MemberGoo(Value == null ? new Member() : Value);// .Duplicate() as SafetyFactors);
    }

    public override IGH_GeometricGoo DuplicateGeometry()
    {
      if (Value == null)
        return null;
      else
        return (IGH_GeometricGoo)this.Duplicate();
    }
    #endregion

    #region properties
    public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "Member";
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

    public override BoundingBox Boundingbox
    {
      get
      {
        return BoundingBox.Empty;
        //if (Value == null) { return BoundingBox.Empty; }
        //if (Line == null) { return BoundingBox.Empty; }
        //return Line.GetBoundingBox(false);
      }
    }
    public override BoundingBox GetBoundingBox(Transform xform)
    {
      return BoundingBox.Empty;
      //if (Value == null) { return BoundingBox.Empty; }
      //if (Line == null) { return BoundingBox.Empty; }
      //return Line.GetBoundingBox(xform);
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of our custom class into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(Member)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      ////Cast to Curve
      //if (typeof(Q).IsAssignableFrom(typeof(Line)))
      //{
      //  if (Value == null)
      //    target = default;
      //  else
      //    target = (Q)(object)this.Line;
      //  return true;
      //}
      //if (typeof(Q).IsAssignableFrom(typeof(GH_Line)))
      //{
      //  if (Value == null)
      //    target = default;
      //  else
      //  {
      //    GH_Line ghLine = new GH_Line();
      //    GH_Convert.ToGHLine(this.Line, GH_Conversion.Both, ref ghLine);
      //    target = (Q)(object)ghLine;
      //  }

      //  return true;
      //}
      //if (typeof(Q).IsAssignableFrom(typeof(Curve)))
      //{
      //  if (Value == null)
      //    target = default;
      //  else
      //    target = (Q)(object)this.Line;
      //  return true;
      //}
      //if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
      //{
      //  if (Value == null)
      //    target = default;
      //  else
      //  {
      //    target = (Q)(object)new GH_Curve(this.Line);
      //  }
      //  return true;
      //}

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into our custom class.

      if (source == null) { return false; }

      //Cast from GsaMaterial
      if (typeof(Member).IsAssignableFrom(source.GetType()))
      {
        Value = (Member)source;
        return true;
      }

      try
      {
        //// Cast from GsaGH
        //if (GsaGHConverter.IsPresent())
        //{
        //  Type type = GsaGHConverter.GetTypeFor(typeof(IComposMember));
        //  if (type.IsAssignableFrom(source.GetType()))
        //  {
        //    Value = (Member)GsaGHConverter.CastToComposBeam(source);
        //    return true;
        //  }
        //}
        //// Cast from AdSecGH
        //if (AdSecGHConverter.IsPresent())
        //{
        //  Type type = AdSecGHConverter.GetTypeFor(typeof(IComposMember));
        //  if (type.IsAssignableFrom(source.GetType()))
        //  {
        //    Value = (Member)AdSecGHConverter.CastToComposBeam(source);
        //    return true;
        //  }
        //}
        //// Cast from Speckle
        //if (SpeckleConverter.IsPresent())
        //{
        //  // todo: implement
        //}

      }
      catch (Exception)
      {
        return false;
      }

      return false;
    }
    #endregion

    #region transformation methods
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      return null;


      //if (Value == null) { return null; }
      //if (Line == null) { return null; }

      //BeamGoo dup = new BeamGoo(this);
      //LineCurve xLn = dup.Line;
      //xLn.Transform(xform);
      //dup.Line = xLn;
      //dup.UpdatePreview();

      //return dup;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      return null;

      //if (Value == null) { return null; }
      //if (Line == null) { return null; }

      //BeamGoo dup = new BeamGoo(this);
      //LineCurve xLn = dup.Line;
      //xmorph.Morph(xLn);
      //dup.Line = xLn;
      //dup.LengthUnit = this.LengthUnit;
      //dup.UpdatePreview();

      //return dup;
    }

    #endregion

    #region preview geometry
    List<PolyCurve> profileOutlines;
    List<Brep> profileExtrusions;
    List<Brep> stiffenerPlates;
    void UpdatePreview()
    {

    }
    #endregion

    #region drawing methods
    public BoundingBox ClippingBox
    {
      get { return Boundingbox; }
    }
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      ////Draw shape.
      //if (profileExtrusions != null)
      //{
      //  if (args.Material.Diffuse == Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
      //  {
      //    foreach (Brep brep in profileExtrusions)
      //      args.Pipeline.DrawBrepShaded(brep, UI.Colour.Steel); //UI.Colour.Member2dFace
      //  }
      //  else
      //  {
      //    foreach (Brep brep in profileExtrusions)
      //      args.Pipeline.DrawBrepShaded(brep, UI.Colour.Steel);
      //  }
      //}
    }
    Color defaultCol = Instances.Settings.GetValue("DefaultPreviewColour", Color.White);
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }

      ////Draw lines
      //if (Line != null)
      //{
      //  if (args.Color.R == defaultCol.R && args.Color.G == defaultCol.G && args.Color.B == defaultCol.B) // not selected
      //  {
      //    // line
      //    args.Pipeline.DrawCurve(Line, UI.Colour.OasysBlue, 3);

      //    // profiles
      //    foreach (PolyCurve crv in profileOutlines)
      //      args.Pipeline.DrawCurve(crv, UI.Colour.OasysBlue, 2);


      //  }
      //  else // selected
      //  {
      //    // line
      //    args.Pipeline.DrawCurve(Line, UI.Colour.ArupRed, 4);

      //    // profiles
      //    foreach (PolyCurve crv in profileOutlines)
      //      args.Pipeline.DrawCurve(crv, UI.Colour.ArupRed, 3);
      //  }
      //  // extrusion wirefram
      //  foreach (Brep brep in profileExtrusions)
      //    args.Pipeline.DrawBrepWires(brep, UI.Colour.OasysDarkGrey, -1);
    }
    #endregion
  }

  /// <summary>
  /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class ComposMemberParameter : GH_PersistentGeometryParam<MemberGoo>, IGH_PreviewObject
  {
    public ComposMemberParameter()
      : base(new GH_InstanceDescription("Member", "Mem", "Maintains a collection of Compos Member data.", Components.Ribbon.CategoryName.Name(), Components.Ribbon.SubCategoryName.Cat10()))
    {
    }

    public override Guid ComponentGuid => new Guid("a94f9373-e1a3-49d9-9b98-d3a2618fb9f8");

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override Bitmap Icon => Properties.Resources.MemberParam;

    //We do not allow users to pick parameter, 
    //therefore the following 4 methods disable all this ui.
    protected override GH_GetterResult Prompt_Plural(ref List<MemberGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref MemberGoo value)
    {
      return GH_GetterResult.cancel;
    }
    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
    {
      System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
      {
        Text = "Not available",
        Visible = false
      };
      return item;
    }
    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
    {
      System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
      {
        Text = "Not available",
        Visible = false
      };
      return item;
    }

    #region preview methods
    public BoundingBox ClippingBox
    {
      get
      {
        return Preview_ComputeClippingBox();
      }
    }
    public void DrawViewportMeshes(IGH_PreviewArgs args)
    {
      //Use a standard method to draw gunk, you don't have to specifically implement this.
      Preview_DrawMeshes(args);
    }
    public void DrawViewportWires(IGH_PreviewArgs args)
    {
      //Use a standard method to draw gunk, you don't have to specifically implement this.
      Preview_DrawWires(args);
    }

    private bool m_hidden = false;
    public bool Hidden
    {
      get { return m_hidden; }
      set { m_hidden = value; }
    }
    public bool IsPreviewCapable
    {
      get { return true; }
    }
    #endregion
  }
}

