﻿using System;
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
    /// Логика взаимодействия для PROTOCOL.xaml
    /// </summary>
    public partial class PROTOCOL : Window
    {
        public int myMetal = -1;
        public int temperature = -1;
        public double dav = -1;
        public double davH = -1;
        public DataTable dataTable = new DataTable();
        private TableCheck tableCheck;
        private CheckData checkData;
        public PROTOCOL(int _myMetal, DataTable _dataTable, int _temperature)
        {
            InitializeComponent();

            myMetal = _myMetal;
            dataTable = _dataTable;
            temperature = _temperature;

            this.m.Text = dataTable.Rows[myMetal].ItemArray.GetValue(1).ToString();
            this.V1.Text = dataTable.Rows[myMetal].ItemArray.GetValue(3).ToString();
            this.temperature_text.Text = "T(опыта) = " + temperature.ToString() + " °С";

            switch (temperature)
            {
                case 18:
                    this.dav_text.Text = "P(водяного пара) = 2063,5 Па";
                    dav = 2063.5;
                    davH = 99377.8;
                    break;
                case 21:
                    this.dav_text.Text = "P(водяного пара) = 2486,0 Па";
                    dav = 2486;
                    davH = 98955.3;
                    break;
                case 23:
                    this.dav_text.Text = "P(водяного пара) = 2808,6 Па";
                    dav = 2808.6;
                    davH = 98632.7;
                    break;
                case 25:
                    this.dav_text.Text = "P(водяного пара) = 3167,2 Па";
                    dav = 3167.2;
                    davH = 98274.1;
                    break;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tableCheck.Close();
            }
            catch (Exception ignore) { }
            tableCheck = new TableCheck();
            tableCheck.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.T.Text == (273 + temperature).ToString() && (this.Patm.Text == "101441.3" || this.Patm.Text == "101441,3") && double.Parse(this.Ph20.Text, System.Globalization.CultureInfo.InvariantCulture) == dav && double.Parse(this.Ph.Text, System.Globalization.CultureInfo.InvariantCulture) == davH)
            {
                End end = new End(myMetal, dataTable, temperature, dav, davH);
                end.Show();
                this.Close();
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
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
