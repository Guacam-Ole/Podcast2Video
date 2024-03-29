﻿using P2VEntities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;

namespace P2VBL
{
    public class Rss
    {
        public Podcast Podcast { get; set; }

        public Rss(string localRssFile)
        {
            var rssContent = File.ReadAllBytes(localRssFile);
            Podcast = ReadRss(new MemoryStream(rssContent));
        }

        public Rss(Uri rssFeed)
        {
            var rssContent = new WebClient().DownloadData(rssFeed);
            Podcast = ReadRss(new MemoryStream(rssContent));
        }

        private Podcast ReadRss(Stream rssStream)
        {
            var doc = new XmlDocument();
            doc.Load(rssStream);
            XmlNamespaceManager namespacemanager = new XmlNamespaceManager(doc.NameTable);
            namespacemanager.AddNamespace("itunes", "http://www.itunes.com/dtds/podcast-1.0.dtd");
            namespacemanager.AddNamespace("psc", "http://podlove.org/simple-chapters");

            var channel = doc.SelectSingleNode("/rss/channel");
            var podcast = new Podcast
            {
                Title = channel.SelectSingleNode("title").InnerText,
                Description = channel.SelectSingleNode("description").InnerText,
                Image = channel.SelectSingleNode("image").SelectSingleNode("url").InnerText,
                Episodes = new List<Episode>()
            };

            var episodes = channel.SelectNodes("item");
            foreach (XmlNode xmlEpisode in episodes)
            {
                int counter = 0;
                var episode = new Episode
                {
                    Title = xmlEpisode.SelectSingleNode("title").InnerText,
                    Description = xmlEpisode.SelectSingleNode("description").InnerText,
                    Link = xmlEpisode.SelectNodes("link")[0].InnerText,
                    Id = counter++,
                    Unique = xmlEpisode.SelectSingleNode("itunes:episode", namespacemanager)?.InnerText,
                    Duration = xmlEpisode.SelectSingleNode("itunes:duration", namespacemanager).InnerText.ToTimeSpan(),
                    Audio = xmlEpisode.SelectSingleNode("enclosure").Attributes["url"].InnerText,
                    Chapters = new List<Chapter>(),
                    Published=DateTime.Parse(xmlEpisode.SelectSingleNode("pubDate").InnerText)
                };

                var chapterContainer = xmlEpisode.SelectSingleNode("psc:chapters", namespacemanager);
                if (chapterContainer != null)
                {
                    var xmlChapters = chapterContainer.SelectNodes("psc:chapter", namespacemanager);
                    foreach (XmlNode xmlChapter in xmlChapters)
                    {
                        var chapter = new Chapter
                        {
                            Offset = xmlChapter.Attributes["start"].Value.ToTimeSpan(),
                            Title = xmlChapter.Attributes["title"].Value,
                            Url = xmlChapter.Attributes["href"]?.Value
                        };
                        episode.Chapters.Add(chapter);
                    }
                }
                podcast.Episodes.Add(episode);
            }
            return podcast;
        }
    }
}