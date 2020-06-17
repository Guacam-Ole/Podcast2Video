using P2VEntities;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace P2VBL
{
    public class Output
    {
        private Episode _currentEpisode;
        private Podcast _podcast;
        private Image _podcastImage;
        private Brush _backgroundBrush = Brushes.White;
        private Brush _timelineBrush = Brushes.DarkOrange;
        private Brush _timelineCurrentBrush = Brushes.Orange;
        private Brush _textBrush = Brushes.Black;

        private const int Width = 640;
        private const int Height = 220;
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

        public Output(Podcast podcast, string episodeId = null)
        {
            _podcast = podcast;
            _currentEpisode = _podcast.Episodes.First();
            if (episodeId != null) _currentEpisode = _podcast.Episodes.First(q => q.Unique == episodeId);
            _podcastImage = DownloadImage();
            //CreateImage();
        }

        private Image DownloadImage()
        {
            var image = new WebClient().DownloadData(_podcast.Image);
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
            DrawCenteredText(ref graphics, _textBrush, PodCastTitleX, PodCastTitleY, _podcast.Title, "Tahoma", 12);
            DrawCenteredText(ref graphics, _textBrush, EpisodeTitleX, EpisodeTitleY, _currentEpisode.Title, "Tahoma", 8);
        }

        private void DrawCenteredText(ref Graphics graphics, Brush brush, int x, int y, string text, string fontname = "Tahoma", int fontsize = 8)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            StringFormat strinfFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            graphics.DrawString(text, new Font(fontname, fontsize), brush, new PointF(x, y), strinfFormat);
        }

        public void CreateImage()
        {

            var emptyBackground = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var grp = Graphics.FromImage(emptyBackground);
            DrawBackground(ref grp);
            DrawBar(ref grp);
            DrawTitle(ref grp);
            DrawImage(ref grp);
            grp.Dispose();
            emptyBackground.Save("d:\\empty.jpeg", ImageFormat.Jpeg);
        }
    }
}
