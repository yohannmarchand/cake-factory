using CakeMachine.Fabrication.ContexteProduction;
using CakeMachine.Fabrication.Elements;
using CakeMachine.Fabrication.Opérations;
using CakeMachine.Utils;

namespace CakeMachine.Simulation.Algorithmes
{
    internal class FourRempliSansRebut : Algorithme
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
                var gâteauxCrus = PréparerNConformes(postePréparation,
                    usine.OrganisationUsine.ParamètresCuisson.NombrePlaces,
                    usine.StockInfiniPlats, usine);

                var gâteauxCuits = posteCuisson.Cuire(gâteauxCrus);
                var gâteauxCuitsConformes = gâteauxCuits.TrierRebut(usine);
                
                var gâteauxEmballés = gâteauxCuitsConformes
                    .Select(posteEmballage.Emballer)
                    .ToArray();

                foreach (var gâteauEmballé in gâteauxEmballés)
                    yield return gâteauEmballé;
            }
        }

        private static GâteauCru[] PréparerNConformes(Préparation postePréparation, ushort gâteaux, IEnumerable<Plat> stockPlats, Usine usine)
        {
            var gâteauxConformes = new List<GâteauCru>(gâteaux);

            do
            {
                // ReSharper disable once PossibleMultipleEnumeration
                var plats = stockPlats.Take(gâteaux - gâteauxConformes.Count);

                var gâteauxCrus = plats.Select(postePréparation.Préparer);
                gâteauxConformes.AddRange(gâteauxCrus.TrierRebut(usine));
            } while (gâteauxConformes.Count < gâteaux);

            return gâteauxConformes.ToArray();
        }
    }
}
