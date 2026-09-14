using System.Numerics;
using ImGuiNET;

namespace MMO.Client.UI;

public enum LoginOutcome { None, SubmitLogin, SubmitRegister, Cancelled }

public enum LoginPhase { Idle, Working, Done, Error }

public class LoginScreen
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Error { get; set; }

    private LoginPhase _phase = LoginPhase.Idle;
    public LoginPhase Phase
    {
        get => _phase;
        set => _phase = value;
    }

    public bool CanSubmit => _phase is LoginPhase.Idle or LoginPhase.Error;

    public void BeginWork()
    {
        Error = null;
        _phase = LoginPhase.Working;
    }

    public void Complete()
    {
        _phase = LoginPhase.Done;
    }

    public void Fail(string message)
    {
        Error = message;
        _phase = LoginPhase.Error;
    }

    public LoginOutcome Draw()
    {
        var outcome = LoginOutcome.None;
        var working = _phase == LoginPhase.Working;

        var viewport = ImGui.GetMainViewport();
        var size = new Vector2(360, 240);
        var pos = new Vector2(
            viewport.Pos.X + (viewport.Size.X - size.X) * 0.5f,
            viewport.Pos.Y + (viewport.Size.Y - size.Y) * 0.5f);

        ImGui.SetNextWindowPos(pos, ImGuiCond.Always);
        ImGui.SetNextWindowSize(size, ImGuiCond.Always);
        ImGui.Begin("Login", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse);

        ImGui.InputText("Username", ref _usernameBuf, 64);
        Username = _usernameBuf;

        ImGui.InputText("Password", ref _passwordBuf, 64, ImGuiInputTextFlags.Password);
        Password = _passwordBuf;

        if (!string.IsNullOrEmpty(Error))
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0.4f, 0.4f, 1f));
            ImGui.TextWrapped(Error);
            ImGui.PopStyleColor();
        }

        ImGui.Spacing();
        ImGui.BeginDisabled(working);
        if (ImGui.Button("Login", new Vector2(160, 30)))
        {
            Error = null;
            outcome = LoginOutcome.SubmitLogin;
        }
        ImGui.SameLine();
        if (ImGui.Button("Register", new Vector2(160, 30)))
        {
            Error = null;
            outcome = LoginOutcome.SubmitRegister;
        }
        ImGui.EndDisabled();

        if (working)
        {
            ImGui.Spacing();
            ImGui.TextDisabled("Connecting...");
        }

        ImGui.End();

        return outcome;
    }

    public void ResetBuffers()
    {
        _usernameBuf = Username;
        _passwordBuf = Password;
    }

    private string _usernameBuf = string.Empty;
    private string _passwordBuf = string.Empty;
}
