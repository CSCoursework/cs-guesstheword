using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace guessTheWord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private char[] _currentWord;
        private string[] _wordList;
        private List<char> _chosenLetters = new List<char>();
        private readonly Random _random = new Random();
        private int _livesRemaining;
        private readonly int _maxLives = 5;
        
        public MainWindow()
        {
            InitializeComponent();

            var text = "";
            
            try
            {
                text = File.ReadAllText(@"dictionary.txt");                
            } catch (FileNotFoundException)
            {
                MessageBox.Show("Dictionary file not found.");
                Environment.Exit(2); // ERROR_FILE_NOT_FOUND
            }
            
            _wordList = text.Split('\n');
        }

        private void SetDisplayNewGame()
        {
            _chosenLetters = new List<char>();
            PlayButton.Visibility = Visibility.Visible;
            WordReadoutTextBlock.Visibility = Visibility.Hidden;
            LivesReadoutTextBox.Visibility = Visibility.Hidden;
            LetterGuessTextBox.Visibility = Visibility.Hidden;
        }

        private void NewGame()
        {
            // Hide button
            PlayButton.Visibility = Visibility.Hidden;
            // Generate word
            GenerateWord();
            // Show word text
            WordReadoutTextBlock.Text = GenerateDisplayedWord();
            WordReadoutTextBlock.Visibility = Visibility.Visible;
            // Show lives text
            _livesRemaining = _maxLives;
            LivesReadoutTextBox.Text = "Lives remaining: " + _livesRemaining;
            LivesReadoutTextBox.Visibility = Visibility.Visible;
            // Show entry box
            LetterGuessTextBox.Visibility = Visibility.Visible;
        }
        
        private void GenerateWord() 
        {
            // GenerateWord picks a random word from the list
            
            // Pick random word
            var word = _wordList[_random.Next(_wordList.Length)].Trim();
            // Split into an array
            _currentWord = word.ToCharArray();
        }

        private string GenerateDisplayedWord()
        {

            // Censors letters that have not yet been guessed
            
            var thing = new char[_currentWord.Length];
            
            for (var i = 0; i < _currentWord.Length; i++)
            {
                var currentChar = _currentWord[i];
                char toAppend;
                
                if (!_chosenLetters.Contains(currentChar))
                {
                    toAppend = '-';
                }
                else
                {
                    toAppend = currentChar;
                }

                thing[i] = toAppend;

            }

            return String.Join(" ", thing);
        }
        
        private void LetterGuessTextBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            var boxVal = LetterGuessTextBox.Text;
            
            if (boxVal == "")
            {
                return;
            }

            var numOfLetters = Regex.Matches(boxVal,@"[a-zA-Z]").Count;
            if (numOfLetters == 0)
            {
                MessageBox.Show("Must only be letters");
                LetterGuessTextBox.Text = "";
                return;
            }

            var boxChar = char.ToLower(LetterGuessTextBox.Text[0]);
            
            LetterGuessTextBox.Text = "";
            
            if (_chosenLetters.Contains(boxChar))
            {
                MessageBox.Show("You've already guessed this letter!");
                return;
            }

            _chosenLetters.Add(boxChar);

            if (!_currentWord.Contains(boxChar))
            {
                _livesRemaining--;
                LivesReadoutTextBox.Text = "Lives remaining: " + _livesRemaining;
            }

            if (_livesRemaining == 0)
            {
                MessageBox.Show("Game over! The word was '" + String.Join("", _currentWord) + "'");
                SetDisplayNewGame();
                return;
            }
            
            var generatedWord = GenerateDisplayedWord();
            WordReadoutTextBlock.Text = GenerateDisplayedWord();

            // Check if the word is completed:
            if (generatedWord == String.Join(" ", _currentWord))
            {
                MessageBox.Show("Word correct!");
                SetDisplayNewGame();
            }
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            NewGame();
        }
    }
}