﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ComposAPI;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace ComposGH.Parameters {
  /// <summary>
  /// GeometryGoo wrapper class, makes sure our custom class can be used and displayed in Grasshopper.
  /// </summary>
  public class MemberGoo : GH_GeometricGoo<IMember>, IGH_PreviewData {
    //public LineCurve Line { get; set; }
    //LengthUnit LengthUnit { get; set; }
    public static string Name => "Member";
    public static string NickName => "Mem";
    public static string Description => "Compos Member";

    #region constructors

    public MemberGoo() {
      Value = new Member();
    }

    public MemberGoo(IMember item) {
      if (item == null) {
        item = new Member();
      }
      Value = item;
    }

    public override IGH_Goo Duplicate() {
      return DuplicateGoo();
    }

    public MemberGoo DuplicateGoo() {
      return new MemberGoo(Value ?? new Member());
    }

    public override IGH_GeometricGoo DuplicateGeometry() {
      if (Value == null) {
        return null;
      } else {
        return (IGH_GeometricGoo)Duplicate();
      }
    }

    #endregion

    #region properties
    public override bool IsValid => Value != null;
    public override string TypeName => "Member";
    public override string TypeDescription => "Compos " + TypeName + " Parameter";
    public override string IsValidWhyNot {
      get {
        if (IsValid) { return string.Empty; }
        return IsValid.ToString(); //Todo: beef this up to be more informative.
      }
    }

    public override string ToString() {
      if (Value == null) {
        return "Null";
      } else {
        return "Compos " + TypeName + " {" + Value.ToString() + "}";
      }
    }

    public override BoundingBox Boundingbox => BoundingBox.Empty;

    public override BoundingBox GetBoundingBox(Transform xform) {
      return BoundingBox.Empty;
      //if (Value == null) { return BoundingBox.Empty; }
      //if (Line == null) { return BoundingBox.Empty; }
      //return Line.GetBoundingBox(xform);
    }

    #endregion

    #region casting methods

    public override bool CastTo<Q>(ref Q target) {
      // This function is called when Grasshopper needs to convert this
      // instance of our custom class into some other type Q.

      if (typeof(Q).IsAssignableFrom(typeof(Member))) {
        if (Value == null) {
          target = default;
        } else {
          target = (Q)(object)Value;
        }
        return true;
      }

      ////Cast to Curve
      //if (typeof(Q).IsAssignableFrom(typeof(Line)))
      //{
      //  if (Value == null)
      //    target = default;
      //  else
      //    target = (Q)(object)Line;
      //  return true;
      //}
      //if (typeof(Q).IsAssignableFrom(typeof(GH_Line)))
      //{
      //  if (Value == null)
      //    target = default;
      //  else
      //  {
      //    GH_Line ghLine = new GH_Line();
      //    GH_Convert.ToGHLine(Line, GH_Conversion.Both, ref ghLine);
      //    target = (Q)(object)ghLine;
      //  }

      //  return true;
      //}
      //if (typeof(Q).IsAssignableFrom(typeof(Curve)))
      //{
      //  if (Value == null)
      //    target = default;
      //  else
      //    target = (Q)(object)Line;
      //  return true;
      //}
      //if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
      //{
      //  if (Value == null)
      //    target = default;
      //  else
      //  {
      //    target = (Q)(object)new GH_Curve(Line);
      //  }
      //  return true;
      //}

      target = default;
      return false;
    }

    public override bool CastFrom(object source) {
      // This function is called when Grasshopper needs to convert other data
      // into our custom class.

      if (source == null) { return false; }

      //Cast from GsaMaterial
      if (typeof(Member).IsAssignableFrom(source.GetType())) {
        Value = (Member)source;
        return true;
      }

      try {
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
      } catch (Exception) {
        return false;
      }

      return false;
    }

    #endregion

    #region transformation methods

    public override IGH_GeometricGoo Transform(Transform xform) {
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

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      return null;

      //if (Value == null) { return null; }
      //if (Line == null) { return null; }

      //BeamGoo dup = new BeamGoo(this);
      //LineCurve xLn = dup.Line;
      //xmorph.Morph(xLn);
      //dup.Line = xLn;
      //dup.LengthUnit = LengthUnit;
      //dup.UpdatePreview();

      //return dup;
    }

    #endregion

    #region drawing methods
    public BoundingBox ClippingBox => Boundingbox;

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) {
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

    private Color defaultCol = Instances.Settings.GetValue("DefaultPreviewColour", Color.White);

    public void DrawViewportWires(GH_PreviewWireArgs args) {
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
  public class ComposMemberParameter : GH_PersistentGeometryParam<MemberGoo>, IGH_PreviewObject {

    public ComposMemberParameter()
      : base(new GH_InstanceDescription(
        "Member",
        "Mem",
        "Compos Member parameter",
        Components.Ribbon.CategoryName.Name(),
        Components.Ribbon.SubCategoryName.Cat10())) { }

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty Member parameter" : base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? "Member" : base.TypeName;
    public override Guid ComponentGuid => new Guid("a94f9373-e1a3-49d9-9b98-d3a2618fb9f8");

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override Bitmap Icon => Properties.Resources.MemberParam;

    //We do not allow users to pick parameter,
    //therefore the following 4 methods disable all this ui.
    protected override GH_GetterResult Prompt_Plural(ref List<MemberGoo> values) {
      return GH_GetterResult.cancel;
    }

    protected override GH_GetterResult Prompt_Singular(ref MemberGoo value) {
      return GH_GetterResult.cancel;
    }

    protected override ToolStripMenuItem Menu_CustomSingleValueItem() {
      var item = new ToolStripMenuItem {
        Text = "Not available",
        Visible = false
      };
      return item;
    }

    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem() {
      var item = new ToolStripMenuItem {
        Text = "Not available",
        Visible = false
      };
      return item;
    }

    #region preview methods
    public BoundingBox ClippingBox => Preview_ComputeClippingBox();

    public void DrawViewportMeshes(IGH_PreviewArgs args) {
      //Use a standard method to draw gunk, you don't have to specifically implement
      Preview_DrawMeshes(args);
    }

    public void DrawViewportWires(IGH_PreviewArgs args) {
      //Use a standard method to draw gunk, you don't have to specifically implement
      Preview_DrawWires(args);
    }

    public bool Hidden { get; set; } = false;
    public bool IsPreviewCapable => true;
    #endregion
  }
}
