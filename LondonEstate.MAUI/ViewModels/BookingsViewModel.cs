using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LondonEstate.Core.Dtos;
using LondonEstate.MAUI.Services;
using System.Collections.ObjectModel;

namespace LondonEstate.MAUI.ViewModels;

public partial class BookingsViewModel : ObservableObject
{
    private readonly IFlatService _flatService;
    private List<BookingDto> _allFlats = new();

    [ObservableProperty]
    private ObservableCollection<BookingDto> _flats = new();

    [ObservableProperty]
    private ObservableCollection<BookingDto> _emptyFlats = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isSuccess = true;

    [ObservableProperty]
    private bool _hasStatusMessage;

    [ObservableProperty]
    private bool _isBusy;

    public BookingsViewModel(IFlatService flatService)
    {
        _flatService = flatService;
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        IsBusy = true;
        try
        {
            var rawFlats = await _flatService.GetBookings();
            _allFlats = rawFlats.ToList();

            var cutoff = DateTime.Today.AddHours(11);

            var emptyList = _allFlats
                .Where(f => f.CheckOut < cutoff)
                .OrderBy(f => f.Name)
                .Select(f => new BookingDto { Id = f.Id, Name = f.Name });

            EmptyFlats = new ObservableCollection<BookingDto>(emptyList);
            ApplyFilter();
        }
        catch (Exception ex)
        {
            ShowStatus($"Error loading bookings: {ex.Message}", false);
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Flats = new ObservableCollection<BookingDto>(_allFlats);
            return;
        }

        var term = SearchText.Trim().ToLower();
        var filtered = _allFlats.Where(item =>
            (item.Name?.ToLower().Contains(term) ?? false) ||
            (item.GuestName?.ToLower().Contains(term) ?? false) ||
            (item.Listings != null && item.Listings.Any(l => l.ListingName?.ToLower().Contains(term) ?? false))
        );

        Flats = new ObservableCollection<BookingDto>(filtered);
    }

    [RelayCommand]
    private async Task CopyEmptyFlatsAsync()
    {
        if (!EmptyFlats.Any())
        {
            ShowStatus("No empty flats to copy.", false);
            return;
        }

        var text = string.Join(Environment.NewLine, EmptyFlats.Select(f => f.Name));
        await Clipboard.Default.SetTextAsync(text);
        ShowStatus("Empty flats copied to clipboard!", true);
    }

    //[RelayCommand]
    //private async Task NavigateToCheckinAsync(int id)
    //{
    //    // Adjust the route according to your Shell navigation configuration
    //    await Shell.Current.GoToAsync($"CheckinPage?id={id}");
    //}

    private void ShowStatus(string message, bool isSuccess)
    {
        StatusMessage = message;
        IsSuccess = isSuccess;
        HasStatusMessage = true;
    }

    //[RelayCommand]
    //private void DismissStatus()
    //{
    //    HasStatusMessage = false;
    //}
}