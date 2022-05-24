using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using Xunit;

namespace ComposAPI.Beams.Tests
{
  public class BeamTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(6)]
    public void ConstructorTest(double length)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();

      // 2 create object instance with constructor
      Restraint restraint = new Restraint();
      SteelMaterial material = new SteelMaterial();
      List<IBeamSection> sections = new List<IBeamSection>() { new BeamSection() };
      List<IWebOpening> openings = new List<IWebOpening>() { new WebOpening() };

      Beam beam = new Beam(new Length(length, units.Length), restraint, material, sections, openings);

      // 3 check that inputs are set in object's members
      Assert.Equal(length, beam.Length.Value);
      Assert.Equal(restraint, beam.Restraint);
      Assert.Equal(material, beam.Material);
      Assert.Equal(sections, beam.Sections);
      Assert.Equal(openings, beam.WebOpenings);
    }
  }
}
