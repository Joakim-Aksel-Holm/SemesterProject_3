namespace BeerProduction.Enums;

public enum MachineSpeed
{
    // Pilsner speed range: 0-600
    PilsnerSlow = 200,
    PilsnerMedium = 400,
    PilsnerFast = 600,
    // Wheat speed range: 0-300
    WheatSlow = 100,
    WheatMedium = 200,
    WheatFast = 300,

    // IPA speed range: 0-150
    IPASlow = 50,
    IPAMedium = 100,
    IPAFast = 150,

    // Stout speed range: 0-200
    StoutSlow = 80,
    StoutMedium = 140,
    StoutFast = 200,

    // Ale speed range: 0-100
    AleSlow = 40,
    AleMedium = 70,
    AleFast = 100,

    // Alcohol Free speed range: 0-125
    AlcoholFreeSlow = 50,
    AlcoholFreeMedium = 90,
    AlcoholFreeFast = 125
}