/*
 *program.cs
 *a client application that accesses the word game server, to play the game 
 *
 *Revision History 
 *      Cody Lefebvre, 2015.11.29: Created
 *      Cody Lefebvre, 2015.12.1: added exceptions
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CLAssignment3__Client
{
    class Program
    {
        static bool onValidationCallback(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        //main function of the console application
        static void Main(string[] args)
        {//variable declaration for the proxy, and bool.
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(onValidationCallback);
            //initiates the proxy
            WordGameReference.WordScrambleGameClient proxy = new WordGameReference.WordScrambleGameClient();
            //sets game to playable
            bool canPlayGAme = true;

            //takes input and assignes to playername
            Console.WriteLine("What is your name?..");
            string playerName = Console.ReadLine();

            try
            {  
                if (!proxy.isGameBeingHosted())
                {
                    //asks if you would like to host the game,yes/no
                    Console.WriteLine("Welcome " + playerName + "! do you want to host the game?");
                    if (Console.ReadLine().CompareTo("yes") == 0)
                    {
                        //if yes set the word the be guessed by other players
                        //disable host from being able to play game
                        Console.WriteLine("type the word to scramble.");
                        string inputWord = Console.ReadLine();
                        string scrambledWord = proxy.hostGame(playerName, "", inputWord);
                        canPlayGAme = false;
                        Console.WriteLine("your hosting the game with word '" + inputWord + "'scrambled as: " + scrambledWord);
                        Console.ReadKey();
                    }
                    try
                    {
                        //if not hosting
                        if (Console.ReadLine().CompareTo("no") == 0)
                        {
                            //generate an error for the game not being hosted yet and throw
                            WordGameReference.GameBeingHostedException fault = new WordGameReference.GameBeingHostedException();
                            throw new FaultException<WordGameReference.GameBeingHostedException>(fault, new FaultReason("Game not hosted "));
                        }

                        if (proxy.isGameBeingHosted() == true)
                        {
                            //generate an error for the game already being hosted and throw
                            WordGameReference.GameAlreadyHostedException fault = new WordGameReference.GameAlreadyHostedException();

                            throw new FaultException<WordGameReference.GameAlreadyHostedException>(fault, new FaultReason("game is already hosted "));
                        }
                    }
                    catch (FaultException<WordGameReference.GameAlreadyHostedException>e)
                    {
                        //catch exception
                        Console.WriteLine("Error due to " + e.Reason);
                    }
                }
            }
            catch (FaultException<WordGameReference.GameBeingHostedException> e)
            {
                //catch exception
                canPlayGAme = false;
                Console.WriteLine("Error Due to " + e.Reason);
            }
            catch(FaultException<WordGameReference.MaximumNumberOfPlayersException>e)
            {
                //catch exception
                Console.WriteLine("Error Due to " + e.Reason);
            }
            catch(FaultException<WordGameReference.HostCantJoinException>e)
            {
                //catch exception
                Console.WriteLine("Error Due to" + e.Reason);
            }

            if (canPlayGAme)
            {
                //if elegible ask to play game
                Console.WriteLine("do you want to play this game?");

                //yes id like to play
                    if (Console.ReadLine().CompareTo("yes") == 0)
                    {
                        //player joins game and shows scrambled word
                            WordGameReference.Word gameWords = proxy.join(playerName);
                            Console.WriteLine("can you unscramble this word? => " + gameWords.scrambledWord);
                            string guessedWord;
                            bool gameOver = false;

                            try
                            {
                                //while the game isnt over
                                while (!gameOver)
                                {
                                    //accept guesses from all users
                                    guessedWord = Console.ReadLine();
                                    gameOver = proxy.guessWord(playerName, guessedWord, gameWords.unscrambledWord);

                                    if (!gameOver)
                                    {
                                        //if wrong guess relay message
                                        Console.WriteLine("Nope, Try Again..");
                                    }
                                }
                            }
                            catch (FaultException<WordGameReference.PlayerNotPLayingTheGameException> e)
                                //generate error for player not in the game
                            {
                                Console.WriteLine("Error Due to " + e.Reason);
                            }

                            Console.WriteLine("YOU WON!");
                    }
            }

        }
    }
}
