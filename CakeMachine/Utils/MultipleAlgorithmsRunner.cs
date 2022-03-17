using System.Diagnostics;
using System.Reflection;
using CakeMachine.Fabrication;
using CakeMachine.Simulation;

namespace CakeMachine.Utils
{
    internal class MultipleAlgorithmsRunner
    {
        private readonly Algorithme[] _algorithmes;

        public MultipleAlgorithmsRunner()
        {
            _algorithmes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => type.BaseType == typeof(Algorithme))
                .Select(Activator.CreateInstance)
                .Cast<Algorithme>()
                .ToArray();
        }

        public async Task ProduirePendant(TimeSpan timeSpan)
        {
            var résultats = _algorithmes.ToDictionary(algorithme => algorithme, _ => new Dictionary<bool, uint>());

            foreach (var algorithme in _algorithmes)
            {
                var builder = new UsineBuilder();
                algorithme.ConfigurerUsine(builder);
                var usine = builder.Build();

                if (algorithme.SupportsSync)
                {
                    uint gâteauxConformes = 0;
                    var cancellationTokenSource = new CancellationTokenSource(timeSpan);

                    foreach (var gâteau in algorithme.Produire(usine, cancellationTokenSource.Token))
                    {
                        if (gâteau.EstConforme) gâteauxConformes++;
                    }
                    
                    résultats[algorithme][false] = gâteauxConformes;
                }

                if (algorithme.SupportsAsync)
                {
                    uint gâteauxConformes = 0;
                    var cancellationTokenSource = new CancellationTokenSource(timeSpan);

                    await foreach (var gâteau in algorithme.ProduireAsync(usine, cancellationTokenSource.Token))
                    {
                        if (gâteau.EstConforme) gâteauxConformes++;
                    }

                    résultats[algorithme][true] = gâteauxConformes;
                }
            }

            foreach (var (algorithme, perfomances) in résultats)
            {
                if (perfomances.ContainsKey(false))
                    Console.WriteLine(
                        $"Avec l'algorithme {algorithme}[Sync], {perfomances[false]} gâteaux ont été produits en {timeSpan:g}");

                if (perfomances.ContainsKey(true))
                    Console.WriteLine(
                        $"Avec l'algorithme {algorithme}[Async], {perfomances[true]} gâteaux ont été produits en {timeSpan:g}");
            }
        }

        public async Task ProduireNGâteaux(uint nombreGâteaux)
        {
            var stopWatch = new Stopwatch();

            var résultats = _algorithmes.ToDictionary(algorithme => algorithme, _ => new Dictionary<bool, TimeSpan>());

            foreach (var algorithme in _algorithmes)
            {
                var builder = new UsineBuilder();
                algorithme.ConfigurerUsine(builder);
                var usine = builder.Build();

                if (algorithme.SupportsSync)
                {
                    stopWatch.Start();

                    var gâteauxConformes = 0;

                    while (gâteauxConformes < nombreGâteaux)
                    {
                        var gâteaux = algorithme.Produire(nombreGâteaux, usine);
                        gâteauxConformes += gâteaux.Count(gâteau => gâteau.EstConforme);
                    }

                    stopWatch.Stop();

                    résultats[algorithme][false] = stopWatch.Elapsed;
                    stopWatch.Reset();
                }

                if (algorithme.SupportsAsync)
                {
                    stopWatch.Start();

                    var gâteauxConformes = 0;

                    while (gâteauxConformes < nombreGâteaux)
                    {
                        var gâteaux = await algorithme.ProduireAsync(nombreGâteaux, usine).ToEnumerableAsync();
                        gâteauxConformes += gâteaux.Count(gâteau => gâteau.EstConforme);
                    }

                    stopWatch.Stop();

                    résultats[algorithme][true] = stopWatch.Elapsed;
                    stopWatch.Reset();
                }
            }

            foreach (var (algorithme, perfomances) in résultats)
            {
                if (perfomances.ContainsKey(false))
                    Console.WriteLine(
                        $"Avec l'algorithme {algorithme}[Sync], {perfomances[false].TotalSeconds:F}s se sont écoulés pour produire {nombreGâteaux} gâteaux");

                if (perfomances.ContainsKey(true))
                    Console.WriteLine(
                        $"Avec l'algorithme {algorithme}[Async], {perfomances[true].TotalSeconds:F}s se sont écoulés pour produire {nombreGâteaux} gâteaux");
            }
        }
    }
}
