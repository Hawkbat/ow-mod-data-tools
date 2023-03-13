using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets
{
    [CreateAssetMenu]
    public class Curiosity : EntryBase
    {
        [Tooltip("The color of the curiosity and associated entries")]
        [ColorUsage(false)]
        public Color Color = Color.white;

        public Color NormalColor
        {
            get
            {
                Color.RGBToHSV(Color, out float h, out float s, out float v);
                return Color.HSVToRGB(h, s, v * 0.7f);
            }
        }

        public Color HighlightColor => Color;

        public override Curiosity GetCuriosity() => this;
    }
}
