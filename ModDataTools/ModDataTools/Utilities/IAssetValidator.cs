using ModDataTools.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModDataTools.Utilities
{
    public interface IAssetValidator
    {
        void Error(DataAsset asset, string message);
        void Warn(DataAsset asset, string message);
    }
}
