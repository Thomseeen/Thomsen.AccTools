
using System.Runtime.InteropServices;

namespace Thomsen.AccTools.SharedMemory.Models;

public enum FlagType {
    NO_FLAG = 0,
    BLUE_FLAG = 1,
    YELLOW_FLAG = 2,
    BLACK_FLAG = 3,
    WHITE_FLAG = 4,
    CHECKERED_FLAG = 5,
    PENALTY_FLAG = 6
}

public enum GameStatus {
    OFF = 0,
    REPLAY = 1,
    LIVE = 2,
    PAUSE = 3
}

public enum SessionType {
    UNKNOWN = -1,
    PRACTICE = 0,
    QUALIFY = 1,
    RACE = 2,
    HOTLAP = 3,
    TIME_ATTACK = 4,
    DRIFT = 5,
    DRAG = 6
}

[StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
[Serializable]
public struct Graphics {
    public int PacketId;
    public GameStatus Status;
    public SessionType Session;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
    public string CurrentTime;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
    public string LastTime;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
    public string BestTime;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
    public string Split;
    public int CompletedLaps;
    public int Position;
    public int iCurrentTime;
    public int iLastTime;
    public int iBestTime;
    public float SessionTimeLeft;
    public float DistanceTraveled;
    public int IsInPit;
    public int CurrentSectorIndex;
    public int LastSectorTime;
    public int NumberOfLaps;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
    public string TyreCompound;

    public float ReplayTimeMultiplier;
    public float NormalizedCarPosition;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public float[] CarCoordinates;

    public float PenaltyTime;
    public FlagType Flag;
    public int IdealLineOn;

    // since 1.5
    public int IsInPitLane;
    public float SurfaceGrip;

    // since 1.13
    public int MandatoryPitDone;
}
