using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace AusazTxandak
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<string> Izenak { get; set; } = new ObservableCollection<string>();
        private List<string> erabilgarriIzenak; // Erabilgarrid auden izenak

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = this;
        }

 
        private async void OnLoadNamesClicked(object sender, EventArgs e)
        {
            if (erabilgarriIzenak == null)
            {
                await KargatuIzenak();
            }

            if (erabilgarriIzenak != null && erabilgarriIzenak.Count > 0)
            {
                var Aukeratutakoizena = erabilgarriIzenak[0];
                Izenak.Add(Aukeratutakoizena);
           
                erabilgarriIzenak.RemoveAt(0);

                if (erabilgarriIzenak.Count == 0)
                {
                   
                    await DisplayAlert("Bukatuta", "Izen guztiak gehitu dira.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Ez daude izen gehiago erabilgarri.", "OK");
            }
        }

  
        private async void OnSaveNamesClicked(object sender, EventArgs e)
        {
            var fitxatizena = NombreArchivoEntry.Text;
            var rutafitxat = RutaArchivoEntry.Text;

            if (string.IsNullOrWhiteSpace(fitxatizena) || string.IsNullOrWhiteSpace(rutafitxat))
            {
                await DisplayAlert("Error", "Mesdez, sartu ruta eta fitxategi izen egokia.", "OK");
                return;
            }

            var fitxateiakonpleto = Path.Combine(rutafitxat, fitxatizena);
            await GuardarNombresAsync(fitxateiakonpleto);
        }

        private async Task GuardarNombresAsync(string fitxategiaa)
        {
            try
            {
                using (var stream = new FileStream(fitxategiaa, FileMode.Create))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        foreach (var nombre in Izenak)
                        {
                            await writer.WriteLineAsync(nombre); 
                        }
                    }
                }

               
                await DisplayAlert("Ondo", "Izenak gorde dira.", "OK");
            }
            catch (Exception ex)
            {
              
                await DisplayAlert("Error", "Arazo bat egon da izenak gordetzerakoan.", "OK");
            }
        }

        private async Task KargatuIzenak()
        {
            var nombresArchivo = await LeerArchivoAsync("Izenak.txt");

            if (nombresArchivo != null && nombresArchivo.Length > 0)
            {
                Console.WriteLine("Cargando nombres...");
                erabilgarriIzenak = nombresArchivo.ToList();
                erabilgarriIzenak = erabilgarriIzenak.OrderBy(x => Guid.NewGuid()).ToList();
                Console.WriteLine("Nombres cargados y mezclados.");
            }
            else
            {
              
                NombresLabel.Text = "No se encontraron nombres.";
            }
        }

        private async Task<string[]> LeerArchivoAsync(string archivo)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = $"AusazTxandak.Resources.Raw.{archivo}";
            Console.WriteLine(resourcePath);

            using (var stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var contenido = await reader.ReadToEndAsync();
                        Console.WriteLine(contenido);
                        return contenido.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                }
                else
                {
                    Console.WriteLine("El archivo no se encontró.");
                    NombresLabel.Text = "El archivo no se encontró.";
                }
            }

            return new string[0];
        }
    }
}
