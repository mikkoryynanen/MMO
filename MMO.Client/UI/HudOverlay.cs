using System.Numerics;
using ImGuiNET;
using MMO.Client.Game;

namespace MMO.Client.UI;

public class HudOverlay
{
    public void Draw(GameState state, bool connected, string? connectionError, sbyte inputX, sbyte inputY)
    {
        var viewport = ImGui.GetMainViewport();
        var pos = new Vector2(viewport.Pos.X + 12, viewport.Pos.Y + 12);
        ImGui.SetNextWindowPos(pos, ImGuiCond.Always);

        ImGui.Begin("HUD",
            ImGuiWindowFlags.AlwaysAutoResize |
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoResize |
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoCollapse);

        ImGui.Text($"User: {state.Self.Username}");
        ImGui.Text($"Pos: ({state.Self.X:F1}, {state.Self.Y:F1})");
        ImGui.Text($"Remotes: {state.RemotePlayers.Count}");
        ImGui.Text(connected ? "Connected" : "Disconnected");

        ImGui.Separator();
        var inputLabel = $"Input: ({inputX}, {inputY})";
        if (inputX != 0 || inputY != 0)
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.6f, 1f, 0.6f, 1f));
        ImGui.Text(inputLabel);
        if (inputX != 0 || inputY != 0)
            ImGui.PopStyleColor();

        if (!string.IsNullOrEmpty(connectionError))
        {
            ImGui.Separator();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0.5f, 0.3f, 1f));
            ImGui.TextWrapped(connectionError);
            ImGui.PopStyleColor();
        }

        if (!string.IsNullOrEmpty(state.LastEvent))
        {
            ImGui.Separator();
            ImGui.TextWrapped(state.LastEvent);
        }

        ImGui.Separator();
        ImGui.Text("WASD / Arrows — move");
        ImGui.Text("Esc — disconnect");

        ImGui.End();
    }
}
