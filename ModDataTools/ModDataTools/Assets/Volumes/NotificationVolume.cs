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
                writer.WriteProperty("entryNotification", EntryNotification);
            if (!string.IsNullOrEmpty(ExitNotification.DisplayMessage))
                writer.WriteProperty("exitNotification", ExitNotification);
        }

        public enum NotificationTarget
        {
            All = 0,
            Ship = 1,
            Player = 2,
        }

        [Serializable]
        public class Notification : IJsonSerializable
        {
            [Tooltip("The message that will be displayed.")]
            public string DisplayMessage;
            [Tooltip("The duration this notification will be displayed.")]
            public float Duration = 5f;

            public void ToJson(JsonTextWriter writer)
            {
                writer.WriteStartObject();
                writer.WriteProperty("displayMessage", DisplayMessage);
                if (Duration != 5f)
                    writer.WriteProperty("duration", Duration);
                writer.WriteEndObject();
            }
        }
    }

    [CreateAssetMenu(menuName = VOLUME_MENU_PREFIX + nameof(NotificationVolumeAsset))]
    public class NotificationVolumeAsset : GeneralVolumeAsset<NotificationVolumeData> { }
    public class NotificationVolumeComponent : GeneralVolumeComponent<NotificationVolumeData> { }
}
