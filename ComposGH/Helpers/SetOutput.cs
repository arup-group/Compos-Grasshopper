using ComposGH.Components;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet.Serialization.JsonNet;

namespace ComposGH.Components
{
  class SetOutput
  {
    private static UnitsNetIQuantityJsonConverter converter = new UnitsNetIQuantityJsonConverter();
    internal static void GenericGoo(GH_OasysDropDownComponent owner, IGH_DataAccess DA, int inputid, object data)
    {
      DA.SetData(inputid, data);
      int outputs_serialized = JsonConvert.SerializeObject(data, converter).GetHashCode();

      if (owner.existing_outputs_serialized[0] != outputs_serialized)
      {
        owner.UpdateOutput = true;
        owner.existing_outputs_serialized[0] = outputs_serialized;
      }
      else
        owner.UpdateOutput = false;
    }
    internal static void GenericGoo(GH_OasysDropDownComponent owner, IGH_DataAccess DA, int inputid, List<object> data)
    {
      DA.SetDataList(inputid, data);
      for (int i = 0; i < data.Count; i++)
      {
        int outputs_serialized = JsonConvert.SerializeObject(data, converter).GetHashCode();
        if (owner.existing_outputs_serialized.Count == i)
        {
          owner.UpdateOutput = true;
          owner.existing_outputs_serialized.Add(outputs_serialized);
        }
        else if (owner.existing_outputs_serialized[i] != outputs_serialized)
        {
          owner.UpdateOutput = true;
          owner.existing_outputs_serialized[i] = outputs_serialized;
        }
        else
          owner.UpdateOutput = false;
      }
    }

    internal static void GenericGoo(GH_OasysDropDownComponent owner, IGH_DataAccess DA, int inputid, DataTree<object> dataTree)
    {
      DA.SetDataTree(inputid, dataTree);
      int counter = 0;
      for (int p = 0; p < dataTree.Paths.Count; p++)
      {
        List<object> data = dataTree.Branch(dataTree.Paths[p]);
        for (int i = counter; i < data.Count - counter; i++)
        {
          int outputs_serialized = JsonConvert.SerializeObject(data, converter).GetHashCode();
          if (owner.existing_outputs_serialized.Count == i)
          {
            owner.UpdateOutput = true;
            owner.existing_outputs_serialized.Add(outputs_serialized);
            return;
          }
          else if (owner.existing_outputs_serialized[i] != outputs_serialized)
          {
            owner.UpdateOutput = true;
            owner.existing_outputs_serialized[i] = outputs_serialized;
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
