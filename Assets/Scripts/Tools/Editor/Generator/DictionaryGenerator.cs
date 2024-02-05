using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Data.Trie;
using Game.Data;
using ModestTree;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Tools.Editor.Generator
{
    public sealed class DictionaryGenerator
    {
        const string FilePath = "Assets/Data/Editor/Words/";
        const string OutFilePath = "Assets/Resources/Words/";
        const string FileNameTables = "dict_corp_vis.txt";
        const string FilePathWithNouns = "Nouns/";
        const string FilePathWithNounsTables = "NounsTables/";

        const string NounTag = "noun";

        static readonly string[] _tagsToSkip = {"fname", "lname", "pname", "prop", "abbr", "geo", "alt", "pers"};

        [MenuItem("22/Tools/WordsGenerator", priority = 1)]
        public static void Generate()
        {
            try
            {
                EditorUtility.ClearProgressBar();

                var nounsDirectoryPath = $"{FilePath}{FilePathWithNouns}";
                var nounsTablesDirectoryPath = $"{FilePath}{FilePathWithNounsTables}";

                var nounsList = new List<string>();
                var nounsTablesList = new List<string>();
                using (var fileStream = new StreamReader(FilePath + FileNameTables))
                {
                    string line;
                    string[] parts;
                    string[] tags;

                    EditorUtility.DisplayCancelableProgressBar("Read and process words", string.Empty, 0);
                    while ((line = fileStream.ReadLine()) != null)
                    {
                        if (line.StartsWith(" "))
                            continue;

                        parts = line.Split(' ');

                        tags = parts[1].Split(':');

                        if (!tags.Contains(NounTag))
                            continue;

                        if (_tagsToSkip.Any(tag => tags.Contains(tag)))
                            continue;

                        var word = parts[0];
                        if (word.Contains('-') || word.Contains('.') || word.Any(char.IsUpper))
                        {
                            Debug.Log($"Skipped word :: {word}");
                            continue;
                        }

                        nounsList.Add(parts[0]);
                        nounsTablesList.Add(line);
                    }
                }

                Debug.Log($"Result words count :: {nounsList.Count}");

                Dictionary<char, List<string>> nounGroups = GroupByLetters(nounsList);
                Dictionary<char, List<string>> nounTableGroups = GroupByLetters(nounsTablesList);

                foreach (var letter in nounGroups.Keys)
                {
                    var nounFilePath = $"{nounsDirectoryPath}{letter}.txt";
                    var nounTablesFilePath = $"{nounsTablesDirectoryPath}{letter}.txt";

                    using (var writerNouns = File.CreateText(nounFilePath))
                    using (var writerNounsTables = File.CreateText(nounTablesFilePath))
                    {
                        for (int wordIndex = 0; wordIndex < nounGroups[letter].Count; wordIndex++)
                        {
                            writerNouns.WriteLine(nounGroups[letter][wordIndex]);
                            writerNounsTables.WriteLine(nounTableGroups[letter][wordIndex]);
                        }
                    }
                }

                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                EditorUtility.ClearProgressBar();
                throw;
            }
        }

        [MenuItem("22/Tools/GenerateJsonWordsData", priority = 2)]
        private static void GenerateJsonWordsData()
        {
            try
            {
                if (!Directory.Exists(OutFilePath))
                    Directory.CreateDirectory(OutFilePath);
                
                var files = Directory.GetFiles($"{FilePath}{FilePathWithNouns}")
                    .Where(f => !f.Contains(".meta") && !f.Contains(".DS_Store"))
                    .ToArray();
                var counter = 1;
                foreach (var file in files)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Generate JSON data", string.Empty, 
                        (float)counter / files.Length))
                        break;
                    
                    var trie = new Trie();
                    var letter = Path.GetFileNameWithoutExtension(file);
                    var jsonFilePath = $"{OutFilePath}{letter}.json";
                    
                    using (var writerJson = File.CreateText(jsonFilePath))
                    using (var fileStream = new StreamReader(file))
                    {
                        string word;
                        while ((word = fileStream.ReadLine()) != null)
                            trie.Insert(word);
                    
                        writerJson.Write(JsonConvert.SerializeObject(trie));
                    }

                    counter++;
                }
                
                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                Console.WriteLine(e);
                throw;
            }
        }

        [MenuItem("22/Tools/DetermineLettersRate", priority = 3)]
        private static void DetermineLettersRate()
        {
            EditorUtility.ClearProgressBar();
            
            var lettersConfig = (LettersRateConfig)Resources.Load("LettersRateConfig");

            var lettersGlobalRate = new Dictionary<char, uint>(lettersConfig.Letters.Length);
            var lettersWordsRate = new Dictionary<char, List<int>>(lettersConfig.Letters.Length);
            
            uint totalLetters = 0;
            foreach (var letter in lettersConfig.Letters)
            {
                lettersGlobalRate.Add(letter, 0);
                lettersWordsRate.Add(letter, new List<int>());
            }
            
            foreach (var currentLetter in lettersConfig.Letters)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Generate JSON data", string.Empty, 
                    (float)lettersConfig.Letters.IndexOf(currentLetter) / lettersConfig.Letters.Length))
                    break;
                
                var filePath = $"{FilePath}{FilePathWithNouns}{currentLetter}.txt";
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"There is no file : {filePath}");
                    continue;
                }

                var lettersInWords = new Dictionary<char, int>();
                
                using (var fileStream = new StreamReader(filePath))
                {
                    string word;
                    while ((word = fileStream.ReadLine()) != null)
                    {
                        foreach (var letter in word)
                        {
                            if (letter == '.')
                            {
                                Debug.Log("");
                                EditorUtility.ClearProgressBar();
                                return;
                            }
                            
                            if (!lettersInWords.ContainsKey(letter))
                                lettersInWords.Add(letter, 0);

                            lettersInWords[letter]++;
                            lettersGlobalRate[letter]++;
                            totalLetters++;
                        }

                        foreach (var letter in lettersInWords.Keys)
                            lettersWordsRate[letter].Add(lettersInWords[letter]);
                        
                        lettersInWords.Clear();
                    }
                }
            }
            
            EditorUtility.ClearProgressBar();

            Debug.Log($"Total letters = {totalLetters}");
            
            foreach (var letter in lettersGlobalRate.Keys)
            {
                var rateGlobal = (float) Math.Round((double) lettersGlobalRate[letter] / totalLetters, 4);
                var rateInWord = lettersWordsRate[letter].Max() / 2;
                Debug.Log($"{letter} :: global = {rateGlobal}, inWord = {rateInWord}");
                lettersConfig.SetRate(letter, rateGlobal, rateInWord);
            }
            
            EditorUtility.SetDirty(lettersConfig);
            AssetDatabase.SaveAssets();
        }

        private static Dictionary<char, List<string>> GroupByLetters(List<string> words)
        {
            var result = new Dictionary<char, List<string>>();
            foreach (var word in words)
            {
                var firstLetter = word[0];
                if (!result.ContainsKey(firstLetter))
                    result.Add(firstLetter, new List<string>());
                
                result[firstLetter].Add(word);
            }

            return result;
        }
    }
}