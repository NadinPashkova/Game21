using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Game21
{
	class Program
	{
		enum AnswerType
		{
			NO,
			YES
		}

		enum PlayerName
		{
			Computer,
			You
		}

		enum Suit
		{
			Spades,
			Clubs,
			Hearts,
			Diamonds
		}

		enum Rank
		{
			Six = 6,
			Seven,
			Eight,
			Nine,
			Ten,
			Jack = 2,
			Qween,
			King,
			Ace = 11
		}

		struct Card
		{
			public Suit Suit;
			public Rank Rank;
			public bool isUsed;
		}

		struct Player
		{
			public PlayerName PlayerName;
			public int PlayerScore;
			public Card[] PlayerDeck;
			public bool HasTwoAces;
			public bool FinishGame;
			public int NumberOfWins;
		}

		static bool RecieveYesOrNoAnswer()
		{
			int attemp = 3;

			string answ = string.Empty;

			do
			{
				Console.WriteLine(">>Enter Yes or No");

				answ = Console.ReadLine().ToUpper();

				attemp--;

				if (attemp == 0)
				{
					Console.WriteLine("\r\n >>You failed to get a correct answer! Game Over...");
					Console.WriteLine(">>Press any key to exit");
					Environment.Exit(0);
				}

			} 	while (!Enum.IsDefined(typeof(AnswerType), answ));
		
			return Convert.ToBoolean(Enum.Parse(typeof(AnswerType), answ)); 
		}

		static void FillInDeck(Card[] deck)
		{
			int index = 0;

			foreach (Suit suit in Enum.GetValues(typeof(Suit)))
			{
				foreach (Rank rank in Enum.GetValues(typeof(Rank)))
				{
					deck[index] = new Card { Suit = suit, Rank = rank };

					index++;
				}
			}
		}

		static int CalculateValue(Card[] deck)
		{
			int value = 0;

			foreach (var card in deck)
			{
				value += (int)card.Rank;
			}

			return value;
		}

		static bool HasDoubleAces(Card[] deck)
		{
			int acesCount = 0;

			foreach (var card in deck)
			{
				if (card.Rank.Equals(Rank.Ace))
					acesCount++;
			}

			return acesCount >= 2 ? true : false;
		}

		static Card[] GetCard(ref Card[] deck, int NumberOfCard)
		{
			Card[] pickedCard = new Card[NumberOfCard];

			Random rn = new Random();

			Card[] notUsedCard = new Card[] { };

			for (int i = 0; i < deck.Length; i++)
			{
				if (!deck[i].isUsed)
				{
					Array.Resize(ref notUsedCard, notUsedCard.Length + 1);

					notUsedCard[notUsedCard.Length-1] = deck[i];
				}
			}

			for (int i = 0; i < NumberOfCard; i++)
			{
				var cardIndex = rn.Next(0, notUsedCard.Length - 1);

				pickedCard[i] = notUsedCard[cardIndex];

				for (int j = 0; j < deck.Length; j++)
				{
					if (deck[j].Rank == notUsedCard[cardIndex].Rank && deck[j].Suit == notUsedCard[cardIndex].Suit)
					{
						deck[j].isUsed = true;

						break;
					}
				}
			}

			//deck = deck.Except(pickedCard).ToArray();

			return pickedCard;
		}

		static PlayerName ChooseFirstPlayer()
		{
			int playerNumber = 0;
			int attemp = 3;
			string firstPlayer = string.Empty;

			Console.WriteLine("<<----- GAME 21 ------>>");

			Console.WriteLine("\r\n>>Please choose who will get cards first.");

			do
			{
				Console.WriteLine("0 - Computer, 1 - You. Enter the number.");

				firstPlayer = Console.ReadLine();

				attemp--;

				if (attemp == 0)
				{
					Console.WriteLine("You failed to enter correct number! Gave Over...");
					Console.ReadKey();
					Environment.Exit(0);
				}
			}
			while (!int.TryParse(firstPlayer, out playerNumber) || (!Enum.IsDefined(typeof(PlayerName), playerNumber)));

			return (PlayerName)Enum.ToObject(typeof(PlayerName), playerNumber);
		}

		static void AddCardsToPlayers(ref Card[] originalDeck, Card[] cardsToAdd)
		{
			var index = originalDeck.Length;

			Array.Resize(ref originalDeck, originalDeck.Length + cardsToAdd.Length);

			foreach (var card in cardsToAdd)
			{
				originalDeck[index] = card;
				index++;
			}
		}

		static string PrintCard(Card[] deck)
		{
			string cards = string.Empty;

			foreach (var card in deck)
			{
				string rank = string.Empty;
				string symb = "";
				
				switch (card.Rank)
				{
					case Rank.Six:
					case Rank.Seven:
					case Rank.Eight:
					case Rank.Nine:
					case Rank.Ten:
						rank = ((int)card.Rank).ToString();
						break;
					case Rank.Jack:
						rank = "J";
						break;
					case Rank.Qween:
						rank = "Q";
						break;
					case Rank.King:
						rank = "K";
						break;
					case Rank.Ace:
						rank = "A";
						break;

				};

				switch (card.Suit)
				{

					case Suit.Clubs:
						symb = "\u2663";//'♣';
						break;
					case Suit.Diamonds:
						symb = "\u2660";
						break;
					case Suit.Hearts:
						symb = "\u2665";
						break;
					case Suit.Spades:
						symb = "\u2666";
						break;
				}

				cards += $@"[{rank} {symb}] ";
			}

			return cards;
		}

		static bool ProceedTheGame(PlayerName name)
		{
			bool result;

			Random rn = new Random();

			if (name.Equals(PlayerName.Computer))
			{
				Console.WriteLine($"\r\n >>Computer will decide whether to continue the game or not: ");

				result = rn.Next(0, 2) == 1;

				Console.Write($">>Computer has chosen to {(result ? "continue" : "stop" )} playing\r\n");

				return result;
			}

			Console.WriteLine("\r\n>>Would you like yo get one more card?");

			return RecieveYesOrNoAnswer();
		}

		static void ChooseTheWinner(ref Player[] players, int round)
		{
			string result = null;

			if (players[0].FinishGame && players[1].FinishGame)
			{
				for (int i = 0; i < players.Length; i++)
				{
					if (players[0].HasTwoAces)
						players[0].PlayerScore = 21;
				}

				if (players[0].PlayerScore > players[1].PlayerScore)
				{
					result = players[1].PlayerName.ToString();
					players[1].NumberOfWins += 1;
				}
				else if (players[0].PlayerScore == players[1].PlayerScore)
				{
					result = "Draw! Both players are winners!";
					players[0].NumberOfWins += 1;
					players[1].NumberOfWins += 1;

				}
				else
				{
					result = players[0].PlayerName.ToString();
					players[0].NumberOfWins += 1;
				}
			}
			else if (players[0].FinishGame || players[1].FinishGame)
			{
				if (players[0].FinishGame)
					players[0].NumberOfWins += 1;
				else
					players[1].NumberOfWins += 1;

				result = (players[0].FinishGame ? players[0].PlayerName : players[1].PlayerName).ToString();
			}

			result = result ?? "Nobody reached 21 - there is no winner !";

			Console.ForegroundColor = ConsoleColor.Red;

			Console.WriteLine($"\r\n>>Winner of {round} round : {result} ");

			Console.ResetColor();
		}

		static void Main()
		{
			Console.OutputEncoding = Encoding.UTF8;

			bool startNewGame = false;

			Player computer = new Player { PlayerName = PlayerName.Computer };
						
			Player human = new Player { PlayerName = PlayerName.You };

			Player[] playersArr = new Player[2];

			do
			{
				bool computerFirst = ChooseFirstPlayer().Equals(PlayerName.Computer);

				playersArr[0] = computerFirst ? computer : human;

				playersArr[1] = computerFirst ? human : computer;

				playersArr[0].PlayerDeck = new Card[] { };

				playersArr[1].PlayerDeck = new Card[] { };

				Game21(ref playersArr);

				Console.WriteLine("\r\n>>Would you like to start a new game?");

				startNewGame = RecieveYesOrNoAnswer();

			} while (startNewGame);

			Console.ForegroundColor = ConsoleColor.Green;

			Console.WriteLine("\r\n>>Game results:");

			Console.ResetColor();

			foreach (var player in playersArr)
			{
				Console.WriteLine($"'{player.PlayerName}' won '{player.NumberOfWins}' time(s)");
			}

			Console.ReadKey();
		}

		static void Game21(ref Player[] playersArr)
		{
			Card[] deck = new Card[36];

			FillInDeck(deck);

			bool continuePlaying = true;

			int numberOfCard = 0;

			int round = 1;

			while (continuePlaying)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;

				Console.WriteLine($"\r\n---------Round {round}---------");

				Console.ResetColor();

				round++;

				for (int i = 0; i < playersArr.Length; i++)
				{
					if (i == 0)
						numberOfCard = playersArr[i].PlayerDeck.Length == 0 ? 2 : 1;
					
					AddCardsToPlayers(ref playersArr[i].PlayerDeck, GetCard(ref deck, numberOfCard));

					playersArr[i].PlayerScore = CalculateValue(playersArr[i].PlayerDeck);

					playersArr[i].HasTwoAces = HasDoubleAces(playersArr[i].PlayerDeck);

					Console.Write($"\r\n>> For '{playersArr[i].PlayerName}' the cards are: {PrintCard(playersArr[i].PlayerDeck)}");

					Console.Write($"\t Score: {(playersArr[i].HasTwoAces ? "Has TWO aces" : playersArr[i].PlayerScore.ToString())}\r\n");

					playersArr[i].FinishGame = playersArr[i].PlayerScore >= 21 || playersArr[i].HasTwoAces;
				}

				continuePlaying = (playersArr[0].FinishGame || playersArr[1].FinishGame) ? false : ProceedTheGame(playersArr[0].PlayerName);
			}

			ChooseTheWinner(ref playersArr, round-1);
		}
	}
}