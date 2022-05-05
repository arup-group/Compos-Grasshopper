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
  public static class DeckingConfigurationMother
  {
    public static DeckingConfiguration CreateDeckingConfiguration()
    {
      AngleUnit angleUnit = AngleUnit.Radian;
      return new DeckingConfiguration(new Angle(Math.PI / 2, angleUnit), true, true);
    }
  }

  public class DeckingConfigurationTest
  {

  }
}
