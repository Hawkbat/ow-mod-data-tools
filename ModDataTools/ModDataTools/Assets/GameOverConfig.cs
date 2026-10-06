using ModDataTools.Assets.Resources;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets
{
    [Serializable]
    public class GameOverConfig
    {
        [Tooltip("Text displayed in orange on game over.")]
        public string Text;
        [Tooltip("Change the colour of the game over text. Leave empty to use the default orange.")]
        public NullishColor Colour;
        [Tooltip("Condition that must be true for this game over to trigger. If this is on a credits volume, leave empty to always trigger this game over. Note this is a regular dialogue condition, not a persistent condition.")]
        public ConditionAsset Condition;
        [Tooltip("The type of credits that will run after the game over message is shown")]
        public CreditsType Type;
        [Tooltip("The audio to use for the credits music. Credits will be silent unless this is specified.")]
        [ConditionalField(nameof(Type), CreditsType.Custom)]
        public AudioConfig Audio;
        [Tooltip("The volume of the credits music.")]
        [ConditionalField(nameof(Type), CreditsType.Custom)]
        public float AudioVolume = 1f;
        [Tooltip("Determines if the credits music should loop.")]
        [ConditionalField(nameof(Type), CreditsType.Custom)]
        public bool AudioLooping;
        [Tooltip("Duration of the credits scroll in seconds.")]
        [ConditionalField(nameof(Type), CreditsType.Custom)]
        public float Length = 120f;

        public void ToJson(JsonTextWriter writer, string textKey, Func<UnityEngine.Object, string> getAudioPath)
        {
            writer.WriteStartObject();
            if (!string.IsNullOrEmpty(Text))
                writer.WriteProperty("text", textKey);
            writer.WriteProperty("colour", Colour);
            if (Condition)
                writer.WriteProperty("condition", Condition.FullID);
            if (Type != CreditsType.Fast)
                writer.WriteProperty("creditsType", Type);
            if (Type == CreditsType.Custom)
            {
                writer.WriteProperty("audio", Audio, getAudioPath);
                if (AudioVolume != 1f)
                    writer.WriteProperty("audioVolume", AudioVolume);
                if (AudioLooping)
                    writer.WriteProperty("audioLooping", AudioLooping);
                if (Length != 120f)
                    writer.WriteProperty("length", Length);
            }
            writer.WriteEndObject();
        }

        public void Localize(string textKey, Localization l10n)
        {
            if (!string.IsNullOrEmpty(Text))
                l10n.AddUI(textKey, Text);
        }

        public void Validate(DataAsset asset, IAssetValidator validator)
        {
            if (Condition && Condition.Persistent)
                validator.Error(asset, $"Game over condition '{Condition.FullID}' must not be persistent.");
        }

        public IEnumerable<AssetResource> GetResources(Func<UnityEngine.Object, string> getAudioPath)
        {
            if (Type == CreditsType.Custom)
                foreach (var resource in Audio.GetResources(getAudioPath))
                    yield return resource;
        }

        public enum CreditsType
        {
            Fast = 0,
            Final = 1,
            Kazoo = 2,
            None = 3,
            Custom = 4,
        }
    }
}
