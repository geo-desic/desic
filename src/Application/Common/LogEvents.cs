namespace Desic.Application.Common;

// do not change existing values once they are used in production in case any log query infrastructure depends on them
public static class LogEvents
{
#pragma warning disable format
    internal const int Start                               = 10000000;

    #region Non Entity Specific
    #endregion

    internal const int StartEntities                       = Start + OffsetNonEntities;

    #region EntityTypes
    internal const int StartEntityTypes                    = StartEntities + OffsetEntity;
    public const int ListEntityTypes                       = StartEntityTypes + OffsetList;
    public const int SeedEntityTypes                       = StartEntityTypes + OffsetSeed;
    #endregion

    #region Labels
    internal const int StartLabels                         = StartEntityTypes + OffsetEntity;
    public const int SeedLabels                            = StartLabels + OffsetSeed;
    #endregion

    #region Tags
    internal const int StartTags                           = StartLabels + OffsetEntity;
    #endregion

    #region Users
    internal const int StartUsers                          = StartTags + OffsetEntity;
    public const int CreateUser                            = StartUsers + OffsetCreate;
    public const int GetUser                               = StartUsers + OffsetGet;
    public const int SeedTestUsers                         = StartUsers + OffsetSeedTest;
    #endregion

    #region Iso3166Countries
    internal const int StartIso3166Countries               = StartUsers + OffsetEntity;
    public const int ListIso3166Countries                  = StartIso3166Countries + OffsetList;
    public const int SeedIso3166Countries                  = StartIso3166Countries + OffsetSeed;
    #endregion

    #region Processes
    internal const int StartProcesses                      = StartIso3166Countries + OffsetEntity;
    #endregion

    #region Offsets
    internal const int OffsetNonEntities                   = 1000000;
    internal const int OffsetEntity                        = 1000;
    internal const int OffsetCreate                        = 10;
    internal const int OffsetDelete                        = 20;
    internal const int OffsetGet                           = 30;
    internal const int OffsetList                          = 40;
    internal const int OffsetPatch                         = 50;
    internal const int OffsetSeed                          = 60;
    internal const int OffsetSeedTest                      = 70;
    internal const int OffsetUpdate                        = 80;
    #endregion
#pragma warning restore format
}
