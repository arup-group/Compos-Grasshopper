﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateRebarMaterial: GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("E91D37A1-81D4-427D-9910-E8A514466F3C");
    public CreateRebarMaterial()
      : base("Create" + ReinforcementMaterialGoo.Name.Replace(" ", string.Empty),
          ReinforcementMaterialGoo.Name.Replace(" ", string.Empty),
          "Create a Standard " + ReinforcementMaterialGoo.Description + " for a " + TransverseReinforcementGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.RebarMaterial;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout

    // list of lists with all dropdown lists conctent
    List<List<string>> DropDownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 

    List<string> SpacerDescriptions = new List<string>(new string[]
    {
      "Grade",
      "Unit"
    });

    private bool First = true;
    private PressureUnit StressUnit = Units.StressUnit;
    private RebarGrade Grade = RebarGrade.EN_500B;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // grade
        DropDownItems.Add(Enum.GetValues(typeof(RebarGrade)).Cast<RebarGrade>().Select(x => x.ToString()).ToList());
        SelectedItems.Add(Grade.ToString());

        // strength
        DropDownItems.Add(Units.FilteredStressUnits);
        SelectedItems.Add(StressUnit.ToString());

        First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropDownItems, SelectedItems, SpacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropDownItems[i][j];

      if (i == 0) // change is made to grade
      {
        Grade = (RebarGrade)Enum.Parse(typeof(RebarGrade), SelectedItems[i]);
      }
      if (i == 1) // change is made to unit
      {
        StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), SelectedItems[i]);
      }

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      if (SelectedItems[0] != "Custom")
        Grade = (RebarGrade)Enum.Parse(typeof(RebarGrade), SelectedItems[0]);

      StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), SelectedItems[1]);


      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(this.StressUnit);
      pManager.AddGenericParameter("Strength [" + stressUnitAbbreviation + "]", "fu", "(Optional) Custom Characteristic Steel Strength", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(ReinforcementMaterialGoo.Name, ReinforcementMaterialGoo.NickName, ReinforcementMaterialGoo.Description + " for a " + TransverseReinforcementGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      if (this.Params.Input[0].Sources.Count > 0)
      {
        SelectedItems[0] = "Custom";
        DA.SetData(0, new ReinforcementMaterialGoo(new ReinforcementMaterial(GetInput.Stress(this, DA, 0, StressUnit))));
      }
      else
        DA.SetData(0, new ReinforcementMaterialGoo(new ReinforcementMaterial(Grade)));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, DropDownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropDownItems, ref SelectedItems, ref SpacerDescriptions);

      UpdateUIFromSelectedItems();

      First = false;

      return base.Read(reader);
    }
    #endregion

    #region IGH_VariableParameterComponent null implementation
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(this.StressUnit);
      Params.Input[0].Name = "Strength [" + stressUnitAbbreviation + "]";
    }
    #endregion
  }
}
