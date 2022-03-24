using System.Diagnostics;
using CakeMachine.Fabrication.ContexteProduction;
using CakeMachine.Fabrication.Elements;
using CakeMachine.Simulation.Algorithmes;

namespace CakeMachine.Simulation
{
    internal class SingleAlgorithmRunner
    {
        private readonly Algorithme _algorithme;

        public SingleAlgorithmRunner(Algorithme algorithme)
        {
            _algorithme = algorithme;
        }

        public SingleAlgorithmRunner(Type type) 
            : this((Algorithme) Activator.CreateInstance(type)!)
        {
        }

        public async Task<(RésultatSimulation? Sync, RésultatSimulation? Async)> ProduirePendantAsync(TimeSpan timeSpan)
            => await Produire(tuple => tuple.TempsÉcoulé >= timeSpan);

        public async Task<(RésultatSimulation? Sync, RésultatSimulation? Async)> ProduireNGâteaux(uint nombreGâteaux)
            => await Produire(tuple => tuple.GâteauxValidesProduits >= nombreGâteaux);

        private async Task<(RésultatSimulation? Sync, RésultatSimulation? Async)> Produire(
            Predicate<(TimeSpan TempsÉcoulé, uint GâteauxValidesProduits)> conditionSortie)
        {
            (RésultatSimulation? Sync, RésultatSimulation? Async) résultats = (default, default);
            var stopWatch = new Stopwatch();
            
            var builder = new UsineBuilder();
            _algorithme.ConfigurerUsine(builder);
            var usine = builder.Build();

            if (_algorithme.SupportsSync)
            {
                uint gâteauxConformes = 0;
                var gâteauxProduits = new List<GâteauEmballé>();

                var tokenSource = new CancellationTokenSource();
                using var producteur = _algorithme.Produire(usine, tokenSource.Token).GetEnumerator();
                
                stopWatch.Start();

                while (!conditionSortie((stopWatch.Elapsed, gâteauxConformes)))
                {
                    if (!producteur.MoveNext())
                        throw new Exception(
                            $"L'algorithme {_algorithme} n'a pas été capable de produire suffisamment de gâteaux.");

                    Debug.Assert(producteur.Current != null, "producteur.Current != null");
                    var gâteau = producteur.Current;

                    gâteauxProduits.Add(producteur.Current);
                    if (gâteau.EstConforme) gâteauxConformes++;
                }

                stopWatch.Stop();
                tokenSource.Cancel();

                var destinationPlats = usine.DestinationPlats(gâteauxProduits);

                if (destinationPlats[DestinationPlat.RebutMaisConforme] != 0)
                    throw new InvalidOperationException("Vous avez mis au rebut un produit conforme.");

                if (destinationPlats[DestinationPlat.Inconnu] != 0)
                    throw new InvalidOperationException("Vous avez tenté de créer un plat. Vous devez utiliser les plats du stock de l'usine.");

                if (destinationPlats[DestinationPlat.RéutiliséFrauduleusement] != 0)
                    throw new InvalidOperationException("Vous avez tenté de réutiliser un plat, repréparer un gâteau raté, " +
                                                        "recuire un gâteau mal cuit ou réemballer un gâteau mal emballé. C'est interdit.");

                résultats.Sync = new RésultatSimulation(_algorithme, true, stopWatch.Elapsed, destinationPlats);
                stopWatch.Reset();
            }

            if (_algorithme.SupportsAsync)
            {
                uint gâteauxConformes = 0;
                var gâteauxProduits = new List<GâteauEmballé>();

                var tokenSource = new CancellationTokenSource();
                await using var producteur = _algorithme
                    .ProduireAsync(usine, tokenSource.Token)
                    .GetAsyncEnumerator(tokenSource.Token);

                stopWatch.Start();

                while (!conditionSortie((stopWatch.Elapsed, gâteauxConformes)))
                {
                    if (!await producteur.MoveNextAsync())
                        throw new Exception(
                            $"L'algorithme {_algorithme} n'a pas été capable de produire suffisamment de gâteaux.");

                    var gâteau = producteur.Current;
                    if (gâteau.EstConforme) gâteauxConformes++;
                    gâteauxProduits.Add(gâteau);
                }

                stopWatch.Stop();
                tokenSource.Cancel();

                résultats.Async = new RésultatSimulation(_algorithme, true, stopWatch.Elapsed, usine.DestinationPlats(gâteauxProduits));
                stopWatch.Reset();
            }

            return résultats;
        }
    }
}
