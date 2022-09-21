using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SudokuGame.Models
{
    public class SudokuGameModel
    {
        public TextboxModel[] SudokuBoard { get; set; }

        public TextBox[] SudokuBoardAnswer { get; set; }

        public string DifficultyLevel { get; set; }

        public int Time { get; set; }

        public int NumberOfMistakes { get; set; }

        public int NumberOfHints { get; set; }

        public bool NotesOn { get; set; }

        public bool FinishSetup { get; set; }

        public Stack<string[,]> SudokuBoardStack { get; set; }

        public int PrevFocusIndex { get; set; }

        public Stack<int> PrevFocusIndexStack { get; set; }

        public SudokuGameModel(TextboxModel[] sudokuBoard)
        {
            SudokuBoard = sudokuBoard;
            SudokuBoardAnswer = new TextBox[81];
            DifficultyLevel = MainWindow.difficulty;
            SudokuBoardStack = new Stack<string[,]>();
            NumberOfMistakes = 0;
            NumberOfHints = 3;
            Time = 0;
            PrevFocusIndex = 0;
            NotesOn = false;
            PrevFocusIndexStack = new Stack<int>();
            FinishSetup = false;

        }
    }
}
