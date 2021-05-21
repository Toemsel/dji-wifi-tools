using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibVLCSharp.Shared;
using System;

namespace Dji.Camera
{
    public partial class VideoPlayer : Window, IDisposable
    {
        private LibVLC _libVlc = new LibVLC();
        private MediaPlayer _mediaPlayer;
        private Media _streamMedia;
        private string _videoFile;

        static VideoPlayer() => Core.Initialize();

        public VideoPlayer() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        public static void PlayVideo(string videoFile)
        {
            var videoPlayer = new VideoPlayer();
            videoPlayer._videoFile = videoFile;

            videoPlayer._streamMedia = new(videoPlayer._libVlc, new Uri(videoPlayer._videoFile));
            videoPlayer._mediaPlayer = new MediaPlayer(videoPlayer._streamMedia);
            videoPlayer.DataContext = videoPlayer._mediaPlayer;

            videoPlayer.Show();
        }

        protected override void OnOpened(EventArgs e) => _mediaPlayer.Play();

        public void Dispose()
        {
            _streamMedia.Dispose();
            _mediaPlayer.Dispose();
            _libVlc.Dispose();
        }
    }
}