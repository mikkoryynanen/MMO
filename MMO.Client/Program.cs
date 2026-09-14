using MMO.Client.Game;
using MMO.Client.Network;
using MMO.Client.Render;
using MMO.Client.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raylib_cs;
using rlImGui_cs;

internal static class Program
{
    private const int WindowWidth = 1024;
    private const int WindowHeight = 768;

    [STAThread]
    private static async Task<int> Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var (worldHost, worldPort) = ResolveWorldEndpoint(builder.Configuration);
        var authBase = ResolveAuthEndpoint(builder.Configuration);

        builder.Services.AddHttpClient<AuthClient>(client =>
        {
            client.BaseAddress = new Uri(authBase);
        });

        builder.Services.AddSingleton(_ => new GameClient(
            _.GetRequiredService<AuthClient>(),
            worldHost,
            worldPort));

        var host = builder.Build();
        var game = host.Services.GetRequiredService<GameClient>();

        var camera = new WorldCamera
        {
            ScreenWidth = WindowWidth,
            ScreenHeight = WindowHeight,
            Zoom = 3.5f,
        };
        var renderer = new Renderer(camera);
        var login = new LoginScreen();
        var hud = new HudOverlay();

        Raylib.SetConfigFlags(ConfigFlags.VSyncHint | ConfigFlags.Msaa4xHint);
        Raylib.InitWindow(WindowWidth, WindowHeight, "MMO Client");
        rlImGui.Setup(true);

        try
        {
            while (!Raylib.WindowShouldClose())
            {
                float dt = Raylib.GetFrameTime();
                camera.Follow(game.State.Self);

                game.Update(dt, Raylib.IsKeyPressed(KeyboardKey.Escape));

                Raylib.BeginDrawing();
                if (game.Mode == ClientMode.InGame)
                {
                    renderer.DrawWorld(game.State);
                }
                else
                {
                    Raylib.ClearBackground(new Color(15, 18, 24, 255));
                }

                rlImGui.Begin();
                if (game.Mode == ClientMode.Login)
                {
                    var outcome = login.Draw();
                    if (outcome != LoginOutcome.None && login.CanSubmit)
                    {
                        login.BeginWork();
                        _ = HandleLoginOutcome(game, login, outcome);
                    }
                }
                else
                {
                    var (ix, iy) = game.Input.Vector();
                    hud.Draw(
                        game.State,
                        game.Connection?.IsConnected ?? false,
                        game.Connection?.ConnectionError,
                        ix,
                        iy);
                }
                rlImGui.End();

                Raylib.EndDrawing();
            }
        }
        finally
        {
            await game.DisposeAsync();
            rlImGui.Shutdown();
            Raylib.CloseWindow();
        }

        return 0;
    }

    private static async Task HandleLoginOutcome(GameClient game, LoginScreen login, LoginOutcome outcome)
    {
        try
        {
            bool ok = outcome switch
            {
                LoginOutcome.SubmitLogin    => await game.TryLoginAsync(login.Username, login.Password, default),
                LoginOutcome.SubmitRegister => await game.TryRegisterAsync(login.Username, login.Password, default),
                _ => false,
            };

            if (ok)
            {
                login.Complete();
            }
            else
            {
                login.Fail(outcome == LoginOutcome.SubmitRegister
                    ? "Registration failed (username may be taken)."
                    : "Login failed.");
            }
        }
        catch (OperationCanceledException)
        {
            login.Fail("Connection timed out.");
        }
        catch (Exception ex)
        {
            login.Fail(ex.Message);
        }
    }

    private static (string host, int port) ResolveWorldEndpoint(IConfiguration config)
    {
        var aspireKey = config["services:world-server:tcp:0"]
                        ?? Environment.GetEnvironmentVariable("services__world-server__tcp__0")
                        ?? "tcp://localhost:24000";

        var uri = new Uri(aspireKey);
        return (uri.Host, uri.Port);
    }

    private static string ResolveAuthEndpoint(IConfiguration config)
    {
        var aspireKey = config["services:auth-api:http:0"]
                        ?? Environment.GetEnvironmentVariable("services__auth-api__http__0")
                        ?? "http://localhost:5133";

        var uri = new Uri(aspireKey);
        return $"{uri.Scheme}://{uri.Host}:{uri.Port}";
    }
}
