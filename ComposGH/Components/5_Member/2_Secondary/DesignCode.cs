using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using Grasshopper.Kernel.Parameters;
using ComposAPI;
using static ComposAPI.EC4Options;

namespace ComposGH.Components
{
  public class DesignCode : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("f89b420e-a35e-4197-9c64-87504fe02b59");
    public DesignCode()
      : base("Design Code", "DC", "Create Compos Design Code",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateDesignCode;
    #endregion

    #region Custom UI
    // This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // code
        dropdownitems.Add(designcodesPretty);
        selecteditems.Add(designcodesPretty[2]); //EC4 default

        // national annex
        dropdownitems.Add(Enum.GetValues(typeof(NationalAnnex)).Cast<NationalAnnex>()
            .Select(x => x.ToString().Replace("_", " ")).ToList());
        selecteditems.Add(dropdownitems[1][0]); // Generic default

        // cement type
        dropdownitems.Add(Enum.GetValues(typeof(CementClass)).Cast<CementClass>()
            .Select(x => "Cement class " + x.ToString()).ToList());
        selecteditems.Add("Cement class " + ec4codeOptions.CementType.ToString()); // Class N default

        checkboxes = new List<bool>();
        checkboxes.Add(designOptions.ProppedDuringConstruction);
        checkboxes.Add(designOptions.InclSteelBeamWeight);
        checkboxes.Add(designOptions.InclThinFlangeSections);
        checkboxes.Add(designOptions.InclConcereteSlabWeight);
        checkboxes.Add(designOptions.ConsiderShearDeflection);
        checkboxes.Add(ec4codeOptions.ConsiderShrinkageDeflection);
        checkboxes.Add(ec4codeOptions.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
        checkboxes.Add(ec4codeOptions.ApproxModularRatios);

        first = false;
      }
      m_attributes = new UI.MultiDropDownCheckBoxesComponentUI(this, SetSelected, dropdownitems, selecteditems, CheckBoxToggles, checkboxes, checkboxNames, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      if (i == 0)
      {
        for (int k = 0; k < designcodesPretty.Count; k++)
        {
          if (selecteditems[i] == designcodesPretty[k])
          {
            if (code == (Code)k)
              return;

            code = (Code)k;
          }
        }
        switch (code)
        {
          case Code.BS5950_3_1_1990_Superseded:
          case Code.BS5950_3_1_1990_A1_2010:
          case Code.HKSUOS_2005:
          case Code.HKSUOS_2011:
            // change dropdown content
            while (dropdownitems.Count > 1)
              dropdownitems.RemoveAt(1);
            while (selecteditems.Count > 1)
              selecteditems.RemoveAt(1);



            break;

          case Code.EN1994_1_1_2004:
            // change dropdown content
            while (dropdownitems.Count > 1)
              dropdownitems.RemoveAt(1);
            while (selecteditems.Count > 1)
              selecteditems.RemoveAt(1);
            // national annex
            dropdownitems.Add(Enum.GetValues(typeof(NationalAnnex)).Cast<NationalAnnex>()
                .Select(x => x.ToString().Replace("_", " ")).ToList());
            selecteditems.Add(NA.ToString().Replace("_", " "));
            // cement type
            dropdownitems.Add(Enum.GetValues(typeof(CementClass)).Cast<CementClass>()
                .Select(x => "Cement class " + x.ToString()).ToList());
            selecteditems.Add("Cement class " + cementClass.ToString()); 

            break;

          case Code.AS_NZS2327_2017:
            // change dropdown content
            while (dropdownitems.Count > 1)
              dropdownitems.RemoveAt(1);
            while (selecteditems.Count > 1)
              selecteditems.RemoveAt(1);

            break;
          
          default:
            break;
        }
      }
      if (i == 1)
        NA = (NationalAnnex)Enum.Parse(typeof(NationalAnnex), selecteditems[i].Replace(" ", "_"));
      if (i == 2)
        cementClass = (CementClass)Enum.Parse(typeof(CementClass), selecteditems[i].Last().ToString());


      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void CheckBoxToggles(List<bool> checkboxes)
    {

    }

    private void UpdateUIFromSelectedItems()
    {
      for (int i = 0; i < designcodesPretty.Count; i++)
      {
        if (selecteditems[0] == designcodesPretty[i])
          code = (Code)i;
      }
      if (code == Code.EN1994_1_1_2004)
      {
        NA = (NationalAnnex)Enum.Parse(typeof(NationalAnnex), selecteditems[1].Replace(" ", "_"));
        cementClass = (CementClass)Enum.Parse(typeof(CementClass), selecteditems[2].Last().ToString());
      }


      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
      "Code",
      "National Annex",
      "Cement Type",
      "Settings"
    });
    List<string> designcodesPretty = new List<string>(new string[]
    {
      "BS5950-3.1:1990 (superseded)",
      "BS5950-3.1:1990+A1:2010",
      "EN1994-1-1:2004",
      "HKSUOS:2005",
      "HKSUOS:2011",
      "AS/NZS2327:2017"
    });
    List<string> checkboxNames = new List<string>(new string[]
    {
      "Beam propped during construction",
      "Include steel beam weight",
      "Include thin-flange sections",
      "Include concrete slabe weight",
      "Consider shear deflection",
      "Consider shrikage declection",
      "Ignore shrinkage def. if L/d < 20",
      "Use approx. modular ratios"
    });

    private bool first = true;
    private Code code = Code.EN1994_1_1_2004;
    private NationalAnnex NA = NationalAnnex.Generic;
    private CementClass cementClass = CementClass.N;
    
    private DesignOptions designOptions = new DesignOptions();
    private CodeOptions codeOptions = new CodeOptions();
    private EC4Options ec4codeOptions = new EC4Options();
    List<bool> checkboxes = new List<bool>();
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Creep&Shrinkage Shrinkage", "cs", "(Optional) Creep and Shrinkage parameters for Shrinkage", GH_ParamAccess.item);
      pManager.AddGenericParameter("Creep&Shrinkage", "CS", "(Optional) Creep and Shrinkage parameters Long term", GH_ParamAccess.item);
      pManager.AddGenericParameter("EC4 Safety Factors", "SF", "(Optional) EC4 Safety Factors", GH_ParamAccess.item);
      
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Design Code", "DC", "Compos Design Code", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      
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
      
    }
    #endregion
  }
}
