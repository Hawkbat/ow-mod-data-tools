using ModDataTools.Assets.Props;
using ModDataTools.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModDataTools.Assets.PlanetModules
{
    [Serializable]
    public class SpawnModule : PlanetModule
    {
        public override void WriteJsonProps(PlanetAsset planet, JsonTextWriter writer)
        {
            var playerSpawns = AssetRepository.GetProps<PlayerSpawnPropData>(planet);
            var playerSpawn = playerSpawns.FirstOrDefault(s => s.Data.IsDefault) ?? playerSpawns.FirstOrDefault();
            var shipSpawn = AssetRepository.GetProps<ShipSpawnPropData>(planet).FirstOrDefault();
            if (playerSpawn != null)
                writer.WriteProperty("playerSpawn", playerSpawn);
            if (shipSpawn != null)
                writer.WriteProperty("shipSpawn", shipSpawn);
        }

        public override bool ShouldWrite(PlanetAsset planet)
        {
            var playerSpawns = AssetRepository.GetProps<PlayerSpawnPropData>(planet);
            var shipSpawns = AssetRepository.GetProps<ShipSpawnPropData>(planet);
            return playerSpawns.Any() || shipSpawns.Any();
        }
    }
}
