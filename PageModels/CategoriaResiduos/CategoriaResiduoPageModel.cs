using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Pages.CategoriaResiduo;

namespace MauiFirebase.PageModels.CategoriaResiduos;

public class CategoriaResiduoPageModel : INotifyPropertyChanged
{
    private IClosePopup? _popupCloser; // 🔧 Instancia UI del popup actual
    private readonly IAlertaHelper _alertaHelper;


    private readonly ICategoriaResiduoRepository _repository;
    private readonly ITicketRepository _ticketRepository;
    private readonly SincronizacionFirebaseService _sincronizacionFirebaseService;
    public ObservableCollection<Models.Ticket> ListaTickets { get; } = new();

    public ObservableCollection<Models.CategoriaResiduo> Categorias { get; set; } = new();

    private string _nombreCategoria;
    public string NombreCategoria
    {
        get => _nombreCategoria;
        set { _nombreCategoria = value; OnPropertyChanged(); }
    }

    private string _idTicket;
    public string IdTicket
    {
        get => _idTicket;
        set { _idTicket = value; OnPropertyChanged(); }
    }
    // capturar Id Conticket Seleccionado
    private Models.Ticket? _ticketSeleccionado;
    public Models.Ticket? TicketSeleccionado
    {
        get => _ticketSeleccionado;
        set
        {
            _ticketSeleccionado = value;
            IdTicket = value?.IdTicket ?? string.Empty;
            OnPropertyChanged();
        }
    }


