using Chess.Core;
using Shouldly;

namespace Chess_Coding_Adventure.Test;

public class SearcherTests
{
    [Fact]
    public void ChoosesAMove()
    {
        var board = Board.CreateBoard();
        var search = new Searcher(board);

        Move? move = null;
        search.OnSearchComplete += m => move = m;

        Task.Run(() => search.StartSearch());
        Task.Delay(10).Wait();
        search.EndSearch();
        Task.Run(() =>
        {
            while (move == null)
            {
                Thread.Sleep(1);
            }
        }).Wait();

        move.ShouldNotBeNull();
        search.searchDiagnostics.numPositionsEvaluated.ShouldBeGreaterThan(10);
        search.searchDiagnostics.isBook.ShouldBeFalse(
            "searcher doesn't use book openings, bot does");

        // uncomment this to see some debug output
        // search.debugInfo.ShouldBeEmpty();
    }

    [Fact]
    public void SupportsCancellation()
    {
        var board = Board.CreateBoard();
        var search = new Searcher(board);

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));

        Move? move = null;

        try
        {
            search.StartSearch(cts.Token);
        }
        catch (OperationCanceledException)
        {
            move = search.BestMoveSoFar;
        }

        move.ShouldNotBeNull();
        search.searchDiagnostics.numPositionsEvaluated.ShouldBeGreaterThan(10);
    }
}
