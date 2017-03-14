using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordEater

    


{
    public enum WordPosition { First, Middle, Last };
    
    public class Word
    {
        public string Name;
        public WordPosition Position;
        public string WordAfter;
        public int NameFrequency;  //How Often does a word with this name occur?
        public int weight;  //How often does this identical word object occur?
        public bool HasBeenUsed;
private   string word;

public    Word(string word)
    {
        this.Name = word;
        this.HasBeenUsed = false;
    }
        
        
     }

       



    }

    //public class WordStatistics
    //{
    //    //public Word wordBefore;  //add to this list every word that comes before this word
    //    public Word wordAfter;    //add to this list every word that comes after this word
    //    //public int positionInLine; //add to this list every position this word has in a line
    //    public WordPosition wordPosition; //First Middle or Last? 
    //}

