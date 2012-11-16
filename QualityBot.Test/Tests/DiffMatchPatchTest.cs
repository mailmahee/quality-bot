/*
 * Copyright 2008 Google Inc. All Rights Reserved.
 * Author: fraser@google.com (Neil Fraser)
 * Author: anteru@developer.shelter13.net (Matthaeus G. Chajdas)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Diff Match and Patch -- Test Harness
 * http://code.google.com/p/google-diff-match-patch/
 */

namespace QualityBot.Test.Tests {

    using QualityBot.Util;
    using System.Collections.Generic;
    using System;
    using System.Text;
    using NUnit.Framework;
    [TestFixture()]
  public class diff_match_patchTest : DiffMatchPatch{
    [Test()]
    public void diff_commonPrefixTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Detect any common suffix.
      // Null case.
      Assert.AreEqual(0, dmp.DiffCommonPrefix("abc", "xyz"));

      // Non-null case.
      Assert.AreEqual(4, dmp.DiffCommonPrefix("1234abcdef", "1234xyz"));

      // Whole case.
      Assert.AreEqual(4, dmp.DiffCommonPrefix("1234", "1234xyz"));
    }

    [Test()]
    public void diff_commonSuffixTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Detect any common suffix.
      // Null case.
      Assert.AreEqual(0, dmp.DiffCommonSuffix("abc", "xyz"));

      // Non-null case.
      Assert.AreEqual(4, dmp.DiffCommonSuffix("abcdef1234", "xyz1234"));

