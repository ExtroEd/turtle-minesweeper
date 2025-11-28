using System.Windows;
using System.Windows.Controls;

namespace Client.Logic;

public class SinglePlayerControl(SinglePlayerSettings settings)
{
    private bool _internal;

    public void UpdateTotal(int total, Slider splitSlider, TextBlock minesBlock, TextBlock wallsBlock, UIElement splitPanel)
    {
        if (_internal) return;
        _internal = true;

        total = Math.Max(1, total);
        splitSlider.Maximum = total;
        splitPanel.Visibility = Visibility.Visible;

        if (splitSlider.Value > total)
            splitSlider.Value = total;

        var mines = (int)splitSlider.Value;
        var walls = total - mines;

        minesBlock.Text = mines.ToString();
        wallsBlock.Text = walls.ToString();

        settings.MinePercent = mines;
        settings.WallPercent = walls;

        _internal = false;
    }

    public void UpdateSplit(int total, Slider splitSlider, TextBlock minesBlock, TextBlock wallsBlock)
    {
        if (_internal) return;
        _internal = true;

        var mines = (int)splitSlider.Value;
        if (mines > total)
        {
            mines = total;
            splitSlider.Value = total;
        }

        var walls = total - mines;

        minesBlock.Text = mines.ToString();
        wallsBlock.Text = walls.ToString();

        settings.MinePercent = mines;
        settings.WallPercent = walls;

        _internal = false;
    }

    public static int ClampGridSize(int value)
        => Math.Max(10, Math.Min(500, value));

    public static int ClampFoxSpeed(int value)
        => Math.Max(1, Math.Min(10, value));

    public void Randomize()
    {
        var rnd = new Random();

        settings.GridSize = rnd.Next(10, 501);

        var totalPercent = rnd.Next(1, 81);
        settings.MinePercent = rnd.Next(0, totalPercent + 1);
        settings.WallPercent = totalPercent - settings.MinePercent;

        settings.FoxSpeed = rnd.Next(1, 11);
        settings.EnableFox = rnd.Next(0, 2) == 1;
    }

    public static (int mines, int walls) ComputeSplit(int total, int mines)
    {
        mines = Math.Max(0, Math.Min(total, mines));
        return (mines, total - mines);
    }

    public bool TryValidateAll(TextBox gridSizeBox, TextBlock minesBlock, TextBlock wallsBlock,
                               TextBox foxSpeedBox, bool foxEnabled)
    {
        if (!int.TryParse(gridSizeBox.Text, out var grid) || grid < 10 || grid > 500)
        {
            MessageBox.Show("Grid Size must be 10–500.");
            return false;
        }

        if (!int.TryParse(minesBlock.Text, out var mines)) return false;
        if (!int.TryParse(wallsBlock.Text, out var walls)) return false;

        var foxSpeed = 0;
        if (foxEnabled)
        {
            if (!int.TryParse(foxSpeedBox.Text, out foxSpeed) || foxSpeed < 1 || foxSpeed > 10)
            {
                MessageBox.Show("Fox Speed must be 1–10.");
                return false;
            }
        }

        settings.GridSize = grid;
        settings.MinePercent = mines;
        settings.WallPercent = walls;
        settings.EnableFox = foxEnabled;
        settings.FoxSpeed = foxSpeed;

        return true;
    }

    public void SaveSettings()
        => settings.Save();
}
