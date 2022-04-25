﻿using System;
using System.Linq;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System.Data.SQLite;
using System.Data;

namespace ComposGH.Helpers
{
  /// <summary>
  /// Class containing functions to interface with SQLite db files.
  /// </summary>
  public class SqlReadValues
  {
    /// <summary>
    /// Get catalogue data from SQLite file (.db3). The method returns a tuple with:
    /// Item1 = list of catalogue name (string)
    /// where first item will be "All"
    /// Item2 = list of catalogue number (int)
    /// where first item will be "-1" representing All
    /// </summary>
    /// <param name="filePath">Path to SecLib.db3</param>
    /// <returns></returns>
    public static List<double> GetProfileValuesFromSQLite(string filePath, string profileString)
    {
      // Create empty lists to work on:
      List<double> values = new List<double>();

      using (var db = SqlReader.Connection(filePath))
      {
        db.Open();
        SQLiteCommand cmd = db.CreateCommand();
        cmd.CommandText = $"Select SECT_DEPTH_DIAM || ' -- ' || SECT_WIDTH || ' -- ' || SECT_WEB_THICK || ' -- ' || SECT_FLG_THICK || ' -- ' || SECT_ROOT_RAD as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_NAME = \"{profileString}\" ORDER BY SECT_DATE_ADDED";

        List<string> data = new List<string>();
        
        cmd.CommandType = CommandType.Text;
        SQLiteDataReader r = cmd.ExecuteReader();
        while (r.Read())
        {
          // get data
          string sqlData = System.Convert.ToString(r["SECT_NAME"]);

          // split text string
          // example (IPE100): 0.1 --  0.055 -- 0.0041 -- 0.0057 -- 0.007
          data.Add(sqlData);
        }
        db.Close();

        string[] vals = data[0].Split(new string[] { " -- " }, StringSplitOptions.None);

        foreach (string val in vals)
          values.Add(double.Parse(val));
      }
      return values;
    }
  }
}