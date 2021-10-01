using System;
using System.Timers;
using System.Collections.Generic;

public class Boggle {
  private readonly int _rows = 5;
  private readonly int _cols = 5;
  private readonly int _maxLengthWord;
  private string[][] _board;
  private HashSet<string> _foundWords;
  private Timer _timer;
  private Random _r;
  private int _score;

  // all 25, random letter dice
  private string[][] dice = new string[][] {
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
  private Dictionary<int, int> scoreBank = new Dictionary<int, int>() {
    [3] = 1,
    [4] = 1,
    [5] = 2,
    [6] = 3,
    [7] = 5,
    [8] = 11
  };

  // all adjacent dice in 8 directions
  private int[][] directions = new int[][] {
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

  public void StartNewGame(){
    _foundWords.Clear();
    _score = 0;
    GenerateRandomBoard();
  }

  public int GetFoundWordsCount() {
    return _foundWords.Count;
  }

  public int GetScore () {
    return _score;
  }

  public bool FindWord(string word) {
    if (_foundWords.Contains(word)) {
      return true;
    }

    if (word.Length > _maxLengthWord) {
      return false;
    }

    for (int row = 0; row < _rows; row++) {
      for (int col = 0; col < _cols; col++) {
        if (_board[row][col] == word[0].ToString()) {
          if (ValidateWord(word, 0, row, col)) {
            _foundWords.Add(word);
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

  public void PrintAllFoundWords() {
    foreach(string word in _foundWords){
      Console.WriteLine($"{word}");
    }
  }

  public int GetFoundWordCount() {
    return _foundWords.Count;
  }

  public void StartGameTimer() {
    Console.WriteLine("START!");
    _timer.Start();
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
        _board[row][col] = dice[randomDice][randomFace];
      }
    }
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
    if (index >= word.Length) {
      return true;
    }

    if(word[index] != _board[row][col]){
      return false;
    }

  }

  private static void FinishGameEvent(object source, EventArgs e) {
    Console.Beep(261, 2000); // middle C for 2 seconds
    Console.WriteLine("FINISH!");
  }
}
