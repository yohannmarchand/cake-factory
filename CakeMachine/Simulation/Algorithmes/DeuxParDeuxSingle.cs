using System.Runtime.CompilerServices;
using CakeMachine.Fabrication.ContexteProduction;
using CakeMachine.Fabrication.Elements;

namespace CakeMachine.Simulation.Algorithmes;

internal class DeuxParDeuxSingle : Algorithme
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
            var plat = usine.StockInfiniPlats.First();

            var gâteauCru = postePréparation.Préparer(plat);
            var gâteauCuit = posteCuisson.Cuire(gâteauCru).Single();
            var gâteauEmballé = posteEmballage.Emballer(gâteauCuit);
                
            yield return gâteauEmballé;
        }
    }
}