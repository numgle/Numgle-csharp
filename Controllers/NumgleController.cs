using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numgle.Controllers
{
    public class NumgleController : ControllerBase
    {
        [Route("/{input}")]
        public ActionResult<string> Get(string input)
        {
            return new NumgleConvertor().ConvertStringToNumgle(input);
        }
    }

    public class NumgleConvertor
    {
        #region Numgle Data
        private string[] converted_cho = { "J", "ᖵ", "r", "n", "Д", "ru", "ㅁ", "ㄸ", "뚠", ">", "ᕒ", "ㅇ", "Ʞ", "ᕒ|", "Ʞ-", "ㅚ", "m", "ㅒ", "아" };
        private string[] converted_jong = { "", "J", "ᖵ", "⋝'", "r", "5ı", "δ˫", "n", "ru", "「늬", "「님", "ꈉ'", "⪞", "ꉱ", "", "ꂚ˫", "ㅁ", "ㄸ", "⪚", ">", "ᕒ", "ㅇ", "Ʞ", "Ʞ-", "ㅚ", "m", "ㅒ", "아" };
        private string[] converted_jung = { "ㅏ", "ᅷ", "左", "上", "ㅑ", "ㅓ", "ᅺ", "곤", "ᅼ", "ㅕ", "l", "⊥" };
        private string[,] converted_cj = { { "ᆗ", "ᅾ", "F", "宁", "早", "「뉘", "무", "뚜", "", "수", "寻", "우", "쉬", "퀴", "쉬-", "ᆗ┘", "쑤", "부", "위-" },
                                           { "≚", "", "도", "모", "圼", "ꄹ", "문", "뚠", "", "㒰", "", "운", "쉰", "퀸", "쉰-", "ꁈ", "쑨", "분", "윈-" },
                                           { "", "", "斤", "「규", "l큐", "「뉴|", "뮤", "뜌", "", "슈", "", "유", "슈|", "큐|", "슈|-", "=뉴l", "쓔", "뷰", "유|-" },
                                           { "", "", "됴", "묘", "昱", "「뉸l", "뮨", "뜐", "", "슌", "", "윤", "슌|", "큔|", "슌|-", "=뉸l", "쓘", "뷴", "윤|-" },
                                           { "ᖵ", "ᅽ", "「工", "「고", "l코", "ꌰ", "모", "또", "", "소", "", "오", "쇠", "쾨", "솨", "=뇌", "쏘", "보", "와" },
                                           { "≚", "", "뜨", "「곤", "l콘", "ꄹ", "몬", "똔", "", "손", "", "온", "쇤", "쾬", "솬", "ꁈ", "쏜", "본", "완" },
                                           { "", "", "「立", "「교", "l쿄", "「뇨|", "묘", "뚀", "", "쇼", "", "요", "쇼|", "쿄|", "쇼|-", "=뇨l", "쑈", "뵤", "요|-" },
                                           { "", "", "「프", "「굔", "l쿈", "「뇬|", "묜", "뚄", "", "숀", "", "욘", "숀|", "쿈|", "숀|-", "=뇬l", "쑌", "뵨", "욘|-" },
                                           { "⇲", "", "ㄷ", "「그", "日", "Ṉ", "므", "뜨", "", "≥", "", "으", "≥|", "킈", "≥|-", "=늬", "쓰", "브", "의-" }
                                         };
        private string[] converted_han = { "J", "ᖵ", "⋝'", "r", "5ı", "δ˫", "n", "Д", "ru", "「늬", "「님", "ꈉ'", "ꉱ", "", "ꂚ˫", "ㅁ", "ㄸ", "뚠", "⪚", ">", "ᕒ", "ㅇ", "ꓘ", "ᕒ|", "ꓘ-", "ㅚ", "m", "ㅒ", "아", "ㅜ", "工", "ㅠ", "ㅍ", "ㅗ", "〧", "ㅛ", "", "ㅏ", "ᅷ", "左", "上", "ㅑ", "ㅓ", "ᅺ", "", "ᅼ", "ㅕ", "l", "⊥", "ㅡ" };
        private string[] converted_english = { "ᗆ", "ϖ", "∩", "ᗜ", "m", "ㄲ", "ᘏ", "工", "ㅡ", "(__", "ㅈ", "┌-", "ᕒ", "Z", "O", "‾ᗜ", ",O", "7ᗜ", "∽", "-ㅓ", "⊂", "<", "ε", "X", "-<", "N" };
        private string[] converted_number = { "o", "ㅡ", "ru", "ω", "-F", "UT", "0‾‾", "__|", "∞", "__0" };
        private string[] converted_special = { "·-J", "·ㅡ", ".", ">", "ㅣ" };
        #endregion

        public string ConvertStringToNumgle(string input)
        {
            if (input.Length == 0) return "";

            var output = ConvertCharToNumgle(input[0]);

            for (var i = 1; i < input.Length; i++)
                output += '\n' + ConvertCharToNumgle(input[i]);

            return output;
        }

        public string ConvertCharToNumgle(char input)
        {
            var letterType = GetLetterType(input);

            switch (letterType)
            {
                case LetterType.Empty: 
                    return "";

                case LetterType.CompleteHangul: {
                    var seperatedHan = SeperateHan(input);

                    if (!IsInData(seperatedHan.cho, seperatedHan.jung, seperatedHan.jong))
                    {
                        Console.WriteLine("변환하지 못한 글자가 포함되어 있습니다.");
                        return "";
                    }

                    if (seperatedHan.jung >= 8 && seperatedHan.jung != 20)
                    {
                        return converted_jong[seperatedHan.jong] + converted_jung[seperatedHan.jung - 8] + converted_cho[seperatedHan.cho];
                    }

                    return converted_jong[seperatedHan.jong] + converted_cj[Math.Min(8, seperatedHan.jung), seperatedHan.cho];
                }

                case LetterType.NotCompleteHangul:
                    return converted_han[input - 3131];

                case LetterType.English:
                    return converted_english[input - 65];

                case LetterType.Number:
                    return converted_number[input - 48];

                case LetterType.SpecialLetter:
                    return converted_special["?!.^-".IndexOf(input)];

                case LetterType.Unknown:
                default:
                    Console.WriteLine("변환하지 못한 글자가 포함되어 있습니다.");
                    return "";
            }
        }

        public (int cho, int jung, int jong) SeperateHan(char han)
        {
            return ((han - 44032) / 28 / 21, (han - 44032) / 28 % 21, (han - 44032) % 28);
        }

        public bool IsInData(int cho_num, int jung_num, int jong_num)
        {
            if (jong_num == 0 || converted_jong[jong_num] != "") return true;
            if (jung_num >= 8 && jung_num != 20) return converted_jung[jung_num - 8] != "";
            else return converted_cj[Math.Min(8, jung_num), cho_num] != "";
        }

        public LetterType GetLetterType(char letter)
        {
            if (letter == ' ' || letter == '\r' || letter == '\n') return LetterType.Empty;
            else if (letter >= 44032 && letter <= 55203) return LetterType.CompleteHangul;
            else if (letter >= 3131 && letter <= 3163) return LetterType.NotCompleteHangul;
            else if (letter >= 65 && letter <= 90) return LetterType.English;
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
        English,
        Number,
        SpecialLetter,
        Unknown
    }
}
