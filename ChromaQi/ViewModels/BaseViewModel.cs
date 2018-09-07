using GalaSoft.MvvmLight;
using System.Threading.Tasks;

namespace ChromaQi.ViewModels
{
    public abstract class BaseViewModel : ViewModelBase, IViewModel
    {
        public virtual Task LoadAsync(object data = null) => Task.CompletedTask;

        public virtual Task SaveAsync() => Task.CompletedTask;
    }
}
