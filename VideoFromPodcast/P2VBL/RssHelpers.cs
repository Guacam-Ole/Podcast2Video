using Newtonsoft.Json;

using P2VEntities;
//using P2VEntities.Config;
//using P2VEntities.Config;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace P2VBL
{
    public static class RssHelpers
    {
        public static TimeSpan ToTimeSpan(this string offset)
        {
            int ms = 0;
            var offsetSplit = offset.Split(":");
            if (offsetSplit.Length < 3) throw new Exception($"Cannot parse '{offset}'");

            bool parseSuccess=int.TryParse(offsetSplit[0], out int hour);
            parseSuccess &= int.TryParse(offsetSplit[1], out int minute);

            var secondSplit = offsetSplit[2].Split(".");
            parseSuccess &= int.TryParse(secondSplit[0], out int second);
            if (secondSplit.Length>1)
            {
                parseSuccess &= int.TryParse(secondSplit[1], out ms);
            }

            if (!parseSuccess) throw new Exception($"Cannot parse '{offset}'");
            return new TimeSpan(0, hour, minute, second, ms);
        }

        public static T Clone <T>(this T original)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(original));
        }

        public static string ToFilename(this string potentialFilename)
        {
            return string.Join("_", potentialFilename.Split(Path.GetInvalidFileNameChars()));
        }

        public static Chapter GetChapterAt(this Episode episode,  TimeSpan position)
        {
            return episode.Chapters.OrderByDescending(q => q.Offset).FirstOrDefault(q => q.Offset <= position);
        }

        public static Chapter GetNextChapter(this Episode episode, Chapter currentchapter)
        {
            int chapterIndex = episode.Chapters.IndexOf(currentchapter);
            if (chapterIndex < 0 || chapterIndex > episode.Chapters.Count - 1) return null;
            return episode.Chapters.ElementAt(chapterIndex + 1);
        }

        public static Rectangle ToRectangle(this P2VEntities.Config.BlockElement blockelement)
        {
            return new Rectangle(blockelement.X, blockelement.Y, blockelement.Width, blockelement.Height);
        }
    }
}
