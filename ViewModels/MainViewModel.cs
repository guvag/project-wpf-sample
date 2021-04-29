using LCRSimulatorApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LCRSimulatorApp.ViewModels
{
    public  class MainViewModel: INotifyPropertyChanged
    {
        private Game _game;
        public MainViewModel()
        {
            _game = new Game();
        }

        #region Input Properties

        public int PlayerNumber
        {
            get { return _game.PlayerNumber; }
            set
            {
                _game.PlayerNumber = value;
            }
        }

        private int _gameNumber;
        public int GameNumber
        {
            get { return _game.GameNumber; }
            set
            {
                _game.GameNumber = value;
            }
        }

        private string _error = "";
        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                OnPropertyChange("Error");
            }
        }

        private ICommand _startCommand;
        public ICommand StartCommand
        {
            get
            {
                _startCommand = new CommandHandler(StartGame, true);
                return _startCommand;
            }
        }

        #endregion

        #region Game Properties

        private int _playerNumberInTurn;
        public int PlayerNumberInTurn
        {
            get { return _playerNumberInTurn; }
            set
            {
                _playerNumberInTurn = value;
                OnPropertyChange("PlayerNumberInTurn");
            }
        }

        private int _turnNumber;
        public int TurnNumber
        {
            get { return _turnNumber; }
            set
            {
                _turnNumber = value;
                OnPropertyChange("TurnNumber");
            }
        }

        private int _matchNumberInTurn;
        public int MatchNumberInTurn
        {
            get { return _matchNumberInTurn; }
            set
            {
                _matchNumberInTurn = value;
                OnPropertyChange("MatchNumberInTurn");
            }
        }

        private int _chipsNumberInTurn;
        public int ChipsNumberInTurn
        {
            get { return _chipsNumberInTurn; }
            set
            {
                _chipsNumberInTurn = value;
                OnPropertyChange("ChipsNumberInTurn");
            }
        }

        private int _centerChipsNumberInTurn;
        public int CenterChipsNumberInTurn
        {
            get { return _centerChipsNumberInTurn; }
            set
            {
                _centerChipsNumberInTurn = value;
                OnPropertyChange("CenterChipsNumberInTurn");
            }
        }

        private string _rollDiceResults;
        public string RollDiceResults
        {
            get { return _rollDiceResults; }
            set
            {
                _rollDiceResults = value;
                OnPropertyChange("RollDiceResults");
            }
        }

        private int _gamePlayersNumber;
        public int GamePlayersNumber
        {
            get { return _gamePlayersNumber; }
            set
            {
                _gamePlayersNumber = value;
                OnPropertyChange("GamePlayersNumber");
            }
        }

        public int LongestTurnNumber
        {
            get { return _game.LongestTurnNumber; }
            set
            {
                _game.LongestTurnNumber = value;
                OnPropertyChange("LongestTurnNumber");
            }
        }

        public int ShortestTurnNumber
        {
            get { return _game.ShortestTurnNumber; }
            set
            {
                _game.ShortestTurnNumber = value;
                OnPropertyChange("ShortestTurnNumber");
            }
        }

        public double Average
        {
            get { return _game.Average; }
            set
            {
                _game.Average = value;
                OnPropertyChange("Average");
            }
        }

        #endregion

        #region Simulation Methods

        private Random myDice = new Random();
        public int RollStandardDice()
        {
            return myDice.Next(1, 6 + 1);
        }

        public void StartGame()
        {
            if(_game.PlayerNumber < 3)
            {
                Error = "Number of players must be 3 or more";
            } else  if (_game.GameNumber < 1)
                {
                    Error = "Number of games must be 1 or more";
            }
            else
            {
                Console.WriteLine("Start Game");
                Error = "";
                

                SetGameInputs(_game.PlayerNumber, _game.GameNumber);

                Thread thread = new Thread(SimulateMatches);
                thread.Start();

                
            }
            
        }

        public void SetGameInputs(int playerNumber, int matchNumber)
        {
            //Set the player list
            _game.PlayerList = new List<Player>();
            for (int i = 0; i < playerNumber; i++)
            {
                Player player = new Player();
                player.PlayerIndex = i + 1;
                player.ChipNumber = 3;
                _game.PlayerList.Add(player);
            }

            GamePlayersNumber = _game.PlayerList.Count;

            //Set the match list
            _game.MatchList = new List<Match>();
            for (int i = 0; i < matchNumber; i++)
            {
                Match match = new Match();
                match.MatchNumber = i+1;
                _game.MatchList.Add(match);
            }

            // Set the Dice dictionary Values
            _game.Dice = new Dictionary<int, string>();
            _game.Dice.Add(1, "L");
            _game.Dice.Add(2, ".");
            _game.Dice.Add(3, "C");
            _game.Dice.Add(4, ".");
            _game.Dice.Add(5, "R");
            _game.Dice.Add(6, ".");

        }

        public void SimulateMatches()
        {
            LinkedList<Player> intLinkedList = new LinkedList<Player>(_game.PlayerList);

            foreach (Match match in _game.MatchList)
            {
                int turn = 1;
                bool matchActive = true;

                Console.WriteLine(string.Format("Match {0}", match.MatchNumber));
                MatchNumberInTurn = match.MatchNumber;

                while (matchActive)
                {

                    

                    foreach (Player player in _game.PlayerList)
                    {
                        Console.WriteLine(string.Format("Turn {0}", turn));

                        TurnNumber = turn;
                        PlayerNumberInTurn = player.PlayerIndex;
                        ChipsNumberInTurn = player.ChipNumber;

                        Console.WriteLine(string.Format("Player {0}, Chips: {1}", player.PlayerIndex, player.ChipNumber));

                        string rollResult = "";

                        int diceNumber = 3;

                        for (int i=0; i < diceNumber; i++)
                        {
                            int res = RollStandardDice();
                            string value = _game.Dice[res];


                            if (value.Equals("L"))
                            {
                                Player next;
                                if (intLinkedList.Last.Value.PlayerIndex == player.PlayerIndex)
                                {
                                    next = intLinkedList.First.Value;
                                }
                                else
                                {

                                    next = intLinkedList.Find(intLinkedList.FirstOrDefault(n => n.PlayerIndex == player.PlayerIndex)).Next.Value;
                                }

                                Console.WriteLine("Next Player " + next.PlayerIndex);

                                var players = _game.PlayerList.Where(c => c.PlayerIndex == next.PlayerIndex).ToList();
                                players.ForEach(c => c.ChipNumber = c.ChipNumber + 1);
                                player.ChipNumber = player.ChipNumber - 1;
                            }
                            else if (value.Equals("R"))
                            {
                                Player prev;
                                if (intLinkedList.First.Value.PlayerIndex == player.PlayerIndex)
                                {
                                    prev = intLinkedList.Last.Value;
                                }
                                else
                                {
                                    prev = intLinkedList.Find(intLinkedList.FirstOrDefault(n => n.PlayerIndex == player.PlayerIndex)).Previous.Value;
                                }

                                Console.WriteLine("Previous Player " + prev.PlayerIndex);

                                var players = _game.PlayerList.Where(c => c.PlayerIndex == prev.PlayerIndex).ToList();
                                players.ForEach(c => c.ChipNumber = c.ChipNumber + 1);
                                player.ChipNumber = player.ChipNumber - 1;

                            }

                            else if (value.Equals("C"))
                            {
                                player.ChipNumber = player.ChipNumber - 1;
                                match.CenterChipNumber = match.CenterChipNumber + 1;
                                CenterChipsNumberInTurn = match.CenterChipNumber;
                            }
                            else
                            {
                                Console.WriteLine("Dice vlue is Dot");
                            }

                            rollResult = string.Concat(rollResult, " ", value);

                            Thread.Sleep(6000);
                        } // for roll dice

                        RollDiceResults = rollResult;
                        turn++;

                        var validWinner = _game.PlayerList.Where(c => c.ChipNumber > 0).ToList();
                        if (validWinner.Count > 1)
                        {
                            Console.WriteLine("No winners in this turn");

                        }
                        else
                        {
                            match.Winner = validWinner.FirstOrDefault();
                            match.TurnNumber = turn;
                            Console.WriteLine(string.Format("Winner is Player {0}", match.Winner.PlayerIndex));
                            matchActive = false;
                            break;
                        }
                        Thread.Sleep(6000);
                    } // for Player turns

                    Thread.Sleep(6000);
                }  //while match active


            }// foreach matches

            SetResults();
        }

        public void SetResults()
        {
            
                LongestTurnNumber = _game.MatchList.Max(t => t.TurnNumber);
                ShortestTurnNumber = _game.MatchList.Min(t => t.TurnNumber);
                Average = _game.MatchList.Average(t => t.TurnNumber);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    #region Command Handler
    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }

    #endregion
}
