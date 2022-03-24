using System.Reflection;
using CakeMachine.Simulation.Algorithmes;

namespace CakeMachine.Simulation
{
    internal class MultipleAlgorithmsRunner
    {
        private readonly IEnumerable<SingleAlgorithmRunner> _runners;

        public MultipleAlgorithmsRunner()
        {
            _runners = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => type.BaseType == typeof(Algorithme))
                .Select(algorithm => new SingleAlgorithmRunner(algorithm));
        }

        public async Task ProduirePendant(TimeSpan timeSpan)
        {
            var résultats = new List<RésultatSimulation>();

            foreach (var runner in _runners)
            {
                var (sync, async) = await runner.ProduirePendantAsync(timeSpan);
                if (async is not null) résultats.Add(async);
                if (sync is not null) résultats.Add(sync);
            }

            foreach (var résultat in résultats.OrderByDescending(r => r)) Console.WriteLine(résultat);
        }

        public async Task ProduireNGâteaux(uint nombreGâteaux)
        {
            var résultats = new List<RésultatSimulation>();

            foreach (var runner in _runners)
            {
                var (sync, async) = await runner.ProduireNGâteaux(nombreGâteaux);
                if (async is not null) résultats.Add(async);
                if (sync is not null) résultats.Add(sync);
            }

            foreach (var résultat in résultats.OrderByDescending(r => r)) Console.WriteLine(résultat);
        }
    }
}
