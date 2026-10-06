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
    public class ItemConfig
    {
        [Tooltip("The name of the item to be displayed in the UI. Defaults to the name of the detail object.")]
        public string Name;
        [Tooltip("The type of the item, which determines its orientation when held and what sockets it fits into. This can be a custom string, or a vanilla ItemType (Scroll, SharedStone, ConversationStone, Lantern, SlideReel, DreamLantern, or VisionTorch). Defaults to the item name. Note: This cannot be the base game WarpCore item type as those have complicated custom behaviour.")]
        public string ItemType;
        [Tooltip("The furthest distance where the player can interact with this item. Defaults to two meters, same as most vanilla items. Set this to zero to disable all interaction by default.")]
        public float InteractRange = 2f;
        [Tooltip("The radius that the added sphere collider will use for collision and hover detection. If there's already a collider on the detail, you can make this 0.")]
        public float ColliderRadius = 0.5f;
        [Tooltip("Whether the added sphere collider will be a trigger (interactible but does not collide).")]
        public bool ColliderIsTrigger = true;
        [Tooltip("Whether the item can be dropped.")]
        public bool Droppable = true;
        [Tooltip("A relative offset to apply to the item's position when dropping it on the ground.")]
        public NullishVector3 DropOffset;
        [Tooltip("The direction the item will be oriented when dropping it on the ground. Defaults to up (0, 1, 0).")]
        public NullishVector3 DropNormal;
        [Tooltip("A relative offset to apply to the item's position when holding it. The initial position varies for vanilla item types.")]
        public NullishVector3 HoldOffset;
        [Tooltip("A relative offset to apply to the item's rotation when holding it.")]
        public NullishVector3 HoldRotation;
        [Tooltip("A relative offset to apply to the item's position when placing it into a socket.")]
        public NullishVector3 SocketOffset;
        [Tooltip("A relative offset to apply to the item's rotation when placing it into a socket.")]
        public NullishVector3 SocketRotation;
        [Tooltip("The audio to play when this item is picked up. Only applies to custom/non-vanilla item types. Defaults to ToolItemWarpCorePickUp.")]
        public AudioClip PickupAudio;
        [Tooltip("The audio to play when this item is picked up, if not using a custom audio clip.")]
        [ConditionalField(nameof(PickupAudio), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType PickupAudioType;
        [Tooltip("The audio to play when this item is dropped. Only applies to custom/non-vanilla item types. Defaults to ToolItemWarpCoreDrop.")]
        public AudioClip DropAudio;
        [Tooltip("The audio to play when this item is dropped, if not using a custom audio clip.")]
        [ConditionalField(nameof(DropAudio), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType DropAudioType;
        [Tooltip("The audio to play when this item is inserted into a socket. Only applies to custom/non-vanilla item types. Defaults to the pickup audio.")]
        public AudioClip SocketAudio;
        [Tooltip("The audio to play when this item is inserted into a socket, if not using a custom audio clip.")]
        [ConditionalField(nameof(SocketAudio), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType SocketAudioType;
        [Tooltip("The audio to play when this item is removed from a socket. Only applies to custom/non-vanilla item types. Defaults to the drop audio.")]
        public AudioClip UnsocketAudio;
        [Tooltip("The audio to play when this item is removed from a socket, if not using a custom audio clip.")]
        [ConditionalField(nameof(UnsocketAudio), (AudioClip)null)]
        [EnumValuePicker]
        public AudioType UnsocketAudioType;
        [Tooltip("A dialogue condition to set when picking up this item.")]
        public ConditionAsset PickupCondition;
        [Tooltip("Whether the pickup condition should be cleared when dropping the item.")]
        [ConditionalField(nameof(PickupCondition))]
        public bool ClearPickupConditionOnDrop = true;
        [Tooltip("A ship log fact to reveal when picking up this item.")]
        public FactAsset PickupFact;
        [Tooltip("A detail with an item socket that this item will be automatically inserted into.")]
        public DetailPropAsset InitialSocketAsset;
        [Tooltip("A detail with an item socket that this item will be automatically inserted into.")]
        public DetailPropComponent InitialSocket;

        public IProp GetInitialSocket() => InitialSocketAsset ? (IProp)InitialSocketAsset : InitialSocket ? InitialSocket : null;

        public string GetNameKey(PropContext context) => $"{context.GetProp().PropID}_ITEM";

        public void ToJson(PropContext context, JsonTextWriter writer)
        {
            writer.WriteStartObject();
            if (!string.IsNullOrEmpty(Name))
                writer.WriteProperty("name", GetNameKey(context));
            if (!string.IsNullOrEmpty(ItemType))
                writer.WriteProperty("itemType", ItemType);
            if (InteractRange != 2f)
                writer.WriteProperty("interactRange", InteractRange);
            if (ColliderRadius != 0.5f)
                writer.WriteProperty("colliderRadius", ColliderRadius);
            if (!ColliderIsTrigger)
                writer.WriteProperty("colliderIsTrigger", ColliderIsTrigger);
            if (!Droppable)
                writer.WriteProperty("droppable", Droppable);
            writer.WriteProperty("dropOffset", DropOffset);
            writer.WriteProperty("dropNormal", DropNormal);
            writer.WriteProperty("holdOffset", HoldOffset);
            writer.WriteProperty("holdRotation", HoldRotation);
            writer.WriteProperty("socketOffset", SocketOffset);
            writer.WriteProperty("socketRotation", SocketRotation);
            WriteAudio(context, writer, "pickupAudio", PickupAudio, PickupAudioType);
            WriteAudio(context, writer, "dropAudio", DropAudio, DropAudioType);
            WriteAudio(context, writer, "socketAudio", SocketAudio, SocketAudioType);
            WriteAudio(context, writer, "unsocketAudio", UnsocketAudio, UnsocketAudioType);
            if (PickupCondition)
            {
                writer.WriteProperty("pickupCondition", PickupCondition.FullID);
                if (!ClearPickupConditionOnDrop)
                    writer.WriteProperty("clearPickupConditionOnDrop", ClearPickupConditionOnDrop);
            }
            if (PickupFact)
                writer.WriteProperty("pickupFact", PickupFact.FullID);
            var initialSocket = GetInitialSocket();
            if (initialSocket != null)
            {
                var socketPath = AssetRepository.GetPropPlanetPath<DetailPropData>(context.Planet, initialSocket);
                if (socketPath != null)
                    writer.WriteProperty("pathToInitialSocket", socketPath);
            }
            writer.WriteEndObject();
        }

        void WriteAudio(PropContext context, JsonTextWriter writer, string name, AudioClip clip, AudioType type)
        {
            if (clip)
                writer.WriteProperty(name, context.Planet.GetResourcePath(clip));
            else if (type != AudioType.None)
                writer.WriteProperty(name, type, false);
        }

        public void Localize(PropContext context, Localization l10n)
        {
            if (!string.IsNullOrEmpty(Name))
                l10n.AddUI(GetNameKey(context), Name);
        }

        public void Validate(PropContext context, IAssetValidator validator)
        {
            if (PickupCondition && PickupCondition.Persistent)
                validator.Error(context.Planet, $"{nameof(PickupCondition)} must not be a persistent condition");
            var initialSocket = GetInitialSocket();
            if (initialSocket != null)
            {
                var socketContext = AssetRepository.GetPropContext<DetailPropData>(context.Planet, initialSocket);
                if (socketContext == null)
                    validator.Error(context.Planet, $"Item '{context.GetProp().PropName}' has an initial socket '{initialSocket.PropName}' that is not on the same planet");
                else if (!socketContext.Data.IsItemSocket)
                    validator.Error(context.Planet, $"Item '{context.GetProp().PropName}' has an initial socket '{initialSocket.PropName}' that is not an item socket");
            }
        }

        public IEnumerable<AssetResource> GetResources(PropContext context)
        {
            if (PickupAudio)
                yield return new AudioResource(PickupAudio, context.Planet);
            if (DropAudio)
                yield return new AudioResource(DropAudio, context.Planet);
            if (SocketAudio)
                yield return new AudioResource(SocketAudio, context.Planet);
            if (UnsocketAudio)
                yield return new AudioResource(UnsocketAudio, context.Planet);
        }
    }

    [Serializable]
    public class ItemSocketConfig
    {
        [Tooltip("The type of item allowed in this socket. This can be a custom string, or a vanilla ItemType (Scroll, WarpCode, SharedStone, ConversationStone, Lantern, SlideReel, DreamLantern, or VisionTorch).")]
        public string ItemType;
        [Tooltip("A child of the detail's prefab that will act as the socket point for the item.")]
        public Transform SocketPoint;
        [Tooltip("The relative path to a child game object of this detail that will act as the socket point for the item.")]
        [ConditionalField(nameof(SocketPoint), (Transform)null)]
        public string SocketPath;
        [Tooltip("The position of the socket point relative to the detail, if no socket point is set.")]
        [ConditionalField(nameof(SocketPoint), (Transform)null)]
        public Vector3 Position;
        [Tooltip("The rotation of the socket point relative to the detail, if no socket point is set.")]
        [ConditionalField(nameof(SocketPoint), (Transform)null)]
        public Vector3 Rotation;
        [Tooltip("The furthest distance where the player can interact with this item socket. Defaults to two meters, same as most vanilla item sockets. Set this to zero to disable all interaction by default.")]
        public float InteractRange = 2f;
        [Tooltip("Default collider radius when interacting with the socket")]
        public float ColliderRadius;
        [Tooltip("Whether the added sphere collider will be a trigger (interactible but does not collide).")]
        public bool ColliderIsTrigger = true;
        [Tooltip("Whether to use \"Give Item\" / \"Take Item\" prompts instead of \"Insert Item\" / \"Remove Item\".")]
        public bool UseGiveTakePrompts;
        [Tooltip("A dialogue condition to set when inserting an item into this socket.")]
        public ConditionAsset InsertCondition;
        [Tooltip("Whether the insert condition should be cleared when removing the socketed item.")]
        [ConditionalField(nameof(InsertCondition))]
        public bool ClearInsertConditionOnRemoval = true;
        [Tooltip("A ship log fact to reveal when inserting an item into this socket.")]
        public FactAsset InsertFact;
        [Tooltip("A dialogue condition to set when removing an item from this socket, or when the socket is empty.")]
        public ConditionAsset RemovalCondition;
        [Tooltip("Whether the removal condition should be cleared when inserting a socketed item.")]
        [ConditionalField(nameof(RemovalCondition))]
        public bool ClearRemovalConditionOnInsert = true;
        [Tooltip("A ship log fact to reveal when removing an item from this socket, or when the socket is empty.")]
        public FactAsset RemovalFact;

        public void ToJson(PropContext context, JsonTextWriter writer)
        {
            writer.WriteStartObject();
            if (!string.IsNullOrEmpty(ItemType))
                writer.WriteProperty("itemType", ItemType);
            if (SocketPoint)
                writer.WriteProperty("socketPath", UnityUtility.GetTransformPath(SocketPoint, true));
            else if (!string.IsNullOrEmpty(SocketPath))
                writer.WriteProperty("socketPath", SocketPath);
            else
            {
                writer.WriteProperty("isRelativeToParent", true);
                writer.WriteProperty("position", Position);
                writer.WriteProperty("rotation", Rotation);
            }
            if (InteractRange != 2f)
                writer.WriteProperty("interactRange", InteractRange);
            if (ColliderRadius != 0f)
                writer.WriteProperty("colliderRadius", ColliderRadius);
            if (!ColliderIsTrigger)
                writer.WriteProperty("colliderIsTrigger", ColliderIsTrigger);
            if (UseGiveTakePrompts)
                writer.WriteProperty("useGiveTakePrompts", UseGiveTakePrompts);
            if (InsertCondition)
            {
                writer.WriteProperty("insertCondition", InsertCondition.FullID);
                if (!ClearInsertConditionOnRemoval)
                    writer.WriteProperty("clearInsertConditionOnRemoval", ClearInsertConditionOnRemoval);
            }
            if (InsertFact)
                writer.WriteProperty("insertFact", InsertFact.FullID);
            if (RemovalCondition)
            {
                writer.WriteProperty("removalCondition", RemovalCondition.FullID);
                if (!ClearRemovalConditionOnInsert)
                    writer.WriteProperty("clearRemovalConditionOnInsert", ClearRemovalConditionOnInsert);
            }
            if (RemovalFact)
                writer.WriteProperty("removalFact", RemovalFact.FullID);
            writer.WriteEndObject();
        }

        public void Validate(PropContext context, IAssetValidator validator)
        {
            if (InsertCondition && InsertCondition.Persistent)
                validator.Error(context.Planet, $"{nameof(InsertCondition)} must not be a persistent condition");
            if (RemovalCondition && RemovalCondition.Persistent)
                validator.Error(context.Planet, $"{nameof(RemovalCondition)} must not be a persistent condition");
        }
    }
}
