using System.IO;
using System.Runtime.CompilerServices;
using Android.Graphics;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.particles;
using sharpdungeon.actors;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects;
using sharpdungeon.items;
using sharpdungeon.items.wands;
using sharpdungeon.levels;
using sharpdungeon.levels.features;
using sharpdungeon.plants;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;
using sharpdungeon.windows;
using Camera = pdsharp.noosa.Camera;

namespace sharpdungeon.scenes
{
    public class GameScene : PixelScene
    {
        private const string TxtWelcome = "Welcome to the Level {0} of Pixel Dungeon!";
        private const string TxtWelcomeBack = "Welcome back to the Level {0} of Pixel Dungeon!";
        private const string TxtNightMode = "Be cautious, since the dungeon is even more dangerous at night!";

        private const string TxtChasm = "Your steps echo across the dungeon.";
        private const string TxtWater = "You hear the water splashing around you.";
        private const string TxtGrass = "The smell of vegetation is thick in the air.";
        private const string TxtSecrets = "The atmosphere hints that this floor hides many secrets.";

        internal static GameScene Scene;

        private SkinnedBlock _water;
        private DungeonTilemap _tiles;
        private FogOfWar _fog;
        private HeroSprite _hero;

        private GameLog _log;

        private BusyIndicator _busy;

        private static CellSelector _cellSelector;

        private Group _terrain;
        private Group _ripples;
        private Group _plants;
        private Group _heaps;
        private Group _mobs;
        private Group _emitters;
        private Group _effects;
        private Group _gases;
        private Group _spells;
        private Group _statuses;
        private Group _emoicons;

        private Toolbar _toolbar;
        private Toast _prompt;

