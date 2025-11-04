namespace BeerProduction.Enums;

public enum MachineState
{
    Deactivated = 0,
    Clearing = 1,
    Stopped = 2,
    Starting = 3,
    Idle = 4,
    Suspended = 5,
    Execute = 6,
    Stopping = 7,
    Aborting = 8,
    Aborted = 9,
    Holding = 10,
    Held = 11,
    Resetting = 15,
    Completing = 16,
    Complete = 17,
    Deactivating = 18,
    Activating = 19
}