using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using ComposGH.Helpers;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateMeshReinforcement : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public CreateMeshReinforcement()
        : base("Create" + MeshReinforcementGoo.Name.Replace(" ", string.Empty),
          MeshReinforcementGoo.Name.Replace(" ", string.Empty),
          "Create a Standard " + MeshReinforcementGoo.Description + " for a " + SlabGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; }
    public override Guid ComponentGuid => new Guid("17960644-0DFC-4F5D-B17C-45E6FBC3732E");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.MeshReinforcement;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout

    // list of lists with all dropdown lists content
    List<List<string>> DropDownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
      "Standard Mesh",
      "Unit"
    });
    private bool First = true;
    private LengthUnit LengthUnit = Units.LengthUnitSection;
    private ReinforcementMeshType mesh = ReinforcementMeshType.A393;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // mesh
        DropDownItems.Add(Enum.GetValues(typeof(ReinforcementMeshType)).Cast<ReinforcementMeshType>().Select(x => x.ToString()).ToList());
        DropDownItems[0].RemoveAt(0); //
        SelectedItems.Add(mesh.ToString());

        // length
        DropDownItems.Add(Units.FilteredLengthUnits);
        SelectedItems.Add(LengthUnit.ToString());

        First = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropDownItems, SelectedItems, SpacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)  // change is made to code 
      {
        if (mesh.ToString() == SelectedItems[i])
          return; // return if selected value is same as before

        mesh = (ReinforcementMeshType)Enum.Parse(typeof(ReinforcementMeshType), SelectedItems[i]);
      }
      else
      {
        LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);
      }

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }


    private void UpdateUIFromSelectedItems()
    {
      mesh = (ReinforcementMeshType)Enum.Parse(typeof(ReinforcementMeshType), SelectedItems[0]);
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[1]);

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
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddGenericParameter("Cover [" + unitAbbreviation + "]", "Cov", "Reinforcement cover", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Rotated", "Rot", "If the mesh type is assymetrical, setting 'Rotated' to true will align the stronger direction with the beam's direction", GH_ParamAccess.item, false);

    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(MeshReinforcementGoo.Name, MeshReinforcementGoo.NickName, MeshReinforcementGoo.Description + " for a " + SlabGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // get default length inputs used for all cases
      Length cov = Length.Zero;
      if (this.Params.Input[0].Sources.Count > 0)
        cov = GetInput.Length(this, DA, 0, LengthUnit, true);

      bool rotated = false;
      DA.GetData(1, ref rotated);
      DA.SetData(0, new MeshReinforcementGoo(new MeshReinforcement(cov, mesh, rotated)));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      DeSerialization.writeDropDownComponents(ref writer, DropDownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      DeSerialization.readDropDownComponents(ref reader, ref DropDownItems, ref SelectedItems, ref SpacerDescriptions);
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
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit); 
      Params.Input[0].Name = "Cover [" + unitAbbreviation + "]";
    }

  }
  #endregion
}
