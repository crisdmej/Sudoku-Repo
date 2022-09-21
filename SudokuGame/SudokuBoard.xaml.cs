using SudokuGame.DataAccess;
using SudokuGame.Models;
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
using System.Windows.Threading;

namespace SudokuGame
{
    /// <summary>
    /// Interaction logic for SudokuBoard.xaml
    /// </summary>
    public partial class SudokuBoard : Window
    {
        private const string easySudoku1 = "EasySudoku1.csv";
        private const string easySudoku1Answer = "EasySudoku1Answer.csv";
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public TextboxModel[] sudokuBoard = new TextboxModel[81];
        public static SudokuGameModel sudokuGame;
        public static Label time = new Label();

        public SudokuBoard()
        {
            InitializeComponent();

            InitializeTextboxModel();

            InitializeSudokuBoard();

            GenerateSudokuBoardAnswer();

            GenerateSudokuBoard();

            StartTimer();
        }

        private void textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                for (int i = 0; i < sudokuGame.SudokuBoard.Length; i++)
                {
                    if (sudokuGame.SudokuBoard[i].SudokuTextBox.IsFocused)
                    {
                        if (i < 72)
                        {
                            i += 9;
                            sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Background = Brushes.White;
                            sudokuGame.SudokuBoard[i].SudokuTextBox.Focus();
                            sudokuGame.SudokuBoard[i].SudokuTextBox.Background = new SolidColorBrush(Color.FromRgb(0xb3, 0xe5, 0xfc));
                            sudokuGame.PrevFocusIndex = i;
                            e.Handled = true;
                        }
                    }
                }
            }

            if (e.Key == Key.Up)
            {
                for (int i = 0; i < sudokuGame.SudokuBoard.Length; i++)
                {
                    if (sudokuGame.SudokuBoard[i].SudokuTextBox.IsFocused)
                    {
                        if (i > 8)
                        {
                            i -= 9;
                            sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Background = Brushes.White;
                            sudokuGame.SudokuBoard[i].SudokuTextBox.Focus();
                            sudokuGame.SudokuBoard[i].SudokuTextBox.Background = new SolidColorBrush(Color.FromRgb(0xb3, 0xe5, 0xfc));
                            sudokuGame.PrevFocusIndex = i;
                            e.Handled = true;
                        }
                    }
                }
            }

            if (e.Key == Key.Left)
            {
                for (int i = 0; i < sudokuGame.SudokuBoard.Length; i++)
                {
                    if (sudokuGame.SudokuBoard[i].SudokuTextBox.IsFocused)
                    {
                        if (i % 9 != 0)
                        {
                            i -= 1;
                            sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Background = Brushes.White;
                            sudokuGame.SudokuBoard[i].SudokuTextBox.Focus();
                            sudokuGame.SudokuBoard[i].SudokuTextBox.Background = new SolidColorBrush(Color.FromRgb(0xb3, 0xe5, 0xfc));
                            sudokuGame.PrevFocusIndex = i;
                            e.Handled = true;
                        }
                    }
                }
            }

            if (e.Key == Key.Right)
            {
                for (int i = 0; i < sudokuGame.SudokuBoard.Length; i++)
                {
                    if (sudokuGame.SudokuBoard[i].SudokuTextBox.IsFocused)
                    {
                        if ((i - 8) % 9 != 0)
                        {
                            i += 1;
                            sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Background = Brushes.White;
                            sudokuGame.SudokuBoard[i].SudokuTextBox.Focus();
                            sudokuGame.SudokuBoard[i].SudokuTextBox.Background = new SolidColorBrush(Color.FromRgb(0xb3, 0xe5, 0xfc));
                            sudokuGame.PrevFocusIndex = i;
                            e.Handled = true;
                        }
                    }
                }
            }

