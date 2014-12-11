using pdsharp.noosa;
using System;
using Android.Util;
using pdsharp.noosa.particles;
using pdsharp.utils;
using System.Collections.Generic;

namespace sharpdungeon.effects
{
    public class Speck : Image
    {
        public const int HEALING = 0;
        public const int STAR = 1;
        public const int LIGHT = 2;
        public const int QUESTION = 3;
        public const int UP = 4;
        public const int SCREAM = 5;
        public const int BONE = 6;
        public const int WOOL = 7;
        public const int ROCK = 8;
        public const int NOTE = 9;
        public const int CHANGE = 10;
        public const int HEART = 11;
        public const int BUBBLE = 12;
        public const int STEAM = 13;
        public const int COIN = 14;

        public const int DISCOVER = 101;
        public const int EVOKE = 102;
        public const int MASTERY = 103;
        public const int KIT = 104;
        public const int RATTLE = 105;
        public const int JET = 106;
        public const int TOXIC = 107;
        public const int PARALYSIS = 108;
        public const int DUST = 109;
        public const int FORGE = 110;
        public const int CONFUSION = 111;

        private const int Size = 7;

        private int _type;
        private float _lifespan;
        private float _left;

        private static TextureFilm _film;

        private static readonly Dictionary<int, Emitter.Factory> Factories = new Dictionary<int, Emitter.Factory>();

        public Speck()
        {
            Texture(Assets.SPECKS);

            if (_film == null)
                _film = new TextureFilm(texture, Size, Size);

            Origin.Set(Size / 2f);
        }

