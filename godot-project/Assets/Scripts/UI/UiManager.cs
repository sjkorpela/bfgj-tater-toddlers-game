using Godot;

namespace Tater.Scripts.UI;

public partial class UiManager : Control
{
    [ExportCategory("Node References")]
    [Export] private PackedScene _startMenuScene;
    [Export] private PackedScene _startMenuSettingsScene;
    [Export] private PackedScene _startMenuAboutScene;
    [Export] private PackedScene _gameUiScene;
    [Export] private PackedScene _pauseMenuScene;
    [Export] private PackedScene _pauseMenuSettingsScene;
    [Export] private PackedScene _gameOverScene;
    
    private GameManager _gm;

    private Control _startMenu;
    private Control _startMenuSettings;
    private Control _startMenuAbout;
    private Control _gameUi;
    private Control _pauseMenu;
    private Control _pauseMenuSettings;
    private Control _gameOver;
    
    public override void _EnterTree()
    {
        _gm = Global.Instance.GameManager;
        _gm.OnGameStateChange += _onStateChange;
    }

    public override void _ExitTree()
    {
        _gm.OnGameStateChange -= _onStateChange;
    }

    private void _onStateChange(GameState newState, GameState oldState)
    {
        switch (oldState)
        {
            case GameState.MainMenu:
                _startMenu.QueueFree();
                break;
            case GameState.MainMenuSettings:
                _startMenuSettings.QueueFree();
                break;
            case GameState.MainMenuAbout:
                _startMenuAbout.QueueFree();
                break;
            case GameState.GameActive:
                _gameUi.QueueFree();
                break;
            case GameState.GamePaused:
                _pauseMenu.QueueFree();
                break;
            case GameState.GamePausedSettings:
                _pauseMenuSettings.QueueFree();
                break;
            case GameState.GameOver:
                _gameOver.QueueFree();
                break;
        }
        
        switch (newState)
        {
            case GameState.MainMenu:
                _startMenu = _startMenuScene.Instantiate<Control>();
                this.AddChild(_startMenu);
                break;
            case GameState.MainMenuSettings:
                _startMenuSettings = _startMenuSettingsScene.Instantiate<Control>();
                this.AddChild(_startMenuSettings);
                break;
            case GameState.MainMenuAbout:
                _startMenuAbout = _startMenuAboutScene.Instantiate<Control>();
                this.AddChild(_startMenuAbout);
                break;
            case GameState.GameActive:
                _gameUi = _gameUiScene.Instantiate<Control>();
                this.AddChild(_gameUi);
                break;
            case GameState.GamePaused:
                _pauseMenu = _pauseMenuScene.Instantiate<Control>();
                this.AddChild(_pauseMenu);
                break;
            case GameState.GamePausedSettings:
                _pauseMenuSettings = _pauseMenuSettingsScene.Instantiate<Control>();
                this.AddChild(_pauseMenuSettings);
                break;
            case GameState.GameOver:
                _gameOver = _gameOverScene.Instantiate<Control>();
                this.AddChild(_gameOver);
                break;
        }
    }
}