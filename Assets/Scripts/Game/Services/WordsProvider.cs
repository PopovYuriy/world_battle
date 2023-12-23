using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Data.Trie;
using Newtonsoft.Json;
using UnityEngine;
using Utils.Extensions;

namespace Game.Services
{
    public sealed class WordsProvider
    {
        const string OutFilePath = "Words/";

        private Dictionary<char, Trie> _charToTrieMap;
        
        public void Initialize(char[] letters)
        {
            var uniqueLetters = letters.Distinct().Except(new []{'ь'}).ToArray();
            _charToTrieMap = new Dictionary<char, Trie>(uniqueLetters.Length);
            foreach (var letter in uniqueLetters)
            {
                try
                {
                    var jsonFilePath = $"{OutFilePath}{letter}";
                    var jsonAsset = (TextAsset) Resources.Load(jsonFilePath);
                    var trie = JsonConvert.DeserializeObject<Trie>(jsonAsset.text);
                    _charToTrieMap.Add(letter, trie);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }
            }
        }

        public bool IsValidWord(string word)
        {
            if (word.IsNullOrEmpty())
            {
                Debug.LogWarning("Passed word cannot be null or empty");
                return false;
            }
            
            var letter = word[0];
            if (_charToTrieMap.TryGetValue(letter, out var trie)) 
                return trie.HasWord(word);
            
            Debug.LogWarning($"Cannot find trie with first letter '{letter}'");
            return false;

        }

        
        public IEnumerable<string> GetAvailableWords(List<char> chars, List<string> except)
        {
            var result = _charToTrieMap.Values.SelectMany(t => t.SearchWords(chars)).ToList();
            return result.Except(except);
        }
        
        
        private void PermuteAllLengths(char[] array)
        {
            for (var length = 2; length <= array.Length; length++)
            {
                Permute(array, length);
            }
        }

        private void Permute(char[] array, int length)
        {
            var combination = new char[length];
            PermuteHelper(array, combination, 0, length);
        }

        private void PermuteHelper(IReadOnlyList<char> array, char[] combination, int startIndex, int length)
        {
            if (startIndex == length)
            {
                var word = new string(combination);
                Debug.Log($"Word = {word}, valid = {IsValidWord(word)}");
                return;
            }

            foreach (var c in array)
            {
                combination[startIndex] = c;
                PermuteHelper(array, combination, startIndex + 1, length);
            }
        }
    }
}