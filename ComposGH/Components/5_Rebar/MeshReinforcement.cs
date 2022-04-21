using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using ComposGH.Parameters;
using Rhino.Geometry;
using UnitsNet;
using UnitsNet.Units;
using ComposGH.Components;
using ComposGH.Helpers;

namespace ComposGH.Components
{
  public class CreateMesh : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public CreateMesh()
        : base("Mesh Reinforcement", "MeshRb", "Create Compos Slab Reinforcement from a Standard Reinforment Mesh",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = false; }
    public override Guid ComponentGuid => new Guid("17960644-0DFC-4F5D-B17C-45E6FBC3732E");
    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.MeshReinforcement;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // mesh
        dropdownitems.Add(Enum.GetValues(typeof(MeshReinforcement.ReinforcementMeshType)).Cast<MeshReinforcement.ReinforcementMeshType>().Select(x => x.ToString()).ToList());
        dropdownitems[0].RemoveAt(0); //
        selecteditems.Add(mesh.ToString());

        // length
        dropdownitems.Add(Units.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());


        first = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      if (i == 0)  // change is made to code 
      {
        if (mesh.ToString() == selecteditems[i])
          return; // return if selected value is same as before

        mesh = (MeshReinforcement.ReinforcementMeshType)Enum.Parse(typeof(MeshReinforcement.ReinforcementMeshType), selecteditems[i]);

        //ToggleInput();
      }
      else
      {
        lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);
      }

        // update name of inputs (to display unit on sliders)
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }


    private void UpdateUIFromSelectedItems()
    {
      mesh = (MeshReinforcement.ReinforcementMeshType)Enum.Parse(typeof(MeshReinforcement.ReinforcementMeshType), selecteditems[0]);
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[1]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output

    // list of lists with all dropdown lists content
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Standard Mesh",
            "Unit"
    });
    private bool first = true;
    private LengthUnit lengthUnit = Units.LengthUnitGeometry;
    private MeshReinforcement.ReinforcementMeshType mesh = MeshReinforcement.ReinforcementMeshType.A393;
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity length = new Length(0, lengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Cover [" + unitAbbreviation + "]", "Cov", "Reinforcement cover", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Rotated", "Rot", "If the mesh type is assymetrical, setting 'Rotated' to true will align the stronger direction with the beam's direction", GH_ParamAccess.item, false);

    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Reinforcement", "Rb", "Mesh Reinforcement for Compos Slab", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // get default length inputs used for all cases
      Length cov = Length.Zero;
      if (this.Params.Input[0].Sources.Count > 0)
        cov = GetInput.Length(this, DA, 0, lengthUnit, true);

      bool rotated = false;
      DA.GetData(1, ref rotated);
      DA.SetData(0, new ComposReinforcementGoo(new ComposReinforcement(cov, mesh, rotated)));
    }


    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
      UpdateUIFromSelectedItems();
      first = false;
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
      IQuantity length = new Length(0, lengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      Params.Input[0].Name = "Cover [" + unitAbbreviation + "]";
    }

  }
  #endregion
}
