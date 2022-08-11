using System;
using ComposAPI;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using Xunit;
using ComposGHTests.Helpers;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Grasshopper.Kernel.Types;

namespace ComposGHTests
{
  public class PrimitiveFixture : GrasshopperFixture
  {
    public PrimitiveFixture() : base("Primitives.gh") { }
  }

  public class GH_OasysGooTest : IClassFixture<PrimitiveFixture>
  {
    PrimitiveFixture fixture { get; set; }

    public GH_OasysGooTest(PrimitiveFixture fixture)
    {
      this.fixture = fixture;
    }
    
    [Theory]
    [InlineData(typeof(BeamSectionGoo), typeof(BeamSection))]
    [InlineData(typeof(RestraintGoo), typeof(Restraint))]
    [InlineData(typeof(SupportsGoo), typeof(Supports))]
    [InlineData(typeof(SteelMaterialGoo), typeof(SteelMaterial))]
    [InlineData(typeof(WebOpeningGoo), typeof(WebOpening))]
    [InlineData(typeof(WebOpeningStiffenersGoo), typeof(WebOpeningStiffeners))]
    [InlineData(typeof(StudDimensionsGoo), typeof(StudDimensions))]
    [InlineData(typeof(StudGoo), typeof(Stud))]
    [InlineData(typeof(StudGroupSpacingGoo), typeof(StudGroupSpacing))]
    [InlineData(typeof(StudSpecificationGoo), typeof(StudSpecification))]
    [InlineData(typeof(ConcreteMaterialGoo), typeof(ConcreteMaterial))]
    [InlineData(typeof(ERatioGoo), typeof(ERatio))]
    [InlineData(typeof(DeckingConfigurationGoo), typeof(DeckingConfiguration))]
    [InlineData(typeof(DeckingGoo), typeof(Decking))]
    [InlineData(typeof(CustomTransverseReinforcementLayoutGoo), typeof(CustomTransverseReinforcementLayout))]
    [InlineData(typeof(MeshReinforcementGoo), typeof(MeshReinforcement))]
    [InlineData(typeof(ReinforcementMaterialGoo), typeof(ReinforcementMaterial))]
    [InlineData(typeof(TransverseReinforcementGoo), typeof(TransverseReinforcement))]
    [InlineData(typeof(SlabDimensionGoo), typeof(SlabDimension))]
    [InlineData(typeof(SlabGoo), typeof(Slab))]
    [InlineData(typeof(LoadGoo), typeof(Load))]
    [InlineData(typeof(CreepShrinkageParametersGoo), typeof(CreepShrinkageParametersEN))]
    [InlineData(typeof(CreepShrinkageParametersGoo), typeof(CreepShrinkageParametersASNZ))]
    [InlineData(typeof(DesignCodeGoo), typeof(DesignCode))]
    [InlineData(typeof(SafetyFactorsENGoo), typeof(SafetyFactorsEN))]
    [InlineData(typeof(SafetyFactorsGoo), typeof(SafetyFactors))]
    [InlineData(typeof(BeamSizeLimitsGoo), typeof(BeamSizeLimits))]
    [InlineData(typeof(DeflectionLimitGoo), typeof(DeflectionLimit))]
    [InlineData(typeof(DesignCriteriaGoo), typeof(DesignCriteria))]
    [InlineData(typeof(FrequencyLimitsGoo), typeof(FrequencyLimits))]
    public void GenericGH_OasysGooTest(Type gooType, Type wrapType)
    {
      // Create the actual API object
      Object value = Activator.CreateInstance(wrapType);
      Object[] parameters = { value };

      // Create GH_OasysGoo<API_Object> 
      Object objectGoo = Activator.CreateInstance(gooType, parameters);

      // we can't cast directly to objectGoo.Value, so we do this instead
      PropertyInfo[] gooPropertyInfo = gooType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      foreach (PropertyInfo gooProperty in gooPropertyInfo)
      {
        if (gooProperty.Name == "Value")
        {
          Object gooValue = gooProperty.GetValue(objectGoo, null);
          // here check that the value in the goo object is a duplicate of the original object
          Duplicates.AreEqual(value, gooValue);

          // check some member properties have been set correctly
          Type typeSource = gooValue.GetType();
          PropertyInfo[] wrappedPropertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
          foreach (PropertyInfo wrapProperty in wrappedPropertyInfo)
          {
            // check the grasshopper tostring method (when you hover over the input/output)
            if (wrapProperty.Name == "TypeName")
            {
              string typeName = (string)wrapProperty.GetValue(gooValue, null);
              Assert.StartsWith("Compos " + typeName + " {", gooValue.ToString());
            }
            // check the name, input/output parameters
            if (wrapProperty.Name == "Name")
            {
              string name = (string)wrapProperty.GetValue(gooValue, null);
              // require a name longer than 3 characters (stud being the shortest accepted)
              Assert.True(name.Length > 3);
            }
            // check the nickname, input/output parameters
            if (wrapProperty.Name == "NickName")
            {
              string nickName = (string)wrapProperty.GetValue(gooValue, null);
              // require a nickname not longer than 3 characters excluding dots (".cob" being the exception)
              nickName = nickName.Replace(".", string.Empty);
              Assert.True(nickName.Length < 4);
            }
            if (wrapProperty.Name == "Description")
            {
              string description = (string)wrapProperty.GetValue(gooValue, null);
              // require a description to start with "Compos"
              Assert.StartsWith("Compos ", description);
              Assert.True(description.Length > 10);
            }
          }
        }
      }
    }
  }
}
