namespace CakeMachine.Fabrication.Elements;

internal interface IConforme
{
    bool EstConforme { get; }
    Plat PlatSousJacent { get; }
}