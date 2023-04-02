using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.PlanetModules
{
    [Serializable]
    public class CloakModule : PlanetModule
    {
        [Tooltip("Radius of the cloaking field around the planet. It's a bit finicky so experiment with different values.")]
        public float Radius;
        [Tooltip("The AudioClip that will play when entering the cloaking field.")]
        public AudioClip Audio;
        [Tooltip("The AudioType that will play when entering the cloaking field, if not using a custom audio clip.")]
        [ConditionalField(nameof(Audio), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType AudioType;

        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            if (Audio)
                writer.WriteProperty("audio", planet.GetResourcePath(Audio));
            else if (AudioType != AudioType.None)
                writer.WriteProperty("audio", AudioType, false);
        }

        public override IEnumerable<AssetResource> GetResources(PlanetAsset planet)
        {
            if (Audio)
                yield return new AudioResource(Audio, planet);
        }
    }
}
