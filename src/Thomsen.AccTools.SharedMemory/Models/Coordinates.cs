using System.Runtime.InteropServices;

namespace Thomsen.AccTools.SharedMemory.Models;

[StructLayout(LayoutKind.Sequential)]
[Serializable]
public struct Coordinates {
    public float X;
    public float Y;
    public float Z;
}

[StructLayout(LayoutKind.Sequential)]
[Serializable]
public struct TyreStat {
    public float FrontLeft;
    public float FrontRight;
    public float RearLeft;
    public float RearRight;
}