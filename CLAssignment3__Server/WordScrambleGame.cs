/*
 *WordScrambleGAme.cs
 *a server application that hosts the wordScramble game 
 *
 *Revision History 
 *      Cody Lefebvre, 2015.11.29: Created
 *      Cody Lefebvre, 2015.12.1: Completed empty functions
 *      Cody Lefebvre, 2015.12.2: modified functions and added exceptions
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CLAssignment3__Server;
using System.Collections;

namespace CLAssignment3__Server
{
    public class WordScrambleGame : IWordScrambleGame
    {
        //properties of the word game declared and set
        private const int MAX_PLAYERS = 2;
        private static string userHostingTheGame = null;
        public static Word gameWords = new Word();
        private static List<string> activePlayers = new List<string>(5);
      
        /// <summary>
        /// determines if game is hosted or not
        /// </summary>
        /// <returns>is game hosted</returns>
        [OperationBehavior]
        public bool isGameBeingHosted()
        {
                if(userHostingTheGame == null)
                {
                    return false;
                }

            return true;
        }
       
        /// <summary>
        /// hosts the game and scrambled word from playername and host address
        /// </summary>
        /// <param name="playerName">players name</param>
        /// <param name="hostAddress">address of host</param>
        /// <param name="wordToScramble">word before its scrambled</param>
        /// <returns></returns>
        [OperationBehavior]
        public string hostGame(string playerName,string hostAddress,string wordToScramble)
        {
            if (userHostingTheGame == null && activePlayers.Count > 0)
            {
                //throws exceptoion to client 
                GameBeingHostedException fault = new GameBeingHostedException();
                throw new FaultException<GameBeingHostedException>(fault, new FaultReason("Game not hosted"));
            }
            //continue to host
            userHostingTheGame = playerName;
            gameWords.unscrambledWord = wordToScramble;
            gameWords.scrambledWord = scrambledWord(wordToScramble);
            return gameWords.scrambledWord;
            
        }
        
        /// <summary>
        /// allows player to join game
        /// </summary>
        /// <param name="playerName">name of player</param>
        /// <returns>game word</returns>
        [OperationBehavior]
        public Word join(string playerName)
        {
                if (activePlayers.Count >= MAX_PLAYERS)
                {
                    //throws exception to client
                    MaximumNumberOfPlayersException fault = new MaximumNumberOfPlayersException();
                    throw new FaultException<MaximumNumberOfPlayersException>(fault, new FaultReason("Too many Players"));
                }

                if(activePlayers.Count < MAX_PLAYERS)
                {
                    //add player to active players
                    activePlayers.Add(playerName);
                }

                if(playerName == userHostingTheGame)
                {
                    //throws exception to client
                    HostCantJoinException fault = new HostCantJoinException();
                    throw new FaultException<HostCantJoinException>(fault, new FaultReason("Host Cannot play"));
                }
                
                return gameWords;

        }
        
        /// <summary>
        /// allows a player to guess unscrambled word
        /// </summary>
        /// <param name="playerName">name of player</param>
        /// <param name="guessedWord"> word player guessed</param>
        /// <param name="unscrambledWord"> correct word</param>
        /// <returns> true or false</returns>
        [OperationBehavior]
        public bool guessWord(string playerName,string guessedWord,string unscrambledWord)
        {
            if(guessedWord == unscrambledWord)
            {
                return true;
            }
            if(!activePlayers.Contains(playerName))
            {
                //throws exception to client
                PlayerNotPLayingTheGameException fault = new PlayerNotPLayingTheGameException();
                throw new FaultException<PlayerNotPLayingTheGameException>(fault, new FaultReason("Player must join the game"));
            }
            return false;
        }

        /// <summary>
        /// scrambles word set by host
        /// </summary>
        /// <param name="word"> word to be scrambled</param>
        /// <returns></returns>
        private string scrambledWord(string word)
        {
            char[] chars = word.ToArray();

            Random r = new Random(2011);

            for (int i = 0; i < chars.Length; i++)
            {

                int randomIndex = r.Next(0, chars.Length);

                char temp = chars[randomIndex];

                chars[randomIndex] = chars[i];

                chars[i] = temp;

            }

            return new string(chars);
        }
    }
}
