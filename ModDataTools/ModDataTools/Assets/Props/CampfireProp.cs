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
    public class CampfirePropData : GeneralPropData
    {
        [Tooltip("The initial state of the campfire.")]
        public CampfireState InitialState = CampfireState.Lit;
        [Tooltip("Whether the player can sleep at this campfire.")]
        public bool CanSleepHere = true;
        [Tooltip("Whether the player should look up at the sky while sleeping at this campfire.")]
        [ConditionalField(nameof(CanSleepHere))]
        public bool LookUpWhileSleeping;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (InitialState != CampfireState.Lit)
                writer.WriteProperty("initialState", InitialState);
            if (!CanSleepHere)
                writer.WriteProperty("canSleepHere", CanSleepHere);
            else if (LookUpWhileSleeping)
                writer.WriteProperty("lookUpWhileSleeping", LookUpWhileSleeping);
        }

        public enum CampfireState
        {
            Unlit = 0,
            Smoldering = 1,
            Lit = 2,
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(CampfirePropAsset))]
    public class CampfirePropAsset : GeneralPropAsset<CampfirePropData> { }
    public class CampfirePropComponent : GeneralPropComponent<CampfirePropData> { }
}
