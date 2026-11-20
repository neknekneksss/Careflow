
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CareFlow.Data;
using CareFlow.ViewModels;
using CareFlow.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using CareFlow.Services;

namespace CareFlow;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    private IServiceScope? _appScope;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();

            services.AddDbContext<CareFlowDbContext>(options =>
                options.UseSqlite(
                    $"Data Source={CareFlowDatabase.GetDatabasePath()}"));

            services.AddScoped<BookingService>();
            services.AddScoped<MainViewModel>();

            _serviceProvider = services.BuildServiceProvider();
            _appScope = _serviceProvider.CreateScope();

            var db = _appScope.ServiceProvider
                .GetRequiredService<CareFlowDbContext>();

            db.Database.Migrate();
            DbSeeder.Seed(db);

            desktop.MainWindow = new MainWindow
            {
                DataContext = _appScope.ServiceProvider
                    .GetRequiredService<MainViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
