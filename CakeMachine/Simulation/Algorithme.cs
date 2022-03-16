using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;

namespace CakeMachine.Simulation
{
    internal abstract class Algorithme
    {
        public abstract bool SupportsAsync { get; }
        public abstract bool SupportsSync { get; }

        public virtual void ConfigurerUsine(IConfigurationUsine builder) { }

        public abstract IEnumerable<GâteauEmballé> Produire(int nombreGâteaux, Usine usine);
        public abstract IAsyncEnumerable<GâteauEmballé> ProduireAsync(int nombreGâteaux, Usine usine);
    }
}
