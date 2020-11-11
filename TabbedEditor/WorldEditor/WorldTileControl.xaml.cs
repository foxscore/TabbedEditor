using System.Collections.Generic;
using System.Windows.Media;
using TabbedEditor.WorldEditor.Data;

namespace TabbedEditor.WorldEditor
{
    public partial class WorldTileControl
    {
        private readonly TileData _tile;
        public WorldTileControl(TileData data)
        {
            _tile = data;
            InitializeComponent();
            UpdateData();
        }

        public static readonly Dictionary<TileType, Color> TileTypeToColor = new Dictionary<TileType, Color>()
        {
            { TileType.Grass, Colors.Green },
            { TileType.Sand, Colors.SandyBrown },
            { TileType.Stone, Colors.LightSlateGray },
            { TileType.Water, Colors.DodgerBlue }
        };

        public TileType TileType
        {
            get { return _tile.TileType; }
            set
            {
                _tile.TileType = value;
                UpdateData();
            }
        }

        public int EnemyCount
        {
            get { return _tile.EnemyCount; }
            set
            {
                _tile.EnemyCount = value;
                UpdateData();
            }
        }
        
        private void UpdateData()
        {
            TileBackground.Background = new SolidColorBrush(TileTypeToColor[_tile.TileType]);
            EnemyCountLabel.Content = _tile.EnemyCount.ToString();
        }
    }
}