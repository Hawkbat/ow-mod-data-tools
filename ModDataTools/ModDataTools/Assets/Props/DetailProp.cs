using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class DetailPropData : GeneralScaleablePropData
    {
        [Tooltip("The prefab to spawn, if spawning a custom object")]
        public GameObject Prefab;
        [Tooltip("The path in the scene hierarchy of the item to copy")]
        [ConditionalField(nameof(Prefab), (GameObject)null)]
        public string Path;
        [Tooltip("A list of children to remove from this detail")]
        public List<string> RemoveChildren = new();
        [Tooltip("Do we reset all the components on this object? Useful for certain props that have dialogue components attached to\r\nthem.")]
        public bool RemoveComponents;
        [Tooltip("Should this detail stay loaded even if you're outside the sector (good for very large props)")]
        public bool KeepLoaded;
        [Tooltip("Should this object dynamically move around? This tries to make all mesh colliders convex, as well as adding a sphere collider in case the detail has no others.")]
        public bool HasPhysics;
        [Tooltip("The mass of the physics object. Most pushable props use the default value, which matches the player mass.")]
        [ConditionalField(nameof(HasPhysics))]
        public float PhysicsMass = 0.001f;
        [Tooltip("The radius that the added sphere collider will use for physics collision. If there's already good colliders on the detail, you can make this 0.")]
        [ConditionalField(nameof(HasPhysics))]
        public float PhysicsRadius = 1f;
        [Tooltip("If true, this detail will stay still until it touches something. Good for zero-g props.")]
        [ConditionalField(nameof(HasPhysics))]
        public bool PhysicsSuspendUntilImpact;
        [Tooltip("Activates this game object when the dialogue/persistent condition is met")]
        public ConditionAsset ActivationCondition;
        [Tooltip("Deactivates this game object when the dialogue/persistent condition is met")]
        public ConditionAsset DeactivationCondition;
        [Tooltip("Should the player close their eyes while the activation state changes. Only relevant if an activation or deactivation condition is set.")]
        public bool BlinkWhenActiveChanged = true;
        [Tooltip("Should this detail be treated as an interactible item")]
        public bool IsItem;
        [Tooltip("The interactible item settings for this detail")]
        [ConditionalField(nameof(IsItem))]
        public ItemConfig Item;
        [Tooltip("Should this detail be treated as a socket for an interactible item")]
        public bool IsItemSocket;
        [Tooltip("The item socket settings for this detail")]
        [ConditionalField(nameof(IsItemSocket))]
        public ItemSocketConfig ItemSocket;
        [Tooltip("Set to true if this object's lighting should ignore the effects of sunlight")]
        public bool IgnoreSun;
        [Tooltip("Whether this detail will only be shown from 50km away. Meant to be lower resolution.")]
        public bool IsProxyDetail;

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
            if (RemoveChildren.Any())
                writer.WriteProperty("removeChildren", RemoveChildren);
            if (RemoveComponents)
                writer.WriteProperty("removeComponents", RemoveComponents);
            if (KeepLoaded)
                writer.WriteProperty("keepLoaded", KeepLoaded);
            if (HasPhysics)
            {
                writer.WriteProperty("hasPhysics", HasPhysics);
                writer.WriteProperty("physicsMass", PhysicsMass);
                writer.WriteProperty("physicsRadius", PhysicsRadius);
                if (PhysicsSuspendUntilImpact)
                    writer.WriteProperty("physicsSuspendUntilImpact", PhysicsSuspendUntilImpact);
            }
            if (IgnoreSun)
                writer.WriteProperty("ignoreSun", IgnoreSun);
            if (ActivationCondition)
                writer.WriteProperty("activationCondition", ActivationCondition.FullID);
            if (DeactivationCondition)
                writer.WriteProperty("deactivationCondition", DeactivationCondition.FullID);
            if ((ActivationCondition || DeactivationCondition) && !BlinkWhenActiveChanged)
                writer.WriteProperty("blinkWhenActiveChanged", BlinkWhenActiveChanged);
            if (IsItem)
            {
                writer.WritePropertyName("item");
                Item.ToJson(context, writer);
            }
            if (IsItemSocket)
            {
                writer.WritePropertyName("itemSocket");
                ItemSocket.ToJson(context, writer);
            }
        }

        public override void Localize(PropContext context, Localization l10n)
        {
            base.Localize(context, l10n);
            if (IsItem)
                Item.Localize(context, l10n);
        }

        public override void Validate(PropContext context, IAssetValidator validator)
        {
            base.Validate(context, validator);
            if (IsItem)
                Item.Validate(context, validator);
            if (IsItemSocket)
                ItemSocket.Validate(context, validator);
        }

        public override IEnumerable<AssetResource> GetResources(PropContext context)
        {
            if (Prefab)
                yield return new PrefabResource(Prefab, string.Empty);
            if (IsItem)
                foreach (var resource in Item.GetResources(context))
                    yield return resource;
        }
    }

    public abstract class DetailPropAsset<T> : GeneralScaleablePropAsset<T>, IQuantumGroupMember where T : DetailPropData
    {
        [Tooltip("If this value is not null, this prop will be quantum. Assign this field to the quantum group it should be a part of. The group it is assigned to determines what kind of quantum object it is")]
        public QuantumGroupPropAsset QuantumGroup;
        [Tooltip("When used in a quantum socket this object will randomize its rotation around the local Y axis.")]
        [ConditionalField(nameof(QuantumGroup))]
        public bool RandomizeYRotation = true;
        [Tooltip("When used in a quantum socket this object will align to the nearest gravity volume. Else use the rotation of the quantum socket.")]
        [ConditionalField(nameof(QuantumGroup))]
        public bool AlignWithGravity = true;

        public IProp GetQuantumGroup() => QuantumGroup ? QuantumGroup : null;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (QuantumGroup)
            {
                if (!RandomizeYRotation)
                    writer.WriteProperty("randomizeYRotation", RandomizeYRotation);
                if (!AlignWithGravity)
                    writer.WriteProperty("alignWithGravity", AlignWithGravity);
            }
            base.WriteJsonProps(context, writer);
        }
    }

    public abstract class DetailPropComponent<T> : GeneralScaleablePropComponent<T>, IQuantumGroupMember where T : DetailPropData
    {
        [Tooltip("If this value is not null, this prop will be quantum. Assign this field to the quantum group it should be a part of. The group it is assigned to determines what kind of quantum object it is")]
        public QuantumGroupPropAsset QuantumGroupAsset;
        [Tooltip("If this value is not null, this prop will be quantum. Assign this field to the quantum group it should be a part of. The group it is assigned to determines what kind of quantum object it is")]
        public QuantumGroupPropComponent QuantumGroup;
        [Tooltip("When used in a quantum socket this object will randomize its rotation around the local Y axis.")]
        public bool RandomizeYRotation = true;
        [Tooltip("When used in a quantum socket this object will align to the nearest gravity volume. Else use the rotation of the quantum socket.")]
        public bool AlignWithGravity = true;

        public IProp GetQuantumGroup() => QuantumGroupAsset ? (IProp)QuantumGroupAsset : QuantumGroup ? QuantumGroup : null;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (GetQuantumGroup() != null)
            {
                if (!RandomizeYRotation)
                    writer.WriteProperty("randomizeYRotation", RandomizeYRotation);
                if (!AlignWithGravity)
                    writer.WriteProperty("alignWithGravity", AlignWithGravity);
            }
            base.WriteJsonProps(context, writer);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(DetailPropAsset))]
    public class DetailPropAsset : DetailPropAsset<DetailPropData> { }

    public class DetailPropComponent : DetailPropComponent<DetailPropData> { }
}