        public override void Create()
        {

            Music.Instance.Play(Assets.TUNE, true);
            Music.Instance.Volume(1f);

            PixelDungeon.LastClass(Dungeon.Hero.heroClass.Ordinal());

            base.Create();
            Camera.Main.ZoomTo(defaultZoom + PixelDungeon.Zoom());

            Scene = this;

            _terrain = new Group();
            Add(_terrain);

            _water = new SkinnedBlock(levels.Level.Width * DungeonTilemap.Size, Level.Height * DungeonTilemap.Size, Dungeon.Level.WaterTex());
            _terrain.Add(_water);

            _ripples = new Group();
            _terrain.Add(_ripples);

            _tiles = new DungeonTilemap();
            _terrain.Add(_tiles);

            Dungeon.Level.AddVisuals(this);

            _plants = new Group();
            Add(_plants);

            foreach (var plant in Dungeon.Level.plants.Values)
                AddPlantSprite(plant);

            _heaps = new Group();
            Add(_heaps);

            foreach (var heap in Dungeon.Level.heaps.Values)
                AddHeapSprite(heap);

            _emitters = new Group();
            _effects = new Group();
            _emoicons = new Group();

            _mobs = new Group();
            Add(_mobs);

            foreach (var mob in Dungeon.Level.mobs)
            {
                AddMobSprite(mob);
                if (Statistics.AmuletObtained)
                    mob.Beckon(Dungeon.Hero.pos);
            }

            Add(_emitters);
            Add(_effects);

            _gases = new Group();
            Add(_gases);

            foreach (var blob in Dungeon.Level.Blobs.Values)
            {
                blob.Emitter = null;
                AddBlobSprite(blob);
            }

            _fog = new FogOfWar(Level.Width, Level.Height);
            _fog.UpdateVisibility(Dungeon.Visible, Dungeon.Level.visited, Dungeon.Level.mapped);
            Add(_fog);

            Brightness(PixelDungeon.Brightness());

            _spells = new Group();
            Add(_spells);

            _statuses = new Group();
            Add(_statuses);

            Add(_emoicons);

            _hero = new HeroSprite();
            _hero.Place(Dungeon.Hero.pos);
            _hero.UpdateArmor();
            _mobs.Add(_hero);


            Add(new HealthIndicator());

            Add(_cellSelector = new CellSelector(_tiles));

            var sb = new StatusPane();
            sb.Camera = uiCamera;
            sb.SetSize(uiCamera.CameraWidth, 0);
            Add(sb);

            _toolbar = new Toolbar();
            _toolbar.Camera = uiCamera;
            _toolbar.SetRect(0, uiCamera.CameraHeight - _toolbar.Height, uiCamera.CameraWidth, _toolbar.Height);
            Add(_toolbar);

            var attack = new AttackIndicator();
            attack.Camera = uiCamera;
            attack.SetPos(uiCamera.CameraWidth - attack.Width, _toolbar.Top() - attack.Height);
            Add(attack);

            _log = new GameLog();
            _log.Camera = uiCamera;
            _log.SetRect(0, _toolbar.Top(), attack.Left(), 0);
            Add(_log);

            if (Dungeon.Depth < Statistics.DeepestFloor)
                GLog.Information(TxtWelcomeBack, Dungeon.Depth);
            else
            {
                GLog.Information(TxtWelcome, Dungeon.Depth);
                Sample.Instance.Play(Assets.SND_DESCEND);
            }

            switch (Dungeon.Level.feeling)
            {
                case Level.Feeling.CHASM:
                    GLog.Warning(TxtChasm);
                    break;
                case Level.Feeling.WATER:
                    GLog.Warning(TxtWater);
                    break;
                case Level.Feeling.GRASS:
                    GLog.Warning(TxtGrass);
                    break;
            }

            if (Dungeon.Level is RegularLevel && ((RegularLevel)Dungeon.Level).SecretDoors > pdsharp.utils.Random.IntRange(3, 4))
                GLog.Warning(TxtSecrets);

            if (Dungeon.NightMode && !Dungeon.BossLevel())
                GLog.Warning(TxtNightMode);

            _busy = new BusyIndicator();
            _busy.Camera = uiCamera;
            _busy.X = 1;
            _busy.Y = sb.Bottom() + 1;
            Add(_busy);

            switch (InterlevelScene.mode)
            {
                case InterlevelScene.Mode.RESURRECT:
                    WandOfBlink.Appear(Dungeon.Hero, Dungeon.Level.entrance);
                    new Flare(8, 32).Color(0xFFFF66, true).Show(_hero, 2f);
                    break;
                case InterlevelScene.Mode.RETURN:
                    WandOfBlink.Appear(Dungeon.Hero, Dungeon.Hero.pos);
                    break;
                case InterlevelScene.Mode.FALL:
                    Chasm.HeroLand();
                    break;
                case InterlevelScene.Mode.DESCEND:
                    switch (Dungeon.Depth)
                    {
                        case 1:
                            WndStory.ShowChapter(WndStory.ID_SEWERS);
                            break;
                        case 6:
                            WndStory.ShowChapter(WndStory.ID_PRISON);
                            break;
                        case 11:
                            WndStory.ShowChapter(WndStory.ID_CAVES);
                            break;
                        case 16:
                            WndStory.ShowChapter(WndStory.ID_METROPOLIS);
                            break;
                        case 22:
                            WndStory.ShowChapter(WndStory.ID_HALLS);
                            break;
                    }

                    if (Dungeon.Hero.IsAlive && Dungeon.Depth != 22)
                        Badge.ValidateNoKilling();

                    break;
            }

            Camera.Main.Target = _hero;

            //var m = new string[Level.Width];
            //var b = new StringBuilder();
            //for (var i = 0; i < Level.passable.Length; i++)
            //{
            //    var cx = i % Level.Width;
            //    var cy = i / Level.Width;
            //    if (i == Dungeon.Hero.pos)
            //    {
            //        m[cx] += "H";
            //        continue;
            //    }

            //    if (Level.passable[i])
            //        m[cx] += ".";
            //    else
            //        m[cx] += "#";
            //}
            //foreach (var s in m)
            //    b.AppendLine(s);
            //Debug.WriteLine(b);

            //for (var i = 0; i < Dungeon.Level.mapped.Length; i++)
            //    Dungeon.Level.mapped[i] = true;

            FadeIn();
        }

