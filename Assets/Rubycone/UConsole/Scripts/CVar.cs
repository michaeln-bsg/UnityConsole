using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole {
    public delegate void OnValueChanged(ValueContainer oldValues, CVar cvar);

    [System.Flags]
    public enum CVarFlags {
        None = 0,
        Archive = 1,
        ReadOnly = 2,
        Cheat = 4,
        SP_Only = 8,
        MP_Only = 16
    }

    public struct ValueContainer {
        string sVal;
        float fVal;
        int iVal;

        public ValueContainer(string sVal, float fVal, int iVal) {
            this.sVal = sVal;
            this.fVal = fVal;
            this.iVal = iVal;
        }
    }

    public sealed class CVar : CFunc {
        public event OnValueChanged CVarValueChanged;

        string defaultVal;

        public CVarFlags flags { get; private set; }
        public string sVal { get; private set; }
        public float fVal { get; private set; }
        public int iVal { get; private set; } //also represents boolean state, nonzero == true

        float _fMin, _fMax;
        public float fMin {
            get { return _fMin; }
            set {
                if(value <= _fMax) {
                    _fMin = value;
                }
            }
        }
        public float fMax {
            get { return _fMax; }
            set {
                if(value >= _fMin) {
                    _fMax = value;
                }
            }
        }

        public CVar(string alias, string description, object defaultVal)
            : this(alias, description, defaultVal, CVarFlags.None) { }

        public CVar(string alias, string description, object defaultVal, CVarFlags flags)
            : base(alias, description) {
            this.defaultVal = defaultVal.ToString();

            if(CheckFlags(flags) == false) {
                throw new System.ArgumentException("Invalid flags!");
            }
            this.flags = flags;
            SetValue(defaultVal);
        }

        private bool CheckFlags(CVarFlags flags) {
            if((flags & CVarFlags.SP_Only) != 0 &&
                (flags & CVarFlags.MP_Only) != 0) {
                return false;
            }
            return true;
        }

        public void FireValueChanged(ValueContainer oldValues) {
            if(CVarValueChanged != null) {
                CVarValueChanged(oldValues, this);
            }
        }


        public void Revert() {
            SetValue(defaultVal);
        }

        public void SetValue(object value) {
            var valueStr = value.ToString();

            var oldValues = new ValueContainer(sVal, fVal, iVal);

            //parse float value if possible
            var fNewVal = 0f;
            var fOldVal = fVal;
            bool isNum = false;
            if(value == null) {
                fNewVal = 0f;
            }
            else {
                isNum = float.TryParse(valueStr, out fNewVal);
            }

            ClampFVal(fNewVal, out fNewVal);

            fVal = fNewVal;
            iVal = (int)fNewVal;

            if(isNum) {
                sVal = fNewVal.ToString();
            }
            else {
                sVal = valueStr;
            }

            FireValueChanged(oldValues);
        }

        bool ClampFVal(float fOldVal, out float fNewVal) {
            if(fMin != fMax) {
                fNewVal = Mathf.Clamp(fOldVal, fMin, fMax);
                return true;
            }
            else {
                fNewVal = fOldVal;
                return false;
            }
        }
    }
}