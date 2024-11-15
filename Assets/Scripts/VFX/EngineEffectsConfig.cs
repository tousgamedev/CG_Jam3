using System;
using UnityEngine;

namespace VFX
{
    [Serializable]
    public struct EngineEffectsConfig
    {
        public Color primaryColor;
        public Color secondaryColor;
        public Vector2 speedRange;
        public Vector2 sizeRange;
    }
}