namespace SharedProj;

public enum GuideTypes
{
    Select,
    Land,
    Sea,
    Air,
    Country,
    AirTemplate,
    LandTemplate,
    NavalTemplate
}

public enum LandTemplate
{
    Infantry,
    Tank,
    MotorizedInfantry,
    SpaceMarine,
    Cavalry,
    Garrison,
}

public enum AirTemplate
{
    Fighter,
    CAS,
    HeavyBomber,
    Carrier,
    Recon,
    NavalBomber
}

public enum NavalTemplate
{
    Destroyer,
    Cruiser,
    Heavy,
    SuperHeavy,
    Carrier,
    Submarine,
}