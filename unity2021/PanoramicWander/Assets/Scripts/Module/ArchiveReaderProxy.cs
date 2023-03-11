using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.oelArchive;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace XTC.FMP.MOD.PanoramicWander.LIB.Unity
{
    /// <summary>
    /// 归档读取器代理类
    /// </summary>
    public class ArchiveReaderProxy
    {
        public class PreReadCallback
        {
            public System.Action onSuccess;
            public System.Action onFailure;
        }

        public LibMVCS.Logger logger { get; set; }
        private string archiveUri { get; set; }

        public PreReadCallback NewPreReadCallback()
        {
            return new PreReadCallback();
        }

        /// <summary>
        /// 加载到内存中的对象
        /// </summary>
        /// <remarks>
        /// key: 文件的entry
        /// </remarks>
        private Dictionary<string, Object> objects_ = new Dictionary<string, Object>();

        /// <summary>
        /// 文件型归档读取器
        /// </summary>
        private FileReader reader_ = null;

        private List<CancellationTokenSource> tokenSourceS_ = new List<CancellationTokenSource>();

        public void Open(string _uri)
        {
            archiveUri = _uri;
            if (!archiveUri.EndsWith("#"))
            {
                reader_ = new FileReader();
                if (!File.Exists(archiveUri))
                    logger.Error("{0} not found ", archiveUri);
            }
            else
            {
                if (!Directory.Exists(archiveUri))
                    logger.Error("{0} not found ", archiveUri);
            }
        }

        public void Close()
        {
            logger.Trace("ready to cancel {0} tasks", tokenSourceS_.Count);
            foreach (var tokenSource in tokenSourceS_)
            {
                tokenSource.Cancel();
            }
            tokenSourceS_.Clear();

            if (null != reader_)
            {
                reader_.Close();
                reader_ = null;
            }
            logger.Trace("ready to destroy {0} objects from memory", objects_.Count);
            foreach (var obj in objects_.Values)
            {
                Object.Destroy(obj);
            }
            objects_.Clear();
            printObjectsStatus();
        }

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="_entry"></param>
        /// <returns></returns>
        public byte[] ReadBytes(string _entry)
        {
            if (null == reader_)
            {
                string path = Path.Combine(archiveUri, _entry);
                if (!File.Exists(path))
                {
                    logger.Error("{0} not found in directory", _entry);
                    return null;
                }
                return File.ReadAllBytes(path);
            }
            else
            {
                if (!reader_.entries.Contains(_entry))
                {
                    logger.Error("{0} not found in archive", _entry);
                    return null;
                }
                return reader_.Read(_entry);
            }
        }

        public Texture2D ReadTexture(string _entry, TextureFormat _textureFormat)
        {
            Object obj = null;
            if (objects_.TryGetValue(_entry, out obj))
            {
                return obj as Texture2D;
            }

            byte[] bytes = null;
            if (null == reader_)
            {
                string path = Path.Combine(archiveUri, _entry);
                if (!File.Exists(path))
                {
                    logger.Error("{0} not found in directory", _entry);
                    return null;
                }
                bytes = File.ReadAllBytes(path);
            }
            else
            {
                if (!reader_.entries.Contains(_entry))
                {
                    logger.Error("{0} not found in archive", _entry);
                    return null;
                }
                bytes = reader_.Read(_entry);
            }

            var texture = new Texture2D(0, 0, _textureFormat, false);
            texture.LoadImage(bytes);
            objects_[_entry] = texture;
            printObjectsStatus();
            return texture;
        }

        public Sprite CreateSprite(Texture2D _texture, Rect _rect, Vector4 _border)
        {
            var sprite = Sprite.Create(_texture, _rect, new Vector2(0.5f, 0.5f), 100, 1, SpriteMeshType.Tight, _border);
            return sprite;
        }

        private void printObjectsStatus()
        {
            logger.Trace("current archive has {0} Objects loaded in memory", objects_.Count);
        }
    }
}
