using System;
using System.Linq;
using System.Collections.Generic;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH;

namespace ComposGH.Components
{
  public class CreateStandardASNZSteelMaterial : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("8656c967-817c-49fe-9297-d863664b714a");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StandardASNZSteelMaterial;
    public CreateStandardASNZSteelMaterial()
      : base("StandardASNZ" + SteelMaterialGoo.Name.Replace(" ", string.Empty),
          "ASNZ" + SteelMaterialGoo.NickName.Replace(" ", string.Empty),
          "Look up a Standard ASNZ " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Grade", "G", "(Optional) Grade", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new SteelMaterialParam(), "Standard " + SteelMaterialGoo.Name, SteelMaterialGoo.NickName, "Standard ASNZ " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // override steel grade?
      if (this.Params.Input[0].Sources.Count > 0)
      {
        string grade = "";
        DA.GetData(0, ref grade);
        try
        {
          this.SteelGrade = (StandardASNZSteelMaterialGrade)Enum.Parse(typeof(StandardASNZSteelMaterialGrade), grade.Replace(" ", "_"));
          this._dropDownItems[0] = new List<string>();
          this._selectedItems[0] = "-";
          this.Override_dropDownItems[0] = true;
        }
        catch (ArgumentException)
        {
          string text = "Could not parse steel grade. Valid AS/NZS steel grades are ";
          foreach (string g in Enum.GetValues(typeof(StandardASNZSteelMaterialGrade)).Cast<StandardASNZSteelMaterialGrade>().Select(x => x.ToString()).ToList())
          {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          this._dropDownItems[0] = Enum.GetValues(typeof(StandardASNZSteelMaterialGrade)).Cast<StandardASNZSteelMaterialGrade>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, text);
          return;
        }
      }
      else if (this.Override_dropDownItems[0])
      {
        this._dropDownItems[0] = Enum.GetValues(typeof(StandardASNZSteelMaterialGrade)).Cast<StandardASNZSteelMaterialGrade>().Select(x => x.ToString()).ToList();
        this.Override_dropDownItems[0] = false;
      }

      Output.SetItem(this, DA, 0, new SteelMaterialGoo(new ASNZSteelMaterial(SteelGrade)));
    }

    #region Custom UI
    List<bool> Override_dropDownItems;
    private StandardASNZSteelMaterialGrade SteelGrade = StandardASNZSteelMaterialGrade.C450_AS1163;

    protected override void InitialiseDropdowns()
    {
      this._spacerDescriptions = new List<string>(new string[] { "Grade" });

      this._dropDownItems = new List<List<string>>();
      this._selectedItems = new List<string>();

      // SteelType
      List<StandardASNZSteelMaterialGrade> grades = Enum.GetValues(typeof(StandardASNZSteelMaterialGrade)).Cast<StandardASNZSteelMaterialGrade>().ToList();
      
      this._dropDownItems.Add(grades.Select(x => x.ToString().Replace("_", " ")).ToList());
      this._selectedItems.Add(_dropDownItems[0][0]);

      this.Override_dropDownItems = new List<bool>() { false };

      this._isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this._selectedItems[i] = this._dropDownItems[i][j];

      if (i == 0)  // change is made to code 
      {
        if (this.SteelGrade.ToString().Replace("_", " ") == this._selectedItems[i])
          return; // return if selected value is same as before
        StandardASNZSteelMaterialGrade grade = (StandardASNZSteelMaterialGrade)Enum.Parse(typeof(StandardASNZSteelMaterialGrade), this._selectedItems[i].Replace(" ", "_"));
        this.SteelGrade = grade;
      }

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      if (this._selectedItems[0] != "-")
      {
        StandardASNZSteelMaterialGrade grade = (StandardASNZSteelMaterialGrade)Enum.Parse(typeof(StandardASNZSteelMaterialGrade), this._selectedItems[0].Replace(" ", "_"));
        this.SteelGrade = grade;
      }

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
