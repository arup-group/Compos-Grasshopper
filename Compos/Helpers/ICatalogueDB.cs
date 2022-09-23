using System.Collections.Generic;

namespace ComposAPI.Helpers
{
  public interface ICatalogueDB
  {
    List<double> GetCatalogueProfileValues(string profileString);
    List<double> GetCatalogueDeckingValues(string catalogue, string profile);
  }
}
