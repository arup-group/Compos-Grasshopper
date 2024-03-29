﻿using System.Collections.Generic;
using OasysUnits;

namespace ComposAPI {
  public interface ITransverseRebarResult {
    /// <summary>
    /// Rebar area per meter. Values provided applies between corresponding <see cref="StartPosition"/> and <see cref="EndPosition"/>
    /// </summary>
    List<Area> Area { get; }
    /// <summary>
    /// Concrete shear resistance at <see cref="Positions"/>
    /// </summary>
    List<Force> ConcreteShearResistance { get; }
    /// <summary>
    /// Failure surface at <see cref="Positions"/>, being either a-a section, b-b section, or e-e section
    /// </summary>
    List<string> ControlSurface { get; }
    /// <summary>
    /// Rebar cover. Values provided applies between corresponding <see cref="StartPosition"/> and <see cref="EndPosition"/>
    /// </summary>
    List<Length> Cover { get; }
    /// <summary>
    /// Decking shear resistance at <see cref="Positions"/>
    /// </summary>
    List<Force> DeckingShearResistance { get; }
    /// <summary>
    /// Rebar diameter. Values provided applies between corresponding <see cref="StartPosition"/> and <see cref="EndPosition"/>
    /// </summary>
    List<Length> Diameter { get; }
    /// <summary>
    /// Effective perimeter at <see cref="Positions"/>
    /// </summary>
    List<Length> EffectiveShearPerimeter { get; }
    /// <summary>
    /// Rebar ending position, measured from start
    /// </summary>
    List<Length> EndPosition { get; }
    /// <summary>
    /// Maximum allowable shear resistance at <see cref="Positions"/>
    /// </summary>
    List<Force> MaxAllowedShearResistance { get; }
    /// <summary>
    /// Mesh reinforcement bar shear resistance at <see cref="Positions"/>
    /// </summary>
    List<Force> MeshBarShearResistance { get; }
    /// <summary>
    /// Critical transverse shear position
    /// </summary>
    List<Length> Positions { get; }
    /// <summary>
    /// Rebar shear resistance at <see cref="Positions"/>
    /// </summary>
    List<Force> RebarShearResistance { get; }
    /// <summary>
    /// Rebar interval. Values provided applies between corresponding <see cref="StartPosition"/> and <see cref="EndPosition"/>
    /// </summary>
    List<Length> Spacing { get; }
    /// <summary>
    /// Rebar starting position, measured from start
    /// </summary>
    List<Length> StartPosition { get; }
    /// <summary>
    /// Combined shear resistance [Concrete] + [Decking] + [Mesh bar] + [Rebar] at <see cref="Positions"/>
    /// </summary>
    List<Force> TotalShearResistance { get; }
    /// <summary>
    /// Actual transverse shear force at <see cref="Positions"/>
    /// </summary>
    List<Force> TransverseShearForce { get; }
  }
}
