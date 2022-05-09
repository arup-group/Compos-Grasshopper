using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using System.IO;
using ComposGH.Parameters;
using ComposGH.Helpers;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateCatalogueDeck : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public CreateCatalogueDeck()
        : base("Standard Decking", "StdDeck", "Create Standard Decking for Compos Slab",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = false; }
    public override Guid ComponentGuid => new Guid("6796D3E6-CF84-4AC6-ABB7-012C20E6DB9A");
    public override GH_Exposure Exposure => GH_Exposure.quinary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.StandardDecking;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout

    // list of lists with all dropdown lists content
    List<List<string>> DropdownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
      "Type",
      "Decking",
      "Steel Type"
    });
    private bool First = true;
    string Catalogue = null;
    string Profile = null;
    private DeckingSteelGrade SteelGrade = DeckingSteelGrade.S350;
    // Catalogues
    List<string> CatalogueNames = SqlReader.GetDeckCataloguesDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"));

    // Sections
    List<string> SectionList = null;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // catalogue
        DropdownItems.Add(CatalogueNames);
        SelectedItems.Add(CatalogueNames[0]);
        Catalogue = SelectedItems[0];

        // decking
        SectionList = SqlReader.GetDeckingDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), Catalogue);
        DropdownItems.Add(SectionList);
        SelectedItems.Add(SectionList[0]);
        Profile = SelectedItems[1];

        // steel
        DropdownItems.Add(Enum.GetValues(typeof(DeckingSteelGrade)).Cast<DeckingSteelGrade>().Select(x => x.ToString()).ToList());
        SelectedItems.Add(SteelGrade.ToString());

        First = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropdownItems, SelectedItems, SpacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropdownItems[i][j];

      if (i == 0)  // change is made to code 
      {
        // update selected section to be all
        Catalogue = SelectedItems[0];
        DropdownItems[1] = SqlReader.GetDeckingDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), Catalogue);
      }

      if (i == 1)
      {
        // update displayed selected
        Profile = SelectedItems[1];
      }

      if(i ==2)
      {
        SteelGrade = (DeckingSteelGrade)Enum.Parse(typeof(DeckingSteelGrade), SelectedItems[i]);
      }

        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      SectionList = SqlReader.GetDeckingDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "decking.db3"), Catalogue);
      Catalogue = SelectedItems[0];
      Profile = SelectedItems[1];
      SteelGrade = (DeckingSteelGrade)Enum.Parse(typeof(DeckingSteelGrade), SelectedItems[2]);

      CreateAttributes();
      ExpireSolution(true);
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Deck Config", "DC", "Compos Deck Configuration", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Catalogue Decking", "Dk", "Compos Decking", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      if (this.Params.Input[0].Sources.Count > 0)
      {
        DeckingConfigurationGoo dconf = (DeckingConfigurationGoo)GetInput.GenericGoo<DeckingConfigurationGoo>(this, DA, 0);
        if (dconf == null) { return; }
        DA.SetData(0, new DeckingGoo(new CatalogueDecking(Catalogue, Profile, SteelGrade, dconf.Value)));
      }

      DA.SetData(0, new DeckingGoo(new CatalogueDecking(Catalogue, Profile, SteelGrade, new DeckingConfiguration())));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, DropdownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropdownItems, ref SelectedItems, ref SpacerDescriptions);
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
      //empty
    }

  }
  #endregion
}
