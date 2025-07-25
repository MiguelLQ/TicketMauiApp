﻿using CommunityToolkit.Maui;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Repositories;
using MauiFirebase.Data.Sources;
using MauiFirebase.Helpers;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.PageModels.Canjes;
using MauiFirebase.PageModels.CategoriaResiduos;
using MauiFirebase.PageModels.Conversiones;
using MauiFirebase.PageModels.Logins;
using MauiFirebase.PageModels.Premios;
using MauiFirebase.PageModels.Registers;
using MauiFirebase.PageModels.RegistroDeReciclajes;
using MauiFirebase.PageModels.Residentes;
using MauiFirebase.PageModels.Residuos;
using MauiFirebase.PageModels.Rutas;
using MauiFirebase.PageModels.Ticket;
using MauiFirebase.PageModels.Usuarios;
using MauiFirebase.PageModels.Vehiculos;
using MauiFirebase.Pages.Canje;
using MauiFirebase.Pages.CategoriaResiduo;
using MauiFirebase.Pages.Convertidores;
using MauiFirebase.Pages.Home;
using MauiFirebase.Pages.Mapa;
using MauiFirebase.Pages.Premio;
using MauiFirebase.Pages.Register;
using MauiFirebase.Pages.RegistroCiudadano;
using MauiFirebase.Pages.RegistroDeReciclaje;
using MauiFirebase.Pages.ResidentesView;
using MauiFirebase.Pages.Residuo;
using MauiFirebase.Pages.Ruta;
using MauiFirebase.Pages.Ticket;
using MauiFirebase.Pages.usuario;
using MauiFirebase.Pages.Vehiculo;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using AgregarCanjePage = MauiFirebase.Pages.Canje.AgregarCanjePage;
using MauiFirebase.PageModels.Mapas;
using ZXing.Net.Maui.Controls;
using MauiFirebase.Pages.CamScaner;
using MauiFirebase.PageModels.CamScaners;

