using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;
using CakeMachine.Fabrication.Opérations;

namespace CakeMachine.Simulation
{
    internal class FourRempliSansRebus : Algorithme
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
                    usine.OrganisationUsine.ParamètresCuisson.NombrePlaces);

                var gâteauxCuits = posteCuisson.Cuire(gâteauxCrus);
                var gâteauxCuitsConformes = gâteauxCuits.Where(gâteau => gâteau.EstConforme);
                
                var gâteauxEmballés = gâteauxCuitsConformes
                    .Select(posteEmballage.Emballer)
                    .ToArray();

                foreach (var gâteauEmballé in gâteauxEmballés)
                    yield return gâteauEmballé;
            }
        }

        private static GâteauCru[] PréparerNConformes(Préparation postePréparation, ushort gâteaux)
        {
            var gâteauxConformes = new List<GâteauCru>(gâteaux);

            do
            {
                var plats = Enumerable.Range(0, gâteaux - gâteauxConformes.Count)
                    .Select(_ => new Plat());

                var gâteauxCrus = plats.Select(postePréparation.Préparer);
                gâteauxConformes.AddRange(gâteauxCrus.Where(gâteau => gâteau.EstConforme));
            } while (gâteauxConformes.Count < gâteaux);

            return gâteauxConformes.ToArray();
        }
    }
}
