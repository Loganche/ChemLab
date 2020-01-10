using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        private int Answers_i = 0;
        private String DbFileName;
        private String email_from = "chemistrylabnum2@gmail.com";
        private String email_password = "chemlabnum2";
        private String email_to;
        private String result;
        private String theme = "Лабораторная работа по химии №2";
        private String text_message;
        private String name;
        private String group;
        private SQLiteConnection M_dbConn;
        private SQLiteCommand M_sqlCmd;

        private class Questions
        {
            public String Question;
            public List<String> answers = new List<String>();
            public int correct;
            public bool good = false;

            public Questions(String question, List<String> answer, int correct_answer)
            {
                Question = question;
                answers = answer;
                correct = correct_answer;
            }
        }

        private List<Questions> questions = new List<Questions>();

        bool StringToBool(String str)
        {
            if (str.Equals("1"))
                return true;
            else
                return false;
        }

        public Test(String _result)
        {
            InitializeComponent();

            DataTable dataTable = new DataTable();

            M_dbConn = new SQLiteConnection();
            M_sqlCmd = new SQLiteCommand();

            DbFileName = "question_and_metal.db";

            try
            {
                M_dbConn = new SQLiteConnection("Data Source=" + DbFileName + "; Version=3;");
                M_dbConn.Open();

                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM Questions", M_dbConn);

                dataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataTable data = new DataTable();

                    dataAdapter = new SQLiteDataAdapter("SELECT Answer.answer, Answer.correct FROM Answer WHERE id = " + dataTable.Rows[i].ItemArray.GetValue(1).ToString(), M_dbConn);

                    dataAdapter.Fill(data);

                    List<String> answers = new List<string>();
                    int answer = -1;
                    String question = dataTable.Rows[i].ItemArray.GetValue(0).ToString();

                    for (int j = 0; j < data.Rows.Count; j++)
                    {
                        answers.Add(data.Rows[j].ItemArray.GetValue(0).ToString());

                        if (StringToBool(data.Rows[j].ItemArray.GetValue(1).ToString()) == true)
                            answer = j;
                    }

                    questions.Add(new Questions(question, answers, answer));
                }

                this.Question.Text = questions[Answers_i].Question;

                Random random = new Random();

                for (int i = 3; i >= 1; i--)
                {
                    int j = random.Next() % (i + 1);
                    var temp = questions[Answers_i].answers[j];

                    if (questions[Answers_i].correct == j)
                        questions[Answers_i].correct = i;
                    else if (questions[Answers_i].correct == i)
                        questions[Answers_i].correct = j;
                    questions[Answers_i].answers[j] = questions[Answers_i].answers[i];
                    questions[Answers_i].answers[i] = temp;
                }

                this.radio1Text.Text = questions[Answers_i].answers[0];
                this.radio2Text.Text = questions[Answers_i].answers[1];
                this.radio3Text.Text = questions[Answers_i].answers[2];
                this.radio4Text.Text = questions[Answers_i].answers[3];

                SQLiteDataAdapter dataAdapterEmail = new SQLiteDataAdapter("SELECT * FROM Profile", M_dbConn);
                
                dataTable = new DataTable();

                dataAdapterEmail.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    name = dataTable.Rows[i].ItemArray.GetValue(0).ToString();
                    email_to = dataTable.Rows[i].ItemArray.GetValue(1).ToString();
                    group = dataTable.Rows[i].ItemArray.GetValue(2).ToString();
                    result = _result;
                }
            }
       
            catch (SQLiteException ignore) { }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.Radio1.IsChecked == true || this.Radio2.IsChecked == true || this.Radio3.IsChecked == true || this.Radio4.IsChecked == true)
            {
                switch (questions[Answers_i].correct)
                {
                    case 0:
                        if (this.Radio1.IsChecked == true)
                            questions[Answers_i].good = true;
                        break;
                    case 1:
                        if (this.Radio2.IsChecked == true)
                            questions[Answers_i].good = true;
                        break;
                    case 2:
                        if (this.Radio3.IsChecked == true)
                            questions[Answers_i].good = true;
                        break;
                    case 3:
                        if (this.Radio4.IsChecked == true)
                            questions[Answers_i].good = true;
                        break;
                }

                if (Answers_i == questions.Count - 1)
                {
                    int answer = 0;
                    for (int i = 0; i < questions.Count; i++)
                        if (questions[i].good == true)
                            answer++;

                    if (System.Windows.MessageBox.Show("Верно " + answer.ToString() + " из " + questions.Count.ToString() + "\nВаш результат отправлен на почту: " + email_to, "Ваш результат", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        MailAddress from = new MailAddress("chemistrylabnum2@gmail.com", "");
                        
                        MailAddress to = new MailAddress(email_to);
                        
                        MailMessage m = new MailMessage(from, to);
                        
                        m.Subject = theme;
                        
                        m.Body = "Студент " + name + " группы №" + group + " выполнил подготовку по лабораторной работе №2 \nПогрешность: " +
                            result + "%\nРезультат теста: " + answer.ToString() + " из " + questions.Count.ToString();
                        
                        m.IsBodyHtml = true;
                        
                        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                        
                        smtp.Credentials = new NetworkCredential("chemistrylabnum2@gmail.com", "chemlabnum2");
                        smtp.EnableSsl = true;
                        try
                        {
                            smtp.Send(m);
                        }
                        catch (Exception ex)
                        {

                        }

                        Menu menu = new Menu();

                        menu.Show();

                        this.Close();
                    }
                }
                else
                {
                    Answers_i++;

                    this.Question.Text = questions[Answers_i].Question;

                    Random random = new Random();

                    for (int i = 3; i >= 1; i--)
                    {
                        int j = random.Next() % (i + 1);
                        var temp = questions[Answers_i].answers[j];

                        if (questions[Answers_i].correct == j)
                            questions[Answers_i].correct = i;
                        else if (questions[Answers_i].correct == i)
                            questions[Answers_i].correct = j;
                        questions[Answers_i].answers[j] = questions[Answers_i].answers[i];
                        questions[Answers_i].answers[i] = temp;
                    }

                    this.radio1Text.Text = questions[Answers_i].answers[0];
                    this.radio2Text.Text = questions[Answers_i].answers[1];
                    this.radio3Text.Text = questions[Answers_i].answers[2];
                    this.radio4Text.Text = questions[Answers_i].answers[3];

                    this.Radio1.IsChecked = false;
                    this.Radio2.IsChecked = false;
                    this.Radio3.IsChecked = false;
                    this.Radio4.IsChecked = false;

                    if (Answers_i == questions.Count - 1)
                        this.ButtonNext.Content = "Готово";
                }
            }
            else System.Windows.MessageBox.Show("Выберите ответ");
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
