using System.Reflection;
using CakeMachine.Simulation.Algorithmes;

namespace CakeMachine.Simulation
{
    internal class MultipleAlgorithmsRunner
    {
        private readonly IDictionary<Algorithme, SingleAlgorithmRunner> _runners;

        public MultipleAlgorithmsRunner()
        {
            _runners = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => type.BaseType == typeof(Algorithme))
                .Select(Activator.CreateInstance)
                .Cast<Algorithme>()
                .ToDictionary(algorithm => algorithm, algorithm => new SingleAlgorithmRunner(algorithm));
        }

        public async Task ProduirePendant(TimeSpan timeSpan)
        {
            var résultats = _runners.Keys.ToDictionary(algorithme => algorithme, _ => new Dictionary<bool, uint>());

            foreach (var (algorithme, runner) in _runners)
            {
                var (sync, async) = await runner.ProduirePendant(timeSpan);
                if (algorithme.SupportsAsync) résultats[algorithme][true] = async;
                if (algorithme.SupportsSync) résultats[algorithme][false] = sync;
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
            var résultats = _runners.Keys.ToDictionary(algorithme => algorithme, _ => new Dictionary<bool, TimeSpan>());

            foreach (var (algorithme, runner) in _runners)
            {
                var (sync, async) = await runner.ProduireNGâteaux(nombreGâteaux);
                if(algorithme.SupportsAsync) résultats[algorithme][true] = async;
                if (algorithme.SupportsSync) résultats[algorithme][false] = sync;
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
