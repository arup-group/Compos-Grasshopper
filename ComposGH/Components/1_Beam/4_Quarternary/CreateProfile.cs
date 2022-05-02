﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using System.Text.RegularExpressions;
using Grasshopper.Kernel.Parameters;
using System.Linq;
using System.IO;
using System.Reflection;
using UnitsNet;
using ComposGH.Helpers;
using ComposGH.Parameters;
using ComposAPI;

namespace ComposGH.Components
{
  /// <summary>
  /// Component to create AdSec profile
  /// </summary>
  public class CreateProfile : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("dd28f981-592c-4c70-9295-740409300472");
    public CreateProfile()
      : base("Create Profile", "Profile", "Create Profile text-string for Compos Beam",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    protected override string HtmlHelp_Source()
    {
      string help = "GOTO:https://arup-group.github.io/oasys-combined/adsec-api/api/Oasys.Profiles.html";
      return help;
    }

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateProfile;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        if (selecteditems == null)
        {
          // create a new list of selected items and add the first material type
          selecteditems = new List<string>();
          selecteditems.Add("Catalogue");
        }
        if (dropdownitems == null)
        {
          // create a new list of selected items and add the first material type
          dropdownitems = new List<List<string>>();
          dropdownitems.Add(profileTypes.Keys.ToList());
        }

        // length
        dropdownitems.Add(Units.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());

        SetSelected(-1, 0);
        first = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // input -1 to force update of catalogue sections to include/exclude superseeded
      bool updateCat = false;
      if (i == -1)
      {
        selecteditems[0] = "Catalogue";
        updateCat = true;
        i = 0;
      }
      else
      {
        // change selected item
        selecteditems[i] = dropdownitems[i][j];
      }

      if (selecteditems[0] == "Catalogue")
      {
        // update spacer description to match catalogue dropdowns
        spacerDescriptions[1] = "Catalogue";

        // if FoldMode is not currently catalogue state, then we update all lists
        if (_mode != FoldMode.Catalogue | updateCat)
        {
          // remove any existing selections
          while (selecteditems.Count > 1)
            selecteditems.RemoveAt(1);

          // set catalogue selection to all
          catalogueIndex = -1;

          catalogueNames = cataloguedata.Item1;
          catalogueNumbers = cataloguedata.Item2;

          // set types to all
          typeIndex = -1;
          // update typelist with all catalogues
          typedata = SqlReader.GetTypesDataFromSQLite(catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), inclSS);
          typeNames = typedata.Item1;
          typeNumbers = typedata.Item2;

          // update section list to all types
          sectionList = SqlReader.GetSectionsDataFromSQLite(typeNumbers, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), inclSS);

          // filter by search pattern
          filteredlist = new List<string>();
          if (search == "")
          {
            filteredlist = sectionList;
          }
          else
          {
            for (int k = 0; k < sectionList.Count; k++)
            {
              if (sectionList[k].ToLower().Contains(search))
              {
                filteredlist.Add(sectionList[k]);
              }
              if (!search.Any(char.IsDigit))
              {
                string test = sectionList[k].ToString();
                test = Regex.Replace(test, "[0-9]", string.Empty);
                test = test.Replace(".", string.Empty);
                test = test.Replace("-", string.Empty);
                test = test.ToLower();
                if (test.Contains(search))
                {
                  filteredlist.Add(sectionList[k]);
                }
              }
            }
          }

          // update displayed selections to all
          selecteditems.Add(catalogueNames[0]);
          selecteditems.Add(typeNames[0]);
          selecteditems.Add(filteredlist[0]);

          // call graphics update
          Mode1Clicked();
        }

        // update dropdown lists
        while (dropdownitems.Count > 1)
          dropdownitems.RemoveAt(1);

        // add catalogues (they will always be the same so no need to rerun sql call)
        dropdownitems.Add(catalogueNames);

        // type list
        // if second list (i.e. catalogue list) is changed, update types list to account for that catalogue
        if (i == 1)
        {
          // update catalogue index with the selected catalogue
          catalogueIndex = catalogueNumbers[j];
          selecteditems[1] = catalogueNames[j];

          // update typelist with selected input catalogue
          typedata = SqlReader.GetTypesDataFromSQLite(catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), inclSS);
          typeNames = typedata.Item1;
          typeNumbers = typedata.Item2;

