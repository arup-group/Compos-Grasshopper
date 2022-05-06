﻿using UnitsNet;

namespace ComposAPI
{
  public interface IMeshReinforcement
  {
    Length Cover { get; }
    bool Rotated { get; }
    ReinforcementMeshType MeshType { get; }

    string ToCoaString(string name);
  }
}