using System.IO;
using System.Windows.Media;

namespace Client.Logic;

public class MusicManager : IDisposable
{
    private static MusicManager? _instance;
    public static MusicManager Instance => _instance ??= new MusicManager();

    private readonly MediaPlayer _mediaPlayer = new();
    private readonly List<string> _musicFiles = [];
    private readonly Random _random = new();
    private string? _currentFile;
    private bool _disposed;
    private int _currentMusicIndex;

    private double _targetVolume = 0.5;
    private double _currentVolume = 0;
    private const double FadeDuration = 2.0;
    private double _fadeElapsed;
    private bool _isFadingIn;
    private bool _isPlayingMusic;

    private DateTime _lastTrackEndTime = DateTime.MinValue;
    private const double IntervalBetweenTracks = 60.0;

    private MusicManager()
    {
        _mediaPlayer.MediaEnded += OnMediaEnded;
        LoadMusicFiles();
    }

    public double Volume
    {
        get => _targetVolume;
        set
        {
            _targetVolume = Math.Clamp(value, 0, 1);
            if (_isPlayingMusic)
                _mediaPlayer.Volume = _targetVolume;
        }
    }

    private void LoadMusicFiles()
    {
        try
        {
            var musicDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Music");
            if (!Directory.Exists(musicDir))
            {
                Directory.CreateDirectory(musicDir);
                return;
            }

            var files = Directory.GetFiles(musicDir, "*.mp3");
            _musicFiles.AddRange(files);

            if (_musicFiles.Count > 0)
            {
                ShuffleMusicList();
                _currentMusicIndex = 0;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to load music files: {ex}");
        }
    }

    private void ShuffleMusicList()
    {
        for (var i = _musicFiles.Count - 1; i > 0; i--)
        {
            var j = _random.Next(i + 1);
            (_musicFiles[i], _musicFiles[j]) = (_musicFiles[j], _musicFiles[i]);
        }
    }

    public void Start()
    {
        if (_disposed || _musicFiles.Count == 0)
            return;

        PlayNextTrack();
        _isPlayingMusic = true;
    }

    public void Stop()
    {
        _isPlayingMusic = false;
        _isFadingIn = false;
        _mediaPlayer.Stop();
    }

    public void Update(double deltaTime)
    {
        if (_disposed || !_isPlayingMusic)
            return;

        if (_isFadingIn)
        {
            _fadeElapsed += deltaTime;
            if (_fadeElapsed >= FadeDuration)
            {
                _currentVolume = _targetVolume;
                _isFadingIn = false;
            }
            else
            {
                var progress = _fadeElapsed / FadeDuration;
                _currentVolume = _targetVolume * progress;
            }
        }
        else
        {
            if (Math.Abs(_currentVolume - _targetVolume) > 0.01)
            {
                var diff = _targetVolume - _currentVolume;
                _currentVolume += diff * (float)(deltaTime / 0.2);
                _currentVolume = Math.Clamp(_currentVolume, 0, 1);
            }
            else
            {
                _currentVolume = _targetVolume;
            }
        }

        _mediaPlayer.Volume = _currentVolume;

        if (_mediaPlayer.Source == null && _musicFiles.Count > 0)
        {
            var timeSinceEnd = (DateTime.Now - _lastTrackEndTime).TotalSeconds;
            if (timeSinceEnd >= IntervalBetweenTracks)
            {
                PlayNextTrack();
            }
        }
    }

    private void PlayNextTrack()
    {
        if (_musicFiles.Count == 0)
            return;

        _currentFile = _musicFiles[_currentMusicIndex];
        _currentMusicIndex++;

        if (_currentMusicIndex >= _musicFiles.Count)
        {
            ShuffleMusicList();
            _currentMusicIndex = 0;
        }

        try
        {
            _mediaPlayer.Open(new Uri(_currentFile, UriKind.Absolute));
            _mediaPlayer.Play();
            _fadeElapsed = 0;
            _isFadingIn = true;
            _currentVolume = 0;
            _lastTrackEndTime = DateTime.Now;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to play music file {_currentFile}: {ex}");
        }
    }

    private void OnMediaEnded(object? sender, EventArgs e)
    {
        _lastTrackEndTime = DateTime.Now;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _mediaPlayer.Stop();
        _mediaPlayer.Close();
        GC.SuppressFinalize(this);
    }
}
