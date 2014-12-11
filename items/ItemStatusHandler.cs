using System;
using System.Collections.Generic;
using System.Linq;
using pdsharp.utils;

namespace sharpdungeon.items
{
    public class ItemStatusHandler
    {
        private readonly Type[] _items;

        private readonly Dictionary<Type, int?> _images;

        private readonly Dictionary<Type, string> _labels;

        private readonly HashSet<Type> _known;

        public ItemStatusHandler(Type[] items, IEnumerable<string> allLabels, IEnumerable<int?> allImages)
        {
            _items = items;

            _images = new Dictionary<Type, int?>();

            _labels = new Dictionary<Type, string>();

            _known = new HashSet<Type>();

            var labelsLeft = new List<string>(allLabels);
            var imagesLeft = new List<int?>(allImages);

            foreach (var item in items)
            {
                var index = pdsharp.utils.Random.Int(labelsLeft.Count);

                _labels.Add(item, labelsLeft[index]);
                labelsLeft.RemoveAt(index);

                _images.Add(item, imagesLeft[index]);
                imagesLeft.Remove(index);
            }
        }

        public ItemStatusHandler(Type[] items, string[] labels, int?[] images, Bundle bundle)
        {
            _items = items;

            _images = new Dictionary<Type, int?>();

            _labels = new Dictionary<Type, string>();

            _known = new HashSet<Type>();

            Restore(bundle, labels, images);
        }

        private const string PFX_IMAGE = "_image";
        private const string PFX_LABEL = "_label";
        private const string PFX_KNOWN = "_known";

        public virtual void Save(Bundle bundle)
        {
            foreach (var t in _items)
            {
                var itemName = t.ToString();
                bundle.Put(itemName + PFX_IMAGE, _images[t]);
                bundle.Put(itemName + PFX_LABEL, _labels[t]);
                bundle.Put(itemName + PFX_KNOWN, Known().Contains(t));
            }
        }

        private void Restore(Bundle bundle, string[] allLabels, int?[] allImages)
        {
            var labelsLeft = new List<string>(allLabels);
            var imagesLeft = new List<int?>(allImages);

            foreach (var item in _items)
            {
                var itemName = item.ToString();

                if (bundle.Contains(itemName + PFX_LABEL))
                {
                    var label = bundle.GetString(itemName + PFX_LABEL);
                    _labels.Add(item, label);
                    labelsLeft.Remove(label);

                    var image = bundle.GetInt(itemName + PFX_IMAGE);
                    _images.Add(item, image);
                    imagesLeft.Remove(image);

                    if (bundle.GetBoolean(itemName + PFX_KNOWN))
                        _known.Add(item);
                }
                else
                {
                    var index = pdsharp.utils.Random.Int(labelsLeft.Count);

                    _labels.Add(item, labelsLeft[index]);
                    labelsLeft.RemoveAt(index);

                    _images.Add(item, imagesLeft[index]);
                    imagesLeft.Remove(index);
                }
            }
        }

        public virtual int? Image(Type item)
        {
            return _images[item];
        }

        public virtual string Label(Type item)
        {
            return _labels[item];
        }

        public virtual bool IsKnown(Type item)
        {
            return _known.Contains(item);
        }

        public virtual void Know(Type item)
        {
            _known.Add(item);

            if (Known().Count != _items.Length - 1)
                return;

            if (_items.Any(t => !_known.Contains(item)))
                _known.Add(item);
        }


        public virtual HashSet<Type> Known()
        {
            return new HashSet<Type>(_known.Select(k => k));
        }

        public virtual HashSet<Type> Unknown()
        {
            var result = new HashSet<Type>();

            foreach (var i in _items)
                if (!_known.Contains(i))
                    result.Add(i);

            return result;
        }
    }
}