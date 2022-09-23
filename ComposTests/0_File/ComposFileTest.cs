using System.Collections.Generic;
using System.IO;
using System.Text;
using ComposAPI.Helpers;
using Xunit;

namespace ComposAPI.File.Tests
{
  [Collection("ComposAPI Fixture collection")]
  public class ComposFileTest
  {
    static readonly string RelativePath = "..\\..\\..\\..\\TestFiles\\";

    [Theory]
    [InlineData("01_Default.coa")]
    [InlineData("02_DesignCode[Manual_CreepShrinkage].coa")]
    [InlineData("04_SteelMaterial[Custom].coa")]
    [InlineData("05_SteelMaterial[AZCode].coa")]
    [InlineData("07_BeamSection[CatalogueSearch].coa")]
    [InlineData("08_WebOpenings[Notch].coa")]
    [InlineData("09_WebOpenings[NotchWithStiffeners].coa")]
    [InlineData("10_WebOpenings[Opening].coa")]
    [InlineData("11_WebOpenings[OpeningWithStiffeners].coa")]
    [InlineData("12_ConcreteMaterial[HongKongCustom].coa")]
    [InlineData("13_ConcreteMaterial[AZ].coa")]
    [InlineData("14_ConcreteMaterial[BSCustom].coa")]
    [InlineData("15_Reinforcement[CustomRebarLayout].coa")]
    [InlineData("16_Decking[Custom].coa")]
    [InlineData("17_Studs[Standard].coa")]
    [InlineData("18_Studs[StandardTypeCustomDim].coa")]
    [InlineData("19_Studs[PartialTypeBSSpecification].coa")]
    [InlineData("20_Studs[MinTypeCustomSpecification].coa")]
    [InlineData("21_Studs[CustomTypeCustomSpecification].coa")]
    [InlineData("22_Loads[Point].coa")]
    [InlineData("23_Loads[Linear].coa")]
    [InlineData("24_Loads[TriLinear].coa")]
    [InlineData("25_Loads[Patch].coa")]
    [InlineData("26_Loads[Axial].coa")]
    [InlineData("Compos1.coa")]
    [InlineData("Compos2_UTF8.coa")] //Not found: UNIT_DATA	STRESS	N/m�	1.00000 
    public void AnalyseTest(string fileName)
    {
      string path = Path.GetFullPath(ComposFileTest.RelativePath);

      ComposFile file = ComposFile.Open(Path.Combine(path, fileName));
      short status = file.Analyse();
      Assert.Equal(0, status);
    }

    [Theory]
    [InlineData("Compos1.coa", "MEMBER-1")]
    public void AnalyseMemberTest(string fileName, string memberName)
    {
      ComposFile file = ComposFile.Open(Path.GetFullPath(ComposFileTest.RelativePath + fileName));
      short status = file.Analyse(memberName);
      Assert.Equal(0, status);
    }

    [Theory]
    [InlineData("Compos1.coa", "MEMBER-1", 0)]
    public void CodeSatisfiedTest(string searchPattern, string memberName, int expextedStatus)
    {
      string path = Path.GetFullPath(ComposFileTest.RelativePath);

      foreach (string fileName in Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly))
      {
        ComposFile file = ComposFile.Open(fileName);
        short status = file.CodeSatisfied(memberName);
        Assert.Equal(expextedStatus, status);
      }
    }

    [Theory]
    [InlineData("01_Default.coa")]
    [InlineData("02_DesignCode[Manual_CreepShrinkage].coa")]
    [InlineData("04_SteelMaterial[Custom].coa")]
    [InlineData("05_SteelMaterial[AZCode].coa")]
    [InlineData("07_BeamSection[CatalogueSearch].coa")]
    [InlineData("08_WebOpenings[Notch].coa")]
    [InlineData("09_WebOpenings[NotchWithStiffeners].coa")]
    [InlineData("10_WebOpenings[Opening].coa")]
    [InlineData("11_WebOpenings[OpeningWithStiffeners].coa")]
    [InlineData("12_ConcreteMaterial[HongKongCustom].coa")]
    [InlineData("13_ConcreteMaterial[AZ].coa")]
    [InlineData("14_ConcreteMaterial[BSCustom].coa")]
    [InlineData("15_Reinforcement[CustomRebarLayout].coa")]
    [InlineData("16_Decking[Custom].coa")]
    [InlineData("17_Studs[Standard].coa")]
    [InlineData("18_Studs[StandardTypeCustomDim].coa")]
    [InlineData("19_Studs[PartialTypeBSSpecification].coa")]
    [InlineData("20_Studs[MinTypeCustomSpecification].coa")]
    [InlineData("21_Studs[CustomTypeCustomSpecification].coa")]
    [InlineData("22_Loads[Point].coa")]
    [InlineData("23_Loads[Linear].coa")]
    [InlineData("24_Loads[TriLinear].coa")]
    [InlineData("25_Loads[Patch].coa")]
    [InlineData("26_Loads[Axial].coa")]
    [InlineData("Compos1.coa")]
    [InlineData("Compos2_UTF8.coa")] //Not found: UNIT_DATA	STRESS	N/m�	1.00000
    public void DesignTest(string fileName)
    {
      string path = Path.GetFullPath(ComposFileTest.RelativePath);

      ComposFile file = ComposFile.Open(Path.Combine(path, fileName));
      short status = file.Design();
      Assert.Equal(0, status);
    }

