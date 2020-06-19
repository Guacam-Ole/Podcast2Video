using P2VEntities;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace P2VBL
{
    public class Image
    {
        public Episode CurrentEpisode { get; set; }
        public Podcast Podcast { get; set; }
        private System.Drawing.Image _podcastImage;
        private Brush _backgroundBrush = Brushes.White;
        private Brush _timelineBrush = Brushes.LightGray;
        private Brush _timelineCurrentBrush = Brushes.DarkOrange;
        private Brush _textBrush = Brushes.Black;
        private Bitmap _background = null;
        private Brush _currentChapterBrush = Brushes.Black;
        private Brush _otherChapterBrush = Brushes.DarkGray;

        private int _chapterX = 420;
        private const int _chapterY = 140;
        private const int _otherChaptersDistance = 15;
        private const string FontName = "Tahoma";
        private const int FontSizeTitle = 12;
        private const int FontSizeEpisode = 10;
        private const int FontSizeChapter = 10;
        private const int FontSizeOtherChapters = 9;

        public const int Width = 640;
        public const int Height = 220;
        private const int ImageX = 10;
        private const int ImageY = 10;
        private const int ImageWidth = 200;
        private const int ImageHeight = 200;
        private const int TimelineX = 240;
        private const int TimelineY = 60;
        private const int TimelineWidth = 380;
        private const int TimelineHeight = 10;
        private const int PodCastTitleX = 420;
        private const int PodCastTitleY = 10;
        private const int EpisodeTitleX = 420;
        private const int EpisodeTitleY = 30;

        public Image(Podcast podcast, string episodeId = null)
        {
            Podcast = podcast;
            CurrentEpisode = Podcast.Episodes.First();
            if (episodeId != null) CurrentEpisode = Podcast.Episodes.First(q => q.Unique == episodeId);
            _podcastImage = DownloadImage();
        }

        private System.Drawing.Image DownloadImage()
        {
            var image = new WebClient().DownloadData(Podcast.Image);
            return new Bitmap(new MemoryStream(image));
        }

        private void DrawBackground(ref Graphics graphics)
        {
            graphics.FillRectangle(_backgroundBrush, 0, 0, Width, Height);  // White Background
        }

        private void DrawImage(ref Graphics graphics)
        {
            var targetRegion = new Rectangle(ImageX, ImageY, ImageWidth, ImageHeight);
            var sourceRegion = new Rectangle(0, 0, _podcastImage.Width, _podcastImage.Height);
            graphics.DrawImage(_podcastImage, targetRegion, sourceRegion, GraphicsUnit.Pixel);
        }

        private void DrawBar(ref Graphics graphics)
        {
            graphics.FillRectangle(_timelineBrush, new Rectangle(TimelineX, TimelineY, TimelineWidth, TimelineHeight));
        }

        private void DrawTitle(ref Graphics graphics)
        {
            DrawCenteredText(ref graphics, _textBrush, PodCastTitleX, PodCastTitleY, Podcast.Title, FontName, FontSizeTitle);
            DrawCenteredText(ref graphics, _textBrush, EpisodeTitleX, EpisodeTitleY, CurrentEpisode.Title, FontName, FontSizeEpisode);
        }

        private void DrawCenteredText(ref Graphics graphics, Brush brush, int x, int y, string text, string fontname, int fontsize)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            StringFormat strinfFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            graphics.DrawString(text, new Font(fontname, fontsize), brush, new PointF(x, y), strinfFormat);
        }

        public Bitmap CreateBackgroundImage()
        {
            var emptyBackground = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            var graphics = Graphics.FromImage(emptyBackground);
            DrawBackground(ref graphics);
            DrawBar(ref graphics);
            DrawTitle(ref graphics);
            DrawImage(ref graphics);
            graphics.Dispose();
            return emptyBackground;
        }

      private double GetRelativePosition(TimeSpan currentPosition)
        {
            return currentPosition.TotalMilliseconds / CurrentEpisode.Duration.TotalMilliseconds;
        }

        private void DrawBarPosition(ref Graphics graphics, TimeSpan position)
        {
            graphics.FillRectangle(_timelineCurrentBrush, new Rectangle(TimelineX, TimelineY, (int)(TimelineWidth*GetRelativePosition(position)), TimelineHeight));
        }

        private Chapter GetNearChapter(Chapter currentChapter, int relativePosition)
        {
            if (currentChapter == null) return null;
            int currentChapterPosition = CurrentEpisode.Chapters.IndexOf(currentChapter);
            
            int absolutePosition = currentChapterPosition + relativePosition;
            if (absolutePosition< 0 || absolutePosition>= CurrentEpisode.Chapters.Count) return null;
            return CurrentEpisode.Chapters.ElementAt(absolutePosition);
        }

        private Chapter GetChapterAt(TimeSpan position)
        {
            if (CurrentEpisode.Chapters == null) return null;
            return CurrentEpisode.Chapters.OrderByDescending(q=>q.Offset).FirstOrDefault(q => q.Offset <= position);
        }

        private void DrawChapterInfo(ref Graphics graphics, TimeSpan position)
        {
            var currentChapter = GetChapterAt(position);
            if (currentChapter == null) return;
            DrawCenteredText(ref graphics, _currentChapterBrush, _chapterX, _chapterY, currentChapter.Title, FontName, FontSizeChapter);
            for (int i=-2; i<=2; i++)
            {
                if (i == 0) continue;
                var chapter = GetNearChapter(currentChapter, i);
                if (chapter == null) continue;
                DrawCenteredText(ref graphics, _otherChapterBrush, _chapterX, _chapterY+i*_otherChaptersDistance, chapter.Title, FontName, FontSizeOtherChapters);
            }
        }

        public Bitmap CreateImageForTime(TimeSpan position)
        {
            _background = _background ?? CreateBackgroundImage();
            var frame = new Bitmap(_background);
            var graphics = Graphics.FromImage(frame);
            DrawBarPosition(ref graphics, position);
            DrawChapterInfo(ref graphics, position);
            return frame;
        }
    }
}
