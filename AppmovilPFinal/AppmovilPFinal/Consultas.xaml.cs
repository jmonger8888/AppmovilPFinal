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
    public partial class Consultas : ContentPage
    {
        //clases para desarrollar metodos
        public FirebaseClient firebaseClient;//interactuar con firebase
        public List<Attendance> absencesList;//cargar lista de ausencias
        public Consultas()
        {
            //se incian los componentes osea el constructor
            InitializeComponent();
            firebaseClient = new FirebaseClient("https://proyectofinalmovil-ed3f4-default-rtdb.firebaseio.com");
            absencesList = new List<Attendance>();
        }

        //Método para realizar la consulta
        public async void OnConsultClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(studentCodeEntry.Text))
            {
                var ausencias = await firebaseClient
                    .Child("Ausencias")
                    .OrderBy("Code")
                    .EqualTo(studentCodeEntry.Text)
                    .OnceAsync<AppmovilPFinal.Attendance>();

                absencesList = ausencias.Select(item => new AppmovilPFinal.Attendance
                {
                    StudentName = item.Object.StudentName,
                    Date = item.Object.Date,
                    IsJustified = item.Object.IsJustified
                }).ToList();

                absencesListView.ItemsSource = absencesList;
            }
            else
            {
                await DisplayAlert("Error", "Por favor, ingresa un código de estudiante válido", "OK");
            }
        }
    }
}