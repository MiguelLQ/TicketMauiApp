using CommunityToolkit.Maui;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Repositories;
using MauiFirebase.Data.Sources;
using MauiFirebase.PageModels.CategoriaResiduos;
using MauiFirebase.PageModels.RegistroDeReciclajes;
using MauiFirebase.PageModels.Residuos;
using MauiFirebase.PageModels.Ticket;
using MauiFirebase.PageModels.Canjes;
using MauiFirebase.Pages.CategoriaResiduo;
using MauiFirebase.Pages.RegistroDeReciclaje;
using MauiFirebase.Pages.Premio;
using MauiFirebase.Pages.Residuo;
using MauiFirebase.Pages.Canje;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using MauiFirebase.PageModels.Premios;
using MauiFirebase.Pages.Ticket;
using MauiFirebase.PageModels.Residentes;
using MauiFirebase.Pages.ResidentesView;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Helpers;
using AgregarCanjePage = MauiFirebase.Pages.Canje.AgregarCanjePage;

using MauiFirebase.PageModels.Conversiones;
using MauiFirebase.Pages.Convertidores;

using MauiFirebase.PageModels.Logins;
using MauiFirebase.Pages.usuario;
using MauiFirebase.PageModels.Usuarios;
using MauiFirebase.PageModels.Registers;
using MauiFirebase.Pages.Register;

namespace MauiFirebase;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; }///////////////////////////////
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
        builder.Services.AddSingleton<IAlertaHelper, AlertaHelpers>();
        // servicios firebase
        builder.Services.AddSingleton<FirebaseAuthService>();

        /*================================================================
         * Conexion local sqlite
         ================================================================*/
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "appdb.db3");
        builder.Services.AddSingleton(new AppDatabase(dbPath));
        /*================================================================
        * servicios residuos
        ================================================================*/
        builder.Services.AddSingleton<IResiduoRepository, ResiduoRepository>();
        builder.Services.AddSingleton<ResiduoPageModel>();
        builder.Services.AddSingleton<EditarResiduoPageModel>();
        builder.Services.AddSingleton<CrearResiduoPageModel>();
        builder.Services.AddSingleton<ListarResiduoPage>();
        builder.Services.AddSingleton<AgregarResiduoPage>();
        builder.Services.AddSingleton<EditarResiduoPage>();
        // premio
        builder.Services.AddSingleton<IPremioRepository, PremioRepository>();
        builder.Services.AddSingleton<PremioPageModel>();
        builder.Services.AddSingleton<EditarPremioPageModel>();
        builder.Services.AddSingleton<CrearPremioPageModel>();
        builder.Services.AddSingleton<ListarPremioPage>();
        builder.Services.AddSingleton<AgregarPremioPage>();
        builder.Services.AddSingleton<EditarPremioPage>();

        // convertidor
        builder.Services.AddSingleton<IConvertidorRepository, ConvertidorRepository>();
        builder.Services.AddSingleton<ConversionesPageModel>();
        builder.Services.AddSingleton<EditarConvertidorPageModel>();
        builder.Services.AddSingleton<CrearConvertidorPageModel>();
        builder.Services.AddSingleton<ListarConvertidorPage>();
        builder.Services.AddSingleton<AgregarConvertidorPage>();
        builder.Services.AddSingleton<EditarConvertidorPage>();
        // Ticket
        builder.Services.AddSingleton<ITicketRepository, TicketRepository>();
        builder.Services.AddSingleton<TicketPageModel>();
        builder.Services.AddTransient<ListarTicketPage>();
        // Categoria Residuo
        builder.Services.AddSingleton<ICategoriaResiduoRepository, CategoriaResiduoRepository>();
        builder.Services.AddSingleton<CategoriaResiduoPageModel>();
        builder.Services.AddSingleton<CategoriaResiduoPage>();

        builder.Services.AddSingleton<ITicketRepository, TicketRepository>();
        builder.Services.AddSingleton<TicketPageModel>();

        // registro reciclae

        builder.Services.AddSingleton<IRegistroDeReciclajeRepository, RegistroDeReciclajeRepository>();
        builder.Services.AddSingleton<IPremioRepository, PremioRepository>();
        builder.Services.AddSingleton<AgregarRegistroPageModel>();
        builder.Services.AddSingleton<ListarRegistrosPageModel>();
        builder.Services.AddSingleton<AgregarRegistroPage>();
        builder.Services.AddSingleton<ListarRegistrosPage>();

        ////canje
        builder.Services.AddSingleton<IPremioRepository, PremioRepository>();
     
        // Canje
        builder.Services.AddSingleton<ICanjeRepository, CanjeRepository>();
        builder.Services.AddSingleton<CanjePageModel>();

        builder.Services.AddSingleton<AgregarCanjePage>();

        builder.Services.AddSingleton<ListarCanjePage>();

        builder.Services.AddTransient<CrearCanjePageModel>();
        builder.Services.AddTransient<AgregarCanjePage>();

        builder.Services.AddTransient<EditarCanjePageModel>();
        builder.Services.AddTransient<EditarCanjePage>();
        // usuario
        builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
        builder.Services.AddSingleton<UsuarioPageModel>();
        builder.Services.AddSingleton<ListarUsuarioPage>();


        builder.UseMauiCommunityToolkit(); //  Esto es obligatorio


        builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>("project");
        builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");
        // Residente
        builder.Services.AddSingleton<IResidenteRepository, ResidenteRepository>();
        builder.Services.AddTransientWithShellRoute<ResidenteFormPage, ResidenteFormPageModel>("residenteForm");
        builder.Services.AddTransientWithShellRoute<ResidenteListPage, ResidenteListPageModel>("residenteList");
        
        // login
        builder.Services.AddSingleton<LoginPageModel>();

        //Registro
        //builder.Services.AddSingleton<RegisterPageModel>();
        //builder.Services.AddSingleton<RegisterPage>(); 



        // ==========================================================
        var app = builder.Build();
        Services = app.Services;
        // ==========================================================


        return app;
    }
}
