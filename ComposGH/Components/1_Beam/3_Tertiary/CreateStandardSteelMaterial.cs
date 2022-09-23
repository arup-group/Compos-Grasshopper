﻿using System;
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
  public class CreateStandardSteelMaterial : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("a671a346-5989-47e0-aacc-920c77fdfb1f");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StandardSteelMaterial;
    public CreateStandardSteelMaterial()
      : base("Standard" + SteelMaterialGoo.Name.Replace(" ", string.Empty),
          "STD" + SteelMaterialGoo.NickName.Replace(" ", string.Empty),
          "Look up a Standard " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description,
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
      pManager.AddParameter(new SteelMaterialParam(), "Standard " + SteelMaterialGoo.Name, SteelMaterialGoo.NickName, "Standard " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description, GH_ParamAccess.item);
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
          this.DropDownItems[0] = new List<string>();
          this.SelectedItems[0] = " -- ";
          this.OverrideDropDownItems[0] = true;
        }
        catch (ArgumentException)
        {
          string text = "Could not parse steel grade. Valid steel grades are ";
          foreach (string g in Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList())
          {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          this.DropDownItems[0] = Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, text);
          return;
        }
      }
      else if (this.OverrideDropDownItems[0])
      {
        this.DropDownItems[0] = Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList();
        this.OverrideDropDownItems[0] = false;
      }

      Output.SetItem(this, DA, 0, new SteelMaterialGoo(new SteelMaterial(SteelGrade, Code.BS5950_3_1_1990_A1_2010))); //any code other than EN or ASNZ will do...
    }

    #region Custom UI
    List<bool> OverrideDropDownItems;
    private StandardSteelGrade SteelGrade = StandardSteelGrade.S235;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Grade" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // SteelType
      this.DropDownItems.Add(Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList());
      this.SelectedItems.Add(SteelGrade.ToString());

      this.OverrideDropDownItems = new List<bool>() { false };

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)  // change is made to code 
      {
        if (this.SteelGrade.ToString() == this.SelectedItems[i])
          return; // return if selected value is same as before

        this.SteelGrade = (StandardSteelGrade)Enum.Parse(typeof(StandardSteelGrade), this.SelectedItems[i]);
      }

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      if (this.SelectedItems[0] != " -- ")
        this.SteelGrade = (StandardSteelGrade)Enum.Parse(typeof(StandardSteelGrade), this.SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