        public override void Destroy()
        {
            Scene = null;
            Badge.SaveGlobal();

            base.Destroy();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Pause()
        {
            try
            {
                Dungeon.SaveAll();
                Badge.SaveGlobal();
            }
            catch (IOException)
            {

            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Update()
        {
            if (Dungeon.Hero == null)
                return;

            base.Update();

            _water.Offset(0, -5 * Game.Elapsed);

            Actor.Process();

            if (Dungeon.Hero.ready && !Dungeon.Hero.Paralysed)
                _log.NewLine();

            _cellSelector.enabled = Dungeon.Hero.ready;
        }

        protected override void OnBackPressed()
        {
            if (!Cancel())
                Add(new WndGame());
        }

        protected override void OnMenuPressed()
        {
            if (Dungeon.Hero.ready)
                SelectItem(null, WndBag.Mode.ALL, null);
        }

        public virtual void Brightness(bool value)
        {
            _water.Rm = _water.Gm = _water.Bm = _tiles.Rm = _tiles.Gm = _tiles.Bm = value ? 1.5f : 1.0f;
            if (value)
            {
                _fog.Am = +2f;
                _fog.Aa = -1f;
            }
            else
            {
                _fog.Am = +1f;
                _fog.Aa = 0f;
            }
        }

        private void AddHeapSprite(Heap heap)
        {
            var sprite = heap.Sprite = _heaps.Recycle<ItemSprite>();
            sprite.Revive();
            sprite.Link(heap);
            _heaps.Add(sprite);
        }

        private void AddDiscardedSprite(Heap heap)
        {
            heap.Sprite = _heaps.Recycle<DiscardedItemSprite>();
            heap.Sprite.Revive();
            heap.Sprite.Link(heap);
            _heaps.Add(heap.Sprite);
        }

        private void AddPlantSprite(Plant plant)
        {
            var plantSprite = _plants.Recycle<PlantSprite>();
            plantSprite.Reset(plant);
            plant.Sprite = plantSprite;
        }

        private void AddBlobSprite(Blob gas)
        {
            if (gas.Emitter == null)
                _gases.Add(new BlobEmitter(gas));
        }

        private void AddMobSprite(Mob mob)
        {
            var sprite = mob.Sprite;
            sprite.Visible = Dungeon.Visible[mob.pos];
            _mobs.Add(sprite);
            sprite.Link(mob);
        }

        private void Prompt(string text)
        {
            if (_prompt != null)
            {
                _prompt.KillAndErase();
                _prompt = null;
            }

            if (text == null)
                return;

            _prompt = new Toast(text);
            _prompt.CloseAction = CancelAction;
            _prompt.Camera = uiCamera;
            _prompt.SetPos((uiCamera.CameraWidth - _prompt.Width) / 2, uiCamera.CameraHeight - 60);
            Add(_prompt);
        }

        private void CancelAction()
        {
            Cancel();
        }

        private void ShowBanner(Banner banner)
        {
            banner.Camera = uiCamera;
            banner.X = Align(uiCamera, (uiCamera.CameraWidth - banner.Width) / 2);
            banner.Y = Align(uiCamera, (uiCamera.CameraHeight - banner.Height) / 3);
            Add(banner);
        }

        // -------------------------------------------------------

        public static void Add(Plant plant)
        {
            if (Scene != null)
                Scene.AddPlantSprite(plant);
        }

        public static void Add(Blob gas)
        {
            Actor.Add(gas);
            if (Scene != null)
                Scene.AddBlobSprite(gas);
        }

        public static void Add(Heap heap)
        {
            if (Scene != null)
                Scene.AddHeapSprite(heap);
        }

        public static void Discard(Heap heap)
        {
            if (Scene != null)
                Scene.AddDiscardedSprite(heap);
        }

        public static void Add(Mob mob)
        {
            Dungeon.Level.mobs.Add(mob);
            Actor.Add(mob);
            Actor.OccupyCell(mob);
            Scene.AddMobSprite(mob);
        }

        public static void Add(Mob mob, float delay)
        {
            Dungeon.Level.mobs.Add(mob);
            Actor.AddDelayed(mob, delay);
            Actor.OccupyCell(mob);
            Scene.AddMobSprite(mob);
        }

        public static void Add(EmoIcon icon)
        {
            Scene._emoicons.Add(icon);
        }

        public static void Effect(Visual effect)
        {
            Scene._effects.Add(effect);
        }

        public static Ripple Ripple(int pos)
        {
            var ripple = Scene._ripples.Recycle<Ripple>();
            ripple.Reset(pos);
            return ripple;
        }

        public static SpellSprite SpellSprite()
        {
            return Scene._spells.Recycle<SpellSprite>();
        }

        public static Emitter Emitter()
        {
            if (Scene == null)
                return null;

            var emitter = Scene._emitters.Recycle<Emitter>();
            emitter.Revive();
            return emitter;
        }

        public static FloatingText Status()
        {
            return Scene != null ? Scene._statuses.Recycle<FloatingText>() : null;
        }

        public static void PickUp(Item item)
        {
            Scene._toolbar.Pickup(item);
        }

        public static void UpdateMap()
        {
            if (Scene != null)
            {
                Scene._tiles.updated.Set(0, 0, Level.Width, Level.Height);
            }
        }

        public static void UpdateMap(int cell)
        {
            if (Scene != null)
                Scene._tiles.updated.Union(cell % Level.Width, cell / Level.Width);
        }

        public static void DiscoverTile(int pos, int oldValue)
        {
            if (Scene != null)
                Scene._tiles.Discover(pos, oldValue);
        }

        public static void Show(Window wnd)
        {
            CancelCellSelector();
            Scene.Add(wnd);
        }

        public static void AfterObserve()
        {
            if (Scene == null)
                return;

            Scene._fog.UpdateVisibility(Dungeon.Visible, Dungeon.Level.visited, Dungeon.Level.mapped);

            foreach (var mob in Dungeon.Level.mobs)
                mob.Sprite.Visible = Dungeon.Visible[mob.pos];
        }

        public static void Flash(Color color)
        {
            var c = Color.Argb(0xFF, 0x00, 0x00, 0x00) | color;
            Scene.FadeIn(new Color(c), true);
        }

        public static void GameOver()
        {
            Banner gameOver = new Banner(BannerSprites.Get(BannerSprites.Type.GameOver));
            gameOver.Show(0x000000, 1f);
            Scene.ShowBanner(gameOver);

            Sample.Instance.Play(Assets.SND_DEATH);
        }

        public static void BossSlain()
        {
            if (!Dungeon.Hero.IsAlive)
                return;

            Banner bossSlain = new Banner(BannerSprites.Get(BannerSprites.Type.BossSlain));
            bossSlain.Show(0xFFFFFF, 0.3f, 5f);
            Scene.ShowBanner(bossSlain);

            Sample.Instance.Play(Assets.SND_BOSS);
        }

        public static void HandleCell(int cell)
        {
            _cellSelector.Select(cell);
        }

        public static void SelectCell(CellSelector.Listener listener)
        {
            _cellSelector.listener = listener;
            Scene.Prompt(listener.Prompt());
        }

        private static bool CancelCellSelector()
        {
            if (!(_cellSelector.listener is DefaultCellListener))
                return false;

            _cellSelector.Cancel();

            return true;
        }

        public static WndBag SelectItem(WndBag.Listener listener, WndBag.Mode mode, string title)
        {
            CancelCellSelector();

            var wnd = mode == WndBag.Mode.SEED ? WndBag.SeedPouch(listener, mode, title) : WndBag.LastBag(listener, mode, title);
            Scene.Add(wnd);

            return wnd;
        }

        internal static bool Cancel()
        {
            if (Dungeon.Hero.curAction != null || Dungeon.Hero.RestoreHealth)
            {

                Dungeon.Hero.curAction = null;
                Dungeon.Hero.RestoreHealth = false;
                return true;

            }

            return CancelCellSelector();
        }

        public static void Ready()
        {
            SelectCell(new DefaultCellListener());
            QuickSlot.Cancel();
        }

        class DefaultCellListener : CellSelector.Listener
        {
            public void OnSelect(int? target)
            {
                if (target != null && Dungeon.Hero.Handle(target.Value))
                {
                    //Actor.next();
                    Dungeon.Hero.Next();
                }
            }

            string CellSelector.Listener.Prompt()
            {
                return null;
            }
        }
    }
}