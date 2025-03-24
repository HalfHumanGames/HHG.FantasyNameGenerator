using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HHG.FantasyNameGenerator.Runtime
{
    public class FantasyNameGenerator : MonoBehaviour
    {
        private const string vowels = "aeiou";
        private const string consonants = "bcdfghjklmnpqrstvwxyz";
        private const string middleLetters = "aeioulnrst";

        [SerializeField] private int minLength = 4;
        [SerializeField] private int maxLength = 8;
        [SerializeField] private float middleLetterChance = 0.35f;
        [SerializeField] private TextAsset prefixesFile;
        [SerializeField] private TextAsset suffixesFile;

        private List<string> prefixes = new List<string>();
        private List<string> suffixes = new List<string>();

        private void Start()
        {
            LoadInputFiles();
        }

        private void LoadInputFiles()
        {
            if (prefixesFile == null)
            {
                Debug.LogError("No prefix file assigned!", this);
                return;
            }

            if (suffixesFile == null)
            {
                Debug.LogError("No suffix file assigned!", this);
                return;
            }

            LoadFileContentsIntoList(prefixesFile.text, prefixes);
            LoadFileContentsIntoList(suffixesFile.text, suffixes);
        }

        private void LoadFileContentsIntoList(string text, List<string> list)
        {
            list.Clear();
            list.AddRange(Regex.Split(text, @"\r?\n").Select(s => s.Trim()).Where(s => s.Length > 0));
        }

        private string InsertMiddleLetter(string prefix, string suffix)
        {
            if (prefix.Length == 0 || suffix.Length == 0)
            {
                return prefix + suffix;
            }

            char prefixLastChar = prefix[^1];
            char suffixFirstChar = suffix[0];

            bool prefixIsVowel = vowels.Contains(prefixLastChar);
            bool suffixIsVowel = vowels.Contains(suffixFirstChar);

            string validMiddleLetters;

            if (!prefixIsVowel && !suffixIsVowel)
            {
                validMiddleLetters = vowels;
            }
            else if (prefixIsVowel && suffixIsVowel)
            {
                validMiddleLetters = consonants;
            }
            else
            {
                validMiddleLetters = middleLetters;
            }

            char middleLetter;

            do
            {
                middleLetter = validMiddleLetters[Random.Range(0, validMiddleLetters.Length)];
            }
            while (middleLetter == prefixLastChar || middleLetter == suffixFirstChar);

            return prefix + middleLetter + suffix;
        }

        public void GenerateNames(int count, List<string> names)
        {
            names.Clear();

            for (int i = 0; i < count; i++)
            {
                names.Add(GenerateName());
            }
        }

        public string GenerateName()
        {
            if (prefixes.Count == 0 || suffixes.Count == 0)
            {
                Debug.LogError("Prefix or Suffix list is empty. Check your text file assignments.", this);
                return string.Empty;
            }

            int length;
            string prefix = string.Empty;
            string suffix = string.Empty;

            do
            {
                prefix = prefixes[Random.Range(0, prefixes.Count)];
                suffix = suffixes[Random.Range(0, suffixes.Count)];

                length = prefix.Length + suffix.Length;

            } while(prefix.ToLower() == suffix.ToLower() || length < minLength || length > maxLength);

            char prefixLastChar = prefix[^1];
            char suffixFirstChar = suffix[0];

            bool insertMiddleChar = Random.value < middleLetterChance || prefixLastChar == suffixFirstChar || length < maxLength;

            string name = insertMiddleChar ? InsertMiddleLetter(prefix, suffix) : prefix + suffix;

            return name;
        }

        [ContextMenu(nameof(Test))]
        private void Test()
        {
            LoadInputFiles();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 1000; i++)
            {
                sb.AppendLine(GenerateName());
            }

            Debug.Log(sb.ToString());
        }
    }
}
