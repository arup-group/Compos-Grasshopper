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
  public class CreateStandardENSteelMaterial : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("e671a346-5989-47e0-aacc-920c77fdfb1f");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StandardENSteelMaterial;
    public CreateStandardENSteelMaterial()
      : base("StandardEN" + SteelMaterialGoo.Name.Replace(" ", string.Empty),
          "EN" + SteelMaterialGoo.NickName.Replace(" ", string.Empty),
          "Look up a Standard EN " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description,
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
      pManager.AddParameter(new SteelMaterialParam(), "Standard " + SteelMaterialGoo.Name, SteelMaterialGoo.NickName, "Standard EN1993-1-1 " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description, GH_ParamAccess.item);
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
          if (Char.IsDigit(grade[0]))
            grade = "S" + grade;
          this.SteelGrade = (StandardSteelGrade)Enum.Parse(typeof(StandardSteelGrade), grade);
          this._dropDownItems[0] = new List<string>();
          this._selectedItems[0] = "-";
          this.Override_dropDownItems[0] = true;
        }
        catch (ArgumentException)
        {
          string text = "Could not parse steel grade. Valid EC4 steel grades are ";
          List<string> gradesEN = Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList();
          gradesEN.RemoveAt(3);
          foreach (string g in gradesEN)
          {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          this._dropDownItems[0] = gradesEN;
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, text);
          return;
        }
      }
      else if (this.Override_dropDownItems[0])
      {
        List<string> gradesEN = Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList();
        gradesEN.RemoveAt(3);
        this._dropDownItems[0] = gradesEN;
        this.Override_dropDownItems[0] = false;
      }

      Output.SetItem(this, DA, 0, new SteelMaterialGoo(new SteelMaterial(SteelGrade, Code.EN1994_1_1_2004)));
    }

    #region Custom UI
    List<bool> Override_dropDownItems;
    private StandardSteelGrade SteelGrade = StandardSteelGrade.S235;
    protected override void InitialiseDropdowns()
    {
      this._spacerDescriptions = new List<string>(new string[] { "Grade" });

      this._dropDownItems = new List<List<string>>();
      this._selectedItems = new List<string>();

      // SteelType
      this._dropDownItems.Add(Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList());
      this._dropDownItems[0].RemoveAt(3); // remove S450
      this._selectedItems.Add(this.SteelGrade.ToString());

      this.Override_dropDownItems = new List<bool>() { false };

      this._isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this._selectedItems[i] = this._dropDownItems[i][j];

      if (i == 0)  // change is made to code 
      {
        if (this.SteelGrade.ToString() == this._selectedItems[i])
          return; // return if selected value is same as before

        this.SteelGrade = (StandardSteelGrade)Enum.Parse(typeof(StandardSteelGrade), _selectedItems[i]);
      }

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      if (this._selectedItems[0] != "-")
        this.SteelGrade = (StandardSteelGrade)Enum.Parse(typeof(StandardSteelGrade), this._selectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
