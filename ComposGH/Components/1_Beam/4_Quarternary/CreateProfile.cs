﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Components {
  /// <summary>
  /// Component to create profile
  /// </summary>
  public class CreateProfile : GH_OasysDropDownComponent {
    private enum FoldMode {
      Catalogue,
      Other
    }

    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("dd28f981-592c-4c70-9295-740409300472");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateProfile;

    // for catalogue selection
    // Catalogues
    private readonly Tuple<List<string>, List<int>> Cataloguedata = ComposAPI.Helpers.SqlReader.Instance.GetCataloguesDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));

    private FoldMode _mode = FoldMode.Catalogue;

    private int CatalogueIndex = -1;

    private List<string> CatalogueNames = new List<string>();

    private List<int> CatalogueNumbers = new List<int>();

    private bool comingFromSave = false;

    private List<string> Filteredlist = new List<string>();

    // internal db catalogue numbers
    // list of displayed catalogues
    private bool InclSS;

    private bool lastInputWasSecant;

    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    private int numberOfInputs;

    // list of sections as outcome from selections
    private string ProfileString = "HE HE600.B";

    // temporary manual implementation of profile types (to be replaced by reflection of Oasys.Profiles)
    //Dictionary<string, Type> profileTypes;
    private Dictionary<string, string> ProfileTypes = new Dictionary<string, string> {
      { "Catalogue", "ICatalogueProfile" },
      { "I Beam Asymmetrical", "IIBeamAsymmetricalProfile" },
      { "I Beam Symmetrical", "IIBeamSymmetricalProfile" },
    };

    private string Search = "";

    // Sections
    // list of displayed sections
    private List<string> SectionList = ComposAPI.Helpers.SqlReader.Instance.GetSectionsDataFromSQLite(new List<int> { -1 }, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), false);

    private string Typ = "IRectangleProfile";

    // Types
    private Tuple<List<string>, List<int>> Typedata = ComposAPI.Helpers.SqlReader.Instance.GetTypesDataFromSQLite(-1, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), false);

    //-1 is all
    private int TypeIndex = -1;

    private List<string> TypeNames = new List<string>();

    private List<int> TypeNumbers = new List<int>();

    public CreateProfile() : base("CreateProfile", "Profile", "Create or look up a Profile text-string for a " + BeamGoo.Description + " or a " + BeamSectionGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1()) { Hidden = true; }

    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), reader.GetString("mode"));
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), reader.GetString("lengthUnit"));

      InclSS = reader.GetBoolean("inclSS");
      numberOfInputs = reader.GetInt32("NumberOfInputs");

      CatalogueIndex = reader.GetInt32("catalogueIndex");
      TypeIndex = reader.GetInt32("typeIndex");
      Search = reader.GetString("search");
      comingFromSave = true;
      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) {
      // input -1 to force update of catalogue sections to include/exclude superseded
      bool updateCat = false;
      if (i == -1) {
        _selectedItems[0] = "Catalogue";
        updateCat = true;
        i = 0;
      } else {
        // change selected item
        _selectedItems[i] = _dropDownItems[i][j];
      }

      if (_selectedItems[0] == "Catalogue") {
        // update spacer description to match catalogue dropdowns
        _spacerDescriptions[1] = "Catalogue";

        // if FoldMode is not currently catalogue state, then we update all lists
        if (_mode != FoldMode.Catalogue | updateCat) {
          // remove any existing selections
          while (_selectedItems.Count > 1) {
            _selectedItems.RemoveAt(1);
          }

          // set catalogue selection to all
          CatalogueIndex = -1;

          CatalogueNames = Cataloguedata.Item1;
          CatalogueNumbers = Cataloguedata.Item2;

          // set types to all
          TypeIndex = -1;
          // update typelist with all catalogues
          Typedata = ComposAPI.Helpers.SqlReader.Instance.GetTypesDataFromSQLite(CatalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), InclSS);
          TypeNames = Typedata.Item1;
          TypeNumbers = Typedata.Item2;

          // update section list to all types
          SectionList = ComposAPI.Helpers.SqlReader.Instance.GetSectionsDataFromSQLite(TypeNumbers, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), InclSS);

          // filter by search pattern
          Filteredlist = new List<string>();
          if (Search == "") {
            Filteredlist = SectionList;
          } else {
            for (int k = 0; k < SectionList.Count; k++) {
              if (SectionList[k].ToLower().Contains(Search)) {
                Filteredlist.Add(SectionList[k]);
              }
              if (!Search.Any(char.IsDigit)) {
                string test = SectionList[k].ToString();
                test = Regex.Replace(test, "[0-9]", string.Empty);
                test = test.Replace(".", string.Empty);
                test = test.Replace("-", string.Empty);
                test = test.ToLower();
                if (test.Contains(Search)) {
                  Filteredlist.Add(SectionList[k]);
                }
              }
            }
          }

          // update displayed selections to all
          _selectedItems.Add(CatalogueNames[0]);
          _selectedItems.Add(TypeNames[0]);
          _selectedItems.Add(Filteredlist[0]);

          // call graphics update
          Mode1Clicked();
        }

        // update dropdown lists
        while (_dropDownItems.Count > 1) {
          _dropDownItems.RemoveAt(1);
        }

        // add catalogues (they will always be the same so no need to rerun sql call)
        _dropDownItems.Add(CatalogueNames);

        // type list
        // if second list (i.e. catalogue list) is changed, update types list to account for that catalogue
        if (i == 1) {
          // update catalogue index with the selected catalogue
          CatalogueIndex = CatalogueNumbers[j];
          _selectedItems[1] = CatalogueNames[j];

          // update typelist with selected input catalogue
          Typedata = ComposAPI.Helpers.SqlReader.Instance.GetTypesDataFromSQLite(CatalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), InclSS);
          TypeNames = Typedata.Item1;
          TypeNumbers = Typedata.Item2;

          // update section list from new types (all new types in catalogue)
          var types = TypeNumbers.ToList();
          types.RemoveAt(0); // remove -1 from beginning of list
          SectionList = ComposAPI.Helpers.SqlReader.Instance.GetSectionsDataFromSQLite(types, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), InclSS);

          // filter by search pattern
          Filteredlist = new List<string>();
          if (Search == "") {
            Filteredlist = SectionList;
          } else {
            for (int k = 0; k < SectionList.Count; k++) {
              if (SectionList[k].ToLower().Contains(Search)) {
                Filteredlist.Add(SectionList[k]);
              }
              if (!Search.Any(char.IsDigit)) {
                string test = SectionList[k].ToString();
                test = Regex.Replace(test, "[0-9]", string.Empty);
                test = test.Replace(".", string.Empty);
                test = test.Replace("-", string.Empty);
                test = test.ToLower();
                if (test.Contains(Search)) {
                  Filteredlist.Add(SectionList[k]);
                }
              }
            }
          }

          // update selections to display first item in new list
          _selectedItems[2] = TypeNames[0];
          _selectedItems[3] = Filteredlist[0];
        }
        _dropDownItems.Add(TypeNames);

        // section list
        // if third list (i.e. types list) is changed, update sections list to account for these section types

        if (i == 2) {
          // update catalogue index with the selected catalogue
          TypeIndex = TypeNumbers[j];
          _selectedItems[2] = TypeNames[j];

          // create type list
          var types = new List<int>();
          if (TypeIndex == -1) // if all
          {
            types = TypeNumbers.ToList(); // use current selected list of type numbers
            types.RemoveAt(0); // remove -1 from beginning of list
          } else {
            types = new List<int> { TypeIndex }; // create empty list and add the single selected type
          }

          // section list with selected types (only types in selected type)
          SectionList = ComposAPI.Helpers.SqlReader.Instance.GetSectionsDataFromSQLite(types, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), InclSS);

          // filter by search pattern
          Filteredlist = new List<string>();
          if (Search == "") {
            Filteredlist = SectionList;
          } else {
            for (int k = 0; k < SectionList.Count; k++) {
              if (SectionList[k].ToLower().Contains(Search)) {
                Filteredlist.Add(SectionList[k]);
              }
              if (!Search.Any(char.IsDigit)) {
                string test = SectionList[k].ToString();
                test = Regex.Replace(test, "[0-9]", string.Empty);
                test = test.Replace(".", string.Empty);
                test = test.Replace("-", string.Empty);
                test = test.ToLower();
                if (test.Contains(Search)) {
                  Filteredlist.Add(SectionList[k]);
                }
              }
            }
          }

          // update selected section to be all
          _selectedItems[3] = Filteredlist[0];
        }
        _dropDownItems.Add(Filteredlist);

        // selected profile
        // if fourth list (i.e. section list) is changed, updated the sections list to only be that single profile
        if (i == 3) {
          // update displayed selected
          _selectedItems[3] = Filteredlist[j];
        }
        ProfileString = _selectedItems[3];

        if (!_isInitialised) { return; }
        base.UpdateUI();
      } else {
        // update spacer description to match none-catalogue dropdowns
        _spacerDescriptions[1] = "Measure";// = new List<string>(new string[]

        if (_mode != FoldMode.Other) {
          // remove all catalogue dropdowns
          while (_dropDownItems.Count > 1) {
            _dropDownItems.RemoveAt(1);
          }

          // add length measure dropdown list
          _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));

          // set selected length
          _selectedItems[1] = Length.GetAbbreviation(LengthUnit);
        }

        if (i == 0) {
          // update profile type if change is made to first dropdown menu
          Typ = ProfileTypes[_selectedItems[0]];
          Mode2Clicked();
        } else {
          // change unit
          LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

          base.UpdateUI();
        }
      }
    }

    public override void VariableParameterMaintenance() {
      if (_mode == FoldMode.Catalogue) {
        int i = 0;
        Params.Input[i].NickName = "S";
        Params.Input[i].Name = "Search";
        Params.Input[i].Description = "Text to search from";
        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = true;

        i++;
        Params.Input[i].NickName = "iSS";
        Params.Input[i].Name = "InclSuperseded";
        Params.Input[i].Description = "Input true to include superseded catalogue sections";
        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = true;
      } else {
        string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

        int i = 0;
        //// angle
        //if (Typ == "IAngleProfile") //(typ.Name.Equals(typeof(IAngleProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The depth of the angle profile (leg in the local z axis).";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "W";
        //  Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The width of the angle profile (leg in the local y axis).";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Tw";
        //  Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The web thickness of the angle profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Tf";
        //  Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The flange thickness of the angle profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  //dup = IAngleProfile.Create(angle.Depth, angle.Flange, angle.Web);
        //}

        //// channel
        //else if (Typ == "IChannelProfile") //(typ.Name.Equals(typeof(IChannelProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The depth of the channel profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "B";
        //  Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The width of the flange of the channel profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Tw";
        //  Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The web thickness of the channel profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Tf";
        //  Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The flange thickness of the channel profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = IChannelProfile.Create(channel.Depth, channel.Flanges, channel.Web);
        //}

        //// circle hollow
        //else if (Typ == "ICircleHollowProfile") //(typ.Name.Equals(typeof(ICircleHollowProfile).Name))
        //{
        //  Params.Input[i].NickName = "Ø";
        //  Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The diameter of the hollow circle.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "t";
        //  Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The wall thickness of the hollow circle.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = ICircleHollowProfile.Create(circleHollow.Diameter, circleHollow.WallThickness);
        //}

        //// circle
        //else if (Typ == "ICircleProfile") //(typ.Name.Equals(typeof(ICircleProfile).Name))
        //{
        //  Params.Input[i].NickName = "Ø";
        //  Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The diameter of the circle.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  //dup = ICircleProfile.Create(circle.Diameter);
        //}

        //// ICruciformSymmetricalProfile
        //else if (Typ == "ICruciformSymmetricalProfile") //(typ.Name.Equals(typeof(ICruciformSymmetricalProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The depth (local z axis leg) of the profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "B";
        //  Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The width of the flange (local y axis leg) of the cruciform.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Tw";
        //  Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The web thickness of the cruciform.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Tf";
        //  Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The flange thickness of the cruciform.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = ICruciformSymmetricalProfile.Create(cruciformSymmetrical.Depth, cruciformSymmetrical.Flange, cruciformSymmetrical.Web);
        //}

        //// IEllipseHollowProfile
        //else if (Typ == "IEllipseHollowProfile") //(typ.Name.Equals(typeof(IEllipseHollowProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The depth of the hollow ellipse.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "B";
        //  Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The width of the hollow ellipse.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "t";
        //  Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The wall thickness of the hollow ellipse.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = IEllipseHollowProfile.Create(ellipseHollow.Depth, ellipseHollow.Width, ellipseHollow.WallThickness);
        //}

        //// IEllipseProfile
        //else if (Typ == "IEllipseProfile") //(typ.Name.Equals(typeof(IEllipseProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The depth of the ellipse.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "B";
        //  Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The width of the ellipse.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = IEllipseProfile.Create(ellipse.Depth, ellipse.Width);
        //}

        //// IGeneralCProfile
        //else if (Typ == "IGeneralCProfile") //(typ.Name.Equals(typeof(IGeneralCProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The depth of the generic c section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "B";
        //  Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The flange width of the generic c section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "L";
        //  Params.Input[i].Name = "Lip [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The lip of the generic c section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "t";
        //  Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The thickness of the generic c section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = IGeneralCProfile.Create(generalC.Depth, generalC.FlangeWidth, generalC.Lip, generalC.Thickness);
        //}

        //// IGeneralZProfile
        //else if (Typ == "IGeneralZProfile") //(typ.Name.Equals(typeof(IGeneralZProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The depth of the generic z section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Bt";
        //  Params.Input[i].Name = "TopWidth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The top flange width of the generic z section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Bb";
        //  Params.Input[i].Name = "BottomWidth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The bottom flange width of the generic z section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Lt";
        //  Params.Input[i].Name = "Top Lip [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The top lip of the generic z section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Lb";
        //  Params.Input[i].Name = "Lip [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The top lip of the generic z section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "t";
        //  Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The thickness of the generic z section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = IGeneralZProfile.Create(generalZ.Depth, generalZ.TopFlangeWidth, generalZ.BottomFlangeWidth, generalZ.TopLip, generalZ.BottomLip, generalZ.Thickness);
        //}

        // IIBeamAsymmetricalProfile
        if (Typ == "IIBeamAsymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamAsymmetricalProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bt";
          Params.Input[i].Name = "TopFlangeWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the top flange of the beam. Top is relative to the beam local access.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bb";
          Params.Input[i].Name = "BottomFlangeWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the bottom flange of the beam. Bottom is relative to the beam local access.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "t";
          Params.Input[i].Name = "Web Thickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the beam.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tt";
          Params.Input[i].Name = "TopFlangeThk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top flange thickness.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tb";
          Params.Input[i].Name = "BottomFlangeThk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The bpttom flange thickness.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          //dup = IIBeamAsymmetricalProfile.Create(iBeamAsymmetrical.Depth, iBeamAsymmetrical.TopFlange, iBeamAsymmetrical.BottomFlange, iBeamAsymmetrical.Web);
        }

        //// IIBeamCellularProfile
        //else if (Typ == "IIBeamCellularProfile") //(typ.Name.Equals(typeof(IIBeamCellularProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The depth of the profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "B";
        //  Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The width of the flanges of the beam.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Tw";
        //  Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The web thickness of the angle profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Tf";
        //  Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The flange thickness of the angle profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "O";
        //  Params.Input[i].Name = "WebOpening [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The size of the web opening.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "P";
        //  Params.Input[i].Name = "Pitch [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The pitch (spacing) between the web openings.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = IIBeamCellularProfile.Create(iBeamCellular.Depth, iBeamCellular.Flanges, iBeamCellular.Web, iBeamCellular.WebOpening);
        //}

        // IIBeamSymmetricalProfile
        else if (Typ == "IIBeamSymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamSymmetricalProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the flanges of the beam.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "t";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "T";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          //dup = IIBeamSymmetricalProfile.Create(iBeamSymmetrical.Depth, iBeamSymmetrical.Flanges, iBeamSymmetrical.Web);
        }

        //// IRectangleHollowProfile
        //else if (Typ == "IRectangleHollowProfile") //(typ.Name.Equals(typeof(IRectangleHollowProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The depth of the hollow rectangle.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "B";
        //  Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The width of the hollow rectangle.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Tw";
        //  Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The side thickness of the hollow rectangle.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Tf";
        //  Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The top/bottom thickness of the hollow rectangle.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = IRectangleHollowProfile.Create(rectangleHollow.Depth, rectangleHollow.Flanges, rectangleHollow.Webs);
        //}

        //// IRectangleProfile
        //else if (Typ == "IRectangleProfile") //(typ.Name.Equals(typeof(IRectangleProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "Depth of the rectangle, in local z-axis direction.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "B";
        //  Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "Width of the rectangle, in loca y-axis direction.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = IRectangleProfile.Create(rectangle.Depth, rectangle.Width);
        //}

        //// IRectoEllipseProfile
        //else if (Typ == "IRectoEllipseProfile") //(typ.Name.Equals(typeof(IRectoEllipseProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The overall depth of the recto-ellipse profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Df";
        //  Params.Input[i].Name = "DepthFlat [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The flat length of the profile's overall depth.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "B";
        //  Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The overall width of the recto-ellipse profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Bf";
        //  Params.Input[i].Name = "WidthFlat [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The flat length of the profile's overall width.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = IRectoEllipseProfile.Create(rectoEllipse.Depth, rectoEllipse.DepthFlat, rectoEllipse.Width, rectoEllipse.WidthFlat);
        //}

        //// ISecantPileProfile
        //else if (Typ == "ISecantPileProfile") //(typ.Name.Equals(typeof(ISecantPileProfile).Name))
        //{
        //  Params.Input[i].NickName = "Ø";
        //  Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The diameter of the piles.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "c/c";
        //  Params.Input[i].Name = "PileCentres [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The centre to centre distance between adjacent piles.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "No";
        //  Params.Input[i].Name = "PileCount";
        //  Params.Input[i].Description = "The number of piles in the profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "W/S";
        //  Params.Input[i].Name = "isWall";
        //  Params.Input[i].Description = "Converts the profile into a wall secant pile profile if true -- Converts the profile into a section secant pile profile if false.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = ISecantPileProfile.Create(secantPile.Diameter, secantPile.PileCentres, secantPile.PileCount, secantPile.IsWallNotSection);
        //}

        //// ISheetPileProfile
        //else if (Typ == "ISheetPileProfile") //(typ.Name.Equals(typeof(ISheetPileProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The depth of the sheet pile section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "B";
        //  Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The overall width of the sheet pile section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Bt";
        //  Params.Input[i].Name = "TopFlangeWidth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The top flange width of the sheet pile section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Bb";
        //  Params.Input[i].Name = "BottomFlangeWidth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The bottom flange width of the sheet pile section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Ft";
        //  Params.Input[i].Name = "FlangeThickness [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The flange thickness of the sheet pile section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Wt";
        //  Params.Input[i].Name = "WebThickness [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The web thickness of the sheet pile section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = ISheetPileProfile.Create(sheetPile.Depth, sheetPile.Width, sheetPile.TopFlangeWidth, sheetPile.BottomFlangeWidth, sheetPile.FlangeThickness, sheetPile.WebThickness);
        //}

        //// IStadiumProfile
        //else if (Typ == "IStadiumProfile") //(typ.Name.Equals(typeof(IStadiumProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The profile's overall depth considering the side length of the rectangle and the radii of the semicircles on the two ends.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "B";
        //  Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The profile's width (diameter of the semicircles).";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = IStadiumProfile.Create(stadium.Depth, stadium.Width);
        //}

        //// ITrapezoidProfile
        //else if (Typ == "ITrapezoidProfile") //(typ.Name.Equals(typeof(ITrapezoidProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The depth in z-axis direction of trapezoidal profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Bt";
        //  Params.Input[i].Name = "TopWidth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The top width of trapezoidal profile. Top is relative to the local z-axis.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Bb";
        //  Params.Input[i].Name = "BottomWidth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The bottom width of trapezoidal profile. Bottom is relative to the local z-axis.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;
        //  //dup = ITrapezoidProfile.Create(trapezoid.Depth, trapezoid.TopWidth, trapezoid.BottomWidth);
        //}

        //// ITSectionProfile
        //else if (Typ == "ITSectionProfile") //(typ.Name.Equals(typeof(ITSectionProfile).Name))
        //{
        //  Params.Input[i].NickName = "D";
        //  Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The depth of the T section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "B";
        //  Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The width of the T section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Tw";
        //  Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The web thickness of the T section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  i++;
        //  Params.Input[i].NickName = "Tf";
        //  Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
        //  Params.Input[i].Description = "The flange thickness of the T section profile.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  //dup = ITSectionProfile.Create(tSection.Depth, tSection.Flange, tSection.Web);
        //}
        //// IPerimeterProfile
        //else if (Typ == "IPerimeterProfile") //(typ.Name.Equals(typeof(IPerimeterProfile).Name))
        //{
        //  Params.Input[i].NickName = "B";
        //  Params.Input[i].Name = "Boundary";
        //  Params.Input[i].Description = "Planar Brep or closed planar curve.";
        //  Params.Input[i].Access = GH_ParamAccess.item;
        //  Params.Input[i].Optional = false;

        //  //i++;
        //  //Params.Input[i].NickName = "V";
        //  //Params.Input[i].Name = "[Optional] VoidPolylines";
        //  //Params.Input[i].Description = "The void polygons within the solid polygon of the perimeter profile. If first input is a BRep this input will be ignored.";
        //  //Params.Input[i].Access = GH_ParamAccess.list;
        //  //Params.Input[i].Optional = true;
        //}
      }
    }

    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      writer.SetString("mode", _mode.ToString());
      writer.SetString("lengthUnit", LengthUnit.ToString());
      writer.SetBoolean("inclSS", InclSS);
      writer.SetInt32("NumberOfInputs", numberOfInputs);
      writer.SetInt32("catalogueIndex", CatalogueIndex);
      writer.SetInt32("typeIndex", TypeIndex);
      writer.SetString("search", Search);
      return base.Write(writer);
    }

    protected override string HtmlHelp_Source() {
      string help = "GOTO:https://arup-group.github.io/oasys-combined/adsec-api/api/Oasys.Profiles.html";
      return help;
    }

    // sets the initial state of the component to hidden

    //  internal db type numbers
    // list of displayed types
    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Profile type", "Measure", "Type", "Profile" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(ProfileTypes.Keys.ToList());
      _selectedItems.Add("Catalogue");

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      SetSelected(-1, 0);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Search", "S", "Text to search from", GH_ParamAccess.item);
      pManager.AddBooleanParameter("InclSuperseded", "iSS", "Input true to include superseded catalogue sections", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new BeamSectionParam());
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      ClearRuntimeMessages();
      for (int i = 0; i < Params.Input.Count; i++) {
        Params.Input[i].ClearRuntimeMessages();
      }

      #region catalogue
      ClearRuntimeMessages();
      if (_mode == FoldMode.Catalogue) {
        // get user input filter search string
        bool incl = false;
        if (DA.GetData(1, ref incl)) {
          if (InclSS != incl) {
            SetSelected(-1, 0);
            ExpireSolution(true);
          }
        }

        // get user input filter search string
        string inSearch = "";
        if (DA.GetData(0, ref inSearch)) {
          inSearch = inSearch.ToLower().Replace(" ", string.Empty).Replace(".", string.Empty);
        }
        if (!inSearch.Equals(Search)) {
          Search = inSearch;
          SetSelected(-1, 0);
          ExpireSolution(true);
        }

        DA.SetData(0, new BeamSectionGoo(new BeamSection("CAT " + ProfileString)));

        return;
      }
      #endregion

      #region other
      if (_mode == FoldMode.Other) {
        string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

        //IProfile profile = null;
        string unit = "(" + unitAbbreviation + ") ";
        string profile = "STD ";

        // IIBeamAsymmetricalProfile
        if (Typ == "IIBeamAsymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamAsymmetricalProfile).Name))
        {
          profile += "GI" + unit +
              Input.UnitNumber(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.UnitNumber(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.UnitNumber(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.UnitNumber(this, DA, 3, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.UnitNumber(this, DA, 4, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.UnitNumber(this, DA, 5, LengthUnit).As(LengthUnit).ToString();

          //profile = IIBeamAsymmetricalProfile.Create(
          //    Input.UnitNumber(this, DA, 0, lengthUnit),
          //    GetInput.Flange(this, DA, 1),
          //    GetInput.Flange(this, DA, 2),
          //    GetInput.Web(this, DA, 3));
        }
        // IIBeamSymmetricalProfile
        else if (Typ == "IIBeamSymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamSymmetricalProfile).Name))
        {
          profile += "I" + unit +
             Input.UnitNumber(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.UnitNumber(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.UnitNumber(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.UnitNumber(this, DA, 3, LengthUnit).As(LengthUnit).ToString();

          //profile = IIBeamSymmetricalProfile.Create(
          //    Input.UnitNumber(this, DA, 0, lengthUnit),
          //    GetInput.Flange(this, DA, 1),
          //    (IWebConstant)GetInput.Web(this, DA, 2));
        } else {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to create profile");
          return;
        }

        DA.SetData(0, new BeamSectionGoo(new BeamSection(profile)));
      }
      #endregion
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems[0] == "Catalogue") {
        // update spacer description to match catalogue dropdowns
        _spacerDescriptions = new List<string>(new string[]
        {
          "Profile type", "Catalogue", "Type", "Profile"
        });

        CatalogueNames = Cataloguedata.Item1;
        CatalogueNumbers = Cataloguedata.Item2;
        Typedata = ComposAPI.Helpers.SqlReader.Instance.GetTypesDataFromSQLite(CatalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), InclSS);
        TypeNames = Typedata.Item1;
        TypeNumbers = Typedata.Item2;

        // call graphics update
        comingFromSave = true;
        Mode1Clicked();
        comingFromSave = false;

        ProfileString = _selectedItems[3];
      } else {
        // update spacer description to match none-catalogue dropdowns
        _spacerDescriptions = new List<string>(new string[]
        {
          "Profile type", "Measure", "Type", "Profile"
        });

        Typ = ProfileTypes[_selectedItems[0]];
        Mode2Clicked();
      }

      base.UpdateUIFromSelectedItems();
    }

    private void Mode1Clicked() {
      if (_mode == FoldMode.Catalogue) {
        if (!comingFromSave) {
          return;
        }
      }

      //remove input parameters
      while (Params.Input.Count > 0) {
        Params.UnregisterInputParameter(Params.Input[0], true);
      }

      //register input parameter
      Params.RegisterInputParam(new Param_String());
      Params.RegisterInputParam(new Param_Boolean());

      _mode = FoldMode.Catalogue;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    private void Mode2Clicked() {
      // check if mode is correct
      if (_mode != FoldMode.Other) {
        // if we come from catalogue mode remove all input parameters
        while (Params.Input.Count > 0) {
          Params.UnregisterInputParameter(Params.Input[0], true);
        }

        // set mode to other
        _mode = FoldMode.Other;
      }

      // angle
      if (Typ == "IAngleProfile") //(typ.Name.Equals(typeof(IAngleProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IAngleProfile.Create(angle.Depth, angle.Flange, angle.Web);
      }

      // channel
      else if (Typ == "IChannelProfile") //(typ.Name.Equals(typeof(IChannelProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IChannelProfile.Create(channel.Depth, channel.Flanges, channel.Web);
      }

      // circle hollow
      else if (Typ == "ICircleHollowProfile") //(typ.Name.Equals(typeof(ICircleHollowProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = ICircleHollowProfile.Create(circleHollow.Diameter, circleHollow.WallThickness);
      }

      // circle
      else if (Typ == "ICircleProfile") //(typ.Name.Equals(typeof(ICircleProfile).Name))
      {
        SetNumberOfGenericInputs(1);
        //dup = ICircleProfile.Create(circle.Diameter);
      }

      // ICruciformSymmetricalProfile
      else if (Typ == "ICruciformSymmetricalProfile") //(typ.Name.Equals(typeof(ICruciformSymmetricalProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = ICruciformSymmetricalProfile.Create(cruciformSymmetrical.Depth, cruciformSymmetrical.Flange, cruciformSymmetrical.Web);
      }

      // IEllipseHollowProfile
      else if (Typ == "IEllipseHollowProfile") //(typ.Name.Equals(typeof(IEllipseHollowProfile).Name))
      {
        SetNumberOfGenericInputs(3);
        //dup = IEllipseHollowProfile.Create(ellipseHollow.Depth, ellipseHollow.Width, ellipseHollow.WallThickness);
      }

      // IEllipseProfile
      else if (Typ == "IEllipseProfile") //(typ.Name.Equals(typeof(IEllipseProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = IEllipseProfile.Create(ellipse.Depth, ellipse.Width);
      }

      // IGeneralCProfile
      else if (Typ == "IGeneralCProfile") //(typ.Name.Equals(typeof(IGeneralCProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IGeneralCProfile.Create(generalC.Depth, generalC.FlangeWidth, generalC.Lip, generalC.Thickness);
      }

      // IGeneralZProfile
      else if (Typ == "IGeneralZProfile") //(typ.Name.Equals(typeof(IGeneralZProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = IGeneralZProfile.Create(generalZ.Depth, generalZ.TopFlangeWidth, generalZ.BottomFlangeWidth, generalZ.TopLip, generalZ.BottomLip, generalZ.Thickness);
      }

      // IIBeamAsymmetricalProfile
      else if (Typ == "IIBeamAsymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamAsymmetricalProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = IIBeamAsymmetricalProfile.Create(iBeamAsymmetrical.Depth, iBeamAsymmetrical.TopFlange, iBeamAsymmetrical.BottomFlange, iBeamAsymmetrical.Web);
      }

      // IIBeamCellularProfile
      else if (Typ == "IIBeamCellularProfile") //(typ.Name.Equals(typeof(IIBeamCellularProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IIBeamCellularProfile.Create(iBeamCellular.Depth, iBeamCellular.Flanges, iBeamCellular.Web, iBeamCellular.WebOpening);
      }

      // IIBeamSymmetricalProfile
      else if (Typ == "IIBeamSymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamSymmetricalProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IIBeamSymmetricalProfile.Create(iBeamSymmetrical.Depth, iBeamSymmetrical.Flanges, iBeamSymmetrical.Web);
      }

      // IRectangleHollowProfile
      else if (Typ == "IRectangleHollowProfile") //(typ.Name.Equals(typeof(IRectangleHollowProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IRectangleHollowProfile.Create(rectangleHollow.Depth, rectangleHollow.Flanges, rectangleHollow.Webs);
      }

      // IRectangleProfile
      else if (Typ == "IRectangleProfile") //(typ.Name.Equals(typeof(IRectangleProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = IRectangleProfile.Create(rectangle.Depth, rectangle.Width);
      }

      // IRectoEllipseProfile
      else if (Typ == "IRectoEllipseProfile") //(typ.Name.Equals(typeof(IRectoEllipseProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IRectoEllipseProfile.Create(rectoEllipse.Depth, rectoEllipse.DepthFlat, rectoEllipse.Width, rectoEllipse.WidthFlat);
      }

      // ISecantPileProfile
      else if (Typ == "ISecantPileProfile") //(typ.Name.Equals(typeof(ISecantPileProfile).Name))
      {
        SetNumberOfGenericInputs(4, true);
        //dup = ISecantPileProfile.Create(secantPile.Diameter, secantPile.PileCentres, secantPile.PileCount, secantPile.IsWallNotSection);
      }

      // ISheetPileProfile
      else if (Typ == "ISheetPileProfile") //(typ.Name.Equals(typeof(ISheetPileProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = ISheetPileProfile.Create(sheetPile.Depth, sheetPile.Width, sheetPile.TopFlangeWidth, sheetPile.BottomFlangeWidth, sheetPile.FlangeThickness, sheetPile.WebThickness);
      }

      // IStadiumProfile
      else if (Typ == "IStadiumProfile") //(typ.Name.Equals(typeof(IStadiumProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = IStadiumProfile.Create(stadium.Depth, stadium.Width);
      }

      // ITrapezoidProfile
      else if (Typ == "ITrapezoidProfile") //(typ.Name.Equals(typeof(ITrapezoidProfile).Name))
      {
        SetNumberOfGenericInputs(3);
        //dup = ITrapezoidProfile.Create(trapezoid.Depth, trapezoid.TopWidth, trapezoid.BottomWidth);
      }

      // ITSectionProfile
      else if (Typ == "ITSectionProfile") //(typ.Name.Equals(typeof(ITSectionProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = ITSectionProfile.Create(tSection.Depth, tSection.Flange, tSection.Web);
      }
      // IPerimeterProfile
      else if (Typ == "IPerimeterProfile") //(typ.Name.Equals(typeof(IPerimeterProfile).Name))
      {
        SetNumberOfGenericInputs(1);
        //dup = IPerimeterProfile.Create();
        //solidPolygon;
        //voidPolygons;
      }

        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    private void SetNumberOfGenericInputs(int inputs, bool isSecantPile = false) {
      numberOfInputs = inputs;

      // if last input previously was a bool and we no longer need that
      if (lastInputWasSecant || isSecantPile) {
        if (Params.Input.Count > 0) {
          // make sure to remove last param
          Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
          Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
        }
      }

      // remove any additional inputs
      while (Params.Input.Count > inputs) {
        Params.UnregisterInputParameter(Params.Input[inputs], true);
      }

      if (isSecantPile) // add two less generic than input says
      {
        while (Params.Input.Count > inputs + 2) {
          Params.UnregisterInputParameter(Params.Input[inputs + 2], true);
        }
        inputs -= 2;
      }

      // add inputs parameter
      while (Params.Input.Count < inputs) {
        Params.RegisterInputParam(new Param_GenericObject());
      }

      if (isSecantPile) // finally add int and bool param if secant
      {
        Params.RegisterInputParam(new Param_Integer());
        Params.RegisterInputParam(new Param_Boolean());
        lastInputWasSecant = true;
      }
    }
  }
}
