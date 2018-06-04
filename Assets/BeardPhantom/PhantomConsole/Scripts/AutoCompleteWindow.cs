using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.PhantomConsole
{
    /// <summary>
    /// Used to show auto complete results
    /// </summary>
    public class AutoCompleteWindow : MonoBehaviour
    {
        /// <summary>
        /// Labels for results
        /// </summary>
        private ConsoleTextLabel[] _rowLabels;

        private void Awake()
        {
            _rowLabels = GetComponentsInChildren<ConsoleTextLabel>(true);
        }

        /// <summary>
        /// Updates view with results
        /// </summary>
        /// <param name="results"></param>
        public void SetRowValues(IList<string> results)
        {
            if(!gameObject.activeInHierarchy)
            {
                return;
            }
            for(var i = 0; i < _rowLabels.Length; i++)
            {
                _rowLabels[i].gameObject.SetActive(i < results.Count);
                if(i < results.Count)
                {
                    _rowLabels[i].Text = i == _rowLabels.Length - 1
                        ? string.Format("<{0} more results>", results.Count - (_rowLabels.Length - 1))
                        : results[i];
                }
            }
        }
    }
}