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
    public class WebOpeningStiffener
    {
        public Length DistanceFrom { get; set; }
        public Length TopStiffenerWidth { get; set; }
        public Length TopStiffenerThickness { get; set; }
        public Length BottomStiffenerWidth { get; set; }
        public Length BottomStiffenerThickness { get; set; }
        public bool BothSides { get; set; }


        #region constructors
        public WebOpeningStiffener()
        {
            // empty constructor
        }
        
        // add constructors here

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

        public WebOpeningStiffener Duplicate()
        {
            if (this == null) { return null; }
            WebOpeningStiffener dup = (WebOpeningStiffener)this.MemberwiseClone();
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
    public class WebOpeningStiffenerGoo : GH_Goo<WebOpeningStiffener>
    {
        #region constructors
        public WebOpeningStiffenerGoo()
        {
            this.Value = new WebOpeningStiffener();
        }
        public WebOpeningStiffenerGoo(WebOpeningStiffener item)
        {
            if (item == null)
                item = new WebOpeningStiffener();
            this.Value = item.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGoo();
        }
        public WebOpeningStiffenerGoo DuplicateGoo()
        {
            return new WebOpeningStiffenerGoo(Value == null ? new WebOpeningStiffener() : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid => true;
        public override string TypeName => "Web Opening Stiffener";
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

            if (typeof(Q).IsAssignableFrom(typeof(WebOpeningStiffener)))
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
            if (typeof(WebOpeningStiffener).IsAssignableFrom(source.GetType()))
            {
                Value = (WebOpeningStiffener)source;
                return true;
            }

            return false;
        }
        #endregion
    }
}
