using CakeMachine.Fabrication.Elements;
using CakeMachine.Fabrication.Opérations;

namespace CakeMachine.Simulation
{
    internal class SingleThread : IAlgorithme
    {
        /// <inheritdoc />
        public bool IsAsync => false;

        /// <inheritdoc />
        public IEnumerable<GâteauEmballé> Produire(int nombreGâteaux, 
            Préparation postePréparation, 
            Cuisson posteCuisson,
            Emballage posteEmballage)
        {
            for (var i = 0; i < nombreGâteaux; i++)
            {
                var plat = new Plat();

                var gâteauCru = postePréparation.Préparer(plat);
                var gâteauCuit = posteCuisson.Cuire(gâteauCru).Single();
                var gâteauEmballé = posteEmballage.Emballer(gâteauCuit);

                yield return gâteauEmballé;
            }
        }

        /// <inheritdoc />
        public IAsyncEnumerable<GâteauEmballé> ProduireAsync(int nombreGâteaux, 
            Préparation postePréparation, 
            Cuisson posteCuisson,
            Emballage posteEmballage)
        {
            throw new NotImplementedException();
        }
    }
}
