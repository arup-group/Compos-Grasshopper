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
        switch (parameters[0])
        {
          case (CoaIdentifier.UnitData):
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
            break;

          case (CoaIdentifier.MemberName):
            // create new member and add it to dictionary under it's title
            Member newMember = new Member();
            newMember.Name = parameters[1];
            newMember.GridReference = (parameters.Count > 1) ? parameters[2] : "";
            newMember.Note = (parameters.Count > 2) ? parameters[3] : "";
            members.Add(newMember.Name, newMember);
            break;

          case (CoaIdentifier.Load):
            Load load = new Load().FromCoaString(parameters, forceUnit, lengtGeometryUnit);
            Member addLoadMember = members[parameters[1]];
            if (addLoadMember.Loads == null)
              addLoadMember.Loads = new List<ILoad>();
            addLoadMember.Loads.Add(load);
            break;
          
          
          
          
          default:
            break;
            //throw new Exception("Unable to convert " + line + " to Compos Slab.");
        }
      }

      this.Members = members.Values.Select(x => (IMember)x).ToList();
    }

    public string ToCoaString()
    {
      string coaString = "";
      foreach (IMember member in Members)
        coaString += member.ToCoaString(AngleUnit.Radian, Units.DensityUnit, Units.ForceUnit, Units.LengthUnitGeometry, Units.StressUnit, Units.StrainUnit);
      
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
      foreach(IMember member in Members)  
        str += member.ToString();
      return str;
    }
    #endregion
  }
}
