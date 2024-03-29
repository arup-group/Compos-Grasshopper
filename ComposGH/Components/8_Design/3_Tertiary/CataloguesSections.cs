﻿using System;
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

namespace ComposGH.Components {
  public class CataloguesSections : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("38a7e123-4bdc-4f26-96f0-65ab64ab964e");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CatalogueID;
    private List<string> Catalogues = ComposAPI.Helpers.CatalogueSectionType.CatalogueSectionTypes.Values.Select(x => x.ToString()).ToList();

    public CataloguesSections() : base("SectionCatID", "Cat", "Get Compos Section Catalogue IDs for a " + DesignCriteriaGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat8()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Catalogue" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(Catalogues);
      _selectedItems.Add(_dropDownItems[0][4]); // Europe IPEs

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Search", "S", "Text to search from", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddIntegerParameter("Catalogue ID", "CID", "Compos Section Catalogue ID for a " + DesignCriteriaGoo.Description, GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      Dictionary<int, string> catDic = ComposAPI.Helpers.CatalogueSectionType.CatalogueSectionTypes;

      string s = "";
      if (DA.GetData(0, ref s)) {
        s = s.ToLower();
        var catIDs = new List<int>();
        for (int i = 0; i < Catalogues.Count; i++) {
          if (Catalogues[i].ToLower().Contains(s)) {
            catIDs.Add(catDic.Keys.ElementAt(i));
          }
          if (!s.Any(char.IsDigit)) {
            string test = Catalogues[i].ToString();
            test = Regex.Replace(test, "[0-9]", string.Empty);
            test = test.Replace(".", string.Empty);
            test = test.Replace("-", string.Empty);
            test = test.ToLower();
            if (test.Contains(s)) {
              catIDs.Add(catDic.Keys.ElementAt(i));
            }
          }
        }

        if (catIDs.Count > 1) {
          _selectedItems[0] = " - ";
          DA.SetDataList(0, catIDs);
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Found the following catalogues:");
          foreach (int i in catIDs) {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, catDic[i]);
          }
        } else if (catIDs.Count == 1) {
          _selectedItems[0] = catDic[catIDs[0]];
          DA.SetDataList(0, catIDs);
        } else {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Could not find a matching catalogue");
        }
        return;
      } else {
        if (_selectedItems[0] == " - ") {
          _selectedItems[0] = Catalogues[4];
        }

        DA.SetData(0, new GH_Integer(catDic.FirstOrDefault(x => x.Value == _selectedItems[0]).Key));
      }
    }
  }
}
