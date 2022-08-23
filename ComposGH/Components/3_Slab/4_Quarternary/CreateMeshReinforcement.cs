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
  public class CreateMeshReinforcement : GH_OasysDropDownComponent
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

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

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
      SetOutput.Item(this, DA, 0, new MeshReinforcementGoo(new MeshReinforcement(cov, Mesh, rotated)));
    }

    #region Custom UI
    private LengthUnit LengthUnit = Units.LengthUnitSection;
    private ReinforcementMeshType Mesh = ReinforcementMeshType.A393;

    internal override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Standard Mesh", "Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // mesh
      this.DropDownItems.Add(Enum.GetValues(typeof(ReinforcementMeshType)).Cast<ReinforcementMeshType>().Select(x => x.ToString()).ToList());
      this.DropDownItems[0].RemoveAt(0); //
      this.SelectedItems.Add(this.Mesh.ToString());

      // length
      this.DropDownItems.Add(Units.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (i == 0)  // change is made to code 
      {
        if (Mesh.ToString() == SelectedItems[i])
          return; // return if selected value is same as before

        Mesh = (ReinforcementMeshType)Enum.Parse(typeof(ReinforcementMeshType), SelectedItems[i]);
      }
      else
        LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      this.Mesh = (ReinforcementMeshType)Enum.Parse(typeof(ReinforcementMeshType), SelectedItems[0]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit); 
      Params.Input[0].Name = "Cover [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
