using JsonData;
using UniRx.Async;
using UnityEngine.UI;

namespace Luna
{
    public class RecipeLayerItem : Item
    {
        public int ID;
        public ItemJd ItemJd;
        public RecipeJd RecipeJd;

        public override void Init()
        {
            JB = Object.GetComponent<JButton>();
            Icon = Transform.Find("img_icon").GetComponent<Image>();
        }

        /// <summary>
        /// 显示想要显示的个数
        /// </summary>
        /// <param name="_Ijo"></param>
        /// <param name="_Count"></param>
        public void SetItem(RecipeItem mi)
        {
            Object.SetActive(true);

            if (mi == null)
            {
                Icon.gameObject.SetActive(false);
                RecipeJd = null;
                return;
            }

            var iJd = JsonDataMgr.Instance.ItemMap[mi.ItemID];
            RecipeJd = mi.RecipeJd;
            ItemJd = iJd;
            Icon.gameObject.SetActive(true);
        }
    }
}
