using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Models;

namespace MauiFirebase.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}