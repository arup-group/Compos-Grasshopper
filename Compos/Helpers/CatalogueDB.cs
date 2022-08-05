using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;

namespace ComposAPI.Helpers
{
  public class CatalogueDB : ICatalogueDB
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
    public List<double> GetCatalogueProfileValues(string profileString)
    {
      // Create empty lists to work on:
      List<double> values = new List<double>();

      using (var db = SqlDBconnection.ConnectSectionCatalogue())
      {
        db.Open();
        SQLiteCommand cmd = db.CreateCommand();
        cmd.CommandText = $"Select " +
          $"SECT_DEPTH_DIAM || ' -- ' || " +
          $"SECT_WIDTH || ' -- ' || " +
          $"SECT_WEB_THICK || ' -- ' || " +
          $"SECT_FLG_THICK || ' -- ' || " +
          $"SECT_ROOT_RAD " +
          $"as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_NAME = \"{profileString}\" ORDER BY SECT_DATE_ADDED";

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

        NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

        foreach (string val in vals)
          if (val != "")
            values.Add(Convert.ToDouble(val, noComma));
      }
      return values;
    }

    /// <summary>
    /// This method will return a list of double with values in [m] units and ordered as follows:
    /// [0]: d, Depth
    /// [1]: b1, Spacing
    /// [2]: b2, Upper width
    /// [3]: b3, Lower width
    /// [4]: b4, Proj Height
    /// [5]: b5, Proj Width
    /// [5]: t, thickness
    /// </summary>
    /// <param name="catalogue"></param>
    /// <param name="profile"></param>
    /// <returns></returns>
    public List<double> GetCatalogueDeckingValues(string catalogue, string profile)
    {
      // Create empty lists to work on:
      List<double> values = new List<double>();

      using (var db = SqlDBconnection.ConnectDeckingCatalogue())
      {
        db.Open();
        SQLiteCommand cmd = db.CreateCommand();
        cmd.CommandText = $"Select " +
          $"Deck_Depth || ' -- ' || " +
          $"Deck_Spacing || ' -- ' || " +
          $"Deck_UpperWidth || ' -- ' || " +
          $"Deck_LowerWidth || ' -- ' || " +
          $"Deck_Proj_Height || ' -- ' || " +
          $"Deck_Proj_Width || ' -- ' || " +
          $"Deck_thickness " +
          $"as Deck_Name from Deck_{catalogue} where Deck_Name = \"{profile}\" ";

        List<string> data = new List<string>();

        cmd.CommandType = CommandType.Text;
        SQLiteDataReader r = cmd.ExecuteReader();
        while (r.Read())
        {
          // get data
          string sqlData = Convert.ToString(r["Deck_Name"]);

          // split text string
          data.Add(sqlData);
        }
        db.Close();

        if(data == null || data.Count < 1)
          return values;

        string[] vals = data[0].Split(new string[] { " -- " }, StringSplitOptions.None);

        NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

        foreach (string val in vals)
          values.Add(Convert.ToDouble(val, noComma));
      }
      return values;
    }
  }
}
