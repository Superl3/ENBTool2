using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ENBTool
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public class disp
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public disp(string key, string value)
            {
                Key = key;
                Value = value;
            }
        }
        public ObservableCollection<disp> lsINI = new ObservableCollection<disp>();


        public MainWindow()
        {
            InitializeComponent();
        }
        Dictionary<string, Dictionary<string, string>> Files = new Dictionary<string, Dictionary<string, string>>();
        List<string> FileName = new List<string>();
        string Path = "";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Path = ___Path.Text;
            string[] enbseries = File.ReadAllLines(Path + "\\enbseries.ini");
            if (enbseries.Length == 0) return;
            Path += "\\enbseries\\";
            fndFiles();
            readFiles();
            loadFiles();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }
        private void fndFiles()
        {
            string[] weather = File.ReadAllLines(Path + "_weatherlist.ini");
            foreach (string line in weather)
            {
                if (line.Contains("FileName="))
                {
                    int startIdx = line.IndexOf('=') + 1;
                    string val = line.Substring(startIdx).Trim();
                    if (val == "")
                        break;
                    FileName.Add(val);
                }
            }
        }
        private void readFiles()
        {
            FileName.Add("..\\enbseries.ini");
            foreach (string f in FileName)
            {
                string[] contents = File.ReadAllLines(Path + f);
                string categories = "";
                foreach (string line in contents)
                {
                    if (line.Contains('['))
                        categories = line;
                    if (line.Contains('='))
                    {
                        int startIdx = line.IndexOf('=');
                        string key = line.Substring(0, startIdx).Trim();
                        string val = line.Substring(startIdx + 1).Trim();
                        Dictionary<string, string> item = new Dictionary<string, string>();
                        if (Files.ContainsKey(f))
                        {
                            item = Files[f];
                            Files.Remove(f);
                        }
                        item.Add(categories + key, val);
                        Files.Add(f, item);
                        
                    }
                }
            }
        }
        private void loadFiles()
        {
            string categories = "";
            foreach (KeyValuePair<string, string> item in Files["..\\enbseries.ini"])
            {
                if(item.Key.Contains(']'))
                {
                    string cur = item.Key.Substring(0, item.Key.IndexOf(']')+1);
                    if (cur != categories)
                    {
                        lsINI.Add(new disp(cur, ""));
                        categories = cur;
                    }
                lsINI.Add(new disp(item.Key.Substring(cur.Length), item.Value));

                }
            }
            INI.ItemsSource = lsINI;
        }
    }
}
