using System.Windows;
using StudentClinicMIS.Models; // сюда scaffold добавил модели
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace StudentClinicMIS
{
    public partial class MainWindow : Window
    {
        private readonly PolyclinicContext _context;

        public MainWindow()
        {
            InitializeComponent();

            // получаем контекст из DI (если используешь Host), или напрямую
            _context = new PolyclinicContext(); // если контекст с дефолтным конструктором

            LoadPatients();
        }

        private async void LoadPatients()
        {
            var patients = await _context.Patients.ToListAsync();
            PatientsGrid.ItemsSource = patients;
        }
    }
}
