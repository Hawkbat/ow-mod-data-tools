using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets
{
    [Serializable]
    public class AudioConfig
    {
        [Tooltip("A custom audio clip to play")]
        public AudioClip Clip;
        [Tooltip("A base game audio type to play, if not using a custom audio clip")]
        public AudioType Type;

        public bool HasValue => Clip || Type != AudioType.None;

        public IEnumerable<AssetResource> GetResources(Func<UnityEngine.Object, string> getClipPath)
        {
            if (Clip)
                yield return new AudioResource(Clip, getClipPath(Clip));
        }
    }
}
