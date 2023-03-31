using System;
using System.Collections.Generic;
using System.Text;

namespace Thomsen.AccTools.SharedMemory.Models;

public enum TrackGripStatus {
    GREEN = 0,
    FAST = 1,
    OPTIMUM = 2,
    GREASY = 3,
    DAMP = 4,
    WET = 5,
    FLOODED = 6
}

public enum RainIntensity {
    NO_RAIN = 0,
    DRIZZLE = 1,
    LIGHT_RAIN = 2,
    MEDIUM_RAIN = 3,
    HEAVY_RAIN = 4,
    THUNDERSTORM = 5
}

public enum PenaltyType {
    None = 0,
    DriveThrough_Cutting = 1,
    StopAndGo_10_Cutting = 2,
    StopAndGo_20_Cutting = 3,
    StopAndGo_30_Cutting = 4,
    Disqualified_Cutting = 5,
    RemoveBestLaptime_Cutting = 6,
    DriveThrough_PitSpeeding = 7,
    StopAndGo_10_PitSpeeding = 8,
    StopAndGo_20_PitSpeeding = 9,
    StopAndGo_30_PitSpeeding = 10,
    Disqualified_PitSpeeding = 11,
    RemoveBestLaptime_PitSpeeding = 12,
    Disqualified_IgnoredMandatoryPit = 13,
    PostRaceTime = 14,
    Disqualified_Trolling = 15,
    Disqualified_PitEntry = 16,
    Disqualified_PitExit = 17,
    Disqualified_Wrongway = 18,
    DriveThrough_IgnoredDriverStint = 19,
    Disqualified_IgnoredDriverStint = 20,
    Disqualified_ExceededDriverStintLimit = 21
}

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
