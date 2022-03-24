using CakeMachine.Fabrication.ContexteProduction;
using CakeMachine.Fabrication.Elements;

namespace CakeMachine.Simulation.Algorithmes
{
    internal class DeuxParDeux : Algorithme
    {
        /// <inheritdoc />
        public override bool SupportsSync => true;

        /// <inheritdoc />
        public override IEnumerable<GâteauEmballé> Produire(Usine usine, CancellationToken token)
        {
            var postePréparation = usine.Préparateurs.Single();
            var posteCuisson = usine.Fours.Single();
            var posteEmballage = usine.Emballeuses.Single();

            while (!token.IsCancellationRequested)
            {
                var plats = usine.StockInfiniPlats.Take(2).ToArray();

                var gâteauCru1 = postePréparation.Préparer(plats.First());
                var gâteauCru2 = postePréparation.Préparer(plats.Last());

                var gâteauxCuits = posteCuisson.Cuire(gâteauCru1, gâteauCru2);

                var gâteauEmballé1 = posteEmballage.Emballer(gâteauxCuits.First());
                var gâteauEmballé2 = posteEmballage.Emballer(gâteauxCuits.Last());

                yield return gâteauEmballé1;
                yield return gâteauEmballé2;
            }
        }
    }
}
