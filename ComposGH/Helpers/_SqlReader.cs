﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;

namespace ComposGH.Helpers
{
  /// <summary>
  /// Class containing functions to interface with SQLite db files.
  /// </summary>
  public class SqlReader
  {
    /// <summary>
    /// Method to set up a SQLite Connection to a specified .db3 file.
    /// Will return a SQLite connection to the aforementioned .db3 file database.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static SQLiteConnection Connection(string filePath)
    {
      return new SQLiteConnection($"URI=file:{filePath};mode=ReadOnly");
    }

    /// <summary>
    /// Get catalogue data from SQLite file (.db3). The method returns a tuple with:
    /// Item1 = list of catalogue name (string)
    /// where first item will be "All"
    /// Item2 = list of catalogue number (int)
    /// where first item will be "-1" representing All
    /// </summary>
    /// <param name="filePath">Path to SecLib.db3</param>
    /// <returns></returns>
    public static Tuple<List<string>, List<int>> GetCataloguesDataFromSQLite(string filePath)
    {
      // Create empty lists to work on:
      List<string> catNames = new List<string>();
      List<int> catNumber = new List<int>();

      using (var db = Connection(filePath))
      {
        db.Open();
        SQLiteCommand cmd = db.CreateCommand();
        cmd.CommandText = @"Select CAT_NAME || ' -- ' || CAT_NUM as CAT_NAME from Catalogues";

        cmd.CommandType = CommandType.Text;
        SQLiteDataReader r = cmd.ExecuteReader();
        while (r.Read())
        {
          // get data
          string sqlData = Convert.ToString(r["CAT_NAME"]);

          // split text string
          // example: British -- 2
          catNames.Add(sqlData.Split(new string[] { " -- " }, StringSplitOptions.None)[0]);
          catNumber.Add(Int32.Parse(sqlData.Split(new string[] { " -- " }, StringSplitOptions.None)[1]));
        }
        db.Close();
      }
      catNames.Insert(0, "All");
      catNumber.Insert(0, -1);
      return new Tuple<List<string>, List<int>>(catNames, catNumber);
    }

    /// <summary>
    /// Get section type data from SQLite file (.db3). The method returns a tuple with:
    /// Item1 = list of type name (string)
    /// where first item will be "All"
    /// Item2 = list of type number (int)
    /// where first item will be "-1" representing All
    /// </summary>
    /// <param name="catalogue_number">Catalogue number to get section types from. Input -1 in first item of the input list to get all types</param>
    /// <param name="filePath">Path to SecLib.db3</param>
    /// <param name="inclSuperseded">True if you want to include superseded items</param>
    /// <returns></returns>
    public static Tuple<List<string>, List<int>> GetTypesDataFromSQLite(int catalogue_number, string filePath, bool inclSuperseded = false)
    {
      // Create empty lists to work on:
      List<string> typeNames = new List<string>();
      List<int> typeNumber = new List<int>();

      // get Catalogue numbers if input is -1 (All catalogues)
      List<int> catNumbers = new List<int>();
      if (catalogue_number == -1)
      {
        Tuple<List<string>, List<int>> catalogueData = GetCataloguesDataFromSQLite(filePath);
        catNumbers = catalogueData.Item2;
        catNumbers.RemoveAt(0); // remove -1 from beginning of list
      }
      else
        catNumbers.Add(catalogue_number);

      using (var db = Connection(filePath))
      {
        for (int i = 0; i < catNumbers.Count; i++)
        {
          int cat = catNumbers[i];

          db.Open();
          SQLiteCommand cmd = db.CreateCommand();
          if (inclSuperseded)
            cmd.CommandText = $"Select TYPE_NAME || ' -- ' || TYPE_NUM as TYPE_NAME from Types where TYPE_CAT_NUM = {cat}";
          else
            cmd.CommandText = $"Select TYPE_NAME || ' -- ' || TYPE_NUM as TYPE_NAME from Types where TYPE_CAT_NUM = {cat} and not (TYPE_SUPERSEDED = True or TYPE_SUPERSEDED = TRUE or TYPE_SUPERSEDED = 1)";
          cmd.CommandType = CommandType.Text;
          SQLiteDataReader r = cmd.ExecuteReader();
          while (r.Read())
          {
            // get data
            string sqlData = Convert.ToString(r["TYPE_NAME"]);

            // split text string
            // example: Universal Beams -- 51
            typeNames.Add(sqlData.Split(new string[] { " -- " }, StringSplitOptions.None)[0]);
            typeNumber.Add(Int32.Parse(sqlData.Split(new string[] { " -- " }, StringSplitOptions.None)[1]));
          }
          db.Close();
        }
      }
      typeNames.Insert(0, "All");
      typeNumber.Insert(0, -1);
      return new Tuple<List<string>, List<int>>(typeNames, typeNumber);
    }


