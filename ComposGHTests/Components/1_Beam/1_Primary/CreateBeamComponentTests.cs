﻿using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;

namespace ComposGHTests.Beam {
  [Collection("GrasshopperFixture collection")]
  public class CreateBeamComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateBeam();
      comp.CreateAttributes();

      var start = new Point3d(0, 0, 0);
      var end = new Point3d(0, 7, 0);
      var input1 = new Line(start, end);

      var input2 = new RestraintGoo(new Restraint());

      var input3 = new SteelMaterialGoo(new SteelMaterial(StandardSteelGrade.S355, Code.EN1994_1_1_2004));

      var input4 = new BeamSectionGoo(new BeamSection("CAT HE HE500.B"));

      ComponentTestHelper.SetInput(comp, input1, 0);
      ComponentTestHelper.SetInput(comp, input2, 1);
      ComponentTestHelper.SetInput(comp, input3, 2);
      ComponentTestHelper.SetInput(comp, input4, 3);

      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponentWithInputsTest() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var expectedRestraint = new RestraintGoo(new Restraint());

      var expectedMaterial = new SteelMaterialGoo(new SteelMaterial(StandardSteelGrade.S355, Code.EN1994_1_1_2004));

      var expectetBeamSection = new BeamSectionGoo(new BeamSection("CAT HE HE500.B"));

      var output = (BeamGoo)ComponentTestHelper.GetOutput(comp);
      Duplicates.AreEqual(expectedRestraint.Value, output.Value.Restraint);
      Duplicates.AreEqual(expectedMaterial.Value, output.Value.Material);
      Duplicates.AreEqual(expectetBeamSection.Value, output.Value.Sections[0]);
    }

    [Fact]
    public void CreateComponentWithInputsTest2() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var beamSection = new BeamSection("CAT IPE IPE400") {
        StartPosition = new Ratio(50, RatioUnit.Percent)
      };
      var input4_2 = new BeamSectionGoo(beamSection);
      ComponentTestHelper.SetInput(comp, input4_2, 3);

      var input5_1 = (WebOpeningGoo)ComponentTestHelper.GetOutput(CreateNotchComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input5_1, 4);

      var input5_2 = (WebOpeningGoo)ComponentTestHelper.GetOutput(CreateWebOpeningComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input5_2, 4);

      var output = (BeamGoo)ComponentTestHelper.GetOutput(comp);
      Duplicates.AreEqual(input4_2.Value, output.Value.Sections[1]);
      Duplicates.AreEqual(input5_1.Value, output.Value.WebOpenings[0]);
      Duplicates.AreEqual(input5_2.Value, output.Value.WebOpenings[1]);
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
