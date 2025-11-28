namespace BeerProduction.Services;

public class MachineManager
{
    private readonly Dictionary<int, MachineControlService> _machines = new();

    public void AddMachine(int machineId, string name, string opcUrl)
    {
        var machine = new MachineControl(machineId, opcUrl, name);
        var queue = new BatchQueue();
        var service = new MachineControlService(machine, queue);
        _machines[machineId] = service;
    }

    public MachineControlService GetMachine(int machineId)
        => _machines.TryGetValue(machineId, out var service) ? service : null;

    public IEnumerable<MachineControlService> GetAllMachines()
        => _machines.Values;
}
