using System.Threading.Tasks;
using QFramework;

namespace HotUpdate.Enemy
{
    public interface IEnemyDataModel : IModel
    {
        bool IsDataLoaded { get; }
        Task LoadDataAsync();
        string GetEnemyAddress(string enemyName);
    }
}