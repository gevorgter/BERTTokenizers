using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace BERTTokenizers;

public class Tokenizer
{
    protected Dictionary<string, int> _vocabularyDict;
    int _unkToken = 0;
    int _separationToken = 0;
    int _classificationToken = 0;

    public void Init()
    {
        using var st = GetType().Assembly.GetManifestResourceStream("BERTTokenizers.vocab.txt");
        Init(st, false);
    }

    public void Init(string vocabularyFilePath, bool caseSensitive)
    {
        using var st = File.OpenRead(vocabularyFilePath);
        Init(st, caseSensitive);
    }

    public void Init(Stream st, bool caseSensitive)
    {
        _vocabularyDict = caseSensitive ? new Dictionary<string, int>(StringComparer.Ordinal) : new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        int lineIndex = 0;
        using var reader = new StreamReader(st);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();
            if (line.Length > 0)
                _vocabularyDict[line] = lineIndex++;
        }

        _unkToken = _vocabularyDict[Tokens.Unknown];
        _separationToken = _vocabularyDict[Tokens.Separation];
        _classificationToken = _vocabularyDict[Tokens.Classification];
    }
    /*
    public List<(string Word, long InputId, long AttentionMask)> Encode(string text, int sequenceLength, int maxSequenceLength)
    {
        var lst = new List<(string Word, long InputId, long AttentionMask)>();
        foreach (var (token, index) in Tokenize(text, maxSequenceLength))
            lst.Add((token, index, 1));
        if (lst.Count < sequenceLength)
        {
            for (int i = lst.Count; i < sequenceLength; i++)
                lst.Add((Tokens.Padding, 0, 0));
        }
        return lst;
    }

    public IEnumerable<(string Token, int VocabularyIndex)> Tokenize(string text, int maxLength)
    {
        int count = 2; //count 2 tokens CLS and SEP
        yield return (Tokens.Classification, _classificationToken);
        foreach (var word in BreakUpSentence(text))
        {
            foreach (var t in TokenizeWord(word))
            {
                yield return t;
                count++;
                if (count == maxLength)
                    break;
            }
            if (count == maxLength)
                break;
        }
        yield return (Tokens.Separation, _separationToken);
    }

    */

    private IEnumerable<(string Token, int VocabularyIndex)> TokenizeWord(string word)
    {
        if (_vocabularyDict.TryGetValue(word, out int vocabularyIndex))
            yield return (word, vocabularyIndex);
        else
        {
            var remaining = word;
            while (remaining.Length > 0)
            {
                string prefix = null;
                int subwordLength = remaining.Length;
                while (subwordLength >= 1)
                {
                    string subword = remaining[..subwordLength];
                    if (!_vocabularyDict.TryGetValue(subword, out vocabularyIndex))
                    {
                        subwordLength--;
                        continue;
                    }
                    prefix = subword;
                    break;
                }

                if (prefix == null)
                {
                    yield return (Tokens.Unknown, _unkToken);
                    break;
                }

                if (prefix.Length != remaining.Length)
                    remaining = "##" + remaining[prefix.Length..];
                else
                    remaining = "";
                yield return (prefix, vocabularyIndex);
            }
        }

    }
    /*
    public static IEnumerable<string> BreakUpSentence(string text)
    {
        var i = 0;
        StringBuilder bld = new(30);
        while (i < text.Length)
        {
            char c = text[i++];
            if (char.IsLetterOrDigit(c))
                bld.Append(c);
            else
            {
                if (bld.Length == 0)
                    continue;
                yield return bld.ToString();
                bld.Clear();

                if (c != '\r' && c != '\t' && c != ' ')
                    yield return c.ToString();
            }
        }
        if (bld.Length != 0)
            yield return bld.ToString();
    }
    */
    public IEnumerable<(string token, long inputId, long attentionMask)> Process(IEnumerable<string> words, int sequenceLength, int maxLength)
    {
        yield return (Tokens.Classification, _classificationToken, 1);

        int tokenCounter = 2; //accomodate first token and last token
        foreach (var w in words)
        {
            foreach (var (token, index) in TokenizeWord(w))
            {
                yield return (token, index, 1);
                tokenCounter++;
                if (tokenCounter == sequenceLength)
                    break;
            }
            if (tokenCounter == sequenceLength)
                break;
        }
        while (tokenCounter < maxLength)
        {
            yield return (Tokens.Padding, 0, 0);
            tokenCounter++;
        }
        yield return (Tokens.Separation, _separationToken, 1);
    }

}
