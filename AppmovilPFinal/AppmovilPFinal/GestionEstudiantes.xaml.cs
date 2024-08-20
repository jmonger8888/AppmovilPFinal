using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace AppmovilPFinal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GestionEstudiantes : ContentPage
    {
        //clases requeridas
        public FirebaseClient firebaseClient;//interaccion con firebase
        public Estudiante estudianteSeleccionado;//cuando se seleccionan los estudiantes en la lista
        public GestionEstudiantes()
        {
            //contructor que inicia todo
            InitializeComponent();

            firebaseClient = new FirebaseClient("https://proyectofinalmovil-ed3f4-default-rtdb.firebaseio.com");
            CargarEstudiantes();
        }
        //metodo para seleccionar una imagen des dispositivo
        public async void OnSelectImageClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await Xamarin.Essentials.FilePicker.PickAsync(new Xamarin.Essentials.PickOptions
                {
                    FileTypes = Xamarin.Essentials.FilePickerFileType.Images,
                    PickerTitle = "Selecciona una imagen"
                });

                if (result != null)
                {
                    // Aquí podrías guardar la URL o ruta del archivo seleccionado
                    await DisplayAlert("Imagen seleccionada", result.FileName, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo seleccionar la imagen: {ex.Message}", "OK");
            }
        }

        //carga los estudiantes que ya estas registrados en la bd y los muestra en la lista
        public async void CargarEstudiantes()
        {
            var estudiantes = await ObtenerEstudiantesAsync();
            studentsListView.ItemsSource = estudiantes;
        }

        public async Task<List<Estudiante>> ObtenerEstudiantesAsync()
        {
            var estudiantes = await firebaseClient
                .Child("Estudiantes")
                .OnceAsync<Estudiante>();

            return estudiantes.Select(item => new Estudiante
            {
                Codigo = item.Object.Codigo,
                Nombre = item.Object.Nombre,
                CodigoPadre = item.Object.CodigoPadre,
                NombrePadre = item.Object.NombrePadre,
                ImagenUrl = item.Object.ImagenUrl
            }).ToList();
        }

        // Método para agregar un estudiante nuevo a la base de datos
        public async void OnAddClicked(object sender, EventArgs e)
        {
            var nuevoEstudiante = new Estudiante
            {
                Codigo = studentCodeEntry.Text,
                Nombre = studentNameEntry.Text,
                CodigoPadre = parentCodeEntry.Text,
                NombrePadre = parentNameEntry.Text,
                ImagenUrl = "URL_DE_PRUEBA" // Aquí deberías asignar la URL de la imagen seleccionada
            };

            await GuardarEstudianteAsync(nuevoEstudiante);
            await DisplayAlert("Éxito", "Estudiante agregado correctamente", "OK");
            LimpiarFormulario();
            CargarEstudiantes();
        }

        //Método para modificar un estudiante existente
        public async void OnModifyClicked(object sender, EventArgs e)
        {
            if (estudianteSeleccionado != null)
            {
                // Verificar si el código del estudiante ha cambiado
                if (estudianteSeleccionado.Codigo != studentCodeEntry.Text)
                {
                    await DisplayAlert("Error", "No se puede modificar el código del estudiante", "OK");
                    return; // Detener el proceso si el código ha cambiado
                }

                // Actualizar los otros campos
                estudianteSeleccionado.Nombre = studentNameEntry.Text;
                estudianteSeleccionado.CodigoPadre = parentCodeEntry.Text;
                estudianteSeleccionado.NombrePadre = parentNameEntry.Text;

                await ModificarEstudianteAsync(estudianteSeleccionado);
                await DisplayAlert("Éxito", "Estudiante modificado correctamente", "OK");
                LimpiarFormulario();
                CargarEstudiantes();
            }
            else
            {
                await DisplayAlert("Error", "Selecciona un estudiante para modificar", "OK");
            }
        }

        public async Task GuardarEstudianteAsync(Estudiante estudiante)
        {
            await firebaseClient
                .Child("Estudiantes")
                .PostAsync(estudiante);
        }

        public async Task ModificarEstudianteAsync(Estudiante estudiante)
        {
            var estudiantes = await firebaseClient
                .Child("Estudiantes")
                .OnceAsync<Estudiante>();

            var estudianteAEditar = estudiantes.FirstOrDefault(a => a.Object.Codigo == estudiante.Codigo);

            if (estudianteAEditar != null)
            {
                await firebaseClient.Child("Estudiantes").Child(estudianteAEditar.Key).PutAsync(estudiante);
            }
        }

        //Método para eliminar un estudiante
        public async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (estudianteSeleccionado != null)
            {
                await EliminarEstudianteAsync(estudianteSeleccionado);
                await DisplayAlert("Éxito", "Estudiante eliminado correctamente", "OK");
                LimpiarFormulario();
                CargarEstudiantes();
            }
            else
            {
                await DisplayAlert("Error", "Selecciona un estudiante para eliminar", "OK");
            }
        }

        public async Task EliminarEstudianteAsync(Estudiante estudiante)
        {
            var estudiantes = await firebaseClient
                .Child("Estudiantes")
                .OnceAsync<Estudiante>();

            var estudianteAEliminar = estudiantes.FirstOrDefault(a => a.Object.Codigo == estudiante.Codigo);

            if (estudianteAEliminar != null)
            {
                await firebaseClient.Child("Estudiantes").Child(estudianteAEliminar.Key).DeleteAsync();
            }
        }

        //Método para limpiar el formulario desdues de ejecutar algun metodo
        public void LimpiarFormulario()
        {
            studentCodeEntry.Text = string.Empty;
            studentNameEntry.Text = string.Empty;
            parentCodeEntry.Text = string.Empty;
            parentNameEntry.Text = string.Empty;
            estudianteSeleccionado = null;
        }

        //Método para seleccionar un estudiante de la lista y cargar su información en el formulario
        public void studentsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                estudianteSeleccionado = e.SelectedItem as Estudiante;

                
                studentCodeEntry.Text = estudianteSeleccionado.Codigo;
                studentNameEntry.Text = estudianteSeleccionado.Nombre;
                parentCodeEntry.Text = estudianteSeleccionado.CodigoPadre;
                parentNameEntry.Text = estudianteSeleccionado.NombrePadre;

                //Limpia la selección para que se pueda volver a seleccionar un elemento 
                studentsListView.SelectedItem = null;
            }
        }
    }

    public class Estudiante
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string CodigoPadre { get; set; }
        public string NombrePadre { get; set; }
        public string ImagenUrl { get; set; }
    }
}
