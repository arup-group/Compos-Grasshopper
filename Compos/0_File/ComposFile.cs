using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ComposAPI.Helpers;
using Compos_8_6;
using Oasys.Units;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class ComposFile : IComposFile
  {
    internal static IAutomation ComposCOM { get; } = new Automation();
    internal static string CurrentGuid { get; set; } = "";

    // verbose
    internal static int counter;

    public string Guid { get; set; } = System.Guid.NewGuid().ToString();
    internal IList<IMember> Members = new List<IMember>();
    internal bool IsAnalysed { get; set; } = false;
    internal bool IsDesigned { get; set; } = false;

    public string JobTitle { get; set; }
    public string JobSubTitle { get; set; }
    public string CalculationHeader { get; set; }
    public string JobNumber { get; set; }
    public string Initials { get; set; }

    #region constructors
    public ComposFile()
    {
    }

    public ComposFile(List<IMember> members)
    {
      foreach (IMember member in members)
      {
        this.AddMember(member);
      }
    }
    #endregion

    #region methods
    public void AddMember(IMember member)
    {
      member.Register(this);
      this.Members.Add(member);
    }

    /// <summary>
    /// Analyse all members. 
    /// </summary>
    /// <returns>Returns a status, as follows:
    /// 0 – OK
    /// 1 – One or more members failed
    /// </returns>
    internal short Analyse()
    {
      counter++;

      short status = 0;
      foreach (Member member in this.Members)
      {
        if (this.Analyse(member.Name) == 1)
          status = 1;
      }
      this.IsAnalysed = true;
      return status;
    }

    /// <summary>
    /// Analyse the member with the given name. 
    /// </summary>
    /// <param name="memberName">the name of the member to be analysed</param>
    /// <returns>Returns a status, as follows:
    /// 0 – OK
    /// 1 – failed
    /// </returns>
    public short Analyse(string memberName)
    {
      return ComposFile.ComposCOM.Analyse(memberName);
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
    /// 4 - there is no results for the given named member
    /// </returns>
    public short CodeSatisfied(string memberName)
    {
      this.Initialise();
      return ComposFile.ComposCOM.CodeSatisfied(memberName);
    }

    /// <summary>
    /// Design all members.
    /// </summary>
    /// <returns> Returns a status, as follows:
    /// 0 – OK
    /// 1 – One or more members failed
    /// </returns>
    internal short Design()
    {
      short status = 0;
      foreach (Member member in this.Members)
      {
        if (this.Design(member.Name) == 1)
          status = 1;
      }
      this.IsDesigned = true;
      return status;
    }

    /// <summary>
    /// Design the member with the given name.
    /// </summary>
    /// <param name="memberName">the name of the member to be designed</param>
    /// <returns> Returns a status, as follows:
    /// 0 – OK
    /// 1 – failed
    /// </returns>
    public short Design(string memberName)
    {
      return ComposFile.ComposCOM.Design(memberName);
    }

    public IMember GetMember(string name)
    {
      return this.Members.First(x => x.Name == name);
    }

    public static ComposFile Open(string fileName)
    {
      ComposFile.ComposCOM.Close();
      ComposFile.ComposCOM.Open(fileName);

      // save COM object to a temp coa file
      string tempCoa = Path.GetTempPath() + System.Guid.NewGuid().ToString() + ".coa";
      int status = ComposFile.ComposCOM.SaveAs(tempCoa);

      if (status == 1)
        return null;

      // open temp coa file as ASCII string
      string coaString = File.ReadAllText(tempCoa, Encoding.UTF7);
      ComposFile file = ComposFile.FromCoaString(coaString);

      return file;
    }

    public string MemberName(int index)
    {
      this.Initialise();
      return ComposFile.ComposCOM.MemberName(index);
    }

    /// <summary>
    /// Return the number of intermediate positions where analysis results are available. 
    /// </summary>
    /// <param name="memberName"></param>
    /// <returns>If the function is not success, the return values are
    /// -1 - member doesn't exist
    /// 0 - there is no results for the given named member
    /// </returns>
    public short NumIntermediatePos(string memberName)
    {
      this.Initialise();
      return ComposFile.ComposCOM.NumIntermediatePos(memberName);
    }

    /// <summary>
    /// Return the results for the given member, option and position
    /// </summary>
    /// <param name="memberName"></param>
    /// <param name="option"></param>
    /// <param name="position">position number</param>
    /// <returns></returns>
    public float Result(string memberName, string option, short position)
    {
      this.Initialise();
      return ComposFile.ComposCOM.Result(memberName, option, position);
    }

    /// <summary>
    /// Return the maximum result and the position for the given member
    /// </summary>
    /// <param name="memberName"></param>
    /// <param name="option"></param>
    /// <param name="position">position number</param>
    /// <returns></returns>
    public float MaxResult(string memberName, string option, short position)
    {
      this.Initialise();
      return ComposFile.ComposCOM.MaxResult(memberName, option.ToString(), out position);
    }

    /// <summary>
    /// Return the position of the maximum result for the given member
    /// </summary>
    /// <param name="memberName"></param>
    /// <param name="option"></param>
    /// <param name="position">position number</param>
    /// <returns></returns>
    public short MaxResultPosition(string memberName, string option, short position)
    {
      this.Initialise();
      ComposFile.ComposCOM.MaxResult(memberName, option.ToString(), out position);
      return position;
    }

    /// <summary>
    /// Return the minimum result and the position for the given member
    /// </summary>
    /// <param name="memberName"></param>
    /// <param name="option"></param>
    /// <param name="position">position number</param>
    /// <returns></returns>
    public float MinResult(string memberName, string option, short position)
    {
      this.Initialise();
      return ComposFile.ComposCOM.MinResult(memberName, option.ToString(), out position);
    }

    /// <summary>
    /// Return the position of the minimum result for the given member
    /// </summary>
    /// <param name="memberName"></param>
    /// <param name="option"></param>
    /// <param name="position">position number</param>
    /// <returns></returns>
    public short MinResultPosition(string memberName, string option, short position)
    {
      this.Initialise();
      ComposFile.ComposCOM.MinResult(memberName, option.ToString(), out position);
      return position;
    }

    /// <summary>
    /// Return the number of transverse Rebars available. 
    /// </summary>
    /// <param name="membername"></param>
    /// <returns>If the function is not success, the return values are
    /// -1 - member doesn't exist
    /// 0 - there is no results for the given named member
    /// </returns>
    public short NumTranRebar(string memberName)
    {
      this.Initialise();
      return ComposFile.ComposCOM.NumTranRebar(memberName);
    }

    /// <summary>
    /// Return the properties of the rebar for the given member, rebar number and option
    /// </summary>
    /// <param name="memberName"></param>
    /// <param name="option"></param>
    /// <param name="rebarnum">rebar number</param>
    /// <returns></returns>
    public float TranRebarProp(string memberName, TransverseRebarOption option, short rebarnum)
    {
      this.Initialise();
      return ComposFile.ComposCOM.TranRebarProp(memberName, option.ToString(), rebarnum);
    }

    /// <summary>
    /// Save the data to COB, COAor CSV file. 
    /// </summary>
    /// <param name="fileName">the name of the file to be saved, including path and extension.</param>
    /// <returns>Returns a status, as follows:
    /// 0 – OK
    /// 1 – no Compos file is open
    /// 2 – invalid file extension
    /// 3 – failed to save
    /// </returns>
    public int SaveAs(string fileName)
    {
      Initialise();

      // save to .cob with COM object
      if (!fileName.EndsWith(".cob"))
        fileName = fileName + ".cob";

      int status = ComposFile.ComposCOM.SaveAs(fileName);

      return status;
    }

    private short Initialise()
    {
      if (this.Guid == ComposFile.CurrentGuid)
        return -1;

      ComposFile.ComposCOM.Close();
      ComposFile.CurrentGuid = this.Guid;

      // create coastring from members
      string coaString = ToCoaString();

      // save coa string to a temp to coa file (ASCII format)
      string tempCoa = Path.GetTempPath() + this.Guid + ".coa";
      File.WriteAllLines(tempCoa, new string[] { coaString }, Encoding.UTF8);

      short status = ComposFile.ComposCOM.Open(tempCoa);
      this.Analyse();

      return status;
    }

    public override string ToString()
    {
      string str = "";
      foreach (IMember member in Members)
        str += member.ToString();
      return str;
    }

    /// <summary>
    /// Return the utilisation factor (natural frequency) for the given member and the option
    /// </summary>
    /// <param name="memberName"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    public float UtilisationFactor(string memberName, UtilisationFactorOption option)
    {
      this.Initialise();
      return ComposFile.ComposCOM.UtilisationFactor(memberName, option.ToString());
    }
    #endregion

    #region coa interop
    internal static ComposFile FromCoaString(string coaString)
    {
      List<IMember> members = new List<IMember>();

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
          return new ComposFile(members);

        // ### member ###
        if (coaIdentifier == CoaIdentifier.MemberName)
        {
          IMember member = Member.FromCoaString(parameters);
          members.Add(member);
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
      foreach (Member member in members)
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
      return new ComposFile(members);
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

      foreach (IMember member in this.Members)
        coaString += member.ToCoaString(units);

      coaString += "END\n";

      return coaString;
    }
    #endregion
  }
}
