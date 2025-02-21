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
            var shipSpawns = AssetRepository.GetProps<ShipSpawnPropData>(planet);
            if (playerSpawns.Any())
                writer.WriteProperty("playerSpawnPoints", playerSpawns);
            if (shipSpawns.Any())
                writer.WriteProperty("shipSpawnPoints", shipSpawns);
        }

        public override void Validate(PlanetAsset planet, IAssetValidator validator)
        {
            foreach (var spawn in AssetRepository.GetProps<PlayerSpawnPropData>(planet))
                spawn.Data.Validate(spawn, planet, validator);
            foreach (var spawn in AssetRepository.GetProps<ShipSpawnPropData>(planet))
                spawn.Data.Validate(spawn, planet, validator);
        }

        public override bool ShouldWrite(PlanetAsset planet)
        {
            var playerSpawns = AssetRepository.GetProps<PlayerSpawnPropData>(planet);
            var shipSpawns = AssetRepository.GetProps<ShipSpawnPropData>(planet);
            return playerSpawns.Any() || shipSpawns.Any();
        }
    }
}
