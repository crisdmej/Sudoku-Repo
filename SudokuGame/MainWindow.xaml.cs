using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
using System.Windows.Threading;

namespace SudokuGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string difficulty = "";
        public static DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public static int minute = 0;
        public static int second = 0;

        public MainWindow()
        {
            InitializeComponent();

            InitializeTimer();
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            difficulty = difficultyComboBox.Text;
            if(difficulty != "Select Difficulty")
            {
                SudokuBoard sudokuBoard = new SudokuBoard();
                dispatcherTimer.Start();
                sudokuBoard.Show();
            }
            else
            {
                MessageBox.Show("Please select a difficulty.", "Select Difficulty", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        private void continueGameButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void leaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            HighScores frm = new HighScores();
            frm.Show();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit the game?", "Exit Game", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void InitializeTimer()
        {
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += dispatchTimer_Tick;
        }

        private void dispatchTimer_Tick(object sender, EventArgs e)
        {
            SudokuBoard.sudokuGame.Time++;
            int minute = SudokuBoard.sudokuGame.Time / 60;
            int second = SudokuBoard.sudokuGame.Time % 60;
            SudokuBoard.time.Content = $"Time: {minute.ToString("00")}:{second.ToString("00")}";
        }
    }
}
