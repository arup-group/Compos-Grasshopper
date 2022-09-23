using Xunit;

namespace ComposAPI.Results.Tests
{
    [Collection("ComposAPI Fixture collection")]

  public class UtilisationsTest
  {
    [Fact]
    public void MomentUtilisationTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IUtilisation res = r.Utilisations;

      Assert.Equal(130.8, res.Moment.Percent, 1);
    }

    [Fact]
    public void ShearUtilisationTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IUtilisation res = r.Utilisations;

      Assert.Equal(99.39, res.Shear.Percent, 2);
    }

    [Fact]
    public void MomentConstructionUtilisationTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IUtilisation res = r.Utilisations;

      Assert.Equal(165.8, res.MomentConstruction.Percent, 1);
    }

    [Fact]
    public void ShearConstructionUtilisationTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IUtilisation res = r.Utilisations;

      Assert.Equal(40.86, res.ShearConstruction.Percent, 2);
    }

    [Fact]
    public void BucklingConstructionUtilisationTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IUtilisation res = r.Utilisations;

      Assert.Equal(241.2, res.BucklingConstruction.Percent, 1);
    }

    [Fact]
    public void DeflectionUtilisationTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IUtilisation res = r.Utilisations;

      Assert.Equal(0, res.Deflection.Percent, 2);
    }

    [Fact]
    public void DeflectionConstructionUtilisationTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IUtilisation res = r.Utilisations;

      Assert.Equal(0, res.DeflectionConstruction.Percent, 2);
    }

    [Fact]
    public void TransverseShearUtilisationTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IUtilisation res = r.Utilisations;

      Assert.Equal(55.71, res.TransverseShear.Percent, 2);
    }

    [Fact]
    public void WebOpeningUtilisationTest()
    {
      IResult r = ResultsTest.ResultMember.Result;
      IUtilisation res = r.Utilisations;

      Assert.Equal(0, res.WebOpening.Percent, 2);
    }
  }
}
