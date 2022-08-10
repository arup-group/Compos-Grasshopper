
using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Helpers;

namespace ComposGH.Components
{
  public abstract class GH_OasysDropDownComponent : GH_OasysComponent, IGH_VariableParameterComponent
  {
    public GH_OasysDropDownComponent(string name, string nickname, string description, string category, string subCategory) : base(name, nickname, description, category, subCategory)
    {
    }

    internal List<List<string>> DropdownItems;
    internal List<string> SelectedItems;
    internal List<string> SpacerDescriptions;
    internal bool IsInitialised = false;

    #region UI
    public override void CreateAttributes()
    {
      if (!this.IsInitialised)
        this.InitialiseDropdowns();

      m_attributes = new UI.MultiDropDownComponentUI(this, this.SetSelected, this.DropdownItems, this.SelectedItems, this.SpacerDescriptions);
    }

    internal abstract void InitialiseDropdowns();

    internal abstract void SetSelected(int i, int j);

    internal virtual void UpdateUIFromSelectedItems()
    {
      this.CreateAttributes();
      this.UpdateUI();
    }

    internal virtual void UpdateUI()
    {
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      this.ExpireSolution(true);
      this.Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, this.DropdownItems, this.SelectedItems, this.SpacerDescriptions);
      return base.Write(writer);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref this.DropdownItems, ref this.SelectedItems, ref this.SpacerDescriptions);

      this.IsInitialised = true;
      this.UpdateUIFromSelectedItems();

      return base.Read(reader);
    }
    #endregion

    #region IGH_VariableParameterComponent null implementation
    public abstract void VariableParameterMaintenance();

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) => false;

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) => false;

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) => null;

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;
    #endregion
  }
}
