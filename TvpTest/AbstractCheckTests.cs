﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AddInSideViews;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TvpMain.Project;
using TvpMain.Text;

namespace TvpTest
{
    public abstract class AbstractCheckTests
    {
        /// <summary>
        /// Test book number, for book-scale tests.
        /// </summary>
        protected const int TEST_BOOK_NUM = 10;

        /// <summary>
        /// Test chapter number, for chapter-scale tests.
        /// </summary>
        protected const int TEST_CHAPTER_NUM = 10;

        /// <summary>
        /// Multiplier for book numbers in BCV-style references.
        /// </summary>
        protected static readonly int TestBookRefMultiplier = 1000000;

        /// <summary>
        /// Multiplier for chapter numbers in BCV-style references.
        /// </summary>
        protected static readonly int TestChapRefMultiplier = 1000;

        /// <summary>
        /// Range ref parts (i.e., chapters, verses).
        /// </summary>
        protected static readonly int TestRefPartRange = 1000;

        /// <summary>
        /// Minimum number of books in test datasets.
        /// </summary>
        protected static readonly int MinTestBooks = 11;

        /// <summary>
        /// Minimum number of verses in test datasets.
        /// </summary>
        protected static readonly int MinTestVerses = 111;

        /// <summary>
        /// Test project name.
        /// </summary>
        protected const string TEST_PROJECT_NAME = "testProjectName";

        /// <summary>
        /// Test versification name.
        /// </summary>
        protected const string TEST_VERSIFICATION_NAME = "testVersificationName";

        /// <summary>
        /// Books present setting for test project.
        /// </summary>
        protected const string TEST_BOOKS_PRESENT_SETTING =
            "111111111111111111111111111111111111111111111111111111111111111111000000000000000000000000000000000000000000000000000000000";

        /// <summary>
        /// Chapter verse separator setting.
        /// </summary>
        protected const string CHAPTER_VERSE_SEPARATOR_SETTING = ":";

        /// <summary>
        /// Chapter verse separator setting.
        /// </summary>
        protected const string RANGE_INDICATOR_SETTING = "-";

        /// <summary>
        /// Sequence indicator setting.
        /// </summary>
        protected const string SEQUENCE_INDICATOR_SETTING = ",|;| («terrenal»),";

        /// <summary>
        /// Chapter range separator setting.
        /// </summary>
        protected const string CHAPTER_RANGE_SEPARATOR_SETTING = "–| al |—|b—";

        /// <summary>
        /// Book sequence separator setting.
        /// </summary>
        protected const string BOOK_SEQUENCE_SEPARATOR_SETTING = "; ";

        /// <summary>
        /// Chapter number separator setting.
        /// </summary>
        protected const string CHAPTER_NUMBER_SEPARATOR_SETTING = "; | y | ( ";

        /// <summary>
        /// Reference extra material setting.
        /// </summary>
        protected const string REFERENCE_EXTRA_MATERIAL_SETTING = "a|Salmos |capítulos |capítulo |cap. |Cap. | –| Tít.-50 –|cf.|) ";

        /// <summary>
        /// Reference final punctuation setting.
        /// </summary>
        protected const string REFERENCE_FINAL_PUNCTUATION_SETTING = "";

        /// <summary>
        /// Mock Paratext scripture extractor.
        /// </summary>
        protected Mock<IScrExtractor> MockExtractor;

        /// <summary>
        /// Mock Paratext scripture host.
        /// </summary>
        protected Mock<IHost> MockHost;

        /// <summary>
        /// Mock settings manager.
        /// </summary>
        protected Mock<ProjectManager> MockProjectManager;

        /// <summary>
        /// Test setup for verse lines and main mocks.
        /// </summary>
        public virtual void TestSetup()
        {
            MockHost = new Mock<IHost>(MockBehavior.Strict);
            MockExtractor = new Mock<IScrExtractor>(MockBehavior.Strict);
            MockProjectManager = new Mock<ProjectManager>(MockBehavior.Strict,
                MockHost.Object,
                TEST_PROJECT_NAME);

            // host setup
            MockHost.Setup(hostItem => hostItem.GetScriptureExtractor(TEST_PROJECT_NAME, ExtractorType.USFM))
                .Returns(MockExtractor.Object);
            MockHost.Setup(hostItem => hostItem.GetProjectVersificationName(TEST_PROJECT_NAME))
                .Returns(TEST_VERSIFICATION_NAME);
            MockHost.Setup(hostItem => hostItem.GetCurrentRef(TEST_VERSIFICATION_NAME))
                .Returns<string>((versificationName) =>
                GetVerseRef(TEST_BOOK_NUM, TEST_CHAPTER_NUM, 1));
            MockHost.Setup(hostItem => hostItem.GetLastChapter(It.IsAny<int>(), TEST_VERSIFICATION_NAME))
                .Returns<int, string>((bookNum, versificationName) => bookNum + MinTestBooks);
            MockHost.Setup(hostItem => hostItem.GetLastVerse(It.IsAny<int>(), It.IsAny<int>(), TEST_VERSIFICATION_NAME))
                .Returns<int, int, string>((bookNum, chapterNum, versificationName) => chapterNum + MinTestVerses);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "BooksPresent"))
                .Returns<string, string>((projectName, settingsKey) => TEST_BOOKS_PRESENT_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "ChapterVerseSeparator"))
                .Returns<string, string>((projectName, settingsKey) => CHAPTER_VERSE_SEPARATOR_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "RangeIndicator"))
                .Returns<string, string>((projectName, settingsKey) => RANGE_INDICATOR_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "SequenceIndicator"))
                .Returns<string, string>((projectName, settingsKey) => SEQUENCE_INDICATOR_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "ChapterRangeSeparator"))
                .Returns<string, string>((projectName, settingsKey) => CHAPTER_RANGE_SEPARATOR_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "BookSequenceSeparator"))
                .Returns<string, string>((projectName, settingsKey) => BOOK_SEQUENCE_SEPARATOR_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "ChapterNumberSeparator"))
                .Returns<string, string>((projectName, settingsKey) => CHAPTER_NUMBER_SEPARATOR_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "ReferenceExtraMaterial"))
                .Returns<string, string>((projectName, settingsKey) => REFERENCE_EXTRA_MATERIAL_SETTING);
            MockHost.Setup(hostItem => hostItem.GetProjectSetting(TEST_PROJECT_NAME, "ReferenceFinalPunctuation"))
                .Returns<string, string>((projectName, settingsKey) => REFERENCE_FINAL_PUNCTUATION_SETTING);
            MockHost.Setup(hostItem => hostItem.GetFigurePath(TEST_PROJECT_NAME, false))
                .Returns<string, bool>((projectName, localFlag) => Path.Combine(Directory.GetCurrentDirectory(), "test"));
        }

        /// <summary>
        /// Turn independent BCV values into a coordinate.
        /// </summary>
        /// <param name="bookNum">Paratext book number (1-66).</param>
        /// <param name="chapterNum">Paratext chapter number (book- and versification-specific).</param>
        /// <param name="verseNum">Paratext verse number (chapter- and versification-specific).</param>
        /// <returns></returns>
        protected static int GetVerseRef(int bookNum, int chapterNum, int verseNum)
        {
            var verseRef = bookNum * TestBookRefMultiplier;
            verseRef += chapterNum * TestChapRefMultiplier;
            verseRef += verseNum;
            return verseRef;
        }
    }
}
