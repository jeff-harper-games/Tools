using UnityEngine;
using UnityEngine.UI;

public class TestingLerp : MonoBehaviour
{
    public Image image;
    private CoroutineLerp lerp = new CoroutineLerp(); 

    // Start is called before the first frame update
    void Start()
    {
        lerp.marker += Marker;
        lerp.Begin(2.0f, this);
    }

    public void Marker(bool value)
    {
        Debug.Log(value ? "Start" : "End");

        if (!value)
            lerp.marker -= Marker;
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = lerp.Progress;
        image.color = Color.Lerp(Color.blue, Color.red, lerp.Progress);
    }
}
