public static class Machines
{
    private static readonly Dictionary<int, MachineControl> _machines = new();
    private static readonly object _lock = new();

    static Machines()
    {
        _machines[1] = new MachineControl(1, "opc.tcp://127.0.0.1:4840");
        _machines[2] = new MachineControl(2, "opc.tcp://192.168.0.20:4840");
    }
    public static MachineControl Get(int id, string url)
    {
        //Lock to avoid connection on multiple threads
        lock (_lock)
        {
            //If machine already exists return machine
            //If not machine is made
            if (!_machines.TryGetValue(id, out MachineControl machine))
            {
                machine = new MachineControl(id, url);
                _machines[id] = machine;
            }
            return machine;
        }
    }
}