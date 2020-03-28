using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classificator;

namespace Text_Classification {
    class Program {

        static List<string> keyWordsC = new List<string> { "Colombia", "Republic of Colombia", "country",
                                        "South America", "America", "Caribbean Sea", "Bogotá", "departments",
                                        "Andes", "coast", "pacific", "Atlantic", "Amazon", "Amazon rainforest"};
        static List<string> keyWordsV = new List<string> { "Venezuela", "Bolivarian Republic of Venezuela",
                                        "country", "South America", "America", "Caribbean Sea", "Caracas",
                                        "Atlantic", "biodiversity", "Andes", "Orinoco River", "Amazon",
                                        "llanos plains", "Orinoco"};

        //From wikipedia
        static string text1 = @"Colombia, officially the Republic of Colombia, is a country largely situated in 
                        the north of South America, with land and territories in North America. Colombia is bounded 
                        on the north by the Caribbean Sea, the northwest by Panama, the south by both Ecuador and 
                        Peru, the east by Venezuela, the southeast by Brazil, and the west by the Pacific. It 
                        Urban centres are concentrated in the Andean highlands and the Caribbean coast.
                        Colombia has the second-highest biodiversity in the world and is one of the world's 17 
                        megadiverse countries; its territory encompasses Amazon rainforest, highlands, 
                        grasslands, deserts, and islands and coastlines along both the Atlantic and Pacific 
                        (the only country in South America).";

        static string text2 = @"Venezuela, officially the Bolivarian Republic of Venezuela, is a country on the 
                        northern coast of South America, consisting of a continental landmass and many small 
                        islands and islets in the Caribbean Sea. It has a territorial extension of 916,445 km2. 
                        The continental territory is bordered on the north by the Caribbean Sea and the Atlantic 
                        Ocean, on the west by Colombia, Brazil on the south, Trinidad and Tobago to the north-east 
                        and on the east by Guyana. The capital and largest urban agglomeration is the city of 
                        Caracas.
                        The country has extremely high biodiversity and is ranked seventh in the world's list of 
                        nations with the most number of species. There are habitats ranging from the Andes 
                        Mountains in the west to the Amazon basin rain-forest in the south via extensive llanos 
                        plains, the Caribbean coast and the Orinoco River Delta in the east.";

        static void Main(string[] args) {
            TextClassificator tc = new TextClassificator();

            tc.AddVocabulary("Colombia", keyWordsC);
            tc.AddVocabulary("Venezuela", keyWordsV);

            List<KeyValuePair<string, double>> score1 =  tc.ComputeAll(text1);
            Console.WriteLine("Topic of text1 is " + score1[0].Key + " with score " + score1[0].Value);

            List<KeyValuePair<string, double>> score2 = tc.ComputeAll(text2);
            Console.WriteLine("Topic of text2 is " + score2[0].Key + " with score " + score2[0].Value);

            Console.Read();
        }
    }
}
