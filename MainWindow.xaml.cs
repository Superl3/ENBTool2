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
            public string Value {
                get;set;
            }
            public string RealKey;
            public disp(string key, string value, string realkey)
            {
                Key = key;
                Value = value;
                RealKey = realkey;
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
            FileName.Clear();
            Files.Clear();
            lsINI.Clear();
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
            List<disp> changeList = new List<disp>();
            string msg = "";
            foreach (disp d in lsINI)
            {
                if (d.RealKey == "") continue;
                if (Files["..\\enbseries.ini"][d.RealKey] != d.Value)
                {
                    changeList.Add(d);
                    msg += d.Key + "\n";
                }
            }
            MessageBoxResult ask = MessageBox.Show("Following Items are modified. : \n" + msg + "press OK to save the progress.", "Confirmation", MessageBoxButton.YesNo);
            if (ask != MessageBoxResult.Yes)
                return;
            SaveList res = new SaveList(FileName);
            res.ShowDialog();

            foreach (string f in res.saveFiles)
            {
                write_progress(f,changeList);
            }
        }

        void write_progress(string key, List<disp> change)
        {
            Dictionary<string, string> Data = Files[key];
            
            foreach(disp d in change)
                Data[d.RealKey] = d.Value;

            List<string> raw = new List<string>();
            string categories = "";

            foreach (KeyValuePair<string, string> item in Data)
            {
                if (!item.Key.Contains(']'))
                    continue;
                int idx = item.Key.IndexOf(']') + 1;
                string cur = item.Key.Substring(0, idx);
                if (cur != categories)
                {
                    raw.Add(cur);
                    categories = cur;
                }
                raw.Add(item.Key.Substring(cur.Length) + '=' + item.Value);
            }

            File.WriteAllLines(Path + key, raw);
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
                if (item.Key.Contains(']'))
                {
                    string cur = item.Key.Substring(0, item.Key.IndexOf(']') + 1);
                    if (cur != categories)
                    {
                        lsINI.Add(new disp(cur, "", ""));
                        categories = cur;
                    }
                    lsINI.Add(new disp(item.Key.Substring(cur.Length), item.Value, item.Key));
                }
            }
            INI.ItemsSource = lsINI;
            INI.Columns[0].IsReadOnly = true;
            INI.ColumnWidth = DataGridLength.SizeToCells;
        }
    }
}
