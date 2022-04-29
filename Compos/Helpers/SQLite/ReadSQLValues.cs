using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace ComposAPI.Helpers
{
  public static class SqlDBconnection
  {
    public static SQLiteConnection ConnectSectionCatalogue()
    {
      string filePath = Path.Combine(ComposIO.InstallPath, "sectlib.db3");
      return new SQLiteConnection($"URI=file:{filePath};mode=ReadOnly");
    }
    public static SQLiteConnection ConnectDeckingCatalogue()
    {
      string filePath = Path.Combine(ComposIO.InstallPath, "sectlib.db3");
      return new SQLiteConnection($"URI=file:{filePath};mode=ReadOnly");
    }
  }
  public class CatalogueValues
  {
    /// <summary>
    /// This method will return a list of double with values in [m] units and ordered as follows:
    /// [0]: Depth
    /// [1]: Width
    /// [2]: Web THK
    /// [3]: Flange THK
    /// [4]: Root radius
    /// </summary>
    /// <param name="profileString"></param>
    /// <returns></returns>
    public static List<double> GetCatalogueProfileValues(string profileString)
    {
      // Create empty lists to work on:
      List<double> values = new List<double>();

      using (var db = SqlDBconnection.ConnectSectionCatalogue())
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
          string sqlData = Convert.ToString(r["SECT_NAME"]);

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
