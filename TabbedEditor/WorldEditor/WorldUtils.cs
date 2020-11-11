using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TabbedEditor.IO;
using TabbedEditor.WorldEditor.Data;
using worldProto;

namespace TabbedEditor.WorldEditor
{
    public static class WorldUtils
    {
        public static WorldData LoadWorldData(string path)
        {
            var data = File.ReadAllText(path);
            if (!IsValidJson(data))
                throw new Exception("File is not a valid JSON file.");
            var conversionSettings = new JsonSerializerSettings();
            conversionSettings.Error += (sender, args) => throw new Exception("File is not serializable as a WorldData object.");
            var worldData = JsonConvert.DeserializeObject<WorldData>(data,  conversionSettings);
            return worldData;
        }
        
        public static bool TrySaveWorldData(WorldData worldData, string path)
        {
            WorldRules[] brokenRules = CheckForBrokenRules(worldData);
            if (brokenRules.Length != 0)
            {
                var brokenRulesAsString = brokenRules.Aggregate("", (current, rule) => current + ("\n\t- " + rule));
                MessageBox.Show("The following errors have occured:" + brokenRulesAsString, "Saving not permitted");

                return false;
            }

            try { File.WriteAllText(path, JsonConvert.SerializeObject(worldData)); }
            catch (Exception e)
            {
                Logger.DumpException(e);
                MessageBox.Show("The following exception occured while trying to save:\n\n" + e.Message, "Failed to save world file");
                return false;
            }

            return true;
        }

        public enum WorldRules
        {
            TooMuchWater
        }

        public static WorldRules[] CheckForBrokenRules(WorldData data)
        {
            List<WorldRules> brokenRules = new List<WorldRules>();
            int xSize = data.TileArray.GetLength(1);
            int ySize = data.TileArray.GetLength(0);

            var totalTiles = xSize * ySize;
            
            // Checking amount of water tiles | must be less than half
            var waterTiles = 0;
            for (var y = 0; y < ySize; y++)
            for (var x = 0; x < xSize; x++)
                if (data.TileArray[y, x].TileType == TileType.Water)
                    waterTiles++;

            if (waterTiles >= totalTiles / 2f)
                brokenRules.Add(WorldRules.TooMuchWater);

            return brokenRules.ToArray();
        }
        
        // IsValidJson Function created by Habib & Oleksii (https://stackoverflow.com/a/14977915)
        // Optimized by Rider IDE and myself
        private static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false;}
            
            strInput = strInput.Trim();
            
            if ((!strInput.StartsWith("{") || !strInput.EndsWith("}")) && (!strInput.StartsWith("[") || !strInput.EndsWith("]"))) return false;
            
            try
            {
                JToken.Parse(strInput);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static WorldMessage ToWorldMessage(WorldData worldData)
        {
            int xLenght = worldData.TileArray.GetLength(0);
            int yLenght = worldData.TileArray.GetLength(1);
            
            WorldMessage worldMessage = new WorldMessage();

            for (int y = 0; y < yLenght; y++)
            {
                RowMessage rowMessage = new RowMessage();
                for (int x = 0; x < xLenght; x++)
                {
                    TileDataMessage tileDataMessage = new TileDataMessage();
                    TileData tileData = worldData.TileArray[x, y];
                    tileDataMessage.tileType = (int)tileData.TileType;
                    tileDataMessage.enemyCount = tileData.EnemyCount;
                    rowMessage.rows.Add(tileDataMessage);
                }
                worldMessage.TileArray.Add(rowMessage);
            }

            return worldMessage;
        }
    }
}