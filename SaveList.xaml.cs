using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ENBTool
{
    /// <summary>
    /// SaveList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SaveList : Window
    {
        public class File
        {
            public bool Checked
            {
                get; set;
            }
            public string Name
            {
                get; set;
            }
            public File(string Name)
            {
                this.Checked = false;
                this.Name = Name;
            }
        }
        public ObservableCollection<File> FileList = new ObservableCollection<File>();
        public SaveList(List<string> list)
        {
            InitializeComponent();
            foreach (string item in list)
                FileList.Add(new File(item));
            FileList_DataGrid.ItemsSource = FileList;
        }

        private void unchk_Click(object sender, RoutedEventArgs e)
        {
            foreach (File f in FileList)
                f.Checked = false;
            FileList_DataGrid.Items.Refresh();
        }

        private void chk_Click(object sender, RoutedEventArgs e)
        {
            foreach (File f in FileList)
                f.Checked = true;
            FileList_DataGrid.Items.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            saveFiles.Clear();
            this.Close();
        }

        public List<string> saveFiles = new List<string>();

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            saveFiles.Clear();
            foreach(File f in FileList)
            {
                if (f.Checked == true)
                    saveFiles.Add(f.Name);
            }
            this.Close();
        }
    }
}
