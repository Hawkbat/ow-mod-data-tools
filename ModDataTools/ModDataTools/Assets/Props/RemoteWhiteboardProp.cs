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
    public class RemoteWhiteboardPropData : GeneralPropData
    {
        [Tooltip("The remote projection that this whiteboard belongs to")]
        public RemoteProjectionAsset RemoteProjection;
        [Tooltip("Disable the wall, leaving only the pedestal and text.")]
        public bool DisableWall;

        public override void WriteJsonProps(PropContext context, JsonTextWriter writer)
        {
            var childStones = AssetRepository.GetAllProps<RemoteStonePropData>()
                .Where(ctx => ctx.Data.RemoteProjection.StarSystem == RemoteProjection.StarSystem);

            writer.WriteProperty("nomaiText", childStones);
            writer.WriteStartArray();
            foreach (var stone in childStones)
            {
                var stoneData = stone.Prop.GetData() as RemoteStonePropData;
                writer.WriteStartObject();
                writer.WriteProperty("id", stoneData.RemoteProjection.FullID);
                writer.WriteProperty("arcInfo", stoneData.TranslatorText.TextBlocks.Select(b => b.Arc));
                writer.WriteProperty("seed", stoneData.TranslatorText.Seed);
                writer.WriteProperty("location", stoneData.RemoteProjection == RemoteProjection ? TranslatorTextAsset.Location.A : TranslatorTextAsset.Location.B);
                writer.WriteProperty("xmlFile", stoneData.TranslatorText.GetXmlOutputPath());
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            if (DisableWall)
                writer.WriteProperty("disableWall", DisableWall);
        }
    }

    [CreateAssetMenu(menuName = PROP_MENU_PREFIX + nameof(RemoteWhiteboardPropAsset))]
    public class RemoteWhiteboardPropAsset : GeneralPropAsset<RemoteWhiteboardPropData> { }
    public class RemoteWhiteboardPropComponent : GeneralPropComponent<RemoteWhiteboardPropData> { }
}
