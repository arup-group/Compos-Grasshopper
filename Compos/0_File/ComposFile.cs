﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ComposAPI.Helpers;
using Compos_8_6;
using UnitsNet.Units;

namespace ComposAPI
{
  public class ComposFile : IComposFile, IDisposable
  {
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string CalculationHeader { get; set; }
    public string Initials { get; set; }
    public string JobNumber { get; set; }
    public string JobSubTitle { get; set; }
    public string JobTitle { get; set; }
    public ComposUnits Units { get; set; }
    private static IAutomation ComposCOM { get; set; }
    private static Guid CurrentGuid { get; set; } = Guid.Empty;
    private readonly IList<IMember> Members = new List<IMember>();
    // verbose
    private static int Counter;

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
      ((Member)member).Register(this);
      this.Members.Add(member);
    }

    public IList<IMember> GetMembers()
    {
      return this.Members;
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
      ComposFile.Counter++;

      short status = 0;
      foreach (Member member in this.Members)
      {
        if (this.Analyse(member.Name) == 1)
          status = 1;
      }
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

    public static short Close()
    {
      if (ComposFile.ComposCOM == null) { return 0; }
      short status = ComposFile.ComposCOM.Close();
      ComposFile.ComposCOM = null;
      return status;
    }

    public void Dispose()
    {
      Close();
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
      this.Initialise();
      short status = 0;
      foreach (Member member in this.Members)
      {
        if (this.Design(member.Name) == 1)
          status = 1;
      }
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
      this.Initialise();
      return ComposFile.ComposCOM.Design(memberName);
    }

    public string BeamSectDesc(string memberName)
    {
      return ComposFile.ComposCOM.BeamSectDesc(memberName);
    }

    public IMember GetMember(string name)
    {
      return this.Members.First(x => x.Name == name);
    }

    public static ComposFile Open(string fileName)
    {
      if (ComposFile.ComposCOM != null)
      {
        ComposFile.ComposCOM.Close();
        ComposFile.ComposCOM = null;
      }
      ComposFile.ComposCOM = new Automation();
      int status = ComposFile.ComposCOM.Open(fileName);
      if (status == 1)
        return null;

      // save COM object to a temp coa file
      string tempCoa = Path.GetTempPath() + System.Guid.NewGuid().ToString() + ".coa";
      status = ComposFile.ComposCOM.SaveAs(tempCoa);
      if (status == 1)
        return null;

      // open temp coa file as ASCII string
      string coaString = File.ReadAllText(tempCoa, Encoding.Default);
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
    internal float TranRebarProp(string memberName, TransverseRebarOption option, short rebarnum)
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

      // save to .coa with COM object
      if (!fileName.EndsWith(".coa"))
        fileName = fileName + ".coa";

      int status = ComposFile.ComposCOM.SaveAs(fileName);

      return status;
    }

    /// <summary>
    /// Open a COB, COA or CSV file. Returns a status, as follows:
    /// </summary>
    /// <param name="checkGUID"></param>
    /// <returns>
    /// 0 – OK
    /// 1 – failed to open
    /// </returns>
    private short Initialise(bool checkGUID = true)
    {
      if (checkGUID)
      {
        if (this.Guid == ComposFile.CurrentGuid)
          return -1;
      }

      short status;
      if (ComposFile.ComposCOM != null)
      {
        ComposFile.ComposCOM.Close();
        ComposFile.ComposCOM = null;
      }
      ComposFile.CurrentGuid = this.Guid;

      // create coastring from members
      string coaString = ToCoaString();

      // save coa string to a temp to coa file (ASCII format)
      string tempCoa = Path.GetTempPath() + this.Guid + ".coa";
      File.WriteAllLines(tempCoa, new string[] { coaString }, Encoding.Default);

      ComposFile.ComposCOM = new Automation();
      status = ComposFile.ComposCOM.Open(tempCoa);
      this.Analyse();

      return status;
    }

    public override string ToString()
    {
      string str = "";
      foreach (IMember member in Members)
        str += member.ToString() + " ";
      str.TrimEnd(' ');
      return str;
    }

    /// <summary>
    /// Return the utilisation factor (natural frequency) for the given member and the option
    /// </summary>
    /// <param name="memberName"></param>
    /// <param name="option"></param>
    /// <returns></returns>
    internal float UtilisationFactor(string memberName, UtilisationFactorOption option)
    {
      this.Initialise();
      return ComposFile.ComposCOM.UtilisationFactor(memberName, option.ToString());
    }

    /// <summary>
    /// Triggers an update of the Compos model.
    /// </summary>
    public void Update()
    {
      ComposFile.CurrentGuid = Guid.Empty;
    }
    #endregion

    #region coa interop
    internal static ComposFile FromCoaString(string coaString)
    {
      List<IMember> members = new List<IMember>();
      ComposUnits units = ComposUnits.GetStandardUnits();
      List<string> lines = CoaHelper.SplitAndStripLines(coaString);

      // ### collect data from each line ###
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);
        string coaIdentifier = parameters[0];

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
      }

      // ### Set data to members ###
      foreach (Member member in members)
      {
        string name = member.Name;
        member.DesignCode = DesignCode.FromCoaString(coaString, name, units);
        Code code = member.DesignCode.Code;
        member.DesignCriteria = DesignCriteria.FromCoaString(coaString, name, units);

        member.Beam = Beam.FromCoaString(coaString, name, units, code);
        member.Stud = Stud.FromCoaString(coaString, name, code, units);
        member.Slab = Slab.FromCoaString(coaString, name, code, units);
        member.Loads = Load.FromCoaString(coaString, name, units);
      }
      if (members.Count < 0)
        return null;

      ComposFile file = new ComposFile(members);
      file.Units = units;

      return file;
    }

    public string ToCoaString()
    {
      if (this.Units == null)
      {
        this.Units = new ComposUnits
        {
          Angle = AngleUnit.Degree,
          Density = UnitsHelper.DensityUnit,
          Force = UnitsHelper.ForceUnit,
          Length = UnitsHelper.LengthUnitGeometry,
          Displacement = UnitsHelper.LengthUnitResult,
          Section = UnitsHelper.LengthUnitSection,
          Stress = UnitsHelper.StressUnit,
          Strain = UnitsHelper.StrainUnit,
          Mass = UnitsHelper.MassUnit,
        };
      }

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

      coaString += this.Units.ToCoaString();

      foreach (IMember member in this.Members)
      {
        coaString += member.ToCoaString(this.Units);
        coaString += "FLOOR_RESPONSE\t" + member.Name + "\tFLOOR_RESPONSE_ANALYSIS_NO\n";
      }

      coaString += "GROUP\tALL\tDefault group containing all the members\t1";
      foreach (IMember member in this.Members)
        coaString += "\t" + member.Name;
      coaString += "\nEND\n";

      return coaString;
    }
    #endregion
  }
}
