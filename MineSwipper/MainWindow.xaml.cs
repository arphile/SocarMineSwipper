using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace MineSwipper
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 화면 구성을 위한 클래스 선언
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Initialize();
        }

        /// <summary>
        /// 지뢰의 개수 지정
        /// </summary>
        public int MineAmount { get => 10; }

        /// <summary>
        /// 상자의 크기 지정 (정사각형으로 만들도록 구성함)
        /// </summary>
        public int BoxSize { get => 10; }

        /// <summary>
        /// 현재 영토에 대한 정의 리스트
        /// </summary>
        public Dictionary<LandData, Button> Locations { get; set; }

        /// <summary>
        /// 초기화를 진행한다.
        /// </summary>
        public void Initialize()
        {
            Locations = new Dictionary<LandData, Button>();

            var Grid = new UniformGrid()
            {
                Rows = BoxSize,
                Columns = BoxSize
            };

            for ( int i = 0; i < BoxSize; i++ )
            {
                for ( int j = 0; j < BoxSize; j++ )
                {
                    Button Btn = new Button(){
                        Width = 30,
                        Height = 30,
                        Tag = 0
                    };

                    Grid.Children.Add(Btn);
                    LandData CurLoc = new LandData()
                    {
                        X = i,
                        Y = j
                    };

                    Locations.Add(CurLoc, Btn);
                }
            }

            MineArea.Children.Add(Grid);

            GenerateMines();
        }

        /// <summary>
        /// 지뢰를 생성한다.
        /// </summary>
        public void GenerateMines()
        {
            RemoveMines();

            List<int> MineLocations = new List<int>();

            Random r = new Random();
            while( MineLocations.Count < 10 )
            {
                int Target = r.Next() % Locations.Count;
                if ( !MineLocations.Contains(Target) )
                {
                    MineLocations.Add(Target);
                }
            }

            List<LandData> Lists = Locations.Keys.ToList();
            for (int i = 0; i < MineLocations.Count; i++)
            {
                LandData Current = Lists[MineLocations[i]];
                Current.IsMineArea = true;

                List<LandData> Nearby = Lists.Where(Elem => (Math.Abs(Elem.X - Current.X) <= 1 && Math.Abs(Elem.Y - Current.Y) <= 1)).ToList();
                foreach ( LandData NB in Nearby )
                {
                    if (Current != NB)
                    {
                        NB.MinesNearby++;
                    }
                }
            }

            InvalidateProperties();
        }

        /// <summary>
        /// 현재 지뢰가 있는 영역을 구한다.
        /// </summary>
        /// <returns></returns>
        public List<LandData> GetMineLocations()
        {
            return Locations.Keys.Where(x => x.IsMineArea).ToList();
        }

        /// <summary>
        /// 지뢰를 제거한다.
        /// 그리고 근처에 있는 카운팅 또한 제거한다.
        /// </summary>
        public void RemoveMines()
        {
            foreach (KeyValuePair<LandData, Button> KVP in Locations)
            {
                KVP.Key.IsMineArea = false;
                KVP.Key.MinesNearby = 0;
            }

            InvalidateProperties();
        }

        /// <summary>
        /// 현재 각 영토에 대한 UI 처리를 진행한다.
        /// </summary>
        public void InvalidateProperties()
        {
            foreach( KeyValuePair<LandData, Button> KVP in Locations )
            {
                LandData Current = KVP.Key;
                Button Btn = KVP.Value;

                Btn.Content = Current.MinesNearby;
                Btn.Background = Current.IsMineArea ? Brushes.HotPink : Current.MinesNearby == 0 ? Brushes.White : Brushes.LightGray;
            }
        }

        /// <summary>
        /// Relocate 버튼에 대한 처리
        /// </summary>
        /// <param name="sender">Relocate Button</param>
        /// <param name="e">Mouse Event</param>
        private void OnRelocate(object sender, MouseButtonEventArgs e)
        {
            GenerateMines();
        }
    }

    /// <summary>
    /// 영토 데이터에 대한 정의
    /// </summary>
    public class LandData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int MinesNearby { get; set; }
        public bool IsMineArea { get; set; }
    }
}