using Microcharts.Maui;
using Microsoft.Maui.Controls.Maps;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace MauiFirebase;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; }

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .UseMauiCommunityToolkit()
            .UseBarcodeReader()
            .UseMauiMaps()
            .UseSkiaSharp() // ✅ Activa soporte para Microcharts
            .ConfigureSyncfusionToolkit()
            
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

        builder.Services.AddSingleton<IAlertaHelper, AlertaHelpers>();
        builder.Services.AddSingleton<FirebaseAuthService>();

        // Base de datos SQLite
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app2.db3");
        builder.Services.AddSingleton(new AppDatabase(dbPath));

        // Repositorios y PageModels
        builder.Services.AddSingleton<IResiduoRepository, ResiduoRepository>();
        builder.Services.AddSingleton<ResiduoPageModel>();
        builder.Services.AddSingleton<EditarResiduoPageModel>();
        builder.Services.AddSingleton<CrearResiduoPageModel>();
        builder.Services.AddSingleton<ListarResiduoPage>();
        builder.Services.AddSingleton<AgregarResiduoPage>();
        builder.Services.AddSingleton<EditarResiduoPage>();

        builder.Services.AddSingleton<IPremioRepository, PremioRepository>();
        builder.Services.AddSingleton<PremioPageModel>();
        builder.Services.AddSingleton<EditarPremioPageModel>();
        builder.Services.AddSingleton<ListarPremioPage>();
        builder.Services.AddTransient<AgregarPremioPage>();
        builder.Services.AddTransient<CrearPremioPageModel>();
        builder.Services.AddSingleton<EditarPremioPage>();
        builder.Services.AddSingleton<FirebasePremioService>();

        builder.Services.AddSingleton<IConvertidorRepository, ConvertidorRepository>();
        builder.Services.AddSingleton<ConversionesPageModel>();
        builder.Services.AddSingleton<EditarConvertidorPageModel>();
        builder.Services.AddTransient<CrearConvertidorPageModel>();
        builder.Services.AddSingleton<ListarConvertidorPage>();
        builder.Services.AddSingleton<AgregarConvertidorPage>();
        builder.Services.AddTransient<EditarConvertidorPage>();

        builder.Services.AddSingleton<ITicketRepository, TicketRepository>();
        builder.Services.AddSingleton<TicketPageModel>();
        builder.Services.AddTransient<ListarTicketPage>();

        builder.Services.AddSingleton<ICategoriaResiduoRepository, CategoriaResiduoRepository>();
        builder.Services.AddSingleton<CategoriaResiduoPageModel>();
        builder.Services.AddSingleton<CategoriaResiduoPage>();

        builder.Services.AddSingleton<IRegistroDeReciclajeRepository, RegistroDeReciclajeRepository>();
        builder.Services.AddSingleton<AgregarRegistroPageModel>();
        builder.Services.AddSingleton<ListarRegistrosPageModel>();
        builder.Services.AddSingleton<AgregarRegistroPage>();
        builder.Services.AddSingleton<ListarRegistrosPage>();

        builder.Services.AddSingleton<ICanjeRepository, CanjeRepository>();
        builder.Services.AddSingleton<CanjePageModel>();
        builder.Services.AddSingleton<AgregarCanjePage>();
        builder.Services.AddSingleton<ListarCanjePage>();
        builder.Services.AddTransient<CrearCanjePageModel>();
        builder.Services.AddTransient<AgregarCanjePage>();
        builder.Services.AddTransient<EditarCanjePageModel>();
        builder.Services.AddTransient<EditarCanjePage>();
        //Rutas
        builder.Services.AddSingleton<IRutaRepository, RutaRepository>();
        builder.Services.AddSingleton<CrearRutaPageModel>();
        builder.Services.AddSingleton<EditarRutaPageModel>();
        builder.Services.AddSingleton<RutaPageModel>();
        builder.Services.AddSingleton<AgregarRutaPage>();
        builder.Services.AddSingleton<EditarRutaPage>();
        builder.Services.AddSingleton<ListarRutaPage>();
        builder.Services.AddSingleton<DibujarRutaPage>();
        // usuario

        builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
        builder.Services.AddSingleton<UsuarioPageModel>();
        builder.Services.AddSingleton<ListarUsuarioPage>();
        builder.Services.AddSingleton<AgregarUsuarioPage>();
        builder.Services.AddSingleton<EditarUsuarioPage>();

        builder.Services.AddSingleton<IResidenteRepository, ResidenteRepository>();
        builder.Services.AddTransient<ResidenteFormPage, ResidenteFormPageModel>();
        builder.Services.AddSingleton<ResidenteListPage, ResidenteListPageModel>();
        builder.Services.AddSingleton<InicioCiudadanoPageModel>();
        builder.Services.AddSingleton<inicioCiudadanoPage>();
        //home
        builder.Services.AddSingleton<LoginPageModel>();
        builder.Services.AddSingleton<DashboardPageModel>();
        builder.Services.AddSingleton<InicioPage>();
        //vehiculo
        builder.Services.AddSingleton<IVehiculoRepository, VehiculoRepository>();
        builder.Services.AddSingleton<VehiculoPageModel>();
        builder.Services.AddSingleton<EditarVehiculoPageModel>();
        builder.Services.AddSingleton<CrearVehiculoPageModel>();
        builder.Services.AddSingleton<ListarVehiculoPage>();
        builder.Services.AddSingleton<AgregarVehiculoPage>();
        builder.Services.AddSingleton<EditarVehiculoPageModel>();


        builder.Services.AddSingleton<RegisterPageModel>();
        builder.Services.AddSingleton<RegisterPage>();
        builder.Services.AddSingleton<inicioCiudadanoPage>();
        builder.Services.AddSingleton<RegistroCiudadanoPage>();
        builder.Services.AddSingleton<inicioConductorPage>();
        //camion
        builder.Services.AddSingleton<IUbicacionVehiculo, UbicacionVehiculoRepository>();
        builder.Services.AddSingleton<MonitorearCamionPage>();
        builder.Services.AddSingleton<UbicacionVehiculoPageModel>();
        builder.Services.AddSingleton<ConductorUbicacionPageModel>();
        builder.Services.AddSingleton<EnviarUbicacionPage>();
        //para scaner qr
        builder.Services.AddTransient<CamScanerPage>();
        builder.Services.AddTransient<CamScanerPageModel>();

        builder.Services.AddSingleton<RegistroCiudadanoPageModel>();
        // ==========================================================
        builder.Services.AddSingleton<RutaService>();
        builder.Services.AddSingleton<SincronizacionFirebaseService>();
        builder.Services.AddSingleton<FirebaseConvertidorService>();
        builder.Services.AddSingleton<FirebaseResidenteService>();
        builder.Services.AddSingleton<FirebaseRegistroReciclajeService>();
        builder.Services.AddSingleton<FirebasePremioService>();
        builder.Services.AddSingleton<FirebaseCanjeService>();
        builder.Services.AddSingleton<FirebaseVehiculoService>();
        builder.Services.AddSingleton<FirebaseTicketService>();
        builder.Services.AddSingleton<FirebaseCategoriaResiduoService>();
        builder.Services.AddSingleton<FirebaseResiduoService>();
        builder.Services.AddSingleton<FirebaseUbicacionService>();
        builder.Services.AddSingleton<FirebaseRutaService>();
        var app = builder.Build();
        Services = app.Services;
        return app;
    }
}