    private bool _estadoCategoriaResiduo = true;
    public bool EstadoCategoriaResiduo
    {
        get => _estadoCategoriaResiduo;
        set { _estadoCategoriaResiduo = value; OnPropertyChanged(); }
    }
    //propiedad para manejar la categoría que se está editando
    private Models.CategoriaResiduo? _categoriaSeleccionada;
    public Models.CategoriaResiduo? CategoriaSeleccionada
    {
        get => _categoriaSeleccionada;
        set
        {
            _categoriaSeleccionada = value;
            if (value != null)
            {
                NombreCategoria = value.NombreCategoria!;
                EstadoCategoriaResiduo = value.EstadoCategoriaResiduo;
                TicketSeleccionado = ListaTickets.FirstOrDefault(t => t.IdTicket == value.IdTicket);
            }
            OnPropertyChanged();
        }
    }

    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(); }
    }

    // Comandos
    public ICommand AddCommand { get; }
    public ICommand LoadCommand { get; }

    //public ICommand LoadCommand { get; }
    public ICommand ChangeEstadoCommand { get; }
    public ICommand MostrarAgregarCategoriaCommand => new Command(MostrarAgregarCategoriaPopup);
    public ICommand GuardarNuevaCategoriaCommand => new Command(async () => await GuardarNuevaCategoriaAsync());
    public ICommand EditCategoriaResiduoCommand { get; }
    public ICommand GuardarEdicionCategoriaCommand => new Command(async () => await GuardarCambiosCategoriaAsync());


    public CategoriaResiduoPageModel(ICategoriaResiduoRepository repository, ITicketRepository ticketRepository, IAlertaHelper alertaHelper, SincronizacionFirebaseService sincronizacionFirebaseService)
    {
        _repository = repository;
        _ticketRepository = ticketRepository;
        _alertaHelper = alertaHelper;
        _sincronizacionFirebaseService = sincronizacionFirebaseService;
        AddCommand = new Command(async () => await AddAsync());
        LoadCommand = new Command(async () => await LoadAsync());
        ChangeEstadoCommand = new Command<string>(async (id) => await CambiarEstado(id));
        EditCategoriaResiduoCommand = new Command<Models.CategoriaResiduo>(async categoria => await OnEditCategoriaResiduo(categoria));
        _ = CargarTicketsAsync();
        _ = LoadAsync();
    }
    public async Task CargarTicketsAsync()
    {
        try
        {
            var lista = await _ticketRepository.GetAllTicketAync();
            ListaTickets.Clear();
            foreach (var t in lista)
                ListaTickets.Add(t);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error cargando tickets: {ex.Message}");
        }
    }

    public async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                await _sincronizacionFirebaseService!.SincronizarCategoriaResiduoDesdeFirebaseAsync();

            }
            Categorias.Clear();
            var listaCategorias = await _repository.GetAllCategoriaResiduoAsync();
            var listaTickets = await _ticketRepository.GetAllTicketAync();

            foreach (var categoria in listaCategorias)
            {
                categoria.Ticket = listaTickets.FirstOrDefault(t => t.IdTicket == categoria.IdTicket);
                Categorias.Add(categoria);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
   
    // Establece el popup actual para poder cerrarlo desde el ViewModel
    public void SetPopupCloser(IClosePopup popup)
    {
        _popupCloser = popup;
    }
    private async void MostrarAgregarCategoriaPopup()
    {
        NombreCategoria = string.Empty;
        EstadoCategoriaResiduo = true;
        IdTicket = IdTicket;

        var popup = new AgregarCategoriaPopup();
        popup.BindingContext = this;
        SetPopupCloser(popup); // << Esto permite luego cerrarlo
        await Application.Current.MainPage.ShowPopupAsync(popup);
    }
    private async Task GuardarNuevaCategoriaAsync()
    {
        if (string.IsNullOrWhiteSpace(NombreCategoria))
        {
            await _alertaHelper.ShowErrorAsync("El nombre de la categoria es obligatorio.");
            return;
        }

        var nuevo = new Models.CategoriaResiduo
        {
            NombreCategoria = NombreCategoria,
            EstadoCategoriaResiduo = EstadoCategoriaResiduo,
            IdTicket = IdTicket,
            Sincronizado = false
        };

        await _repository.CreateCategoriaResiduoAsync(nuevo);
        await LoadAsync();
        LimpiarFormulario();

        _popupCloser?.ClosePopup();
        await _alertaHelper.ShowSuccessAsync("Categoria agregada correctamente.");

        await IntentarSincronizarAsync();
    }
    private async Task GuardarCambiosCategoriaAsync()
    {
        if (CategoriaSeleccionada == null)
        {
            await _alertaHelper.ShowErrorAsync("No hay categoría seleccionada.");
            return;
        }

        CategoriaSeleccionada.NombreCategoria = NombreCategoria;
        CategoriaSeleccionada.EstadoCategoriaResiduo = EstadoCategoriaResiduo;
        CategoriaSeleccionada.IdTicket = TicketSeleccionado?.IdTicket ?? string.Empty;
        CategoriaSeleccionada.Sincronizado = false;

        await _repository.UpdateCategoriaResiduoAsync(CategoriaSeleccionada);
        await LoadAsync();

        _popupCloser?.ClosePopup();
        await _alertaHelper.ShowSuccessAsync("Categoría editada correctamente.");

        await IntentarSincronizarAsync();
    }

    private async Task IntentarSincronizarAsync()
    {
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            try
            {
                await _sincronizacionFirebaseService.SincronizarCategoriasResiduoAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" Error sincronizando: {ex.Message}");
                await _alertaHelper.ShowWarningAsync("Cambios guardados localmente. Se sincronizarán cuando haya conexión.");
            }
        }
    }

    private async Task OnEditCategoriaResiduo(Models.CategoriaResiduo categoria)
    {
        if (categoria == null) return;

        CategoriaSeleccionada = categoria;

        try
        {
            var popup = new EditarCategoriaPopup(); // Asegúrate de tener esta vista creada
            popup.BindingContext = this;
            SetPopupCloser(popup);
            await Application.Current.MainPage.ShowPopupAsync(popup);
        }
        catch (Exception ex)
        {
            await _alertaHelper.ShowErrorAsync($"Error al editar: {ex.Message}");
        }
    }


    private async Task AddAsync()
    {
        if (string.IsNullOrWhiteSpace(NombreCategoria)) return;

        var nuevo = new Models.CategoriaResiduo
        {
            NombreCategoria = NombreCategoria,
            EstadoCategoriaResiduo = EstadoCategoriaResiduo,
            IdTicket = IdTicket
        };

        await _repository.CreateCategoriaResiduoAsync(nuevo);
        await LoadAsync();

        NombreCategoria = string.Empty;
        EstadoCategoriaResiduo = true;
    }

    private async Task CambiarEstado(string id)
    {
        await _repository.ChangeEstadoCategoriaResiduoAsync(id);
        await _alertaHelper.ShowSuccessAsync("Estado cambiado correctamente.");
        await LoadAsync();
    }
    private void LimpiarFormulario()
    {
        NombreCategoria = string.Empty;
        EstadoCategoriaResiduo = true;
        TicketSeleccionado = null;
    }
    // =================== INOTIFY =========================
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
