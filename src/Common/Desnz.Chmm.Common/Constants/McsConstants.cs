namespace Desnz.Chmm.Common.Constants;

public static class McsConstants
{
    public static class NewBuildOptions
    {
        public const int Yes = 1;
        public const int No = 2;
    }

    public static class HeatPumpTechnologyTypes
    {
        public const int AirSourceHeatPump = 1;
        public const int Biomass = 2;
        public const int GroundSourceHeatPump = 3;
        public const int GroundWaterSourceHeatPump = 4;
        public const int HotWaterHeatPump = 6;
        public const int Hydro = 7;
        public const int MicroChp = 8;
        public const int SolarKeymark = 9;
        public const int SolarPhotovoltaic = 10;
        public const int WindTurbine = 13;
        public const int ExhaustAirHeatPump = 14;
        public const int WaterSourceHeatPump = 15;
        public const int GasAbsorbtionHeatPump = 16;
        public const int SolarAssistedHeatPump = 17;
        public const int SolarThermal = 18;
        public const int BatteryStorage = 19;

    }

    public static class HeatPumpAirSourceTypes
    {
        public const int AirToAirSource = 1;
        public const int AirToWaterSource = 2;
    }

    public static class HeatPumpInstallationAges
    {
        public const int InstalledAtTheSameTime = 1;
        public const int UpToThreeMonths = 2;
        public const int ThreeMonthsToAYear	 = 3;
        public const int OverAYearAgo = 4;
        public const int NotKnown = 5;

    }
    public static class RenewableSystemDesigns
    {
        public const int SpaceHeatDhwAndAnotherPurpose = 1;
        public const int SpaceHeatAndAnotherPurpose = 2;
        public const int DhwAndAnotherPurpose = 3;
        public const int SpaceHeatAndDhw = 4;
        public const int SpaceHeatOnly = 5;
        public const int DhwOnly = 6;
        public const int AnotherPurposeOnly = 7;
    }


    public static class AlternativeSystemTypes
    {
        public const int BackBoiler = 1;
        public const int CombiBoiler = 2;
        public const int CommunityHeating = 3;
        public const int CondensingBoiler = 4;
        public const int CondensingCombiBoiler = 5;
        public const int CombinedPrimaryStorageUnitsCpsu = 6;
        public const int ElectricPanelConvectorOrRadiantHeaters = 7;
        public const int ElectricUnderfloorCeilingHeating = 8;
        public const int FireOrWallHeater = 9;
        public const int HeatPump = 10;
        public const int HighOrUnknownThermalCapacityBoiler = 11;
        public const int NoneOrDefaultPortableElectricHeaters = 12;
        public const int OpenFireWithBackBoiler = 13;
        public const int RangeCookerBoilerIntegralOvenAndBoiler = 14;
        public const int StandardBoiler = 15;
        public const int StorageHeaters = 16;
        public const int ClosedRoomHeaterStoveWithBackBoiler = 17;
        public const int WallOrFloorMountedBoilerBalancedOrOpenFluePre1998 = 18;
        public const int WarmAirSystems = 19;
        public const int Other = 20;
    }

    public static class AlternativeSystemFuelTypes
    {
        public const int Anthracite = 1;
        public const int BiogasLandfillCommunityHeatingOnly = 2;
        public const int BulkWoodPellets = 3;
        public const int Coal = 4;
        public const int DualFuelMineralAndWood = 5;
        public const int Electricity = 6;
        public const int LPG = 7;
        public const int MainsGas = 8;
        public const int Oil = 9;
        public const int WasteCombustionCommunityHeatingOnly = 10;
        public const int WoodChips = 11;
        public const int WoodLogs = 12;
        public const int NotApplicableNoOtherHeatingSource = 13;
        public const int Other = 14;
        public const int BioliquidHvoBioLpg = 15;
        public const int Solar = 16;
    }
}
