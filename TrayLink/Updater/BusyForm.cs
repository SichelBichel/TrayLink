#nullable enable
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayLink.Updater;

public sealed class BusyForm : Form
{
    private readonly Label _lbl;

    public BusyForm(string text)
    {
        Text = "Please wait";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        ControlBox = false;
        ShowInTaskbar = false;
        TopMost = true;

        ClientSize = new Size(420, 110);

        _lbl = new Label
        {
            Left = 12,
            Top = 12,
            Width = ClientSize.Width - 24,
            Height = 40,
            Text = text ?? "Loading…"
        };

        var bar = new ProgressBar
        {
            Left = 12,
            Top = 62,
            Width = ClientSize.Width - 24,
            Height = 18,
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 30
        };

        Controls.Add(_lbl);
        Controls.Add(bar);
    }

    public void SetText(string text)
    {
        if (InvokeRequired) { BeginInvoke(new Action(() => SetText(text))); return; }
        _lbl.Text = text ?? "";
    }
}

public static class BusyUi
{
    public static async Task RunAsync(IWin32Window owner, string text, Func<Task> action)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        using var dlg = new BusyForm(text);

        // Sobald die Form sichtbar ist, starten wir die eigentliche Arbeit
        dlg.Shown += async (s, e) =>
        {
            try
            {
                // Task.Run stellt sicher, dass CPU-intensive Arbeit nicht die UI blockiert
                await Task.Run(action);
            }
            catch (Exception)
            {
                // Hier optional Fehler loggen
                throw;
            }
            finally
            {
                dlg.DialogResult = DialogResult.OK; // Schließt die Form modal
            }
        };

        // Zeigt die Form modal an und blockiert die Interaktion mit dem Hauptfenster automatisch
        dlg.ShowDialog(owner);

        // Da ShowDialog blockiert, sind wir erst hier fertig, wenn die Form geschlossen wurde
        await Task.CompletedTask;
    }

    public static async Task<T> RunAsync<T>(IWin32Window owner, string text, Func<Task<T>> action)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        using var dlg = new BusyForm(text);
        T result = default!;
        Exception? exception = null;

        dlg.Shown += async (s, e) =>
        {
            try
            {
                result = await Task.Run(action);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                dlg.DialogResult = DialogResult.OK;
            }
        };

        dlg.ShowDialog(owner);

        if (exception != null)
            throw exception;

        return result;
    }
}