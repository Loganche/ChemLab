using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Menu.xaml
    /// </summary>

    public partial class Menu : Window
    {
        private String DbFileName;
        private SQLiteConnection M_dbConn;
        private SQLiteCommand M_sqlCmd;

        private Developers developers;
        public Menu()
        {
            InitializeComponent();

            M_dbConn = new SQLiteConnection();
            M_sqlCmd = new SQLiteCommand();

            DbFileName = "question_and_metal.db";

            try
            {
                M_dbConn = new SQLiteConnection("Data Source=" + DbFileName + "; Version=3;");
                M_dbConn.Open();
            }
            catch (Exception ignore) { }

            this.StartLab.IsEnabled = false;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                developers.Close();
            }
            catch (Exception) { }

            developers = new Developers();
            developers.Show();
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                developers.Close();
            }
            catch (Exception) { }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.textBoxName.Text.ToString() != "" && this.textBoxGroup.Text.ToString() != "" && this.textBoxEmail.Text.ToString() != "")
            {
                this.StartLab.IsEnabled = true;
                this.textBoxName.IsEnabled = false;
                this.textBoxGroup.IsEnabled = false;
                this.textBoxEmail.IsEnabled = false;
                this.ButtonApply.IsEnabled = false;

                ButtonOpenMenu.Visibility = Visibility.Collapsed;
                ButtonCloseMenu.Visibility = Visibility.Visible;

                // Change color textBox - SUCCESS
                try
                {
                    M_sqlCmd.Connection = M_dbConn;

                    M_sqlCmd.CommandText = "DELETE FROM Profile";
                    M_sqlCmd.ExecuteNonQuery();

                    M_sqlCmd.CommandText = "INSERT INTO Profile('Email', 'Group', 'Name') VALUES('" + this.textBoxEmail.Text.ToString() + "', '" + this.textBoxGroup.Text.ToString() + "', '" + this.textBoxName.Text.ToString() + "')";
                    M_sqlCmd.ExecuteNonQuery();

                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

    }
}
