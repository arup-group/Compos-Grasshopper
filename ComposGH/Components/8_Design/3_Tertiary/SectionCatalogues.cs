using System;

using ComposGH.Parameters;
using Grasshopper.Kernel;
using ComposAPI;
using System.Collections.Generic;
using UnitsNet.Units;
using UnitsNet;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComposGH.Components
{
  public class CataloguesSections : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("38a7e123-4bdc-4f26-96f0-65ab64ab964e");
    public CataloguesSections()
      : base("SectionCatID", "Cat", "Get Compos Section Catalogue IDs for a " + DesignCriteriaGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat8())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CatalogueID;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout

    // list of lists with all dropdown lists conctent
    List<List<string>> DropDownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
      "Catalogue",
    });

    private bool First = true;
    List<string> Catalogues = ComposAPI.Helpers.CatalogueSectionType.CatalogueSectionTypes.Values.Select(x => x.ToString()).ToList();
    public override void CreateAttributes()
    {
      if (First)
      {
        DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // length
        DropDownItems.Add(Catalogues);
        SelectedItems.Add(DropDownItems[0][4]); // Europe IPEs

        First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropDownItems, SelectedItems, SpacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
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

        DA.SetData(0, catDic.FirstOrDefault(x => x.Value == SelectedItems[0]).Key);
      }
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, DropDownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropDownItems, ref SelectedItems, ref SpacerDescriptions);

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
