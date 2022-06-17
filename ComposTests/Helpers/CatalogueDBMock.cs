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
      return new List<double> { 0.100, 0.055, 0.055, 0.0041, 0.0057, 0.0057 };
    }

    public List<double> GetCatalogueProfileValues(string profileString)
    {
      // split text string
      // example (IPE100): 0.1 --  0.055 -- 0.0041 -- 0.0057 -- 0.007
      return new List<double> { 0.100, 0.055, 0.041, 0.0057, 0.007 };
    }
  }
}