        public virtual void Reset(int index, float x, float y, int type)
        {
            Revive();

            _type = type;
            switch (type)
            {
                case DISCOVER:
                    Frame(_film.Get(LIGHT));
                    break;
                case EVOKE:
                case MASTERY:
                case KIT:
                case FORGE:
                    Frame(_film.Get(STAR));
                    break;
                case RATTLE:
                    Frame(_film.Get(BONE));
                    break;
                case JET:
                case TOXIC:
                case PARALYSIS:
                case CONFUSION:
                case DUST:
                    Frame(_film.Get(STEAM));
                    break;
                default:
                    Frame(_film.Get(type));
                    break;
            }

            X = x - Origin.X;
            Y = y - Origin.Y;

            ResetColor();
            Scale.Set(1);
            Speed.Set(0);
            Acc.Set(0);
            Angle = 0;
            AngularSpeed = 0;

            switch (type)
            {
                case HEALING:
                    Speed.Set(0, -20);
                    _lifespan = 1f;
                    break;
                case STAR:
                    Speed.Polar(pdsharp.utils.Random.Float(2 * 3.1415926f), pdsharp.utils.Random.Float(128));
                    Acc.Set(0, 128);
                    Angle = pdsharp.utils.Random.Float(360);
                    AngularSpeed = pdsharp.utils.Random.Float(-360, +360);
                    _lifespan = 1f;
                    break;
                case FORGE:
                    Speed.Polar(pdsharp.utils.Random.Float(-3.1415926f, 0), pdsharp.utils.Random.Float(64));
                    Acc.Set(0, 128);
                    Angle = pdsharp.utils.Random.Float(360);
                    AngularSpeed = pdsharp.utils.Random.Float(-360, +360);
                    _lifespan = 0.51f;
                    break;
                case EVOKE:
                    Speed.Polar(pdsharp.utils.Random.Float(-3.1415926f, 0), 50);
                    Acc.Set(0, 50);
                    Angle = pdsharp.utils.Random.Float(360);
                    AngularSpeed = pdsharp.utils.Random.Float(-180, +180);
                    _lifespan = 1f;
                    break;
                case KIT:
                    Speed.Polar(index * 3.1415926f / 5, 50);
                    Acc.Set(-Speed.X, -Speed.Y);
                    Angle = index * 36;
                    AngularSpeed = 360;
                    _lifespan = 1f;
                    break;
                case MASTERY:
                    Speed.Set(pdsharp.utils.Random.Int(2) == 0 ? pdsharp.utils.Random.Float(-128, -64) : pdsharp.utils.Random.Float(+64, +128), 0);
                    AngularSpeed = Speed.X < 0 ? -180 : +180;
                    Acc.Set(-Speed.X, 0);
                    _lifespan = 0.5f;
                    break;
                case LIGHT:
                    Angle = pdsharp.utils.Random.Float(360);
                    AngularSpeed = 90;
                    _lifespan = 1f;
                    break;
                case DISCOVER:
                    Angle = pdsharp.utils.Random.Float(360);
                    AngularSpeed = 90;
                    _lifespan = 0.5f;
                    Am = 0;
                    break;
                case QUESTION:
                    _lifespan = 0.8f;
                    break;
                case UP:
                    Speed.Set(0, -20);
                    _lifespan = 1f;
                    break;
                case SCREAM:
                    _lifespan = 0.9f;
                    break;
                case BONE:
                    _lifespan = 0.2f;
                    Speed.Polar(pdsharp.utils.Random.Float(2 * 3.1415926f), 24 / _lifespan);
                    Acc.Set(0, 128);
                    Angle = pdsharp.utils.Random.Float(360);
                    AngularSpeed = 360;
                    break;
                case RATTLE:
                    _lifespan = 0.5f;
                    Speed.Set(0, -200);
                    Acc.Set(0, -2 * Speed.Y / _lifespan);
                    Angle = pdsharp.utils.Random.Float(360);
                    AngularSpeed = 360;
                    break;
                case WOOL:
                    _lifespan = 0.5f;
                    Speed.Set(0, -50);
                    Angle = pdsharp.utils.Random.Float(360);
                    AngularSpeed = pdsharp.utils.Random.Float(-360, +360);
                    break;
                case ROCK:
                    Angle = pdsharp.utils.Random.Float(360);
                    AngularSpeed = pdsharp.utils.Random.Float(-360, +360);
                    Scale.Set(pdsharp.utils.Random.Float(1, 2));
                    Speed.Set(0, 64);
                    _lifespan = 0.2f;
                    Y -= Speed.Y * _lifespan;
                    break;
                case NOTE:
                    AngularSpeed = pdsharp.utils.Random.Float(-30, +30);
                    Speed.Polar((AngularSpeed - 90) * PointF.G2R, 30);
                    _lifespan = 1f;
                    break;

                case CHANGE:
                    Angle = pdsharp.utils.Random.Float(360);
                    Speed.Polar((Angle - 90) * PointF.G2R, pdsharp.utils.Random.Float(4, 12));
                    _lifespan = 1.5f;
                    break;
                case HEART:
                    Speed.Set(pdsharp.utils.Random.Int(-10, +10), -40);
                    AngularSpeed = pdsharp.utils.Random.Float(-45, +45);
                    _lifespan = 1f;
                    break;
                case BUBBLE:
                    Speed.Set(0, -15);
                    Scale.Set(pdsharp.utils.Random.Float(0.8f, 1));
                    _lifespan = pdsharp.utils.Random.Float(0.8f, 1.5f);
                    break;
                case STEAM:
                    Speed.Y = -pdsharp.utils.Random.Float(20, 30);
                    AngularSpeed = pdsharp.utils.Random.Float(+180);
                    Angle = pdsharp.utils.Random.Float(360);
                    _lifespan = 1f;
                    break;
                case JET:
                    Speed.Y = +32;
                    Acc.Y = -64;
                    AngularSpeed = pdsharp.utils.Random.Float(180, 360);
                    Angle = pdsharp.utils.Random.Float(360);
                    _lifespan = 0.5f;
                    break;
                case TOXIC:
                    Hardlight(0x50FF60);
                    AngularSpeed = 30;
                    Angle = pdsharp.utils.Random.Float(360);
                    _lifespan = pdsharp.utils.Random.Float(1f, 3f);
                    break;
                case PARALYSIS:
                    Hardlight(0xFFFF66);
                    AngularSpeed = -30;
                    Angle = pdsharp.utils.Random.Float(360);
                    _lifespan = pdsharp.utils.Random.Float(1f, 3f);
                    break;
                case CONFUSION:
                    Hardlight(pdsharp.utils.Random.Int(0x1000000) | 0x000080);
                    AngularSpeed = pdsharp.utils.Random.Float(-20, +20);
                    Angle = pdsharp.utils.Random.Float(360);
                    _lifespan = pdsharp.utils.Random.Float(1f, 3f);
                    break;
                case DUST:
                    Hardlight(0xFFFF66);
                    Angle = pdsharp.utils.Random.Float(360);
                    Speed.Polar(pdsharp.utils.Random.Float(2 * 3.1415926f), pdsharp.utils.Random.Float(16, 48));
                    _lifespan = 0.5f;
                    break;
                case COIN:
                    Speed.Polar(-PointF.Pi * pdsharp.utils.Random.Float(0.3f, 0.7f), pdsharp.utils.Random.Float(48, 96));
                    Acc.Y = 256;
                    _lifespan = -Speed.Y / Acc.Y * 2;
                    break;
            }

            _left = _lifespan;
        }

