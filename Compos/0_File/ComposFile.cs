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

      Dictionary<string, List<ILoad>> loads = new Dictionary<string, List<ILoad>>();

      Dictionary<string, Stud> studs = new Dictionary<string, Stud>();
      Dictionary<string, StudDimensions> studDimensions = new Dictionary<string, StudDimensions>();
      Dictionary<string, StudSpecification> studSpecifications = new Dictionary<string, StudSpecification>();
      Dictionary<string, List<StudGroupSpacing>> studGroupSpacings = new Dictionary<string, List<StudGroupSpacing>>();

      AngleUnit angleUnit = AngleUnit.Degree;
      MassUnit massUnit = MassUnit.Kilogram;
      LengthUnit lengtGeometryUnit = LengthUnit.Meter;
      LengthUnit lengtSectionUnit = LengthUnit.Millimeter;
      LengthUnit lengtResultUnit = LengthUnit.Millimeter;
      PressureUnit stressUnit = PressureUnit.NewtonPerSquareMeter;
      StrainUnit strainUnit = StrainUnit.Ratio;
      ForceUnit forceUnit = ForceUnit.Kilonewton;

      List<string> lines = CoaHelper.SplitLines(coaString);

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
              forceUnit = (ForceUnit)UnitParser.Default.Parse(parameters[2], typeof(ForceUnit));
              break;
            case CoaIdentifier.Units.Length_Geometry:
              lengtGeometryUnit = (LengthUnit)UnitParser.Default.Parse(parameters[2], typeof(LengthUnit));
              break;
            case CoaIdentifier.Units.Length_Section:
              lengtSectionUnit = (LengthUnit)UnitParser.Default.Parse(parameters[2], typeof(LengthUnit));
              break;
            case CoaIdentifier.Units.Length_Results:
              lengtResultUnit = (LengthUnit)UnitParser.Default.Parse(parameters[2], typeof(LengthUnit));
              break;
            case CoaIdentifier.Units.Stress:
              stressUnit = (PressureUnit)UnitParser.Default.Parse(parameters[2], typeof(PressureUnit));
              break;
            case CoaIdentifier.Units.Mass:
              massUnit = (MassUnit)UnitParser.Default.Parse(parameters[2], typeof(MassUnit));
              break;
          }
        }

        // ### Design Code ###
        if (parameters[0] == CoaIdentifier.DesignCode)
        {
          DesignCode dc = new DesignCode().FromCoaString(parameters);
          codes.Add(parameters[1], dc);
        }

        // ### create loads ###
        if (parameters[0] == CoaIdentifier.Load)
        {
          Load load = new Load().FromCoaString(parameters, forceUnit, lengtGeometryUnit);
          List<ILoad> memLoads = new List<ILoad>();
          if (loads.ContainsKey(parameters[1]))
            memLoads = loads[parameters[1]];
          memLoads.Add(load);
          if (!loads.ContainsKey(parameters[1]))
            loads.Add(parameters[1], memLoads);
        }

        // ### stud related lines ###
        if (parameters[0] == CoaIdentifier.StudDimensions.StudDefinition)
        {
          Code code = codes[parameters[1]].Code;
          StudDimensions dimensions = new StudDimensions().FromCoaString(parameters, lengtSectionUnit, forceUnit, code);
        }
        if (parameters[0] == CoaIdentifier.StudGroupSpacings.StudLayout)
        {

        }
        if (parameters[0] == CoaIdentifier.StudSpecifications.StudNoZone)
        {

        }
        if (parameters[0] == CoaIdentifier.StudSpecifications.StudEC4)
        {

        }
        if (parameters[0] == CoaIdentifier.StudSpecifications.StudNCCI)
        {

        }
        if (parameters[0] == CoaIdentifier.StudSpecifications.StudReinfPos)
        {

        }
        if (parameters[0] == CoaIdentifier.StudSpecifications.StudReinfPos)
        {

        }
        if (parameters[0] == CoaIdentifier.StudDimensions.StudGradeEC4)
        {

        }

      }



      foreach (string name in loads.Keys)
        members[name].Loads = loads[name];

      foreach (string name in codes.Keys)
        members[name].DesignCode = codes[name];

      this.Members = members.Values.Select(x => (IMember)x).ToList();
    }



    public string ToCoaString()
    {
      string coaString = "";
      foreach (IMember member in Members)
        coaString += member.ToCoaString(AngleUnit.Radian, Units.DensityUnit, Units.ForceUnit, Units.LengthUnitGeometry, Units.LengthUnitSection, Units.StressUnit, Units.StrainUnit);

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
