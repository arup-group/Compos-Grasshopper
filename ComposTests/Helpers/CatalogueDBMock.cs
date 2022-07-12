using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComposAPITests.Helpers
{
  internal class MockCatalogueDB : ICatalogueDB
  {
    public List<double> GetCatalogueDeckingValues(string catalogue, string profile)
    {
      //"RLD", "Ribdeck AL (1.2)"  300, 120, 140, 10, 40, 50, 1.2)]
      //"Kingspan", "Multideck 50 (0.85)" 150, 40, 135, 0, 0, 50, 0.85)
      if (catalogue.ToLower() == "kingspan")
        return new List<double>() { 0.05, 0.15, 0.04, 0.135, 0, 0, 0.00085 };
      else
        return new List<double> { 0.05, 0.300, 0.12, 0.14, 0.01, 0.04, 0.0012 };
    }

    public List<double> GetCatalogueProfileValues(string profileString)
    {
      // split text string
      // example (IPE100): 0.1 --  0.055 -- 0.0041 -- 0.0057 -- 0.007
      return new List<double> { 0.100, 0.055, 0.0041, 0.0057, 0.007 };
    }
  }
}
