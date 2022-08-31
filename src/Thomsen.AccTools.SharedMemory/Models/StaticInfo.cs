
using System.Runtime.InteropServices;

namespace Thomsen.AccTools.SharedMemory.Models;

[StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
[Serializable]
public struct StaticInfo {
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
    public string SMVersion;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
    public string ACVersion;

    // session static info
    public int NumberOfSessions;
    public int NumCars;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
    public string CarModel;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
    public string Track;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
    public string PlayerName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
    public string PlayerSurname;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
    public string PlayerNick;

    public int SectorCount;

    // car static info
    public float MaxTorque;
    public float MaxPower;
    public int MaxRpm;
    public float MaxFuel;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public float[] SuspensionMaxTravel;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public float[] TyreRadius;

    // since 1.5
    public float MaxTurboBoost;
    public float Deprecated1; // AirTemp since 1.6 in physic
    public float Deprecated2; // RoadTemp since 1.6 in physic
    public int PenaltiesEnabled;
    public float AidFuelRate;
    public float AidTireRate;
    public float AidMechanicalDamage;
    public int AidAllowTyreBlankets;
    public float AidStability;
    public int AidAutoClutch;
    public int AidAutoBlip;

    // since 1.7.1
    public int HasDRS;
    public int HasERS;
    public int HasKERS;
    public float KersMaxJoules;
    public int EngineBrakeSettingsCount;
    public int ErsPowerControllerCount;

    // since 1.7.2
    public float TrackSPlineLength;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
    public string TrackConfiguration;

    // since 1.10.2
    public float ErsMaxJ;

    // since 1.13
    public int IsTimedRace;
    public int HasExtraLap;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
    public string CarSkin;
    public int ReversedGridPositions;
    public int PitWindowStart;
    public int PitWindowEnd;
}
