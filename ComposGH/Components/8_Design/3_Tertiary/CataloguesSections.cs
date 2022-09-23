using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;

namespace ComposGH.Components
{
  public class CataloguesSections : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("38a7e123-4bdc-4f26-96f0-65ab64ab964e");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CatalogueID;
    public CataloguesSections()
      : base("SectionCatID", "Cat", "Get Compos Section Catalogue IDs for a " + DesignCriteriaGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat8())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddTextParameter("Search", "S", "Text to search from", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddIntegerParameter("Catalogue ID", "CID", "Compos Section Catalogue ID for a " + DesignCriteriaGoo.Description, GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Dictionary<int, string> catDic = ComposAPI.Helpers.CatalogueSectionType.CatalogueSectionTypes;
      
      string s = "";
      if (DA.GetData(0, ref s))
      {
        s = s.ToLower();
        List<int> catIDs = new List<int>();
        for (int i = 0; i < Catalogues.Count; i++)
        {
          if (Catalogues[i].ToLower().Contains(s))
          {
            catIDs.Add(catDic.Keys.ElementAt(i));
          }
          if (!s.Any(char.IsDigit))
          {
            string test = Catalogues[i].ToString();
            test = Regex.Replace(test, "[0-9]", string.Empty);
            test = test.Replace(".", string.Empty);
            test = test.Replace("-", string.Empty);
            test = test.ToLower();
            if (test.Contains(s))
            {
              catIDs.Add(catDic.Keys.ElementAt(i));
            }
          }
        }

        if (catIDs.Count > 1)
        {
          SelectedItems[0] = " - ";
          DA.SetDataList(0, catIDs);
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Found the following catalogues:");
          foreach (int i in catIDs)
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, catDic[i]);
        }
        else if (catIDs.Count == 1)
        {
          SelectedItems[0] = catDic[catIDs[0]];
          DA.SetDataList(0, catIDs);
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Could not find a matching catalogue");
        }
        return;
      }
      else
      {
        if (SelectedItems[0] == " - ")
          SelectedItems[0] = Catalogues[4];

        Output.SetItem(this, DA, 0, new GH_Integer(catDic.FirstOrDefault(x => x.Value == SelectedItems[0]).Key));
      }
    }

    #region Custom UI
    List<string> Catalogues = ComposAPI.Helpers.CatalogueSectionType.CatalogueSectionTypes.Values.Select(x => x.ToString()).ToList();

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Catalogue" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      DropDownItems.Add(Catalogues);
      SelectedItems.Add(DropDownItems[0][4]); // Europe IPEs

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      base.UpdateUI();
    }
    #endregion
  }
}
