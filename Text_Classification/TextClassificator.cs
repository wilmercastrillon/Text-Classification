using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classificator {
    class TextClassificator {

        private List<List<string>> vocabulary = new List<List<string>>();
        private List<string> topics = new List<string>();
        private List<string> exceptions = new List<string>();

        /// <summary>
        /// Palabras a ser excluidas |Columnas en DataTable -> [word]
        /// </summary>
        /// <param name="dt"></param>
        public void AddExceptions(DataTable dt) {
            List<string> list = (from DataRow dr in dt.Rows
                                 select dr["word"].ToString()).ToList();
            exceptions.AddRange(list);
            exceptions.Sort();
        }

        /// <summary>
        /// Palabras a ser excluidas
        /// </summary>
        /// <param name="dt"></param>
        public void AddExceptions(List<string> words) {
            exceptions.AddRange(words);
            exceptions.Sort();
        }

        /// <summary>
        /// Cargar palabras clave de un tema |Columnas -> [word]
        /// </summary>
        /// <param name="dt"></param>
        public void AddVocabulary(string topicName, DataTable dt) {
            List<string> list = (from DataRow dr in dt.Rows
                                 select  prepareText(dr["word"].ToString())).ToList();
            list.Sort();
            vocabulary.Add(list);
            topics.Add(topicName);
        }

        /// <summary>
        /// Cargar palabras clave de un tema
        /// </summary>
        /// <param name="dt"></param>
        public void AddVocabulary(string topicName, List<string> list) {
            for(int i = 0; i < list.Count; i++) {
                list[i] = prepareText(list[i]);
            }
            list.Sort();
            vocabulary.Add(list);
            topics.Add(topicName);
        }

        /// <summary>
        /// Preparar texto para la clasificacion
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public string prepareText(string text) {
            string[] str = text.Split(' ');
            string aux2;

            StringBuilder sb = new StringBuilder("");
            foreach(string aux in str) {
                aux2 = prepareWord(aux);
                if(aux2.Length == 0 || exceptions.BinarySearch(aux2) >= 0) {
                    continue;
                }
                sb.Append(aux2).Append(" ");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        /// <summary>
        /// Remplazar caracteres especiales y quitar no alfanumericos
        /// </summary>
        /// <param name="palabra"></param>
        /// <returns></returns>
        public static string prepareWord(string palabra) {
            string str = palabra.ToLower(CultureInfo.CurrentCulture);
            char[] special = { 'á', 'é', 'í', 'ó', 'ú', 'ü' };
            char[] equivalent = { 'a', 'e', 'i', 'o', 'u', 'u' };
            char add;
            StringBuilder sb = new StringBuilder("");

            for(int i = 0; i < str.Length; i++) {
                if(Char.IsLetterOrDigit(str[i])) {
                    add = str[i];
                    for(int j = 0; j < special.Length; j++) {
                        if(special[j] == str[i]) {
                            add = equivalent[j];
                            break;
                        }
                    }
                    sb.Append(add);
                } else {
                    sb.Append(' ');
                }
            }
            return sb.ToString().Trim();
        }

        private double score(string word, int count) {
            int temas = 0;
            foreach(List<string> ls in vocabulary) {
                if(ls.BinarySearch(word) >= 0)
                    temas++;
            }
            double res = (1.0 * count * Math.Log10(1.0 * vocabulary.Count / temas));
            //Console.Write(word + " : " + res + ", ");
            return res;
        }

        /// <summary>
        /// Realiza pruebas de un texto en cada tema
        /// </summary>
        /// <param name="tema"></param>
        /// <param name="texto"></param>
        /// <returns></returns>
        public List<KeyValuePair<string, double>> ComputeAll(string text) {
            List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>();
            text = prepareText(text);

            for(int i = 0; i < topics.Count; i++) {
                list.Add(new KeyValuePair<string, double>(topics[i], Compute(i, text, true)));
            }
            list.Sort((x, y) => x.Value.CompareTo(y.Value));
            list.Reverse();
            return list;
        }

        private void AlgoritmoDeBordes(int[] bordes, string subStr) {
            int i = 0, j = -1;
            bordes[0] = -1;

            while(i < subStr.Length) {
                while(j >= 0 && subStr[i] != subStr[j])
                    j = bordes[j];
                i++;
                j++;
                bordes[i] = j;
            }
        }

        private int KMP(int[] bordes, string str, string subStr) {
            int i = 0, j = 0, lastIdx = -1, count = 0;
            str = str + " ";
            while(i < str.Length - 1) {
                while(j >= 0 && str[i] != subStr[j])
                    j = bordes[j];
                i++;
                j++;
                if(j == subStr.Length) {
                    if(i - j > lastIdx && str[i] == ' ') {
                        lastIdx = i - j;
                        count++;
                    }
                    j = bordes[j];
                }
            }
            return count;
        }

        /// <summary>
        /// Cuenta cuantas veces aparece una palabra en un texto
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="palabra"></param>
        /// <returns></returns>
        public int countWord(string text, string word, bool textIsPrepared = false) {
            if(!textIsPrepared)
                text = prepareText(text);
            int[] bordes = new int[text.Length + 10];
            AlgoritmoDeBordes(bordes, word);
            return KMP(bordes, text, word);
        }

        /// <summary>
        /// Realiza prueba de pertenecia de un texto en un tema
        /// </summary>
        /// <param name="tema"></param>
        /// <param name="texto"></param>
        /// <returns></returns>
        public double Compute(int topicIndex, string text, bool textIsPrepared = false) {
            Dictionary<string, int> ocurrencia = new Dictionary<string, int>();
            double acum = 0.0;
            int cont;
            if(!textIsPrepared)
                text = prepareText(text);

            for(int j = 0; j < vocabulary[topicIndex].Count; j++) {
                cont = countWord(text, vocabulary[topicIndex][j], true);
                acum += score(vocabulary[topicIndex][j], cont);
                //Console.WriteLine("palabra " + vocabulary[topicIndex][j] + " aparece " + cont +
                //    ", puntaje: " + score(vocabulary[topicIndex][j], cont));
            }
            //Console.WriteLine("puntaje final: " + acum);
            return acum;
        }

        public double Compute(string topic, string text) {
            int idx = topics.IndexOf(topic);
            if(idx < 0)
                throw new Exception("Topic no exists.");
            return Compute(idx, text);
        }
    }
}
