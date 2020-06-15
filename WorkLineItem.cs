using Google.Protobuf;
using JsonData;
using Luna;
using System.Collections.Generic;
using System.Data;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorkLineItem : PoolUnit
{
    public int WorkCount;
    public PLandBuild WorkBuild;
    public bool IsWork;

    public Transform tr_noWork;
    public Transform tr_inWork;

    public UnityAction<BaseEventData> GetAction;
    public UnityAction<BaseEventData> ShowProduceAction;
    public UnityAction<BaseEventData> QuickenAction;

    public RecipeJd RecipeJd;

    private JButton _getBtn;
    private JButton _goProBtn;
    private JButton _produceBtn;
    private JButton _quickenBtn;

    private Text _workLineName;
    private Text _time;
    public RecipeItem NowWorkItem;

    public override void Init()
    {
        tr_noWork = Transform.Find("NoWork");
        tr_inWork = Transform.Find("OnWork");

        _workLineName = Transform.Find("tex_name").GetComponent<Text>(); 
        _time = tr_inWork.Find("tex_time").GetComponent<Text>();
        _getBtn = Transform.Find("btn_get").GetComponent<JButton>();
        _produceBtn = Transform.Find("btn_produce").GetComponent<JButton>();
        _quickenBtn = tr_inWork.Find("btn_quicken").GetComponent<JButton>();
        _goProBtn = tr_noWork.Find("btn_gopro").GetComponent<JButton>();

        _goProBtn.gameObject.name = Object.name;
        _getBtn.gameObject.name = Object.name;
        _produceBtn.gameObject.name = Object.name;
        _quickenBtn.gameObject.name = Object.name;

        NowWorkItem = new RecipeItem();
        NowWorkItem.Object = tr_inWork.Find("Item").gameObject;
        NowWorkItem.Init();

        _workLineName.text = $"生产线{int.Parse(Object.name) + 1}";
    }

    public async UniTask SetItem(PLandBuild pbuild)
    {
        Object.SetActive(true);

        WorkBuild = pbuild;

        IsWork = WorkBuild.ProduceTask != null && WorkBuild.ProduceTask.EndTime != 0;

        tr_noWork.gameObject.SetActive(!IsWork);
        tr_inWork.gameObject.SetActive(IsWork);

        if (IsWork)
        {
            RecipeJd = JsonDataMgr.Instance.RecipeMap[pbuild.ProduceTask.DictID];
            WorkCount = (WorkBuild.ProduceTask.EndTime - WorkBuild.ProduceTask.BeginTime) / RecipeJd.Time;
            await NowWorkItem.SetWorkLineItemAsync(RecipeJd);
        }
    }
    public void SetActions() 
    {
        _getBtn.AddEvent(EventTriggerType.PointerClick, GetAction);
        _produceBtn.AddEvent(EventTriggerType.PointerClick, ShowProduceAction);
        _goProBtn.AddEvent(EventTriggerType.PointerClick, ShowProduceAction);
        _quickenBtn.AddEvent(EventTriggerType.PointerClick, QuickenAction);
    }

    public void SetTime(int nowTime)
    {
        var time = WorkBuild.ProduceTask.EndTime - nowTime;

        if (time <= 0)
        {
            _time.text = $"订单已完成";
            NowWorkItem.SetTime((int)GameMgr.Instance.UnixTimestamp.TotalSeconds, WorkBuild);
            return;
        }

        var h = time / 3600;
        var m = (time - 3600 * h) / 60;
        var s = (time - 3600 * h - 60 * m);

        var showh = h >= 10 ? $"{h}" : "0" + h;
        var showm = m >= 10 ? $"{m}" : "0" + m;
        var shows = s >= 10 ? $"{s}" : "0" + s;

        _time.text = $"{showh}:{showm}:{shows}";
        NowWorkItem.SetTime((int)GameMgr.Instance.UnixTimestamp.TotalSeconds, WorkBuild);
    }

    public async UniTask RefeshWorkLineData()
    {
        KingdomDataMgr.Instance.BuildDataMap.TryGetValue(WorkBuild.InsID, out WorkBuild);

        IsWork = WorkBuild.ProduceTask != null && WorkBuild.ProduceTask.EndTime != 0;

        tr_noWork.gameObject.SetActive(!IsWork);
        tr_inWork.gameObject.SetActive(IsWork);

        if (IsWork)
        {
            RecipeJd = JsonDataMgr.Instance.RecipeMap[WorkBuild.ProduceTask.DictID];
            WorkCount = (WorkBuild.ProduceTask.EndTime - WorkBuild.ProduceTask.BeginTime) / RecipeJd.Time;
            await NowWorkItem.SetWorkLineItemAsync(RecipeJd);
        }
    }
}
