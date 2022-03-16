using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;

namespace CakeMachine.Simulation
{
    internal class SingleThread : Algorithme
    {
        /// <inheritdoc />
        public override bool SupportsAsync => false;

        /// <inheritdoc />
        public override bool SupportsSync => true;

        /// <inheritdoc />
        public override IEnumerable<GâteauEmballé> Produire(int nombreGâteaux, Usine usine)
        {
            for (var i = 0; i < nombreGâteaux; i++)
            {
                var plat = new Plat();

                var gâteauCru = usine.Préparateurs.First().Préparer(plat);
                var gâteauCuit = usine.Fours.First().Cuire(gâteauCru).Single();
                var gâteauEmballé = usine.Emballeuses.First().Emballer(gâteauCuit);

                yield return gâteauEmballé;
            }
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<GâteauEmballé> ProduireAsync(int nombreGâteaux, Usine usine)
        {
            throw new NotImplementedException();
        }
    }
}
