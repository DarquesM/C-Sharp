using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace JeuCapitalesWPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string paysATrouver = null;
        string capitaleATrouver = null;     //Je place mes déclararions ici pour qu'elles soient globales
        int scoreJoueur;
        int nombreParties;
        Random rand = new Random();
        string nomFichier = "Capitales.txt";        //Fichier contenant la liste de pays/capitale
        int nbPays;
        List<Pays> listePays = new List<Pays>();
        HashSet<int> checkRnd = new HashSet<int>();
        int[] propositionRepNb = new int[4];

        public MainWindow()
        {
            InitializeComponent();
            listePays = ouvreFichier(nomFichier);
            nbPays = listePays.Count;               //Nombre total de pays dans la liste            
            int highScore = getHighScore();
            afficheHighScore(highScore);
        }

        class Pays
        {
            public string nomPays { get; set; }
            public string Capitale { get; set; }
        }

   public void checkReponse(string repUser, string capitaleATrouver)
    {       
       nombreParties = nombreParties + 1;
       bool correct = string.Equals(repUser, capitaleATrouver, StringComparison.OrdinalIgnoreCase);
       if (correct)   //Je vérifie la réponse en ignorant la casse
                {
                    bonneReponse();      
                }
                else
                {
                    //mauvaiseReponse();
                }       
       newQuestion(correct);        
    }

   public void bonneReponse()
   {
       scoreJoueur = scoreJoueur+1;
       isHighScore(scoreJoueur);
       afficheScore(scoreJoueur);       
   }
   void mauvaiseReponse()
   {
       afficheScore(scoreJoueur);     
   }
        public void Reponse1(object sender, RoutedEventArgs e)
    {
            string repUser = btn_prop1.Content.ToString();
            checkReponse(repUser, capitaleATrouver);
    }
        public void Reponse2(object sender, RoutedEventArgs e)
        {
            string repUser = btn_prop2.Content.ToString();
            checkReponse(repUser, capitaleATrouver);

        }
        public void Reponse3(object sender, RoutedEventArgs e)
        {
            string repUser = btn_prop3.Content.ToString();
            checkReponse(repUser, capitaleATrouver);

        }
        public void Reponse4(object sender, RoutedEventArgs e)
        {
            string repUser = btn_prop4.Content.ToString();
            checkReponse(repUser, capitaleATrouver);

        }
        static List<Pays> ouvreFichier(string nomFichier)
        {
            string line = null;
            List<string> liste = new List<string>();
            List<Pays> listePays = new List<Pays>();
            using (StreamReader reader = new StreamReader(nomFichier))
            {
                line = reader.ReadLine();
                while (line != null)
                {
                    liste.Add(line);
                    line = reader.ReadLine();       //Je stocke les informations dans une liste pour pouvoir la manipuler ensuite                  
                }
            }
            foreach (string ttt in liste)
            {
                string[] test = ttt.Split('\t');
                listePays.Add(new Pays() { nomPays = test[0], Capitale = test[1] });
            }
            return listePays;
        }

        
        void rndPays(Random rand, int nbPays, List<Pays> listePays, out string paysATrouver, out string capitaleATrouver)
        {
            paysATrouver = null;
            capitaleATrouver = null;
            int curRnd = rand.Next(nbPays);
            foreach (var pays in listePays)
            {
                if (listePays.IndexOf(pays) == curRnd)
                {
                    paysATrouver = pays.nomPays;
                    capitaleATrouver = pays.Capitale;
                }
            }
            labelQuestion.Content = paysATrouver;
        }

        string[] createListePropositions(HashSet<int> checkRnd, Random rand, int nbPays, List<Pays> listePays, int[] propositionRepNb, string capitaleATrouver)
        {
            checkRnd.Clear();
            for (int i = 0; i < 4; i++)
            {
                int currentRnd = rand.Next(nbPays);     //Je génère un nb aléatoire entre 0 et le nb de pays dans la liste
                while (checkRnd.Contains(currentRnd))   //Tant que le nb aléatoire currentRnd est dans la liste j'en tire un autre
                {
                    currentRnd = rand.Next(nbPays);
                }
                checkRnd.Add(currentRnd);               //Stockage de cette valeur dans le hash (plus rapide à parcourir qu'une liste car pas indexé)
                propositionRepNb[i] = currentRnd;       //Puis stockage dans ma liste qui contient les n° de pays tirés au sort.
            }
            string[] propositionRepStr = new string[4]; //Stockage des 4 propositions
            int currentRnd2 = rand.Next(4);
            foreach (Pays pays in listePays)
            {
                int indexer = listePays.IndexOf(pays);  //indexer contient l'index de chaque pays
                for (int j = 0; j < 4; j++)
                {
                    if (indexer == propositionRepNb[j]) //Si l'index correspond au numéro tiré aléatoirement, alors
                    {
                        propositionRepStr[j] = pays.Capitale;   //Je le stocke dans mon vecteur de réponses (plutôt faire une liste ?)
                    }
                }
            }
            bool test = propositionRepStr.Contains(capitaleATrouver); //Je teste si la bonne réponse est déjà proposée parmi les 4
            if (test == false)
            {                                                       //Si oui je ne fais rien
                propositionRepStr[currentRnd2] = capitaleATrouver;    //Si non, j'écrase une des réponses avec la bonne réponse pour être sur qu'elle n'y est qu'une seule fois
            }
            btn_prop1.Content = propositionRepStr[0];
            btn_prop2.Content = propositionRepStr[1];
            btn_prop3.Content = propositionRepStr[2];
            btn_prop4.Content = propositionRepStr[3];
            return propositionRepStr;
        }

        public void newGame(object sender, RoutedEventArgs e)
        {
            rndPays(rand, nbPays, listePays, out paysATrouver, out capitaleATrouver);
            createListePropositions(checkRnd, rand, nbPays, listePays, propositionRepNb, capitaleATrouver);
        }
        
        public void newQuestion(bool correct)
        {
            if (correct != true) //Si on s'est trompé, on remet les compteurs à zéro
            {
                scoreJoueur = 0;
                nombreParties = 0;
            }
            else
            {
                //isHighScore(scoreJoueur);
            }
            afficheScore(scoreJoueur);            
            rndPays(rand, nbPays, listePays, out paysATrouver, out capitaleATrouver);
            createListePropositions(checkRnd, rand, nbPays, listePays, propositionRepNb, capitaleATrouver);
        }
        public void afficheScore(int scoreJoueur)
        {
            LabelScore.Content = scoreJoueur.ToString();
        }
        public void afficheHighScore(int Highscore)
        {
            LabelHighScore.Content = Highscore.ToString();
        }

        public void ecritHighScore(int highScore)
        {
            string line = scoreJoueur.ToString();
            using (StreamWriter writer = new StreamWriter("Highscore.txt"))
            {
                writer.Write(line);
            }
        }

        public void isHighScore(int scoreJoueur)
        {            
            //En option pour plus tard : demander le nom du joueur en cas de highscore et le rajouter dans le fichier
            int HighScore = getHighScore();
            if (scoreJoueur > HighScore)
            {
                ecritHighScore(scoreJoueur);
                afficheHighScore(scoreJoueur);
            }
            else
            {

            }
        }

        public int getHighScore()
        {
            string line = null;
            int highScore;
            using (StreamReader reader = new StreamReader("Highscore.txt"))
            {
                line = reader.ReadLine();
                int.TryParse(line,out highScore);
            }
            return highScore;
        }

        public void terminate(object sender, RoutedEventArgs e)
        {            
            Environment.Exit(-1);
        }
        public void about(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(" World Capitals \n\n By Darksoft \n\n v0.3 - 2014");
        }
       
    }
}
