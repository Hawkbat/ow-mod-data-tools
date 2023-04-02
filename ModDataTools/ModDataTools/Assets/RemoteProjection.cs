using ModDataTools.Assets.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets
{
    [CreateAssetMenu(menuName = ASSET_MENU_PREFIX + nameof(RemoteProjectionAsset))]
    public class RemoteProjectionAsset : DataAsset
    {
        [Tooltip("The star system this remote projection belongs in")]
        public StarSystemAsset StarSystem;
        [Header("Data")]
        [Tooltip("Icon that will show on the stone, pedastal of the whiteboard, and pedastal of the platform.")]
        public Texture2D Decal;

        public override IEnumerable<AssetResource> GetResources()
        {
            if (Decal)
                yield return new ImageResource(Decal, StarSystem);
        }
    }
}
