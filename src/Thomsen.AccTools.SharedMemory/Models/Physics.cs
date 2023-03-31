using System.Runtime.InteropServices;

namespace Thomsen.AccTools.SharedMemory.Models;

[StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
[Serializable]
public struct Physics {
    public int PacketId;
    public float Gas;
    public float Brake;
    public float Fuel;
    public int Gear;
    public int Rpms;
    public float SteerAngle;
    public float SpeedKmh;
    public Coordinates Velocity;
    public Coordinates AccG;
    public TyreStat WheelSlip;
    public TyreStat WheelLoad;
    public TyreStat WheelsPressure;
    public TyreStat WheelAngularSpeed;
    public TyreStat TyreWear;
    public TyreStat TyreDirtyLevel;
    public TyreStat TyreCoreTemperature;
    public TyreStat CamberRad;
    public TyreStat SuspensionTravel;
    public float Drs;
    public float TC;
    public float Heading;
    public float Pitch;
    public float Roll;
    public float CgHeight;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public float[] CarDamage;
    public int NumberOfTyresOut;
    public int PitLimiterOn;
    public float Abs;
    public float KersCharge;
    public float KersInput;
    public int AutoShifterOn;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public float[] RideHeight;
    public float TurboBoost;
    public float Ballast;
    public float AirDensity;
    public float AirTemp;
    public float RoadTemp;
    public Coordinates LocalAngularVelocity;
    public float FinalFF;
    public float PerformanceMeter;
    public int EngineBrake;
    public int ErsRecoveryLevel;
    public int ErsPowerLevel;
    public int ErsHeatCharging;
    public int ErsisCharging;
    public float KersCurrentKJ;
    public int DrsAvailable;
    public int DrsEnabled;
    public TyreStat BrakeTemp;
    public float Clutch;
    public TyreStat TyreTempI;
    public TyreStat TyreTempM;
    public TyreStat TyreTempO;
    public int IsAIControlled;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public Coordinates[] TyreContactPoint;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public Coordinates[] TyreContactNormal;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public Coordinates[] TyreContactHeading;
    public float BrakeBias;
    public Coordinates LocalVelocity;
    public int P2PActivation;
    public int P2PStatus;
    public float CurrentMaxRpm;
    public TyreStat Mz;
    public TyreStat Fx;
    public TyreStat Fy;
    public TyreStat SlipRatio;
    public TyreStat SlipAngle;
    public int TcinAction;
    public int AbsInAction;
    public TyreStat SuspensionDamage;
    public TyreStat TyreTemp;
    public float WaterTemp;
    public TyreStat BrakePressure;
    public int FrontBrakeCompound;
    public int RearBrakeCompound;
    public TyreStat PadLife;
    public TyreStat DiscLife;
    public int IgnitionOn;
    public int StarterEngineOn;
    public int IsEngineRunning;
    public float KerbVibration;
    public float SlipVibrations;
    public float GVibrations;
    public float AbsVibrations;
}
