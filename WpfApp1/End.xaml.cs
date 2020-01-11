using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
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
    /// Interaction logic for End.xaml
    /// </summary>
    public partial class End : Window
    {
        private TableCheck tableCheck;
        private ProtocolZnach protocolZnach;
        public int myMetal = -1;
        public double dav = -1;
        public double davH = -1;
        public int temperature = -1;
        public DataTable dataTable = new DataTable();
        private CheckData checkData;
        private ChecDataMetal checDataMetal;

        public End(int _myMetal, DataTable _dataTable, int _temperature, double _dav, double _davH)
        {
            InitializeComponent();

            myMetal = _myMetal;
            dataTable = _dataTable;
            temperature = _temperature;
            dav = _dav;
            davH = _davH;
        }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                tableCheck.Close();
            }
            catch (Exception ignore) { }
            tableCheck = new TableCheck();
            tableCheck.Show();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                protocolZnach.Close();
            }
            catch (Exception ignore) { }

            protocolZnach = new ProtocolZnach(myMetal, dataTable, temperature, dav, davH);
            protocolZnach.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double V0_need = 0;
            double Mek_need = 0;
            double Mat_need = 0;
            double Mat_dataBase = Convert.ToDouble(dataTable.Rows[myMetal].ItemArray.GetValue(2).ToString());
            int Z_dataBase = Convert.ToInt32(dataTable.Rows[myMetal].ItemArray.GetValue(4).ToString());
            double M_dataBase = Convert.ToDouble(dataTable.Rows[myMetal].ItemArray.GetValue(1).ToString());
            double V_dataBase = Convert.ToDouble(dataTable.Rows[myMetal].ItemArray.GetValue(3).ToString());
            String name_metal = dataTable.Rows[myMetal].ItemArray.GetValue(0).ToString();

            V0_need = ((davH * V_dataBase) / (273.0 + temperature)) * (273.0 / 101325.0);
            Mek_need = (M_dataBase * 11.2) / V0_need;
            Mat_need = Z_dataBase * Mek_need;

            if ((Double.Parse(V0.Text.ToString(), CultureInfo.InvariantCulture) <= V0_need * 1.03 && Double.Parse(V0.Text.ToString(), CultureInfo.InvariantCulture) >= V0_need * 0.97) && Double.Parse(Mek.Text.ToString(), CultureInfo.InvariantCulture) <= Mek_need * 1.035 && Double.Parse(Mek.Text.ToString(), CultureInfo.InvariantCulture) >= Mek_need * 0.975 && Double.Parse(Mat.Text.ToString(), CultureInfo.InvariantCulture) <= Mat_need * 1.05 && Double.Parse(Mat.Text.ToString(), CultureInfo.InvariantCulture) >= Mat_need * 0.95)
            {
                if (String.Equals(this.Met.Text.ToString(), name_metal))
                {
                    if (MessageBox.Show("Ваш металл: " + name_metal + "\nМат = " + Mat.Text.ToString() + "\nВаша погрешность = " + (Math.Abs(float.Parse(Mat.Text.ToString(), CultureInfo.InvariantCulture) - Mat_dataBase) / Mat_dataBase) * 100 + "%", "Результат") == MessageBoxResult.OK)
                    {
                        try
                        {
                            tableCheck.Close();
                        }
                        catch (Exception ignore) { }

                        try
                        {
                            protocolZnach.Close();
                        }
                        catch (Exception ignore) { }

                        Test test = new Test(((Math.Abs(float.Parse(Mat.Text.ToString(), CultureInfo.InvariantCulture) - Mat_dataBase) / Mat_dataBase) * 100).ToString());

                        test.Show();

                        this.Close();
                    }
                }
                else
                {
                    try
                    {
                        checDataMetal.Close();
                    }
                    catch (Exception ignore) { }
                    checDataMetal = new ChecDataMetal();
                    checDataMetal.Show();
                };
            }
            else
            {
                try
                {
                    checkData.Close();
                }
                catch (Exception ignore) { }
                checkData = new CheckData();
                checkData.Show();
            };


        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            protocolZnach = new ProtocolZnach(myMetal, dataTable, temperature, dav, davH);
            protocolZnach.Show();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
