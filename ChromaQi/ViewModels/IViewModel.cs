using System.Threading.Tasks;

namespace ChromaQi.ViewModels
{
    public interface IViewModel
    {
        Task LoadAsync(object data = null);
        Task SaveAsync();
    }
}