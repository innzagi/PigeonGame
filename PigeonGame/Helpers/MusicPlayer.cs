using System;
using System.IO;
using System.Text;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace PigeonGame.Helpers;

// Зацикленное воспроизведение MP3 через WinRT MediaPlayer.
// Работает на Windows 10/11 без Windows Media Player.
// Play() идемпотентен — повторный вызов с тем же треком ничего не делает.
public static class MusicPlayer
{
    private static volatile string? _current;
    private static MediaPlayer?     _player;
    private static readonly object  _sync = new();

    static MusicPlayer()
    {
        // У WinForms-приложения может не быть консоли — тогда задать кодировку нельзя.
        try { Console.OutputEncoding = Encoding.UTF8; }
        catch { /* консоль не подключена — игнорируем */ }
    }

    /// <summary>name — имя файла без расширения: "Intro" или "Fight"</summary>
    public static void Play(string name)
    {
        lock (_sync)
        {
            if (_current == name) return;
            _current = name;
        }

        string path = Path.GetFullPath($"Resources/{name}.mp3");

        if (!File.Exists(path))
        {
            Log($"ОШИБКА: файл не найден — {path}");
            return;
        }

        Log($"Воспроизвожу: {path}");
        EnsurePlayer();

        _player!.Source          = MediaSource.CreateFromUri(new Uri(path));
        _player.IsLoopingEnabled = true;
        _player.Play();
    }

    public static void Stop()
    {
        lock (_sync) { _current = null; }
        _player?.Pause();
    }

    private static void EnsurePlayer()
    {
        if (_player != null) return;

        _player = new MediaPlayer();
        _player.MediaFailed += (_, e) =>
            Log($"MediaFailed: {e.ExtendedErrorCode.Message}");
        _player.MediaOpened += (_, _) =>
            Log("MediaOpened — воспроизведение начато");
    }

    private static void Log(string msg) =>
        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}  {msg}");
}
