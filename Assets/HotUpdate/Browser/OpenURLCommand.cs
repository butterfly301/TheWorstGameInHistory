using QFramework;

namespace HotUpdate.Browser
{
    public class OpenURLCommand : AbstractCommand, IController
    {
        private readonly string _url;

        public OpenURLCommand(string url)
        {
            _url = url;
        }

        protected override void OnExecute()
        {
            this.GetSystem<BrowserSystem>().HandleURL(_url);
        }
    }
}