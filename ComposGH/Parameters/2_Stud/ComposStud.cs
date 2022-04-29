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

namespace ComposGH.Parameters
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class ComposStud
  {
    public StudDimensions StudDimension { get; set; }
    public StudSpecification StudSpecification { get; set; }

    // Stud Spacing
    public List<StudGroupSpacing> CustomSpacing { get; set; } = null;
    public double Interaction { get; set; }
    public double MinSavingMultipleZones { get; set; }
    public bool CheckStudSpacing { get; set; }
    public StudGroupSpacing.StudSpacingType StudSpacingType
    {
      get { return this.m_spacingType; }
      set
      {
        this.m_spacingType = value;
        switch (value)
        {
          case StudGroupSpacing.StudSpacingType.Custom:
            this.Interaction = double.NaN;
            this.MinSavingMultipleZones = double.NaN;
            break;
          case StudGroupSpacing.StudSpacingType.Automatic:
          case StudGroupSpacing.StudSpacingType.Min_Num_of_Studs:
            this.Interaction = double.NaN;
            if (this.MinSavingMultipleZones == double.NaN)
              this.MinSavingMultipleZones = 0.20;
            break;
          case StudGroupSpacing.StudSpacingType.Partial_Interaction:
            if (this.Interaction == double.NaN)
              this.Interaction = 0.85;
            if (this.MinSavingMultipleZones == double.NaN)
              this.MinSavingMultipleZones = 0.20;
            break;
        }
      }
    }
    private StudGroupSpacing.StudSpacingType m_spacingType;

    #region constructors
    public ComposStud()
    {
      // empty constructor
    }
    /// <summary>
    /// Create Stud Dimensions with Custom spacing type
    /// </summary>
    /// <param name="stud"></param>
    /// <param name="spec"></param>
    /// <param name="spacings"></param>
    /// <param name="checkSpacing"></param>
    public ComposStud(StudDimensions stud, StudSpecification spec, List<StudGroupSpacing> spacings, bool checkSpacing)
    {
      this.StudDimension = stud;
      this.StudSpecification = spec;
      this.CustomSpacing = spacings;
      this.CheckStudSpacing = checkSpacing;
      this.StudSpacingType = StudGroupSpacing.StudSpacingType.Custom;
      this.Interaction = double.NaN;
      this.MinSavingMultipleZones = double.NaN;
    }

    /// <summary>
    /// Create Stud Dimensions with Automatic or Minimum number of Studs types
    /// </summary>
    /// <param name="stud"></param>
    /// <param name="spec"></param>
    /// <param name="minSaving"></param>
    /// <param name="type"></param>
    /// <exception cref="ArgumentException"></exception>
    public ComposStud(StudDimensions stud, StudSpecification spec, double minSaving, StudGroupSpacing.StudSpacingType type)
    {
      this.StudDimension = stud;
      this.StudSpecification = spec;
      this.StudSpacingType = type;
      this.MinSavingMultipleZones = minSaving;
      switch (type)
      {
        case StudGroupSpacing.StudSpacingType.Min_Num_of_Studs:
        case StudGroupSpacing.StudSpacingType.Automatic:
          break;

        default:
          throw new ArgumentException("Stud spacing type must be either Automatic or Minimum Number of Studs");
      }
      this.Interaction = double.NaN;
    }

    /// <summary>
    /// Create Stud Dimensions with Partial interaction type
    /// </summary>
    /// <param name="stud"></param>
    /// <param name="spec"></param>
    /// <param name="minSaving"></param>
    /// <param name="interaction"></param>
    public ComposStud(StudDimensions stud, StudSpecification spec, double minSaving, double interaction)
    {
      this.StudDimension = stud;
      this.StudSpecification = spec;
      this.StudSpacingType = StudGroupSpacing.StudSpacingType.Partial_Interaction;
      this.MinSavingMultipleZones = minSaving;
      this.Interaction = interaction;
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
    internal ComposStud(string coaString)
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

    public ComposStud Duplicate()
    {
      if (this == null) { return null; }
      ComposStud dup = (ComposStud)this.MemberwiseClone();
      dup.StudDimension = this.StudDimension.Duplicate();
      dup.StudSpecification = this.StudSpecification.Duplicate();
      if (this.CustomSpacing != null)
        dup.CustomSpacing = this.CustomSpacing.ToList();
      return dup;
    }

    public override string ToString()
    {
      string size = this.StudDimension.Diameter.As(Units.LengthUnitGeometry).ToString("f0") + "/" + this.StudDimension.Height.ToUnit(Units.LengthUnitGeometry).ToString("f0");
      return size.Replace(" ", string.Empty);
    }

    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class ComposStudGoo : GH_Goo<ComposStud>
  {
    #region constructors
    public ComposStudGoo()
    {
      this.Value = new ComposStud();
    }
    public ComposStudGoo(ComposStud item)
    {
      if (item == null)
        item = new ComposStud();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public ComposStudGoo DuplicateGoo()
    {
      return new ComposStudGoo(Value == null ? new ComposStud() : Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid => true;
    public override string TypeName => "Stud";
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
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of our custom class into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(ComposStud)))
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

      //Cast from GsaMaterial
      if (typeof(ComposStud).IsAssignableFrom(source.GetType()))
      {
        Value = (ComposStud)source;
        return true;
      }

      return false;
    }
    #endregion
  }

  /// <summary>
  /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>

  public class ComposStudParameter : GH_PersistentParam<ComposStudGoo>
  {
    public ComposStudParameter()
      : base(new GH_InstanceDescription("Stud", "Std", "Compos Stud", ComposGH.Components.Ribbon.CategoryName.Name(), ComposGH.Components.Ribbon.SubCategoryName.Cat10()))
    {
    }

    public override Guid ComponentGuid => new Guid("e0b6cb52-99c8-4b2a-aec1-7f8a2d720daa");

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => ComposGH.Properties.Resources.StudParam;

    protected override GH_GetterResult Prompt_Plural(ref List<ComposStudGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref ComposStudGoo value)
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

    public bool Hidden
    {
      get { return true; }
      //set { m_hidden = value; }
    }
    public bool IsPreviewCapable
    {
      get { return false; }
    }
    #endregion
  }

}
