using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH;
using OasysUnits.Units;
using OasysUnits;
using ComposAPI;
using ComposAPI.Helpers;

namespace ComposGH.Components
{
  public class CreateCatalogueDeck : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("6796D3E6-CF84-4AC6-ABB7-012C20E6DB9A");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StandardDecking;
    public CreateCatalogueDeck()
        : base("Catalogue" + DeckingGoo.Name.Replace(" ", string.Empty),
          DeckingGoo.Name.Replace(" ", string.Empty),
          "Look up a Catalogue " + DeckingGoo.Description + " for a " + SlabGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter(DeckingConfigurationGoo.Name, DeckingConfigurationGoo.NickName, DeckingConfigurationGoo.Description, GH_ParamAccess.item);
      pManager[0].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ComposDeckingParameter(), "Catalogue " + DeckingGoo.Name, DeckingGoo.NickName, "Standard Catalogue " + DeckingGoo.Description + " for a " + SlabGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      if (this.Params.Input[0].Sources.Count > 0)
      {
        DeckingConfigurationGoo dconf = (DeckingConfigurationGoo)Input.GenericGoo<DeckingConfigurationGoo>(this, DA, 0);
        if (dconf == null)
        {
          return;
        }
        DA.SetData(0, new DeckingGoo(new CatalogueDecking(this.Catalogue, this.Profile, this.SteelGrade, dconf.Value)));
      }
      else
        Output.SetItem(this, DA, 0, new DeckingGoo(new CatalogueDecking(this.Catalogue, this.Profile, this.SteelGrade, new DeckingConfiguration())));
    }

    #region Custom UI
    string Catalogue = null;
    string Profile = null;
    private DeckingSteelGrade SteelGrade = DeckingSteelGrade.S350;
    List<string> CatalogueNames = ComposAPI.Helpers.SqlReader.Instance.GetDeckCataloguesDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"));
    List<string> SectionList = null;

    protected override void InitialiseDropdowns()
    {
      this._spacerDescriptions = new List<string>(new string[] {
        "Type",
        "Decking",
        "Steel Type" });

      this._dropDownItems = new List<List<string>>();
      this._selectedItems = new List<string>();

      // catalogue
      this._dropDownItems.Add(this.CatalogueNames);
      this._selectedItems.Add(this.CatalogueNames[0]);
      this.Catalogue = this._selectedItems[0];

      // decking
      this.SectionList = ComposAPI.Helpers.SqlReader.Instance.GetDeckingDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), this.Catalogue);
      this._dropDownItems.Add(this.SectionList);
      this._selectedItems.Add(this.SectionList[0]);
      this.Profile = this._selectedItems[1];

      // steel
      this._dropDownItems.Add(Enum.GetValues(typeof(DeckingSteelGrade)).Cast<DeckingSteelGrade>().Select(x => x.ToString()).ToList());
      this._selectedItems.Add(SteelGrade.ToString());

      this._isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this._selectedItems[i] = this._dropDownItems[i][j];

      if (i == 0)  // change is made to code 
      {
        // update selected section to be all
        this.Catalogue = _selectedItems[0];
        this._dropDownItems[1] = ComposAPI.Helpers.SqlReader.Instance.GetDeckingDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), this.Catalogue);
      }

      if (i == 1)
        this.Profile = this._selectedItems[1];

      if (i == 2)
        this.SteelGrade = (DeckingSteelGrade)Enum.Parse(typeof(DeckingSteelGrade), this._selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      this.SectionList = ComposAPI.Helpers.SqlReader.Instance.GetDeckingDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), this.Catalogue);
      this.Catalogue = this._selectedItems[0];
      this.Profile = this._selectedItems[1];
      this.SteelGrade = (DeckingSteelGrade)Enum.Parse(typeof(DeckingSteelGrade), this._selectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
