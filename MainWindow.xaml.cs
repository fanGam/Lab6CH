using System.Diagnostics;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.IO.Compression;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using VersOne.Epub;

namespace Lab6CH
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string direct;
        private string appPath;
        private string bookpath;
        private string bookname;
        private EpubBook book;
        private bool BP = false;
        public MainWindow()
        {
            //проверяем на наличие сохранённой директории
            appPath = Directory.GetCurrentDirectory();
            try
            {
                using (var Stream = File.Open("path.txt", FileMode.Open))
                {
                    using (var reader = new StreamReader(Stream))
                    {
                        direct = reader.ReadLine();
                    }
                }
            }
            catch
            {

            }
            InitializeComponent();
            if (direct != null)
            {
                Lable2.Content = direct;
            }
        }
        //Метод, который позволяет выбрать пользователю директорию и запоминает её
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFolderDialog
            Microsoft.Win32.OpenFolderDialog dlg = new Microsoft.Win32.OpenFolderDialog();
            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a Label
            if (result == true)
            {
                // Open document
                direct = dlg.FolderName;
                Lable2.Content = direct;
                Directory.SetCurrentDirectory(appPath);
                using (var Stream = File.Open("path.txt", FileMode.OpenOrCreate))
                {
                    using (var writer = new StreamWriter(Stream))
                    {
                        writer.Write(direct);
                        Lable2.Content = direct;
                    }
                }
                Directory.SetCurrentDirectory(direct);
                button2.IsEnabled = false;
                button3.IsEnabled = true;
            }
        }
        //Метод который перемещает файл в выбранную ддиректорию при этом проверяя наличие в папке уже этого автора
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (direct != null || book != null)
            {
                try
                {
                    var Files = Directory.EnumerateDirectories(direct);
                    foreach (string currentDir in Files)
                    {
                        string DirName = currentDir.Substring(direct.Length + 1);
                        if (book.Author == DirName)
                        {
                            Directory.Move(bookpath, Path.Combine($"{currentDir}", $"{book.Author}-{book.Title}.epub"));
                            BP = true; break;
                        }
                    }
                    if (!BP)
                    {
                        Directory.SetCurrentDirectory(direct);
                        Directory.CreateDirectory(book.Author);
                        Directory.Move(bookpath, Path.Combine($@"{direct}\{book.Author}", $"{book.Author}-{book.Title}.epub"));
                    }
                }
                catch
                {

                }
                button3.IsEnabled = false;
                button2.IsEnabled = false;
                button1.IsEnabled = true;
            }
            else
            {
                if (direct == null)
                {

                    Lable2.Foreground = Brushes.Red;
                }
                if (book == null)
                {
                    Lable1.Foreground = Brushes.Red;
                }
            }
        }
        //Метод который возвращает редительскую директорию
        private string GetDir(string filepath)
        {
            string newdir = "";
            int i = filepath.Length - 1;
            while (filepath[i] != ((char)92))
            {
                i--;
            }
            newdir = filepath.Substring(0, i);
            return newdir;
        }
        //Метод, который находит файл и запоминает путь до него и его имя при этом создаёт EPub объект
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".epub";
            dlg.Filter = "EPUB Files (*.epub)|*.epub";
            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open files
                bookpath = dlg.FileName;
                bookname = dlg.SafeFileName;
                Directory.SetCurrentDirectory(GetDir(bookpath));
                try
                {
                    book = EpubReader.ReadBook(bookname);
                    Lable1.Content = $"{book.Author}-{book.Title}";
                    button1.IsEnabled = false;
                    button2.IsEnabled = true;
                    if (direct != null)
                    {
                        button3.IsEnabled = true;
                    }
                    Lable1.Foreground = Brushes.Black;
                }
                catch
                {
                    Lable1.Foreground = Brushes.Red;
                    Lable1.Content = "Something went wrong!\nTry again!";
                }
            }
        }
    }
}