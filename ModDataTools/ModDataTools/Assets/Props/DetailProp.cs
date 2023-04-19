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
using UnityEngine.Assertions;

namespace ModDataTools.Assets.Props
{
    [Serializable]
    public class DetailPropData : GeneralPropData
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
            }
            if (IgnoreSun)
                writer.WriteProperty("ignoreSun", IgnoreSun);
        }

        public override IEnumerable<AssetResource> GetResources(PropContext context)
        {
            if (Prefab)
                yield return new PrefabResource(Prefab, string.Empty);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(DetailPropAsset))]
    public class DetailPropAsset : GeneralPropAsset<DetailPropData>
    {
        [Tooltip("Scale the prop")]
        public float Scale = 1f;
        [Tooltip("Scale each axis of the prop. Multiplied with scale.")]
        public Vector3 Stretch = Vector3.one;
        [Tooltip("If this value is not null, this prop will be quantum. Assign this field to the quantum group it should be a part of. The group it is assigned to determines what kind of quantum object it is")]
        public QuantumGroupPropAsset QuantumGroup;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (Stretch == Vector3.one)
                writer.WriteProperty("scale", Scale);
            else
                writer.WriteProperty("stretch", Stretch * Scale);
            if (QuantumGroup)
                writer.WriteProperty("quantumGroupID", QuantumGroup.FullID);
            base.WriteJsonProps(context, writer);
        }
    }

    public class DetailPropComponent : GeneralPropComponent<DetailPropData> {
        [Tooltip("If this value is not null, this prop will be quantum. Assign this field to asset representing the quantum group it should be a part of. The group it is assigned to determines what kind of quantum object it is")]
        public QuantumGroupPropAsset QuantumGroupAsset;
        [Tooltip("If this value is not null, this prop will be quantum. Assign this field to the quantum group it should be a part of. The group it is assigned to determines what kind of quantum object it is")]
        public QuantumGroupPropComponent QuantumGroup;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            if (transform.localScale.IsUniform())
                writer.WriteProperty("scale", transform.localScale.x);
            else
                writer.WriteProperty("stretch", transform.localScale);
            if (QuantumGroupAsset)
                writer.WriteProperty("quantumGroupID", QuantumGroupAsset.FullID);
            else if (QuantumGroup)
                writer.WriteProperty("quantumGroupID", QuantumGroup.PropID);
            base.WriteJsonProps(context, writer);
        }
    }
}
