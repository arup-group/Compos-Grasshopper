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
    public class WebOpeningsSpec
    {
        public enum Opening
        {
            Rectangular,
            Notch_Left,
            Notch_Right,
            Circular
        }

        public enum Stiffeners
        {
            Without_Stiffeners,
            With_Stiffeners
        }

        public Opening Opening_Type { get; set; }
        public Stiffeners Stiffener_Type { get; set; }

        #region constructors
        public WebOpeningsSpec()
        {
            this.Opening_Type = Opening.Circular;
            this.Stiffener_Type = Stiffeners.Without_Stiffeners;
        }
        
        public WebOpeningsSpec(Opening openingType = Opening.Circular, Stiffeners stiffenerType = Stiffeners.With_Stiffeners)
        {
            this.Opening_Type = openingType;
            this.Stiffener_Type = stiffenerType;
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

        #region methods

        public WebOpeningsSpec Duplicate()
        {
            if (this == null) { return null; }
            WebOpeningsSpec dup = (WebOpeningsSpec)this.MemberwiseClone();
            return dup;
        }
        public override string ToString()
        {
            // to do: make sensible to-string method
            return "Web Opening Stiffener";
        }
        #endregion
    }

    /// <summary>
    /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
    /// </summary>
    public class WebOpeningsGoo : GH_Goo<WebOpeningsSpec>
    {
        #region constructors
        public WebOpeningsGoo()
        {
            this.Value = new WebOpeningsSpec();
        }
        public WebOpeningsGoo(WebOpeningsSpec item)
        {
            if (item == null)
                item = new WebOpeningsSpec();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public WebOpeningsGoo DuplicateGoo()
        {
            return new WebOpeningsGoo(Value == null ? new WebOpeningsSpec() : Value.Duplicate());
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

            if (typeof(Q).IsAssignableFrom(typeof(WebOpeningsSpec)))
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
            if (typeof(WebOpeningsSpec).IsAssignableFrom(source.GetType()))
            {
                Value = (WebOpeningsSpec)source;
                return true;
            }

            return false;
        }
        #endregion
    }
}
