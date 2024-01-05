using System.Collections.Generic;
using System.Linq;
using Game.Data;
using UnityEngine;
using Utils.Extensions;

namespace Game.Services
{
    public static class LettersGenerator
    {
        private static readonly List<char> Chars = new() { 'а', 'б', 'в', 'г', 'ґ', 'д', 'е', 'є', 'ж', 'з', 'и', 'і', 
            'ї', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ь', 'ю', 'я' };

        private static readonly Dictionary<char, float> CharsRate = new()
        {
            { 'а', 0.064f }, { 'б', 0.013f }, { 'в', 0.046f }, { 'г', 0.013f }, { 'ґ', 0.0001f }, { 'д', 0.027f }, { 'е', 0.042f }, 
            { 'є', 0.005f }, { 'ж', 0.007f }, { 'з', 0.020f }, { 'и', 0.055f }, { 'і', 0.044f }, { 'ї', 0.01f }, { 'й', 0.009f }, 
            { 'к', 0.033f }, { 'л', 0.027f }, { 'м', 0.029f }, { 'н', 0.068f }, { 'о', 0.086f }, { 'п', 0.025f }, { 'р', 0.043f }, 
            { 'с', 0.037f }, { 'т', 0.045f }, { 'у', 0.027f }, { 'ф', 0.003f }, { 'х', 0.011f }, { 'ц', 0.010f }, { 'ч', 0.011f }, 
            { 'ш', 0.005f }, { 'щ', 0.004f }, { 'ь', 0.016f }, { 'ю', 0.008f }, { 'я', 0.019f }
        };
        
        public static char[] Generate(int count)
        {
            return GetRandomLetters(count);
        }

        private static char[] GetRandomLetters(int count)
        {
            var result = new char[count];
            
            var lettersConfig = (LettersRateConfig)Resources.Load("LettersRateConfig");

            var lettersRate = lettersConfig.ToDictionary();
            count.Iterate(index =>
            {
                var rateSum = lettersRate.Values.Sum();
                var eps = Random.Range(0f, rateSum);
                
                var currentDelta = eps;
                var letter = lettersConfig.Letters.First(l =>
                {
                    currentDelta -= lettersRate[l];
                    var match = currentDelta <= 0;
                    if (match)
                    {
                        var newProb = Mathf.Max(lettersRate[l] - lettersRate[l] / lettersConfig.GetMaxOnField(l), 0f);
                        
                        Debug.Log($"Letter '{l}' :: count = {result.Count(k => k == l)}, " +
                                  $"max = {lettersConfig.GetMaxOnField(l)}, prevProb = {lettersRate[l]}, prob = {newProb}");
                        lettersRate[l] = newProb;
                    }
                    return match;
                });
                result[index] = letter;
            });

            return result;
        }
    }
}