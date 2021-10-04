using System;
using System.Linq;
using System.Text;
using System.Timers;
using System.Collections.Generic;

/*
  TODO: implement this when not in Replit Environment
  https://stackoverflow.com/questions/38416265/c-sharp-checking-if-a-word-is-in-an-english-dictionary
  Use NetSpell to make sure the word inputs are actual words
*/

public class Boggle {
  class Cell {
    public int Row { get; }
    public int Col { get; }
    public Cell(int row, int col){
      Row = row;
      Col = col;
    }
  }

  private readonly int _rows = 5;
  private readonly int _cols = 5;
  private readonly int _maxLengthWord;
  private string[][] _board;
  private HashSet<string> _foundWords;
  private Timer _timer;
  private Random _r;
  private int _score;
  private bool _gameComplete;

  // all 25, random letter dice
  private string[][] _dice = new string[][] {
    new string[]{"A", "A", "A", "F", "R", "S"},
    new string[]{"A", "A", "E", "E", "E", "E"},
    new string[]{"A", "A", "F", "I", "R", "S"},
    new string[]{"A", "D", "E", "N", "N", "N"},
    new string[]{"A", "E", "E", "E", "E", "M"},
    new string[]{"A", "E", "E", "G", "M", "U"},
    new string[]{"A", "E", "G", "M", "N", "N"},
    new string[]{"A", "F", "I", "R", "S", "Y"},
    new string[]{"B", "J", "K", "Qu", "X", "Z"},
    new string[]{"C", "C", "E", "N", "S", "T"},
    new string[]{"C", "E", "I", "I", "L", "T"},
    new string[]{"C", "E", "I", "L", "P", "T"},
    new string[]{"C", "E", "I", "P", "S", "T"},
    new string[]{"D", "D", "H", "N", "O", "T"},
    new string[]{"D", "H", "H", "L", "O", "R"},
    new string[]{"D", "H", "L", "N", "O", "R"},
    new string[]{"D", "H", "L", "N", "O", "R"},
    new string[]{"E", "I", "I", "I", "T", "T"},
    new string[]{"E", "M", "O", "T", "T", "T"},
    new string[]{"E", "N", "S", "S", "S", "U"},
    new string[]{"F", "I", "P", "R", "S", "Y"},
    new string[]{"G", "O", "R", "R", "V", "W"},
    new string[]{"I", "P", "R", "R", "R", "Y"},
    new string[]{"N", "O", "O", "T", "U", "W"},
    new string[]{"O", "O", "O", "T", "T", "U"}
  };

  /*
    scoring bank, make sure that any word longer than 8 is
    truncated down to 8 for proper scoring
    KVP: K = word length, V = score
  */
  private Dictionary<int, int> _scoreBank = new Dictionary<int, int>() {
    [3] = 1,
    [4] = 1,
    [5] = 2,
    [6] = 3,
    [7] = 5,
    [8] = 11
  };

  // all adjacent dice in 8 directions
  private int[][] _directions = new int[][] {
    new int[]{-1, 0},
    new int[]{0, -1},
    new int[]{-1, -1},
    new int[]{-1, 1},
    new int[]{1, -1},
    new int[]{1, 1},
    new int[]{1, 0},
    new int[]{0, 1}
  };

  public Boggle(int gameTime) {
    _score = 0;
    _foundWords = new HashSet<string>();
    _r = new Random();
    _maxLengthWord = _rows * _cols;
    GenerateRandomBoard();
    SetupTimer(gameTime);
  }

  public void StartGame(){
    ResetGame();
    PrintBoard();
    Console.WriteLine("\n\n");
    StartGameTimer();
  }

  public int GetFoundWordsCount() {
    return _foundWords.Count;
  }

  public int GetScore () {
    return _score;
  }

  public bool FindWord(string word) {
    if(_gameComplete){
      Console.WriteLine("GAME OVER");
      return false;
    }

    if(word == null || word.Length < 3){
      Console.WriteLine("Too Short!");
      return false;
    }

    if (_foundWords.Contains(word)) {
      Console.WriteLine("Already Found!");
      return true;
    }

    if (word.Length > _maxLengthWord) {
      Console.WriteLine("Too Long!");
      return false;
    }

    for (int row = 0; row < _rows; row++) {
      for (int col = 0; col < _cols; col++) {
        if (_board[row][col] == word[0].ToString()) {
          if (ValidateWord(word, 0, row, col)) {
            int wordLength = (word.Length > 8 ? 8 : word.Length);
            _foundWords.Add(word);
            _score += _scoreBank[wordLength];
            return true;
          }
        }
      }
    }
    return false;
  }

