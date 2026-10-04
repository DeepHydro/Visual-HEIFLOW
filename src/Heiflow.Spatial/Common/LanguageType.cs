//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

namespace Heiflow.Spatial
{
   using System.ComponentModel;

   /// <summary>
   /// This enum contains all possible languages for the Google maps. 
   /// You can find latest information about supported languages in the:
   /// http://tinyurl.com/yh4va36 <- http://spreadsheets.server.com/pub?key=p9pdwsai2hDMsLkXsoM05KQ&gid=1
   /// </summary>
   public enum LanguageType
   {
      [Description("ar")]
      Arabic,

      [Description("bg")]
      Bulgarian,

      [Description("bn")]
      Bengali,

      [Description("ca")]
      Catalan,

      [Description("cs")]
      Czech,

      [Description("da")]
      Danish,

      [Description("de")]
      German,

      [Description("el")]
      Greek,

      [Description("en")]
      English,

      [Description("en-AU")]
      EnglishAustralian,

      [Description("en-GB")]
      EnglishGreatBritain,

      [Description("es")]
      Spanish,

      [Description("eu")]
      Basque,

      [Description("fa")]
      FARSI,

      [Description("fi")]
      Finnish,

      [Description("fil")]
      Filipino,

      [Description("fr")]
      French,

      [Description("gl")]
      Galician,

      [Description("gu")]
      Gujarati,
      [Description("hi")]
      Hindi,

      [Description("hr")]
      Croatian,

      [Description("hu")]
      Hungarian,

      [Description("id")]
      Indonesian,

      [Description("it")]
      Italian,

      [Description("iw")]
      Hebrew,

      [Description("ja")]
      Japanese,

      [Description("kn")]
      Kannada,

      [Description("ko")]
      Korean,

      [Description("lt")]
      Lithuanian,

      [Description("lv")]
      Latvian,

      [Description("ml")]
      Malayalam,

      [Description("mr")]
      Marathi,

      [Description("nl")]
      Dutch,

      [Description("nn")]
      NorwegianNynorsk,

      [Description("no")]
      Norwegian,

      [Description("or")]
      Oriya,

      [Description("pl")]
      Polish,

      [Description("pt")]
      Portuguese,

      [Description("pt-BR")]
      PortugueseBrazil,

      [Description("pt-PT")]
      PortuguesePortugal,

      [Description("rm")]
      Romansch,
      [Description("ro")]
      Romanian,

      [Description("ru")]
      Russian,

      [Description("sk")]
      Slovak,

      [Description("sl")]
      Slovenian,

      [Description("sr")]
      Serbian,

      [Description("sv")]
      Swedish,

      [Description("tl")]
      TAGALOG,

      [Description("ta")]
      Tamil,

      [Description("te")]
      Telugu,

      [Description("th")]
      Thai,

      [Description("tr")]
      Turkish,

      [Description("uk")]
      Ukrainian,

      [Description("vi")]
      Vietnamese,

      [Description("zh-CN")]
      ChineseSimplified,

      [Description("zh-TW")]
      ChineseTraditional,
   }
}
