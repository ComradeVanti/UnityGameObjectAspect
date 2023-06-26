using System;
using UnityEngine;

namespace Dev.ComradeVanti.GameObjectAspect
{
    public interface IGameObjectAspect : IEquatable<IGameObjectAspect>
    {
        public GameObject GameObject { get; }


        bool IEquatable<IGameObjectAspect>.Equals(IGameObjectAspect other) =>
            other != null && other.GameObject == GameObject;
    }
}