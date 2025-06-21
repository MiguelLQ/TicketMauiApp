using CommunityToolkit.Maui;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Repositories;
using MauiFirebase.Data.Sources;
using MauiFirebase.PageModels.CategoriaResiduos;
using MauiFirebase.PageModels.Residuos;
using MauiFirebase.PageModels.Ticket;
using MauiFirebase.Pages.CategoriaResiduo;
using MauiFirebase.Pages.Premio;
using MauiFirebase.Pages.Residuo;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

namespace MauiFirebase;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionToolkit()
            .ConfigureMauiHandlers(handlers =>
            {
#if IOS || MACCATALYST
				handlers.AddHandler<Microsoft.Maui.Controls.CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
#endif
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
            });

#if DEBUG
		builder.Logging.AddDebug();
		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        builder.Services.AddSingleton<ProjectRepository>();
        builder.Services.AddSingleton<TaskRepository>();
        builder.Services.AddSingleton<CategoryRepository>();
        builder.Services.AddSingleton<TagRepository>();
        builder.Services.AddSingleton<SeedDataService>();
        builder.Services.AddSingleton<ModalErrorHandler>();
        builder.Services.AddSingleton<MainPageModel>();
        builder.Services.AddSingleton<ProjectListPageModel>();
        builder.Services.AddSingleton<ManageMetaPageModel>();
        // sql lite
        //builder.Services.AddSingleton(new SQLiteDataSource(
        //         Path.Combine(FileSystem.AppDataDirectory, "appdb.db3")));

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "appdb.db3");
        builder.Services.AddSingleton(new AppDatabase(dbPath));
  
        builder.Services.AddSingleton<ICategoriaRepository, ResiduoRepository>();
        builder.Services.AddSingleton<ResiduoPageModel>();
        builder.Services.AddSingleton<AgregarResiduoPage>();
        builder.Services.AddSingleton<ListarResiduoPage>();
        builder.Services.AddSingleton<EditarResiduoPage>();

        builder.Services.AddSingleton<ICategoriaResiduoRepository, CategoriaResiduoRepository>();
        builder.Services.AddSingleton<CategoriaResiduoPageModel>();
        builder.Services.AddSingleton<CategoriaResiduoPage>();

        builder.Services.AddSingleton<ITicketRepository, TicketRepository>();
        builder.Services.AddSingleton<TicketPageModel>();
   




        builder.Services.AddSingleton<IPremioRepository, PremioRepository>();
        builder.Services.AddTransient<PremioPage>();

        builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>("project");
        builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");

        return builder.Build();
    }
}
