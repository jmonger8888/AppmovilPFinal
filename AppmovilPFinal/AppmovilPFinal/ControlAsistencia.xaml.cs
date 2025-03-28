﻿using System;
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
    public partial class ControlAsistencia : ContentPage
    {
        //clases para los metodos
        public FirebaseClient firebaseClient;//interaccion con firebase
        public List<Attendance> attendanceList;//lista de asistencia

        public ControlAsistencia()
        {
            InitializeComponent();
            firebaseClient = new FirebaseClient("https://proyectofinalmovil-ed3f4-default-rtdb.firebaseio.com");
            attendanceList = new List<Attendance>();
            CargarAusencias();
        }

        //Método para cargar las ausencias desde Firebase por si ya hay existentes en la base de dtos
        public async void CargarAusencias()
        {
            var ausencias = await firebaseClient
                .Child("Ausencias")
                .OnceAsync<Attendance>();

            attendanceList = ausencias.Select(item => new Attendance
            {
                Id = item.Key,
                Code = item.Object.Code,
                StudentName = item.Object.StudentName,
                Date = item.Object.Date,
                Time = item.Object.Time,
                IsJustified = item.Object.IsJustified
            }).ToList();

            attendanceListView.ItemsSource = attendanceList;
        }

        //Método para registrar una nueva ausencia en la base de datos y mostrarla en la lista
        public async void OnAddAbsenceClicked(object sender, EventArgs e)
        {
            var newAttendance = new Attendance
            {
                Code = studentCodeEntry.Text,
                StudentName = studentNameEntry.Text,
                Date = absenceDatePicker.Date,
                Time = absenceTimePicker.Time,
                IsJustified = false
            };

            var addedAttendance = await firebaseClient
                .Child("Ausencias")
                .PostAsync(newAttendance);

            newAttendance.Id = addedAttendance.Key; 

            attendanceList.Add(newAttendance);
            attendanceListView.ItemsSource = null;  
            attendanceListView.ItemsSource = attendanceList;

            LimpiarFormulario();

            //mensaje de confirmación
            await DisplayAlert("Éxito", "Ausencia registrada correctamente", "OK");
        }
        //Método para eliminar una ausencia
        public async void OnDeleteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var attendanceToDelete = button?.CommandParameter as Attendance;

            if (attendanceToDelete != null)
            {
                bool isConfirmed = await DisplayAlert("Confirmación", "¿Estás seguro de que deseas eliminar esta ausencia?", "Sí", "No");
                if (isConfirmed)
                {
                    // Eliminar de Firebase
                    await firebaseClient
                        .Child("Ausencias")
                        .Child(attendanceToDelete.Id)
                        .DeleteAsync();

                    // Eliminar de la lista en la app
                    attendanceList.Remove(attendanceToDelete);
                    attendanceListView.ItemsSource = null;
                    attendanceListView.ItemsSource = attendanceList;

                    await DisplayAlert("Éxito", "Ausencia eliminada correctamente", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo eliminar la ausencia", "OK");
            }
        }

        //Método para justificar una ausencia seleccionada
        public async void OnJustifyAbsenceClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(absenceCodeEntry.Text))
            {
                var absenceToJustify = attendanceList.FirstOrDefault(a => a.Code == absenceCodeEntry.Text);
                if (absenceToJustify != null)
                {
                    absenceToJustify.IsJustified = true;

                    //Actualizar en Firebase
                    await firebaseClient
                        .Child("Ausencias")
                        .Child(absenceToJustify.Id)
                        .PutAsync(absenceToJustify);

                    attendanceListView.ItemsSource = null;
                    attendanceListView.ItemsSource = attendanceList;
                    await DisplayAlert("Éxito", "Ausencia justificada correctamente", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "No se encontró la ausencia con ese código", "OK");
                }

                absenceCodeEntry.Text = string.Empty;
            }
            else
            {
                await DisplayAlert("Error", "Por favor, ingresa un código de ausencia válido", "OK");
            }
        }

        //Método para seleccionar una ausencia de la lista y cargar su información en el campo de justificación
        public void OnAttendanceSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var selectedAttendance = e.SelectedItem as Attendance;
                absenceCodeEntry.Text = selectedAttendance.Code;

                // Limpiar selección
                attendanceListView.SelectedItem = null;
            }
        }

        //Método para limpiar el formulario
        public void LimpiarFormulario()
        {
            studentCodeEntry.Text = string.Empty;
            studentNameEntry.Text = string.Empty;
            absenceDatePicker.Date = DateTime.Now;
            absenceTimePicker.Time = TimeSpan.Zero;
        }
    }

    //Modelo de datos para Attendance
    public class Attendance
    {
        public string Id { get; set; }  
        public string Code { get; set; }
        public string StudentName { get; set; } 
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public bool IsJustified { get; set; }
    }
}