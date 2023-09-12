using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;

namespace ComposGH.Components {
  public class CreateCatalogueDeck : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("6796D3E6-CF84-4AC6-ABB7-012C20E6DB9A");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StandardDecking;
    private string Catalogue = null;

    private List<string> CatalogueNames = ComposAPI.Helpers.SqlReader.Instance.GetDeckCataloguesDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"));

    private string Profile = null;

    private List<string> SectionList = null;

    private DeckingSteelGrade SteelGrade = DeckingSteelGrade.S350;

    public CreateCatalogueDeck() : base("Catalogue" + DeckingGoo.Name.Replace(" ", string.Empty),
      DeckingGoo.Name.Replace(" ", string.Empty),
      "Look up a Catalogue " + DeckingGoo.Description + " for a " + SlabGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3()) { Hidden = true; }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0)  // change is made to code
      {
        // update selected section to be all
        Catalogue = _selectedItems[0];
        _dropDownItems[1] = ComposAPI.Helpers.SqlReader.Instance.GetDeckingDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), Catalogue);
      }

      if (i == 1) {
        Profile = _selectedItems[1];
      }

      if (i == 2) {
        SteelGrade = (DeckingSteelGrade)Enum.Parse(typeof(DeckingSteelGrade), _selectedItems[i]);
      }

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] {
        "Type",
        "Decking",
        "Steel Type" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // catalogue
      _dropDownItems.Add(CatalogueNames);
      _selectedItems.Add(CatalogueNames[0]);
      Catalogue = _selectedItems[0];

      // decking
      SectionList = ComposAPI.Helpers.SqlReader.Instance.GetDeckingDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), Catalogue);
      _dropDownItems.Add(SectionList);
      _selectedItems.Add(SectionList[0]);
      Profile = _selectedItems[1];

      // steel
      _dropDownItems.Add(Enum.GetValues(typeof(DeckingSteelGrade)).Cast<DeckingSteelGrade>().Select(x => x.ToString()).ToList());
      _selectedItems.Add(SteelGrade.ToString());

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter(DeckingConfigurationGoo.Name, DeckingConfigurationGoo.NickName, DeckingConfigurationGoo.Description, GH_ParamAccess.item);
      pManager[0].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new ComposDeckingParameter(), "Catalogue " + DeckingGoo.Name, DeckingGoo.NickName, "Standard Catalogue " + DeckingGoo.Description + " for a " + SlabGoo.Description, GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      if (Params.Input[0].Sources.Count > 0) {
        var dconf = (DeckingConfigurationGoo)Input.GenericGoo<DeckingConfigurationGoo>(this, DA, 0);
        if (dconf == null) {
          return;
        }
        DA.SetData(0, new DeckingGoo(new CatalogueDecking(Catalogue, Profile, SteelGrade, dconf.Value)));
      } else {
        DA.SetData(0, new DeckingGoo(new CatalogueDecking(Catalogue, Profile, SteelGrade, new DeckingConfiguration())));
      }
    }

    protected override void UpdateUIFromSelectedItems() {
      SectionList = ComposAPI.Helpers.SqlReader.Instance.GetDeckingDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), Catalogue);
      Catalogue = _selectedItems[0];
      Profile = _selectedItems[1];
      SteelGrade = (DeckingSteelGrade)Enum.Parse(typeof(DeckingSteelGrade), _selectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
