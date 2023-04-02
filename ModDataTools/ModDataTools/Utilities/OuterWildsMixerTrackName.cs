using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ModDataTools.Utilities
{
    public enum OuterWildsMixerTrackName
    {
        Undefined = 0,
        Menu = 1,
        Music = 2,
        Environment = 4,
        EnvironmentUnfiltered = 5,
        EndTimesSfx = 8,
        Signal = 16,
        Death = 32,
        Player = 64,
        PlayerExternal = 65,
        Ship = 128,
        Map = 256,
        EndTimesMusic = 512,
        MuffleWhileRafting = 1024,
        MuffleIndoors = 2048,
        SlideReelMusic = 4096,
    }
}
