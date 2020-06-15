using Google.Protobuf;
using JsonData;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;

namespace Luna
{


    public class WorkHeroItem : PoolUnit
    {
        public int HeroID;
        public JButton JB;

        public PAvatar CData;
        public CharacterJd CJd;

        public GameObject _goHave;
        private GameObject _goNull;
        private Image _imgIcon;
        private Text _texName;

        public override void Init()
        {
            JB = Object.GetComponent<JButton>();
            _goHave = Transform.Find("Have").gameObject;
            _goNull = Transform.Find("img_Null").gameObject;
            _imgIcon = Transform.Find("Have/_imgIcon").GetComponent<Image>();
            if (Transform.Find("Have/_texName") != null)
            {
                _texName = Transform.Find("Have/_texName").GetComponent<Text>();
            }
        }

        public void SetHero(CharacterJd characterJd)
        {
            SetNull(false);

            HeroID = characterJd.ID;

            CJd = characterJd;
            CData = CharDataMgr.Instance.CharMap[HeroID];

            if (_texName != null)
            {
                _texName.text = CJd.Name;
            }
            

            SetIcon();
        }

        public void SetNull(bool ishow)
        {
            HeroID = 0;

            _goHave.SetActive(!ishow);
            _goNull.SetActive(ishow);
        }

        private async UniTask SetIcon()
        {
            var pname = $"{CData.Id}_{CData.SkinId}/{HeroID}_{CData.SkinId}_sc";
            _imgIcon.sprite = await AssetMgr.Instance.LoadSprite(
                AssetMgr.Character_PATH, pname);
            _imgIcon.gameObject.SetActive(true);
        }
    }

}