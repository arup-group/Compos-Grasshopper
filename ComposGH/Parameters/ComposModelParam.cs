using System;
using System.IO;
using ComposGH.Helpers;
using Grasshopper.Kernel.Types;
using OasysGH;

namespace ComposGH.Parameters {
  /// <summary>
  /// Model class, this class defines the basic properties and methods for any Compos Model
  /// </summary>
  [Serializable]
  public class ComposModel {
    public string FileName {
      get => m_filename;
      set => m_filename = value;
    }
    public Guid GUID => m_guid;
    public bool IsValid {
      get {
        if (m_model == null) {
          return false;
        }
        return true;
      }
    }
    public dynamic Model {
      get => m_model;
      set => m_model = value;
    }
    private string m_filename = "";
    private Guid m_guid = Guid.NewGuid();
    private dynamic m_model;

    public ComposModel() {
      m_model = System.Activator.CreateInstance(System.Type.GetTypeFromProgID("Compos.Automation"));
    }

    /// <summary>
    /// This method will save .coa as a temp file and reopen it,
    /// </summary>
    /// <returns>Return opened model with new GUID</returns>
    public ComposModel Clone() {
      var clone = new ComposModel();

      // workaround duplicate model
      string tempfilename = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Oasys") + "Compos-Grasshopper_temp_.coa";
      m_model.SaveAs(tempfilename);
      clone.Model.Open(tempfilename);

      clone.FileName = m_filename;
      clone.m_guid = Guid.NewGuid();
      return clone;
    }

    public override string ToString() {
      //Could add detailed description of model content here
      string s = "";
      if (FileName != null) {
        if (FileName != "" && FileName.Length > 4) {
          s = Path.GetFileName(FileName);
          s = s.Substring(0, s.Length - 4);
          s = " (" + s + ")";
        }
      }

      return "Compos Model" + s;
    }
  }

  /// <summary>
  /// ComposMember Goo wrapper class, makes sure GsaMember can be used in Grasshopper.
  /// </summary>
  public class ComposModelGoo : GH_Goo<ComposModel> {
    public override bool IsValid {
      get {
        if (Value.Model == null) { return false; }
        return true;
      }
    }

    public override string IsValidWhyNot {
      get {
        //if (Value == null) { return "No internal GsaMember instance"; }
        if (Value.IsValid) { return string.Empty; }
        return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
      }
    }

    public override string TypeDescription => "Compos Model";

    public override string TypeName => "Compos Model";

    public ComposModelGoo() {
      Value = new ComposModel();
    }

    public ComposModelGoo(ComposModel model) {
      if (model == null) {
        model = new ComposModel();
      }
      Value = model; //model.Duplicate();
    }

    public override bool CastFrom(object source) {
      // This function is called when Grasshopper needs to convert other data
      // into GsaModel.

      if (source == null) { return false; }

      //Cast from GsaModel
      if (typeof(ComposModel).IsAssignableFrom(source.GetType())) {
        Value = (ComposModel)source;
        return true;
      }

      return false;
    }

    public override bool CastTo<Q>(ref Q target) {
      // This function is called when Grasshopper needs to convert this
      // instance of GsaModel into some other type Q.

      if (typeof(Q).IsAssignableFrom(typeof(ComposModel))) {
        if (Value == null) {
          target = default;
        } else {
          target = (Q)(object)Value.Duplicate();
        }
        return true;
      }

      target = default;
      return false;
    }

    public override IGH_Goo Duplicate() {
      return DuplicateComposModel();
    }

    public ComposModelGoo DuplicateComposModel() {
      return new ComposModelGoo(Value ?? new ComposModel()); //Value.Duplicate());
    }

    public override string ToString() {
      if (Value == null) {
        return "Null Compos Model";
      } else {
        return Value.ToString();
      }
    }
  }

  /// <summary>
  /// This class provides a Parameter interface for the Data_GsaModel type.
  /// </summary>
}
