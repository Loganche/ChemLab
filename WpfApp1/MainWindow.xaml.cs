using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Windows.Threading;
using Microsoft.VisualStudio.PlatformUI;
using System.Data.SQLite;
using System.Data;

namespace WpfApp1
{   
    public partial class MainWindow : System.Windows.Window
    {
        private String DbFileName;
        private SQLiteConnection M_dbConn;
        private SQLiteCommand M_sqlCmd;
        private RenderWindow _renderWindow;
        private SFML.System.Clock clock = new Clock();
        private int stage = 1;
        private List<Item> items = new List<Item>();
        private Sprite sprite_temp = new Sprite();
        private Texture texture_temp;
        private bool complete_stand = false;
        private Vector2f pos;
        private bool isMove = false;
        private int myMetal = -1;
        private int temperature = -1;
        private RectangleShape rec;
        private float dX = 0;
        private float dY = 0;
        private int iterator_move;
        private Text measurement;
        private float reaction = 0;
        private DataTable dataTable;
        private readonly DispatcherTimer _timer;

        class Item
        {
            private Texture texture;
            private SFML.Graphics.Image image;
            private String name;
            private bool object_movement;
	        public Sprite sprite = new Sprite();
            public Item(String filePath, Vector2f _position, IntRect rect, bool _object_movement, String _name)
            {
                object_movement = _object_movement;
                name = _name;

                image = new SFML.Graphics.Image(filePath);
                image.CreateMaskFromColor(Color.White);

                texture = new Texture(image);

                sprite.Texture = texture;
                sprite.TextureRect = rect;
                sprite.Position = _position;
            }

            public bool GetObjectMovement()
            {
                return object_movement;
            }

            public void DrawSprite(ref RenderWindow window)
            {
                window.Draw(sprite);
            }

            public void SetNewSprite(String filePath, Vector2f _position, IntRect rect, bool _object_movement)
            {
                image = new SFML.Graphics.Image(filePath);
                
                image.CreateMaskFromColor(Color.White);

                texture = new Texture(image);

                sprite.Texture = texture;
                sprite.TextureRect = rect;
                sprite.Position = _position;

                object_movement = _object_movement;
            }

            public String GetName()
            {
                return name;
            }
        }

