using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppmovilPFinal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Reportes : ContentPage
    {
        public FirebaseClient firebaseClient;
        public List<Reporte> reportesList;

        public Reportes()
        {
            InitializeComponent();
            firebaseClient = new FirebaseClient("https://proyectofinalmovil-ed3f4-default-rtdb.firebaseio.com");
            reportesList = new List<Reporte>();
            CargarReportes();
        }

        // Método para cargar los reportes desde Firebase
        public async void CargarReportes()
        {
            var reportes = await firebaseClient
                .Child("Reportes")
                .OnceAsync<Reporte>();

            reportesList = reportes.Select(item => new Reporte
            {
                Id = item.Key,
                Title = item.Object.Title,
                Date = item.Object.Date,
                Description = item.Object.Description
            }).ToList();

            reportsListView.ItemsSource = reportesList;
        }

        // Método para agregar un nuevo reporte
        public async void OnAddReporteClicked(object sender, EventArgs e)
        {
            var nuevoReporte = new Reporte
            {
                Title = reportTitleEntry.Text,
                Date = reportDateEntry.Text,
                Description = reportDescriptionEditor.Text
            };

            try
            {
                var addedReporte = await firebaseClient
                    .Child("Reportes")
                    .PostAsync(nuevoReporte);

                nuevoReporte.Id = addedReporte.Key; // Asignar la clave generada por Firebase

                reportesList.Add(nuevoReporte);
                reportsListView.ItemsSource = null;  // Reset del ItemsSource
                reportsListView.ItemsSource = reportesList;

                LimpiarFormulario();

                // Mostrar mensaje de confirmación
                await DisplayAlert("Éxito", "Reporte generado correctamente", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo generar el reporte: {ex.Message}", "OK");
            }
        }

        // Método para seleccionar un reporte de la lista y cargar su información en el formulario
        public void OnReporteSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var reporteSeleccionado = e.SelectedItem as Reporte;
                reportTitleEntry.Text = reporteSeleccionado.Title;
                reportDateEntry.Text = reporteSeleccionado.Date;
                reportDescriptionEditor.Text = reporteSeleccionado.Description;

                // Guardar el reporte seleccionado para la actualización
                reporteSeleccionadoActual = reporteSeleccionado;

                // Limpiar selección
                reportsListView.SelectedItem = null;
            }
        }

        // Método para actualizar un reporte existente
        public async void OnUpdateReporteClicked(object sender, EventArgs e)
        {
            if (reporteSeleccionadoActual != null)
            {
                reporteSeleccionadoActual.Title = reportTitleEntry.Text;
                reporteSeleccionadoActual.Date = reportDateEntry.Text;
                reporteSeleccionadoActual.Description = reportDescriptionEditor.Text;

                try
                {
                    // Actualizar en Firebase
                    await firebaseClient
                        .Child("Reportes")
                        .Child(reporteSeleccionadoActual.Id)
                        .PutAsync(reporteSeleccionadoActual);

                    // Refrescar la lista
                    reportsListView.ItemsSource = null;
                    reportsListView.ItemsSource = reportesList;

                    LimpiarFormulario();

                    await DisplayAlert("Éxito", "Reporte actualizado correctamente", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"No se pudo actualizar el reporte: {ex.Message}", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Selecciona un reporte para actualizar", "OK");
            }
        }

        // Método para limpiar el formulario
        public void LimpiarFormulario()
        {
            reportTitleEntry.Text = string.Empty;
            reportDateEntry.Text = string.Empty;
            reportDescriptionEditor.Text = string.Empty;
            reporteSeleccionadoActual = null;
        }

        // Modelo de datos para Reporte
        public class Reporte
        {
            public string Id { get; set; }  // ID generado por Firebase
            public string Title { get; set; }
            public string Date { get; set; }
            public string Description { get; set; }
        }

        // Variable para almacenar el reporte seleccionado
        private Reporte reporteSeleccionadoActual;
    }
}