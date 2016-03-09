/*
 *WordScrambleGAme.cs
 *an interface for the wordScramble game
 *
 *Revision History 
 *      Cody Lefebvre, 2015.11.29: Created
 *      Cody Lefebvre, 2015.12.1: created exception classes
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CLAssignment3__Server
{
    [ServiceContract]
    public interface IWordScrambleGame
    {
        //returns true if game is being hosted,else false
        [FaultContract(typeof(GameAlreadyHostedException))]
        [OperationContract]
        bool isGameBeingHosted();

        //allows user to host game
        [FaultContract(typeof(GameBeingHostedException))]
        [OperationContract]
        string hostGame(String userName,string hostAddress, string wordToScramble);

        //allow user to join the game
        [FaultContract(typeof(HostCantJoinException))]
        [FaultContract(typeof(MaximumNumberOfPlayersException))]
        [OperationContract]
        Word join(string playerName);

        //allows user to guess a word
        [FaultContract(typeof(PlayerNotPLayingTheGameException))]
        [OperationContract]
        bool guessWord(string playerName, string guessedWord, string unscrambledWord);    
    }

    [DataContract]
    public class Word
    {
        //class for word
        [DataMember]
        public string scrambledWord;
        [DataMember]
        public string unscrambledWord;
    }

    [DataContract]
    public class MaximumNumberOfPlayersException
    {
        //classfor maxplayers exception
        [DataMember]
        public int playerAmmount;

        [DataMember]
        public string reason;
    }

    [DataContract]
    public class GameAlreadyHostedException
    {
        //class for alreadyhosted exception
        [DataMember]
        public bool isGameBeingHosted;

        [DataMember]
        public string reason;
    }

    [DataContract]
    public class HostCantJoinException
    {
        //class for host cant join exception
        [DataMember]
        public int playerAmmount;

        [DataMember]
        public string reason;
    }

    [DataContract]
    public class GameBeingHostedException
    {
        //class for gamebeinghosted exception
        [DataMember]
        public bool hosted;

        [DataMember]
        public string reason;
    }

    [DataContract]
    public class PlayerNotPLayingTheGameException
    {
        //class for playernotplaying exception
        [DataMember]
        public string player;

        [DataMember]
        public string reason;
    }
}
