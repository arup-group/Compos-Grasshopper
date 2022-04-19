using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino;
using Grasshopper.Documentation;
using Rhino.Collections;
using UnitsNet;

namespace ComposGH.Parameters
{
    /// <summary>
    /// Custom class: this class defines the basic properties and methods for our custom class
    /// </summary>
    public class ComposWebOpening
    {
        public enum WebOpeningShape
        {
            Rectangular,
            Circular
        }
        public enum NotchPosition
        {
            Start,
            End
        }
        public enum OpeningType
        {
            Rectangular,
            Circular,
            Start_notch,
            End_notch
        }
        public OpeningType WebOpeningType { get; set; }
        public Length Width { get; set; }
        public Length Height { get; set; }
        public Length Diameter { get; set; }
        public Length CentroidPosFromStart { get; set; }
        public Length CentroidPosFromTop { get; set; }
        public WebOpeningStiffeners OpeningStiffeners { get; set; } = null;

        #region constructors
        public ComposWebOpening()
        {
            // empty constructor
        }
        /// <summary>
        /// Rectangular web opening
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="positionCentroidFromStart"></param>
        /// <param name="positionCentroidFromTop"></param>
        /// <param name="stiffeners"></param>
        public ComposWebOpening(Length width, Length height, Length positionCentroidFromStart, Length positionCentroidFromTop, WebOpeningStiffeners stiffeners = null)
        {
            // static type for this constructor
            this.WebOpeningType = OpeningType.Rectangular;
            // inputs
            this.Width = width;
            this.Height = height;
            this.CentroidPosFromStart = positionCentroidFromStart;
            this.CentroidPosFromTop = positionCentroidFromTop;
            this.OpeningStiffeners = stiffeners;
            // set stiffeners properties
            if (this.OpeningStiffeners != null)
            {
                this.OpeningStiffeners = new WebOpeningStiffeners(
                    stiffeners.DistanceFrom, stiffeners.TopStiffenerWidth, stiffeners.TopStiffenerThickness,
                    stiffeners.BottomStiffenerWidth, stiffeners.BottomStiffenerThickness, stiffeners.isBothSides);
            }
        }
        /// <summary>
        /// Circular web opening
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="positionCentroidFromStart"></param>
        /// <param name="positionCentroidFromTop"></param>
        /// <param name="stiffeners"></param>
        public ComposWebOpening(Length diameter, Length positionCentroidFromStart, Length positionCentroidFromTop, WebOpeningStiffeners stiffeners = null)
        {
            // static type for this constructor
            this.WebOpeningType = OpeningType.Circular;
            // inputs
            this.Diameter = diameter;
            this.CentroidPosFromStart = positionCentroidFromStart;
            this.CentroidPosFromTop = positionCentroidFromTop;
            this.OpeningStiffeners = stiffeners;
            // set stiffeners properties
            if (this.OpeningStiffeners != null)
            {
                this.OpeningStiffeners = new WebOpeningStiffeners(
                    stiffeners.DistanceFrom, stiffeners.TopStiffenerWidth, stiffeners.TopStiffenerThickness,
                    stiffeners.BottomStiffenerWidth, stiffeners.BottomStiffenerThickness, stiffeners.isBothSides);
            }
        }
        /// <summary>
        /// Notch web opening
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="positionCentroidFromStart"></param>
        /// <param name="positionCentroidFromTop"></param>
        /// <param name="stiffeners"></param>
        public ComposWebOpening(Length width, Length height, NotchPosition position, WebOpeningStiffeners stiffeners = null)
        {
            // static type for this constructor
            if (position == NotchPosition.Start)
                this.WebOpeningType = OpeningType.Start_notch;
            else
                this.WebOpeningType = OpeningType.End_notch;
            // inputs
            this.Width = width;
            this.Height = height;
            this.OpeningStiffeners = stiffeners;
            // set stiffeners properties
            if (this.OpeningStiffeners != null)
            {
                this.OpeningStiffeners = new WebOpeningStiffeners(
                    stiffeners.DistanceFrom, stiffeners.TopStiffenerWidth, 
                    stiffeners.TopStiffenerThickness, stiffeners.isBothSides);
            }
        }

        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region coa interop
        internal ComposWebOpening(string coaString)
        {
            // to do - implement from coa string method
        }

