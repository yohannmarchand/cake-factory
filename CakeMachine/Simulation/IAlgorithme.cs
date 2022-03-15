using CakeMachine.Fabrication.Elements;
using CakeMachine.Fabrication.Opérations;

namespace CakeMachine.Simulation
{
    internal interface IAlgorithme
    {
        bool IsAsync { get; }

        IEnumerable<GâteauEmballé> Produire(int nombreGâteaux, Préparation postePréparation, Cuisson posteCuisson, Emballage posteEmballage);
        IAsyncEnumerable<GâteauEmballé> ProduireAsync(int nombreGâteaux, Préparation postePréparation, Cuisson posteCuisson, Emballage posteEmballage);
    }
}
