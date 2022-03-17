using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;

namespace CakeMachine.Simulation
{
    internal class SingleThread : Algorithme
    {
        /// <inheritdoc />
        public override bool SupportsSync => true;

        /// <inheritdoc />
        public override IEnumerable<GâteauEmballé> Produire(Usine usine, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var plat = new Plat();

                var gâteauCru = usine.Préparateurs.First().Préparer(plat);
                var gâteauCuit = usine.Fours.First().Cuire(gâteauCru).Single();
                var gâteauEmballé = usine.Emballeuses.First().Emballer(gâteauCuit);

                yield return gâteauEmballé;
            }
        }
    }
}
