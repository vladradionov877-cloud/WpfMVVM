using Bogus;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using WpfMVVM.Model;

namespace WpfMVVM
{
    /// <summary>   
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDark = true;

        // Для зміни теми вікна-заголовка
        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_CAPTION_COLOR = 35; // Працює у Windows 11

        private readonly ObservableCollection<UserVM> users = new();

        public MainWindow()
        {
            InitializeComponent();

            // По замовчуванню буде темна тема
            ApplyTheme(_isDark);
            dgvUsers.ItemsSource = users;
        }

        // Палітри кольорів для обох тем
        // Світла тема
        private static readonly Dictionary<string, string> LightTheme = new()
        {
            ["BgBrush"] = "#F5F7FA",
            ["SurfaceBrush"] = "#FFFFFF",
            ["AltRowBrush"] = "#F9FAFB",
            ["HeaderBrush"] = "#EFF6FF",
            ["TextBrush"] = "#1F2937",
            ["AppBorderBrush"] = "#E5E7EB",
            ["AccentBrush"] = "#2563EB",
            ["AccentHoverBrush"] = "#1D4ED8",
            ["AccentTextBrush"] = "#FFFFFF",
            ["HoverBrush"] = "#EFF6FF",
        };

        // Темна тема
        private static readonly Dictionary<string, string> DarkTheme = new()
        {
            ["BgBrush"] = "#000000",
            ["SurfaceBrush"] = "#0B0B0B",
            ["AltRowBrush"] = "#121212",
            ["HeaderBrush"] = "#181818",
            ["TextBrush"] = "#F3F3F3",
            ["AppBorderBrush"] = "#242424",
            ["AccentBrush"] = "#3B82F6",
            ["AccentHoverBrush"] = "#2563EB",
            ["AccentTextBrush"] = "#000000",
            ["HoverBrush"] = "#1C1C1C",
        };

        // Кнопка "Додати"
        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            var faker = new Faker("uk");
            int nextId = users.Count + 1;
            int avatarSeed = faker.Random.Int(1, 70);

            UserVM newUser = new UserVM
            {
                Id = nextId,
                Name = faker.Name.FullName(),
                Phone = faker.Phone.PhoneNumber("+380 ## ### ####"),
                Image = $"https://i.pravatar.cc/1200?img={avatarSeed}&v={Guid.NewGuid():N}"
            };

            users.Add(newUser);
        }

        // Кнопка зміни теми
        private void btnTheme_Click(object sender, RoutedEventArgs e)
        {
            _isDark = !_isDark;
            ApplyTheme(_isDark);
        }

        // Ф-ція зміни теми
        private void ApplyTheme(bool dark)
        {
            var palette = dark ? DarkTheme : LightTheme;

            foreach (var item in palette)
            {
                // Підміняємо весь ресурс - усі DynamicResource оновляться автоматично
                var color = (Color)ColorConverter.ConvertFromString(item.Value);
                Resources[item.Key] = new SolidColorBrush(color);
            }

            // Зміна теми вікна-заголовка
            ApplyTitleBarTheme(dark, palette["BgBrush"]);

            // Зміна тексту кнопки відповідно до теми
            btnTheme.Content = dark ? "☀️  Світла тема" : "🌙  Темна тема";
        }

        // Ф-ція зміни теми вікна-заголовка
        private void ApplyTitleBarTheme(bool dark, string captionHexColor)
        {
            IntPtr hwnd = new WindowInteropHelper(this).EnsureHandle();

            // Темний режим системної рамки та кнопок вікна (Win 10/11)
            int darkMode = dark ? 1 : 0;
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkMode, sizeof(int));

            // Точний колір фону заголовка під колір вікна (Win 11)
            var color = (Color)ColorConverter.ConvertFromString(captionHexColor);
            int colorRef = color.R | (color.G << 8) | (color.B << 16);
            DwmSetWindowAttribute(hwnd, DWMWA_CAPTION_COLOR, ref colorRef, sizeof(int));
        }
    }
}