          // update section list from new types (all new types in catalogue)
          List<int> types = typeNumbers.ToList();
          types.RemoveAt(0); // remove -1 from beginning of list
          sectionList = SqlReader.GetSectionsDataFromSQLite(types, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), inclSS);

          // filter by search pattern
          filteredlist = new List<string>();
          if (search == "")
          {
            filteredlist = sectionList;
          }
          else
          {
            for (int k = 0; k < sectionList.Count; k++)
            {
              if (sectionList[k].ToLower().Contains(search))
              {
                filteredlist.Add(sectionList[k]);
              }
              if (!search.Any(char.IsDigit))
              {
                string test = sectionList[k].ToString();
                test = Regex.Replace(test, "[0-9]", string.Empty);
                test = test.Replace(".", string.Empty);
                test = test.Replace("-", string.Empty);
                test = test.ToLower();
                if (test.Contains(search))
                {
                  filteredlist.Add(sectionList[k]);
                }
              }
            }
          }

          // update selections to display first item in new list
          selecteditems[2] = typeNames[0];
          selecteditems[3] = filteredlist[0];
        }
        dropdownitems.Add(typeNames);

        // section list
        // if third list (i.e. types list) is changed, update sections list to account for these section types

        if (i == 2)
        {
          // update catalogue index with the selected catalogue
          typeIndex = typeNumbers[j];
          selecteditems[2] = typeNames[j];

          // create type list
          List<int> types = new List<int>();
          if (typeIndex == -1) // if all
          {
            types = typeNumbers.ToList(); // use current selected list of type numbers
            types.RemoveAt(0); // remove -1 from beginning of list
          }
          else
            types = new List<int> { typeIndex }; // create empty list and add the single selected type 


          // section list with selected types (only types in selected type)
          sectionList = SqlReader.GetSectionsDataFromSQLite(types, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), inclSS);

          // filter by search pattern
          filteredlist = new List<string>();
          if (search == "")
          {
            filteredlist = sectionList;
          }
          else
          {
            for (int k = 0; k < sectionList.Count; k++)
            {
              if (sectionList[k].ToLower().Contains(search))
              {
                filteredlist.Add(sectionList[k]);
              }
              if (!search.Any(char.IsDigit))
              {
                string test = sectionList[k].ToString();
                test = Regex.Replace(test, "[0-9]", string.Empty);
                test = test.Replace(".", string.Empty);
                test = test.Replace("-", string.Empty);
                test = test.ToLower();
                if (test.Contains(search))
                {
                  filteredlist.Add(sectionList[k]);
                }
              }
            }
          }

          // update selected section to be all
          selecteditems[3] = filteredlist[0];
        }
        dropdownitems.Add(filteredlist);

        // selected profile
        // if fourth list (i.e. section list) is changed, updated the sections list to only be that single profile
        if (i == 3)
        {
          // update displayed selected
          selecteditems[3] = filteredlist[j];
        }
        profileString = selecteditems[3];

        if (first) { return; }
          (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        ExpireSolution(true);
        Params.OnParametersChanged();
        this.OnDisplayExpired(true);
      }
      else
      {
        // update spacer description to match none-catalogue dropdowns
        spacerDescriptions[1] = "Measure";// = new List<string>(new string[]

        if (_mode != FoldMode.Other)
        {
          // remove all catalogue dropdowns
          while (dropdownitems.Count > 1)
            dropdownitems.RemoveAt(1);

          // add length measure dropdown list
          dropdownitems.Add(Units.FilteredLengthUnits);

          // set selected length
          selecteditems[1] = lengthUnit.ToString();
        }

        if (i == 0)
        {
          // update profile type if change is made to first dropdown menu
          typ = profileTypes[selecteditems[0]];
          Mode2Clicked();
        }
        else
        {
          // change unit
          lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[i]);

          IQuantity quantity = new Length(0, lengthUnit);
          unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

          // update name of inputs (to display unit on sliders)
          (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
          ExpireSolution(true);
          Params.OnParametersChanged();
          this.OnDisplayExpired(true);
        }
      }
    }

    private void UpdateUIFromSelectedItems()
    {

      if (selecteditems[0] == "Catalogue")
      {
        // update spacer description to match catalogue dropdowns
        spacerDescriptions = new List<string>(new string[]
        {
          "Profile type", "Catalogue", "Type", "Profile"
        });

        catalogueNames = cataloguedata.Item1;
        catalogueNumbers = cataloguedata.Item2;
        typedata = SqlReader.GetTypesDataFromSQLite(catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), inclSS);
        typeNames = typedata.Item1;
        typeNumbers = typedata.Item2;

        // call graphics update
        comingFromSave = true;
        Mode1Clicked();
        comingFromSave = false;

        profileString = selecteditems[3];
      }
      else
      {
        // update spacer description to match none-catalogue dropdowns
        spacerDescriptions = new List<string>(new string[]
        {
          "Profile type", "Measure", "Type", "Profile"
        });

        typ = profileTypes[selecteditems[0]];
        Mode2Clicked();
      }

      IQuantity quantity = new Length(0, lengthUnit);
      unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      CreateAttributes();
      ExpireSolution(true);
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion


    #region Input and output
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Profile type", "Measure", "Type", "Profile"
    });

    // temporary manual implementation of profile types (to be replaced by reflection of Oasys.Profiles)
    //Dictionary<string, Type> profileTypes;
    Dictionary<string, string> profileTypes = new Dictionary<string, string>
        {
            { "Catalogue", "ICatalogueProfile" },
            { "I Beam Asymmetrical", "IIBeamAsymmetricalProfile" },
            { "I Beam Symmetrical", "IIBeamSymmetricalProfile" },
        };

    private UnitsNet.Units.LengthUnit lengthUnit = Units.LengthUnitSection;
    string unitAbbreviation;

    #region catalogue sections
    // for catalogue selection
    // Catalogues
    readonly Tuple<List<string>, List<int>> cataloguedata = SqlReader.GetCataloguesDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
    List<int> catalogueNumbers = new List<int>(); // internal db catalogue numbers
    List<string> catalogueNames = new List<string>(); // list of displayed catalogues
    bool inclSS;

    // Types
    Tuple<List<string>, List<int>> typedata = SqlReader.GetTypesDataFromSQLite(-1, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), false);
    List<int> typeNumbers = new List<int>(); //  internal db type numbers
    List<string> typeNames = new List<string>(); // list of displayed types

    // Sections
    // list of displayed sections
    List<string> sectionList = SqlReader.GetSectionsDataFromSQLite(new List<int> { -1 }, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), false);
    List<string> filteredlist = new List<string>();
    int catalogueIndex = -1; //-1 is all
    int typeIndex = -1;
    // list of sections as outcome from selections
    string profileString = "HE HE600.B";
    string search = "";
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddTextParameter("Search", "S", "Text to search from", GH_ParamAccess.item);
      pManager.AddBooleanParameter("InclSuperseeded", "iSS", "Input true to include superseeded catalogue sections", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Beam Section", "Bs", "Beam Section for a Compos Beam", GH_ParamAccess.item);
    }
    #endregion
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      this.ClearRuntimeMessages();
      for (int i = 0; i < this.Params.Input.Count; i++)
        this.Params.Input[i].ClearRuntimeMessages();

      #region catalogue
      this.ClearRuntimeMessages();
      if (_mode == FoldMode.Catalogue)
      {
        // get user input filter search string
        bool incl = false;
        if (DA.GetData(1, ref incl))
        {
          if (inclSS != incl)
          {
            SetSelected(-1, 0);
            this.ExpireSolution(true);
          }
        }

        // get user input filter search string
        string inSearch = "";
        if (DA.GetData(0, ref inSearch))
        {
          inSearch = inSearch.ToLower();

        }
        if (!inSearch.Equals(search))
        {
          search = inSearch.ToString();
          SetSelected(-1, 0);
          this.ExpireSolution(true);
        }

        DA.SetData(0, new BeamSectionGoo(new BeamSection("CAT " + profileString)));

        return;
      }
      #endregion
      #region other
      if (_mode == FoldMode.Other)
      {
        //IProfile profile = null;
        string unit = "(" + unitAbbreviation + ") ";
        string profile = "STD ";

        // IIBeamAsymmetricalProfile
        if (typ == "IIBeamAsymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamAsymmetricalProfile).Name))
        {
          profile += "GI" + unit +
              GetInput.Length(this, DA, 0, lengthUnit).As(lengthUnit).ToString() + " " +
              GetInput.Length(this, DA, 1, lengthUnit).As(lengthUnit).ToString() + " " +
              GetInput.Length(this, DA, 2, lengthUnit).As(lengthUnit).ToString() + " " +
              GetInput.Length(this, DA, 3, lengthUnit).As(lengthUnit).ToString() + " " +
              GetInput.Length(this, DA, 4, lengthUnit).As(lengthUnit).ToString() + " " +
              GetInput.Length(this, DA, 5, lengthUnit).As(lengthUnit).ToString();

          //profile = IIBeamAsymmetricalProfile.Create(
          //    GetInput.Length(this, DA, 0, lengthUnit),
          //    GetInput.Flange(this, DA, 1),
          //    GetInput.Flange(this, DA, 2),
          //    GetInput.Web(this, DA, 3));
        }
        // IIBeamSymmetricalProfile
        else if (typ == "IIBeamSymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamSymmetricalProfile).Name))
        {
          profile += "I" + unit +
             GetInput.Length(this, DA, 0, lengthUnit).As(lengthUnit).ToString() + " " +
             GetInput.Length(this, DA, 1, lengthUnit).As(lengthUnit).ToString() + " " +
             GetInput.Length(this, DA, 2, lengthUnit).As(lengthUnit).ToString() + " " +
             GetInput.Length(this, DA, 3, lengthUnit).As(lengthUnit).ToString();

          //profile = IIBeamSymmetricalProfile.Create(
          //    GetInput.Length(this, DA, 0, lengthUnit),
          //    GetInput.Flange(this, DA, 1),
          //    (IWebConstant)GetInput.Web(this, DA, 2));
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to create profile");
          return;
        }

        DA.SetData(0, new BeamSectionGoo(new BeamSection(profile)));
        return;
      }
      #endregion
    }


    #region menu override
    private enum FoldMode
    {
      Catalogue,
      Other
    }
    private bool first = true;
    private FoldMode _mode = FoldMode.Catalogue;


    private void Mode1Clicked()
    {
      if (_mode == FoldMode.Catalogue)
        if (!comingFromSave) { return; }

      //remove input parameters
      while (Params.Input.Count > 0)
        Params.UnregisterInputParameter(Params.Input[0], true);

      //register input parameter
      Params.RegisterInputParam(new Param_String());
      Params.RegisterInputParam(new Param_Boolean());

      _mode = FoldMode.Catalogue;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
    private void SetNumberOfGenericInputs(int inputs, bool isSecantPile = false)
    {
      numberOfInputs = inputs;

      // if last input previously was a bool and we no longer need that
      if (lastInputWasSecant || isSecantPile)
      {
        if (Params.Input.Count > 0)
        {
          // make sure to remove last param
          Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
          Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
        }
      }

      // remove any additional inputs
      while (Params.Input.Count > inputs)
        Params.UnregisterInputParameter(Params.Input[inputs], true);

      if (isSecantPile) // add two less generic than input says
      {
        while (Params.Input.Count > inputs + 2)
          Params.UnregisterInputParameter(Params.Input[inputs + 2], true);
        inputs -= 2;
      }

      // add inputs parameter
      while (Params.Input.Count < inputs)
        Params.RegisterInputParam(new Param_GenericObject());

      if (isSecantPile) // finally add int and bool param if secant
      {
        Params.RegisterInputParam(new Param_Integer());
        Params.RegisterInputParam(new Param_Boolean());
        lastInputWasSecant = true;
      }
    }
    private bool lastInputWasSecant;
    private int numberOfInputs;
    private string typ = "IRectangleProfile";
    private void Mode2Clicked()
    {
      // check if mode is correct
      if (_mode != FoldMode.Other)
      {
        // if we come from catalogue mode remove all input parameters
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        // set mode to other
        _mode = FoldMode.Other;
      }

      // angle
      if (typ == "IAngleProfile") //(typ.Name.Equals(typeof(IAngleProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IAngleProfile.Create(angle.Depth, angle.Flange, angle.Web);
      }

      // channel
      else if (typ == "IChannelProfile") //(typ.Name.Equals(typeof(IChannelProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IChannelProfile.Create(channel.Depth, channel.Flanges, channel.Web);
      }

      // circle hollow
      else if (typ == "ICircleHollowProfile") //(typ.Name.Equals(typeof(ICircleHollowProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = ICircleHollowProfile.Create(circleHollow.Diameter, circleHollow.WallThickness);
      }

      // circle
      else if (typ == "ICircleProfile") //(typ.Name.Equals(typeof(ICircleProfile).Name))
      {
        SetNumberOfGenericInputs(1);
        //dup = ICircleProfile.Create(circle.Diameter);
      }

      // ICruciformSymmetricalProfile
      else if (typ == "ICruciformSymmetricalProfile") //(typ.Name.Equals(typeof(ICruciformSymmetricalProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = ICruciformSymmetricalProfile.Create(cruciformSymmetrical.Depth, cruciformSymmetrical.Flange, cruciformSymmetrical.Web);
      }

      // IEllipseHollowProfile
      else if (typ == "IEllipseHollowProfile") //(typ.Name.Equals(typeof(IEllipseHollowProfile).Name))
      {
        SetNumberOfGenericInputs(3);
        //dup = IEllipseHollowProfile.Create(ellipseHollow.Depth, ellipseHollow.Width, ellipseHollow.WallThickness);
      }

      // IEllipseProfile
      else if (typ == "IEllipseProfile") //(typ.Name.Equals(typeof(IEllipseProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = IEllipseProfile.Create(ellipse.Depth, ellipse.Width);
      }

      // IGeneralCProfile
      else if (typ == "IGeneralCProfile") //(typ.Name.Equals(typeof(IGeneralCProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IGeneralCProfile.Create(generalC.Depth, generalC.FlangeWidth, generalC.Lip, generalC.Thickness);
      }

      // IGeneralZProfile
      else if (typ == "IGeneralZProfile") //(typ.Name.Equals(typeof(IGeneralZProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = IGeneralZProfile.Create(generalZ.Depth, generalZ.TopFlangeWidth, generalZ.BottomFlangeWidth, generalZ.TopLip, generalZ.BottomLip, generalZ.Thickness);
      }

      // IIBeamAsymmetricalProfile
      else if (typ == "IIBeamAsymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamAsymmetricalProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = IIBeamAsymmetricalProfile.Create(iBeamAsymmetrical.Depth, iBeamAsymmetrical.TopFlange, iBeamAsymmetrical.BottomFlange, iBeamAsymmetrical.Web);
      }

      // IIBeamCellularProfile
      else if (typ == "IIBeamCellularProfile") //(typ.Name.Equals(typeof(IIBeamCellularProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IIBeamCellularProfile.Create(iBeamCellular.Depth, iBeamCellular.Flanges, iBeamCellular.Web, iBeamCellular.WebOpening);
      }

      // IIBeamSymmetricalProfile
      else if (typ == "IIBeamSymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamSymmetricalProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IIBeamSymmetricalProfile.Create(iBeamSymmetrical.Depth, iBeamSymmetrical.Flanges, iBeamSymmetrical.Web);
      }

      // IRectangleHollowProfile
      else if (typ == "IRectangleHollowProfile") //(typ.Name.Equals(typeof(IRectangleHollowProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IRectangleHollowProfile.Create(rectangleHollow.Depth, rectangleHollow.Flanges, rectangleHollow.Webs);
      }

      // IRectangleProfile
      else if (typ == "IRectangleProfile") //(typ.Name.Equals(typeof(IRectangleProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = IRectangleProfile.Create(rectangle.Depth, rectangle.Width);
      }

      // IRectoEllipseProfile
      else if (typ == "IRectoEllipseProfile") //(typ.Name.Equals(typeof(IRectoEllipseProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IRectoEllipseProfile.Create(rectoEllipse.Depth, rectoEllipse.DepthFlat, rectoEllipse.Width, rectoEllipse.WidthFlat);
      }

      // ISecantPileProfile
      else if (typ == "ISecantPileProfile") //(typ.Name.Equals(typeof(ISecantPileProfile).Name))
      {
        SetNumberOfGenericInputs(4, true);
        //dup = ISecantPileProfile.Create(secantPile.Diameter, secantPile.PileCentres, secantPile.PileCount, secantPile.IsWallNotSection);
      }

      // ISheetPileProfile
      else if (typ == "ISheetPileProfile") //(typ.Name.Equals(typeof(ISheetPileProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = ISheetPileProfile.Create(sheetPile.Depth, sheetPile.Width, sheetPile.TopFlangeWidth, sheetPile.BottomFlangeWidth, sheetPile.FlangeThickness, sheetPile.WebThickness);
      }

      // IStadiumProfile
      else if (typ == "IStadiumProfile") //(typ.Name.Equals(typeof(IStadiumProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = IStadiumProfile.Create(stadium.Depth, stadium.Width);
      }

      // ITrapezoidProfile
      else if (typ == "ITrapezoidProfile") //(typ.Name.Equals(typeof(ITrapezoidProfile).Name))
      {
        SetNumberOfGenericInputs(3);
        //dup = ITrapezoidProfile.Create(trapezoid.Depth, trapezoid.TopWidth, trapezoid.BottomWidth);
      }

      // ITSectionProfile
      else if (typ == "ITSectionProfile") //(typ.Name.Equals(typeof(ITSectionProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = ITSectionProfile.Create(tSection.Depth, tSection.Flange, tSection.Web);
      }
      // IPerimeterProfile
      else if (typ == "IPerimeterProfile") //(typ.Name.Equals(typeof(IPerimeterProfile).Name))
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

    #endregion
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      writer.SetString("mode", _mode.ToString());
      writer.SetString("lengthUnit", lengthUnit.ToString());
      writer.SetBoolean("inclSS", inclSS);
      writer.SetInt32("NumberOfInputs", numberOfInputs);
      writer.SetInt32("catalogueIndex", catalogueIndex);
      writer.SetInt32("typeIndex", typeIndex);
      writer.SetString("search", search);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      first = false;

      DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);

      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), reader.GetString("mode"));
      lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), reader.GetString("lengthUnit"));

      inclSS = reader.GetBoolean("inclSS");
      numberOfInputs = reader.GetInt32("NumberOfInputs");

      catalogueIndex = reader.GetInt32("catalogueIndex");
      typeIndex = reader.GetInt32("typeIndex");
      search = reader.GetString("search");

      UpdateUIFromSelectedItems();

      return base.Read(reader);
    }
    bool comingFromSave = false;
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
    #endregion
    #region IGH_VariableParameterComponent null implementation
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      if (_mode == FoldMode.Catalogue)
      {
        int i = 0;
        Params.Input[i].NickName = "S";
        Params.Input[i].Name = "Search";
        Params.Input[i].Description = "Text to search from";
        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = true;

        i++;
        Params.Input[i].NickName = "iSS";
        Params.Input[i].Name = "InclSuperseeded";
        Params.Input[i].Description = "Input true to include superseeded catalogue sections";
        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = true;
      }
      else
      {
        IQuantity quantity = new Length(0, lengthUnit);
        unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

        int i = 0;
        // angle
        if (typ == "IAngleProfile") //(typ.Name.Equals(typeof(IAngleProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the angle profile (leg in the local z axis).";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "W";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the angle profile (leg in the local y axis).";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          //dup = IAngleProfile.Create(angle.Depth, angle.Flange, angle.Web);
        }

        // channel
        else if (typ == "IChannelProfile") //(typ.Name.Equals(typeof(IChannelProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the channel profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the flange of the channel profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the channel profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the channel profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IChannelProfile.Create(channel.Depth, channel.Flanges, channel.Web);
        }

        // circle hollow
        else if (typ == "ICircleHollowProfile") //(typ.Name.Equals(typeof(ICircleHollowProfile).Name))
        {
          Params.Input[i].NickName = "Ø";
          Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The diameter of the hollow circle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "t";
          Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The wall thickness of the hollow circle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = ICircleHollowProfile.Create(circleHollow.Diameter, circleHollow.WallThickness);
        }

        // circle
        else if (typ == "ICircleProfile") //(typ.Name.Equals(typeof(ICircleProfile).Name))
        {
          Params.Input[i].NickName = "Ø";
          Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The diameter of the circle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          //dup = ICircleProfile.Create(circle.Diameter);
        }

        // ICruciformSymmetricalProfile
        else if (typ == "ICruciformSymmetricalProfile") //(typ.Name.Equals(typeof(ICruciformSymmetricalProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth (local z axis leg) of the profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the flange (local y axis leg) of the cruciform.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the cruciform.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the cruciform.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = ICruciformSymmetricalProfile.Create(cruciformSymmetrical.Depth, cruciformSymmetrical.Flange, cruciformSymmetrical.Web);
        }

        // IEllipseHollowProfile
        else if (typ == "IEllipseHollowProfile") //(typ.Name.Equals(typeof(IEllipseHollowProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the hollow ellipse.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the hollow ellipse.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "t";
          Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The wall thickness of the hollow ellipse.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IEllipseHollowProfile.Create(ellipseHollow.Depth, ellipseHollow.Width, ellipseHollow.WallThickness);
        }

        // IEllipseProfile
        else if (typ == "IEllipseProfile") //(typ.Name.Equals(typeof(IEllipseProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the ellipse.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the ellipse.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IEllipseProfile.Create(ellipse.Depth, ellipse.Width);
        }

        // IGeneralCProfile
        else if (typ == "IGeneralCProfile") //(typ.Name.Equals(typeof(IGeneralCProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the generic c section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange width of the generic c section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "L";
          Params.Input[i].Name = "Lip [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The lip of the generic c section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "t";
          Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The thickness of the generic c section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IGeneralCProfile.Create(generalC.Depth, generalC.FlangeWidth, generalC.Lip, generalC.Thickness);
        }

        // IGeneralZProfile
        else if (typ == "IGeneralZProfile") //(typ.Name.Equals(typeof(IGeneralZProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the generic z section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bt";
          Params.Input[i].Name = "TopWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top flange width of the generic z section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bb";
          Params.Input[i].Name = "BottomWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The bottom flange width of the generic z section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Lt";
          Params.Input[i].Name = "Top Lip [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top lip of the generic z section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Lb";
          Params.Input[i].Name = "Lip [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top lip of the generic z section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "t";
          Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The thickness of the generic z section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IGeneralZProfile.Create(generalZ.Depth, generalZ.TopFlangeWidth, generalZ.BottomFlangeWidth, generalZ.TopLip, generalZ.BottomLip, generalZ.Thickness);
        }

        // IIBeamAsymmetricalProfile
        else if (typ == "IIBeamAsymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamAsymmetricalProfile).Name))
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
          Params.Input[i].NickName = "Web";
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

        // IIBeamCellularProfile
        else if (typ == "IIBeamCellularProfile") //(typ.Name.Equals(typeof(IIBeamCellularProfile).Name))
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
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "O";
          Params.Input[i].Name = "WebOpening [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The size of the web opening.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "P";
          Params.Input[i].Name = "Pitch [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The pitch (spacing) between the web openings.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IIBeamCellularProfile.Create(iBeamCellular.Depth, iBeamCellular.Flanges, iBeamCellular.Web, iBeamCellular.WebOpening);
        }

        // IIBeamSymmetricalProfile
        else if (typ == "IIBeamSymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamSymmetricalProfile).Name))
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
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          //dup = IIBeamSymmetricalProfile.Create(iBeamSymmetrical.Depth, iBeamSymmetrical.Flanges, iBeamSymmetrical.Web);
        }

        // IRectangleHollowProfile
        else if (typ == "IRectangleHollowProfile") //(typ.Name.Equals(typeof(IRectangleHollowProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the hollow rectangle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the hollow rectangle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The side thickness of the hollow rectangle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top/bottom thickness of the hollow rectangle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IRectangleHollowProfile.Create(rectangleHollow.Depth, rectangleHollow.Flanges, rectangleHollow.Webs);
        }

        // IRectangleProfile
        else if (typ == "IRectangleProfile") //(typ.Name.Equals(typeof(IRectangleProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "Depth of the rectangle, in local z-axis direction.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "Width of the rectangle, in loca y-axis direction.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IRectangleProfile.Create(rectangle.Depth, rectangle.Width);
        }

        // IRectoEllipseProfile
        else if (typ == "IRectoEllipseProfile") //(typ.Name.Equals(typeof(IRectoEllipseProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The overall depth of the recto-ellipse profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Df";
          Params.Input[i].Name = "DepthFlat [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flat length of the profile's overall depth.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The overall width of the recto-ellipse profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bf";
          Params.Input[i].Name = "WidthFlat [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flat length of the profile's overall width.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IRectoEllipseProfile.Create(rectoEllipse.Depth, rectoEllipse.DepthFlat, rectoEllipse.Width, rectoEllipse.WidthFlat);
        }

        // ISecantPileProfile
        else if (typ == "ISecantPileProfile") //(typ.Name.Equals(typeof(ISecantPileProfile).Name))
        {
          Params.Input[i].NickName = "Ø";
          Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The diameter of the piles.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "c/c";
          Params.Input[i].Name = "PileCentres [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The centre to centre distance between adjacent piles.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "No";
          Params.Input[i].Name = "PileCount";
          Params.Input[i].Description = "The number of piles in the profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "W/S";
          Params.Input[i].Name = "isWall";
          Params.Input[i].Description = "Converts the profile into a wall secant pile profile if true -- Converts the profile into a section secant pile profile if false.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = ISecantPileProfile.Create(secantPile.Diameter, secantPile.PileCentres, secantPile.PileCount, secantPile.IsWallNotSection);
        }

        // ISheetPileProfile
        else if (typ == "ISheetPileProfile") //(typ.Name.Equals(typeof(ISheetPileProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the sheet pile section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The overall width of the sheet pile section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bt";
          Params.Input[i].Name = "TopFlangeWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top flange width of the sheet pile section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bb";
          Params.Input[i].Name = "BottomFlangeWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The bottom flange width of the sheet pile section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Ft";
          Params.Input[i].Name = "FlangeThickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the sheet pile section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Wt";
          Params.Input[i].Name = "WebThickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the sheet pile section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = ISheetPileProfile.Create(sheetPile.Depth, sheetPile.Width, sheetPile.TopFlangeWidth, sheetPile.BottomFlangeWidth, sheetPile.FlangeThickness, sheetPile.WebThickness);
        }

        // IStadiumProfile
        else if (typ == "IStadiumProfile") //(typ.Name.Equals(typeof(IStadiumProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The profile's overall depth considering the side length of the rectangle and the radii of the semicircles on the two ends.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The profile's width (diameter of the semicircles).";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IStadiumProfile.Create(stadium.Depth, stadium.Width);
        }

        // ITrapezoidProfile
        else if (typ == "ITrapezoidProfile") //(typ.Name.Equals(typeof(ITrapezoidProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth in z-axis direction of trapezoidal profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bt";
          Params.Input[i].Name = "TopWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top width of trapezoidal profile. Top is relative to the local z-axis.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bb";
          Params.Input[i].Name = "BottomWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The bottom width of trapezoidal profile. Bottom is relative to the local z-axis.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = ITrapezoidProfile.Create(trapezoid.Depth, trapezoid.TopWidth, trapezoid.BottomWidth);
        }

        // ITSectionProfile
        else if (typ == "ITSectionProfile") //(typ.Name.Equals(typeof(ITSectionProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the T section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the T section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the T section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the T section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          //dup = ITSectionProfile.Create(tSection.Depth, tSection.Flange, tSection.Web);
        }
        // IPerimeterProfile
        else if (typ == "IPerimeterProfile") //(typ.Name.Equals(typeof(IPerimeterProfile).Name))
        {
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Boundary";
          Params.Input[i].Description = "Planar Brep or closed planar curve.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          //i++;
          //Params.Input[i].NickName = "V";
          //Params.Input[i].Name = "[Optional] VoidPolylines";
          //Params.Input[i].Description = "The void polygons within the solid polygon of the perimeter profile. If first input is a BRep this input will be ignored.";
          //Params.Input[i].Access = GH_ParamAccess.list;
          //Params.Input[i].Optional = true;
        }
      }
    }
    #endregion
  }
}