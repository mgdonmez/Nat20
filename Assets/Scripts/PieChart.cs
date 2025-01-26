using UnityEngine;
using UnityEngine.UI;

public class PieChart : MonoBehaviour
{
    [SerializeField]
    GameObject[] slices;

    private void FixedUpdate()
    {
        SetStats();
    }

    public void SetStats()
    {
        slices[0].GetComponent<Image>().fillAmount = Manager.Instance.Population[(int)DemographicType.Square].Ratio;
        slices[1].GetComponent<Image>().fillAmount = Manager.Instance.Population[(int)DemographicType.Triangle].Ratio;
    }


}
