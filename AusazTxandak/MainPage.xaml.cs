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


        /// <summary>
        /// Izena eskuratzen duen metodoa, erabilgarri dagoen konprobatzen du
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        /// <summary>
        /// Gure izenarekin fitxzategia sortzen du
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnSaveNamesClicked(object sender, EventArgs e)
        {
            var fitxatizena = FItxategiIzenentry.Text;
            var rutafitxat = RutaFitxategiEntry.Text;

            if (string.IsNullOrWhiteSpace(fitxatizena) || string.IsNullOrWhiteSpace(rutafitxat))
            {
                await DisplayAlert("Error", "Mesdez, sartu ruta eta fitxategi izen egokia.", "OK");
                return;
            }

            var fitxateiakonpleto = Path.Combine(rutafitxat, fitxatizena);
            await GordeIzenakAsync(fitxateiakonpleto);
        }

        /// <summary>
        /// Izenak idazten ditu fitxategian
        /// </summary>
        /// <param name="fitxategiaa"></param>
        /// <returns></returns>
        private async Task GordeIzenakAsync(string fitxategiaa)
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

        /// <summary>
        /// Gure fitxategiko izenak irakurzten ditu eta ezartzen ditu
        /// </summary>
        /// <returns></returns>
        private async Task KargatuIzenak()
        {
            var Fitxatizena = await IrakurriFitxategiakAsync("Izenak.txt");

            if (Fitxatizena != null && Fitxatizena.Length > 0)
            {
                Console.WriteLine("Izenak kargatzen......");
                erabilgarriIzenak = Fitxatizena.ToList();
                erabilgarriIzenak = erabilgarriIzenak.OrderBy(x => Guid.NewGuid()).ToList();
                Console.WriteLine("Izenak nahastuta eta akrgatuta.");
            }
            else
            {

                NombresLabel.Text = "Ez dira izenak aurkitu.";
            }
        }

        /// <summary>
        /// Gure fitxategia riakurtzen duen metodoa
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        private async Task<string[]> IrakurriFitxategiakAsync(string fitxategi)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = $"AusazTxandak.Resources.Raw.{fitxategi}";
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
                    Console.WriteLine("Fitxategia ez da aurkitu.");
                    NombresLabel.Text = "Fitxategia ez da aurkitu";
                }
            }

            return new string[0];
        }
    }
}
