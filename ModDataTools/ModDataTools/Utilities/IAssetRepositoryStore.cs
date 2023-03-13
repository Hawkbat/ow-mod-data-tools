using ModDataTools.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModDataTools.Utilities
{
    public interface IAssetRepositoryStore
    {
        public IEnumerable<T> LoadAssets<T>() where T : DataAsset;
    }
}