    [Theory]
    [InlineData("Compos1.coa", "MEMBER-1")]
    public void DesignMemberTest(string fileName, string memberName)
    {
      ComposFile file = ComposFile.Open(Path.GetFullPath(ComposFileTest.RelativePath + fileName));
      short status = file.Design(memberName);
      Assert.Equal(0, status);
    }

    [Theory]
    [InlineData("Compos1.coa", "MEMBER-1")]
    public void BeamSectDescTest(string fileName, string memberName)
    {
      ComposFile file = ComposFile.Open(Path.GetFullPath(ComposFileTest.RelativePath + fileName));
      string status = file.BeamSectDesc(memberName);
      Assert.Equal("STD I 600. 200. 15. 25.", status);
    }

    [Theory]
    [InlineData("01_Default.coa")]
    [InlineData("02_DesignCode[Manual_CreepShrinkage].coa")]
    [InlineData("04_SteelMaterial[Custom].coa")]
    [InlineData("05_SteelMaterial[AZCode].coa")]
    [InlineData("07_BeamSection[CatalogueSearch].coa")]
    [InlineData("08_WebOpenings[Notch].coa")]
    [InlineData("09_WebOpenings[NotchWithStiffeners].coa")]
    [InlineData("10_WebOpenings[Opening].coa")]
    [InlineData("11_WebOpenings[OpeningWithStiffeners].coa")]
    [InlineData("12_ConcreteMaterial[HongKongCustom].coa")]
    [InlineData("13_ConcreteMaterial[AZ].coa")]
    [InlineData("14_ConcreteMaterial[BSCustom].coa")]
    [InlineData("15_Reinforcement[CustomRebarLayout].coa")]
    [InlineData("16_Decking[Custom].coa")]
    [InlineData("17_Studs[Standard].coa")]
    [InlineData("18_Studs[StandardTypeCustomDim].coa")]
    [InlineData("19_Studs[PartialTypeBSSpecification].coa")]
    [InlineData("20_Studs[MinTypeCustomSpecification].coa")]
    [InlineData("21_Studs[CustomTypeCustomSpecification].coa")]
    [InlineData("22_Loads[Point].coa")]
    [InlineData("23_Loads[Linear].coa")]
    [InlineData("24_Loads[TriLinear].coa")]
    [InlineData("25_Loads[Patch].coa")]
    [InlineData("26_Loads[Axial].coa")]
    [InlineData("Compos1.coa")]
    [InlineData("Compos2_UTF8.coa")] //Not found: UNIT_DATA	STRESS	N/m�	1.00000
    public void FromAndToCoaStringTest(string fileName)
    {
      string path = Path.GetFullPath(ComposFileTest.RelativePath);
      string expectedCoaString = System.IO.File.ReadAllText(Path.Combine(path, fileName), Encoding.UTF8);
      ComposFile file = ComposFile.FromCoaString(expectedCoaString);
      string actualCoaString = file.ToCoaString();
      ComposFileTest.Compare(expectedCoaString, actualCoaString);
    }

    internal static void Compare(string expectedCoaString, string actualCoaString)
    {
      List<string> expectedLines = CoaHelper.SplitAndStripLines(expectedCoaString);
      List<string> actualLines = CoaHelper.SplitAndStripLines(actualCoaString);

      foreach (string expectedLine in expectedLines)
      {
        if (expectedLine != "")
          Assert.Contains(expectedLine, actualLines);
      }
    }
  }
}
