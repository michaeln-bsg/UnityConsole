using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole {
    public abstract class CFunc {
        public string alias { get; protected set; }
        public string description { get; protected set; }

        public CFunc(string alias, string description) {
            if(ValidStrValues(alias, description) == false) {
                throw new System.ArgumentException("Alias and description required!");
            }
            this.alias = alias;
            this.description = description;
            UConsoleDB.RegisterCFunc(this);
        }

        bool ValidStrValues(params string[] strs) {
            foreach(var s in strs) {
                if(s == null || s.Trim().Length == 0) {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode() {
            return alias.GetHashCode();
        }
    }
}