        public override void Update()
        {
            base.Update();

            _left -= Game.Elapsed;
            if (_left <= 0)
                Kill();
            else
            {
                float p = 1 - _left / _lifespan; // 0 -> 1

                switch (_type)
                {
                    case STAR:
                    case FORGE:
                        Scale.Set(1 - p);
                        Am = p < 0.2f ? p * 5f : (1 - p) * 1.25f;
                        break;
                    case KIT:
                    case MASTERY:
                        Am = 1 - p * p;
                        break;
                    case EVOKE:
                    case HEALING:
                        Am = p < 0.5f ? 1 : 2 - p * 2;
                        break;
                    case LIGHT:
                        Am = Scale.Set(p < 0.2f ? p * 5f : (1 - p) * 1.25f).X;
                        break;
                    case DISCOVER:
                        Am = 1 - p;
                        Scale.Set((p < 0.5f ? p : 1 - p) * 2);
                        break;
                    case QUESTION:
                        Scale.Set((float)(Math.Sqrt(p < 0.5f ? p : 1 - p) * 3));
                        break;
                    case UP:
                        Scale.Set((float)(Math.Sqrt(p < 0.5f ? p : 1 - p) * 2));
                        break;
                    case SCREAM:
                        Am = (float)Math.Sqrt((p < 0.5f ? p : 1 - p) * 2f);
                        Scale.Set(p * 7);
                        break;
                    case BONE:
                    case RATTLE:
                        Am = p < 0.9f ? 1 : (1 - p) * 10;
                        break;
                    case ROCK:
                        Am = p < 0.2f ? p * 5 : 1;
                        break;
                    case NOTE:
                        Am = 1 - p * p;
                        break;
                    case WOOL:
                        Scale.Set(1 - p);
                        break;
                    case CHANGE:
                        Am = FloatMath.Sqrt((p < 0.5f ? p : 1 - p) * 2);
                        Scale.Y = (1 + p) * 0.5f;
                        Scale.X = Scale.Y * FloatMath.Cos(_left * 15);
                        break;
                    case HEART:
                        Scale.Set(1 - p);
                        Am = 1 - p * p;
                        break;
                    case BUBBLE:
                        Am = p < 0.2f ? p * 5 : 1;
                        break;
                    case STEAM:
                    case TOXIC:
                    case PARALYSIS:
                    case CONFUSION:
                    case DUST:
                        Am = p < 0.5f ? p : 1 - p;
                        Scale.Set(1 + p * 2);
                        break;
                    case JET:
                        Am = (p < 0.5f ? p : 1 - p) * 2;
                        Scale.Set(p * 1.5f);
                        break;
                    case COIN:
                        Scale.X = FloatMath.Cos(_left * 5);
                        Rm = Gm = Bm = (Math.Abs(Scale.X) + 1) * 0.5f;
                        Am = p < 0.9f ? 1 : (1 - p) * 10;
                        break;
                }
            }
        }

        public static Emitter.Factory Factory(int type)
        {
            return Factory(type, false);
        }

        public static Emitter.Factory Factory(int type, bool lightMode)
        {
            var factory = GetFactoryFromFactories(type);

            if (factory != null)
                return factory;

            factory = new SpeckFactory(type, lightMode);
            Factories.Add(type, factory);

            return factory;
        }

        private static Emitter.Factory GetFactoryFromFactories(int type)
        {
            if (!Factories.ContainsKey(type))
                return null;

            var factory = Factories[type];
            return factory;
        }
    }

    public class SpeckFactory : Emitter.Factory
    {
        private readonly int _type;
        private readonly bool _lightMode;

        public SpeckFactory(int type, bool lightMode)
        {
            _type = type;
            _lightMode = lightMode;
        }

        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            var positive = emitter.Recycle<Speck>();
            positive.Reset(index, x, y, _type);
        }

        public override bool LightMode()
        {
            return _lightMode;
        }
    }
}