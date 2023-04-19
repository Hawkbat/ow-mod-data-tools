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
    public class NotificationVolumeData : GeneralVolumeData
    {
        [Tooltip("What the notification will show for.")]
        public NotificationTarget Target;
        [Tooltip("The notification that will play when you enter this volume.")]
        public Notification EntryNotification;
        [Tooltip("The notification that will play when you exit this volume.")]
        public Notification ExitNotification;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            base.WriteJsonProps(context, writer);
            if (Target != NotificationTarget.All)
                writer.WriteProperty("target", Target);
            if (!string.IsNullOrEmpty(EntryNotification.DisplayMessage))
            {
                writer.WritePropertyName("entryNotification");
                writer.WriteStartObject();
                writer.WriteProperty("displayMessage", $"{context.GetProp().PropID}_ENTRY");
                if (EntryNotification.Duration != 5f)
                    writer.WriteProperty("duration", EntryNotification.Duration);
                writer.WriteEndObject();
            }
            if (!string.IsNullOrEmpty(ExitNotification.DisplayMessage))
            {
                writer.WritePropertyName("exitNotification");
                writer.WriteStartObject();
                writer.WriteProperty("displayMessage", $"{context.GetProp().PropID}_EXIT");
                if (ExitNotification.Duration != 5f)
                    writer.WriteProperty("duration", ExitNotification.Duration);
                writer.WriteEndObject();
            }
        }

        public override void Localize(PropContext context, Localization l10n)
        {
            if (!string.IsNullOrEmpty(EntryNotification.DisplayMessage))
                l10n.AddUI($"{context.GetProp().PropID}_ENTRY", EntryNotification.DisplayMessage);
            if (!string.IsNullOrEmpty(ExitNotification.DisplayMessage))
                l10n.AddUI($"{context.GetProp().PropID}_EXIT", ExitNotification.DisplayMessage);
        }

        public enum NotificationTarget
        {
            All = 0,
            Ship = 1,
            Player = 2,
        }

        [Serializable]
        public class Notification
        {
            [Tooltip("The message that will be displayed.")]
            public string DisplayMessage;
            [Tooltip("The duration this notification will be displayed.")]
            public float Duration = 5f;
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(NotificationVolumeAsset))]
    public class NotificationVolumeAsset : GeneralVolumeAsset<NotificationVolumeData> { }
    public class NotificationVolumeComponent : GeneralVolumeComponent<NotificationVolumeData> { }
}
