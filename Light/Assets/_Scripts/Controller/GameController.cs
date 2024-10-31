using System.Linq;
using Config;
using GameData;
using GMVC.Conditions;
using GMVC.Core;
using GMVC.Utls;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utls;


namespace Controller
{
    public class GameController : ControllerBase
    {
        GameWorld World => Game.World;
        GameConfig Config => Game.Config;

        public void Game_StartNewStage()
        {
            if (World.Status != GameWorld.GameStates.Start)
            {
                Debug.LogWarning("游戏状态错误！");
                return;
            }
            Config.PlayerPrefab.Display(false);
            StartGame(null);
            Debug.Log("游戏执行中！");
        }
        public void Game_LoadStage()
        {
            if (World.Status != GameWorld.GameStates.Start)
            {
                Debug.LogWarning("游戏状态错误！");
                return;
            }
            Config.PlayerPrefab.Display(false);
            var json = PlayerPrefs.GetString(GameTag.PlayerSaveString);
            var save = Json.Deserialize<PlayerSave>(json);
            StartGame(save);
            Debug.Log("游戏执行中！");
        }

        void StartGame(PlayerSave save)
        {
            var player = InstancePlayer(save);
            World.Start();
            World.SetGameStage(player, new StageStory(Config.StageTimeComponent, save?.StageTime ?? 0));
            World.StartGameStage();
        }

        PlayableUnit InstancePlayer(PlayerSave save)
        {
            Player player;
            var playerControl = Object.Instantiate(Config.PlayerPrefab, Game.GameUnitTransform);
            if (save != null)
            {
                player = DefaultPlayer(save.Hp, save.Lantern);
                playerControl.transform.position = playerControl.transform.position.ChangeXZ(save.PosX,save.PosZ);
                foreach (var s in save.Spells)
                {
                    var sp = Config.SpellSo.GetSpell(s.SpellName);
                    player.AddSpell(sp, s.Remain);
                }
            }
            else player = DefaultPlayer(10, 3);
            Config.PlayerCfgSo.Load(playerControl);
            return new PlayableUnit(player, playerControl);
        }

        Player DefaultPlayer(int hp, int lantern) =>
            new(new ConValue("血量",10,10, hp),
                new ConValue("虫灯", 10, 10, lantern));
        public void Game_End()
        {
            World.End();
            Game.End();
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        public void Game_Pause() => World.Stage.Paused();
        public void Game_Resume() => World.Stage.Resume();

        public void Game_Save()
        {
            var stage = Game.World.Stage;
            var player = stage.Player;
            var save = new PlayerSave(player.Hp.Value, player.Firefly.Value, stage.Story.Seconds,
                player.PlayerControl.transform.position, AchievementSystem.SkeletonDeathCount,
                player.Magics.Select(m => new PlayerSpell(m.Spell.SpellName, m.Times)).ToArray());
            var json = Json.Serialize(save);
            PlayerPrefs.SetString(GameTag.PlayerSaveString, json);
        }
    }
}