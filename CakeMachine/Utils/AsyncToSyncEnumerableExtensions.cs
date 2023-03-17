namespace CakeMachine.Utils;

internal static class AsyncToSyncEnumerableExtensions
{
    public static async Task<IEnumerable<T>> ToEnumerableAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
    {
        var list = new List<T>();

        await foreach(var element in asyncEnumerable)
            list.Add(element);

        return list;
    }
}