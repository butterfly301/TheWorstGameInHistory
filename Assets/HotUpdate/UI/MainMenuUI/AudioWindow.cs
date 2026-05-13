using HotUpdate.UI;

public class AudioWindow : WindowBase
{
    public override void Init(MainMenu mainMenuVar)
    {
        base.Init(mainMenuVar);
        GetComponent<AudioPanel>()?.Init();
    }
}