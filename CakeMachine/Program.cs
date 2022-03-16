using System.Diagnostics;
using System.Reflection;
using CakeMachine.Fabrication;
using CakeMachine.Simulation;
using CakeMachine.Utils;

const int nombreGâteaux = 100;
var stopWatch = new Stopwatch();

var algorithmes = Assembly.GetExecutingAssembly().GetTypes()
    .Where(type => type.BaseType == typeof(Algorithme))
    .Select(Activator.CreateInstance)
    .Cast<Algorithme>()
    .ToArray();

var résultats = algorithmes.ToDictionary(algorithme => algorithme, _ => new Dictionary<bool, TimeSpan>());

foreach (var algorithme in algorithmes)
{
    var builder = new UsineBuilder();
    algorithme.ConfigurerUsine(builder);
    var usine = builder.Build();

    if(algorithme.SupportsSync)
    {
        stopWatch.Start();

        var gâteauxConformes = 0;

        while (gâteauxConformes < nombreGâteaux)
        {
            var gâteaux = algorithme.Produire(nombreGâteaux, usine);
            gâteauxConformes += gâteaux.Count(gâteau => gâteau.EstConforme);
        }

        stopWatch.Stop();

        résultats[algorithme][false] = stopWatch.Elapsed;
        stopWatch.Reset();
    }

    if (algorithme.SupportsAsync)
    {
        stopWatch.Start();

        var gâteauxConformes = 0;

        while (gâteauxConformes < nombreGâteaux)
        {
            var gâteaux = await algorithme.ProduireAsync(nombreGâteaux, usine).ToEnumerableAsync();
            gâteauxConformes += gâteaux.Count(gâteau => gâteau.EstConforme);
        }

        stopWatch.Stop();

        résultats[algorithme][true] = stopWatch.Elapsed;
        stopWatch.Reset();
    }
}

foreach (var (algorithme, perfomances) in résultats)
{
    if(perfomances.ContainsKey(false))
        Console.WriteLine($"Avec l'algorithme {algorithme}[Sync], {perfomances[false].TotalSeconds:F}s se sont écoulés pour produire {nombreGâteaux} gâteaux");

    if (perfomances.ContainsKey(true))
        Console.WriteLine($"Avec l'algorithme {algorithme}[Async], {perfomances[true].TotalSeconds:F}s se sont écoulés pour produire {nombreGâteaux} gâteaux");
}