using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardinal.UmbracoExt.Migrations
{
    /// <summary>
    /// A simple wrapper around formatted string to handle comparing major and minor version numbers
    /// </summary>
    public class VersionNumber : IComparable
    {
        private readonly string _versionString;

        public VersionNumber(string versionString)
        {
            _versionString = versionString;

            if (string.IsNullOrEmpty(versionString))
                _versionString = "0";

            if (!_versionString.Contains("."))
                _versionString = _versionString + ".0";

            
        }

        public VersionNumber()
        {
            _versionString = "0";
        }

        public int CompareTo(object obj)
        {
            if (obj is VersionNumber)
            {
                var compareReturn = 0;
                var compareObj = obj as VersionNumber;
                var currentVersionParts = _versionString.Split('.').Select(p => int.Parse(p)).ToArray();
                var compareVersionParts = compareObj._versionString.Split('.').Select(p => int.Parse(p)).ToArray();
                int index = 0;
                while (currentVersionParts.Length - 1 > index && compareVersionParts.Length - 1 > index &&
                       currentVersionParts[index] == compareVersionParts[index])
                {
                    index++;
                }
                if (currentVersionParts[index] != compareVersionParts[index])
                    compareReturn = currentVersionParts[index].CompareTo(compareVersionParts[index]);
                else
                {
                    var longerVersionNumber = (currentVersionParts.Length > compareVersionParts.Length)
                                                  ? currentVersionParts
                                                  : compareVersionParts;
                    var longerNumberHasANonZeroPartInRemaingParts = longerVersionNumber.Skip(index).Sum() > 0;
                    if (longerNumberHasANonZeroPartInRemaingParts)
                        compareReturn = currentVersionParts.Length.CompareTo(compareVersionParts.Length);
                }

                return compareReturn;
            }
            else
                throw new ArgumentException("Must compare to another Version Number");
        }

        public override int GetHashCode()
        {
            return _versionString.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return this.CompareTo(obj) == 0;
        }

        public override string ToString()
        {
            return _versionString;
        }

        public static bool operator ==(VersionNumber v1, VersionNumber v2)
        {
            if (ReferenceEquals(v1, null) && ReferenceEquals(v2, null))
                return true;

            if (ReferenceEquals(v1, null))
                return false;

            return v1.Equals(v2);
        }

        public static bool operator !=(VersionNumber v1, VersionNumber v2)
        {
            return !(v1 == v2);
        }

        public static bool operator <(VersionNumber v1, VersionNumber v2)
        {
            return (v1.CompareTo(v2) < 0);
        }

        public static bool operator >(VersionNumber v1, VersionNumber v2)
        {
            return (v1.CompareTo(v2) > 0);
        }
    }
}
