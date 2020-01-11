using System;
using System.Collections.Generic;
using System.Data;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for ProtocolZnach.xaml
    /// </summary>
    public partial class ProtocolZnach : Window
    {
        public ProtocolZnach()
        {
            InitializeComponent();
        }

        public int myMetal = -1;
        public DataTable dataTable = new DataTable();
        public ProtocolZnach(int _myMetal, DataTable _dataTable, int temperature, double dav)
        {
            InitializeComponent();

            myMetal = _myMetal;
            dataTable = _dataTable;

            this.m.Text = dataTable.Rows[myMetal].ItemArray.GetValue(1).ToString();
            this.V1.Text = dataTable.Rows[myMetal].ItemArray.GetValue(3).ToString();
            this.dav_text.Text = dav.ToString();
            this.temperature_text.Text = (273 + temperature).ToString();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
