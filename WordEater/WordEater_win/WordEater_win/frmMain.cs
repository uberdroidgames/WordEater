using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
namespace WordEater_win
{
    public enum WordPosition { First, Middle, Last };
    
    public partial class frmMain : Form
    {
        public bool Use3rdWord = false;
        public List<RhymeCodeClass> RhymeCodes = new List<RhymeCodeClass>();
        public Word LastWord = new Word();
        public Word LastWord2 = new Word();
        public string nl = System.Environment.NewLine;
        public frmMain()
        {
            InitializeComponent();
            getListOfRhymes();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
       
            string[] lines = brLines(this.txtInput.Text);
            progressBar1.Value = 0;
            progressBar1.Maximum = lines.Count();
            int i=0;
            foreach (string l in lines)
            {
                ReadLineIn(l);
                session.lines.Add(l);
                progressBar1.Value = i;
                i++;
            }
       
            txtInput.Text = "";
        }


        public List<Word> Words = new List<Word>();
        public Session session = new Session();


        public void getListOfRhymes()
        {
            StreamReader s = new StreamReader("dict2.txt");
            string S; RhymeCodeClass Rc; 
            while ((S=s.ReadLine()) != null)
            {
                string[] a = S.Split(" ".ToCharArray());
                Rc = new RhymeCodeClass();
                Rc.name = a[0];
                foreach (string sl in a)
                    if (sl != "")
                        if (IsNumber(sl[sl.Length - 1].ToString())) Rc.numSyllables++;
                if (a.Count() > 1) Rc.RhymeCode = a[a.Count() - 2] + a[a.Count() - 1];
                RhymeCodes.Add(Rc);
            }
        }

        public void ReadLineIn(string line)
        {
            //remove commas  --maybe don't want to do this?
            line = line.Replace(",", "");
            line= line.Replace("\"", "");
            line = line.Replace(")", "");
            line = line.Replace("(", "");
            string[] wordNames = line.Trim().Split((" ".ToCharArray()[0]));

            int i = 0;
            foreach (string word in wordNames)
            {

                Word word1 = new Word() { Name = word };
                
                if (i == wordNames.Count() - 1)
                {
                    word1.Position = WordPosition.Last;
                    word1.WordAfter = ".";
                    GetRhymeCodes(word1);
                    Words.Add(word1);
                    break;
                }
                else

                { 
                    word1.WordAfter = wordNames[i + 1];
                    if (i < wordNames.Count() - 2)
                    
                        {word1.WordAfter2 = wordNames[i + 2];}
                        else
                        {word1.WordAfter2 = ".";}                 
                }


                if (i == 0)
                { word1.Position = WordPosition.First; }
                else
                { word1.Position = WordPosition.Middle; }
                i++;
    
                  
                Words.Add(word1);
            }

           

        }

        public void GetRhymeCodes(Word wd)
        {
            RhymeCodeClass GotWord= new RhymeCodeClass();
            if (wd.Name == "") return;
            string uppername = wd.Name.ToUpper();
            string st = "";
                      IEnumerable<RhymeCodeClass> rcs = from s in RhymeCodes
                                      where s.name==uppername
                                      select s;


                      if (rcs.Count() > 0)
                      {
                          GotWord = rcs.First();
                          st = GotWord.name;
                          // we have our word
                          string[] a = st.Split(" ".ToCharArray());

                          // if(rcs.Count()!=0)
                          {
                              //get syllables
                              wd.NumSyllables = GotWord.numSyllables;
                              wd.RhymeCode = GotWord.RhymeCode;
                          }
                      }
            

 

        }

        public bool IsNumber(string v)
        {
            return Regex.IsMatch(v, "^\\d+$");
        }

