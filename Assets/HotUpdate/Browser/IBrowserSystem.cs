using QFramework;

namespace HotUpdate.Browser
{
    public interface IBrowserSystem : ISystem
    {
        void HandleURL(string url);
    }
}