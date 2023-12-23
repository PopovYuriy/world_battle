using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Core.Data.Trie
{
    [Serializable]
    public sealed class Trie
    {
        [JsonProperty("Root")] private Node _root;

        public Trie()
        {
            _root = new Node();
        }

        public void Insert(string word)
        {
            var currentNode = _root;
            foreach (var c in word)
            {
                if (!currentNode.HasChild(c))
                    currentNode.AddChild(c, new Node());
                
                currentNode = currentNode.GetChild(c);
            }

            currentNode.IsEnd = true;
        }

        public bool HasWord(string word)
        {
            var currentNode = _root;
            foreach (var c in word)
            {
                if (!currentNode.HasChild(c))
                    return false;
                
                currentNode = currentNode.GetChild(c);
            }

            return currentNode.IsEnd;
        }

        public List<string> SearchWords(List<char> letters)
        {
            List<string> result = new List<string>();
            var node = _root;

            Search(letters, _root, result, "");

            return result;
        }

        private void Search(List<char> letters, Node node, List<string> result, string currentWord)
        {
            if (node.IsEnd)
                result.Add(currentWord);
            
            if (letters.Count == 0)
                return;

            foreach (var letter in letters)
            {
                if (!node.HasChild(letter)) 
                    continue;
                
                var childNode = node.GetChild(letter);
                var remainingLetters = new List<char>(letters);
                remainingLetters.Remove(letter);
                Search(remainingLetters, childNode, result, currentWord + letter);
            }
        }

        // public List<string> GetAllWords(char[] letters)
        // {
        //     var result = new List<string>();
        //     for (int length = 2; length <= letters.Length; length++)
        //     {
        //         FindWords(letters, length, result);
        //     }
        //     return result;
        // }
        
        // private void FindWords(char[] letters, int length, List<string> result)
        // {
        //     var usedLetters = new List<char>();
        //     for (var letterIndex = 0; letterIndex < letters.Length; letterIndex++)
        //     {
        //         if (usedLetters.Contains(letters[letterIndex]))
        //             continue;
        //         usedLetters.Add(letters[letterIndex]);
        //         
        //         var combination = new List<char>(length);
        //         PermuteHelper(letters, combination, _root, letterIndex, 0, length, result);
        //     }
        // }

        // private void PermuteHelper(char[] letters, List<char> combination, Node currentNode, int letterIndex, int currentIndex, int length, List<string> result)
        // {
        //     if ()
        //     if (combination.Count >= length && !currentNode.IsEnd)
        //         return;
        //
        //     for (var i = currentIndex; i < length; i++)
        //     {
        //         
        //     }
        //     
        //     if (!currentNode.HasChild(letters[currentIndex]))
        //     {
        //         
        //     }
        //     if (currentIndex == length)
        //     {
        //         var word = new string(combination);
        //         return;
        //     }
        //
        //     for (int i = 0; i < letters.Length; i++)
        //     {
        //         combination[currentIndex] = letters[i];
        //         PermuteHelper(letters, combination, currentIndex + 1, length);
        //     }
        // }
    }
}