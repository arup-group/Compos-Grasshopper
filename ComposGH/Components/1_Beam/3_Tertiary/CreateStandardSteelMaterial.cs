using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateStandardSteelMaterial : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("a671a346-5989-47e0-aacc-920c77fdfb1f");
    public CreateStandardSteelMaterial()
      : base("Standard Steel Material", "StdSteelMat", "Create Standard Steel Material for a Compos Beam",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateSteelMaterial;
    #endregion

    #region Custom UI

    // list of lists with all dropdown lists content
    List<List<string>> DropdownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
            "Grade",
    });

    private bool First = true;
    private StandardSteelGrade SteelGrade = StandardSteelGrade.S235;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // SteelType
        DropdownItems.Add(Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList());
        SelectedItems.Add(SteelGrade.ToString());

        First = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropdownItems, SelectedItems, SpacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropdownItems[i][j];

      if (i == 0)  // change is made to code 
      {
        if (SteelGrade.ToString() == SelectedItems[i])
          return; // return if selected value is same as before

        SteelGrade = (StandardSteelGrade)Enum.Parse(typeof(StandardSteelGrade), SelectedItems[i]);
      }

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      SteelGrade = (StandardSteelGrade)Enum.Parse(typeof(StandardSteelGrade), SelectedItems[0]);

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
      
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("StandardSteelMaterial", "SSM", "Standard Steel Material for a Compos Beam", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
        DA.SetData(0, new SteelMaterialGoo(new SteelMaterial(SteelGrade)));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, DropdownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropdownItems, ref SelectedItems, ref SpacerDescriptions);

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
    }
    #endregion

  }
}
