using Compos_8_6;
using ComposAPI.Helpers;
using Oasys.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class ComposFile : IComposFile
  {
    public List<IMember> Members { get; set; }

    #region constructors
    public ComposFile()
    {
      // empty constructor
    }

    public ComposFile(List<IMember> members)
    {
      this.Members = members;
    }
    #endregion

    #region coa interop
    internal ComposFile(string coaString)
    {
      // to do - implement from coa string method

      Dictionary<string, Member> members = new Dictionary<string, Member>();
      Dictionary<string, DesignCode> codes = new Dictionary<string, DesignCode>();

      Dictionary<string, List<IBeamSection>> beamSections = new Dictionary<string, List<IBeamSection>>();

      Dictionary<string, Stud> studs = new Dictionary<string, Stud>();
      Dictionary<string, StudDimensions> studDimensions = new Dictionary<string, StudDimensions>();
      Dictionary<string, StudDimensions> studECDimensions = new Dictionary<string, StudDimensions>();
      Dictionary<string, bool> studWelded = new Dictionary<string, bool>();
      Dictionary<string, StudSpecification> studSpecifications = new Dictionary<string, StudSpecification>();
      Dictionary<string, List<IStudGroupSpacing>> studGroupSpacings = new Dictionary<string, List<IStudGroupSpacing>>();
      
      Dictionary<string, List<ILoad>> loads = new Dictionary<string, List<ILoad>>();

      ComposUnits units = ComposUnits.GetStandardUnits();

      List<string> lines = CoaHelper.SplitLines(coaString);

      // ### collect data from each line ###
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);

        // ### member ###
        if (parameters[0] == CoaIdentifier.MemberName)
        {
          // create new member and add it to dictionary under it's title
          Member newMember = new Member();
          newMember.Name = parameters[1];
          newMember.GridReference = (parameters.Count > 1) ? parameters[2] : "";
          newMember.Note = (parameters.Count > 2) ? parameters[3] : "";
          members.Add(newMember.Name, newMember);
        }

        // ### change unit ###
        if (parameters[0] == CoaIdentifier.UnitData)
        {
          // change the currently used unit
          switch (parameters[1])
          {
            case CoaIdentifier.Units.Force:
              units.Force = (ForceUnit)UnitParser.Default.Parse(parameters[2], typeof(ForceUnit));
              break;
            case CoaIdentifier.Units.Length_Geometry:
              units.Length = (LengthUnit)UnitParser.Default.Parse(parameters[2], typeof(LengthUnit));
              break;
            case CoaIdentifier.Units.Length_Section:
              units.Section = (LengthUnit)UnitParser.Default.Parse(parameters[2], typeof(LengthUnit));
              break;
            //case CoaIdentifier.Units.Length_Results:
            //  lengtResultUnit = (LengthUnit)UnitParser.Default.Parse(parameters[2], typeof(LengthUnit));
              //break;
            case CoaIdentifier.Units.Stress:
              units.Stress = (PressureUnit)UnitParser.Default.Parse(parameters[2], typeof(PressureUnit));
              break;
            case CoaIdentifier.Units.Mass:
              units.Mass = (MassUnit)UnitParser.Default.Parse(parameters[2], typeof(MassUnit));
              break;
          }
        }

        // ### Design Code ###
        if (parameters[0] == CoaIdentifier.DesignCode)
        {
          DesignCode dc = new DesignCode().FromCoaString(parameters);
          codes.Add(parameters[1], dc);
        }

        // ### beam sections ###
        if (parameters[0] == CoaIdentifier.BeamSectionAtX)
        {
          //BeamSection beamSection = new BeamSection(parameters, lengtGeometryUnit);
          //List<IStudGroupSpacing> spacings = new List<IStudGroupSpacing>();
          //if (!studGroupSpacings.ContainsKey(parameters[1]))
          //  studGroupSpacings.Add(parameters[1], spacings);
          //studGroupSpacings[parameters[1]].Add(custom);
        }

        // ### stud related lines ###
        if (parameters[0] == CoaIdentifier.StudDimensions.StudDefinition)
        {
          Code code = codes[parameters[1]].Code;
          StudDimensions dimensions = new StudDimensions().FromCoaString(parameters, units, code);
          studDimensions.Add(parameters[1], dimensions);

          bool isWelded = parameters.Last() == "WELDED_YES";
          studWelded.Add(parameters[1], isWelded);
        }
        if (parameters[0] == CoaIdentifier.StudGroupSpacings.StudLayout)
        {
          if (parameters[2] != CoaIdentifier.StudGroupSpacings.StudLayoutCustom)
          {
            Stud stud = new Stud().FromCoaString(parameters);
            studs.Add(parameters[1], stud);
          }

          if (parameters[2] == CoaIdentifier.StudGroupSpacings.StudLayoutCustom)
          {
            if (parameters[4] == 1.ToString())
            {
              Stud stud = new Stud().FromCoaString(parameters);
              studs.Add(parameters[1], stud);
            }

            StudGroupSpacing custom = new StudGroupSpacing().FromCoaString(parameters, units);
            List<IStudGroupSpacing> spacings = new List<IStudGroupSpacing>();
            if (!studGroupSpacings.ContainsKey(parameters[1]))
              studGroupSpacings.Add(parameters[1], spacings);
            studGroupSpacings[parameters[1]].Add(custom);
          }
        }

        if (parameters[0] == CoaIdentifier.StudSpecifications.StudNoZone |
          parameters[0] == CoaIdentifier.StudSpecifications.StudEC4 |
          parameters[0] == CoaIdentifier.StudSpecifications.StudNCCI |
          parameters[0] == CoaIdentifier.StudSpecifications.StudReinfPos)
        {
          StudSpecification spec = new StudSpecification();
          if (studSpecifications.ContainsKey(parameters[1]))
            spec = studSpecifications[parameters[1]];
          spec.FromCoaString(parameters, units);
          if (!studSpecifications.ContainsKey(parameters[1]))
            studSpecifications.Add(parameters[1], spec);
        }
        if (parameters[0] == CoaIdentifier.StudDimensions.StudGradeEC4)
        {
          StudDimensions ecDims = new StudDimensions().FromCoaString(parameters, units, Code.EN1994_1_1_2004);
          studECDimensions.Add(parameters[1], ecDims);
        }

        // ### loads ###
        if (parameters[0] == CoaIdentifier.Load)
        {
          Load load = new Load().FromCoaString(parameters, units);
          List<ILoad> memLoads = new List<ILoad>();
          if (!loads.ContainsKey(parameters[1]))
            loads.Add(parameters[1], memLoads);
          loads[parameters[1]].Add(load);
        }
      }

      // ### Set data to members ###

      // add loads to members
      foreach (string name in loads.Keys)
        members[name].Loads = loads[name];

      // add designcode to members
      foreach (string name in codes.Keys)
        members[name].DesignCode = codes[name];

      // add special ec4 stud grade to stud dimensions
      foreach (string name in studECDimensions.Keys)
        studDimensions[name].Fu = studECDimensions[name].Fu;
      // add stud dimensions to studs
      foreach (string name in studDimensions.Keys)
        studs[name].StudDimensions = studDimensions[name];
      // add custom group spacings to studs
      foreach (string name in studGroupSpacings.Keys)
        studs[name].CustomSpacing = studGroupSpacings[name];
      // add if Welded to stud specifications
      foreach (string name in studWelded.Keys)
        studSpecifications[name].Welding = studWelded[name];
      // add stud specifications to studs
      foreach (string name in studSpecifications.Keys)
        studs[name].StudSpecification = studSpecifications[name];
      // add studs to members
      foreach (string name in studs.Keys)
        members[name].Stud = studs[name];

      this.Members = members.Values.Select(x => (IMember)x).ToList();
    }



    public string ToCoaString()
    {
      ComposUnits units = new ComposUnits
      {
        Angle = AngleUnit.Degree,
        Density = Units.DensityUnit,
        Force = Units.ForceUnit,
        Length = Units.LengthUnitGeometry,
        Section = Units.LengthUnitSection,
        Stress = Units.StressUnit,
        Strain = Units.StrainUnit
      };

      string coaString = "";
      foreach (IMember member in Members)
        coaString += member.ToCoaString(units);

      return coaString;
    }
    #endregion

    #region methods
    public IAutomation Open(string pathName)
    {
      IAutomation automation = new Automation();
      automation.Open(pathName);

      return automation;
    }

    public IAutomation SaveAs(string pathName, string coaString)
    {
      File.WriteAllLines(pathName, new string[] { coaString });
      return Open(pathName);
    }

    public override string ToString()
    {
      string str = "";
      foreach (IMember member in Members)
        str += member.ToString();
      return str;
    }
    #endregion
  }
}
