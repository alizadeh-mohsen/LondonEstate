using CommunityToolkit.Mvvm.Input;
using LondonEstate.MAUI.Models;

namespace LondonEstate.MAUI.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}