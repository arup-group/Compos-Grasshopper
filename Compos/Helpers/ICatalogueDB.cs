using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace ComposAPI.Helpers
{
  public interface ICatalogueDB
  {
    List<double> GetCatalogueProfileValues(string profileString);
    List<double> GetCatalogueDeckingValues(string catalogue, string profile);
  }
}
