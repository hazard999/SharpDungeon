using System.IO;
using Java.IO;
using Org.Json;
using System;
using System.Collections.Generic;
using Enum = System.Enum;
using Exception = System.Exception;
using IOException = Java.IO.IOException;

namespace pdsharp.utils
{
    public class Bundle
    {
        private const string CLASS_NAME = "__className";

        private static readonly Dictionary<string, string> Aliases = new Dictionary<string, string>();

        private readonly JSONObject _data;

        public Bundle()
            : this(new JSONObject())
        {
        }

        public override string ToString()
        {
            return _data.ToString();
        }

        private Bundle(JSONObject data)
        {
            this._data = data;
        }

        public virtual bool IsNull
        {
            get
            {
                return _data == null;
            }
        }

        public virtual bool Contains(string key)
        {
            return !_data.IsNull(key);
        }

        public virtual bool GetBoolean(string key)
        {
            return _data.OptBoolean(key);
        }

        public virtual int GetInt(string key)
        {
            return _data.OptInt(key);
        }

        public virtual float GetFloat(string key)
        {
            return (float)_data.OptDouble(key);
        }

        public virtual string GetString(string key)
        {
            return _data.OptString(key);
        }

        public virtual Bundle GetBundle(string key)
        {
            return new Bundle(_data.OptJSONObject(key));
        }

        private Bundlable Get()
        {
            try
            {
                var clName = GetString(CLASS_NAME);
                if (Aliases.ContainsKey(clName))
                    clName = Aliases[clName];

                var cl = Type.GetType(clName);
                if (cl == null)
                    return null;

                var bundlable = (Bundlable)Activator.CreateInstance(cl);
                bundlable.RestoreFromBundle(this);
                return bundlable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public virtual Bundlable Get(string key)
        {
            return GetBundle(key).Get();
        }

        public virtual E GetEnum<E>(string key) where E : struct
        {
            try
            {
                return (E)Enum.Parse(typeof(E), _data.GetString(key));
            }
            catch (JSONException)
            {
                return default(E);

                //TODO: FIX enumClass.EnumConstants[0];
                //return enumClass.EnumConstants[0];
            }
        }

        public virtual int[] GetIntArray(string key)
        {
            try
            {
                JSONArray array = _data.GetJSONArray(key);
                int length = array.Length();
                int[] result = new int[length];
                for (int i = 0; i < length; i++)
                {
                    result[i] = array.GetInt(i);
                }
                return result;
            }
            catch (JSONException)
            {
                return null;
            }
        }

        public virtual bool[] GetBooleanArray(string key)
        {
            try
            {
                JSONArray array = _data.GetJSONArray(key);
                int length = array.Length();
                bool[] result = new bool[length];
                for (int i = 0; i < length; i++)
                {
                    result[i] = array.GetBoolean(i);
                }
                return result;
            }
            catch (JSONException)
            {
                return null;
            }
        }

        public virtual string[] GetStringArray(string key)
        {
            try
            {
                JSONArray array = _data.GetJSONArray(key);
                int length = array.Length();
                string[] result = new string[length];
                for (int i = 0; i < length; i++)
                {
                    result[i] = array.GetString(i);
                }
                return result;
            }
            catch (JSONException)
            {
                return null;
            }
        }

        public virtual ICollection<Bundlable> GetCollection(string key)
        {

            var list = new List<Bundlable>();

            try
            {
                var array = _data.GetJSONArray(key);
                for (int i = 0; i < array.Length(); i++)
                {
                    list.Add(new Bundle(array.GetJSONObject(i)).Get());
                }
            }
            catch (JSONException)
            {

            }

            return list;
        }

        public virtual void put(string key, bool value)
        {
            try
            {
                _data.Put(key, value);
            }
            catch (JSONException)
            {

            }
        }

        public virtual void put(string key, int value)
        {
            try
            {
                _data.Put(key, value);
            }
            catch (JSONException)
            {

            }
        }

        public virtual void put(string key, float value)
        {
            try
            {
                _data.Put(key, value);
            }
            catch (JSONException)
            {

            }
        }

        public virtual void put(string key, string value)
        {
            try
            {
                _data.Put(key, value);
            }
            catch (JSONException)
            {

            }
        }

        public virtual void Put(string key, Bundle bundle)
        {
            try
            {
                _data.Put(key, bundle._data);
            }
            catch (JSONException)
            {

            }
        }

        public virtual void Put(string key, Bundlable bundlable)
        {
            if (bundlable == null)
                return;

            try
            {
                var bundle = new Bundle();
                bundle.Put(CLASS_NAME, bundlable.GetType().Name);
                bundlable.StoreInBundle(bundle);
                _data.Put(key, bundle._data);
            }
            catch (JSONException)
            {
            }
        }

        public virtual void Put<T1>(string key, T1 value)
        {
            if (value != null)
            {
                try
                {
                    _data.Put(key, value.ToString());
                }
                catch (JSONException)
                {
                }
            }
        }

        public virtual void put(string key, int[] array)
        {
            try
            {
                JSONArray jsonArray = new JSONArray();
                for (int i = 0; i < array.Length; i++)
                {
                    jsonArray.Put(i, array[i]);
                }
                _data.Put(key, jsonArray);
            }
            catch (JSONException)
            {

            }
        }

        public virtual void Put(string key, bool[] array)
        {
            try
            {
                JSONArray jsonArray = new JSONArray();
                for (int i = 0; i < array.Length; i++)
                {
                    jsonArray.Put(i, array[i]);
                }
                _data.Put(key, jsonArray);
            }
            catch (JSONException)
            {

            }
        }

        public virtual void put(string key, string[] array)
        {
            try
            {
                JSONArray jsonArray = new JSONArray();
                for (int i = 0; i < array.Length; i++)
                {
                    jsonArray.Put(i, array[i]);
                }
                _data.Put(key, jsonArray);
            }
            catch (JSONException)
            {

            }
        }

        public virtual void put<T1>(string key, ICollection<T1> collection) where T1 : Bundlable
        {
            var array = new JSONArray();
            foreach (var obj in collection)
            {
                var bundle = new Bundle();
                bundle.Put(CLASS_NAME, obj.GetType().Name);
                obj.StoreInBundle(bundle);
                array.Put(bundle._data);
            }
            try
            {
                _data.Put(key, array);
            }
            catch (JSONException)
            {

            }
        }

        public static Bundle Read(Stream stream)
        {

            try
            {
                var reader = new BufferedReader(new InputStreamReader(stream));
                var json = (JSONObject)new JSONTokener(reader.ReadLine()).NextValue();
                reader.Close();

                return new Bundle(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Bundle Read(byte[] bytes)
        {
            try
            {

                var json = (JSONObject)new JSONTokener(System.Text.Encoding.UTF8.GetString(bytes)).NextValue();
                return new Bundle(json);

            }
            catch (JSONException)
            {
                return null;
            }
        }

        public static bool Write(Bundle bundle, Stream stream)
        {
            try
            {
                var writer = new BufferedWriter(new OutputStreamWriter(stream));
                writer.Write(bundle._data.ToString());
                writer.Close();

                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public static void AddAlias<T1>(T1 cl, string alias)
        {
            Aliases.Add(alias, typeof(T1).Name);
        }
    }

}