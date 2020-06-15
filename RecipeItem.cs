using JsonData;
using Luna;
using Google.Protobuf;
using UniRx.Async;
using UnityEngine.UI;

public class RecipeItem : Item
{
    public int ItemID;
    public ItemJd ItemJd;
    public RecipeJd RecipeJd;
    public int WorkDoneNum;

    public override void Init()
    {
        JB = Object.GetComponent<JButton>();
        Icon = Transform.Find("img_icon").GetComponent<Image>();
        if (Transform.Find("tex_count") != null)
        {
            Count = Transform.Find("tex_count").GetComponent<Text>();
        }

        if (Transform.Find("tex_name") != null)
        {
            Name = Transform.Find("tex_name").GetComponent<Text>();
        }
    }

    /// <summary>
    /// 显示想要显示的个数
    /// </summary>
    /// <param name="_Ijo"></param>
    /// <param name="_Count"></param>
    public void SetItemAsync(int id, int _Count)
    {
        Object.SetActive(true);

        JsonDataMgr.Instance.RecipeMap.TryGetValue(id, out RecipeJd);

        JsonDataMgr.Instance.ItemMap.TryGetValue(id,out ItemJd);

        ItemID = ItemJd.ID;

        if (Count != null)
        {
            Count.text = _Count.ToString();
        }

        if (Name != null)
        {
            Name.text = ItemJd.Name;
        }
    }

    public async UniTask SetWorkLineItemAsync(RecipeJd rjd)
    {
        Object.SetActive(true);

        var iJd = JsonDataMgr.Instance.ItemMap[rjd.ID];

        RecipeJd = rjd;
        ItemJd = iJd;
        ItemID = ItemJd.ID;

        await SetIcon(AssetMgr.ICON_PATH, ItemJd.ID.ToString());
    }

    public void SetTime(int nowTime, PLandBuild workBuild)
    {
        if (RecipeJd == null) return;

        var count = (workBuild.ProduceTask.EndTime - workBuild.ProduceTask.BeginTime) / RecipeJd.Time;
        var allTime = nowTime - workBuild.ProduceTask.BeginTime;
        var needTime = workBuild.ProduceTask.EndTime - workBuild.ProduceTask.BeginTime;
        WorkDoneNum = (int)((float)(allTime >= needTime ? needTime : allTime) / (float)(needTime) * count);

        Count.text = $"{WorkDoneNum}/{count}";
    }

public void SetNull()
    {
        Object.SetActive(true);

        //var iJd = JsonDataMgr.Instance.ItemMap[rjd.ID];

        //RecipeJd = rjd;
        //ItemJd = iJd;
        //ItemID = ItemJd.ID;

        //SetIcon(AssetMgr.ICON_PATH, ItemJd.ID.ToString());

        //Count.text = _Count.ToString();

        //Name.text = ItemJd.Name;
    }
}