        public string[] brLines(string source)
        {
            return source.Split(new string[] { "\r\n", "\n", "." ,"?","!" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string GenerateResponse()
        {
            int tries = 0;
            string Response = "";
            while (true)
            {
                Response = "";
                Word nextword = GetRandomFirstWord();
                
                Response += nextword.Name + " ";
                while (nextword.Name != ".")
                {
                    if (nextword.Position == WordPosition.Last) LastWord = nextword;
                    nextword = GetRandomNextWord(nextword,nextword.WordAfter2);
                    Response += nextword.Name + " ";

                }
                foreach (Word w in Words) w.HasBeenUsed = false;

                //try really hard to get an original response that wasn't just 
                //repeating any lines that were put in.  
                if (CheckResponse(Response))
                {  break;}
                else
                {
                    tries++;
                    if (tries > 100) break;
                }
            }
            return Response;
        }

        public Word GetRandomFirstWord()
        {
            List<Word> Words2List = new List<Word>();
            foreach (Word w in Words) if (w.Position == WordPosition.First) Words2List.Add(w);
            if (Words2List.Count() == 0) return new Word { Name = ".", Position = WordPosition.Last };
         
            Random rnd = new Random();
           
                int r = (int)rnd.Next(0, Words2List.Count - 1);

                return Words2List[r];   

         
        }

        public bool CheckResponse(string r)
    {
        r=r.Replace(".", "");
        r=r.Trim();
        foreach(string l in session.lines)
        if(l==r)return false;

        return true;
    }

        public string GenerateResponseThatRhymes()
        {
            String Response = "";
            Boolean Rhymes = false;
            Word tempWord = LastWord;
            int Tries = 0;
            while (!Rhymes)
            {
                
                Response = GenerateResponse();
                Rhymes = RhymesWith(tempWord, LastWord);
                Tries++;
                if (Tries > 5000) break;
            }
            


            return Response;

        }

        public Boolean RhymesWith(Word word1, Word word2)
        {
            if (word1.RhymeCode == word2.RhymeCode && word1.Name != word2.Name) return true;
            return false;
        }

        public Word GetRandomNextWord(Word inword,string WordAfter)
        {
            string outword = "";
            Random rnd = new Random();


            List<Word> Words2List = new List<Word>();

            //this is the make a list first method.
            foreach (Word w in Words)
            {
                if (Use3rdWord)
                {
                    if (w.Name == inword.WordAfter & !w.HasBeenUsed & w.Position != WordPosition.First)
                        if (w.WordAfter == WordAfter || WordAfter == ".")
                            Words2List.Add(w);
                }
                else
                {
                    if (w.Name == inword.WordAfter & !w.HasBeenUsed & w.Position != WordPosition.First)
                   
                            Words2List.Add(w);
                }
            }

            if (Words2List.Count() == 0)
            {
                
                return new Word { Name = ".", Position = WordPosition.Last };
            }
                int r = (int)rnd.Next(0, Words2List.Count());

            outword = Words2List[r].WordAfter;
            Words[Words.IndexOf(Words2List[r])].HasBeenUsed = true;

            //int tries = 0;
            //Boolean DontWorryAboutPositionFirst = false;
            //while (outword == "")
            //{
            //    int r = (int)rnd.Next(0, Words.Count);
            //    if (Words[r].Name == inword)
            //        if (!Words[r].HasBeenUsed & 
            //            (Words[r].Position != WordPosition.First || DontWorryAboutPositionFirst))
            //        {
            //            outword = Words[r].WordAfter;
            //            if (outword == "") outword = ".";
            //            Words[r].HasBeenUsed = true;
            //        }
            //    tries++;
            //    if (tries > Words.Count()/2) DontWorryAboutPositionFirst = true;
            //}
            return Words2List[r];
        }

       

        private void saveWordListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            
            XmlSerializer x = new XmlSerializer(session.GetType());
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "*.wef|*.wef";
            fd.ShowDialog(this);
            string filename = fd.FileName;
            FileStream fs = new FileStream(filename, FileMode.CreateNew);
            x.Serialize(fs, session);
            fs.Close();
        }

        private void loadWordListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlSerializer x = new XmlSerializer(session.GetType());
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "*.wef|*.wef";
            fd.ShowDialog(this);
            string filename = fd.FileName;
            if (!System.IO.File.Exists(filename)) return;
            FileStream fs = new FileStream(filename, FileMode.Open);
            session = (Session)x.Deserialize(fs);
            fs.Close();
            Words.Clear();
            progressBar1.Value = 0;
            progressBar1.Maximum = session.lines.Count();
            int i = 0;
            foreach (string l in session.lines)
            {
                progressBar1.Value = i;
                ReadLineIn(l);
                i++;
            }
            progressBar1.Value = 0;
        }

        private void btnProduceRandom_Click(object sender, EventArgs e)
        {
            txtOutput.Text += GenerateResponse()+System.Environment.NewLine ;
            txtOutput.Text += GenerateResponseThatRhymes() + System.Environment.NewLine;
        }


        private void testmethod()
        {
            char[] c = "hello there what is up".ToCharArray();

            var c2 = from n in c
                    where n == 'h'
                    select n;
        }

        private void clearSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            session.lines.Clear();
            Words.Clear();
        }

        private void wordGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Use3rdWord = wordGroupsToolStripMenuItem.Checked;
        }


        private bool DoesItRhyme(string word1, string word2)
        {

            return false;
        }

        public class RhymeCodeClass
        {
            public string name;
            public int numSyllables;
            public string RhymeCode;
        }

    }
   
    public class Word
    {
        public string Name;
        public WordPosition Position;
        public string WordAfter;
        public string WordAfter2;
        public bool HasBeenUsed;
        public string RhymeCode;
        public int  NumSyllables;

        public Word()
        {
        }


    }
  
    public class Session
    {
        public List<string> lines;
        public Session()
        {
            lines = new List<string>();
        }
    }


}
