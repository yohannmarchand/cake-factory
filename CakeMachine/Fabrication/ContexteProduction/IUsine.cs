using CakeMachine.Fabrication.Elements;
using CakeMachine.Fabrication.Opérations;
using CakeMachine.Fabrication.Paramètres;

namespace CakeMachine.Fabrication.ContexteProduction
{
    internal interface IUsine
    {
        ParamètresUsine OrganisationUsine { get; }

        IEnumerable<Préparation> Préparateurs { get; }
        IEnumerable<Cuisson> Fours { get; }
        IEnumerable<Emballage> Emballeuses { get; }

        IEnumerable<Plat> StockInfiniPlats { get; }
        void MettreAuRebut(IConforme conforme);
    }
}
