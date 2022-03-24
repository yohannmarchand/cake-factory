using CakeMachine.Fabrication.ContexteProduction;
using CakeMachine.Fabrication.Elements;
using CakeMachine.Fabrication.Opérations;
using CakeMachine.Utils;

namespace CakeMachine.Simulation.Algorithmes
{
    internal class DixParDix : Algorithme
    {
        /// <inheritdoc />
        public override bool SupportsSync => true;

        /// <inheritdoc />
        public override IEnumerable<GâteauEmballé> Produire(Usine usine, CancellationToken token)
        {
            var capacitéFour = usine.OrganisationUsine.ParamètresCuisson.NombrePlaces;

            var postePréparation = usine.Préparateurs.Single();
            var posteEmballage = usine.Emballeuses.Single();
            var posteCuisson = usine.Fours.Single();

            while (!token.IsCancellationRequested)
            {
                var plats = usine.StockInfiniPlats.Take(10);

                var gâteauxCrus = plats.Select(postePréparation.Préparer).ToArray();

                var gâteauxCuits = CuireParLots(gâteauxCrus, posteCuisson, capacitéFour);
                var gâteauxEmballés = gâteauxCuits.Select(posteEmballage.Emballer).ToArray();

                foreach (var gâteauEmballé in gâteauxEmballés)
                    yield return gâteauEmballé;
            }
        }

        private static IEnumerable<GâteauCuit> CuireParLots(IEnumerable<GâteauCru> gâteaux, Cuisson four, uint capacitéFour)
        {
            var queue = new Queue<GâteauCru>(gâteaux);

            while (queue.Any())
            {
                var gâteauxCuits = four.Cuire(queue.Dequeue(capacitéFour).ToArray());
                foreach (var gâteauCuit in gâteauxCuits)
                    yield return gâteauCuit;
            }
        }
    }
}
