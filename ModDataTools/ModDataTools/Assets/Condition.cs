using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModDataTools.Utilities;

namespace ModDataTools.Assets
{
    [CreateAssetMenu(menuName = ASSET_MENU_PREFIX + nameof(ConditionAsset))]
    public class ConditionAsset : DataAsset, IValidateableAsset
    {
        [Tooltip("Determines the type of asset this condition will be prefixed under")]
        public ScopeType Scope;
        [Tooltip("The mod this condition belongs to")]
        [ConditionalField(nameof(Scope), ScopeType.Mod)]
        public ModManifestAsset Mod;
        [Tooltip("A dialogue to make this condition unique to")]
        [ConditionalField(nameof(Scope), ScopeType.Dialogue)]
        public DialogueAsset Dialogue;
        [Tooltip("A planet to make this condition unique to")]
        [ConditionalField(nameof(Scope), ScopeType.Planet)]
        public PlanetAsset Planet;
        [Tooltip("A solar system to make this condition unique to")]
        [ConditionalField(nameof(Scope), ScopeType.SolarSystem)]
        public StarSystemAsset SolarSystem;
        [Header("Data")]
        [Tooltip("Whether the condition will only last for the current loop or (if persistent) indefinitely in the current save, unless cancelled or deleted")]
        public bool Persistent;

        public override IEnumerable<DataAsset> GetParentAssets()
        {
            if (Scope == ScopeType.Mod && Mod) yield return Mod;
            if (Scope == ScopeType.Dialogue && Dialogue) yield return Dialogue;
            if (Scope == ScopeType.Planet && Planet) yield return Planet;
            if (Scope == ScopeType.SolarSystem && SolarSystem) yield return SolarSystem;
        }

        public enum ScopeType
        {
            Mod,
            Dialogue,
            Planet,
            SolarSystem,
        }

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (Scope == ScopeType.Mod && !Mod)
                validator.Error(this, $"Missing ${nameof(Mod)}");
            if (Scope == ScopeType.Dialogue && !Dialogue)
                validator.Error(this, $"Missing ${nameof(Dialogue)}");
            if (Scope == ScopeType.Planet && !Planet)
                validator.Error(this, $"Missing ${nameof(Planet)}");
            if (Scope == ScopeType.SolarSystem && !SolarSystem)
                validator.Error(this, $"Missing ${nameof(SolarSystem)}");
        }
    }
}
