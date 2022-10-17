using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GesällProv
{
    public partial class MainWindow : Window
    {
        public bool IsListening;
        public bool IsConnected;
        public string RemotePlayer;
        public string LocalPlayer;
        private bool PlayerOneReady = false;
        private bool PlayerTwoReady = false;
        private bool GameOn = false;
        private bool PlayerTurn = false;
        private bool XWon = false;
        private bool OWon = false;
        private bool Draw = false;
        private readonly TcpListener server = new TcpListener(IPAddress.Parse(GetLocalIPAddress()), 2000);
        private static readonly TcpClient TcpClient = new TcpClient();

        public MainWindow()
        {
            InitializeComponent();
            StartListening();
            IpTextBox.Text = "192.168.1.67";
            RemotePlayer = "";
            LocalPlayer = "";
        }
        public static string GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public void StartListening()
        {
            IsListening = true;
            server.Start();
            Task.Factory.StartNew(Listening, TaskCreationOptions.LongRunning);
        }
        public static void Send(string command)
        {
            try
            {
                if (CheckSocketConnected(TcpClient.Client) == true)
                {
                    byte[] data = Encoding.ASCII.GetBytes(command);
                    NetworkStream stream = TcpClient.GetStream();
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception)
            {

            }
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IpTextBox.Text == "")
            {
                ConnectBtn.IsEnabled = false;
            }
            else
            {
                ConnectBtn.IsEnabled = true;
            }
        }
        private void Connect(object sender, RoutedEventArgs e)
        {
            try
            {
                Task<TcpClient> connectionTask = TcpClient
  .ConnectAsync(IPAddress.Parse(IpTextBox.Text.ToString()), 2000).ContinueWith(task =>
  {
      return task.IsFaulted ? null : TcpClient;
  }, TaskContinuationOptions.ExecuteSynchronously);
                Task<TcpClient> timeoutTask = Task.Delay(1000)
                    .ContinueWith<TcpClient>(task => null, TaskContinuationOptions.ExecuteSynchronously);
                Task<TcpClient> resultTask = Task.WhenAny(connectionTask, timeoutTask).Unwrap();

                resultTask.Wait();
                TcpClient resultTcpClient = resultTask.Result;

                if (resultTcpClient != null)
                {
                    IsConnected = true;
                    Connected_Disconnected.Dispatcher.Invoke(() => Connected_Disconnected.Text = "Connected");
                    Connected_Disconnected.Dispatcher.Invoke(() => Connected_Disconnected.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4FC062")));
                }
                else
                {
                    Connected_Disconnected.Dispatcher.Invoke(() => Connected_Disconnected.Text = "Not Connected");
                    Connected_Disconnected.Dispatcher.Invoke(() => Connected_Disconnected.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D02E49")));

                }
            }
            catch (Exception)
            {

            }
        }
        public static bool CheckSocketConnected(Socket s)
        {
            bool canRead = s.Poll(1000, SelectMode.SelectRead);
            bool dataAvailable = (s.Available == 0);
            if (canRead & dataAvailable)
            {
                return false;
            }
            return true;
        }
        private void Choose(object sender, RoutedEventArgs e)
        {
            if (IsConnected == true && IsListening == true)
            {
                IpLabel.Visibility = Visibility.Hidden;
                IpTextBox.Visibility = Visibility.Hidden;
                ConnectBtn.Visibility = Visibility.Hidden;
                StartGameBtn.Visibility = Visibility.Hidden;
                Player1.Visibility = Visibility.Visible;
                Player2.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Not Connected");
            }
        }
        private void XChoosen(object sender, RoutedEventArgs e)
        {
            Send("X");
            Player1.IsEnabled = false;
            Player2.Visibility = Visibility.Hidden;
            RemotePlayer = "O";
            LocalPlayer = "X";
            InvokeLabel();
            PlayerOneReady = true;
            CheckPlayersReady();
        }
        private void OChoosen(object sender, RoutedEventArgs e)
        {
            Send("O");
            Player2.IsEnabled = false;
            Player1.Visibility = Visibility.Hidden;
            RemotePlayer = "X";
            LocalPlayer = "O";
            InvokeLabel();
            PlayerTwoReady = true;
            CheckPlayersReady();
        }
        public void InvokeButton(Button btn, string remotePlayer, string localPlayer)
        {
            btn.Dispatcher.Invoke(() => btn.IsEnabled = false);
            RemotePlayer = remotePlayer;
            LocalPlayer = localPlayer;
        }
        public void MovePlayed(Button btn, string content, Label label)
        {
            btn.Dispatcher.Invoke(() => btn.Content = content);
            btn.Dispatcher.Invoke(() => btn.Foreground = label.Foreground);
            btn.Dispatcher.Invoke(() => btn.IsEnabled = false);
        }
        private void Btn00_Click(object sender, RoutedEventArgs e)
        {
            Send("00");
            MovePlayed(btn00, LocalPlayer, Player1Label);
        }
        private void Btn01_Click(object sender, RoutedEventArgs e)
        {
            Send("01");
            MovePlayed(btn01, LocalPlayer, Player1Label);
        }
        private void Btn02_Click(object sender, RoutedEventArgs e)
        {
            Send("02");
            MovePlayed(btn02, LocalPlayer, Player1Label);
        }
        private void Btn10_Click(object sender, RoutedEventArgs e)
        {
            Send("10");
            MovePlayed(btn10, LocalPlayer, Player1Label);
        }
        private void Btn11_Click(object sender, RoutedEventArgs e)
        {
            Send("11");
            MovePlayed(btn11, LocalPlayer, Player1Label);
        }
        private void Btn12_Click(object sender, RoutedEventArgs e)
        {
            Send("12");
            MovePlayed(btn12, LocalPlayer, Player1Label);
        }
        private void Btn20_Click(object sender, RoutedEventArgs e)
        {
            Send("20");
            MovePlayed(btn20, LocalPlayer, Player1Label);
        }
        private void Btn21_Click(object sender, RoutedEventArgs e)
        {
            Send("21");
            MovePlayed(btn21, LocalPlayer, Player1Label);
        }
        private void Btn22_Click(object sender, RoutedEventArgs e)
        {
            Send("22");
            MovePlayed(btn22, LocalPlayer, Player1Label);
        }
        public void RunGame()
        {
            while (GameOn == true)
            {
                CheckXWin();
                CheckOWin();
                CheckDraw();
                if (XWon == true)
                {
                    ReslutTextBlock.Dispatcher.Invoke(() => ReslutTextBlock.Visibility = Visibility.Visible);
                    ReslutTextBlock.Dispatcher.Invoke(() => ReslutTextBlock.Text = "X WINS");
                    PlayAgainBtn.Dispatcher.Invoke(() => PlayAgainBtn.Visibility = Visibility.Visible);
                    GameOn = false;
                    break;
                }
                else if (OWon == true)
                {
                    ReslutTextBlock.Dispatcher.Invoke(() => ReslutTextBlock.Visibility = Visibility.Visible);
                    ReslutTextBlock.Dispatcher.Invoke(() => ReslutTextBlock.Text = "O WINS");
                    PlayAgainBtn.Dispatcher.Invoke(() => PlayAgainBtn.Visibility = Visibility.Visible);
                    GameOn = false;
                    break;
                }
                else if (Draw == true)
                {
                    ReslutTextBlock.Dispatcher.Invoke(() => ReslutTextBlock.Visibility = Visibility.Visible);
                    ReslutTextBlock.Dispatcher.Invoke(() => ReslutTextBlock.Text = "DRAW");
                    PlayAgainBtn.Dispatcher.Invoke(() => PlayAgainBtn.Visibility = Visibility.Visible);
                    GameOn = false;
                    break;
                }
                CheckTurn();
            }
        }
        public void Listening()
        {
            while (IsListening == true)
            {
                int i;
                TcpClient newClient = server.AcceptTcpClientAsync().Result;
                while (newClient != null)
                {
                    if (newClient.Client.Connected && CheckSocketConnected(newClient.Client) == false)
                    {

                    }
                    else
                    {
                        try
                        {
                            string recv = "";
                            byte[] buffer = new byte[1024];
                            while ((i = newClient.GetStream().Read(buffer, 0, buffer.Length)) != 0)
                            {
                                recv = Encoding.ASCII.GetString(buffer, 0, i);
                                if (recv == "X")
                                {
                                    if (Player1.Dispatcher.Invoke(() => Player1.IsEnabled = true))
                                    {
                                        InvokeButton(Player1, "X", "O");
                                        PlayerOneReady = true;
                                    }
                                    else
                                    {
                                        InvokeButton(Player2, "O", "X");
                                        PlayerTwoReady = true;
                                    }
                                    CheckPlayersReady();
                                    InvokeLabel();
                                }
                                else if (recv == "O")
                                {
                                    if (Player2.Dispatcher.Invoke(() => Player2.IsEnabled = true))
                                    {
                                        InvokeButton(Player2, "O", "X");
                                        PlayerTwoReady = true;
                                    }
                                    else
                                    {
                                        InvokeButton(Player1, "X", "O");
                                        PlayerOneReady = true;
                                    }
                                    CheckPlayersReady();
                                    InvokeLabel();
                                }
                                else if (recv == "Again")
                                {
                                    PlayAgain();
                                }
                                else
                                {
                                    if (recv == "00")
                                    {
                                        MovePlayed(btn00, RemotePlayer, Player2Label);
                                    }
                                    else if (recv == "01")
                                    {
                                        MovePlayed(btn01, RemotePlayer, Player2Label);
                                    }
                                    else if (recv == "02")
                                    {
                                        MovePlayed(btn02, RemotePlayer, Player2Label);
                                    }
                                    else if (recv == "10")
                                    {
                                        MovePlayed(btn10, RemotePlayer, Player2Label);
                                    }
                                    else if (recv == "11")
                                    {
                                        MovePlayed(btn11, RemotePlayer, Player2Label);
                                    }
                                    else if (recv == "12")
                                    {
                                        MovePlayed(btn12, RemotePlayer, Player2Label);
                                    }
                                    else if (recv == "20")
                                    {
                                        MovePlayed(btn20, RemotePlayer, Player2Label);
                                    }
                                    else if (recv == "21")
                                    {
                                        MovePlayed(btn21, RemotePlayer, Player2Label);
                                    }
                                    else if (recv == "22")
                                    {
                                        MovePlayed(btn22, RemotePlayer, Player2Label);
                                    }
                                    PlayerTurn = true;
                                }
                            }

                        }
                        catch (Exception)
                        {

                        }

                    }

                }
            }
        }
        public string CheckButtonContent(Button btn)
        {
            return btn.Dispatcher.Invoke(() => btn.Content.ToString());
        }
        public void CheckXWin()
        {
            string x = "X";
            if (CheckButtonContent(btn00) == x && CheckButtonContent(btn01) == x && CheckButtonContent(btn02) == x)
            {
                XWon = true;
            }
            else if (CheckButtonContent(btn10) == x && CheckButtonContent(btn11) == x && CheckButtonContent(btn12) == x)
            {
                XWon = true;
            }
            else if (CheckButtonContent(btn20) == x && CheckButtonContent(btn21) == x && CheckButtonContent(btn22) == x)
            {
                XWon = true;
            }
            else if (CheckButtonContent(btn00) == x && CheckButtonContent(btn10) == x && CheckButtonContent(btn20) == x)
            {
                XWon = true;
            }
            else if (CheckButtonContent(btn01) == x && CheckButtonContent(btn11) == x && CheckButtonContent(btn21) == x)
            {
                XWon = true;
            }
            else if (CheckButtonContent(btn02) == x && CheckButtonContent(btn12) == x && CheckButtonContent(btn22) == x)
            {
                XWon = true;
            }
            else if (CheckButtonContent(btn00) == x && CheckButtonContent(btn11) == x && CheckButtonContent(btn22) == x)
            {
                XWon = true;
            }
            else if (CheckButtonContent(btn02) == x && CheckButtonContent(btn11) == x && CheckButtonContent(btn20) == x)
            {
                XWon = true;
            }
        }
        public void CheckOWin()
        {
            string o = "O";
            if (CheckButtonContent(btn00) == o && CheckButtonContent(btn01) == o && CheckButtonContent(btn02) == o)
            {
                OWon = true;
            }
            else if (CheckButtonContent(btn10) == o && CheckButtonContent(btn11) == o && CheckButtonContent(btn12) == o)
            {
                OWon = true;
            }
            else if (CheckButtonContent(btn20) == o && CheckButtonContent(btn21) == o && CheckButtonContent(btn22) == o)
            {
                OWon = true;
            }
            else if (CheckButtonContent(btn00) == o && CheckButtonContent(btn10) == o && CheckButtonContent(btn20) == o)
            {
                OWon = true;
            }
            else if (CheckButtonContent(btn01) == o && CheckButtonContent(btn11) == o && CheckButtonContent(btn21) == o)
            {
                OWon = true;
            }
            else if (CheckButtonContent(btn02) == o && CheckButtonContent(btn12) == o && CheckButtonContent(btn22) == o)
            {
                OWon = true;
            }
            else if (CheckButtonContent(btn00) == o && CheckButtonContent(btn11) == o && CheckButtonContent(btn22) == o)
            {
                OWon = true;
            }
            else if (CheckButtonContent(btn02) == o && CheckButtonContent(btn11) == o && CheckButtonContent(btn20) == o)
            {
                OWon = true;
            }
        }
        public void InvokeLabel()
        {
            Player2Label.Dispatcher.Invoke(() => Player2Label.Content = RemotePlayer);
            Player1Label.Dispatcher.Invoke(() => Player1Label.Content = LocalPlayer);
        }
        public bool CheckButtonEnabled(Button btn)
        {
            if (btn.Dispatcher.Invoke(() => btn.Content == ""))
            {
                return false;
            }
            return true;
        }
        public void CheckDraw()
        {
            if (CheckButtonEnabled(btn00) && CheckButtonEnabled(btn01) && CheckButtonEnabled(btn02) && CheckButtonEnabled(btn10) && CheckButtonEnabled(btn11) && CheckButtonEnabled(btn12) && CheckButtonEnabled(btn20) && CheckButtonEnabled(btn21) && CheckButtonEnabled(btn22) && XWon == false && OWon == false)
            {
                Draw = true;
            }
        }
        public void CheckPlayersReady()
        {
            if (PlayerOneReady == true && PlayerTwoReady == true)
            {
                GameGrid.Dispatcher.Invoke(() => GameGrid.Visibility = Visibility.Visible);
                Player1.Dispatcher.Invoke(() => Player1.Visibility = Visibility.Hidden);
                Player2.Dispatcher.Invoke(() => Player2.Visibility = Visibility.Hidden);
                GameOn = true;
                if (LocalPlayer == "X")
                {
                    PlayerTurn = true;
                }
                else if (LocalPlayer == "O")
                {
                    PlayerTurn = false;
                }
                Task.Factory.StartNew(RunGame, TaskCreationOptions.LongRunning);
            }
        }
        public void CheckTurn()
        {
            if (PlayerTurn == false)
            {
                GameGrid.Dispatcher.Invoke(() => GameGrid.IsEnabled = false);
            }
            if (PlayerTurn == true)
            {
                GameGrid.Dispatcher.Invoke(() => GameGrid.IsEnabled = true);
            }

        }
        public void PlayAgain()
        {
            XWon = false;
            OWon = false;
            Draw = false;
            ReslutTextBlock.Dispatcher.Invoke(() => ReslutTextBlock.Visibility = Visibility.Hidden);
            PlayAgainBtn.Dispatcher.Invoke(() => PlayAgainBtn.Visibility = Visibility.Hidden);
            ResetButtonContent(btn00);
            ResetButtonContent(btn01);
            ResetButtonContent(btn02);
            ResetButtonContent(btn10);
            ResetButtonContent(btn11);
            ResetButtonContent(btn12);
            ResetButtonContent(btn20);
            ResetButtonContent(btn21);
            ResetButtonContent(btn22);
            CheckPlayersReady();
        }
        public void ResetButtonContent(Button btn)
        {
            btn.Dispatcher.Invoke(() => btn.IsEnabled = true);
            btn.Dispatcher.Invoke(() => btn.Content = "");
        }
        private void PlayAgainClicked(object sender, RoutedEventArgs e)
        {
            if (LocalPlayer == "X")
            {
                Send("Again");
                PlayAgain();
            }
        }
    }
}