    /// <summary>
    /// Get a list of section profile strings from SQLite file (.db3). The method returns a string that includes type abbriviation as accepted by GSA. 
    /// </summary>
    /// <param name="type_numbers">List of types to get sections from</param>
    /// <param name="filePath">Path to SecLib.db3</param>
    /// <param name="inclSuperseded">True if you want to include superseded items</param>
    /// <returns></returns>
    public static List<string> GetSectionsDataFromSQLite(List<int> type_numbers, string filePath, bool inclSuperseded = false)
    {
      // Create empty list to work on:
      List<string> section = new List<string>();

      List<int> types = new List<int>();
      if (type_numbers[0] == -1)
      {
        Tuple<List<string>, List<int>> typeData = GetTypesDataFromSQLite(-1, filePath, inclSuperseded);
        types = typeData.Item2;
        types.RemoveAt(0); // remove -1 from beginning of list
      }
      else
        types = type_numbers;

      using (var db = Connection(filePath))
      {
        // get section name
        for (int i = 0; i < types.Count; i++)
        {
          int type = types[i];
          db.Open();
          SQLiteCommand cmd = db.CreateCommand();

          if (inclSuperseded)
            cmd.CommandText = $"Select Types.TYPE_ABR || ' ' || SECT_NAME || ' -- ' || SECT_DATE_ADDED as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_TYPE_NUM = {type} ORDER BY SECT_AREA";
          else
            cmd.CommandText = $"Select Types.TYPE_ABR || ' ' || SECT_NAME as SECT_NAME from Sect INNER JOIN Types ON Sect.SECT_TYPE_NUM = Types.TYPE_NUM where SECT_TYPE_NUM = {type} and not (SECT_SUPERSEDED = True or SECT_SUPERSEDED = TRUE or SECT_SUPERSEDED = 1) ORDER BY SECT_AREA";

          cmd.CommandType = CommandType.Text;
          SQLiteDataReader r = cmd.ExecuteReader();
          while (r.Read())
          {
            if (inclSuperseded)
            {
              string full = Convert.ToString(r["SECT_NAME"]);
              // BSI-IPE IPEAA80 -- 2017-09-01 00:00:00.000
              string profile = full.Split(new string[] { " -- " }, StringSplitOptions.None)[0];
              string date = full.Split(new string[] { " -- " }, StringSplitOptions.None)[1];
              date = date.Replace("-", "");
              date = date.Substring(0, 8);
              section.Add(profile + " " + date);
            }
            else
            {
              string profile = Convert.ToString(r["SECT_NAME"]);
              // BSI-IPE IPEAA80                           
              section.Add(profile);
            }

          }
          db.Close();
        }
      }
            section.Insert(0, "All");
            return section;
    }


    public static List<string> GetDeckCataloguesDataFromSQLite(string filePath)
        {
            // Create empty lists to work on:
            List<string> catNames = new List<string>();

            using (var db = Connection(filePath))
            {
                db.Open();
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = @"Select Catalogue_Name || ' -- ' || Catalogue_ID as Catalogue_Name from Catalogue";

                cmd.CommandType = CommandType.Text;
                SQLiteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    // get data
                    string sqlData = System.Convert.ToString(r["Catalogue_Name"]);

                    // split text string
                    catNames.Add(sqlData.Split(new string[] { " -- " }, StringSplitOptions.None)[0]);
                }
                db.Close();
            }
            return new List<string>(catNames);
        }

    public static List<string> GetDeckingDataFromSQLite(string filePath, string cat)
        {
            // Create empty lists to work on:
            List<string> catNames = new List<string>();

            using (var db = Connection(filePath))
            {
                db.Open();
                SQLiteCommand cmd = db.CreateCommand();
                //cmd.CommandText = $"Select Type_{cat}.TYPE_ABR || ' ' || Deck_Name as Deck_Name from Deck_{cat} INNER JOIN Type_{cat} ON Deck_{cat}.Deck_Type_ID = Type_{cat}.TYPE_ID ORDER BY Deck_Thickness";
                cmd.CommandText = $"Select Deck_Name from Deck_{cat} ORDER BY Deck_Thickness";
                cmd.CommandType = CommandType.Text;
                SQLiteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    // get data
                    string sqlData = System.Convert.ToString(r["Deck_Name"]);

                    // split text string
                    catNames.Add(sqlData);
                }
                db.Close();
            }
            return new List<string>(catNames);
        }

    }
}
