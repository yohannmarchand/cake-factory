using System.Diagnostics;
using CakeMachine.Fabrication.ContexteProduction;

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

        public async Task<(uint Sync, uint Async)> ProduirePendant(TimeSpan timeSpan)
        {
            (uint Sync, uint Async) résultats = (0, 0);

            var builder = new UsineBuilder();
            _algorithme.ConfigurerUsine(builder);
            var usine = builder.Build();

            if (_algorithme.SupportsSync)
            {
                uint gâteauxConformes = 0;
                var cancellationTokenSource = new CancellationTokenSource(timeSpan);

                foreach (var gâteau in _algorithme.Produire(usine, cancellationTokenSource.Token))
                {
                    if (gâteau.EstConforme) gâteauxConformes++;
                }

                if (!cancellationTokenSource.IsCancellationRequested)
                    throw new Exception(
                        $"L'algorithme {_algorithme} n'a pas été capable de produire des gâteaux en continu suffisamment longtemps.");

                résultats.Sync = gâteauxConformes;
            }

            if (_algorithme.SupportsAsync)
            {
                uint gâteauxConformes = 0;
                var cancellationTokenSource = new CancellationTokenSource(timeSpan);

                await foreach (var gâteau in _algorithme.ProduireAsync(usine, cancellationTokenSource.Token))
                {
                    if (gâteau.EstConforme) gâteauxConformes++;
                }

                if(!cancellationTokenSource.IsCancellationRequested)
                    throw new Exception(
                        $"L'algorithme {_algorithme} n'a pas été capable de produire des gâteaux en continu suffisamment longtemps.");

                résultats.Async = gâteauxConformes;
            }

            return résultats;
        }

        public async Task<(TimeSpan Sync, TimeSpan Async)> ProduireNGâteaux(uint nombreGâteaux)
        {
            (TimeSpan Sync, TimeSpan Async) résultats = (default, default);
            var stopWatch = new Stopwatch();
            
            var builder = new UsineBuilder();
            _algorithme.ConfigurerUsine(builder);
            var usine = builder.Build();

            if (_algorithme.SupportsSync)
            {
                var gâteauxConformes = 0;

                var tokenSource = new CancellationTokenSource();
                using var producteur = _algorithme.Produire(usine, tokenSource.Token).GetEnumerator();
                
                stopWatch.Start();

                while (gâteauxConformes < nombreGâteaux)
                {
                    if (!producteur.MoveNext())
                        throw new Exception(
                            $"L'algorithme {_algorithme} n'a pas été capable de produire suffisamment de gâteaux.");

                    Debug.Assert(producteur.Current != null, "producteur.Current != null");
                    var gâteau = producteur.Current;
                    if (gâteau.EstConforme) gâteauxConformes++;
                }

                stopWatch.Stop();
                tokenSource.Cancel();

                résultats.Sync = stopWatch.Elapsed;
                stopWatch.Reset();
            }

            if (_algorithme.SupportsAsync)
            {
                var gâteauxConformes = 0;

                var tokenSource = new CancellationTokenSource();
                await using var producteur = _algorithme
                    .ProduireAsync(usine, tokenSource.Token)
                    .GetAsyncEnumerator(tokenSource.Token);

                stopWatch.Start();

                while (gâteauxConformes < nombreGâteaux)
                {
                    if (!await producteur.MoveNextAsync())
                        throw new Exception(
                            $"L'algorithme {_algorithme} n'a pas été capable de produire suffisamment de gâteaux.");

                    var gâteau = producteur.Current;
                    if (gâteau.EstConforme) gâteauxConformes++;
                }

                stopWatch.Stop();
                tokenSource.Cancel();

                résultats.Async = stopWatch.Elapsed;
                stopWatch.Reset();
            }

            return résultats;
        }
    }
}
