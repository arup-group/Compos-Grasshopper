using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ComposAPI.Helpers;
using Oasys.Units;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Tests
{
  public class TestsFixture : IDisposable
  {
    public TestsFixture()
    {
      // Do "global" initialization here; Only called once.
      
      string installPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Oasys", "Compos 8.6");
      
      const string name = "PATH";
      string pathvar = Environment.GetEnvironmentVariable(name);
      var value = pathvar + ";" + installPath;
      var target = EnvironmentVariableTarget.Process;
      Environment.SetEnvironmentVariable(name, value, target);
    }

    public void Dispose()
    {
      // Do "global" teardown here; Only called once.
      ComposFile.Close();
      Process[] ps = Process.GetProcessesByName("Compos");
      foreach (Process p in ps)
        p.Kill();
    }
  }
  [CollectionDefinition("ComposAPI Fixture collection")]
  public class GrasshopperCollection : ICollectionFixture<TestsFixture>
  {
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
  }
}
