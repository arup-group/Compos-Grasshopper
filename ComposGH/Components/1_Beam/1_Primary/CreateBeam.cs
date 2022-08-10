using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateBeam : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("060641e49fc648eb8d7699f2d6697111");
    public CreateBeam()
      : base("Create"+ BeamGoo.Name.Replace(" ", string.Empty), 
          BeamGoo.Name.Replace(" ", string.Empty), 
          "Create a " + BeamGoo.Description + " for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = false; } // sets the initial state of the component to display

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateBeam;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddCurveParameter("Line [" + unitAbbreviation + "]", "L", "Line drawn to selected units to create Compos Beam from", GH_ParamAccess.item);
      pManager.AddGenericParameter(RestraintGoo.Name, RestraintGoo.NickName, RestraintGoo.Description, GH_ParamAccess.item);
      pManager.AddGenericParameter(SteelMaterialGoo.Name, SteelMaterialGoo.NickName, SteelMaterialGoo.Description, GH_ParamAccess.item);
      pManager.AddGenericParameter(BeamSectionGoo.Name + "(s)", BeamSectionGoo.NickName, BeamSectionGoo.Description + " or an I Profile string descriptions like 'CAT IPE IPE200', 'STD I(cm) 20. 19. 8.5 1.27' or 'STD GI 400 300 250 12 25 20'", GH_ParamAccess.list);
      pManager.AddGenericParameter(WebOpeningGoo.Name, WebOpeningGoo.NickName, "(Optional) " + WebOpeningGoo.Description, GH_ParamAccess.list);
      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(BeamGoo.Name, BeamGoo.NickName, BeamGoo.Description + " for a " + MemberGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_Line ghln = new GH_Line();
      if (DA.GetData(0, ref ghln))
      {
        if (ghln == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Line input is null"); }
        Line ln = new Line();
        if (GH_Convert.ToLine(ghln, ref ln, GH_Conversion.Both))
        {
          RestraintGoo res = (RestraintGoo)GetInput.GenericGoo<RestraintGoo>(this, DA, 1);
          if (res == null) { return; } // return here on non-optional inputs

          SteelMaterialGoo mat = (SteelMaterialGoo)GetInput.GenericGoo<SteelMaterialGoo>(this, DA, 2);
          if (mat == null) { return; } // return here on non-optional inputs

          List<BeamSectionGoo> beamSections = GetInput.GenericGooList<BeamSectionGoo>(this, DA, 3);
          try
          {
            if (this.Params.Input[4].Sources.Count > 0)
            {
              List<WebOpeningGoo> webOpenings = GetInput.GenericGooList<WebOpeningGoo>(this, DA, 4);
              DA.SetData(0, new BeamGoo(new LineCurve(ln), LengthUnit, res.Value, mat.Value, beamSections.Select(x => x.Value as IBeamSection).ToList(), webOpenings.Select(x => x.Value as IWebOpening).ToList()));
            }
            else
            {
              DA.SetData(0, new BeamGoo(new LineCurve(ln), LengthUnit, res.Value, mat.Value, beamSections.Select(x => x.Value as IBeamSection).ToList()));
            }
          }
          catch (Exception e)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            return;
          }
        }
      }
    }

    #region Custom UI
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    internal override void InitialiseDropdowns()
    {
      SpacerDescriptions = new List<string>(new string[] { "Unit" });

      DropdownItems = new List<List<string>>() { Units.FilteredLengthUnits };
      SelectedItems = new List<string>() { LengthUnit.ToString() };

      IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      SelectedItems[i] = DropdownItems[i][j];

      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);
      Params.Input[0].Name = "Line [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
