using System.Text;
using CakeMachine.Fabrication.Elements;
using CakeMachine.Simulation.Algorithmes;

namespace CakeMachine.Simulation
{
    internal record RésultatSimulation(
        Algorithme Algorithme,
        bool IsSync,
        TimeSpan Temps,
        IReadOnlyDictionary<DestinationPlat, uint> DestinationPlats)
    {
        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder($"L'algorithme {Algorithme} a produit ");
            builder.Append($"{DestinationPlats[DestinationPlat.LivréConforme]} gâteaux livrés conformes, ");
            builder.Append($"mis {DestinationPlats[DestinationPlat.Rebut] + DestinationPlats[DestinationPlat.LivréNonConforme]} éléments au rebut, ");
            builder.Append($"perdu {DestinationPlats[DestinationPlat.Perdu]} plats lors du traitement ");
            builder.Append($"en {Temps.TotalSeconds:F}s");
            return builder.ToString();
        }
    }
}
