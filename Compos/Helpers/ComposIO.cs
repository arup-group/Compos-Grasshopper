using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Compos_8_6;

namespace ComposAPI
{
  public static class ComposIO
  {
    public static string InstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Oasys", "Compos 8.6");

    public static IAutomation Open(string pathName)
    {
      IAutomation automation = new Automation();
      automation.Open(pathName);
      return automation;
    }

  }
}
