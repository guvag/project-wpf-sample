using LCRSimulatorApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

                while (matchActive)
                {

                    

                    foreach (Player player in _game.PlayerList)
                    {
                        Console.WriteLine(string.Format("Turn {0}", turn));

                        Console.WriteLine(string.Format("Player {0}, Chips: {1}", player.PlayerIndex, player.ChipNumber));

                        for (int i=0; i < player.ChipNumber; i++)
                        {
                            int res = RollStandardDice();
                            string value = _game.Dice[res];


                            if (value.Equals("L"))
                            {
                                Player next = intLinkedList.Find(intLinkedList.FirstOrDefault(n => n.PlayerIndex == player.PlayerIndex)).Next.Value;
                                Console.WriteLine("Next Player " + next.PlayerIndex);
                                var players = _game.PlayerList.Where(c => c.PlayerIndex == next.PlayerIndex).ToList();
                                players.ForEach(c => c.ChipNumber = c.ChipNumber + 1);
                                player.ChipNumber = player.ChipNumber - 1;
                            }
                            else if (value.Equals("R"))
                            {
                                Player prev = intLinkedList.Find(intLinkedList.FirstOrDefault(n => n.PlayerIndex == player.PlayerIndex)).Previous.Value;
                                Console.WriteLine("Previoues Player " + prev.PlayerIndex);

                                var players = _game.PlayerList.Where(c => c.PlayerIndex == prev.PlayerIndex).ToList();
                                players.ForEach(c => c.ChipNumber = c.ChipNumber + 1);
                                player.ChipNumber = player.ChipNumber - 1;

                            }

                            else if (value.Equals("C"))
                            {
                                player.ChipNumber = player.ChipNumber - 1;
                                match.CenterChipNumber = match.CenterChipNumber + 1;
                            }
                            else
                            {
                                Console.WriteLine("Dice vlue is Dot");
                            }

                        } // for roll dice
                        turn++;

                        var validWinner = _game.PlayerList.Where(c => c.ChipNumber > 0).ToList();
                        if (validWinner.Count > 1)
                        {

                        }

                    } // for Player turns

                    
                }  //while match active


            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        
    }

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
}
