using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordEater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
          

        }

        public List<Word> Words = new List<Word>();

        public void ReadLineIn(string line)
    {
        //remove commas  --maybe don't want to do this?
        line.Replace(",", "");
        line.Replace("\"", "");
         string[] wordNames = line.Split((" ".ToCharArray()[0]));

         int i=0;
         foreach (string word in wordNames)
         {
             
             Word word1 = new Word(word);
             if (i == wordNames.Count() - 1)
             {
                 word1.Position = WordPosition.Last;
                 word1.WordAfter = ".";
                 Words.Add(word1);
                 break;
             }
             else 
              
             { word1.WordAfter = wordNames[i + 1]; }

             if (i == 0)
             { word1.Position = WordPosition.First; }
             else
             { word1.Position = WordPosition.Middle; }
             i++;
             Words.Add(word1);
         }
        
    }

    

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = brLines(textBox1.Text);

            foreach(string l in lines) ReadLineIn(l);
            txtResponse.Text = GenerateResponse();
            textBox1.Text = "";
        }

        public string[] brLines( string source)
        {
            return source.Split(new string[] { "\r\n", "\n", "." }, StringSplitOptions.None);
        }

        public string GenerateResponse()
    {
        string Response = "";
        string nextword = GetRandomFirstWord();
        Response += nextword + " ";
        while (nextword != ".")
        {
            nextword = GetRandomNextWord(nextword);
            Response += nextword + " ";

        }
        foreach (Word w in Words) w.HasBeenUsed = false;
        return Response;    
    }

        public string GetRandomFirstWord()
        {
            string outword = "";
            Random rnd = new Random();
            while (outword == "")
            {
                int r = (int)rnd.Next(0, Words.Count - 1);
                if (Words[r].Position == WordPosition.First)
                {
                    outword = Words[r].Name;
                }
            }
            return outword;
        }


        public string GetRandomNextWord(string inword)
        {
            string outword = "";
           Random rnd = new Random(); 
            while (outword == "") 
            {
                int r = (int)rnd.Next(0, Words.Count);
                if (Words[r].Name == inword)
                if(!Words[r].HasBeenUsed)
                {
                    outword = Words[r].WordAfter;
                    if(outword == "") outword=".";
                    Words[r].HasBeenUsed = true;
                }
            }
            return outword;
        }
       


    }
}
