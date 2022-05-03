using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Tests
{
  public static class CatalogueDeckingMother
  {
    public static CatalogueDecking CreateCatalogueDecking()
    {
      DeckingConfiguration deckingConfiguration = DeckingConfigurationMother.CreateDeckingConfiguration();
      return new CatalogueDecking("catalogue", "profile", CatalogueDecking.DeckingSteelGrade.S280, deckingConfiguration);
    }
  }

  public class CatalogueDeckingTest
  {
   
  }
}
