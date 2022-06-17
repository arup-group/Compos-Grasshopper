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
      string filePath = Path.Combine(ComposIO.InstallPath, "decking.db3");
      return new SQLiteConnection($"URI=file:{filePath};mode=ReadOnly");
    }
  }
}