  public void PrintBoard() {
    for (int row = 0; row < _rows; row++) {
      Console.WriteLine("\n");
      for (int col = 0; col < _cols; col++) {
        Console.Write($"\t{_board[row][col]}\t");
      }
      Console.WriteLine("\n");
    }
  }

  public void PrintScoreBank() {
    Console.WriteLine("\nScore Card");
    Console.WriteLine("================");
    foreach (KeyValuePair<int, int> scoreEntry in _scoreBank) {
      Console.WriteLine($"{scoreEntry.Key} Letter Words: {scoreEntry.Value} Points");
    }
  }

  public void PrintAllFoundWords() {
    foreach(string word in _foundWords){
      Console.WriteLine($"{word}");
    }
  }

  public int GetFoundWordCount() {
    return _foundWords.Count;
  }

  private void StartGameTimer() {
    Console.WriteLine("START!\n");
    _gameComplete = false;
    _timer.Start();
    int inputMovingBoardCount = 0;
    while(!_gameComplete){
      if(inputMovingBoardCount >= 5){
        PrintBoard();
        Console.WriteLine("\n\n");
        inputMovingBoardCount = 0;
      }
      string input = Console.ReadLine();
      string word = CleanWord(input);
      Console.WriteLine(FindWord(word) == true ? "GREAT!\n" : "");
      inputMovingBoardCount++;
    }
    Console.ReadKey();
  }

  private void GenerateRandomBoard() {
    _board = new string[][] {
      new string[_cols],
      new string[_cols],
      new string[_cols],
      new string[_cols],
      new string[_cols]
    };
    HashSet<int> usedDice = new HashSet<int>();
    usedDice.Add(-1);

    for (int row = 0; row < _rows; row++) {
      for (int col = 0; col < _cols; col++) {
        int randomDice = -1;
        while (usedDice.Contains(randomDice)) {
          randomDice = _r.Next(0, _maxLengthWord);
        }
        usedDice.Add(randomDice);
        int randomFace = _r.Next(0, 6);
        _board[row][col] = _dice[randomDice][randomFace];
      }
    }
  }

  private string CleanWord(string word){
    string input = word;
    string output = string.Concat(input.Where(char.IsLetter));
    return output.ToUpper();
  }

  private void SetupTimer(int seconds) {
    int gameLength = seconds * 1000;
    _timer = new Timer();
    _timer.Interval = Convert.ToDouble(gameLength);
    _timer.Elapsed += FinishGameEvent;
    _timer.Enabled = true;
    _timer.AutoReset = false;
  }

  private bool ValidateWord (string word, int index, int row, int col) {
    if(!IsInBounds(row, col)){
      return false;
    }

    if (index >= word.Length) {
      return true;
    }

    if(word[index].ToString() != _board[row][col]){
      return false;
    }

    string currentLetter = _board[row][col];
    _board[row][col] = "_";

    HashSet<bool> isWordCompletelyFound = new HashSet<bool>();
    foreach(Cell adjacent in GetAdjacentCells(row, col)){
      int adjacentRow = adjacent.Row;
      int adjacentCol = adjacent.Col;
      isWordCompletelyFound.Add(ValidateWord(word, index + 1, adjacentRow, adjacentCol));
    }

    _board[row][col] = currentLetter;
    return isWordCompletelyFound.Contains(true);
  }

  private List<Cell> GetAdjacentCells(int row, int col){
    List<Cell> adjacentCells = new List<Cell>();

    foreach (int[] direction in _directions) {
      int adjacentRow = row + direction[0];
      int adjacentCol = col + direction[1];
      if (IsInBounds(adjacentRow, adjacentRow)) {
        adjacentCells.Add(new Cell(adjacentRow, adjacentCol));
      }
    }

    return adjacentCells;
  }

  private bool IsInBounds(int row, int col){
    if (row < 0 || col < 0 || row >= _board.Length || col >= _board[row].Length) {
      return false;
    }
    return true;
  }

  private void PrintEndGameMessage() {
    PrintScoreBank();
    Console.WriteLine("\n");
    Console.WriteLine("WORDS FOUND:");
    Console.WriteLine("================");
    PrintAllFoundWords();
    Console.WriteLine("\n");
    Console.WriteLine("FINAL SCORE:");
    Console.WriteLine("================");
    Console.WriteLine(GetScore() + "\n");
    Console.WriteLine("FINISH!");
  }

  private void ResetGame(){
    _foundWords.Clear();
    _score = 0;
    GenerateRandomBoard();
  }

  private void FinishGameEvent(object source, EventArgs e) {
    Console.Beep(261, 2000); // middle C for 2 seconds
    _gameComplete = true;
    PrintEndGameMessage();
  }
}
