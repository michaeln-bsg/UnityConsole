using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.UConsole
{
    public partial class Console
    {
        private readonly List<string> _inputHistory = new List<string>();

        private int _inputHistoryIndex;

        private void UpdateInputHistory()
        {
            var direction = 0;
            if (GetInputDown(_settings.InputHistoryUp))
            {
                direction = -1;
            }
            else if (GetInputDown(_settings.InputHistoryDown))
            {
                direction = 1;
            }

            if (direction != 0)
            {
                _inputHistoryIndex = Mathf.Clamp(
                    _inputHistoryIndex + direction,
                    0,
                    Mathf.Max(0, _inputHistory.Count - 1));

                if (_inputHistory.Count > 0)
                {
                    SetInput(_inputHistory[_inputHistoryIndex]);
                }
            }
        }
    }
}
