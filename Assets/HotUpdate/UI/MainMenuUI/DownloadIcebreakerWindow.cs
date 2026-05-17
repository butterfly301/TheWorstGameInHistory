public class DownloadIcebreakerWindow : DownloadWindow
{
    private int playThroughCount;

protected override void OnDownloadComplete()
    {
        base.OnDownloadComplete();
        playThroughCount = gameData.playThrough;
        if (playThroughCount >= 1)
            DownloadSuccess();
        else
            DownloadFailure();
    }
}