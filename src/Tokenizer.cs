using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BERTTokenizers;

public class Tokenizer
{
    protected Dictionary<string, int> _vocabularyDict;
    int _unkToken = 0;
    int _separationToken = 0;
    int _classificationToken = 0;

    public void Init(string vocabularyFilePath, bool caseSensitive)
    {
        _vocabularyDict = caseSensitive ? new Dictionary<string, int>(StringComparer.Ordinal) : new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        int lineIndex = 0;
        using var reader = new StreamReader(vocabularyFilePath);
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

    protected static IEnumerable<string> BreakUpSentence(string text)
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
}
