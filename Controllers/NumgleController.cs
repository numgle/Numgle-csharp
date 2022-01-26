using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Numgle.Controllers
{
    public class NumgleController : ControllerBase
    {
        [Route("/{input}")]
        public ActionResult<string> Get(string input)
        {
            return new NumgleConvertor(new DataLoader().Load()).ToNumgle(input);
        }
    }

    public class NumgleConvertor
    {
        private string[] converted_cho, converted_jung, converted_jong, converted_han, converted_english_upper, converted_english_lower, converted_number, converted_special;
        private string[,] converted_cj;

        public NumgleConvertor(NumgleData data)
        {
            converted_cho = data.converted_cho;
            converted_jung = data.converted_jung;
            converted_jong = data.converted_jong;
            converted_cj = data.converted_cj;
            converted_han = data.converted_han;
            converted_english_upper = data.converted_english_upper;
            converted_english_lower = data.converted_english_lower;
            converted_number = data.converted_number;
            converted_special = data.converted_special;
        }

        public string ToNumgle(string input)
        {
            var output = new List<string>();

            for (var i = 0; i < input.Length; i++)
                output.Add(ToNumgle(input[i]));

            return string.Join("\n", output);
        }

        public string ToNumgle(char input)
        {
            var letterType = GetLetterType(input);

            switch (letterType)
            {
                case LetterType.Empty: 
                    return "";
                case LetterType.CompleteHangul:
                    var (cho, jung, jong) = SeperateHan(input);
                    if (!IsInData(cho, jung, jong))
                        return "";
                    if (jung >= 8 && jung != 20)
                        return converted_jong[jong] + converted_jung[jung - 8] + converted_cho[cho];
                    return converted_jong[jong] + converted_cj[Math.Min(8, jung), cho];
                case LetterType.NotCompleteHangul:
                    return converted_han[input - 0x3131];
                case LetterType.EnglishUpper:
                    return converted_english_upper[input - 65];
                case LetterType.EnglishLower:
                    return converted_english_lower[input - 97];
                case LetterType.Number:
                    return converted_number[input - 48];
                case LetterType.SpecialLetter:
                    return converted_special["?!.^-".IndexOf(input)];
                case LetterType.Unknown:
                default:
                    return "";
            }
        }

        public (int cho, int jung, int jong) SeperateHan(char han)
        => ((han - 44032) / 28 / 21, (han - 44032) / 28 % 21, (han - 44032) % 28);

        public bool IsInData(int cho_num, int jung_num, int jong_num)
        {
            if (jong_num != 0 && converted_jong[jong_num] == "") return false;
            if (jung_num >= 8 && jung_num != 20) return converted_jung[jung_num - 8] != "";
            else return converted_cj[Math.Min(8, jung_num), cho_num] != "";
        }

        public LetterType GetLetterType(char letter)
        {
            if (letter == ' ' || letter == '\r' || letter == '\n') return LetterType.Empty;
            else if (letter >= 44032 && letter <= 55203) return LetterType.CompleteHangul;
            else if (letter >= 0x3131 && letter <= 0x3163) return LetterType.NotCompleteHangul;
            else if (letter >= 65 && letter <= 90) return LetterType.EnglishUpper;
            else if (letter >= 97 && letter <= 122) return LetterType.EnglishLower;
            else if (letter >= 48 && letter <= 57) return LetterType.Number;
            else if ("?!.^-".Contains(letter)) return LetterType.SpecialLetter;
            else return LetterType.Unknown;
        }
    }

    public enum LetterType
    {
        Empty,
        CompleteHangul,
        NotCompleteHangul,
        EnglishUpper,
        EnglishLower,
        Number,
        SpecialLetter,
        Unknown
    }
}
