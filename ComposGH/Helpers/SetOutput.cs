using Grasshopper;
using Grasshopper.Kernel;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnitsNet.Serialization.JsonNet;

namespace ComposGH.Components
{
  class SetOutput
  {
    private static UnitsNetIQuantityJsonConverter converter = new UnitsNetIQuantityJsonConverter();
    internal static void Item(GH_OasysDropDownComponent owner, IGH_DataAccess DA, int inputid, object data)
    {
      DA.SetData(inputid, data);
      int outputsSerialized = JsonConvert.SerializeObject(data, converter).GetHashCode();

      if (!owner.ExistingOutputsSerialized.ContainsKey(inputid))
      {
        owner.ExistingOutputsSerialized[inputid] = new List<int>() { outputsSerialized };
        owner.UpdateOutput = true;
      }
      else if (owner.ExistingOutputsSerialized[inputid][0] != outputsSerialized)
      {
        owner.ExistingOutputsSerialized[inputid][0] = outputsSerialized;
        owner.UpdateOutput = true;
      }
      else
        owner.UpdateOutput = false;
    }
    internal static void List<T>(GH_OasysDropDownComponent owner, IGH_DataAccess DA, int inputid, List<T> data)
    {
      DA.SetDataList(inputid, data);
      
      if (!owner.ExistingOutputsSerialized.ContainsKey(inputid))
      {
        owner.ExistingOutputsSerialized[inputid] = new List<int>();
        owner.UpdateOutput = true;
      }
      
      for (int i = 0; i < data.Count; i++)
      {
        int outputsSerialized = JsonConvert.SerializeObject(data, converter).GetHashCode();
        if (owner.ExistingOutputsSerialized[inputid].Count == i)
        {
          owner.UpdateOutput = true;
          owner.ExistingOutputsSerialized[inputid].Add(outputsSerialized);
        }
        else if (owner.ExistingOutputsSerialized[inputid][i] != outputsSerialized)
        {
          owner.UpdateOutput = true;
          owner.ExistingOutputsSerialized[inputid][i] = outputsSerialized;
        }
        else
          owner.UpdateOutput = false;
      }
    }

    internal static void Tree<T>(GH_OasysDropDownComponent owner, IGH_DataAccess DA, int inputid, DataTree<T> dataTree)
    {
      DA.SetDataTree(inputid, dataTree);

      if (!owner.ExistingOutputsSerialized.ContainsKey(inputid))
      {
        owner.ExistingOutputsSerialized[inputid] = new List<int>();
        owner.UpdateOutput = true;
      }

      int counter = 0;
      for (int p = 0; p < dataTree.Paths.Count; p++)
      {
        List<T> data = dataTree.Branch(dataTree.Paths[p]);
        for (int i = counter; i < data.Count - counter; i++)
        {
          int outputsSerialized = JsonConvert.SerializeObject(data, converter).GetHashCode();
          if (owner.ExistingOutputsSerialized[inputid].Count == i)
          {
            owner.UpdateOutput = true;
            owner.ExistingOutputsSerialized[inputid].Add(outputsSerialized);
            return;
          }
          else if (owner.ExistingOutputsSerialized[inputid][i] != outputsSerialized)
          {
            owner.UpdateOutput = true;
            owner.ExistingOutputsSerialized[inputid][i] = outputsSerialized;
            return;
          }
          else
            owner.UpdateOutput = false;
        }
        counter = data.Count;
      }
    }
  }
}
