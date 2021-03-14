﻿using System.Collections.Generic;
using System.Linq;
using Helion.Layer.WorldLayers;
using Helion.Maps;
using Helion.Resources.Definitions.MapInfo;
using Helion.Util.Consoles;
using Helion.Util.Extensions;
using Helion.World.Cheats;

namespace Helion.Client
{
    public partial class Client
    {
        private void Console_OnCommand(object? sender, ConsoleCommandEventArgs ccmdArgs)
        {
            switch (ccmdArgs.Command.ToUpper())
            {
                case "EXIT":
                    m_window.Close();
                    break;

                case "MAP":
                    HandleMap(ccmdArgs.Args);
                    break;
                
                case "STARTGAME":
                    StartNewGame();
                    break;

                case "VOLUME":
                    SetVolume(ccmdArgs.Args);
                    break;

                case "AUDIODEVICE":
                    HandleAudioDevice(ccmdArgs.Args);
                    break;

                case "AUDIODEVICES":
                    PrintAudioDevices();
                    break;

                default:
                    HandleDefault(ccmdArgs);
                    break;
            }
        }

        private void HandleDefault(ConsoleCommandEventArgs ccmdArgs)
        {
            if (!m_layerManager.TryGetLayer(out SinglePlayerWorldLayer? layer) || layer.World.EntityManager.Players.Count == 0)
                return;
            
            if (!CheatManager.Instance.HandleCommand(layer.World.EntityManager.Players[0], ccmdArgs.Command))
                Log.Info($"Unknown command: {ccmdArgs.Command}");
        }

        private void StartNewGame()
        {
            MapInfoDef? mapInfoDef = GetDefaultMap();
            if (mapInfoDef == null)
            {
                Log.Error("Unable to find default map for game to start on");
                return;
            }
            
            LoadMap(mapInfoDef.MapName);
        }

        private void PrintAudioDevices()
        {
            int num = 1;
            foreach (string device in m_audioSystem.GetDeviceNames())
                Log.Info($"{num++}. {device}");
        }

        private void HandleAudioDevice(IList<string> args)
        {
            if (args.Empty())
            {
                Log.Info(m_audioSystem.GetDeviceName());
                return;
            }

            if (!int.TryParse(args[0], out int deviceIndex))
                return;

            deviceIndex--;
            List<string> deviceNames = m_audioSystem.GetDeviceNames().ToList();
            if (deviceIndex < 0 || deviceIndex >= deviceNames.Count)
                return;

            SetAudioDevice(deviceNames[deviceIndex]);
        }

        private void SetAudioDevice(string deviceName)
        {
            m_config.Audio.Device.Set(deviceName);
            m_audioSystem.SetDevice(deviceName);
            m_audioSystem.SetVolume(m_config.Audio.Volume);
        }

        private void SetVolume(IList<string> args)
        {
            if (args.Empty() || !float.TryParse(args[0], out float volume))
            {
                Log.Info("Usage: volume <volume>");
                return;
            }

            m_config.Audio.Volume.Set(volume);
            m_audioSystem.SetVolume(volume);
        }

        private void HandleMap(IList<string> args)
        {
            if (args.Empty())
            {
                Log.Info("Usage: map <mapName>");
                return;
            }

            // For now, we will only have one world layer present. If someone
            // wants to `map mapXX` offline then it will kill their connection
            // and go offline to some world.
            // m_layerManager.Remove<WorldLayer>();

            string mapName = args[0];
            IMap? map = m_archiveCollection.FindMap(mapName);
            if (map == null)
            {
                Log.Warn("Cannot load map '{0}', it cannot be found or is corrupt", mapName);
                return;
            }

            SkillDef? skillDef = m_archiveCollection.Definitions.MapInfoDefinition.MapInfo.GetSkill(m_config.Game.Skill);
            if (skillDef == null)
            {
                Log.Warn($"Could not find skill definition for {m_config.Game.Skill}");
                return;
            }

            MapInfoDef mapInfoDef = m_archiveCollection.Definitions.MapInfoDefinition.MapInfo.GetMapInfoOrDefault(map.Name);
            
            // TODO: Fix me later (we should always create a new layer, and never branch like this)
            if (m_layerManager.TryGetLayer(out SinglePlayerWorldLayer? worldLayer))
                worldLayer.LoadMap(mapInfoDef, false);
            else
            {
                SinglePlayerWorldLayer? newLayer = SinglePlayerWorldLayer.Create(m_layerManager, m_config, m_console, 
                    m_audioSystem, m_archiveCollection, mapInfoDef, m_saveGameManager, skillDef, map);
                if (newLayer == null)
                    return;

                m_layerManager.Add(newLayer);
                newLayer.World.Start();
            }
            
            m_layerManager.RemoveAllBut<WorldLayer>();
        }
    }
}
