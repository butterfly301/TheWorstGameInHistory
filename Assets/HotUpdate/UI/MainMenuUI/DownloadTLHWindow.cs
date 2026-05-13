using HotUpdate.Data.Model;

public class DownloadTLHWindow : DownloadWindow
{
    protected override void OnDownloadComplete()
    {
        base.OnDownloadComplete();
        if (gameData.software.Contains(SoftwareName.IceBreaker))
            DownloadSuccess();
        else
            DownloadFailure();
    }
}