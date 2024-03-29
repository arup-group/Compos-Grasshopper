﻿using System;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Components {
  public class CreateDeckingConfiguration : GH_OasysComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("85E6A4A4-DD97-4780-A679-B733C4B4FE01");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.DeckingConfig;
    private AngleUnit angleUnit = AngleUnit.Radian;

    public CreateDeckingConfiguration() : base("Create" + DeckingConfigurationGoo.Name.Replace(" ", string.Empty),
      DeckingConfigurationGoo.Name.Replace(" ", string.Empty),
      "Create a " + DeckingConfigurationGoo.Description + " for a " + DeckingGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3()) { Hidden = true; } // sets the initial state of the component to hidden

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      if (Params.Input[0] is Param_Number angleParameter) {
        if (angleParameter.UseDegrees) {
          angleUnit = AngleUnit.Degree;
        } else {
          angleUnit = AngleUnit.Radian;
        }
      }
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      IQuantity angle = new Angle(0, angleUnit);
      string unitAbbreviation = string.Concat(angle.ToString().Where(char.IsLetter));

      pManager.AddAngleParameter("Angle", "Angle", "Decking angle", GH_ParamAccess.item, Math.PI / 2);
      pManager.AddBooleanParameter("Discontinaus", "Con", "Is decking discontinous (default = true)", GH_ParamAccess.item, true);
      pManager.AddBooleanParameter("Welded", "Wd", "Is decking welded onto steel beam (default = false)", GH_ParamAccess.item, false);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter(DeckingConfigurationGoo.Name, DeckingConfigurationGoo.NickName, DeckingConfigurationGoo.Description + " for a Custom " + DeckingGoo.Description, GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      double angle = Math.PI / 2;
      DA.GetData(0, ref angle);
      bool isDiscontinous = true;
      bool isWelded = true;
      DA.GetData(1, ref isDiscontinous);
      DA.GetData(2, ref isWelded);

      DA.SetData(0, new DeckingConfigurationGoo(new DeckingConfiguration(new Angle(angle, angleUnit), isDiscontinous, isWelded)));
    }
  }
}
