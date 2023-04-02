using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Resources
{
    [Serializable]
    public class AudioResource : AssetResource
    {
        public AudioClip Audio;

        public AudioResource() { }
        public AudioResource(AudioClip audio, StarSystemAsset starSystem) : base(audio, starSystem) { Audio = audio; }
        public AudioResource(AudioClip audio, PlanetAsset planet) : base(audio, planet) { Audio = audio; }
        public AudioResource(AudioClip audio, string outputPath) : base(audio, outputPath) { Audio = audio; }

        public override UnityEngine.Object GetResource() => Audio;
    }
}
