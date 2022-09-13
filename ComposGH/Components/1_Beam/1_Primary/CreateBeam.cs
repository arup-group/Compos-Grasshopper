using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH.Components;
using OasysGH.Helpers;
using Rhino.Geometry;
using UnitsNet;
using UnitsNet.Units;

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

    protected override System.Drawing.Bitmap Icon => Resources.CreateBeam;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

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
          RestraintGoo res = (RestraintGoo)Input.GenericGoo<RestraintGoo>(this, DA, 1);
          if (res == null) { return; } // return here on non-optional inputs

          SteelMaterialGoo mat = (SteelMaterialGoo)Input.GenericGoo<SteelMaterialGoo>(this, DA, 2);
          if (mat == null) { return; } // return here on non-optional inputs

          List<BeamSectionGoo> beamSections = Input.GenericGooList<BeamSectionGoo>(this, DA, 3);
          try
          {
            if (this.Params.Input[4].Sources.Count > 0)
            {
              List<WebOpeningGoo> webOpenings = Input.GenericGooList<WebOpeningGoo>(this, DA, 4);
              Output.SetItem(this, DA, 0, new BeamGoo(new LineCurve(ln), this.LengthUnit, res.Value, mat.Value, beamSections.Select(x => x.Value as IBeamSection).ToList(), webOpenings.Select(x => x.Value as IWebOpening).ToList()));
            }
            else
            {
              Output.SetItem(this, DA, 0, new BeamGoo(new LineCurve(ln), this.LengthUnit, res.Value, mat.Value, beamSections.Select(x => x.Value as IBeamSection).ToList()));
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

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Unit" });

      this.DropDownItems = new List<List<string>>() { Units.FilteredLengthUnits };
      this.SelectedItems = new List<string>() { this.LengthUnit.ToString() };

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      Params.Input[0].Name = "Line [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
