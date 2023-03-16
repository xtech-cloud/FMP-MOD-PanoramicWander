

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.FMP.MOD.PanoramicWander.LIB.Proto;
using XTC.FMP.MOD.PanoramicWander.LIB.MVCS;
using System.Collections;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace XTC.FMP.MOD.PanoramicWander.LIB.Unity
{
    /// <summary>
    /// 实例类
    /// </summary>
    public class MyInstance : MyInstanceBase
    {
        public class UiReference
        {
            public Transform tDebugPanel;
            public Text textDebugRendererRotationY;
        }

        public class WorldReference
        {
            public Transform tOutRendererAdjust;
            public Transform tOutRenderer;
            public Transform tInRendererAdjust;
            public Transform tInRenderer;
        }

        public class HudReference
        {
            public GameObject objFootMenu;
            public GameObject objHeadMenu;
            public GameObject objFrontMenu;
            public Transform tGoniometer;
            public Transform tEntry;
            public Transform tDebug;
        }

        private UiReference uiReference_ = new UiReference();
        private WorldReference worldReference_ = new WorldReference();
        private HudReference hudReference_ = new HudReference();
        private bool isOpened = false;

        private ArchiveReaderProxy archiveReader_ = null;
        private DummyModel.ArchiveMetaSchema archiveMetaSchema_ = null;
        private Dictionary<string, Texture2D> sceneImageS_ = new Dictionary<string, Texture2D>();
        private List<string> activatedSceneS_ = new List<string>();
        private List<GameObject> entryS_ = new List<GameObject>();
        private Coroutine coroutineSwitchScene_ = null;

        public MyInstance(string _uid, string _style, MyConfig _config, MyCatalog _catalog, LibMVCS.Logger _logger, Dictionary<string, LibMVCS.Any> _settings, MyEntryBase _entry, MonoBehaviour _mono, GameObject _rootAttachments)
            : base(_uid, _style, _config, _catalog, _logger, _settings, _entry, _mono, _rootAttachments)
        {
            archiveReader_ = new ArchiveReaderProxy();
            archiveReader_.logger = _logger;
        }

        /// <summary>
        /// 当被创建时
        /// </summary>
        /// <remarks>
        /// 可用于加载主题目录的数据
        /// </remarks>
        public void HandleCreated()
        {
            Material panoramicMaterial = rootAttachments.transform.Find("PanoramicMaterial").GetComponent<MeshRenderer>().material;
            Material clipPanoramicMaterial = rootAttachments.transform.Find("ClipPanoramicMaterial").GetComponent<MeshRenderer>().material;

            uiReference_.tDebugPanel = rootUI.transform.Find("DebugPanel");
            uiReference_.textDebugRendererRotationY = rootUI.transform.Find("DebugPanel/RendererRotationY/textValue").GetComponent<Text>();

            worldReference_.tOutRendererAdjust = rootWorld.transform.Find("OutRendererAdjust");
            worldReference_.tOutRenderer = rootWorld.transform.Find("OutRendererAdjust/OutRenderer");
            worldReference_.tInRendererAdjust = rootWorld.transform.Find("InRendererAdjust");
            worldReference_.tInRenderer = rootWorld.transform.Find("InRendererAdjust/InRenderer");
            worldReference_.tOutRenderer.GetComponent<MeshRenderer>().material = GameObject.Instantiate(panoramicMaterial);
            worldReference_.tInRenderer.GetComponent<MeshRenderer>().material = GameObject.Instantiate(clipPanoramicMaterial);

            hudReference_.tDebug = rootWorld.transform.Find("Debug");
            hudReference_.objHeadMenu = rootWorld.transform.Find("HeadMenu").gameObject;
            hudReference_.objFootMenu = rootWorld.transform.Find("FootMenu").gameObject;
            hudReference_.objFrontMenu = rootWorld.transform.Find("FrontMenu").gameObject;
            hudReference_.tGoniometer = rootWorld.transform.Find("FootMenu/Canvas/Goniometer");
            hudReference_.tEntry = hudReference_.objFrontMenu.transform.Find("Canvas/Panel/entryPanel/entry");
            hudReference_.tEntry.gameObject.SetActive(false);

            loadTextureFromTheme("Goniometer_Indicator.png", (_texture) =>
            {
                hudReference_.tGoniometer.Find("indicator").GetComponent<RawImage>().texture = _texture;
            }, () => { });

            var canvasHead = hudReference_.objHeadMenu.transform.Find("Canvas");
            var canvasFoot = hudReference_.objFootMenu.transform.Find("Canvas");
            var canvasFront = hudReference_.objFrontMenu.transform.Find("Canvas");
            // 设置菜单的缩放
            canvasHead.localScale = style_.headMenu.scale * Vector3.one;
            canvasFoot.localScale = style_.footMenu.scale * Vector3.one;
            canvasFront.localScale = style_.frontMenu.scale * Vector3.one;
            // 设置菜单的位置
            Vector3 footMenuOffset = new Vector3(0, -style_.footMenu.offset, 0);
            Vector3 headMenuOffset = new Vector3(0, style_.headMenu.offset, 0);
            Vector3 frontMenuOffset = new Vector3(0, 0, style_.frontMenu.offset);
            canvasHead.position = Camera.main.transform.position + footMenuOffset;
            canvasFoot.position = Camera.main.transform.position + headMenuOffset;
            canvasFront.position = Camera.main.transform.position + frontMenuOffset;

            hudReference_.objHeadMenu.SetActive(style_.headMenu.visible);
            hudReference_.objFootMenu.SetActive(style_.footMenu.visible);
            hudReference_.objFrontMenu.SetActive(style_.frontMenu.visible);

            uiReference_.tDebugPanel.gameObject.SetActive(style_.debug.active);
            hudReference_.tDebug.gameObject.SetActive(style_.debug.active);
        }

        /// <summary>
        /// 当被删除时
        /// </summary>
        public void HandleDeleted()
        {
        }

        /// <summary>
        /// 当被打开时
        /// </summary>
        /// <remarks>
        /// 可用于加载内容目录的数据
        /// </remarks>
        public void HandleOpened(string _source, string _uri)
        {
            isOpened = true;

            rootUI.gameObject.SetActive(true);
            rootWorld.gameObject.SetActive(true);

            mono_.StartCoroutine(updateTick());

            // HUD跟随摄像机
            mono_.StartCoroutine(open(_source, _uri));
        }

        /// <summary>
        /// 当被关闭时
        /// </summary>
        public void HandleClosed()
        {
            isOpened = false;
            rootUI.gameObject.SetActive(false);
            rootWorld.gameObject.SetActive(false);

            close();
        }

        private IEnumerator updateTick()
        {
            RectTransform rtFrontPanel = hudReference_.objFrontMenu.transform.Find("Canvas/Panel").GetComponent<RectTransform>();
            Transform tIndicator = hudReference_.tGoniometer.Find("indicator");
            while (isOpened)
            {
                yield return new WaitForEndOfFrame();
                tIndicator.rotation = Camera.main.transform.rotation;
                // 前向菜单开合控制
                {
                    // 计算夹角
                    float angleCamera = Camera.main.transform.rotation.eulerAngles.y;
                    float angleFrontMenu = hudReference_.objFrontMenu.transform.rotation.eulerAngles.y;
                    var angle = Mathf.Abs(angleCamera - angleFrontMenu);
                    int width = (int)Mathf.Lerp(style_.frontMenu.width, 0, (angle - style_.frontMenu.expand) / (style_.frontMenu.collapse - style_.frontMenu.expand));
                    var sizeDelta = rtFrontPanel.sizeDelta;
                    sizeDelta.x = width;
                    rtFrontPanel.sizeDelta = sizeDelta;
                }

                if (style_.debug.active)
                {
                    var eulerAngles = worldReference_.tInRendererAdjust.localRotation.eulerAngles;
                    if (Input.GetKey(KeyCode.A))
                    {
                        eulerAngles.y += 0.5f;
                        worldReference_.tInRendererAdjust.localRotation = Quaternion.Euler(eulerAngles);
                    }
                    if (Input.GetKey(KeyCode.D))
                    {
                        eulerAngles.y -= 0.5f;
                        worldReference_.tInRendererAdjust.localRotation = Quaternion.Euler(eulerAngles);
                    }
                    uiReference_.textDebugRendererRotationY.text = ((int)eulerAngles.y).ToString();
                }
            }
        }

        private IEnumerator open(string _source, string _file)
        {
            yield return new WaitForEndOfFrame();

            string uri = _file;
            if (_source == "assloud://")
            {
                uri = Path.Combine(settings_["path.assets"].AsString(), _file);
            }
            archiveReader_.Open(uri);

            byte[] metaBytes = archiveReader_.ReadBytes("meta.json");
            archiveMetaSchema_ = JsonConvert.DeserializeObject<DummyModel.ArchiveMetaSchema>(Encoding.UTF8.GetString(metaBytes));
            foreach (var scene in archiveMetaSchema_.sceneS)
            {
                //TODO 使用异步加载
                sceneImageS_[scene.image] = archiveReader_.ReadTexture(scene.image, TextureFormat.RGBA32);
                if (1 == sceneImageS_.Count)
                {
                    coroutineSwitchScene_ = mono_.StartCoroutine(switchScene(scene.name));
                }
            }
        }

        private void close()
        {
            if (null != coroutineSwitchScene_)
            {
                mono_.StopCoroutine(coroutineSwitchScene_);
                coroutineSwitchScene_ = null;
            }
            archiveReader_.Close();
        }

        private IEnumerator switchScene(string _name)
        {
            var sceneS = (from scene in archiveMetaSchema_.sceneS where scene.name == _name select scene).ToList();
            if (null == sceneS || sceneS.Count == 0)
            {
                logger_.Error("scene {0} not found", _name);
                yield break;
            }

            var targetScene = sceneS[0];
            if (!activatedSceneS_.Contains(_name))
                activatedSceneS_.Add(_name);

            // 关闭前向菜单
            hudReference_.objFrontMenu.SetActive(false);
            // 刷新前向菜单
            hudReference_.objFrontMenu.transform.localRotation = Quaternion.Euler(0, targetScene.frontMenuAngle, 0);
            hudReference_.objFrontMenu.transform.Find("Canvas/Panel/title/txtName").GetComponent<Text>().text = targetScene.name;
            foreach (var objEntry in entryS_)
            {
                GameObject.Destroy(objEntry);
            }
            entryS_.Clear();
            Color normalColor = Color.white;
            ColorUtility.TryParseHtmlString(style_.hotspot.linkNormalColor, out normalColor);
            Color activatedColor = Color.white;
            ColorUtility.TryParseHtmlString(style_.hotspot.linkActivatedColor, out activatedColor);
            foreach (var hotspot in targetScene.hotspotS)
            {
                GameObject clone = GameObject.Instantiate(hudReference_.tEntry.gameObject, hudReference_.tEntry.parent);
                entryS_.Add(clone);
                var text = clone.transform.Find("text").GetComponent<Text>();
                text.text = hotspot.link;
                text.color = activatedSceneS_.Contains(hotspot.link) ? activatedColor : normalColor;
                clone.gameObject.SetActive(true);
                clone.GetComponent<Button>().onClick.AddListener(() =>
                {
                    mono_.StartCoroutine(switchScene(hotspot.link));
                });
            }

            // 执行的切换动画
            yield return doClipEffect(targetScene);

            // 恢复前向菜单
            hudReference_.objFrontMenu.SetActive(style_.frontMenu.visible);

            coroutineSwitchScene_ = null;
        }

        IEnumerator doClipEffect(DummyModel.ArchiveMetaSchema.Scene _targetScene)
        {
            // 将外球设置为目标场景图
            worldReference_.tOutRenderer.GetComponent<MeshRenderer>().material.mainTexture = sceneImageS_[_targetScene.image];
            worldReference_.tOutRendererAdjust.localRotation = Quaternion.Euler(0, _targetScene.rotation, 0);
            yield return new WaitForEndOfFrame();

            // 执行内球的切换动画，从有到无
            var inRendererMaterail = worldReference_.tInRenderer.GetComponent<MeshRenderer>().material;
            float timer = 0;
            float posTop = worldReference_.tInRenderer.position.y + 110;
            float posBottom = worldReference_.tInRenderer.position.y - 110;
            while (timer < style_.switchEffect.duration)
            {
                float value = Mathf.Lerp(posTop, posBottom, timer / style_.switchEffect.duration);
                inRendererMaterail.SetFloat("_Clip", value);

                yield return new WaitForEndOfFrame();
                timer += Time.deltaTime;
            }
            // 结束后将内球设置为目标场景图，并完全显示遮盖外球
            worldReference_.tInRenderer.GetComponent<MeshRenderer>().material.mainTexture = sceneImageS_[_targetScene.image];
            worldReference_.tInRendererAdjust.localRotation = Quaternion.Euler(0, _targetScene.rotation, 0);
            inRendererMaterail.SetFloat("_Clip", posTop);
        }
    }
}
