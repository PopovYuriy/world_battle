using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "LettersRateConfig", menuName = "LettersRateConfig", order = 0)]
    public sealed class LettersRateConfig : ScriptableObject
    {
        [SerializeField] private char[] _letters;
        [SerializeField] private List<LetterRate> _lettersRate;

        public char[] Letters => _letters;

        public void SetRate(char letter, float rateGlobal, int rateInWord)
        {
            var rateData = _lettersRate.FirstOrDefault(d => d.Letter == letter);
            if (rateData != null)
            {
                rateData.RateGlobal = rateGlobal;
                rateData.RateInWord = rateInWord;
                return;
            }
            
            _lettersRate.Add(new LetterRate(letter, rateGlobal, rateInWord));
        }

        public int GetMaxOnField(char letter) => _lettersRate.First(d => d.Letter == letter).RateInWord; 

        public Dictionary<char, float> ToDictionary()
        {
            var result = new Dictionary<char, float>(_lettersRate.Count);
            foreach (var letterRate in _lettersRate)
                result.Add(letterRate.Letter, letterRate.RateGlobal);
            return result;
        }

        [Serializable]
        private sealed class LetterRate
        {
            [field: SerializeField] public char Letter { get; private set; }
            [field: SerializeField] public float RateGlobal { get; set; }
            [field: SerializeField] public int RateInWord { get; set; }

            public LetterRate(char letter, float rateGlobal, int rateInWord)
            {
                Letter = letter;
                RateGlobal = rateGlobal;
                RateInWord = rateInWord;
            }
        }
    }
}