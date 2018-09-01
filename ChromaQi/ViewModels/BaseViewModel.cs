using GalaSoft.MvvmLight;
using System.Threading.Tasks;

namespace ChromaQi.ViewModels
{
    public abstract class BaseViewModel : ViewModelBase, IViewModel
    {
        public abstract Task LoadAsync(object data = null);

        public abstract Task SaveAsync();
    }
}
