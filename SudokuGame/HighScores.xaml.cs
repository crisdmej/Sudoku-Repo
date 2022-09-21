using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

namespace SudokuGame
{
    /// <summary>
    /// Interaction logic for HighScores.xaml
    /// </summary>
    public partial class HighScores : Window
    {
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();

        public HighScores()
        {
            InitializeComponent();

            if(MainWindow.difficulty != "")
            {
                timeTextbox.Text = SudokuBoard.time.Content.ToString().Substring(6);
            }

            GetData();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(nameTextbox.Text == null || nameTextbox.Text.Trim() == null)
                {
                    MessageBox.Show("Please enter a name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    nameTextbox.Focus();
                    return;
                }
                else if(MainWindow.difficulty == "")
                {
                    MessageBox.Show("Please play a game to submit to the leaderboard", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    if (MessageBox.Show("Are you want to save?", "Save Score", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        mycon();
                        //Insert query to Save data in the table
                        cmd = new SqlCommand("INSERT INTO High_Scores(Name, Difficulty, Time) VALUES(@Name, @Difficulty, @Time)", con);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Name", nameTextbox.Text);
                        cmd.Parameters.AddWithValue("@Difficulty", MainWindow.difficulty);
                        cmd.Parameters.AddWithValue("@Time", timeTextbox.Text);
                        cmd.ExecuteNonQuery();
                        con.Close();

                        MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        ClearMaster();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to cancel?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        public void mycon()
        {
            String Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            con = new SqlConnection(Conn);
            con.Open();
        }

        private void GetData()
        {
            mycon();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("SELECT * FROM High_Scores", con);
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if(dt != null && dt.Rows.Count > 0)
            {
                leaderboardGrid.ItemsSource = dt.DefaultView;
            }
            else
            {
                leaderboardGrid.ItemsSource = null;
            }
            con.Close();
        }

        private void ClearMaster()
        {
            nameTextbox.Text = String.Empty;
            timeTextbox.Text = String.Empty;
            GetData();
        }
    }
}
