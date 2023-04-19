using ModDataTools.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets
{
    [CreateAssetMenu(menuName = ASSET_MENU_PREFIX + nameof(FrequencyAsset))]
    public class FrequencyAsset : DataAsset
    {
        [Tooltip("The mod this frequency belongs in")]
        public ModManifestAsset Mod;

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Mod) yield return Mod;
        }

        public override void Localize(Localization l10n)
        {
            l10n.AddUI(FullID, FullName);
        }
    }
}
