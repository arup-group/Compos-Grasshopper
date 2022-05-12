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
  public class CreateStandardASNZSteelMaterial : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("8656c967-817c-49fe-9297-d863664b714a");
    public CreateStandardASNZSteelMaterial()
      : base("Standard ASNZ Steel Material", "StdASNZSteelMat", "Create Standard AS/NZS2327:2017 Steel Material for a Compos Beam",
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
    private ASNZSteelMaterialGrade SteelGrade = ASNZSteelMaterialGrade.C450_AS1163;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // SteelType
        List<ASNZSteelMaterialGrade> grades = Enum.GetValues(typeof(ASNZSteelMaterialGrade)).Cast<ASNZSteelMaterialGrade>().ToList();
        List<string> gradeStrings = new List<string>();
        foreach (ASNZSteelMaterialGrade grade in grades)
        {
          ASNZSteelMaterial mat = new ASNZSteelMaterial(grade);
          gradeStrings.Add(mat.ToString());
        }
        DropdownItems.Add(gradeStrings);
        SelectedItems.Add(gradeStrings[0]);

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
        ASNZSteelMaterial mat = new ASNZSteelMaterial(SelectedItems[i]);
        SteelGrade = mat.Grade;
      }

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      ASNZSteelMaterial mat = new ASNZSteelMaterial(SelectedItems[0]);
      SteelGrade = mat.Grade;

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
      DA.SetData(0, new SteelMaterialGoo(new ASNZSteelMaterial(SteelGrade)));
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
