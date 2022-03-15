using System.Diagnostics;
using CakeMachine.Fabrication.Opérations;
using CakeMachine.Fabrication.Paramètres;
using CakeMachine.Simulation;
using CakeMachine.Utils;

const int nombreGâteaux = 10;
var stopWatch = new Stopwatch();
var rng = new ThreadSafeRandomNumberGenerator();

var préparation = new Préparation(rng.Fork(),
    new ParamètresPréparation(3, 0.05, TimeSpan.FromMilliseconds(5), TimeSpan.FromMilliseconds(8)));

var cuisson = new Cuisson(rng.Fork(), new ParamètresCuisson(5, 0.05, TimeSpan.FromMilliseconds(10)));

var emballage = new Emballage(rng.Fork(), new ParamètresEmballage(2, 0.05, TimeSpan.FromMilliseconds(2)));

var algorithmes = new IAlgorithme[]
{
    new SingleThread()
};

var résultats = algorithmes.ToDictionary(algorithme => algorithme, _ => TimeSpan.Zero);

foreach (var algorithme in algorithmes)
{
    stopWatch.Start();

    var gâteauxConformes = 0;

    while (gâteauxConformes < nombreGâteaux)
    {
        var gâteaux = algorithme.IsAsync
            ? await algorithme.ProduireAsync(nombreGâteaux, préparation, cuisson, emballage).ToEnumerableAsync()
            : algorithme.Produire(nombreGâteaux, préparation, cuisson, emballage);

        gâteauxConformes += gâteaux.Count(gâteau => gâteau.EstConforme);
    }

    stopWatch.Stop();

    résultats[algorithme] = stopWatch.Elapsed;
    stopWatch.Reset();
}

var résultatsOrdonnés = résultats.OrderBy(kv => kv.Value);

foreach (var (algorithme, temps) in résultatsOrdonnés)
{
    Console.WriteLine($"Avec l'algorithme {algorithme}, {temps.TotalSeconds:F}s se sont écoulés pour produire {nombreGâteaux} gâteaux");
}