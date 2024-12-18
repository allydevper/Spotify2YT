using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Spotify2YT.ViewModel
{
    public partial class CounterViewModel : ObservableObject
    {
        [ObservableProperty]
        private int count;

        [RelayCommand]
        private void Increment()
        {
            Count++;
        }
    }
}