        internal string ToCoaString()
        {
            // to do - implement to coa string method
            return string.Empty;
        }
        #endregion

        #region methods

        public ComposWebOpening Duplicate()
        {
            if (this == null) { return null; }
            ComposWebOpening dup = (ComposWebOpening)this.MemberwiseClone();
            return dup;
        }

        public override string ToString()
        {
            string size = "";
            switch (this.WebOpeningType)
            {
                case OpeningType.Start_notch:
                case OpeningType.End_notch:
                case OpeningType.Rectangular:
                    size = this.Width.As(Units.LengthUnitGeometry).ToString("f0") + "x" + this.Height.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
                    break;
                case OpeningType.Circular:
                    size = "Ø" + this.Diameter.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
                    break;
            }
            
            string typ = "";
            switch (this.WebOpeningType)
            {
                case OpeningType.Start_notch:
                    typ = " Start Notch";
                    break;
                case OpeningType.End_notch:
                    typ = " End Notch";
                    break;
                case OpeningType.Rectangular:
                case OpeningType.Circular:
                    typ = ", Pos:(x:" + this.CentroidPosFromStart.As(Units.LengthUnitGeometry).ToString("f0") + ", z:" + this.CentroidPosFromTop.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty) + ")";
                    break;
            }
            if (this.OpeningStiffeners != null)
                typ += " w/Stiff.";

            return size + typ;
        }

        #endregion
    }

    /// <summary>
    /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
    /// </summary>
    public class ComposWebOpeningGoo : GH_Goo<ComposWebOpening>
    {
        #region constructors
        public ComposWebOpeningGoo()
        {
            this.Value = new ComposWebOpening();
        }
        public ComposWebOpeningGoo(ComposWebOpening item)
        {
            if (item == null)
                item = new ComposWebOpening();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public ComposWebOpeningGoo DuplicateGoo()
        {
            return new ComposWebOpeningGoo(Value == null ? new ComposWebOpening() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid => true;
        public override string TypeName => "Web Opening";
        public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
        public override string IsValidWhyNot
        {
            get
            {
                if (Value.IsValid) { return string.Empty; }
                return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null";
            else
                return "Compos " + TypeName + " {" + Value.ToString() + "}"; ;
        }
        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of our custom class into some other type Q.            

            if (typeof(Q).IsAssignableFrom(typeof(ComposWebOpening)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value;
                return true;
            }

            target = default;
            return false;
        }
        public override bool CastFrom(object source)
        {
            // This function is called when Grasshopper needs to convert other data 
            // into our custom class.

            if (source == null) { return false; }

            //Cast from GsaMaterial
            if (typeof(ComposWebOpening).IsAssignableFrom(source.GetType()))
            {
                Value = (ComposWebOpening)source;
                return true;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the CustomGoo type.
    /// </summary>

    public class ComposWebOpeningParameter : GH_PersistentParam<ComposWebOpeningGoo>
    {
        public ComposWebOpeningParameter()
          : base(new GH_InstanceDescription("WebOpening", "WO", "Compos Web Opening", ComposGH.Components.Ribbon.CategoryName.Name(), ComposGH.Components.Ribbon.SubCategoryName.Cat10()))
        {
        }

        public override Guid ComponentGuid => new Guid("eb70e868-29d9-4fae-9ef7-c465f3762a43");

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        //protected override System.Drawing.Bitmap Icon => ComposGH.Properties.Resources.SteelMaterialParam;

        protected override GH_GetterResult Prompt_Plural(ref List<ComposWebOpeningGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref ComposWebOpeningGoo value)
        {
            return GH_GetterResult.cancel;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };
            return item;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };
            return item;
        }

        #region preview methods

        public bool Hidden
        {
            get { return true; }
            //set { m_hidden = value; }
        }
        public bool IsPreviewCapable
        {
            get { return false; }
        }
        #endregion
    }

}
