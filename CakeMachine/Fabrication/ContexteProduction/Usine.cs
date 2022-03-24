using System.Collections.Concurrent;
using CakeMachine.Fabrication.Elements;
using CakeMachine.Fabrication.Opérations;
using CakeMachine.Fabrication.Paramètres;
using CakeMachine.Utils;

namespace CakeMachine.Fabrication.ContexteProduction
{
    internal class Usine : IUsine
    {
        private readonly ConcurrentBag<Plat> _platsCréés = new();
        private readonly ConcurrentBag<IConforme> _rebut = new();

        public ParamètresUsine OrganisationUsine { get; }
        public const ushort TailleMaxUsine = 35;

        public IEnumerable<Préparation> Préparateurs { get; }
        public IEnumerable<Cuisson> Fours { get; }
        public IEnumerable<Emballage> Emballeuses { get; }

        public Usine(ThreadSafeRandomNumberGenerator rng, ParamètresUsine organisationUsine)
        {
            if (organisationUsine.NombrePréparateurs + organisationUsine.NombreEmballeuses + organisationUsine.NombreFours > TailleMaxUsine)
                throw new InvalidOperationException(
                    $"L'usine est pleine, elle peut compter au maximum {TailleMaxUsine} machines");
            OrganisationUsine = organisationUsine;

            Préparateurs = Enumerable
                .Range(0, organisationUsine.NombrePréparateurs)
                .Select(_ => new Préparation(rng.Fork(), organisationUsine.ParamètresPréparation));

            Fours = Enumerable
                .Range(0, organisationUsine.NombreFours)
                .Select(_ => new Cuisson(rng.Fork(), organisationUsine.ParamètresCuisson));

            Emballeuses = Enumerable
                .Range(0, organisationUsine.NombreEmballeuses)
                .Select(_ => new Emballage(rng.Fork(), organisationUsine.ParamètresEmballage));
        }

        public IEnumerable<Plat> StockInfiniPlats 
        { 
            get 
            {
                while (true)
                {
                    var plat = new Plat();
                    _platsCréés.Add(plat);
                    yield return plat;
                }
                // ReSharper disable once IteratorNeverReturns
            }
        }

        public IReadOnlyDictionary<Plat, DestinationPlat> DestinationPlats(IEnumerable<GâteauEmballé> gâteauxEmballésReçus)
        {
            var dictionary = new Dictionary<Plat, DestinationPlat>();
            var platsCréés = new HashSet<Plat>(_platsCréés);
            var rebut = new HashSet<IConforme>(_rebut);

            var gâteauxEmballésArray = gâteauxEmballésReçus.ToArray();
            var platsArrivésEnBoutDeChaîne = new HashSet<Plat>(gâteauxEmballésArray.Select(gâteau => gâteau.PlatSousJacent));

            foreach (var gâteauEmballé in gâteauxEmballésArray)
            {
                if(!platsCréés.Contains(gâteauEmballé.PlatSousJacent)) 
                    dictionary.Add(gâteauEmballé.PlatSousJacent, DestinationPlat.Inconnu);

                if (!gâteauEmballé.EstConforme) 
                    dictionary.Add(gâteauEmballé.PlatSousJacent, DestinationPlat.Rebut);

                if(rebut.Contains(gâteauEmballé))
                    dictionary.Add(gâteauEmballé.PlatSousJacent, DestinationPlat.RéutiliséFrauduleusement);

                dictionary.Add(gâteauEmballé.PlatSousJacent, DestinationPlat.Conforme);
            }

            foreach (var platCréé in platsCréés.Where(platCréé => !platsArrivésEnBoutDeChaîne.Contains(platCréé)))
            {
                if (rebut.Contains(platCréé))
                    dictionary.Add(platCréé, DestinationPlat.Rebut);

                dictionary.Add(platCréé, DestinationPlat.Perdu);
            }

            return dictionary;
        }

        /// <inheritdoc />
        public void MettreAuRebut(IConforme conforme)
        {
            _rebut.Add(conforme);
        }
    }
}
