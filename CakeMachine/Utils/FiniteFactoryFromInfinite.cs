using System.Collections;
using CakeMachine.Fabrication;
using CakeMachine.Fabrication.Elements;
using CakeMachine.Simulation;

namespace CakeMachine.Utils
{
    internal static class FiniteFactoryFromInfinite
    {
        public static IEnumerable<GâteauEmballé> Produire(this Algorithme algorithme, uint nombreGâteaux, Usine usine)
            => new FiniteEnumerable(algorithme, nombreGâteaux, usine);

        public static IAsyncEnumerable<GâteauEmballé> ProduireAsync(this Algorithme algorithme,
            uint nombreGâteaux, Usine usine)
            => new FiniteAsyncEnumerable(algorithme, nombreGâteaux, usine);

        private class FiniteAsyncEnumerable : IAsyncEnumerable<GâteauEmballé>
        {
            private readonly Algorithme _algorithme;
            private readonly uint _nombreGâteaux;
            private readonly Usine _usine;

            public FiniteAsyncEnumerable(Algorithme algorithme, uint nombreGâteaux, Usine usine)
            {
                _algorithme = algorithme;
                _nombreGâteaux = nombreGâteaux;
                _usine = usine;
            }

            /// <inheritdoc />
            public IAsyncEnumerator<GâteauEmballé> GetAsyncEnumerator(CancellationToken cancellationToken = new())
                => new Enumerator(_algorithme, _nombreGâteaux, _usine, cancellationToken);

            private class Enumerator : IAsyncEnumerator<GâteauEmballé>
            {
                private readonly CancellationTokenSource _cancellationTokenSource;
                private uint _nombreActuel;
                private readonly uint _objectif;
                private readonly IAsyncEnumerator<GâteauEmballé> _infiniteEnumerator;

                public Enumerator(Algorithme algorithme, uint nombreGâteaux, Usine usine, CancellationToken parentToken)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    parentToken.Register(_cancellationTokenSource.Cancel);

                    _infiniteEnumerator = algorithme
                        .ProduireAsync(usine, _cancellationTokenSource.Token)
                        .GetAsyncEnumerator(_cancellationTokenSource.Token);

                    _objectif = nombreGâteaux;
                }

                /// <inheritdoc />
                public async ValueTask DisposeAsync()
                {
                    await _infiniteEnumerator.DisposeAsync();
                    _cancellationTokenSource.Dispose();
                }

                /// <inheritdoc />
                public async ValueTask<bool> MoveNextAsync()
                {
                    if (_nombreActuel > _objectif)
                    {
                        _cancellationTokenSource.Cancel();
                        return false;
                    }

                    _nombreActuel++;

                    return await _infiniteEnumerator.MoveNextAsync();
                }

                /// <inheritdoc />
                public GâteauEmballé Current => _infiniteEnumerator.Current;
            }
        }

        private class FiniteEnumerable : IEnumerable<GâteauEmballé>
        {
            private readonly Algorithme _algorithme;
            private readonly uint _nombreGâteaux;
            private readonly Usine _usine;

            public FiniteEnumerable(Algorithme algorithme, uint nombreGâteaux, Usine usine)
            {
                _algorithme = algorithme;
                _nombreGâteaux = nombreGâteaux;
                _usine = usine;
            }

            /// <inheritdoc />
            public IEnumerator<GâteauEmballé> GetEnumerator()
                => new Enumerator(_algorithme, _nombreGâteaux, _usine);

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private class Enumerator : IEnumerator<GâteauEmballé>
            {
                private readonly CancellationTokenSource _cancellationTokenSource;
                private uint _nombreActuel;
                private readonly uint _objectif;
                private readonly IEnumerator<GâteauEmballé> _infiniteEnumerator;

                public Enumerator(Algorithme algorithme, uint nombreGâteaux, Usine usine)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    _infiniteEnumerator = algorithme.Produire(usine, _cancellationTokenSource.Token).GetEnumerator();
                    _objectif = nombreGâteaux;
                }

                /// <inheritdoc />
                public bool MoveNext()
                {
                    if (_nombreActuel > _objectif)
                    {
                        _cancellationTokenSource.Cancel();
                        return false;
                    }

                    _nombreActuel++;

                    return _infiniteEnumerator.MoveNext();
                }

                /// <inheritdoc />
                public void Reset() => throw new InvalidOperationException();

                /// <inheritdoc />
                // ReSharper disable once AssignNullToNotNullAttribute
                public GâteauEmballé Current => _infiniteEnumerator.Current;

                /// <inheritdoc />
                object IEnumerator.Current => Current;

                /// <inheritdoc />
                public void Dispose()
                {
                    _cancellationTokenSource.Dispose();
                    _infiniteEnumerator.Dispose();
                }
            }
        }
    }
}
