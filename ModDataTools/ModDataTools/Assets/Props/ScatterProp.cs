using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class ScatterPropData : PropData
    {
        [Tooltip("The prefab to spawn, if spawning a custom object")]
        public GameObject Prefab;
        [Tooltip("The path in the scene hierarchy of the item to copy")]
        [ConditionalField(nameof(Prefab), (GameObject)null)]
        public string Path;
        [Tooltip("Number of props to scatter")]
        public int Count;
        [Tooltip("Offset this prop once it is placed")]
        public Vector3 Offset;
        [Tooltip("Rotate this prop once it is placed")]
        public Vector3 Rotation;
        [Tooltip("Scale the prop once it is placed")]
        public float Scale = 1f;
        [Tooltip("Scale each axis of the prop. Multiplied with scale.")]
        public Vector3 Stretch = Vector3.one;
        [Tooltip("The number used as entropy for scattering the props")]
        public int Seed;
        [Tooltip("The lowest height that these object will be placed at (only relevant if there's a heightmap)")]
        public NullishSingle MinHeight;
        [Tooltip("The highest height that these objects will be placed at (only relevant if there's a heightmap)")]
        public NullishSingle MaxHeight;
        [Tooltip("Should we try to prevent overlap between the scattered details? True by default. If it's affecting load times turn it off.")]
        public bool PreventOverlap = true;
        [Tooltip("Should this detail stay loaded even if you're outside the sector (good for very large props)")]
        public bool KeepLoaded;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (Prefab)
            {
                writer.WriteProperty("assetBundle", AssetRepository.GetAssetBundlePath(Prefab));
                writer.WriteProperty("path", AssetRepository.GetAssetPath(Prefab));
            }
            else
            {
                writer.WriteProperty("path", Path);
            }
            writer.WriteProperty("count", Count);
            if (Offset != Vector3.zero)
                writer.WriteProperty("offset", Offset);
            if (Rotation != Vector3.zero)
                writer.WriteProperty("rotation", Rotation);
            if (Stretch == Vector3.one)
                writer.WriteProperty("scale", Scale);
            else
                writer.WriteProperty("stretch", Stretch * Scale);
            if (Seed != 0)
                writer.WriteProperty("seed", Seed);
            writer.WriteProperty("minHeight", MinHeight);
            writer.WriteProperty("maxHeight", MaxHeight);
            if (!PreventOverlap)
                writer.WriteProperty("preventOverlap", PreventOverlap);
            if (KeepLoaded)
                writer.WriteProperty("keepLoaded", KeepLoaded);
        }

        public override IEnumerable<AssetResource> GetResources(PropContext context)
        {
            if (Prefab)
                yield return new PrefabResource(Prefab, string.Empty);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(ScatterPropAsset))]
    public class ScatterPropAsset : PropDataAsset<ScatterPropData> {
        public override string GetPlanetPath(PropContext context) => context.DetailPath;
    }
    public class ScatterPropComponent : PropDataComponent<ScatterPropData>
    {
        public override string GetPlanetPath(PropContext context) => context.DetailPath;
    }
}
