using ModDataTools.Assets.Props;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Volumes
{
    [Serializable]
    public class RevealVolumeData : GeneralVolumeData
    {
        [Tooltip("The max view angle (in degrees) the player can see the volume with to unlock the fact (observe only)")]
        [ConditionalField(nameof(RevealOn), RevealOnType.Observe)]
        public float MaxAngle = 180f;
        [Tooltip("The max distance the user can be away from the volume to reveal the fact (snapshot and observe only)")]
        [ConditionalField(nameof(RevealOn), RevealOnType.Snapshot, RevealOnType.Observe)]
        public NullishSingle MaxDistance;
        [Tooltip("What needs to be done to the volume to unlock the facts")]
        public RevealOnType RevealOn = RevealOnType.Enter;
        [Tooltip("What can enter the volume to unlock the facts (enter only)")]
        [ConditionalField(nameof(RevealOn), RevealOnType.Enter)]
        public RevealForType RevealFor = RevealForType.Both;
        [Tooltip("A list of facts to reveal")]
        public List<FactAsset> RevealFacts = new();
        [Tooltip("An achievement to unlock. Optional.")]
        public AchievementAsset Achievement;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (RevealOn == RevealOnType.Observe && MaxAngle != 180f)
                writer.WriteProperty("maxAngle", MaxAngle);
            if (RevealOn == RevealOnType.Snapshot || RevealOn == RevealOnType.Observe)
                writer.WriteProperty("maxDistance", MaxDistance);
            if (RevealOn != RevealOnType.Enter)
                writer.WriteProperty("revealOn", RevealOn);
            if (RevealOn == RevealOnType.Enter && RevealFor != RevealForType.Both)
                writer.WriteProperty("revealFor", RevealFor);
            if (RevealFacts.Any())
                writer.WriteProperty("reveals", RevealFacts.Select(f => f.FullID));
            if (Achievement)
                writer.WriteProperty("achievementID", Achievement.FullID);
        }

        public enum RevealOnType
        {
            Enter = 0,
            Observe = 1,
            Snapshot = 2,
        }

        public enum RevealForType
        {
            Both = 0,
            Player = 1,
            Probe = 2,
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(RevealVolumeAsset))]
    public class RevealVolumeAsset : GeneralVolumeAsset<RevealVolumeData> { }
    public class RevealVolumeComponent : GeneralVolumeComponent<RevealVolumeData> { }
}
