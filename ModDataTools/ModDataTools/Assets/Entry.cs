using ModDataTools.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Assets
{
    [CreateAssetMenu]
    public class Entry : EntryBase
    {
        [Tooltip("The entry this entry is a child of")]
        public EntryBase Parent;
        [Tooltip("The curiosity this entry belongs to")]
        public Curiosity Curiosity;

        public override Curiosity GetCuriosity() => Curiosity;

        public override void Validate(IAssetValidator validator)
        {
            base.Validate(validator);
            if (!Curiosity)
                validator.Warn(this, $"{nameof(Curiosity)} not set");
        }
    }
}
