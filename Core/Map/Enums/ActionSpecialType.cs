﻿namespace Helion.Map
{
    /// <summary>
    /// An enumeration of every action special ID.
    /// </summary>
    public enum ActionSpecialID : ushort
    {
        None = 0,
        PolyobjStartLine = 1,
        PolyobjRotateLeft = 2,
        PolyobjRotateRight = 3,
        PolyobjMove = 4,
        PolyobjExplicitLine = 5,
        PolyobjMoveTimes8 = 6,
        PolyobjDoorSwing = 7,
        PolyobjDoorSlide = 8,
        LineHorizon = 9,
        DoorClose = 10,
        DoorOpen = 11,
        DoorRaise = 12,
        DoorLockedRaise = 13,
        DoorAnimated = 14,
        Autosave = 15,
        TransferWallLight = 16,
        ThingRaise = 17,
        StartConversation = 18,
        ThingStop = 19,
        FloorLowerByValue = 20,
        FloorLowerToLowest = 21,
        FloorLowerToNearest = 22,
        FloorRaiseByValue = 23,
        FloorRaiseToHighest = 24,
        FloorRaiseToNearest = 25,
        StairsBuildDown = 26,
        StairsBuildUp = 27,
        FloorRaiseAndCrush = 28,
        PillarBuild = 29,
        PillarOpen = 30,
        StairsBuildDownSync = 31,
        StairsBuildUpSync = 32,
        ForceField = 33,
        ClearForceField = 34,
        FloorRaiseByValueTimes8 = 35,
        FloorLowerByValueTimes8 = 36,
        FloorMoveToValue = 37,
        CeilingWaggle = 38,
        TeleportZombieChanger = 39,
        CeilingLowerByValue = 40,
        CeilingRaiseByValue = 41,
        CeilingCrushAndRaise = 42,
        CeilingLowerAndCrush = 43,
        CeilingCrushStop = 44,
        CeilingCrushRaiseAndStay = 45,
        FloorCrushStop = 46,
        CeilingMoveToValue = 47,
        SectorAttach3dMidtex = 48,
        GlassBreak = 49,
        ExtraFloorLightOnly = 50,
        SectorSetLink = 51,
        ScrollWall = 52,
        LineSetTextureOffset = 53,
        SectorChangeFlags = 54,
        LineSetBlocking = 55,
        LineSetTextureScale = 56,
        SectorSetPortal = 57,
        SectorCopyScroller = 58,
        PolyobjORMoveToSpot = 59,
        PlatPerpetualRaise = 60,
        PlatStop = 61,
        PlatDownWaitUpStay = 62,
        PlatDownByValue = 63,
        PlatUpWaitDownStay = 64,
        PlatUpByValue = 65,
        FloorLowerInstant = 66,
        FloorRaiseInstant = 67,
        FloorMoveToValueTimes8 = 68,
        CeilingMoveToValueTimes8 = 69,
        Teleport = 70,
        TeleportNoFog = 71,
        ThrustThing = 72,
        DamageThing = 73,
        TeleportNewMap = 74,
        TeleportEndGame = 75,
        TeleportOther = 76,
        TeleportGroup = 77,
        TeleportInSector = 78,
        ThingSetConversation = 79,
        ACSExecute = 80,
        ACSSuspend = 81,
        ACSTerminate = 82,
        ACSLockedExecute = 83,
        ACSExecuteWithResult = 84,
        ACSLockedExecuteDoor = 85,
        PolyobjMoveToSpot = 86,
        PolyobjStop = 87,
        PolyobjMoveTo = 88,
        PolyobjORMoveTo = 89,
        PolyobjORRotateLeft = 90,
        PolyobjORRotateRight = 91,
        PolyobjORMove = 92,
        PolyobjORMoveTimes8 = 93,
        PillarBuildAndCrush = 94,
        FloorAndCeilingLowerByValue = 95,
        FloorAndCeilingRaiseByValue = 96,
        CeilingLowerAndCrushDist = 97,
        SectorSetTranslucent = 98,
        FloorRaiseAndCrushDoom = 99,
        ScrollTextureLeft = 100,
        ScrollTextureRight = 101,
        ScrollTextureUp = 102,
        ScrollTextureDown = 103,
        CeilingCrushAndRaiseSilentDist = 104,
        DoorWaitRaise = 105,
        DoorWaitClose = 106,
        LineSetPortalTarget = 107,
        LightForceLightning = 109,
        LightRaiseByValue = 110,
        LightLowerByValue = 111,
        LightChangeToValue = 112,
        LightFade = 113,
        LightGlow = 114,
        LightFlicker = 115,
        LightStrobe = 116,
        LightStop = 117,
        PlaneCopy = 118,
        ThingDamage = 119,
        RadiusQuake = 120,
        LineSetIdentification = 121,
        ThingMove = 125,
        ThingSetSpecial = 127,
        ThrustThingZ = 128,
        UsePuzzleItem = 129,
        ThingActivate = 130,
        ThingDeactivate = 131,
        ThingRemove = 132,
        ThingDestroy = 133,
        ThingProjectile = 134,
        ThingSpawn = 135,
        ThingProjectileGravity = 136,
        ThingSpawnNoFog = 137,
        FloorWaggle = 138,
        ThingSpawnFacing = 139,
        SectorChangeSound = 140,
        PlayerSetTeam = 145,
        TeamScore = 152,
        TeamGivePoints = 153,
        TeleportNoStop = 154,
        LineSetPortal = 156,
        SetGlobalFogParameter = 157,
        FSExecute = 158,
        SectorSetPlaneReflection = 159,
        SectorSet3dFloor = 160,
        SectorSetContents = 161,
        CeilingCrushAndRaiseDist = 168,
        GenericCrusher2 = 169,
        SectorSetCeilingScale2 = 170,
        SectorSetFloorScale2 = 171,
        PlatUpNearestWaitDownStay = 172,
        NoiseAlert = 173,
        SendToCommunicator = 174,
        ThingProjectileIntercept = 175,
        ThingChangeTID = 176,
        ThingHate = 177,
        ThingProjectileAimed = 178,
        ChangeSkill = 179,
        ThingSetTranslation = 180,
        PlaneAlign = 181,
        LineMirror = 182,
        LineAlignCeiling = 183,
        LineAlignFloor = 184,
        SectorSetRotation = 185,
        SectorSetCeilingPanning = 186,
        SectorSetFloorPanning = 187,
        SectorSetCeilingScale = 188,
        SectorSetFloorScale = 189,
        StaticInit = 190,
        SetPlayerProperty = 191,
        CeilingLowerToHighestFloor = 192,
        CeilingLowerInstant = 193,
        CeilingRaiseInstant = 194,
        CeilingCrushRaiseAndStayA = 195,
        CeilingCrushAndRaiseA = 196,
        CeilingCrushAndRaiseSilentA = 197,
        CeilingRaiseByValueTimes8 = 198,
        CeilingLowerByValueTimes8 = 199,
        GenericFloor = 200,
        GenericCeiling = 201,
        GenericDoor = 202,
        GenericLift = 203,
        GenericStairs = 204,
        GenericCrusher = 205,
        PlatDownWaitUpStayLip = 206,
        PlatPerpetualRaiseLip = 207,
        TranslucentLine = 208,
        TransferHeights = 209,
        TransferFloorLight = 210,
        TransferCeilingLight = 211,
        SectorSetColor = 212,
        SectorSetFade = 213,
        SectorSetDamage = 214,
        TeleportLine = 215,
        SectorSetGravity = 216,
        StairsBuildUpDoom = 217,
        SectorSetWind = 218,
        SectorSetFriction = 219,
        SectorSetCurrent = 220,
        ScrollTextureBoth = 221,
        ScrollTextureModel = 222,
        ScrollFloor = 223,
        ScrollCeiling = 224,
        ScrollTextureOffsets = 225,
        ACSExecuteAlways = 226,
        PointPushSetForce = 227,
        PlatRaiseAndStayTx0 = 228,
        ThingSetGoal = 229,
        PlatUpByValueStayTx = 230,
        PlatToggleCeiling = 231,
        LightStrobeDoom = 232,
        LightMinNeighbor = 233,
        LightMaxNeighbor = 234,
        FloorTransferTrigger = 235,
        FloorTransferNumeric = 236,
        ChangeCamera = 237,
        FloorRaiseToLowestCeiling = 238,
        FloorRaiseByValueTxTy = 239,
        FloorRaiseByTexture = 240,
        FloorLowerToLowestTxTy = 241,
        FloorLowerToHighest = 242,
        ExitNormal = 243,
        ExitSecret = 244,
        ElevatorRaiseToNearest = 245,
        ElevatorMoveToFloor = 246,
        ElevatorLowerToNearest = 247,
        HealThing = 248,
        DoorCloseWaitOpen = 249,
        FloorDonut = 250,
        FloorAndCeilingLowerRaise = 251,
        CeilingRaiseToNearest = 252,
        CeilingLowerToLowest = 253,
        CeilingLowerToFloor = 254,
        CeilingCrushRaiseAndStaySilA = 255,
        FloorLowerToHighestEE = 256,
        FloorRaiseToLowest = 257,
        FloorLowerToLowestCeiling = 258,
        FloorRaiseToCeiling = 259,
        FloorToCeilingInstant = 260,
        FloorLowerByTexture = 261,
        CeilingRaiseToHighest = 262,
        CeilingToHighestInstant = 263,
        CeilingLowerToNearest = 264,
        CeilingRaiseToLowest = 265,
        CeilingRaiseToHighestFloor = 266,
        CeilingToFloorInstant = 267,
        CeilingRaiseByTexture = 268,
        CeilingLowerByTexture = 269,
        StairsBuildDownDoom = 270,
        StairsBuildUpDoomSync = 271,
        StairsBuildDownDoomSync = 272,
        StairsBuildUpDoomCrush = 273,
        DoorAnimatedClose = 274,
        FloorStop = 275,
        CeilingStop = 276,
        SectorSetFloorGlow = 277,
        SectorSetCeilingGlow = 278,
        FloorMoveToValueAndCrush = 279,
        CeilingMoveToValueAndCrush = 280
    }
}
