using ChromaQi.Helpers;
using System.Threading.Tasks;
using Windows.Storage;

namespace ChromaQi.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private bool _isInTestMode;
        public bool IsInTestMode
        {
            get => _isInTestMode;
            set => Set(ref _isInTestMode, value);
        }

        public override async Task LoadAsync(object data = null)
        {
            IsInTestMode = await ApplicationData.Current.LocalSettings.ReadAsync<bool>("isInTestMode");
        }

        public override async Task SaveAsync()
        {
            await ApplicationData.Current.LocalSettings.SaveAsync("isInTestMode", _isInTestMode);
        }
    }
}
