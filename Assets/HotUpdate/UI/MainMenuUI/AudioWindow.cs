using HotUpdate.UI;

public class AudioWindow : WindowBase
{
    public override void Init(MainMenuForm mainMenuVar)
    {
        base.Init(mainMenuVar);
        GetComponent<AudioPanel>()?.Init();
    }
}