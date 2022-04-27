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
using ComposGH.Converters;
using ComposGH.Helpers;
using UnitsNet.Units;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class ComposBeam
  {
    public LineCurve Line
    {
      get { return (LineCurve)m_line.DuplicateShallow(); }
      set
      {
        m_line = value;
        UpdatePreview();
      }
    }

    LengthUnit LengthUnit { get; set; }

    private LineCurve m_line;
    public Length Length
    {
      get
      {
        if (this.m_line == null)
          throw new Exception("Line has not been set");
        
        return new Length(m_line.GetLength(), LengthUnit);
      }
    }
    public ComposRestraint Restraint { get; set; }
    public ComposSteelMaterial Material { get; set; }
    public List<BeamSection> BeamSections { get; set; } = new List<BeamSection>();
    public List<ComposWebOpening> WebOpenings { get; set; } = null;

    #region constructors
    public ComposBeam()
    {
      // empty constructor
    }

    public ComposBeam(LineCurve line, LengthUnit lengthUnit, ComposRestraint restraint, ComposSteelMaterial material, List<BeamSection> beamSections, List<ComposWebOpening> webOpenings = null)
    {
      this.m_line = line;
      this.LengthUnit = lengthUnit;
      this.Restraint = restraint;
      this.Material = material;
      this.BeamSections = beamSections;
      this.WebOpenings = webOpenings;
      this.UpdatePreview();
    }

    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        return true;
      }
    }
    #endregion

    #region coa interop
    internal ComposBeam(string coaString)
    {
      // to do - implement from coa string method
    }

    internal string ToCoaString()
    {
      // to do - implement to coa string method
      return string.Empty;
    }
    #endregion

    #region methods

    public ComposBeam Duplicate()
    {
      if (this == null) { return null; }
      ComposBeam dup = (ComposBeam)this.MemberwiseClone();
      dup.Material = this.Material.Duplicate();
      dup.Restraint = this.Restraint.Duplicate();
      dup.BeamSections = this.BeamSections.ToList();
      if (this.WebOpenings != null)
        dup.WebOpenings = this.WebOpenings.ToList();
      dup.Line = this.Line; // Get the public member will shallow copy the object
      return dup;
    }

    public override string ToString()
    {
      string profile = (this.BeamSections.Count > 1) ? " multiple sections" : this.BeamSections[0].SectionDescription;
      string mat = this.Material.ToString();
      string line = "L:" + this.Length.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
      return line + ", " + profile + ", " + mat;
    }
    #endregion

    #region preview geometry
    private 
    internal void UpdatePreview()
    {
      // to do
    }
    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class ComposBeamGoo : GH_GeometricGoo<ComposBeam>, IGH_PreviewData
  {
    #region constructors
    public ComposBeamGoo()
    {
      this.Value = new ComposBeam();
    }
    public ComposBeamGoo(ComposBeam item)
    {
      if (item == null)
        item = new ComposBeam();
      this.Value = item.Duplicate();
    }

    public override IGH_GeometricGoo DuplicateGeometry()
    {
      return new ComposBeamGoo(Value == null ? new ComposBeam() : Value.Duplicate()); 
    }
    #endregion

    #region properties
    public override bool IsValid => true;
    public override string TypeName => "Beam";
    public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
    public override string IsValidWhyNot
    {
      get
      {
        if (Value.IsValid) { return string.Empty; }
        return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
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
        if (Value == null) { return BoundingBox.Empty; }
        if (Value.Line == null) { return BoundingBox.Empty; }
        return Value.Line.GetBoundingBox(false);
      }
    }
    public override BoundingBox GetBoundingBox(Transform xform)
    {
      if (Value == null) { return BoundingBox.Empty; }
      if (Value.Line == null) { return BoundingBox.Empty; }
      return Value.Line.GetBoundingBox(xform);
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of our custom class into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(ComposBeam)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      //Cast to Curve
      if (typeof(Q).IsAssignableFrom(typeof(Line)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Line;
        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(GH_Line)))
      {
        if (Value == null)
          target = default;
        else
        {
          GH_Line ghLine = new GH_Line();
          GH_Convert.ToGHLine(Value.Line, GH_Conversion.Both, ref ghLine);
          target = (Q)(object)ghLine;
        }

        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(Curve)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Line;
        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)new GH_Curve(Value.Line);
        }
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

      //Cast from GsaMaterial
      if (typeof(ComposBeam).IsAssignableFrom(source.GetType()))
      {
        Value = (ComposBeam)source;
        return true;
      }

      try
      {
        // Cast from GsaGH
        if (GsaGHConverter.IsPresent())
        {
          Type type = GsaGHConverter.GetTypeFor(typeof(IComposBeam));
          if (type.IsAssignableFrom(source.GetType()))
          {
            Value = (ComposBeam)GsaGHConverter.CastToComposBeam(source);
            return true;
          }
        }
        // Cast from AdSecGH
        if (AdSecGHConverter.IsPresent())
        {
          Type type = AdSecGHConverter.GetTypeFor(typeof(IComposBeam));
          if (type.IsAssignableFrom(source.GetType()))
          {
            Value = (ComposBeam)AdSecGHConverter.CastToComposBeam(source);
            return true;
          }
        }
        // Cast from Speckle
        if (SpeckleConverter.IsPresent())
        {
          // todo: implement
        }

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
      if (Value == null) { return null; }
      if (Value.Line == null) { return null; }

      ComposBeam elem = Value.Duplicate();
      LineCurve xLn = elem.Line;
      xLn.Transform(xform);
      elem.Line = xLn;
      elem.UpdatePreview();

      return new ComposBeamGoo(elem);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      if (Value == null) { return null; }
      if (Value.Line == null) { return null; }

      ComposBeam elem = Value.Duplicate();
      LineCurve xLn = Value.Line;
      xmorph.Morph(xLn);
      elem.Line = xLn;
      elem.UpdatePreview();

      return new ComposBeamGoo(elem);
    }

    #endregion

    #region drawing methods
    public BoundingBox ClippingBox
    {
      get { return Boundingbox; }
    }
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      //Draw shape.
      //no meshes to be drawn
    }
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }

      //Draw lines
      //if (Value.Line != null)
      //{
      //  if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
      //  {
      //    if (Value.IsDummy)
      //      args.Pipeline.DrawDottedLine(Value.previewPointStart, Value.previewPointEnd, UI.Colour.Dummy1D);
      //    else
      //    {
      //      if ((System.Drawing.Color)Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
      //        args.Pipeline.DrawCurve(Value.Line, Value.Colour, 2);
      //      else
      //      {
      //        System.Drawing.Color col = UI.Colour.ElementType(Value.Type);
      //        args.Pipeline.DrawCurve(Value.Line, col, 2);
      //      }
      //      //args.Pipeline.DrawPoint(Value.previewPointStart, Rhino.Display.PointStyle.RoundSimple, 3, UI.Colour.Element1dNode);
      //      //args.Pipeline.DrawPoint(Value.previewPointEnd, Rhino.Display.PointStyle.RoundSimple, 3, UI.Colour.Element1dNode);
      //    }
      //  }
      //  else
      //  {
      //    if (Value.IsDummy)
      //      args.Pipeline.DrawDottedLine(Value.previewPointStart, Value.previewPointEnd, UI.Colour.Element1dSelected);
      //    else
      //    {
      //      args.Pipeline.DrawCurve(Value.Line, UI.Colour.Element1dSelected, 2);
      //      //args.Pipeline.DrawPoint(Value.previewPointStart, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Element1dNodeSelected);
      //      //args.Pipeline.DrawPoint(Value.previewPointEnd, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Element1dNodeSelected);
      //    }
      //  }
      //}
      ////Draw releases
      //if (!Value.IsDummy)
      //{
      //  if (Value.previewGreenLines != null)
      //  {
      //    foreach (Line ln1 in Value.previewGreenLines)
      //      args.Pipeline.DrawLine(ln1, UI.Colour.Support);
      //    foreach (Line ln2 in Value.previewRedLines)
      //      args.Pipeline.DrawLine(ln2, UI.Colour.Release);
      //  }
      //}
    }
    #endregion
  }

  /// <summary>
  /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class ComposBeamParameter : GH_PersistentGeometryParam<ComposBeamGoo>, IGH_PreviewObject
  {
    public ComposBeamParameter()
      : base(new GH_InstanceDescription("Beam", "Bm", "Maintains a collection of Compos Beam data.", Components.Ribbon.CategoryName.Name(), Components.Ribbon.SubCategoryName.Cat10()))
    {
    }

    public override Guid ComponentGuid => new Guid("dc61e94b-c326-4789-92f2-e0fe3caea4c7");

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.BeamParam;

    //We do not allow users to pick parameter, 
    //therefore the following 4 methods disable all this ui.
    protected override GH_GetterResult Prompt_Plural(ref List<ComposBeamGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref ComposBeamGoo value)
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
