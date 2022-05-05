using System.Collections.Generic;

namespace ComposAPI
{
  /// <summary>
  /// Custom interface that defines the basic properties and methods for our custom class
  /// </summary>
  public interface ISlab
  {
     IConcreteMaterial Material { get;  }
     List<ISlabDimension> Dimensions { get;  }  
     ITransverseReinforcement TransverseReinforcement { get;  }
     IMeshReinforcement MeshReinforcement { get;  }  
     IDecking Decking { get;  }  
  }
}
