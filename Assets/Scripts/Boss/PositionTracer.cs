using UnityEngine;

public enum PositionChoiceType
{
    NearDistance,
    MidDistance,
    Outside
}

public class PositionTracer: MonoBehaviour
{
    private Transform rootTrans;

    [SerializeField]
    private Transform outsidePositionsRoot;
    [SerializeField]
    private Transform midDistancePositionsRoot;
    [SerializeField]
    private Transform nearDistancePositionsRoot;

    [Tooltip("随机选点的偏移范围")]
    [SerializeField]
    private float randomBias;
    // public******************************************************************************
    public Vector3 PickNearestPos(PositionChoiceType choiceType, bool needRandomize = true)
    {
        var targetTrans = ChoiceToTransform(choiceType);
        int nearestId = 0;
        var nearestDistance = Vector3.Distance(targetTrans.GetChild(0).transform.position, rootTrans.position);
        for (int i = 1; i < targetTrans.childCount; i++)
        {
            var dis = Vector3.Distance(targetTrans.GetChild(i).transform.position, rootTrans.position);
            if (dis < nearestDistance)
            {
                nearestId = i;
            }
        }
        if (needRandomize)
        {
            return RandomizePos(targetTrans.GetChild(nearestId).position);
        }
        return targetTrans.GetChild(nearestId).position;
    }

    public Vector3 PickFurtherestPos(PositionChoiceType choiceType, bool needRandomize = true)
    {
        var targetTrans = ChoiceToTransform(choiceType);
        int furtherestId = 0;
        var furtherestDistance = Vector3.Distance(targetTrans.GetChild(0).transform.position, rootTrans.position);
        for (int i = 1; i < targetTrans.childCount; i++)
        {
            var dis = Vector3.Distance(targetTrans.GetChild(i).transform.position, rootTrans.position);
            if (dis > furtherestDistance)
            {
                furtherestId = i;
            }
        }
        if (needRandomize)
        {
            return RandomizePos(targetTrans.GetChild(furtherestId).position);
        }
        return targetTrans.GetChild(furtherestId).position;
    }

    public Vector3 PickRandomPos(PositionChoiceType choiceType, bool needRandomize = true)
    {
        var targetTrans = ChoiceToTransform(choiceType);
        int randomId = Random.Range(0, targetTrans.childCount);
        if (needRandomize)
        {
            return RandomizePos(targetTrans.GetChild(randomId).position);
        }
        return targetTrans.GetChild(randomId).position;
    }

    public void Init(Transform rootTrans)
    {
        this.rootTrans = rootTrans;
    }
    // private******************************************************************************
    private Transform ChoiceToTransform(PositionChoiceType choiceType)
    {
        switch (choiceType)
        {
            case PositionChoiceType.NearDistance:
                return nearDistancePositionsRoot;
            case PositionChoiceType.MidDistance:
                return midDistancePositionsRoot;
            case PositionChoiceType.Outside:
                return outsidePositionsRoot;
            default:
                return midDistancePositionsRoot;
        }
    }

    private Vector3 RandomizePos(Vector3 origin)
    {
        var biasX = Random.Range(-randomBias, randomBias);
        var biasY = Random.Range(-randomBias, randomBias);
        var biasZ = Random.Range(-randomBias, randomBias);
        Vector3 randomizedPos = new Vector3(origin.x + biasX, origin.y + biasY, origin.z + biasZ);
        return randomizedPos;
    }
}
