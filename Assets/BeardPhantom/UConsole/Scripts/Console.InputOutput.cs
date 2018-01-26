using System.Text.RegularExpressions;
using UnityEngine;

namespace BeardPhantom.UConsole
{
    /// <summary>
    /// Handles text input/output
    /// </summary>
    public partial class Console
    {
        private const string EMPTY_INPUT_REGEX_PATTERN = @"^\s*\n+$";

        private readonly Regex _emptyInputRegex = new Regex(EMPTY_INPUT_REGEX_PATTERN);

        public void PrintErr(object output)
        {
            Print(output, _settings.ErrorColor);
        }

        public void PrintErr(string output)
        {
            Print(output, _settings.ErrorColor);
        }

        public void PrintWarn(object output)
        {
            Print(output, _settings.WarningColor);
        }

        public void PrintWarn(string output)
        {
            Print(output, _settings.WarningColor);
        }

        public void Print(object output)
        {
            Print(output, _settings.DefaultColor);
        }

        public void Print(string output)
        {
            Print(output, _settings.DefaultColor);
        }

        public void Print(object output, Color color)
        {
            if (output != null)
            {
                PrintInternal(output.ToString(), color);
            }
        }

        public void Print(string output, Color color)
        {
            if (!string.IsNullOrEmpty(output))
            {
                PrintInternal(output, color);
            }
        }

        protected void PrintInternal(string text, Color color)
        {
            var instance = Instantiate(_outputTemplate,
                _scrollRect.content);
            instance.text = text.Trim();
            instance.color = color;
            instance.gameObject.SetActive(true);
            _scrollToEndCounter = 2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void OnInputValueChanged(string value)
        {
            if (_emptyInputRegex.IsMatch(value))
            {
                // Remove strings that are just whitespace + newlines
                _input.text = string.Empty;
            }
            else if (value.EndsWith("\n"))
            {
                SubmitInput(value.Replace("\n", string.Empty).Trim());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetInput(string value)
        {
            _input.text = value;
            _input.caretPosition = _input.text.Length;
        }

        public void ClearOutput()
        {
            var parent = _outputTemplate.transform.parent;
            _outputTemplate.transform.SetAsFirstSibling();
            for (int i = parent.childCount - 1; i > 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
    }
}