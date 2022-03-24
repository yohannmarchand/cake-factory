using System;
using System.Threading.Tasks;
using CakeMachine.Simulation;
using CakeMachine.Simulation.Algorithmes;
using Xunit;
using Xunit.Abstractions;

namespace CakeMachine.Test
{
    public class TestAlgorithme
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestAlgorithme(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(typeof(SingleThread))]
        [InlineData(typeof(DeuxParDeux))]
        [InlineData(typeof(DixParDix))]
        [InlineData(typeof(FourRempli))]
        [InlineData(typeof(FourRempliSansRebut))]
        [InlineData(typeof(AntiRebut))]
        [InlineData(typeof(Optimisée1Poste))]
        public async Task TestAlgoOptimisé(Type algorithme)
        {
            var runner = new SingleAlgorithmRunner(algorithme);
            var (sync, async) = await runner.ProduirePendantAsync(TimeSpan.FromSeconds(5));
            
            if(sync is not null) _testOutputHelper.WriteLine(sync.ToString());
            if (async is not null) _testOutputHelper.WriteLine(async.ToString());
        }
    }
}