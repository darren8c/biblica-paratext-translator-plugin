﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Text
{
    /// <summary>
    /// Paratext verse data.
    /// </summary>
    public class VerseData
    {
        /// <summary>
        /// Verse location.
        /// </summary>
        public VerseLocation VerseLocation { get; }

        /// <summary>
        /// Verse text, containing any combination of main text, footnotes or references, etc.
        /// </summary>
        public string VerseText { get; }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="verseLocation">Verse location (required).</param>
        /// <param name="verseText">Verse text (required).</param>
        public VerseData(VerseLocation verseLocation, string verseText)
        {
            VerseLocation = verseLocation ?? throw new ArgumentNullException(nameof(verseLocation));
            VerseText = verseText ?? throw new ArgumentNullException(nameof(verseText));
        }

        /// <summary>
        /// Helper factory method.
        /// </summary>
        /// <param name="bookNum">Book number (1-based).</param>
        /// <param name="chapterNum">Chapter number (1-based).</param>
        /// <param name="verseNum">Verse number (generally 1-based; 0 = any intro).</param>
        /// <param name="verseText">Verse text (required).</param>
        /// <returns>Created part data.</returns>
        public static VerseData Create(int bookNum, int chapterNum, int verseNum, string verseText)
        {
            return new VerseData(
                    new VerseLocation(bookNum, chapterNum, verseNum),
                    verseText);
        }

        /// <summary>
        /// Typed equality method.
        /// </summary>
        /// <param name="other">Other verse data (required).</param>
        /// <returns>True if equal, false otherwise</returns>
        protected bool Equals(VerseData other)
        {
            return Equals(VerseLocation, other.VerseLocation)
                   && VerseText == other.VerseText;
        }

        /// <summary>
        /// Standard equality method.
        /// </summary>
        /// <param name="obj">Other object (optional, may be null).</param>
        /// <returns>True if equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VerseData)obj);
        }

        /// <summary>
        /// Standard hash code method.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((VerseLocation != null
                            ? VerseLocation.GetHashCode() : 0) * 397)
                       ^ (VerseText != null ? VerseText.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Typed equality operator.
        /// </summary>
        /// <param name="left">Left verse data.</param>
        /// <param name="right">Right verse data.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public static bool operator ==(VerseData left, VerseData right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Standard equality operator.
        /// </summary>
        /// <param name="left">Left object.</param>
        /// <param name="right">Right object.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public static bool operator !=(VerseData left, VerseData right)
        {
            return !Equals(left, right);
        }
    }
}