        public MainWindow()
        {
            this.InitializeComponent();

            M_dbConn = new SQLiteConnection();
            M_sqlCmd = new SQLiteCommand();

            DbFileName = "question_and_metal.db";

            M_dbConn = new SQLiteConnection("Data Source=" + DbFileName + "; Version=3;");
            M_dbConn.Open();

            dataTable = new DataTable();

            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM Metal", M_dbConn);

            dataAdapter.Fill(dataTable);

            Random random = new Random();

            myMetal = random.Next() % dataTable.Rows.Count;

            this.CreateRenderWindow();

            var refreshRate = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            this._timer = new DispatcherTimer { Interval = refreshRate };
            this._timer.Tick += Timer_Tick;
            this._timer.Start();

            Font font = new Font("calibri.TTF");

            measurement = new Text();
            //  "akjrhwelfi", font, 30
            measurement.CharacterSize = 30;
            measurement.Font = font;
            measurement.DisplayedString = dataTable.Rows[myMetal].ItemArray.GetValue(3).ToString() + " л";
            measurement.Color = Color.Black;
            measurement.Position = new Vector2f(290, 60);

            Item stand = new Item("images/640x480/Stage_1.png", new Vector2f(10, 0), new IntRect(0, 0, 285, 476), false, "stand");
            Item crystallizer = new Item("images/640x480/Crystallizer.jpg", new Vector2f(510, 20), new IntRect(0, 0, 202, 62), true, "crystallizer");
            Item eudiometer = new Item("images/640x480/Eudiometer.png", new Vector2f(725, 20), new IntRect(0, 0, 85, 263), true, "eudiometer");
            Item flask_stand = new Item("images/640x480/Flask_Stand.png", new Vector2f(510, 100), new IntRect(0, 0, 120, 149), true, "flask_stand");
            Item flask = new Item("images/640x480/Flask.png", new Vector2f(845, 20), new IntRect(0, 0, 48, 168), true, "flask");
            Item pipe = new Item("images/640x480/Pipe.png", new Vector2f(845, 20), new IntRect(0, 0, 434, 256), true, "pipe");

            items.Add(stand);
            items.Add(crystallizer);
            items.Add(eudiometer);
            items.Add(flask_stand);
            items.Add(flask);
            items.Add(pipe);

            rec = new RectangleShape(new Vector2f(220, 82));
            rec.Position = new Vector2f(100, 360);
            rec.OutlineColor = Color.Green;
            rec.OutlineThickness = 5;
            rec.FillColor = Color.Transparent;

            /*
             * image = new SFML.Graphics.Image(filePath);
                image.CreateMaskFromColor(Color.White);

                texture = new Texture(image);

                sprite.Texture = texture;
                sprite.TextureRect = rect;
                sprite.Position = _position;
             */

            SFML.Graphics.Image image;

            switch (random.Next() % 4)
            {
                case 0:
                    temperature = 18;
                    image = new SFML.Graphics.Image("images/640x480/Termometer_18.png");
                    break;
                case 1:
                    temperature = 21;
                    image = new SFML.Graphics.Image("images/640x480/Termometer_21.png");
                    break;
                case 2:
                    temperature = 23;
                    image = new SFML.Graphics.Image("images/640x480/Termometer_23.png");
                    break;
                case 3:
                    temperature = 25;
                    image = new SFML.Graphics.Image("images/640x480/Termometer_25.png");
                    break;
                default:
                    image = new SFML.Graphics.Image("images/640x480/Termometer_18.png");
                    break;
            }
            
            image.CreateMaskFromColor(Color.White);

            texture_temp = new Texture(image);

            sprite_temp.Texture = texture_temp;
            sprite_temp.TextureRect = new IntRect(0, 0, 43, 368); // 43 * 368
            sprite_temp.Position = new Vector2f(0, 0);
        }
        private void CreateRenderWindow()
        {
            if (this._renderWindow != null)
            {
                this._renderWindow.SetActive(false);
                this._renderWindow.Dispose();
            }

            var context = new ContextSettings { DepthBits = 24 };
            this._renderWindow = new RenderWindow(this.DrawSurface.Handle, context);
            this._renderWindow.MouseButtonPressed += RenderWindow_MouseButtonPressed;
            this._renderWindow.MouseButtonReleased += RenderWindow_MouseButtonReleased;
            this._renderWindow.Closed += RenderWindow_Closed;
            this._renderWindow.SetActive(true);
        }

        private void DrawSurface_SizeChanged(object sender, EventArgs e)
        {
            this.CreateRenderWindow();
        }

        private void RenderWindow_Closed(object sender, EventArgs e)
        {
            this._renderWindow.Close();
        }

        private void RenderWindow_MouseButtonReleased(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (e.Button == SFML.Window.Mouse.Button.Left)
            {
                if (isMove)
                {
                    isMove = false;
                    items[iterator_move].sprite.Color = Color.White;
                }
            }
        }

