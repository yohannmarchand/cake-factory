using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;

namespace CakeMachine.Simulation
{
    internal interface IAlgorithme
    {
        bool SupportsAsync { get; }
        bool SupportsSync { get; }

        virtual void ConfigurerUsine(IConfigurationUsine builder) { }

        IEnumerable<GâteauEmballé> Produire(int nombreGâteaux, Usine usine);
        IAsyncEnumerable<GâteauEmballé> ProduireAsync(int nombreGâteaux, Usine usine);
    }
}
