using Compos_8_6;
using ComposAPI.Helpers;
using Oasys.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public enum UtilisationFactorOption
  {
    FinalMoment,
    FinalShear,
    ConstructionMoment,
    ConstructionShear,
    ConstructionBuckling,
    ConstructionDeflection,
    FinalDeflection,
    TransverseShear,
    WebOpening,
    NaturalFrequency
  }

  public class ComposFile : IComposFile
  {
    public IList<IMember> Members { get; set; } = new List<IMember>();
    internal IAutomation ComposCOM { get; set; }
    public string FileName { get; internal set; }
    public string JobTitle { get; set; }
    public string JobSubTitle { get; set; }
    public string CalculationHeader { get; set; }
    public string JobNumber { get; set; }
    public string Initials { get; set; }

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

    #region methods
    /// <summary>
    /// Analyse the member with the given name. Returns a status, as follows:
    /// 0 – OK
    /// 1 – failed
    /// </summary>
    /// <param name="memberName">the name of the member to be analysed</param>
    /// <returns></returns>
    public short Analyze(string memberName)
    {
      return this.ComposCOM.Analyse(memberName);
    }

    /// <summary>
    /// Returns an integer flag to indicates whether the code requirements are satisfied.  
    /// </summary>
    /// <param name="memberName"></param>
    /// <returns>The return values are:
    /// 0 - all code requirements are met
    /// 1 - except the natural frequency is lower than that required, other code requirements are met
    /// 2 - one or more code requirements are not met
    /// 3 - the given member name is not valid
    /// 4 - there is no results for the given named member</returns>
    public short CodeSatisfied(string memberName)
    {
      return this.ComposCOM.CodeSatisfied(memberName);
    }

    /// <summary>
    /// Design the member with the given name. Returns a status, as follows:
    /// 0 – OK
    /// 1 – failed
    /// </summary>
    /// <param name="memberName">the name of the member to be designed</param>
    /// <returns></returns>
    public short Design(string memberName)
    {
      return this.ComposCOM.Design(memberName);
    }

    public IMember GetMember(string name)
    {
      return this.Members.First(x => x.Name == name);
    }

    public static ComposFile Open(string fileName)
    {
      IAutomation automation = new Automation();
      automation.Open(fileName);

      // save COM object to a temp coa file
      string tempCoa = Path.GetTempPath() + Guid.NewGuid().ToString() + ".coa";
      int status = automation.SaveAs(tempCoa);

      if (status == 1)
        return null;

      // open temp coa file as ASCII string
      string coaString = File.ReadAllText(tempCoa, Encoding.UTF7);
      ComposFile file = ComposFile.FromCoaString(coaString);
      file.ComposCOM = automation;
      file.FileName = fileName;

      return file;
    }

    public int SaveAs(string fileName)
    {
      // create coastring from members
      string coaString = ToCoaString();

      // save coa string to a temp to coa file (ASCII format)
      string tempCoa = Path.GetTempPath() + Guid.NewGuid().ToString() + ".coa";
      File.WriteAllLines(tempCoa, new string[] { coaString }, Encoding.UTF8);

      IAutomation automation = new Automation();
      automation.Open(tempCoa);

      // save to .cob with COM object
      if (!fileName.EndsWith(".cob"))
        fileName = fileName + ".cob";

      this.FileName = fileName;

      int status = automation.SaveAs(fileName);

      return status;
    }

    public override string ToString()
    {
      string str = "";
      foreach (IMember member in Members)
        str += member.ToString();
      return str;
    }

    public float UtilisationFactor(string memberName, UtilisationFactorOption option)
    {
      return this.ComposCOM.UtilisationFactor(memberName, option.ToString());
    }
    #endregion

    #region coa interop
    internal static ComposFile FromCoaString(string coaString)
    {
      ComposFile file = new ComposFile();
      file.Members = new List<IMember>();

      Dictionary<string, Stud> studs = new Dictionary<string, Stud>();
      Dictionary<string, StudDimensions> studDimensions = new Dictionary<string, StudDimensions>();
      Dictionary<string, StudDimensions> studECDimensions = new Dictionary<string, StudDimensions>();
      Dictionary<string, bool> studWelded = new Dictionary<string, bool>();
      Dictionary<string, StudSpecification> studSpecifications = new Dictionary<string, StudSpecification>();
      Dictionary<string, List<IStudGroupSpacing>> studGroupSpacings = new Dictionary<string, List<IStudGroupSpacing>>();
      Dictionary<string, List<ILoad>> loads = new Dictionary<string, List<ILoad>>();

      ComposUnits units = ComposUnits.GetStandardUnits();
      List<string> lines = CoaHelper.SplitAndStripLines(coaString);

      // ### collect data from each line ###
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);
        string coaIdentifier = parameters[0];

        if (coaIdentifier == "END")
          return file;

        // ### member ###
        if (coaIdentifier == CoaIdentifier.MemberName)
        {
          IMember member = Member.FromCoaString(parameters);
          file.Members.Add(member);
        }

        // ### change unit ###
        else if (coaIdentifier == CoaIdentifier.UnitData)
        {
          // change the currently used unit
          units.FromCoaString(parameters);
        }



        // ### stud related lines ###
        else if (coaIdentifier == CoaIdentifier.StudDimensions.StudDefinition)
        {
          //Code code = codes[parameters[1]].Code;
          //StudDimensions dimensions = StudDimensions.FromCoaString(parameters, units, code);
          //studDimensions.Add(parameters[1], dimensions);

          //bool isWelded = parameters.Last() == "WELDED_YES";
          //studWelded.Add(parameters[1], isWelded);
        }
        else if (coaIdentifier == CoaIdentifier.StudGroupSpacings.StudLayout)
        {
          if (parameters[2] != CoaIdentifier.StudGroupSpacings.StudLayoutCustom)
          {
            Stud stud = Stud.FromCoaString(parameters);
            studs.Add(parameters[1], stud);
          }

          if (parameters[2] == CoaIdentifier.StudGroupSpacings.StudLayoutCustom)
          {
            if (parameters[4] == 1.ToString())
            {
              Stud stud = Stud.FromCoaString(parameters);
              studs.Add(parameters[1], stud);
            }

            StudGroupSpacing custom = new StudGroupSpacing().FromCoaString(parameters, units);
            List<IStudGroupSpacing> spacings = new List<IStudGroupSpacing>();
            if (!studGroupSpacings.ContainsKey(parameters[1]))
              studGroupSpacings.Add(parameters[1], spacings);
            studGroupSpacings[parameters[1]].Add(custom);
          }
        }

        else if (coaIdentifier == CoaIdentifier.StudSpecifications.StudNoZone |
          coaIdentifier == CoaIdentifier.StudSpecifications.StudEC4 |
          coaIdentifier == CoaIdentifier.StudSpecifications.StudNCCI |
          coaIdentifier == CoaIdentifier.StudSpecifications.StudReinfPos)
        {
          StudSpecification spec = new StudSpecification();
          if (studSpecifications.ContainsKey(parameters[1]))
            spec = studSpecifications[parameters[1]];
          spec.FromCoaString(parameters, units);
          if (!studSpecifications.ContainsKey(parameters[1]))
            studSpecifications.Add(parameters[1], spec);
        }
        else if (coaIdentifier == CoaIdentifier.StudDimensions.StudGradeEC4)
        {
          StudDimensions ecDims = StudDimensions.FromCoaString(parameters, units, Code.EN1994_1_1_2004);
          studECDimensions.Add(parameters[1], ecDims);
        }

        // ### loads ###
        else if (coaIdentifier == CoaIdentifier.Load)
        {
          Load load = Load.FromCoaString(parameters, units);
          List<ILoad> memLoads = new List<ILoad>();
          if (!loads.ContainsKey(parameters[1]))
            loads.Add(parameters[1], memLoads);
          loads[parameters[1]].Add(load);
        }
      }


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



      // ### Set data to members ###
      foreach (Member member in file.Members)
      {
        string name = member.Name;

        member.DesignCode = DesignCode.FromCoaString(coaString, name, units);
        Code code = member.DesignCode.Code;

        member.Beam = Beam.FromCoaString(coaString, name, units);
        member.Stud = studs[name];
        member.Slab = Slab.FromCoaString(coaString, name, code, units);

        // add loads to members
        member.Loads = loads[name];
      }
      return file;
    }

    public string ToCoaString()
    {
      ComposUnits units = new ComposUnits
      {
        Angle = AngleUnit.Degree,
        Density = Units.DensityUnit,
        Force = Units.ForceUnit,
        Length = Units.LengthUnitGeometry,
        Displacement = Units.LengthUnitResult,
        Section = Units.LengthUnitSection,
        Stress = Units.StressUnit,
        Strain = Units.StrainUnit,
        Mass = Units.MassUnit,
      };

      string version = "0.1"; // ??

      string coaString = "! This file was originally written by ComposGH version " + version + " on " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:MM:ss") + "\n";
      coaString += "!\n";
      coaString += "! Notes:\n";
      coaString += "! 1 All data in this file will be interpreted as being in user units\n";
      coaString += "!   as defined by UNIT_DATA records. These units remain in force until\n";
      coaString += "!   another UNIT_DATA record is found.\n";
      coaString += "!   Length units can be long (LENGTH), short (DISP) or Section (PROP_LENGTH).\n";
      coaString += "!   In general long length units are used except that.\n";
      coaString += "!   Section length unit is used for section sizes and\n";
      coaString += "!   Displacement length unit is used for displacement results.\n";
      coaString += "!   COMPOS stores all data in SI units.\n";
      coaString += "! 2 The date specified in the description for catalogue sections is\n";
      coaString += "!   optional. If omitted then the most recent section in the catalogue\n";
      coaString += "!   bearing that name will be assumed.\n";
      coaString += "!\n";
      coaString += "COMPOS_FILE_VERSION\t1\n";
      coaString += "TITLE\t" + this.JobTitle + "\t" + this.JobSubTitle + "\t" + this.CalculationHeader + "\t" + this.JobNumber + "\t" + this.Initials + "\n";

      coaString += units.ToCoaString();

      foreach (IMember member in Members)
        coaString += member.ToCoaString(units);

      coaString += "END\n";

      return coaString;
    }
    #endregion
  }
}