        private void RenderWindow_MouseButtonPressed(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (e.Button == SFML.Window.Mouse.Button.Left)
            {
                if (!complete_stand)
                {
                    for (int it = 0; it < items.Count; it++)
                    {
                        if (items[it].sprite.GetGlobalBounds().Contains(pos.X, pos.Y))
                            if (items[it].GetObjectMovement())
                            {
                                dX = pos.X - items[it].sprite.Position.X;
                                dY = pos.Y - items[it].sprite.Position.Y;
                                iterator_move = it;
                                isMove = true;
                                break;
                            }
                    }
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            float time = clock.ElapsedTime.AsMicroseconds();
            clock.Restart();
            time = time / 800;

            Vector2i pixelPos = SFML.Window.Mouse.GetPosition(_renderWindow);
            pos = _renderWindow.MapPixelToCoords(pixelPos);


            if (stage == 7)
            {
                reaction += time;

                if (reaction >= 5000)
                {
                    items[0].SetNewSprite("images/640x480/Stage_8.png", new Vector2f(10, 0), new IntRect(0, 0, 640, 476), false);
                    stage++;

                    this.Button2.IsEnabled = true;
                }
            }

            if (isMove)
            {
                items[iterator_move].sprite.Color = Color.Green;
                items[iterator_move].sprite.Position = new Vector2f(pos.X - dX, pos.Y - dY);

                if (items[iterator_move].sprite.GetGlobalBounds().Intersects(rec.GetGlobalBounds()))
                {
                    bool check = false;

                    switch (stage)
                    {
                        case 1:
                            if (items[iterator_move].GetName() == "crystallizer")
                            {
                                items.Remove(items[iterator_move]);

                                iterator_move = 0;

                                items[iterator_move].SetNewSprite("images/640x480/Stage_2.png", new Vector2f(10, 0), new IntRect(0, 0, 380, 476), false);

                                rec.Size = new Vector2f(130, 273);
                                rec.Position = new Vector2f(130, 103);

                                check = true;
                            }
                            break;
                        case 2:
                            if (items[iterator_move].GetName() == "eudiometer")
                            {
                                items.Remove(items[iterator_move]);

                                iterator_move = 0;

                                items[iterator_move].SetNewSprite("images/640x480/Stage_3.png", new Vector2f(10, 0), new IntRect(0, 0, 380, 476), false);

                                rec.Size = new Vector2f(140, 165);
                                rec.Position = new Vector2f(507, 321);

                                check = true;
                            }
                            break;
                        case 3:
                            if (items[iterator_move].GetName() == "flask_stand")
                            {
                                items.Remove(items[iterator_move]);

                                iterator_move = 0;

                                items[iterator_move].SetNewSprite("images/640x480/Stage_4.png", new Vector2f(10, 0), new IntRect(0, 0, 640, 476), false);

                                check = true;
                            }
                            break;
                        case 4:
                            if (items[iterator_move].GetName() == "flask")
                            {
                                items.Remove(items[iterator_move]);

                                iterator_move = 0;

                                items[iterator_move].SetNewSprite("images/640x480/Stage_5.png", new Vector2f(10, 0), new IntRect(0, 0, 640, 476), false);

                                rec.Size = new Vector2f(440, 270);
                                rec.Position = new Vector2f(190, 160);

                                check = true;
                            }
                            break;
                        case 5:
                            if (items[iterator_move].GetName() == "pipe")
                            {
                                items.Remove(items[iterator_move]);

                                iterator_move = 0;

                                items[iterator_move].SetNewSprite("images/640x480/Stage_6.png", new Vector2f(10, 0), new IntRect(0, 0, 640, 476), false);

                                check = true;
                                complete_stand = true;

                                this.Button1.IsEnabled = true;
                            }
                            break;
                        default:
                            break;
                    }

                    if (check)
                    {
                        isMove = false;

                        stage++;
                    }
                }
            }


            this._renderWindow.DispatchEvents();

            this._renderWindow.Clear(Color.White);

            for (int it = 0; it < items.Count; it++)
                items[it].DrawSprite(ref this._renderWindow);

            if (isMove)
                this._renderWindow.Draw(rec);

            if (stage == 9)
                this._renderWindow.Draw(measurement);

            this._renderWindow.Draw(sprite_temp);
            this._renderWindow.Display();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            items[0].SetNewSprite("images/640x480/Stage_7.png", new Vector2f(10, 0), new IntRect(0, 0, 640, 476), false);
            stage++;

            this.Button1.IsEnabled = false;
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            items[0].SetNewSprite("images/640x480/Stage_9.png", new Vector2f(10, 0), new IntRect(0, 0, 640, 476), false);
            stage++;

            this.Button3.IsEnabled = true;
            this.Button2.IsEnabled = false;
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            M_dbConn.Close();

            PROTOCOL protocol = new PROTOCOL(myMetal, dataTable, temperature);
         
            protocol.Show();
            this.Close();
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
    }
}