            if(e.Key == Key.Back)
            {
                for (int i = 0; i < sudokuGame.SudokuBoard.Length; i++)
                {
                    if (sudokuGame.SudokuBoard[i].SudokuTextBox.IsFocused)
                    {
                        foreach(TextBlock textBlock in sudokuGame.SudokuBoard[i].SudokuNotes)
                        {
                            textBlock.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
        }

        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            for (int i = 0; i < sudokuGame.SudokuBoard.Length; i++)
            {
                if (sudokuGame.SudokuBoard[i].SudokuTextBox.IsFocused)
                {
                    sudokuGame.SudokuBoard[i].SudokuTextBox.Background = new SolidColorBrush(Color.FromRgb(0xb3, 0xe5, 0xfc));

                    if (sudokuGame.SudokuBoard[i].SudokuTextBox.Text == "")
                    {
                        if (sudokuGame.FinishSetup)
                        {
                            AddToStack();
                            sudokuGame.PrevFocusIndexStack.Push(i);
                        }
                        continue;
                    }
                    bool isValid = true;
                    int value = 0;
                    isValid = int.TryParse(sudokuGame.SudokuBoard[i].SudokuTextBox.Text, out value);
                    if (!isValid)
                    {
                        sudokuGame.SudokuBoard[i].SudokuTextBox.Text = "";
                        MessageBox.Show("Invalid input: Please enter a number.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (value < 1 || value > 9)
                    {
                        sudokuGame.SudokuBoard[i].SudokuTextBox.Text = "";
                        MessageBox.Show("Invalid input: Please enter a number between 1 and 9.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        if (sudokuGame.NotesOn)
                        {
                            if (sudokuGame.SudokuBoard[i].SudokuNotes[value-1].Visibility == Visibility.Hidden)
                            {
                                sudokuGame.SudokuBoard[i].SudokuNotes[value - 1].Visibility = Visibility.Visible;
                                sudokuGame.SudokuBoard[i].SudokuTextBox.Text = "";
                            }
                            else
                            {
                                sudokuGame.SudokuBoard[i].SudokuNotes[value - 1].Visibility = Visibility.Hidden;
                                sudokuGame.SudokuBoard[i].SudokuTextBox.Text = "";
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                sudokuGame.SudokuBoard[i].SudokuNotes[j].Visibility = Visibility.Hidden;
                            }

                            if (sudokuGame.FinishSetup)
                            {
                                AddToStack();
                                sudokuGame.PrevFocusIndexStack.Push(i);
                            }

                            CheckIfInputIsCorrect();

                            if (IsGameComplete())
                            {
                                MessageBoxResult result = MessageBox.Show("Congratulations on completing this Sudoku Puzzle! Would you like to play again?", "Game Complete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (result == MessageBoxResult.Yes)
                                {
                                    dispatcherTimer.Stop();
                                    HighScores highScores = new HighScores();
                                    highScores.Show();
                                    this.Close();
                                }
                                else
                                {
                                    System.Windows.Application.Current.Shutdown();
                                }
                            }
                        }
                    }
                }
                else
                {
                    sudokuGame.SudokuBoard[i].SudokuTextBox.Background = Brushes.White;
                }
            }
        }

        private void textbox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int i = 0; i < sudokuGame.SudokuBoard.Length; i++)
            {
                if (sudokuGame.SudokuBoard[i].SudokuTextBox.IsFocused)
                {
                    sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Background = Brushes.White;
                    sudokuGame.SudokuBoard[i].SudokuTextBox.Background = new SolidColorBrush(Color.FromRgb(0xb3, 0xe5, 0xfc));
                    sudokuGame.PrevFocusIndex = i;
                }
            }
        }

        private void hintButton_Click(object sender, RoutedEventArgs e)
        {
            if(sudokuGame.NumberOfHints != 0)
            {
                if (sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Text != sudokuGame.SudokuBoardAnswer[sudokuGame.PrevFocusIndex].Text)
                {
                    sudokuGame.NumberOfHints--;
                    hintsLabel.Content = $"Hints: {sudokuGame.NumberOfHints}/3";

                    sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Focus();
                    sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Text = sudokuGame.SudokuBoardAnswer[sudokuGame.PrevFocusIndex].Text;
                }
                if(sudokuGame.NumberOfHints == 0)
                {
                    hintButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void solveButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();

            MessageBoxResult result1 = MessageBox.Show("Are you sure you want to give up?", "Solve Sudoku", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result1 == MessageBoxResult.Yes)
            {
                for (int i = 0; i < 81; i++)
                {
                    if (sudokuGame.SudokuBoard[i].SudokuTextBox.Text != sudokuGame.SudokuBoardAnswer[i].Text)
                    {
                        sudokuGame.SudokuBoard[i].SudokuTextBox.Text = sudokuGame.SudokuBoardAnswer[i].Text;
                        sudokuGame.SudokuBoard[i].SudokuTextBox.Foreground = Brushes.RoyalBlue;
                    }
                }

                MessageBoxResult result2 = MessageBox.Show("Would you like to try to solve a new puzzle?", "New Game?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result2 == MessageBoxResult.Yes)
                {
                    this.Close();
                }
                else
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }

            dispatcherTimer.Start();
        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            if (sudokuGame.SudokuBoardStack.Count > 1)
            {
                sudokuGame.SudokuBoardStack.Pop();
                sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndexStack.Pop()].SudokuTextBox.Focus();

                string[,] temp = sudokuGame.SudokuBoardStack.Peek();

                for (int i = 0; i < 81; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (temp[i,j+1] == "0")
                        {
                            sudokuGame.SudokuBoard[i].SudokuNotes[j].Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            sudokuGame.SudokuBoard[i].SudokuNotes[j].Visibility = Visibility.Visible;
                        }
                    }

                    if (sudokuGame.SudokuBoard[i].SudokuTextBox.IsFocused)
                    {
                        sudokuGame.SudokuBoard[i].SudokuTextBox.Background = new SolidColorBrush(Color.FromRgb(0xb3, 0xe5, 0xfc));
                    }
                    else
                    {
                        sudokuGame.SudokuBoard[i].SudokuTextBox.Background = Brushes.White;
                    }

                    if (sudokuGame.SudokuBoard[i].SudokuTextBox.Text != temp[i, 0])
                    {
                        sudokuGame.SudokuBoard[i].SudokuTextBox.Text = temp[i, 0];
                        sudokuGame.SudokuBoardStack.Pop();
                        sudokuGame.PrevFocusIndex = sudokuGame.PrevFocusIndexStack.Pop();
                    }
                }
            }
            else
            {
                sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Focus();
            }
        }

        private void notesButton_Click(object sender, RoutedEventArgs e)
        {
            if (sudokuGame.NotesOn == false)
            {
                sudokuGame.NotesOn = true;
                notesButton.Background = Brushes.LightGray;
                notesLabel.Content = "Notes: ON";
                sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Focus();
            }
            else
            {
                sudokuGame.NotesOn = false;
                notesButton.Background = Brushes.White;
                notesLabel.Content = "Notes: OFF";
                sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Focus();
            }
        }

        private void InitializeTextboxModel()
        {
            TextBlock[] notes = new TextBlock[9];
            notes[0] = notes1_1;
            notes[1] = notes1_2;
            notes[2] = notes1_3;
            notes[3] = notes1_4;
            notes[4] = notes1_5;
            notes[5] = notes1_6;
            notes[6] = notes1_7;
            notes[7] = notes1_8;
            notes[8] = notes1_9;
            sudokuBoard[0] = new TextboxModel(textBox1, notes);

            notes = new TextBlock[9];
            notes[0] = notes2_1;
            notes[1] = notes2_2;
            notes[2] = notes2_3;
            notes[3] = notes2_4;
            notes[4] = notes2_5;
            notes[5] = notes2_6;
            notes[6] = notes2_7;
            notes[7] = notes2_8;
            notes[8] = notes2_9;
            sudokuBoard[1] = new TextboxModel(textBox2, notes);

            notes = new TextBlock[9];
            notes[0] = notes3_1;
            notes[1] = notes3_2;
            notes[2] = notes3_3;
            notes[3] = notes3_4;
            notes[4] = notes3_5;
            notes[5] = notes3_6;
            notes[6] = notes3_7;
            notes[7] = notes3_8;
            notes[8] = notes3_9;
            sudokuBoard[2] = new TextboxModel(textBox3, notes);

            notes = new TextBlock[9];
            notes[0] = notes4_1;
            notes[1] = notes4_2;
            notes[2] = notes4_3;
            notes[3] = notes4_4;
            notes[4] = notes4_5;
            notes[5] = notes4_6;
            notes[6] = notes4_7;
            notes[7] = notes4_8;
            notes[8] = notes4_9;
            sudokuBoard[3] = new TextboxModel(textBox4, notes);

            notes = new TextBlock[9];
            notes[0] = notes5_1;
            notes[1] = notes5_2;
            notes[2] = notes5_3;
            notes[3] = notes5_4;
            notes[4] = notes5_5;
            notes[5] = notes5_6;
            notes[6] = notes5_7;
            notes[7] = notes5_8;
            notes[8] = notes5_9;
            sudokuBoard[4] = new TextboxModel(textBox5, notes);

            notes = new TextBlock[9];
            notes[0] = notes6_1;
            notes[1] = notes6_2;
            notes[2] = notes6_3;
            notes[3] = notes6_4;
            notes[4] = notes6_5;
            notes[5] = notes6_6;
            notes[6] = notes6_7;
            notes[7] = notes6_8;
            notes[8] = notes6_9;
            sudokuBoard[5] = new TextboxModel(textBox6, notes);

            notes = new TextBlock[9];
            notes[0] = notes7_1;
            notes[1] = notes7_2;
            notes[2] = notes7_3;
            notes[3] = notes7_4;
            notes[4] = notes7_5;
            notes[5] = notes7_6;
            notes[6] = notes7_7;
            notes[7] = notes7_8;
            notes[8] = notes7_9;
            sudokuBoard[6] = new TextboxModel(textBox7, notes);

            notes = new TextBlock[9];
            notes[0] = notes8_1;
            notes[1] = notes8_2;
            notes[2] = notes8_3;
            notes[3] = notes8_4;
            notes[4] = notes8_5;
            notes[5] = notes8_6;
            notes[6] = notes8_7;
            notes[7] = notes8_8;
            notes[8] = notes8_9;
            sudokuBoard[7] = new TextboxModel(textBox8, notes);

            notes = new TextBlock[9];
            notes[0] = notes9_1;
            notes[1] = notes9_2;
            notes[2] = notes9_3;
            notes[3] = notes9_4;
            notes[4] = notes9_5;
            notes[5] = notes9_6;
            notes[6] = notes9_7;
            notes[7] = notes9_8;
            notes[8] = notes9_9;
            sudokuBoard[8] = new TextboxModel(textBox9, notes);

            notes = new TextBlock[9];
            notes[0] = notes10_1;
            notes[1] = notes10_2;
            notes[2] = notes10_3;
            notes[3] = notes10_4;
            notes[4] = notes10_5;
            notes[5] = notes10_6;
            notes[6] = notes10_7;
            notes[7] = notes10_8;
            notes[8] = notes10_9;
            sudokuBoard[9] = new TextboxModel(textBox10, notes);

            notes = new TextBlock[9];
            notes[0] = notes11_1;
            notes[1] = notes11_2;
            notes[2] = notes11_3;
            notes[3] = notes11_4;
            notes[4] = notes11_5;
            notes[5] = notes11_6;
            notes[6] = notes11_7;
            notes[7] = notes11_8;
            notes[8] = notes11_9;
            sudokuBoard[10] = new TextboxModel(textBox11, notes);

            notes = new TextBlock[9];
            notes[0] = notes12_1;
            notes[1] = notes12_2;
            notes[2] = notes12_3;
            notes[3] = notes12_4;
            notes[4] = notes12_5;
            notes[5] = notes12_6;
            notes[6] = notes12_7;
            notes[7] = notes12_8;
            notes[8] = notes12_9;
            sudokuBoard[11] = new TextboxModel(textBox12, notes);

            notes = new TextBlock[9];
            notes[0] = notes13_1;
            notes[1] = notes13_2;
            notes[2] = notes13_3;
            notes[3] = notes13_4;
            notes[4] = notes13_5;
            notes[5] = notes13_6;
            notes[6] = notes13_7;
            notes[7] = notes13_8;
            notes[8] = notes13_9;
            sudokuBoard[12] = new TextboxModel(textBox13, notes);

            notes = new TextBlock[9];
            notes[0] = notes14_1;
            notes[1] = notes14_2;
            notes[2] = notes14_3;
            notes[3] = notes14_4;
            notes[4] = notes14_5;
            notes[5] = notes14_6;
            notes[6] = notes14_7;
            notes[7] = notes14_8;
            notes[8] = notes14_9;
            sudokuBoard[13] = new TextboxModel(textBox14, notes);

            notes = new TextBlock[9];
            notes[0] = notes15_1;
            notes[1] = notes15_2;
            notes[2] = notes15_3;
            notes[3] = notes15_4;
            notes[4] = notes15_5;
            notes[5] = notes15_6;
            notes[6] = notes15_7;
            notes[7] = notes15_8;
            notes[8] = notes15_9;
            sudokuBoard[14] = new TextboxModel(textBox15, notes);

            notes = new TextBlock[9];
            notes[0] = notes16_1;
            notes[1] = notes16_2;
            notes[2] = notes16_3;
            notes[3] = notes16_4;
            notes[4] = notes16_5;
            notes[5] = notes16_6;
            notes[6] = notes16_7;
            notes[7] = notes16_8;
            notes[8] = notes16_9;
            sudokuBoard[15] = new TextboxModel(textBox16, notes);

            notes = new TextBlock[9];
            notes[0] = notes17_1;
            notes[1] = notes17_2;
            notes[2] = notes17_3;
            notes[3] = notes17_4;
            notes[4] = notes17_5;
            notes[5] = notes17_6;
            notes[6] = notes17_7;
            notes[7] = notes17_8;
            notes[8] = notes17_9;
            sudokuBoard[16] = new TextboxModel(textBox17, notes);

            notes = new TextBlock[9];
            notes[0] = notes18_1;
            notes[1] = notes18_2;
            notes[2] = notes18_3;
            notes[3] = notes18_4;
            notes[4] = notes18_5;
            notes[5] = notes18_6;
            notes[6] = notes18_7;
            notes[7] = notes18_8;
            notes[8] = notes18_9;
            sudokuBoard[17] = new TextboxModel(textBox18, notes);

            notes = new TextBlock[9];
            notes[0] = notes19_1;
            notes[1] = notes19_2;
            notes[2] = notes19_3;
            notes[3] = notes19_4;
            notes[4] = notes19_5;
            notes[5] = notes19_6;
            notes[6] = notes19_7;
            notes[7] = notes19_8;
            notes[8] = notes19_9;
            sudokuBoard[18] = new TextboxModel(textBox19, notes);

            notes = new TextBlock[9];
            notes[0] = notes20_1;
            notes[1] = notes20_2;
            notes[2] = notes20_3;
            notes[3] = notes20_4;
            notes[4] = notes20_5;
            notes[5] = notes20_6;
            notes[6] = notes20_7;
            notes[7] = notes20_8;
            notes[8] = notes20_9;
            sudokuBoard[19] = new TextboxModel(textBox20, notes);

            notes = new TextBlock[9];
            notes[0] = notes21_1;
            notes[1] = notes21_2;
            notes[2] = notes21_3;
            notes[3] = notes21_4;
            notes[4] = notes21_5;
            notes[5] = notes21_6;
            notes[6] = notes21_7;
            notes[7] = notes21_8;
            notes[8] = notes21_9;
            sudokuBoard[20] = new TextboxModel(textBox21, notes);

            notes = new TextBlock[9];
            notes[0] = notes22_1;
            notes[1] = notes22_2;
            notes[2] = notes22_3;
            notes[3] = notes22_4;
            notes[4] = notes22_5;
            notes[5] = notes22_6;
            notes[6] = notes22_7;
            notes[7] = notes22_8;
            notes[8] = notes22_9;
            sudokuBoard[21] = new TextboxModel(textBox22, notes);

            notes = new TextBlock[9];
            notes[0] = notes23_1;
            notes[1] = notes23_2;
            notes[2] = notes23_3;
            notes[3] = notes23_4;
            notes[4] = notes23_5;
            notes[5] = notes23_6;
            notes[6] = notes23_7;
            notes[7] = notes23_8;
            notes[8] = notes23_9;
            sudokuBoard[22] = new TextboxModel(textBox23, notes);

            notes = new TextBlock[9];
            notes[0] = notes24_1;
            notes[1] = notes24_2;
            notes[2] = notes24_3;
            notes[3] = notes24_4;
            notes[4] = notes24_5;
            notes[5] = notes24_6;
            notes[6] = notes24_7;
            notes[7] = notes24_8;
            notes[8] = notes24_9;
            sudokuBoard[23] = new TextboxModel(textBox24, notes);

            notes = new TextBlock[9];
            notes[0] = notes25_1;
            notes[1] = notes25_2;
            notes[2] = notes25_3;
            notes[3] = notes25_4;
            notes[4] = notes25_5;
            notes[5] = notes25_6;
            notes[6] = notes25_7;
            notes[7] = notes25_8;
            notes[8] = notes25_9;
            sudokuBoard[24] = new TextboxModel(textBox25, notes);

            notes = new TextBlock[9];
            notes[0] = notes26_1;
            notes[1] = notes26_2;
            notes[2] = notes26_3;
            notes[3] = notes26_4;
            notes[4] = notes26_5;
            notes[5] = notes26_6;
            notes[6] = notes26_7;
            notes[7] = notes26_8;
            notes[8] = notes26_9;
            sudokuBoard[25] = new TextboxModel(textBox26, notes);

            notes = new TextBlock[9];
            notes[0] = notes27_1;
            notes[1] = notes27_2;
            notes[2] = notes27_3;
            notes[3] = notes27_4;
            notes[4] = notes27_5;
            notes[5] = notes27_6;
            notes[6] = notes27_7;
            notes[7] = notes27_8;
            notes[8] = notes27_9;
            sudokuBoard[26] = new TextboxModel(textBox27, notes);

            notes = new TextBlock[9];
            notes[0] = notes28_1;
            notes[1] = notes28_2;
            notes[2] = notes28_3;
            notes[3] = notes28_4;
            notes[4] = notes28_5;
            notes[5] = notes28_6;
            notes[6] = notes28_7;
            notes[7] = notes28_8;
            notes[8] = notes28_9;
            sudokuBoard[27] = new TextboxModel(textBox28, notes);

            notes = new TextBlock[9];
            notes[0] = notes29_1;
            notes[1] = notes29_2;
            notes[2] = notes29_3;
            notes[3] = notes29_4;
            notes[4] = notes29_5;
            notes[5] = notes29_6;
            notes[6] = notes29_7;
            notes[7] = notes29_8;
            notes[8] = notes29_9;
            sudokuBoard[28] = new TextboxModel(textBox29, notes);

            notes = new TextBlock[9];
            notes[0] = notes30_1;
            notes[1] = notes30_2;
            notes[2] = notes30_3;
            notes[3] = notes30_4;
            notes[4] = notes30_5;
            notes[5] = notes30_6;
            notes[6] = notes30_7;
            notes[7] = notes30_8;
            notes[8] = notes30_9;
            sudokuBoard[29] = new TextboxModel(textBox30, notes);

            notes = new TextBlock[9];
            notes[0] = notes31_1;
            notes[1] = notes31_2;
            notes[2] = notes31_3;
            notes[3] = notes31_4;
            notes[4] = notes31_5;
            notes[5] = notes31_6;
            notes[6] = notes31_7;
            notes[7] = notes31_8;
            notes[8] = notes31_9;
            sudokuBoard[30] = new TextboxModel(textBox31, notes);

            notes = new TextBlock[9];
            notes[0] = notes32_1;
            notes[1] = notes32_2;
            notes[2] = notes32_3;
            notes[3] = notes32_4;
            notes[4] = notes32_5;
            notes[5] = notes32_6;
            notes[6] = notes32_7;
            notes[7] = notes32_8;
            notes[8] = notes32_9;
            sudokuBoard[31] = new TextboxModel(textBox32, notes);

            notes = new TextBlock[9];
            notes[0] = notes33_1;
            notes[1] = notes33_2;
            notes[2] = notes33_3;
            notes[3] = notes33_4;
            notes[4] = notes33_5;
            notes[5] = notes33_6;
            notes[6] = notes33_7;
            notes[7] = notes33_8;
            notes[8] = notes33_9;
            sudokuBoard[32] = new TextboxModel(textBox33, notes);

            notes = new TextBlock[9];
            notes[0] = notes34_1;
            notes[1] = notes34_2;
            notes[2] = notes34_3;
            notes[3] = notes34_4;
            notes[4] = notes34_5;
            notes[5] = notes34_6;
            notes[6] = notes34_7;
            notes[7] = notes34_8;
            notes[8] = notes34_9;
            sudokuBoard[33] = new TextboxModel(textBox34, notes);

            notes = new TextBlock[9];
            notes[0] = notes35_1;
            notes[1] = notes35_2;
            notes[2] = notes35_3;
            notes[3] = notes35_4;
            notes[4] = notes35_5;
            notes[5] = notes35_6;
            notes[6] = notes35_7;
            notes[7] = notes35_8;
            notes[8] = notes35_9;
            sudokuBoard[34] = new TextboxModel(textBox35, notes);

            notes = new TextBlock[9];
            notes[0] = notes36_1;
            notes[1] = notes36_2;
            notes[2] = notes36_3;
            notes[3] = notes36_4;
            notes[4] = notes36_5;
            notes[5] = notes36_6;
            notes[6] = notes36_7;
            notes[7] = notes36_8;
            notes[8] = notes36_9;
            sudokuBoard[35] = new TextboxModel(textBox36, notes);

            notes = new TextBlock[9];
            notes[0] = notes37_1;
            notes[1] = notes37_2;
            notes[2] = notes37_3;
            notes[3] = notes37_4;
            notes[4] = notes37_5;
            notes[5] = notes37_6;
            notes[6] = notes37_7;
            notes[7] = notes37_8;
            notes[8] = notes37_9;
            sudokuBoard[36] = new TextboxModel(textBox37, notes);

            notes = new TextBlock[9];
            notes[0] = notes38_1;
            notes[1] = notes38_2;
            notes[2] = notes38_3;
            notes[3] = notes38_4;
            notes[4] = notes38_5;
            notes[5] = notes38_6;
            notes[6] = notes38_7;
            notes[7] = notes38_8;
            notes[8] = notes38_9;
            sudokuBoard[37] = new TextboxModel(textBox38, notes);

            notes = new TextBlock[9];
            notes[0] = notes39_1;
            notes[1] = notes39_2;
            notes[2] = notes39_3;
            notes[3] = notes39_4;
            notes[4] = notes39_5;
            notes[5] = notes39_6;
            notes[6] = notes39_7;
            notes[7] = notes39_8;
            notes[8] = notes39_9;
            sudokuBoard[38] = new TextboxModel(textBox39, notes);

            notes = new TextBlock[9];
            notes[0] = notes40_1;
            notes[1] = notes40_2;
            notes[2] = notes40_3;
            notes[3] = notes40_4;
            notes[4] = notes40_5;
            notes[5] = notes40_6;
            notes[6] = notes40_7;
            notes[7] = notes40_8;
            notes[8] = notes40_9;
            sudokuBoard[39] = new TextboxModel(textBox40, notes);

            notes = new TextBlock[9];
            notes[0] = notes41_1;
            notes[1] = notes41_2;
            notes[2] = notes41_3;
            notes[3] = notes41_4;
            notes[4] = notes41_5;
            notes[5] = notes41_6;
            notes[6] = notes41_7;
            notes[7] = notes41_8;
            notes[8] = notes41_9;
            sudokuBoard[40] = new TextboxModel(textBox41, notes);

            notes = new TextBlock[9];
            notes[0] = notes42_1;
            notes[1] = notes42_2;
            notes[2] = notes42_3;
            notes[3] = notes42_4;
            notes[4] = notes42_5;
            notes[5] = notes42_6;
            notes[6] = notes42_7;
            notes[7] = notes42_8;
            notes[8] = notes42_9;
            sudokuBoard[41] = new TextboxModel(textBox42, notes);

            notes = new TextBlock[9];
            notes[0] = notes43_1;
            notes[1] = notes43_2;
            notes[2] = notes43_3;
            notes[3] = notes43_4;
            notes[4] = notes43_5;
            notes[5] = notes43_6;
            notes[6] = notes43_7;
            notes[7] = notes43_8;
            notes[8] = notes43_9;
            sudokuBoard[42] = new TextboxModel(textBox43, notes);

            notes = new TextBlock[9];
            notes[0] = notes44_1;
            notes[1] = notes44_2;
            notes[2] = notes44_3;
            notes[3] = notes44_4;
            notes[4] = notes44_5;
            notes[5] = notes44_6;
            notes[6] = notes44_7;
            notes[7] = notes44_8;
            notes[8] = notes44_9;
            sudokuBoard[43] = new TextboxModel(textBox44, notes);

            notes = new TextBlock[9];
            notes[0] = notes45_1;
            notes[1] = notes45_2;
            notes[2] = notes45_3;
            notes[3] = notes45_4;
            notes[4] = notes45_5;
            notes[5] = notes45_6;
            notes[6] = notes45_7;
            notes[7] = notes45_8;
            notes[8] = notes45_9;
            sudokuBoard[44] = new TextboxModel(textBox45, notes);

            notes = new TextBlock[9];
            notes[0] = notes46_1;
            notes[1] = notes46_2;
            notes[2] = notes46_3;
            notes[3] = notes46_4;
            notes[4] = notes46_5;
            notes[5] = notes46_6;
            notes[6] = notes46_7;
            notes[7] = notes46_8;
            notes[8] = notes46_9;
            sudokuBoard[45] = new TextboxModel(textBox46, notes);

            notes = new TextBlock[9];
            notes[0] = notes47_1;
            notes[1] = notes47_2;
            notes[2] = notes47_3;
            notes[3] = notes47_4;
            notes[4] = notes47_5;
            notes[5] = notes47_6;
            notes[6] = notes47_7;
            notes[7] = notes47_8;
            notes[8] = notes47_9;
            sudokuBoard[46] = new TextboxModel(textBox47, notes);

            notes = new TextBlock[9];
            notes[0] = notes48_1;
            notes[1] = notes48_2;
            notes[2] = notes48_3;
            notes[3] = notes48_4;
            notes[4] = notes48_5;
            notes[5] = notes48_6;
            notes[6] = notes48_7;
            notes[7] = notes48_8;
            notes[8] = notes48_9;
            sudokuBoard[47] = new TextboxModel(textBox48, notes);

            notes = new TextBlock[9];
            notes[0] = notes49_1;
            notes[1] = notes49_2;
            notes[2] = notes49_3;
            notes[3] = notes49_4;
            notes[4] = notes49_5;
            notes[5] = notes49_6;
            notes[6] = notes49_7;
            notes[7] = notes49_8;
            notes[8] = notes49_9;
            sudokuBoard[48] = new TextboxModel(textBox49, notes);

            notes = new TextBlock[9];
            notes[0] = notes50_1;
            notes[1] = notes50_2;
            notes[2] = notes50_3;
            notes[3] = notes50_4;
            notes[4] = notes50_5;
            notes[5] = notes50_6;
            notes[6] = notes50_7;
            notes[7] = notes50_8;
            notes[8] = notes50_9;
            sudokuBoard[49] = new TextboxModel(textBox50, notes);

            notes = new TextBlock[9];
            notes[0] = notes51_1;
            notes[1] = notes51_2;
            notes[2] = notes51_3;
            notes[3] = notes51_4;
            notes[4] = notes51_5;
            notes[5] = notes51_6;
            notes[6] = notes51_7;
            notes[7] = notes51_8;
            notes[8] = notes51_9;
            sudokuBoard[50] = new TextboxModel(textBox51, notes);

            notes = new TextBlock[9];
            notes[0] = notes52_1;
            notes[1] = notes52_2;
            notes[2] = notes52_3;
            notes[3] = notes52_4;
            notes[4] = notes52_5;
            notes[5] = notes52_6;
            notes[6] = notes52_7;
            notes[7] = notes52_8;
            notes[8] = notes52_9;
            sudokuBoard[51] = new TextboxModel(textBox52, notes);

            notes = new TextBlock[9];
            notes[0] = notes53_1;
            notes[1] = notes53_2;
            notes[2] = notes53_3;
            notes[3] = notes53_4;
            notes[4] = notes53_5;
            notes[5] = notes53_6;
            notes[6] = notes53_7;
            notes[7] = notes53_8;
            notes[8] = notes53_9;
            sudokuBoard[52] = new TextboxModel(textBox53, notes);

            notes = new TextBlock[9];
            notes[0] = notes54_1;
            notes[1] = notes54_2;
            notes[2] = notes54_3;
            notes[3] = notes54_4;
            notes[4] = notes54_5;
            notes[5] = notes54_6;
            notes[6] = notes54_7;
            notes[7] = notes54_8;
            notes[8] = notes54_9;
            sudokuBoard[53] = new TextboxModel(textBox54, notes);

            notes = new TextBlock[9];
            notes[0] = notes55_1;
            notes[1] = notes55_2;
            notes[2] = notes55_3;
            notes[3] = notes55_4;
            notes[4] = notes55_5;
            notes[5] = notes55_6;
            notes[6] = notes55_7;
            notes[7] = notes55_8;
            notes[8] = notes55_9;
            sudokuBoard[54] = new TextboxModel(textBox55, notes);

            notes = new TextBlock[9];
            notes[0] = notes56_1;
            notes[1] = notes56_2;
            notes[2] = notes56_3;
            notes[3] = notes56_4;
            notes[4] = notes56_5;
            notes[5] = notes56_6;
            notes[6] = notes56_7;
            notes[7] = notes56_8;
            notes[8] = notes56_9;
            sudokuBoard[55] = new TextboxModel(textBox56, notes);

            notes = new TextBlock[9];
            notes[0] = notes57_1;
            notes[1] = notes57_2;
            notes[2] = notes57_3;
            notes[3] = notes57_4;
            notes[4] = notes57_5;
            notes[5] = notes57_6;
            notes[6] = notes57_7;
            notes[7] = notes57_8;
            notes[8] = notes57_9;
            sudokuBoard[56] = new TextboxModel(textBox57, notes);

            notes = new TextBlock[9];
            notes[0] = notes58_1;
            notes[1] = notes58_2;
            notes[2] = notes58_3;
            notes[3] = notes58_4;
            notes[4] = notes58_5;
            notes[5] = notes58_6;
            notes[6] = notes58_7;
            notes[7] = notes58_8;
            notes[8] = notes58_9;
            sudokuBoard[57] = new TextboxModel(textBox58, notes);

            notes = new TextBlock[9];
            notes[0] = notes59_1;
            notes[1] = notes59_2;
            notes[2] = notes59_3;
            notes[3] = notes59_4;
            notes[4] = notes59_5;
            notes[5] = notes59_6;
            notes[6] = notes59_7;
            notes[7] = notes59_8;
            notes[8] = notes59_9;
            sudokuBoard[58] = new TextboxModel(textBox59, notes);

            notes = new TextBlock[9];
            notes[0] = notes60_1;
            notes[1] = notes60_2;
            notes[2] = notes60_3;
            notes[3] = notes60_4;
            notes[4] = notes60_5;
            notes[5] = notes60_6;
            notes[6] = notes60_7;
            notes[7] = notes60_8;
            notes[8] = notes60_9;
            sudokuBoard[59] = new TextboxModel(textBox60, notes);

            notes = new TextBlock[9];
            notes[0] = notes61_1;
            notes[1] = notes61_2;
            notes[2] = notes61_3;
            notes[3] = notes61_4;
            notes[4] = notes61_5;
            notes[5] = notes61_6;
            notes[6] = notes61_7;
            notes[7] = notes61_8;
            notes[8] = notes61_9;
            sudokuBoard[60] = new TextboxModel(textBox61, notes);

            notes = new TextBlock[9];
            notes[0] = notes62_1;
            notes[1] = notes62_2;
            notes[2] = notes62_3;
            notes[3] = notes62_4;
            notes[4] = notes62_5;
            notes[5] = notes62_6;
            notes[6] = notes62_7;
            notes[7] = notes62_8;
            notes[8] = notes62_9;
            sudokuBoard[61] = new TextboxModel(textBox62, notes);

            notes = new TextBlock[9];
            notes[0] = notes63_1;
            notes[1] = notes63_2;
            notes[2] = notes63_3;
            notes[3] = notes63_4;
            notes[4] = notes63_5;
            notes[5] = notes63_6;
            notes[6] = notes63_7;
            notes[7] = notes63_8;
            notes[8] = notes63_9;
            sudokuBoard[62] = new TextboxModel(textBox63, notes);

            notes = new TextBlock[9];
            notes[0] = notes64_1;
            notes[1] = notes64_2;
            notes[2] = notes64_3;
            notes[3] = notes64_4;
            notes[4] = notes64_5;
            notes[5] = notes64_6;
            notes[6] = notes64_7;
            notes[7] = notes64_8;
            notes[8] = notes64_9;
            sudokuBoard[63] = new TextboxModel(textBox64, notes);

            notes = new TextBlock[9];
            notes[0] = notes65_1;
            notes[1] = notes65_2;
            notes[2] = notes65_3;
            notes[3] = notes65_4;
            notes[4] = notes65_5;
            notes[5] = notes65_6;
            notes[6] = notes65_7;
            notes[7] = notes65_8;
            notes[8] = notes65_9;
            sudokuBoard[64] = new TextboxModel(textBox65, notes);

            notes = new TextBlock[9];
            notes[0] = notes66_1;
            notes[1] = notes66_2;
            notes[2] = notes66_3;
            notes[3] = notes66_4;
            notes[4] = notes66_5;
            notes[5] = notes66_6;
            notes[6] = notes66_7;
            notes[7] = notes66_8;
            notes[8] = notes66_9;
            sudokuBoard[65] = new TextboxModel(textBox66, notes);

            notes = new TextBlock[9];
            notes[0] = notes67_1;
            notes[1] = notes67_2;
            notes[2] = notes67_3;
            notes[3] = notes67_4;
            notes[4] = notes67_5;
            notes[5] = notes67_6;
            notes[6] = notes67_7;
            notes[7] = notes67_8;
            notes[8] = notes67_9;
            sudokuBoard[66] = new TextboxModel(textBox67, notes);

            notes = new TextBlock[9];
            notes[0] = notes68_1;
            notes[1] = notes68_2;
            notes[2] = notes68_3;
            notes[3] = notes68_4;
            notes[4] = notes68_5;
            notes[5] = notes68_6;
            notes[6] = notes68_7;
            notes[7] = notes68_8;
            notes[8] = notes68_9;
            sudokuBoard[67] = new TextboxModel(textBox68, notes);

            notes = new TextBlock[9];
            notes[0] = notes69_1;
            notes[1] = notes69_2;
            notes[2] = notes69_3;
            notes[3] = notes69_4;
            notes[4] = notes69_5;
            notes[5] = notes69_6;
            notes[6] = notes69_7;
            notes[7] = notes69_8;
            notes[8] = notes69_9;
            sudokuBoard[68] = new TextboxModel(textBox69, notes);

            notes = new TextBlock[9];
            notes[0] = notes70_1;
            notes[1] = notes70_2;
            notes[2] = notes70_3;
            notes[3] = notes70_4;
            notes[4] = notes70_5;
            notes[5] = notes70_6;
            notes[6] = notes70_7;
            notes[7] = notes70_8;
            notes[8] = notes70_9;
            sudokuBoard[69] = new TextboxModel(textBox70, notes);

            notes = new TextBlock[9];
            notes[0] = notes71_1;
            notes[1] = notes71_2;
            notes[2] = notes71_3;
            notes[3] = notes71_4;
            notes[4] = notes71_5;
            notes[5] = notes71_6;
            notes[6] = notes71_7;
            notes[7] = notes71_8;
            notes[8] = notes71_9;
            sudokuBoard[70] = new TextboxModel(textBox71, notes);

            notes = new TextBlock[9];
            notes[0] = notes72_1;
            notes[1] = notes72_2;
            notes[2] = notes72_3;
            notes[3] = notes72_4;
            notes[4] = notes72_5;
            notes[5] = notes72_6;
            notes[6] = notes72_7;
            notes[7] = notes72_8;
            notes[8] = notes72_9;
            sudokuBoard[71] = new TextboxModel(textBox72, notes);

            notes = new TextBlock[9];
            notes[0] = notes73_1;
            notes[1] = notes73_2;
            notes[2] = notes73_3;
            notes[3] = notes73_4;
            notes[4] = notes73_5;
            notes[5] = notes73_6;
            notes[6] = notes73_7;
            notes[7] = notes73_8;
            notes[8] = notes73_9;
            sudokuBoard[72] = new TextboxModel(textBox73, notes);

            notes = new TextBlock[9];
            notes[0] = notes74_1;
            notes[1] = notes74_2;
            notes[2] = notes74_3;
            notes[3] = notes74_4;
            notes[4] = notes74_5;
            notes[5] = notes74_6;
            notes[6] = notes74_7;
            notes[7] = notes74_8;
            notes[8] = notes74_9;
            sudokuBoard[73] = new TextboxModel(textBox74, notes);

            notes = new TextBlock[9];
            notes[0] = notes75_1;
            notes[1] = notes75_2;
            notes[2] = notes75_3;
            notes[3] = notes75_4;
            notes[4] = notes75_5;
            notes[5] = notes75_6;
            notes[6] = notes75_7;
            notes[7] = notes75_8;
            notes[8] = notes75_9;
            sudokuBoard[74] = new TextboxModel(textBox75, notes);

            notes = new TextBlock[9];
            notes[0] = notes76_1;
            notes[1] = notes76_2;
            notes[2] = notes76_3;
            notes[3] = notes76_4;
            notes[4] = notes76_5;
            notes[5] = notes76_6;
            notes[6] = notes76_7;
            notes[7] = notes76_8;
            notes[8] = notes76_9;
            sudokuBoard[75] = new TextboxModel(textBox76, notes);

            notes = new TextBlock[9];
            notes[0] = notes77_1;
            notes[1] = notes77_2;
            notes[2] = notes77_3;
            notes[3] = notes77_4;
            notes[4] = notes77_5;
            notes[5] = notes77_6;
            notes[6] = notes77_7;
            notes[7] = notes77_8;
            notes[8] = notes77_9;
            sudokuBoard[76] = new TextboxModel(textBox77, notes);

            notes = new TextBlock[9];
            notes[0] = notes78_1;
            notes[1] = notes78_2;
            notes[2] = notes78_3;
            notes[3] = notes78_4;
            notes[4] = notes78_5;
            notes[5] = notes78_6;
            notes[6] = notes78_7;
            notes[7] = notes78_8;
            notes[8] = notes78_9;
            sudokuBoard[77] = new TextboxModel(textBox78, notes);

            notes = new TextBlock[9];
            notes[0] = notes79_1;
            notes[1] = notes79_2;
            notes[2] = notes79_3;
            notes[3] = notes79_4;
            notes[4] = notes79_5;
            notes[5] = notes79_6;
            notes[6] = notes79_7;
            notes[7] = notes79_8;
            notes[8] = notes79_9;
            sudokuBoard[78] = new TextboxModel(textBox79, notes);

            notes = new TextBlock[9];
            notes[0] = notes80_1;
            notes[1] = notes80_2;
            notes[2] = notes80_3;
            notes[3] = notes80_4;
            notes[4] = notes80_5;
            notes[5] = notes80_6;
            notes[6] = notes80_7;
            notes[7] = notes80_8;
            notes[8] = notes80_9;
            sudokuBoard[79] = new TextboxModel(textBox80, notes);

            notes = new TextBlock[9];
            notes[0] = notes81_1;
            notes[1] = notes81_2;
            notes[2] = notes81_3;
            notes[3] = notes81_4;
            notes[4] = notes81_5;
            notes[5] = notes81_6;
            notes[6] = notes81_7;
            notes[7] = notes81_8;
            notes[8] = notes81_9;
            sudokuBoard[80] = new TextboxModel(textBox81, notes);
        }

        private void InitializeSudokuBoard()
        {
            sudokuGame = new SudokuGameModel(sudokuBoard);

            time = timeLabel;

            sudokuGame.SudokuBoard[0].SudokuTextBox.Focus();

            sudokuGame.SudokuBoard[0].SudokuTextBox.Background = new SolidColorBrush(Color.FromRgb(0xb3, 0xe5, 0xfc));

            difficultyLabel.Content = sudokuGame.DifficultyLevel;

        }

        private void GenerateSudokuBoard()
        {
            TextConnector.FullFilePath(MainWindow.difficulty, false).LoadFile().ConvertListToSudokuBoardArray();
        }

        private void GenerateSudokuBoardAnswer()
        {
            TextConnector.FullFilePath(MainWindow.difficulty, true).LoadFile().ConvertListToSudokuBoardAnswerArray();
        }

        private void CheckIfInputIsCorrect()
        {
            if (sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Text != "" && sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Text != sudokuGame.SudokuBoardAnswer[sudokuGame.PrevFocusIndex].Text)
            {
                sudokuGame.NumberOfMistakes++;
                sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Foreground = Brushes.Red;
                mistakesLabel.Content = $"Mistakes: {sudokuGame.NumberOfMistakes}/3";
            }
            else
            {
                sudokuGame.SudokuBoard[sudokuGame.PrevFocusIndex].SudokuTextBox.Foreground = Brushes.RoyalBlue;
            }

            if(sudokuGame.NumberOfMistakes == 3)
            {
                dispatcherTimer.Stop();

                MessageBoxResult result = MessageBox.Show("You have made 3 mistakes and lost this game. Try again?", "Game Over", MessageBoxButton.YesNo);
                if(result == MessageBoxResult.Yes)
                {
                    this.Close();
                }
                else
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
        }

        private bool IsGameComplete()
        {
            for (int i = 0; i < 81; i++)
            {
                if (sudokuGame.SudokuBoard[i].SudokuTextBox.Text != sudokuGame.SudokuBoardAnswer[i].Text)
                {
                    return false;
                }
            }

            return true;
        }

        private void StartTimer()
        {
            sudokuGame.FinishSetup = true;

            AddToStack();

            dispatcherTimer.Start();
        }

        private void AddToStack()
        {
            string[,] temp = new string[81,10];
            for(int i = 0; i < 81; i++)
            {
                temp[i,0] += sudokuGame.SudokuBoard[i].SudokuTextBox.Text;

                for (int j = 0; j < 9; j++)
                {
                    temp[i,j+1] += (sudokuGame.SudokuBoard[i].SudokuNotes[j].Visibility == Visibility.Hidden) ? "0" : "1";
                }
            }

            sudokuGame.SudokuBoardStack.Push(temp);
        }
    }
}
