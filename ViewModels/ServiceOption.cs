
using CommunityToolkit.Mvvm.ComponentModel;

namespace CareFlow.ViewModels;

public partial class ServiceOption : ObservableObject
{
    public int ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private string availabilityText = "Check availability";
}
