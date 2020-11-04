# Text Classification C#

Simple implementation of text classification in C#.
This clasify text in diferent topics based on key words.
this use 'tf-idf (term frequency(tf) - inverse dense frequency(idf))' for asing scores and 
KMP algorithm for string matching.

## Example

```C#
TextClassificator tc = new TextClassificator();

tc.AddVocabulary("Colombia", keyWordsListC);
tc.AddVocabulary("Venezuela", keyWordsListV);

List<KeyValuePair<string, double>> score1 =  tc.ComputeAll(text1);
Console.WriteLine("Topic of text1 is " + score1[0].Key + " with score " + score1[0].Value);

List<KeyValuePair<string, double>> score2 = tc.ComputeAll(text2);
Console.WriteLine("Topic of text2 is " + score2[0].Key + " with score " + score2[0].Value);
```