      // Whole case.
      Assert.AreEqual(4, dmp.DiffCommonSuffix("1234", "xyz1234"));
    }

    [Test()]
    public void diff_commonOverlapTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Detect any suffix/prefix overlap.
      // Null case.
      Assert.AreEqual(0, dmp.DiffCommonOverlap("", "abcd"));

      // Whole case.
      Assert.AreEqual(3, dmp.DiffCommonOverlap("abc", "abcd"));

      // No overlap.
      Assert.AreEqual(0, dmp.DiffCommonOverlap("123456", "abcd"));

      // Overlap.
      Assert.AreEqual(3, dmp.DiffCommonOverlap("123456xxx", "xxxabcd"));

      // Unicode.
      // Some overly clever languages (C#) may treat ligatures as equal to their
      // component letters.  E.g. U+FB01 == 'fi'
      Assert.AreEqual(0, dmp.DiffCommonOverlap("fi", "\ufb01i"));
    }

    [Test()]
    public void diff_halfmatchTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      dmp.Diff_Timeout = 1;
      // No match.
      Assert.IsNull(dmp.DiffHalfMatch("1234567890", "abcdef"));

      Assert.IsNull(dmp.DiffHalfMatch("12345", "23"));

      // Single Match.
      CollectionAssert.AreEqual(new string[] { "12", "90", "a", "z", "345678" }, dmp.DiffHalfMatch("1234567890", "a345678z"));

      CollectionAssert.AreEqual(new string[] { "a", "z", "12", "90", "345678" }, dmp.DiffHalfMatch("a345678z", "1234567890"));

      CollectionAssert.AreEqual(new string[] { "abc", "z", "1234", "0", "56789" }, dmp.DiffHalfMatch("abc56789z", "1234567890"));

      CollectionAssert.AreEqual(new string[] { "a", "xyz", "1", "7890", "23456" }, dmp.DiffHalfMatch("a23456xyz", "1234567890"));

      // Multiple Matches.
      CollectionAssert.AreEqual(new string[] { "12123", "123121", "a", "z", "1234123451234" }, dmp.DiffHalfMatch("121231234123451234123121", "a1234123451234z"));

      CollectionAssert.AreEqual(new string[] { "", "-=-=-=-=-=", "x", "", "x-=-=-=-=-=-=-=" }, dmp.DiffHalfMatch("x-=-=-=-=-=-=-=-=-=-=-=-=", "xx-=-=-=-=-=-=-="));

      CollectionAssert.AreEqual(new string[] { "-=-=-=-=-=", "", "", "y", "-=-=-=-=-=-=-=y" }, dmp.DiffHalfMatch("-=-=-=-=-=-=-=-=-=-=-=-=y", "-=-=-=-=-=-=-=yy"));

      // Non-optimal halfmatch.
      // Optimal diff would be -q+x=H-i+e=lloHe+Hu=llo-Hew+y not -qHillo+x=HelloHe-w+Hulloy
      CollectionAssert.AreEqual(new string[] { "qHillo", "w", "x", "Hulloy", "HelloHe" }, dmp.DiffHalfMatch("qHilloHelloHew", "xHelloHeHulloy"));

      // Optimal no halfmatch.
      dmp.Diff_Timeout = 0;
      Assert.IsNull(dmp.DiffHalfMatch("qHilloHelloHew", "xHelloHeHulloy"));
    }

    [Test()]
    public void diff_linesToCharsTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Convert lines down to characters.
      List<string> tmpVector = new List<string>();
      tmpVector.Add("");
      tmpVector.Add("alpha\n");
      tmpVector.Add("beta\n");
      Object[] result = dmp.DiffLinesToChars("alpha\nbeta\nalpha\n", "beta\nalpha\nbeta\n");
      Assert.AreEqual("\u0001\u0002\u0001", result[0]);
      Assert.AreEqual("\u0002\u0001\u0002", result[1]);
      CollectionAssert.AreEqual(tmpVector, (List<string>)result[2]);

      tmpVector.Clear();
      tmpVector.Add("");
      tmpVector.Add("alpha\r\n");
      tmpVector.Add("beta\r\n");
      tmpVector.Add("\r\n");
      result = dmp.DiffLinesToChars("", "alpha\r\nbeta\r\n\r\n\r\n");
      Assert.AreEqual("", result[0]);
      Assert.AreEqual("\u0001\u0002\u0003\u0003", result[1]);
      CollectionAssert.AreEqual(tmpVector, (List<string>)result[2]);

      tmpVector.Clear();
      tmpVector.Add("");
      tmpVector.Add("a");
      tmpVector.Add("b");
      result = dmp.DiffLinesToChars("a", "b");
      Assert.AreEqual("\u0001", result[0]);
      Assert.AreEqual("\u0002", result[1]);
      CollectionAssert.AreEqual(tmpVector, (List<string>)result[2]);

      // More than 256 to reveal any 8-bit limitations.
      int n = 300;
      tmpVector.Clear();
      StringBuilder lineList = new StringBuilder();
      StringBuilder charList = new StringBuilder();
      for (int x = 1; x < n + 1; x++) {
        tmpVector.Add(x + "\n");
        lineList.Append(x + "\n");
        charList.Append(Convert.ToChar(x));
      }
      Assert.AreEqual(n, tmpVector.Count);
      string lines = lineList.ToString();
      string chars = charList.ToString();
      Assert.AreEqual(n, chars.Length);
      tmpVector.Insert(0, "");
      result = dmp.DiffLinesToChars(lines, "");
      Assert.AreEqual(chars, result[0]);
      Assert.AreEqual("", result[1]);
      CollectionAssert.AreEqual(tmpVector, (List<string>)result[2]);
    }

    [Test()]
    public void diff_charsToLinesTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Convert chars up to lines.
      List<Diff> diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "\u0001\u0002\u0001"),
          new Diff(Operation.INSERT, "\u0002\u0001\u0002")};
      List<string> tmpVector = new List<string>();
      tmpVector.Add("");
      tmpVector.Add("alpha\n");
      tmpVector.Add("beta\n");
      dmp.DiffCharsToLines(diffs, tmpVector);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.EQUAL, "alpha\nbeta\nalpha\n"),
          new Diff(Operation.INSERT, "beta\nalpha\nbeta\n")}, diffs);

      // More than 256 to reveal any 8-bit limitations.
      int n = 300;
      tmpVector.Clear();
      StringBuilder lineList = new StringBuilder();
      StringBuilder charList = new StringBuilder();
      for (int x = 1; x < n + 1; x++) {
        tmpVector.Add(x + "\n");
        lineList.Append(x + "\n");
        charList.Append(Convert.ToChar (x));
      }
      Assert.AreEqual(n, tmpVector.Count);
      string lines = lineList.ToString();
      string chars = charList.ToString();
      Assert.AreEqual(n, chars.Length);
      tmpVector.Insert(0, "");
      diffs = new List<Diff> {new Diff(Operation.DELETE, chars)};
      dmp.DiffCharsToLines(diffs, tmpVector);
      CollectionAssert.AreEqual(new List<Diff>
          {new Diff(Operation.DELETE, lines)}, diffs);
    }

    [Test()]
    public void DiffCleanupMergeTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Cleanup a messy diff.
      // Null case.
      List<Diff> diffs = new List<Diff>();
      dmp.DiffCleanupMerge(diffs);
      CollectionAssert.AreEqual(new List<Diff>(), diffs);

      // No change case.
      diffs = new List<Diff> {new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "b"), new Diff(Operation.INSERT, "c")};
      dmp.DiffCleanupMerge(diffs);
      CollectionAssert.AreEqual(new List<Diff> {new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "b"), new Diff(Operation.INSERT, "c")}, diffs);

      // Merge equalities.
      diffs = new List<Diff> {new Diff(Operation.EQUAL, "a"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.EQUAL, "c")};
      dmp.DiffCleanupMerge(diffs);
      CollectionAssert.AreEqual(new List<Diff> {new Diff(Operation.EQUAL, "abc")}, diffs);

      // Merge deletions.
      diffs = new List<Diff> {new Diff(Operation.DELETE, "a"), new Diff(Operation.DELETE, "b"), new Diff(Operation.DELETE, "c")};
      dmp.DiffCleanupMerge(diffs);
      CollectionAssert.AreEqual(new List<Diff> {new Diff(Operation.DELETE, "abc")}, diffs);

      // Merge insertions.
      diffs = new List<Diff> {new Diff(Operation.INSERT, "a"), new Diff(Operation.INSERT, "b"), new Diff(Operation.INSERT, "c")};
      dmp.DiffCleanupMerge(diffs);
      CollectionAssert.AreEqual(new List<Diff> {new Diff(Operation.INSERT, "abc")}, diffs);

      // Merge interweave.
      diffs = new List<Diff> {new Diff(Operation.DELETE, "a"), new Diff(Operation.INSERT, "b"), new Diff(Operation.DELETE, "c"), new Diff(Operation.INSERT, "d"), new Diff(Operation.EQUAL, "e"), new Diff(Operation.EQUAL, "f")};
      dmp.DiffCleanupMerge(diffs);
      CollectionAssert.AreEqual(new List<Diff> {new Diff(Operation.DELETE, "ac"), new Diff(Operation.INSERT, "bd"), new Diff(Operation.EQUAL, "ef")}, diffs);

      // Prefix and suffix detection.
      diffs = new List<Diff> {new Diff(Operation.DELETE, "a"), new Diff(Operation.INSERT, "abc"), new Diff(Operation.DELETE, "dc")};
      dmp.DiffCleanupMerge(diffs);
      CollectionAssert.AreEqual(new List<Diff> {new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "d"), new Diff(Operation.INSERT, "b"), new Diff(Operation.EQUAL, "c")}, diffs);

      // Prefix and suffix detection with equalities.
      diffs = new List<Diff> {new Diff(Operation.EQUAL, "x"), new Diff(Operation.DELETE, "a"), new Diff(Operation.INSERT, "abc"), new Diff(Operation.DELETE, "dc"), new Diff(Operation.EQUAL, "y")};
      dmp.DiffCleanupMerge(diffs);
      CollectionAssert.AreEqual(new List<Diff> {new Diff(Operation.EQUAL, "xa"), new Diff(Operation.DELETE, "d"), new Diff(Operation.INSERT, "b"), new Diff(Operation.EQUAL, "cy")}, diffs);

      // Slide edit left.
      diffs = new List<Diff> {new Diff(Operation.EQUAL, "a"), new Diff(Operation.INSERT, "ba"), new Diff(Operation.EQUAL, "c")};
      dmp.DiffCleanupMerge(diffs);
      CollectionAssert.AreEqual(new List<Diff> {new Diff(Operation.INSERT, "ab"), new Diff(Operation.EQUAL, "ac")}, diffs);

      // Slide edit right.
      diffs = new List<Diff> {new Diff(Operation.EQUAL, "c"), new Diff(Operation.INSERT, "ab"), new Diff(Operation.EQUAL, "a")};
      dmp.DiffCleanupMerge(diffs);
      CollectionAssert.AreEqual(new List<Diff> {new Diff(Operation.EQUAL, "ca"), new Diff(Operation.INSERT, "ba")}, diffs);

      // Slide edit left recursive.
      diffs = new List<Diff> {new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "b"), new Diff(Operation.EQUAL, "c"), new Diff(Operation.DELETE, "ac"), new Diff(Operation.EQUAL, "x")};
      dmp.DiffCleanupMerge(diffs);
      CollectionAssert.AreEqual(new List<Diff> {new Diff(Operation.DELETE, "abc"), new Diff(Operation.EQUAL, "acx")}, diffs);

      // Slide edit right recursive.
      diffs = new List<Diff> {new Diff(Operation.EQUAL, "x"), new Diff(Operation.DELETE, "ca"), new Diff(Operation.EQUAL, "c"), new Diff(Operation.DELETE, "b"), new Diff(Operation.EQUAL, "a")};
      dmp.DiffCleanupMerge(diffs);
      CollectionAssert.AreEqual(new List<Diff> {new Diff(Operation.EQUAL, "xca"), new Diff(Operation.DELETE, "cba")}, diffs);
    }

    [Test()]
    public void DiffCleanupSemanticLosslessTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Slide diffs to match logical boundaries.
      // Null case.
      List<Diff> diffs = new List<Diff>();
      dmp.DiffCleanupSemanticLossless(diffs);
      CollectionAssert.AreEqual(new List<Diff>(), diffs);

      // Blank lines.
      diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "AAA\r\n\r\nBBB"),
          new Diff(Operation.INSERT, "\r\nDDD\r\n\r\nBBB"),
          new Diff(Operation.EQUAL, "\r\nEEE")
      };
      dmp.DiffCleanupSemanticLossless(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.EQUAL, "AAA\r\n\r\n"),
          new Diff(Operation.INSERT, "BBB\r\nDDD\r\n\r\n"),
          new Diff(Operation.EQUAL, "BBB\r\nEEE")}, diffs);

      // Line boundaries.
      diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "AAA\r\nBBB"),
          new Diff(Operation.INSERT, " DDD\r\nBBB"),
          new Diff(Operation.EQUAL, " EEE")};
      dmp.DiffCleanupSemanticLossless(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.EQUAL, "AAA\r\n"),
          new Diff(Operation.INSERT, "BBB DDD\r\n"),
          new Diff(Operation.EQUAL, "BBB EEE")}, diffs);

      // Word boundaries.
      diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "The c"),
          new Diff(Operation.INSERT, "ow and the c"),
          new Diff(Operation.EQUAL, "at.")};
      dmp.DiffCleanupSemanticLossless(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.EQUAL, "The "),
          new Diff(Operation.INSERT, "cow and the "),
          new Diff(Operation.EQUAL, "cat.")}, diffs);

      // Alphanumeric boundaries.
      diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "The-c"),
          new Diff(Operation.INSERT, "ow-and-the-c"),
          new Diff(Operation.EQUAL, "at.")};
      dmp.DiffCleanupSemanticLossless(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.EQUAL, "The-"),
          new Diff(Operation.INSERT, "cow-and-the-"),
          new Diff(Operation.EQUAL, "cat.")}, diffs);

      // Hitting the start.
      diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "a"),
          new Diff(Operation.DELETE, "a"),
          new Diff(Operation.EQUAL, "ax")};
      dmp.DiffCleanupSemanticLossless(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "a"),
          new Diff(Operation.EQUAL, "aax")}, diffs);

      // Hitting the end.
      diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "xa"),
          new Diff(Operation.DELETE, "a"),
          new Diff(Operation.EQUAL, "a")};
      dmp.DiffCleanupSemanticLossless(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.EQUAL, "xaa"),
          new Diff(Operation.DELETE, "a")}, diffs);

      // Sentence boundaries.
      diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "The xxx. The "),
          new Diff(Operation.INSERT, "zzz. The "),
          new Diff(Operation.EQUAL, "yyy.")};
      dmp.DiffCleanupSemanticLossless(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.EQUAL, "The xxx."),
          new Diff(Operation.INSERT, " The zzz."),
          new Diff(Operation.EQUAL, " The yyy.")}, diffs);
    }

    [Test()]
    public void DiffCleanupSemanticTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Cleanup semantically trivial equalities.
      // Null case.
      List<Diff> diffs = new List<Diff>();
      dmp.DiffCleanupSemantic(diffs);
      CollectionAssert.AreEqual(new List<Diff>(), diffs);

      // No elimination #1.
      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "ab"),
          new Diff(Operation.INSERT, "cd"),
          new Diff(Operation.EQUAL, "12"),
          new Diff(Operation.DELETE, "e")};
      dmp.DiffCleanupSemantic(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "ab"),
          new Diff(Operation.INSERT, "cd"),
          new Diff(Operation.EQUAL, "12"),
          new Diff(Operation.DELETE, "e")}, diffs);

      // No elimination #2.
      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "abc"),
          new Diff(Operation.INSERT, "ABC"),
          new Diff(Operation.EQUAL, "1234"),
          new Diff(Operation.DELETE, "wxyz")};
      dmp.DiffCleanupSemantic(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "abc"),
          new Diff(Operation.INSERT, "ABC"),
          new Diff(Operation.EQUAL, "1234"),
          new Diff(Operation.DELETE, "wxyz")}, diffs);

      // Simple elimination.
      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "a"),
          new Diff(Operation.EQUAL, "b"),
          new Diff(Operation.DELETE, "c")};
      dmp.DiffCleanupSemantic(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "abc"),
          new Diff(Operation.INSERT, "b")}, diffs);

      // Backpass elimination.
      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "ab"),
          new Diff(Operation.EQUAL, "cd"),
          new Diff(Operation.DELETE, "e"),
          new Diff(Operation.EQUAL, "f"),
          new Diff(Operation.INSERT, "g")};
      dmp.DiffCleanupSemantic(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "abcdef"),
          new Diff(Operation.INSERT, "cdfg")}, diffs);

      // Multiple eliminations.
      diffs = new List<Diff> {
          new Diff(Operation.INSERT, "1"),
          new Diff(Operation.EQUAL, "A"),
          new Diff(Operation.DELETE, "B"),
          new Diff(Operation.INSERT, "2"),
          new Diff(Operation.EQUAL, "_"),
          new Diff(Operation.INSERT, "1"),
          new Diff(Operation.EQUAL, "A"),
          new Diff(Operation.DELETE, "B"),
          new Diff(Operation.INSERT, "2")};
      dmp.DiffCleanupSemantic(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "AB_AB"),
          new Diff(Operation.INSERT, "1A2_1A2")}, diffs);

      // Word boundaries.
      diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "The c"),
          new Diff(Operation.DELETE, "ow and the c"),
          new Diff(Operation.EQUAL, "at.")};
      dmp.DiffCleanupSemantic(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.EQUAL, "The "),
          new Diff(Operation.DELETE, "cow and the "),
          new Diff(Operation.EQUAL, "cat.")}, diffs);

      // No overlap elimination.
      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "abcxx"),
          new Diff(Operation.INSERT, "xxdef")};
      dmp.DiffCleanupSemantic(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "abcxx"),
          new Diff(Operation.INSERT, "xxdef")}, diffs);

      // Overlap elimination.
      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "abcxxx"),
          new Diff(Operation.INSERT, "xxxdef")};
      dmp.DiffCleanupSemantic(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "abc"),
          new Diff(Operation.EQUAL, "xxx"),
          new Diff(Operation.INSERT, "def")}, diffs);

      // Reverse overlap elimination.
      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "xxxabc"),
          new Diff(Operation.INSERT, "defxxx")};
      dmp.DiffCleanupSemantic(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.INSERT, "def"),
          new Diff(Operation.EQUAL, "xxx"),
          new Diff(Operation.DELETE, "abc")}, diffs);

      // Two overlap eliminations.
      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "abcd1212"),
          new Diff(Operation.INSERT, "1212efghi"),
          new Diff(Operation.EQUAL, "----"),
          new Diff(Operation.DELETE, "A3"),
          new Diff(Operation.INSERT, "3BC")};
      dmp.DiffCleanupSemantic(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "abcd"),
          new Diff(Operation.EQUAL, "1212"),
          new Diff(Operation.INSERT, "efghi"),
          new Diff(Operation.EQUAL, "----"),
          new Diff(Operation.DELETE, "A"),
          new Diff(Operation.EQUAL, "3"),
          new Diff(Operation.INSERT, "BC")}, diffs);
    }

    [Test()]
    public void DiffCleanupEfficiencyTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Cleanup operationally trivial equalities.
      dmp.Diff_EditCost = 4;
      // Null case.
      List<Diff> diffs = new List<Diff> ();
      dmp.DiffCleanupEfficiency(diffs);
      CollectionAssert.AreEqual(new List<Diff>(), diffs);

      // No elimination.
      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "ab"),
          new Diff(Operation.INSERT, "12"),
          new Diff(Operation.EQUAL, "wxyz"),
          new Diff(Operation.DELETE, "cd"),
          new Diff(Operation.INSERT, "34")};
      dmp.DiffCleanupEfficiency(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "ab"),
          new Diff(Operation.INSERT, "12"),
          new Diff(Operation.EQUAL, "wxyz"),
          new Diff(Operation.DELETE, "cd"),
          new Diff(Operation.INSERT, "34")}, diffs);

      // Four-edit elimination.
      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "ab"),
          new Diff(Operation.INSERT, "12"),
          new Diff(Operation.EQUAL, "xyz"),
          new Diff(Operation.DELETE, "cd"),
          new Diff(Operation.INSERT, "34")};
      dmp.DiffCleanupEfficiency(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "abxyzcd"),
          new Diff(Operation.INSERT, "12xyz34")}, diffs);

      // Three-edit elimination.
      diffs = new List<Diff> {
          new Diff(Operation.INSERT, "12"),
          new Diff(Operation.EQUAL, "x"),
          new Diff(Operation.DELETE, "cd"),
          new Diff(Operation.INSERT, "34")};
      dmp.DiffCleanupEfficiency(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "xcd"),
          new Diff(Operation.INSERT, "12x34")}, diffs);

      // Backpass elimination.
      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "ab"),
          new Diff(Operation.INSERT, "12"),
          new Diff(Operation.EQUAL, "xy"),
          new Diff(Operation.INSERT, "34"),
          new Diff(Operation.EQUAL, "z"),
          new Diff(Operation.DELETE, "cd"),
          new Diff(Operation.INSERT, "56")};
      dmp.DiffCleanupEfficiency(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "abxyzcd"),
          new Diff(Operation.INSERT, "12xy34z56")}, diffs);

      // High cost elimination.
      dmp.Diff_EditCost = 5;
      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "ab"),
          new Diff(Operation.INSERT, "12"),
          new Diff(Operation.EQUAL, "wxyz"),
          new Diff(Operation.DELETE, "cd"),
          new Diff(Operation.INSERT, "34")};
      dmp.DiffCleanupEfficiency(diffs);
      CollectionAssert.AreEqual(new List<Diff> {
          new Diff(Operation.DELETE, "abwxyzcd"),
          new Diff(Operation.INSERT, "12wxyz34")}, diffs);
      dmp.Diff_EditCost = 4;
    }

    [Test()]
    public void DiffPrettyHtmlTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Pretty print.
      List<Diff> diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "a\n"),
          new Diff(Operation.DELETE, "<B>b</B>"),
          new Diff(Operation.INSERT, "c&d")};
      Assert.AreEqual("<span>a&para;<br></span><del style=\"background:#ffe6e6;\">&lt;B&gt;b&lt;/B&gt;</del><ins style=\"background:#e6ffe6;\">c&amp;d</ins>",
          dmp.DiffPrettyHtml(diffs));
    }

    [Test()]
    public void diff_textTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Compute the source and destination texts.
      List<Diff> diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "jump"),
          new Diff(Operation.DELETE, "s"),
          new Diff(Operation.INSERT, "ed"),
          new Diff(Operation.EQUAL, " over "),
          new Diff(Operation.DELETE, "the"),
          new Diff(Operation.INSERT, "a"),
          new Diff(Operation.EQUAL, " lazy")};
      Assert.AreEqual("jumps over the lazy", dmp.DiffText1(diffs));

      Assert.AreEqual("jumped over a lazy", dmp.DiffText2(diffs));
    }

    [Test()]
    public void diff_deltaTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Convert a diff into delta string.
      List<Diff> diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "jump"),
          new Diff(Operation.DELETE, "s"),
          new Diff(Operation.INSERT, "ed"),
          new Diff(Operation.EQUAL, " over "),
          new Diff(Operation.DELETE, "the"),
          new Diff(Operation.INSERT, "a"),
          new Diff(Operation.EQUAL, " lazy"),
          new Diff(Operation.INSERT, "old dog")};
      string text1 = dmp.DiffText1(diffs);
      Assert.AreEqual("jumps over the lazy", text1);

      string delta = dmp.DiffToDelta(diffs);
      Assert.AreEqual("=4\t-1\t+ed\t=6\t-3\t+a\t=5\t+old dog", delta);

      // Convert delta string into a diff.
      CollectionAssert.AreEqual(diffs, dmp.DiffFromDelta(text1, delta));

      // Generates error (19 < 20).
      try {
        dmp.DiffFromDelta(text1 + "x", delta);
        Assert.Fail("DiffFromDelta: Too long.");
      } catch (ArgumentException) {
        // Exception expected.
      }

      // Generates error (19 > 18).
      try {
        dmp.DiffFromDelta(text1.Substring(1), delta);
        Assert.Fail("DiffFromDelta: Too short.");
      } catch (ArgumentException) {
        // Exception expected.
      }

      // Generates error (%c3%xy invalid Unicode).
      try {
        dmp.DiffFromDelta("", "+%c3%xy");
        Assert.Fail("DiffFromDelta: Invalid character.");
      } catch (ArgumentException) {
        // Exception expected.
      }

      // Test deltas with special characters.
      char zero = (char)0;
      char one = (char)1;
      char two = (char)2;
      diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "\u0680 " + zero + " \t %"),
          new Diff(Operation.DELETE, "\u0681 " + one + " \n ^"),
          new Diff(Operation.INSERT, "\u0682 " + two + " \\ |")};
      text1 = dmp.DiffText1(diffs);
      Assert.AreEqual("\u0680 " + zero + " \t %\u0681 " + one + " \n ^", text1);

      delta = dmp.DiffToDelta(diffs);
      // Lowercase, due to UrlEncode uses lower.
      Assert.AreEqual("=7\t-7\t+%da%82 %02 %5c %7c", delta, "DiffToDelta: Unicode.");

      CollectionAssert.AreEqual(diffs, dmp.DiffFromDelta(text1, delta), "DiffFromDelta: Unicode.");

      // Verify pool of unchanged characters.
      diffs = new List<Diff> {
          new Diff(Operation.INSERT, "A-Z a-z 0-9 - _ . ! ~ * ' ( ) ; / ? : @ & = + $ , # ")};
      string text2 = dmp.DiffText2(diffs);
      Assert.AreEqual("A-Z a-z 0-9 - _ . ! ~ * \' ( ) ; / ? : @ & = + $ , # ", text2, "DiffText2: Unchanged characters.");

      delta = dmp.DiffToDelta(diffs);
      Assert.AreEqual("+A-Z a-z 0-9 - _ . ! ~ * \' ( ) ; / ? : @ & = + $ , # ", delta, "DiffToDelta: Unchanged characters.");

      // Convert delta string into a diff.
      CollectionAssert.AreEqual(diffs, dmp.DiffFromDelta("", delta), "DiffFromDelta: Unchanged characters.");
    }

    [Test()]
    public void DiffXIndexTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Translate a location in text1 to text2.
      List<Diff> diffs = new List<Diff> {
          new Diff(Operation.DELETE, "a"),
          new Diff(Operation.INSERT, "1234"),
          new Diff(Operation.EQUAL, "xyz")};
      Assert.AreEqual(5, dmp.DiffXIndex(diffs, 2), "DiffXIndex: Translation on equality.");

      diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "a"),
          new Diff(Operation.DELETE, "1234"),
          new Diff(Operation.EQUAL, "xyz")};
      Assert.AreEqual(1, dmp.DiffXIndex(diffs, 3), "DiffXIndex: Translation on deletion.");
    }

    [Test()]
    public void DiffLevenshteinTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      List<Diff> diffs = new List<Diff> {
          new Diff(Operation.DELETE, "abc"),
          new Diff(Operation.INSERT, "1234"),
          new Diff(Operation.EQUAL, "xyz")};
      Assert.AreEqual(4, dmp.DiffLevenshtein(diffs), "DiffLevenshtein: Levenshtein with trailing equality.");

      diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "xyz"),
          new Diff(Operation.DELETE, "abc"),
          new Diff(Operation.INSERT, "1234")};
      Assert.AreEqual(4, dmp.DiffLevenshtein(diffs), "DiffLevenshtein: Levenshtein with leading equality.");

      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "abc"),
          new Diff(Operation.EQUAL, "xyz"),
          new Diff(Operation.INSERT, "1234")};
      Assert.AreEqual(7, dmp.DiffLevenshtein(diffs), "DiffLevenshtein: Levenshtein with middle equality.");
    }

    [Test()]
    public void DiffBisectTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Normal.
      string a = "cat";
      string b = "map";
      // Since the resulting diff hasn't been normalized, it would be ok if
      // the insertion and deletion pairs are swapped.
      // If the order changes, tweak this test as required.
      List<Diff> diffs = new List<Diff> {new Diff(Operation.DELETE, "c"), new Diff(Operation.INSERT, "m"), new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "t"), new Diff(Operation.INSERT, "p")};
      CollectionAssert.AreEqual(diffs, dmp.DiffBisect(a, b, DateTime.MaxValue));

      // Timeout.
      diffs = new List<Diff> {new Diff(Operation.DELETE, "cat"), new Diff(Operation.INSERT, "map")};
      CollectionAssert.AreEqual(diffs, dmp.DiffBisect(a, b, DateTime.MinValue));
    }

    [Test()]
    public void DiffMainTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Perform a trivial diff.
      List<Diff> diffs = new List<Diff> {};
      CollectionAssert.AreEqual(diffs, dmp.DiffMain("", "", false), "DiffMain: Null case.");

      diffs = new List<Diff> {new Diff(Operation.EQUAL, "abc")};
      CollectionAssert.AreEqual(diffs, dmp.DiffMain("abc", "abc", false), "DiffMain: Equality.");

      diffs = new List<Diff> {new Diff(Operation.EQUAL, "ab"), new Diff(Operation.INSERT, "123"), new Diff(Operation.EQUAL, "c")};
      CollectionAssert.AreEqual(diffs, dmp.DiffMain("abc", "ab123c", false), "DiffMain: Simple insertion.");

      diffs = new List<Diff> {new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "123"), new Diff(Operation.EQUAL, "bc")};
      CollectionAssert.AreEqual(diffs, dmp.DiffMain("a123bc", "abc", false), "DiffMain: Simple deletion.");

      diffs = new List<Diff> {new Diff(Operation.EQUAL, "a"), new Diff(Operation.INSERT, "123"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.INSERT, "456"), new Diff(Operation.EQUAL, "c")};
      CollectionAssert.AreEqual(diffs, dmp.DiffMain("abc", "a123b456c", false), "DiffMain: Two insertions.");

      diffs = new List<Diff> {new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "123"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.DELETE, "456"), new Diff(Operation.EQUAL, "c")};
      CollectionAssert.AreEqual(diffs, dmp.DiffMain("a123b456c", "abc", false), "DiffMain: Two deletions.");

      // Perform a real diff.
      // Switch off the timeout.
      dmp.Diff_Timeout = 0;
      diffs = new List<Diff> {new Diff(Operation.DELETE, "a"), new Diff(Operation.INSERT, "b")};
      CollectionAssert.AreEqual(diffs, dmp.DiffMain("a", "b", false), "DiffMain: Simple case #1.");

      diffs = new List<Diff> {new Diff(Operation.DELETE, "Apple"), new Diff(Operation.INSERT, "Banana"), new Diff(Operation.EQUAL, "s are a"), new Diff(Operation.INSERT, "lso"), new Diff(Operation.EQUAL, " fruit.")};
      CollectionAssert.AreEqual(diffs, dmp.DiffMain("Apples are a fruit.", "Bananas are also fruit.", false), "DiffMain: Simple case #2.");

      diffs = new List<Diff> {new Diff(Operation.DELETE, "a"), new Diff(Operation.INSERT, "\u0680"), new Diff(Operation.EQUAL, "x"), new Diff(Operation.DELETE, "\t"), new Diff(Operation.INSERT, new string (new char[]{(char)0}))};
      CollectionAssert.AreEqual(diffs, dmp.DiffMain("ax\t", "\u0680x" + (char)0, false), "DiffMain: Simple case #3.");

      diffs = new List<Diff> {new Diff(Operation.DELETE, "1"), new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "y"), new Diff(Operation.EQUAL, "b"), new Diff(Operation.DELETE, "2"), new Diff(Operation.INSERT, "xab")};
      CollectionAssert.AreEqual(diffs, dmp.DiffMain("1ayb2", "abxab", false), "DiffMain: Overlap #1.");

      diffs = new List<Diff> {new Diff(Operation.INSERT, "xaxcx"), new Diff(Operation.EQUAL, "abc"), new Diff(Operation.DELETE, "y")};
      CollectionAssert.AreEqual(diffs, dmp.DiffMain("abcy", "xaxcxabc", false), "DiffMain: Overlap #2.");

      diffs = new List<Diff> {new Diff(Operation.DELETE, "ABCD"), new Diff(Operation.EQUAL, "a"), new Diff(Operation.DELETE, "="), new Diff(Operation.INSERT, "-"), new Diff(Operation.EQUAL, "bcd"), new Diff(Operation.DELETE, "="), new Diff(Operation.INSERT, "-"), new Diff(Operation.EQUAL, "efghijklmnopqrs"), new Diff(Operation.DELETE, "EFGHIJKLMNOefg")};
      CollectionAssert.AreEqual(diffs, dmp.DiffMain("ABCDa=bcd=efghijklmnopqrsEFGHIJKLMNOefg", "a-bcd-efghijklmnopqrs", false), "DiffMain: Overlap #3.");

      diffs = new List<Diff> {new Diff(Operation.INSERT, " "), new Diff(Operation.EQUAL, "a"), new Diff(Operation.INSERT, "nd"), new Diff(Operation.EQUAL, " [[Pennsylvania]]"), new Diff(Operation.DELETE, " and [[New")};
      CollectionAssert.AreEqual(diffs, dmp.DiffMain("a [[Pennsylvania]] and [[New", " and [[Pennsylvania]]", false), "DiffMain: Large equality.");

      dmp.Diff_Timeout = 0.1f;  // 100ms
      string a = "`Twas brillig, and the slithy toves\nDid gyre and gimble in the wabe:\nAll mimsy were the borogoves,\nAnd the mome raths outgrabe.\n";
      string b = "I am the very model of a modern major general,\nI've information vegetable, animal, and mineral,\nI know the kings of England, and I quote the fights historical,\nFrom Marathon to Waterloo, in order categorical.\n";
      // Increase the text lengths by 1024 times to ensure a timeout.
      for (int x = 0; x < 10; x++) {
        a = a + a;
        b = b + b;
      }
      DateTime startTime = DateTime.Now;
      dmp.DiffMain(a, b);
      DateTime endTime = DateTime.Now;
      // Test that we took at least the timeout period.
      Assert.IsTrue(new TimeSpan(((long)(dmp.Diff_Timeout * 1000)) * 10000) <= endTime - startTime);
      // Test that we didn't take forever (be forgiving).
      // Theoretically this test could fail very occasionally if the
      // OS task swaps or locks up for a second at the wrong moment.
      Assert.IsTrue(new TimeSpan(((long)(dmp.Diff_Timeout * 1000)) * 10000 * 2) > endTime - startTime);
      dmp.Diff_Timeout = 0;

      // Test the linemode speedup.
      // Must be long to pass the 100 char cutoff.
      a = "1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n";
      b = "abcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\nabcdefghij\n";
      CollectionAssert.AreEqual(dmp.DiffMain(a, b, true), dmp.DiffMain(a, b, false), "DiffMain: Simple line-mode.");

      a = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";
      b = "abcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghij";
      CollectionAssert.AreEqual(dmp.DiffMain(a, b, true), dmp.DiffMain(a, b, false), "DiffMain: Single line-mode.");

      a = "1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n1234567890\n";
      b = "abcdefghij\n1234567890\n1234567890\n1234567890\nabcdefghij\n1234567890\n1234567890\n1234567890\nabcdefghij\n1234567890\n1234567890\n1234567890\nabcdefghij\n";
      string[] texts_linemode = diff_rebuildtexts(dmp.DiffMain(a, b, true));
      string[] texts_textmode = diff_rebuildtexts(dmp.DiffMain(a, b, false));
      CollectionAssert.AreEqual(texts_textmode, texts_linemode, "DiffMain: Overlap line-mode.");

      // Test null inputs -- not needed because nulls can't be passed in C#.
    }

    [Test()]
    public void MatchAlphabetTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Initialise the bitmasks for Bitap.
      Dictionary<char, int> bitmask = new Dictionary<char, int>();
      bitmask.Add('a', 4); bitmask.Add('b', 2); bitmask.Add('c', 1);
      CollectionAssert.AreEqual(bitmask, dmp.MatchAlphabet("abc"), "MatchAlphabet: Unique.");

      bitmask.Clear();
      bitmask.Add('a', 37); bitmask.Add('b', 18); bitmask.Add('c', 8);
      CollectionAssert.AreEqual(bitmask, dmp.MatchAlphabet("abcaba"), "MatchAlphabet: Duplicates.");
    }

    [Test()]
    public void MatchBitapTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();

      // Bitap algorithm.
      dmp.Match_Distance = 100;
      dmp.Match_Threshold = 0.5f;
      Assert.AreEqual(5, dmp.MatchBitap("abcdefghijk", "fgh", 5), "MatchBitap: Exact match #1.");

      Assert.AreEqual(5, dmp.MatchBitap("abcdefghijk", "fgh", 0), "MatchBitap: Exact match #2.");

      Assert.AreEqual(4, dmp.MatchBitap("abcdefghijk", "efxhi", 0), "MatchBitap: Fuzzy match #1.");

      Assert.AreEqual(2, dmp.MatchBitap("abcdefghijk", "cdefxyhijk", 5), "MatchBitap: Fuzzy match #2.");

      Assert.AreEqual(-1, dmp.MatchBitap("abcdefghijk", "bxy", 1), "MatchBitap: Fuzzy match #3.");

      Assert.AreEqual(2, dmp.MatchBitap("123456789xx0", "3456789x0", 2), "MatchBitap: Overflow.");

      Assert.AreEqual(0, dmp.MatchBitap("abcdef", "xxabc", 4), "MatchBitap: Before start match.");

      Assert.AreEqual(3, dmp.MatchBitap("abcdef", "defyy", 4), "MatchBitap: Beyond end match.");

      Assert.AreEqual(0, dmp.MatchBitap("abcdef", "xabcdefy", 0), "MatchBitap: Oversized pattern.");

      dmp.Match_Threshold = 0.4f;
      Assert.AreEqual(4, dmp.MatchBitap("abcdefghijk", "efxyhi", 1), "MatchBitap: Threshold #1.");

      dmp.Match_Threshold = 0.3f;
      Assert.AreEqual(-1, dmp.MatchBitap("abcdefghijk", "efxyhi", 1), "MatchBitap: Threshold #2.");

      dmp.Match_Threshold = 0.0f;
      Assert.AreEqual(1, dmp.MatchBitap("abcdefghijk", "bcdef", 1), "MatchBitap: Threshold #3.");

      dmp.Match_Threshold = 0.5f;
      Assert.AreEqual(0, dmp.MatchBitap("abcdexyzabcde", "abccde", 3), "MatchBitap: Multiple select #1.");

      Assert.AreEqual(8, dmp.MatchBitap("abcdexyzabcde", "abccde", 5), "MatchBitap: Multiple select #2.");

      dmp.Match_Distance = 10;  // Strict location.
      Assert.AreEqual(-1, dmp.MatchBitap("abcdefghijklmnopqrstuvwxyz", "abcdefg", 24), "MatchBitap: Distance test #1.");

      Assert.AreEqual(0, dmp.MatchBitap("abcdefghijklmnopqrstuvwxyz", "abcdxxefg", 1), "MatchBitap: Distance test #2.");

      dmp.Match_Distance = 1000;  // Loose location.
      Assert.AreEqual(0, dmp.MatchBitap("abcdefghijklmnopqrstuvwxyz", "abcdefg", 24), "MatchBitap: Distance test #3.");
    }

    [Test()]
    public void MatchMainTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      // Full match.
      Assert.AreEqual(0, dmp.MatchMain("abcdef", "abcdef", 1000), "MatchMain: Equality.");

      Assert.AreEqual(-1, dmp.MatchMain("", "abcdef", 1), "MatchMain: Null text.");

      Assert.AreEqual(3, dmp.MatchMain("abcdef", "", 3), "MatchMain: Null pattern.");

      Assert.AreEqual(3, dmp.MatchMain("abcdef", "de", 3), "MatchMain: Exact match.");

      Assert.AreEqual(3, dmp.MatchMain("abcdef", "defy", 4), "MatchMain: Beyond end match.");

      Assert.AreEqual(0, dmp.MatchMain("abcdef", "abcdefy", 0), "MatchMain: Oversized pattern.");

      dmp.Match_Threshold = 0.7f;
      Assert.AreEqual(4, dmp.MatchMain("I am the very model of a modern major general.", " that berry ", 5), "MatchMain: Complex match.");
      dmp.Match_Threshold = 0.5f;

      // Test null inputs -- not needed because nulls can't be passed in C#.
    }

    [Test()]
    public void patch_patchObjTest() {
      // Patch Object.
      Patch p = new Patch();
      p.start1 = 20;
      p.start2 = 21;
      p.length1 = 18;
      p.length2 = 17;
      p.diffs = new List<Diff> {
          new Diff(Operation.EQUAL, "jump"),
          new Diff(Operation.DELETE, "s"),
          new Diff(Operation.INSERT, "ed"),
          new Diff(Operation.EQUAL, " over "),
          new Diff(Operation.DELETE, "the"),
          new Diff(Operation.INSERT, "a"),
          new Diff(Operation.EQUAL, "\nlaz")};
      string strp = "@@ -21,18 +22,17 @@\n jump\n-s\n+ed\n  over \n-the\n+a\n %0alaz\n";
      Assert.AreEqual(strp, p.ToString(), "Patch: toString.");
    }

    [Test()]
    public void PatchFromTextTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      Assert.IsTrue(dmp.PatchFromText("").Count == 0, "PatchFromText: #0.");

      string strp = "@@ -21,18 +22,17 @@\n jump\n-s\n+ed\n  over \n-the\n+a\n %0alaz\n";
      Assert.AreEqual(strp, dmp.PatchFromText(strp)[0].ToString(), "PatchFromText: #1.");

      Assert.AreEqual("@@ -1 +1 @@\n-a\n+b\n", dmp.PatchFromText("@@ -1 +1 @@\n-a\n+b\n")[0].ToString(), "PatchFromText: #2.");

      Assert.AreEqual("@@ -1,3 +0,0 @@\n-abc\n", dmp.PatchFromText("@@ -1,3 +0,0 @@\n-abc\n") [0].ToString(), "PatchFromText: #3.");

      Assert.AreEqual("@@ -0,0 +1,3 @@\n+abc\n", dmp.PatchFromText("@@ -0,0 +1,3 @@\n+abc\n") [0].ToString(), "PatchFromText: #4.");

      // Generates error.
      try {
        dmp.PatchFromText("Bad\nPatch\n");
        Assert.Fail("PatchFromText: #5.");
      } catch (ArgumentException) {
        // Exception expected.
      }
    }

    [Test()]
    public void PatchToTextTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      string strp = "@@ -21,18 +22,17 @@\n jump\n-s\n+ed\n  over \n-the\n+a\n  laz\n";
      List<Patch> patches;
      patches = dmp.PatchFromText(strp);
      string result = dmp.PatchToText(patches);
      Assert.AreEqual(strp, result);

      strp = "@@ -1,9 +1,9 @@\n-f\n+F\n oo+fooba\n@@ -7,9 +7,9 @@\n obar\n-,\n+.\n  tes\n";
      patches = dmp.PatchFromText(strp);
      result = dmp.PatchToText(patches);
      Assert.AreEqual(strp, result);
    }

    [Test()]
    public void PatchAddContextTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      dmp.Patch_Margin = 4;
      Patch p;
      p = dmp.PatchFromText("@@ -21,4 +21,10 @@\n-jump\n+somersault\n") [0];
      dmp.PatchAddContext(p, "The quick brown fox jumps over the lazy dog.");
      Assert.AreEqual("@@ -17,12 +17,18 @@\n fox \n-jump\n+somersault\n s ov\n", p.ToString(), "PatchAddContext: Simple case.");

      p = dmp.PatchFromText("@@ -21,4 +21,10 @@\n-jump\n+somersault\n")[0];
      dmp.PatchAddContext(p, "The quick brown fox jumps.");
      Assert.AreEqual("@@ -17,10 +17,16 @@\n fox \n-jump\n+somersault\n s.\n", p.ToString(), "PatchAddContext: Not enough trailing context.");

      p = dmp.PatchFromText("@@ -3 +3,2 @@\n-e\n+at\n")[0];
      dmp.PatchAddContext(p, "The quick brown fox jumps.");
      Assert.AreEqual("@@ -1,7 +1,8 @@\n Th\n-e\n+at\n  qui\n", p.ToString(), "PatchAddContext: Not enough leading context.");

      p = dmp.PatchFromText("@@ -3 +3,2 @@\n-e\n+at\n")[0];
      dmp.PatchAddContext(p, "The quick brown fox jumps.  The quick brown fox crashes.");
      Assert.AreEqual("@@ -1,27 +1,28 @@\n Th\n-e\n+at\n  quick brown fox jumps. \n", p.ToString(), "PatchAddContext: Ambiguity.");
    }

    [Test()]
    public void PatchMakeTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      List<Patch> patches;
      patches = dmp.PatchMake("", "");
      Assert.AreEqual("", dmp.PatchToText(patches), "PatchMake: Null case.");

      string text1 = "The quick brown fox jumps over the lazy dog.";
      string text2 = "That quick brown fox jumped over a lazy dog.";
      string expectedPatch = "@@ -1,8 +1,7 @@\n Th\n-at\n+e\n  qui\n@@ -21,17 +21,18 @@\n jump\n-ed\n+s\n  over \n-a\n+the\n  laz\n";
      // The second patch must be "-21,17 +21,18", not "-22,17 +21,18" due to rolling context.
      patches = dmp.PatchMake(text2, text1);
      Assert.AreEqual(expectedPatch, dmp.PatchToText(patches), "PatchMake: Text2+Text1 inputs.");

      expectedPatch = "@@ -1,11 +1,12 @@\n Th\n-e\n+at\n  quick b\n@@ -22,18 +22,17 @@\n jump\n-s\n+ed\n  over \n-the\n+a\n  laz\n";
      patches = dmp.PatchMake(text1, text2);
      Assert.AreEqual(expectedPatch, dmp.PatchToText(patches), "PatchMake: Text1+Text2 inputs.");

      List<Diff> diffs = dmp.DiffMain(text1, text2, false);
      patches = dmp.PatchMake(diffs);
      Assert.AreEqual(expectedPatch, dmp.PatchToText(patches), "PatchMake: Diff input.");

      patches = dmp.PatchMake(text1, diffs);
      Assert.AreEqual(expectedPatch, dmp.PatchToText(patches), "PatchMake: Text1+Diff inputs.");

      patches = dmp.PatchMake(text1, text2, diffs);
      Assert.AreEqual(expectedPatch, dmp.PatchToText(patches), "PatchMake: Text1+Text2+Diff inputs (deprecated).");

      patches = dmp.PatchMake("`1234567890-=[]\\;',./", "~!@#$%^&*()_+{}|:\"<>?");
      Assert.AreEqual("@@ -1,21 +1,21 @@\n-%601234567890-=%5b%5d%5c;',./\n+~!@#$%25%5e&*()_+%7b%7d%7c:%22%3c%3e?\n",
          dmp.PatchToText(patches),
          "PatchToText: Character encoding.");

      diffs = new List<Diff> {
          new Diff(Operation.DELETE, "`1234567890-=[]\\;',./"),
          new Diff(Operation.INSERT, "~!@#$%^&*()_+{}|:\"<>?")};
      CollectionAssert.AreEqual(diffs,
          dmp.PatchFromText("@@ -1,21 +1,21 @@\n-%601234567890-=%5B%5D%5C;',./\n+~!@#$%25%5E&*()_+%7B%7D%7C:%22%3C%3E?\n") [0].diffs,
          "PatchFromText: Character decoding.");

      text1 = "";
      for (int x = 0; x < 100; x++) {
        text1 += "abcdef";
      }
      text2 = text1 + "123";
      expectedPatch = "@@ -573,28 +573,31 @@\n cdefabcdefabcdefabcdefabcdef\n+123\n";
      patches = dmp.PatchMake(text1, text2);
      Assert.AreEqual(expectedPatch, dmp.PatchToText(patches), "PatchMake: Long string with repeats.");

      // Test null inputs -- not needed because nulls can't be passed in C#.
    }

    [Test()]
    public void PatchSplitMaxTest() {
      // Assumes that Match_MaxBits is 32.
      diff_match_patchTest dmp = new diff_match_patchTest();
      List<Patch> patches;

      patches = dmp.PatchMake("abcdefghijklmnopqrstuvwxyz01234567890", "XabXcdXefXghXijXklXmnXopXqrXstXuvXwxXyzX01X23X45X67X89X0");
      dmp.PatchSplitMax(patches);
      Assert.AreEqual("@@ -1,32 +1,46 @@\n+X\n ab\n+X\n cd\n+X\n ef\n+X\n gh\n+X\n ij\n+X\n kl\n+X\n mn\n+X\n op\n+X\n qr\n+X\n st\n+X\n uv\n+X\n wx\n+X\n yz\n+X\n 012345\n@@ -25,13 +39,18 @@\n zX01\n+X\n 23\n+X\n 45\n+X\n 67\n+X\n 89\n+X\n 0\n", dmp.PatchToText(patches));

      patches = dmp.PatchMake("abcdef1234567890123456789012345678901234567890123456789012345678901234567890uvwxyz", "abcdefuvwxyz");
      string oldToText = dmp.PatchToText(patches);
      dmp.PatchSplitMax(patches);
      Assert.AreEqual(oldToText, dmp.PatchToText(patches));

      patches = dmp.PatchMake("1234567890123456789012345678901234567890123456789012345678901234567890", "abc");
      dmp.PatchSplitMax(patches);
      Assert.AreEqual("@@ -1,32 +1,4 @@\n-1234567890123456789012345678\n 9012\n@@ -29,32 +1,4 @@\n-9012345678901234567890123456\n 7890\n@@ -57,14 +1,3 @@\n-78901234567890\n+abc\n", dmp.PatchToText(patches));

      patches = dmp.PatchMake("abcdefghij , h : 0 , t : 1 abcdefghij , h : 0 , t : 1 abcdefghij , h : 0 , t : 1", "abcdefghij , h : 1 , t : 1 abcdefghij , h : 1 , t : 1 abcdefghij , h : 0 , t : 1");
      dmp.PatchSplitMax(patches);
      Assert.AreEqual("@@ -2,32 +2,32 @@\n bcdefghij , h : \n-0\n+1\n  , t : 1 abcdef\n@@ -29,32 +29,32 @@\n bcdefghij , h : \n-0\n+1\n  , t : 1 abcdef\n", dmp.PatchToText(patches));
    }

    [Test()]
    public void PatchAddPaddingTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      List<Patch> patches;
      patches = dmp.PatchMake("", "test");
      Assert.AreEqual("@@ -0,0 +1,4 @@\n+test\n",
         dmp.PatchToText(patches),
         "PatchAddPadding: Both edges full.");
      dmp.PatchAddPadding(patches);
      Assert.AreEqual("@@ -1,8 +1,12 @@\n %01%02%03%04\n+test\n %01%02%03%04\n",
          dmp.PatchToText(patches),
          "PatchAddPadding: Both edges full.");

      patches = dmp.PatchMake("XY", "XtestY");
      Assert.AreEqual("@@ -1,2 +1,6 @@\n X\n+test\n Y\n",
          dmp.PatchToText(patches),
          "PatchAddPadding: Both edges partial.");
      dmp.PatchAddPadding(patches);
      Assert.AreEqual("@@ -2,8 +2,12 @@\n %02%03%04X\n+test\n Y%01%02%03\n",
          dmp.PatchToText(patches),
          "PatchAddPadding: Both edges partial.");

      patches = dmp.PatchMake("XXXXYYYY", "XXXXtestYYYY");
      Assert.AreEqual("@@ -1,8 +1,12 @@\n XXXX\n+test\n YYYY\n",
          dmp.PatchToText(patches),
          "PatchAddPadding: Both edges none.");
      dmp.PatchAddPadding(patches);
      Assert.AreEqual("@@ -5,8 +5,12 @@\n XXXX\n+test\n YYYY\n",
         dmp.PatchToText(patches),
         "PatchAddPadding: Both edges none.");
    }

    [Test()]
    public void PatchApplyTest() {
      diff_match_patchTest dmp = new diff_match_patchTest();
      dmp.Match_Distance = 1000;
      dmp.Match_Threshold = 0.5f;
      dmp.Patch_DeleteThreshold = 0.5f;
      List<Patch> patches;
      patches = dmp.PatchMake("", "");
      Object[] results = dmp.PatchApply(patches, "Hello world.");
      bool[] boolArray = (bool[])results[1];
      string resultStr = results[0] + "\t" + boolArray.Length;
      Assert.AreEqual("Hello world.\t0", resultStr, "PatchApply: Null case.");

      patches = dmp.PatchMake("The quick brown fox jumps over the lazy dog.", "That quick brown fox jumped over a lazy dog.");
      results = dmp.PatchApply(patches, "The quick brown fox jumps over the lazy dog.");
      boolArray = (bool[])results[1];
      resultStr = results[0] + "\t" + boolArray[0] + "\t" + boolArray[1];
      Assert.AreEqual("That quick brown fox jumped over a lazy dog.\tTrue\tTrue", resultStr, "PatchApply: Exact match.");

      results = dmp.PatchApply(patches, "The quick red rabbit jumps over the tired tiger.");
      boolArray = (bool[])results[1];
      resultStr = results[0] + "\t" + boolArray[0] + "\t" + boolArray[1];
      Assert.AreEqual("That quick red rabbit jumped over a tired tiger.\tTrue\tTrue", resultStr, "PatchApply: Partial match.");

      results = dmp.PatchApply(patches, "I am the very model of a modern major general.");
      boolArray = (bool[])results[1];
      resultStr = results[0] + "\t" + boolArray[0] + "\t" + boolArray[1];
      Assert.AreEqual("I am the very model of a modern major general.\tFalse\tFalse", resultStr, "PatchApply: Failed match.");

      patches = dmp.PatchMake("x1234567890123456789012345678901234567890123456789012345678901234567890y", "xabcy");
      results = dmp.PatchApply(patches, "x123456789012345678901234567890-----++++++++++-----123456789012345678901234567890y");
      boolArray = (bool[])results[1];
      resultStr = results[0] + "\t" + boolArray[0] + "\t" + boolArray[1];
      Assert.AreEqual("xabcy\tTrue\tTrue", resultStr, "PatchApply: Big delete, small change.");

      patches = dmp.PatchMake("x1234567890123456789012345678901234567890123456789012345678901234567890y", "xabcy");
      results = dmp.PatchApply(patches, "x12345678901234567890---------------++++++++++---------------12345678901234567890y");
      boolArray = (bool[])results[1];
      resultStr = results[0] + "\t" + boolArray[0] + "\t" + boolArray[1];
      Assert.AreEqual("xabc12345678901234567890---------------++++++++++---------------12345678901234567890y\tFalse\tTrue", resultStr, "PatchApply: Big delete, big change 1.");

      dmp.Patch_DeleteThreshold = 0.6f;
      patches = dmp.PatchMake("x1234567890123456789012345678901234567890123456789012345678901234567890y", "xabcy");
      results = dmp.PatchApply(patches, "x12345678901234567890---------------++++++++++---------------12345678901234567890y");
      boolArray = (bool[])results[1];
      resultStr = results[0] + "\t" + boolArray[0] + "\t" + boolArray[1];
      Assert.AreEqual("xabcy\tTrue\tTrue", resultStr, "PatchApply: Big delete, big change 2.");
      dmp.Patch_DeleteThreshold = 0.5f;

      dmp.Match_Threshold = 0.0f;
      dmp.Match_Distance = 0;
      patches = dmp.PatchMake("abcdefghijklmnopqrstuvwxyz--------------------1234567890", "abcXXXXXXXXXXdefghijklmnopqrstuvwxyz--------------------1234567YYYYYYYYYY890");
      results = dmp.PatchApply(patches, "ABCDEFGHIJKLMNOPQRSTUVWXYZ--------------------1234567890");
      boolArray = (bool[])results[1];
      resultStr = results[0] + "\t" + boolArray[0] + "\t" + boolArray[1];
      Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZ--------------------1234567YYYYYYYYYY890\tFalse\tTrue", resultStr, "PatchApply: Compensate for failed patch.");
      dmp.Match_Threshold = 0.5f;
      dmp.Match_Distance = 1000;

      patches = dmp.PatchMake("", "test");
      string patchStr = dmp.PatchToText(patches);
      dmp.PatchApply(patches, "");
      Assert.AreEqual(patchStr, dmp.PatchToText(patches), "PatchApply: No side effects.");

      patches = dmp.PatchMake("The quick brown fox jumps over the lazy dog.", "Woof");
      patchStr = dmp.PatchToText(patches);
      dmp.PatchApply(patches, "The quick brown fox jumps over the lazy dog.");
      Assert.AreEqual(patchStr, dmp.PatchToText(patches), "PatchApply: No side effects with major delete.");

      patches = dmp.PatchMake("", "test");
      results = dmp.PatchApply(patches, "");
      boolArray = (bool[])results[1];
      resultStr = results[0] + "\t" + boolArray[0];
      Assert.AreEqual("test\tTrue", resultStr, "PatchApply: Edge exact match.");

      patches = dmp.PatchMake("XY", "XtestY");
      results = dmp.PatchApply(patches, "XY");
      boolArray = (bool[])results[1];
      resultStr = results[0] + "\t" + boolArray[0];
      Assert.AreEqual("XtestY\tTrue", resultStr, "PatchApply: Near edge exact match.");

      patches = dmp.PatchMake("y", "y123");
      results = dmp.PatchApply(patches, "x");
      boolArray = (bool[])results[1];
      resultStr = results[0] + "\t" + boolArray[0];
      Assert.AreEqual("x123\tTrue", resultStr, "PatchApply: Edge partial match.");
    }

    private static string[] diff_rebuildtexts(List<Diff> diffs) {
      string[] text = { "", "" };
      foreach (Diff myDiff in diffs) {
        if (myDiff.operation != Operation.INSERT) {
          text[0] += myDiff.text;
        }
        if (myDiff.operation != Operation.DELETE) {
          text[1] += myDiff.text;
        }
      }
      return text;
    }
  }
}
